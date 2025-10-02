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

namespace MrPlagueRaces.Common.Races.Tabaxi
{
	public class Tabaxi : Race
	{
		public override void Load()
        {
			Description = "Nomadic and curious, Tabaxi create miniature wormholes to travel between lands.";
            DisplayName = "[c/00FFD9:Tabaxi]";
            ClothStyle = 4;
			CensorClothing = false;
			StarterShirt = true;
			StarterPants = true;
			HairColor = new Color(237, 208, 165);
			SkinColor = new Color(237, 208, 165);
			DetailColor = new Color(239, 119, 157);
            AuxilaryDetailColor1 = new Color(255, 247, 200);
            EyeColor = new Color(150, 255, 194);
			ShirtColor = new Color(180, 112, 101);
			UnderShirtColor = new Color(108, 74, 61);
			PantsColor = new Color(245, 213, 193);
			ShoeColor = new Color(180, 112, 101);
        }

        public override void PreRaceChange(Player player) // Called before the player's race is changed
        {
            var tabaxiPlayer = player.GetModPlayer<TabaxiPlayer>();
            tabaxiPlayer.phased = false;
            tabaxiPlayer.phaseChargeCounter = 0;
            tabaxiPlayer.phaseActiveCounter = 0;
        }

		public override void ResetEffects(Player player)
		{
			RegisterAbilityDescription(ModContent.GetInstance<TabaxiConfig>().tabaxiAbility1 && ModContent.GetInstance<TabaxiConfig>().tabaxiIntangibility != IntangibilityProgressionTiers.Disabled, $"[c/4DBF60:+] Hold Z to charge up an intangible dash.");
			RegisterAbilityDescription(ModContent.GetInstance<TabaxiConfig>().tabaxiAbility1 && ModContent.GetInstance<TabaxiConfig>().tabaxiIntangibility == IntangibilityProgressionTiers.Disabled, $"[c/4DBF60:+] Hold Z to charge up a dash.");
			RegisterAbilityDescription(ModContent.GetInstance<TabaxiConfig>().tabaxiAbility2, $"[c/4DBF60:+] Press X to set a rewind point, press C to return to it.");
            RegisterAbilityDescription(ModContent.GetInstance<TabaxiConfig>().tabaxiLight, $"[c/4DBF60:+] You are able to see a short distance while in the dark.");
            RegisterAbilityDescription(ModContent.GetInstance<TabaxiConfig>().tabaxiNoFallDmg, $"[c/4DBF60:+] You do not take fall damage.");

			var tabaxiPlayer = player.GetModPlayer<TabaxiPlayer>();
			RaceStatDictionary = ModContent.GetInstance<TabaxiConfig>().tabaxiStats;
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats)
			{
				/*player.moveSpeed += 0.15f;
				player.jumpSpeedBoost += 0.1f;
				player.pickSpeed -= 0.15f;
				player.GetCritChance(DamageClass.Generic) -= 15;
				player.endurance -= 0.1f;*/
				if (ModContent.GetInstance<TabaxiConfig>().tabaxiNoFallDmg)
				{
					player.noFallDmg = true;
				}
			}
            if (tabaxiPlayer.phaseActiveCounter > 0)
            {
                player.direction = mrPlagueRacesPlayer.mouseWorld.X >= player.Center.X ? 1 : -1;
				if (tabaxiPlayer.meetsIntangibilityRequirement(ModContent.GetInstance<TabaxiConfig>().tabaxiIntangibility))
				{
                    player.shimmering = true;
                    player.ClearBuff(BuffID.Shimmer);
                }
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
        }

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var tabaxiPlayer = player.GetModPlayer<TabaxiPlayer>();
			tabaxiPlayer.phased = false;
			tabaxiPlayer.phaseChargeCounter = 0;
			tabaxiPlayer.phaseActiveCounter = 0;
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var tabaxiPlayer = player.GetModPlayer<TabaxiPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
				{
					if (ModContent.GetInstance<TabaxiConfig>().tabaxiAbility1)
					{
						if (MrPlagueRaces.RaceAbilityKeybind1.Current && !player.HasBuff(BuffType<ParticleDeacceleration>()))
						{
							tabaxiPlayer.phased = true;
							if (tabaxiPlayer.phaseChargeCounter == 0)
							{
								SoundEngine.PlaySound(SoundID.Item162, player.Center);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.TabaxiDashChargeSound(-1, Main.myPlayer);
                                }
                            }
							if (tabaxiPlayer.phaseChargeCounter < 100)
							{
								tabaxiPlayer.phaseChargeCounter++;
							}
						}
						else
						{
							tabaxiPlayer.phased = false;
						}
						if (MrPlagueRaces.RaceAbilityKeybind1.JustReleased && !player.HasBuff(BuffType<ParticleDeacceleration>()))
                        {
                            tabaxiPlayer.lastUnobstructedPosition = player.position;
                            player.AddBuff(BuffType<ParticleDeacceleration>(), tabaxiPlayer.phaseChargeCounter * 2);
							SoundEngine.PlaySound(SoundID.Item163, player.Center);
                            if (player.whoAmI == Main.myPlayer)
                            {
                                mrPlagueRacesPlayer.TabaxiDashExecuteSound(-1, Main.myPlayer);
                            }
                        }
					}
					if (ModContent.GetInstance<TabaxiConfig>().tabaxiAbility2)
					{
						if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed)
						{
							if (player.position != tabaxiPlayer.TabaxiSpawn)
							{
								Main.NewText("Rewind point set!", player.eyeColor.R, player.eyeColor.G, player.eyeColor.B);
								for (int i = 0; i < 25; i++)
								{
									int dust = Dust.NewDust(player.position, player.width, player.height, 263);
                                    Main.dust[dust].color = player.eyeColor;
                                    Main.dust[dust].noGravity = true;
									Dust obj6 = Main.dust[dust];
									obj6.velocity *= new Vector2(0.2f, 0.2f) + (player.velocity * 0.1f);
                                }
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.TabaxiTeleportDust(-1, Main.myPlayer);
                                }
                                tabaxiPlayer.TabaxiSpawn = player.position;
								SoundEngine.PlaySound(SoundID.Item105, player.Center);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.TabaxiSetTeleportSound(-1, Main.myPlayer);
                                }
                            }
							else
							{
								Main.NewText("Rewind point removed!", player.eyeColor.R, player.eyeColor.G, player.eyeColor.B);
                                tabaxiPlayer.TabaxiSpawn = new Vector2(-1, -1);
							}
						}
						if (MrPlagueRaces.RaceAbilityKeybind3.JustPressed)
						{
							if (tabaxiPlayer.TabaxiSpawn != new Vector2(-1, -1) && !player.HasBuff(BuffType<Rematerializing>()))
                            {
                                for (int i = 0; i < 25; i++)
                                {
                                    int dust = Dust.NewDust(player.position, player.width, player.height, 263);
                                    Main.dust[dust].color = player.eyeColor;
                                    Main.dust[dust].noGravity = true;
                                    Dust obj6 = Main.dust[dust];
                                    obj6.velocity *= new Vector2(0.2f, 0.2f) + (player.velocity * 0.1f);
                                }
                                player.Teleport(tabaxiPlayer.TabaxiSpawn, -1);
                                for (int i = 0; i < 25; i++)
                                {
                                    int dust = Dust.NewDust(player.position, player.width, player.height, 263);
                                    Main.dust[dust].color = player.eyeColor;
                                    Main.dust[dust].noGravity = true;
                                    Dust obj6 = Main.dust[dust];
                                    obj6.velocity *= new Vector2(0.2f, 0.2f) + (player.velocity * 0.1f);
                                }
                                player.AddBuff(BuffType<Rematerializing>(), 180);
								SoundEngine.PlaySound(SoundID.Item165, player.Center);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.TabaxiUseTeleportSound(-1, Main.myPlayer);
                                }
                            }
							else if (!player.HasBuff(BuffType<Rematerializing>()))
							{
								Main.NewText("No rewind point found!", player.eyeColor.R, player.eyeColor.G, player.eyeColor.B);
                            }
						}
					}
					if (tabaxiPlayer.phaseActiveCounter > 0) {
                        if (player.shimmering && !tabaxiPlayer.isInBlocks())
                        {
                            tabaxiPlayer.lastUnobstructedPosition = player.position;
                        }
                        if (player.controlUseItem)
                        {
                            player.controlUseItem = false;
                        }
                        Vector2 velocity = ((Vector2.Normalize(mrPlagueRacesPlayer.mouseWorld - player.Center) * tabaxiPlayer.phaseActiveCounter * 5f) * (tabaxiPlayer.meetsIntangibilityRequirement(ModContent.GetInstance<TabaxiConfig>().tabaxiIntangibility) ? 1f : 0.4f)) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<TabaxiConfig>().tabaxiDashVelocity]);
						player.maxFallSpeed = 1000000f;
						player.gravity = 0f;
                        player.velocity = velocity;
						if (player.controlUp) {
							player.controlUp = false;
						}
						if (player.controlDown) {
							player.controlDown = false;
						}
						if (player.controlJump) {
							player.controlJump = false;
						}
						if (player.controlLeft) {
							player.controlLeft = false;
						}
						if (player.controlRight) {
							player.controlRight = false;
                        }
                        player.fallStart = (int)(player.position.Y / 16f);
                    }
				}
			}
		}

		public override void PreUpdate(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var tabaxiPlayer = player.GetModPlayer<TabaxiPlayer>();
            if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead) {
					if (!tabaxiPlayer.phased && tabaxiPlayer.phaseChargeCounter > 0) {
						Vector2 offset = mrPlagueRacesPlayer.mouseWorld - player.Center;
						tabaxiPlayer.phaseActiveCounter = tabaxiPlayer.phaseChargeCounter / 3;
						tabaxiPlayer.phaseChargeCounter = 0;
                    }
                    if (tabaxiPlayer.phaseActiveCounter > 0)
                    {
                        tabaxiPlayer.phaseActiveCounter--;
                        if (tabaxiPlayer.phaseActiveCounter == 0)
                        {
                            if (tabaxiPlayer.isInBlocks())
                            {
                                if (ModContent.GetInstance<TabaxiConfig>().tabaxiStuckPrevention == StuckPreventionTiers.Low || ModContent.GetInstance<TabaxiConfig>().tabaxiStuckPrevention == StuckPreventionTiers.High)
                                {
                                    tabaxiPlayer.SeekNearestAirPocket();
                                    player.velocity *= 0f;
                                }
                                if (ModContent.GetInstance<TabaxiConfig>().tabaxiStuckPrevention == StuckPreventionTiers.High && tabaxiPlayer.isInBlocks())
                                {
                                    Vector2 tileAlignedPosition = new Vector2(tabaxiPlayer.lastUnobstructedPosition.X / 16, tabaxiPlayer.lastUnobstructedPosition.Y / 16) * 16;
                                    player.Teleport(new Vector2(tileAlignedPosition.X, tileAlignedPosition.Y - 2), -1);
                                    player.velocity *= 0f;
                                }
                                for (int i = 0; i < 25; i++)
                                {
                                    int dust = Dust.NewDust(player.position, player.width, player.height, 263);
                                    Main.dust[dust].color = player.eyeColor;
                                    Main.dust[dust].noGravity = true;
                                    Dust obj6 = Main.dust[dust];
                                    obj6.velocity *= new Vector2(0.2f, 0.2f) + (player.velocity * 0.1f);
                                }
                            }
                            for (int i = 0; i < 25; i++)
                            {
                                int dust = Dust.NewDust(player.position, player.width, player.height, 263);
                                Main.dust[dust].color = player.eyeColor;
                                Main.dust[dust].noGravity = true;
                                Dust obj6 = Main.dust[dust];
                                obj6.velocity *= new Vector2(0.2f, 0.2f) + (player.velocity * 0.1f);
                            }
                            SoundEngine.PlaySound(SoundID.Item165, player.Center);
                        }
                    }
                    if (ModContent.GetInstance<TabaxiConfig>().tabaxiLight)
                    {
                        Lighting.AddLight(player.Center, player.eyeColor.ToVector3());
                    }
                    if (tabaxiPlayer.isInTempleEarly() && player.HasBuff(BuffType<ParticleDeacceleration>()) && ModContent.GetInstance<TabaxiConfig>().tabaxiNoEarlyTemple)
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
				if (tabaxiPlayer.phaseActiveCounter > 0)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.Center.X + Main.rand.Next(10) - Main.rand.Next(10), player.Center.Y + Main.rand.Next(10) - Main.rand.Next(10)), 0, 0, 263);
                        Main.dust[dust].color = player.eyeColor;
                        Main.dust[dust].noGravity = true;
						Dust obj6 = Main.dust[dust];
						obj6.velocity *= new Vector2(0.2f,0.2f) + (player.velocity * 0.1f);
					}
				}
				if (tabaxiPlayer.phased) {
					for (int i = 0; i < tabaxiPlayer.phaseChargeCounter / 2; i++)
					{
						if (Main.rand.Next(40) == 1) {
							int dust = Dust.NewDust(player.position, player.width, player.height, 263);
                            Main.dust[dust].color = player.eyeColor;
                            Main.dust[dust].noGravity = true;
							Dust obj6 = Main.dust[dust];
							obj6.velocity *= new Vector2(0.2f, 0.2f) + (player.velocity * 0.1f);
						}
					}
				}
			}
        }

        public override bool FreeDodge(Player player, Player.HurtInfo info)
        {
            var tabaxiPlayer = player.GetModPlayer<TabaxiPlayer>();
            if (tabaxiPlayer.phaseActiveCounter > 0)
            {
                return true;
            }
            return false;
        }

        public override void PostUpdate(Player player)
        {
            var tabaxiPlayer = player.GetModPlayer<TabaxiPlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    tabaxiPlayer.SyncPlayer(-1, Main.myPlayer, false);
                }

                var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
                mrPlagueRacesPlayer.SyncRotationsAndOffsets(-1, Main.myPlayer);
            }
        }
    }

	public class TabaxiPlayer : ModPlayer
	{
		public Vector2 TabaxiSpawn = new Vector2(-1, -1);
		public bool phased;
		public int phaseChargeCounter;
		public int phaseActiveCounter;
        public Vector2 lastUnobstructedPosition = new Vector2(-1, -1);

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.TabaxiSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(TabaxiSpawn.X);
			packet.Write(TabaxiSpawn.Y);
			packet.Write(phased);
			packet.Write(phaseChargeCounter);
			packet.Write(phaseActiveCounter);
            packet.Write(lastUnobstructedPosition.X);
            packet.Write(lastUnobstructedPosition.Y);

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
                        if (!isPositionObstructed(currentPosition))
                        {
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

    public class TabaxiConfig : ModConfig
    {
        public static TabaxiConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Tabaxi")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> tabaxiStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%
            [RacialStatType.allCrit] = RacialStatPercentageModifier.n15,
            [RacialStatType.moveSpeed] = RacialStatPercentageModifier.p15,
            [RacialStatType.jumpSpeedBoost] = RacialStatPercentageModifier.p10,
            [RacialStatType.pickSpeed] = RacialStatPercentageModifier.p15,
            [RacialStatType.endurance] = RacialStatPercentageModifier.n10
        };
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool tabaxiAbility1;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool tabaxiAbility2;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier tabaxiDashVelocity;
        [DefaultValue(IntangibilityProgressionTiers.Enabled)]
        [BackgroundColor(110, 141, 255)]
        public IntangibilityProgressionTiers tabaxiIntangibility;
        [DefaultValue(StuckPreventionTiers.High)]
        [BackgroundColor(110, 141, 255)]
        public StuckPreventionTiers tabaxiStuckPrevention;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool tabaxiNoEarlyTemple;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool tabaxiNoFallDmg;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool tabaxiLight;
    }
}