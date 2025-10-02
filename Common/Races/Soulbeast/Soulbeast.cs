using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Config;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Content.Projectiles;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.Races.Soulbeast
{
	public class Soulbeast : Race
	{
		public override void Load()
        {
			Description = "Made from an amalgamation of souls, Soulbeasts can shred through the fabric of space.";
			DisplayName = "[c/00AAFF:Soulbeast]";
			CensorClothing = false;
			HairColor = new Color(57, 59, 70);
			SkinColor = new Color(57, 59, 70);
			DetailColor = new Color(255, 255, 255);
			EyeColor = new Color(118, 194, 255);
        }

        public override void PreRaceChange(Player player) // Called before the player's race is changed
        {
            var soulbeastPlayer = player.GetModPlayer<SoulbeastPlayer>();
            soulbeastPlayer.rending = false;
            soulbeastPlayer.rendDelay = 0;
            soulbeastPlayer.rendTimer = 120 + (player.statLifeMax2 / 10);

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && (projectile.type == ProjectileType<RendClaw>() || projectile.type == ProjectileType<SoulClaw>()) && projectile.owner == player.whoAmI)
                    projectile.Kill();
            }
        }

		public override void ResetEffects(Player player)
		{
			RegisterAbilityDescription(ModContent.GetInstance<SoulbeastConfig>().soulbeastAbility1 && ModContent.GetInstance<SoulbeastConfig>().soulbeastIntangibility != IntangibilityProgressionTiers.Disabled, $"[c/4DBF60:+] Hold Z to tear through reality, turning you into an intangible missile. Maximum duration scales with max health.");
			RegisterAbilityDescription(ModContent.GetInstance<SoulbeastConfig>().soulbeastAbility1 && ModContent.GetInstance<SoulbeastConfig>().soulbeastIntangibility == IntangibilityProgressionTiers.Disabled, $"[c/4DBF60:+] Hold Z to tear through reality, turning you into a missile. Maximum duration scales with max health.");
			RegisterAbilityDescription(ModContent.GetInstance<SoulbeastConfig>().soulbeastAbility2, $"[c/4DBF60:+] Hold X to debuff enemies with your claws, weakening their defense and siphoning their health to you. Damage and healing scale with max health.");
            RegisterAbilityDescription(ModContent.GetInstance<SoulbeastConfig>().soulbeastHealingPotionDenial, $"[c/FF3640:-] You cannot use healing potions.");
            RegisterAbilityDescription(ModContent.GetInstance<SoulbeastConfig>().soulbeastNoRegen, $"[c/FF3640:-] You do not passively regenerate health.");

            RaceStatDictionary = ModContent.GetInstance<SoulbeastConfig>().soulbeastStats;
            var soulbeastPlayer = player.GetModPlayer<SoulbeastPlayer>();
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats)
			{
				/*player.moveSpeed += 0.25f;
				player.GetDamage(DamageClass.Generic) += 0.15f;
				player.endurance -= 0.1f;*/
			}
            if (soulbeastPlayer.rending)
			{
				if (soulbeastPlayer.meetsIntangibilityRequirement(ModContent.GetInstance<SoulbeastConfig>().soulbeastIntangibility))
                {
                    player.shimmering = true;
                    player.ClearBuff(BuffID.Shimmer);
                }
                soulbeastPlayer.direction = (mrPlagueRacesPlayer.mouseWorld.X >= player.Center.X ? 1 : -1);
                player.ChangeDir(soulbeastPlayer.direction);
                if (player.shimmerTransparency < 1)
                {
                    player.shimmerTransparency += 0.25f;
                }
            }
            else if (!player.HasBuff(BuffID.Shimmer))
            {
                if (player.shimmerTransparency > 0)
                {
                    player.shimmerTransparency -= 0.25f;
                }
            }
            soulbeastPlayer.SoulSiphon = false;
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var soulbeastPlayer = player.GetModPlayer<SoulbeastPlayer>();
			soulbeastPlayer.rending = false;
			soulbeastPlayer.rendDelay = 0;
			soulbeastPlayer.rendTimer = 120 + (player.statLifeMax2 / 10);
        }

        public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var soulbeastPlayer = player.GetModPlayer<SoulbeastPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
                {
					if (ModContent.GetInstance<SoulbeastConfig>().soulbeastAbility1)
					{
						if (MrPlagueRaces.RaceAbilityKeybind1.Current && soulbeastPlayer.rendTimer > 0 && !player.HasBuff(BuffType<MolecularRecoil>()))
						{
							soulbeastPlayer.rendDelay++;
							if (soulbeastPlayer.rendDelay == 1)
							{
								SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost, player.Center);
								SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, player.Center);
                                Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, 0, 0, ProjectileType<RendClaw>(), (int)((1f + (player.ConsumedLifeFruit * 0.075f)) * (2 * player.statLifeMax2 / 30)) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<SoulbeastConfig>().soulbeastRendDamage]), 1, player.whoAmI);
							}
							if (soulbeastPlayer.rendDelay == 9)
                            {
                                soulbeastPlayer.lastUnobstructedPosition = player.position;
                                SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, player.Center);
								SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, player.Center);
								SoundEngine.PlaySound(SoundID.Zombie104, player.Center);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.SoulbeastRendSound(-1, Main.myPlayer);
                                }
                                for (int i = 0; i < 20; i++)
								{
									int dust = Dust.NewDust(player.position, player.width, player.height, 264);
									Main.dust[dust].color = player.eyeColor;
									Main.dust[dust].noGravity = true;
									Main.dust[dust].velocity *= 5f;
									dust = Dust.NewDust(player.position, player.width, player.height, 264);
									Main.dust[dust].color = player.eyeColor;
									Main.dust[dust].noGravity = true;
									Main.dust[dust].velocity *= 4f;
                                }
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.SoulbeastRendDust(-1, Main.myPlayer);
                                }
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.SoulbeastRendDust(-1, Main.myPlayer);
                                }
                            }
							if (soulbeastPlayer.rendDelay >= 10)
                            {
                                Vector2 velocity = (Vector2.Normalize(mrPlagueRacesPlayer.mouseWorld - player.Center) * 35f) * (soulbeastPlayer.meetsIntangibilityRequirement(ModContent.GetInstance<SoulbeastConfig>().soulbeastIntangibility) ? 1f : 0.4f);
								player.velocity = velocity * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<SoulbeastConfig>().soulbeastRendVelocity]);
                                player.maxFallSpeed = 1000000f;
                                player.gravity = 0f;
                                player.controlUp = false;
								player.controlLeft = false;
								player.controlDown = false;
								player.controlRight = false;
								player.controlJump = false;
								soulbeastPlayer.rending = true;
								if (player.controlUseItem) {
									player.controlUseItem = false;
								}
								player.fallStart = (int)(player.position.Y / 16f);
							}
						}
						else
						{
							if (soulbeastPlayer.rendDelay >= 10)
							{
								for (int i = 0; i < 10; i++)
								{
									int dust = Dust.NewDust(player.position, player.width, player.height, 264);
									Main.dust[dust].color = player.eyeColor;
									Main.dust[dust].noGravity = true;
									Main.dust[dust].velocity *= 5f;
									dust = Dust.NewDust(player.position, player.width, player.height, 264);
									Main.dust[dust].color = player.eyeColor;
									Main.dust[dust].noGravity = true;
									Main.dust[dust].velocity *= 4f;
								}
                                player.velocity *= 0.45f;
                                player.AddBuff(BuffType<MolecularRecoil>(), 360 - soulbeastPlayer.rendTimer);
                                if (soulbeastPlayer.isInBlocks())
                                {
                                    if (ModContent.GetInstance<SoulbeastConfig>().soulbeastStuckPrevention == StuckPreventionTiers.Low || ModContent.GetInstance<SoulbeastConfig>().soulbeastStuckPrevention == StuckPreventionTiers.High)
                                    {
                                        soulbeastPlayer.SeekNearestAirPocket();
                                        player.velocity *= 0f;
                                    }
                                    if (ModContent.GetInstance<SoulbeastConfig>().soulbeastStuckPrevention == StuckPreventionTiers.High && soulbeastPlayer.isInBlocks())
                                    {
                                        Vector2 tileAlignedPosition = new Vector2(soulbeastPlayer.lastUnobstructedPosition.X / 16, soulbeastPlayer.lastUnobstructedPosition.Y / 16) * 16;
                                        player.Teleport(new Vector2(tileAlignedPosition.X, tileAlignedPosition.Y - 2), -1);
                                        player.velocity *= 0f;
                                    }
                                    for (int i = 0; i < 10; i++)
                                    {
                                        int dust = Dust.NewDust(player.position, player.width, player.height, 264);
                                        Main.dust[dust].color = player.eyeColor;
                                        Main.dust[dust].noGravity = true;
                                        Main.dust[dust].velocity *= 5f;
                                        dust = Dust.NewDust(player.position, player.width, player.height, 264);
                                        Main.dust[dust].color = player.eyeColor;
                                        Main.dust[dust].noGravity = true;
                                        Main.dust[dust].velocity *= 4f;
                                    }
                                    if (player.whoAmI == Main.myPlayer)
                                    {
                                        mrPlagueRacesPlayer.SoulbeastRendDust(-1, Main.myPlayer);
                                    }
                                }
                                SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, player.Center);
								SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, player.Center);
								SoundEngine.PlaySound(SoundID.Zombie103, player.Center);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.SoulbeastRendExitSound(-1, Main.myPlayer);
                                }
                            }
                            soulbeastPlayer.rending = false;
							soulbeastPlayer.rendDelay = 0;
							soulbeastPlayer.rendTimer = (120 + (player.statLifeMax2 / 10)) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<SoulbeastConfig>().soulbeastRendDuration]);
						}
                    }
					if (ModContent.GetInstance<SoulbeastConfig>().soulbeastAbility2)
					{
						if (MrPlagueRaces.RaceAbilityKeybind2.Current && !MrPlagueRaces.RaceAbilityKeybind1.Current)
						{
							if (player.ownedProjectileCounts[ProjectileType<SoulClaw>()] == 0)
							{
								Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, 0, 0, ProjectileType<SoulClaw>(), (int)((1f + (player.ConsumedLifeFruit * 0.075f)) * (2 * player.statLifeMax2 / 30)) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<SoulbeastConfig>().soulbeastShredDamage]), 1, player.whoAmI);
							}
						}
					}
				}
			}
		}

		public override void PreUpdate(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var soulbeastPlayer = player.GetModPlayer<SoulbeastPlayer>();
            if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
                {
                    if (soulbeastPlayer.rending)
                    {
                        if (player.shimmering && !soulbeastPlayer.isInBlocks())
                        {
                            soulbeastPlayer.lastUnobstructedPosition = player.position;
                        }
                        for (int i = 0; i < 25; i++)
						{
							int dust = Dust.NewDust(player.Center, 0, 0, 264);
							Main.dust[dust].color = player.eyeColor;
							Main.dust[dust].noGravity = true;
						}
						if (soulbeastPlayer.rendTimer > 0) {
							soulbeastPlayer.rendTimer--;
						}
					}
				}
				if (soulbeastPlayer.isInTempleEarly() && player.HasBuff(BuffType<MolecularRecoil>()) && ModContent.GetInstance<SoulbeastConfig>().soulbeastNoEarlyTemple)
                {
                    player.statLife = 0;
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " tried to phase through impenetrable material."), 10.0, 0, false);
                            break;
                        case 1:
                            player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " still needs to defeat Plantera."), 10.0, 0, false);
                            break;
                        default:
                            player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " left organs behind in the temple wall."), 10.0, 0, false);
                            break;
                    }
                }
            }
        }

        public override void UpdateBadLifeRegen(Player player)
        {
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            var soulbeastPlayer = player.GetModPlayer<SoulbeastPlayer>();
            if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats && ModContent.GetInstance<SoulbeastConfig>().soulbeastNoRegen)
            {
                if (soulbeastPlayer.SoulSiphon)
                {
                    player.lifeRegen = (player.statLifeMax2 / 8) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<SoulbeastConfig>().soulbeastShredHealing]);
                    player.lifeRegenTime = 900;
                }
                else
                {
                    if (player.lifeRegen > 0)
                        player.lifeRegen = 0;
                    player.lifeRegenTime = 0;
                }
            }
        }

        public override bool CanUseItem(Player player, Item item)
        {
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats && ModContent.GetInstance<SoulbeastConfig>().soulbeastHealingPotionDenial)
            {
                if (item.healLife > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public override bool FreeDodge(Player player, Player.HurtInfo info)
        {
            var soulbeastPlayer = player.GetModPlayer<SoulbeastPlayer>();
            if (soulbeastPlayer.rending)
            {
                return true;
            }
            return false;
        }

        public override void PostUpdate(Player player)
        {
            var soulbeastPlayer = player.GetModPlayer<SoulbeastPlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    soulbeastPlayer.SyncPlayer(-1, Main.myPlayer, false);
                }

                var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
                mrPlagueRacesPlayer.SyncRotationsAndOffsets(-1, Main.myPlayer);
            }
        }
    }

	public class SoulbeastPlayer : ModPlayer
	{
		public bool rending = false;
		public int rendDelay;
		public int rendTimer = (120 + (Main.LocalPlayer.statLifeMax2 / 10)) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<SoulbeastConfig>().soulbeastRendDuration]);
        public Vector2 lastUnobstructedPosition = new Vector2(-1, -1);
        public bool SoulSiphon = false;
        public int direction = 1;

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.SoulbeastSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(rending);
			packet.Write(rendDelay);
			packet.Write(rendTimer);
            packet.Write(lastUnobstructedPosition.X);
            packet.Write(lastUnobstructedPosition.Y);
            packet.Write(direction);

            packet.Send(toWho, fromWho);
        }

        public bool meetsIntangibilityRequirement(IntangibilityProgressionTiers intangibilityCriteria)
        {
            if (intangibilityCriteria == IntangibilityProgressionTiers.Enabled)
            {
                return true;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostEOC)
            {
                return NPC.downedBoss1;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostEvilBoss)
            {
                return NPC.downedBoss2;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostSkeletron)
            {
                return NPC.downedBoss3;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.Hardmode)
            {
                return Main.hardMode;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostMechBosses)
            {
                return NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostPlantera)
            {
                return NPC.downedPlantBoss;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostGolem)
            {
                return NPC.downedGolemBoss;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostMoonLord)
            {
                return NPC.downedMoonlord;
            }
            else
            {
                return false;
            }
        }

        public bool isInTempleEarly()
		{
            Tile[] wallTiles = new Tile[6];
            Point playerTilePoint = (Main.LocalPlayer.position / 16).ToPoint();
            wallTiles[0] = Framing.GetTileSafely(playerTilePoint.X, playerTilePoint.Y);
            wallTiles[1] = Framing.GetTileSafely(playerTilePoint.X, playerTilePoint.Y + 1);
            wallTiles[2] = Framing.GetTileSafely(playerTilePoint.X, playerTilePoint.Y + 2);
            wallTiles[3] = Framing.GetTileSafely(playerTilePoint.X + 1, playerTilePoint.Y);
            wallTiles[4] = Framing.GetTileSafely(playerTilePoint.X + 1, playerTilePoint.Y + 1);
            wallTiles[5] = Framing.GetTileSafely(playerTilePoint.X + 1, playerTilePoint.Y + 2);
            bool behindTempleWall = false;
            foreach (var tile in wallTiles)
            {
                if (tile.WallType == 87)
                {
                    behindTempleWall = true;
                    break;
                }
            }
			return behindTempleWall && !NPC.downedPlantBoss;
        }

        public bool isInBlocks()
        {
            int inBlocks = 0;
            Vector2 playerLocation = new Vector2(Player.position.X / 16, Player.position.Y / 16);
            for (int i = 0; i < 3; i++)
            {
                Tile myTile1 = Main.tile[(int)playerLocation.X + 1, (int)playerLocation.Y];
                Tile myTile2 = Main.tile[(int)playerLocation.X, (int)playerLocation.Y];
                if (myTile1 != null && Main.tileSolid[myTile1.TileType] && myTile1.HasUnactuatedTile)
                {
                    inBlocks++;
                }
                if (myTile2 != null && Main.tileSolid[myTile2.TileType] && myTile2.HasUnactuatedTile)
                {
                    inBlocks++;
                }
                if (playerLocation.Y > 0)
                {
                    playerLocation.Y -= 1;
                }
            }
            return inBlocks > 0;
        }

        public bool isPositionObstructed(Vector2 checkPosition)
        {
            int inBlocks = 0;
            Vector2 playerLocation = checkPosition;
            for (int i = 0; i < 3; i++)
            {
                Tile myTile1 = Main.tile[(int)playerLocation.X + 1, (int)playerLocation.Y];
                Tile myTile2 = Main.tile[(int)playerLocation.X, (int)playerLocation.Y];
                if (myTile1 != null && Main.tileSolid[myTile1.TileType] && myTile1.HasUnactuatedTile)
                {
                    inBlocks++;
                }
                if (myTile2 != null && Main.tileSolid[myTile2.TileType] && myTile2.HasUnactuatedTile)
                {
                    inBlocks++;
                }
                if (playerLocation.Y > 0)
                {
                    playerLocation.Y -= 1;
                }
            }
            return inBlocks > 0;
        }

        public void SeekNearestAirPocket()
        {
            Vector2 playerLocation = new Vector2(Player.position.X / 16, Player.position.Y / 16);
            int SeekRange = 50;
            Vector2 closestPosition = new Vector2(playerLocation.X - SeekRange, playerLocation.Y - SeekRange);
            for (int i = -SeekRange; i < SeekRange; i++)
            {
                for (int j = -SeekRange; j < SeekRange; j++)
                {
                    Vector2 currentPosition = new Vector2((int)playerLocation.X + i, (int)playerLocation.Y + j);
                    if (Vector2.Distance(currentPosition, playerLocation) < Vector2.Distance(closestPosition, playerLocation))
                    {
                        if (!isPositionObstructed(currentPosition)) {
                            closestPosition = currentPosition;
                        }
                    }
                }
            }
            if (!isPositionObstructed(closestPosition))
            {
                Player.Teleport(new Vector2(closestPosition.X * 16, (closestPosition.Y - 2) * 16), -1);
            }
        }
    }

    public enum StuckPreventionTiers
    {
        Off,
        Low,
        High
    }

    public enum IntangibilityProgressionTiers
    {
        Enabled,
        PostEOC,
        PostEvilBoss,
        PostSkeletron,
        Hardmode,
        PostMechBosses,
        PostPlantera,
        PostGolem,
        PostMoonLord,
        Disabled
    }

    public class SoulbeastConfig : ModConfig
    {
        public static SoulbeastConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Soulbeast")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> soulbeastStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%,
            [RacialStatType.statLifeMax2] = RacialStatPercentageModifier.n25,
            [RacialStatType.allDamage] = RacialStatPercentageModifier.p15,
            [RacialStatType.moveSpeed] = RacialStatPercentageModifier.p25,
            [RacialStatType.endurance] = RacialStatPercentageModifier.n25
        };
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool soulbeastAbility1;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool soulbeastAbility2;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier soulbeastRendDamage;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier soulbeastShredDamage;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier soulbeastRendVelocity;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier soulbeastRendDuration;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier soulbeastShredHealing;
        [DefaultValue(IntangibilityProgressionTiers.Enabled)]
        [BackgroundColor(110, 141, 255)]
        public IntangibilityProgressionTiers soulbeastIntangibility;
        [DefaultValue(StuckPreventionTiers.High)]
        [BackgroundColor(110, 141, 255)]
        public StuckPreventionTiers soulbeastStuckPrevention;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool soulbeastNoEarlyTemple;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool soulbeastHealingPotionDenial;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool soulbeastNoRegen;
    }
}