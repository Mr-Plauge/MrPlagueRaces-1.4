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
using MrPlagueRaces.Content.Mounts;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.Races.Vampire
{
	public class Vampire : Race
	{
		public override void Load()
        {
			Description = "Naturally gifted with a form of soul magic, Vampires can morph into a bat-like state.";
            DisplayName = "[c/FF0033:Vampire]";
            //ClothStyle = 3; 
			CensorClothing = false;
            //StarterShirt = true;
            HairColor = new Color(91, 86, 94);
			SkinColor = new Color(91, 86, 94);
			DetailColor = new Color(175, 165, 140);
			EyeColor = new Color(255, 81, 81);
            //ShirtColor = new Color(190, 74, 122);
            //UnderShirtColor = new Color(216, 206, 183);
        }

        public override void PreRaceChange(Player player) // Called before the player's race is changed
        {
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            if (player.mount.Type == MountType<StealthBat>())
            {
                player.fallStart = (int)(player.position.Y / 16f);
                player.mount.Dismount(player);
                if (player.whoAmI == Main.myPlayer)
                {
                    mrPlagueRacesPlayer.SyncDismount(-1, Main.myPlayer);
                }
            }
        }

        public override void ResetEffects(Player player)
        {
            RegisterAbilityDescription(ModContent.GetInstance<VampireConfig>().vampireAbility1, $"[c/4DBF60:+] Press Z to transform into a bat. You enter stealth after a few seconds, increasing your damage and making enemies ignore you.");
            RegisterAbilityDescription(ModContent.GetInstance<VampireConfig>().vampireAbility2, $"[c/4DBF60:+] Press X while in batform to release your tongue, which latches onto enemies and drains their health.");
            RegisterAbilityDescription(ModContent.GetInstance<VampireConfig>().vampireLight == LightTiers.BatOnly, $"[c/4DBF60:+] In batform, you are able to see a short distance while in the dark.");
            RegisterAbilityDescription(ModContent.GetInstance<VampireConfig>().vampireLight == LightTiers.NotBat, $"[c/4DBF60:+] Outside of batform, you are able to see a short distance while in the dark.");
            RegisterAbilityDescription(ModContent.GetInstance<VampireConfig>().vampireLight == LightTiers.On, $"[c/4DBF60:+] You are able to see a short distance while in the dark.");
            RegisterAbilityDescription(ModContent.GetInstance<VampireConfig>().vampireHealingPotionDenial, $"[c/FF3640:-] You cannot use healing potions.");
            RegisterAbilityDescription(ModContent.GetInstance<VampireConfig>().vampireNoRegen, $"[c/FF3640:-] You do not passively regenerate health.");
            RegisterAbilityDescription(ModContent.GetInstance<VampireConfig>().vampireSunlightWeakness, $"[c/FF3640:-] Getting attacked in sunlight burns you.");

            RaceStatDictionary = ModContent.GetInstance<VampireConfig>().vampireStats;
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
                /*player.moveSpeed += 0.1f;
				if (player.mount.Type == MountType<StealthBat>()) {
					player.endurance -= 0.5f;
				}
				else {
					player.endurance -= 0.15f;
				}
                player.statLifeMax2 -= (player.statLifeMax2 / 4);*/
			}
        }

        public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            if (player.mount.Type == MountType<StealthBat>())
            {
                player.fallStart = (int)(player.position.Y / 16f);
                player.mount.Dismount(player);
                if (player.whoAmI == Main.myPlayer)
                {
                    mrPlagueRacesPlayer.SyncDismount(-1, Main.myPlayer);
                }
            }
        }

        public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var vampirePlayer = player.GetModPlayer<VampirePlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
                {
					if (ModContent.GetInstance<VampireConfig>().vampireAbility1)
					{
						if (MrPlagueRaces.RaceAbilityKeybind1.JustPressed)
						{
							if (player.mount.Type != MountType<StealthBat>())
                            {
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.VampireTransformDust(-1, Main.myPlayer);
                                }
                                player.mount.SetMount(MountType<StealthBat>(), player, false);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.SyncBat(-1, Main.myPlayer);
                                }
                                SoundEngine.PlaySound(SoundID.AbigailUpgrade, player.Center);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.VampireTransformSound(-1, Main.myPlayer);
                                }
                                int num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(player.position.X, player.position.Y - 10f), player.velocity, 99);
								Main.gore[num].velocity *= 0.3f;
								num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(player.position.X, player.position.Y + (float)(player.height / 2) - 10f), player.velocity, 99);
								Main.gore[num].velocity *= 0.3f;
								num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(player.position.X, player.position.Y + (float)player.height - 10f), player.velocity, 99);
								Main.gore[num].velocity *= 0.3f;
                            }
							else
                            {
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.VampireTransformDust(-1, Main.myPlayer);
                                }
                                player.fallStart = (int)(player.position.Y / 16f);
                                player.mount.Dismount(player);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.SyncDismount(-1, Main.myPlayer);
                                }
                                SoundEngine.PlaySound(SoundID.AbigailAttack, player.Center);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.VampireExitTransformationSound(-1, Main.myPlayer);
                                }
                                int num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(player.position.X, player.position.Y - 10f), player.velocity, 99);
								Main.gore[num].velocity *= 0.3f;
								num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(player.position.X, player.position.Y + (float)(player.height / 2) - 10f), player.velocity, 99);
								Main.gore[num].velocity *= 0.3f;
								num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(player.position.X, player.position.Y + (float)player.height - 10f), player.velocity, 99);
								Main.gore[num].velocity *= 0.3f;
                            }
						}
                    }
					if (ModContent.GetInstance<VampireConfig>().vampireAbility2)
					{
						if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed && player.mount.Type == MountType<StealthBat>())
						{
							Vector2 velocity = Vector2.Normalize(mrPlagueRacesPlayer.mouseWorld - player.Center) * 10f;
							if (player.ownedProjectileCounts[ProjectileType<LeechTongue>()] == 0)
							{
								SoundEngine.PlaySound(SoundID.Item111, player.Center);
								SoundEngine.PlaySound(SoundID.Item171, player.Center);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.VampireShootTongueSound(-1, Main.myPlayer);
                                }
                                // {T} Now supplying -1 for ai[1] in the below line, since that will now store the index of the NPC the tongue is stuck to.
                                vampirePlayer.LeechTongue = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, velocity.X, velocity.Y, ProjectileType<LeechTongue>(), 1, 0, player.whoAmI, 0, -1);
							}
							else
							{
								Main.projectile[vampirePlayer.LeechTongue].ai[0] = 60;
								NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, vampirePlayer.LeechTongue); // {T} This was never getting synced! It is now :3
                            }
						}
						if (player.mount.Type == MountType<StealthBat>())
						{
							if (player.controlUseItem)
							{
								player.controlUseItem = false;
							}
						}
					}
                }
            }
		}

		public override void UpdateBadLifeRegen(Player player) {
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats && ModContent.GetInstance<VampireConfig>().vampireNoRegen) {
				if (player.lifeRegen > 0)
					player.lifeRegen = 0;
				player.lifeRegenTime = 0;
			}
		}

		public override bool CanUseItem(Player player, Item item)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats && ModContent.GetInstance<VampireConfig>().vampireHealingPotionDenial) {
				if (item.healLife > 0) {
					return false;
				}
				else {
					return true;
				}
			}
			else {
				return true;
			}
		}

		public override void HideDrawLayers(Player player, PlayerDrawSet drawInfo) {
			if (player.mount.Type == MountType<StealthBat>())
			{
				foreach (var layer in PlayerDrawLayerLoader.Layers)
				{
					if (layer != PlayerDrawLayers.MountBack && layer != PlayerDrawLayers.MountFront)
					{
						layer.Hide();
					}
				}
			}
		}

		public override void PreUpdate(Player player) {
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var vampirePlayer = player.GetModPlayer<VampirePlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
				{
					if (player.mount.Type == MountType<StealthBat>() && !vampirePlayer.Leeching) {
						if (vampirePlayer.stealthTimer < 420) {
							vampirePlayer.stealthTimer++;
						}
						if (vampirePlayer.stealthTimer == 420) {
							if (!player.HasBuff(BuffType<Unseen>())) {
								SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse, player.Center);
							}
							player.AddBuff(BuffType<Unseen>(), 120);
						}
					}
					else {
						vampirePlayer.stealthTimer = 0;
                    }
                    if (ModContent.GetInstance<VampireConfig>().vampireSunlightWeakness)
                    {
						if (vampirePlayer.ExposedToSun())
						{
							player.AddBuff(BuffType<Photosensitive>(), 2);
						}
                    }
                    if (ModContent.GetInstance<VampireConfig>().vampireLight == LightTiers.On || (ModContent.GetInstance<VampireConfig>().vampireLight == LightTiers.BatOnly && player.mount.Type == MountType<StealthBat>()) || (ModContent.GetInstance<VampireConfig>().vampireLight == LightTiers.NotBat && player.mount.Type != MountType<StealthBat>()))
                    {
                        Lighting.AddLight(player.Center, player.eyeColor.ToVector3());
                    }
                }
			}
        }

        public override void PostUpdate(Player player)
        {
            var vampirePlayer = player.GetModPlayer<VampirePlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					vampirePlayer.SyncPlayer(-1, Main.myPlayer, false);
				}

                var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
                mrPlagueRacesPlayer.SyncRotationsAndOffsets(-1, Main.myPlayer);
            }
        }
    }

	public class VampirePlayer : ModPlayer
	{
		public int stealthTimer;
		public int LeechTongue;
		public bool Leeching;

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.VampireSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(stealthTimer);
			packet.Write(LeechTongue);
			packet.Write(Leeching);

            packet.Send(toWho, fromWho);
        }

		public bool ExposedToSun()
		{
			Tile[] smallWallTiles = new Tile[2];
			Point playerTilePointSmall = (Main.LocalPlayer.position / 16).ToPoint();
			smallWallTiles[0] = Framing.GetTileSafely(playerTilePointSmall.X, playerTilePointSmall.Y);
			smallWallTiles[1] = Framing.GetTileSafely(playerTilePointSmall.X + 1, playerTilePointSmall.Y);
			bool behindSmallWall = false;
			foreach (var tile in smallWallTiles)
			{
				if (tile.WallType > 0)
				{
					behindSmallWall = true;
				}
				else
				{
					behindSmallWall = false;
					break;
				}
			}
			Tile[] wallTiles = new Tile[6];
			Point playerTilePoint = (Main.LocalPlayer.position / 16).ToPoint();
			wallTiles[0] = Framing.GetTileSafely(playerTilePoint.X, playerTilePoint.Y);
			wallTiles[1] = Framing.GetTileSafely(playerTilePoint.X, playerTilePoint.Y + 1);
			wallTiles[2] = Framing.GetTileSafely(playerTilePoint.X, playerTilePoint.Y + 2);
			wallTiles[3] = Framing.GetTileSafely(playerTilePoint.X + 1, playerTilePoint.Y);
			wallTiles[4] = Framing.GetTileSafely(playerTilePoint.X + 1, playerTilePoint.Y + 1);
			wallTiles[5] = Framing.GetTileSafely(playerTilePoint.X + 1, playerTilePoint.Y + 2);
			bool behindWall = false;
			foreach (var tile in wallTiles)
			{
				if (tile.WallType > 0)
				{
					behindWall = true;
				}
				else
				{
					behindWall = false;
					break;
				}
			}
			Tile[] largeWallTiles = new Tile[36];
			Point playerTilePointLarge = (Main.LocalPlayer.position / 16).ToPoint();
			largeWallTiles[0] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 15);
			largeWallTiles[1] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 14);
			largeWallTiles[2] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 13);
			largeWallTiles[3] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 12);
			largeWallTiles[4] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 11);
			largeWallTiles[5] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 10);
			largeWallTiles[6] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 9);
			largeWallTiles[7] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 8);
			largeWallTiles[8] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 7);
			largeWallTiles[9] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 6);
			largeWallTiles[10] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 5);
			largeWallTiles[11] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 4);
			largeWallTiles[12] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 3);
			largeWallTiles[13] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 2);
			largeWallTiles[14] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 1);
			largeWallTiles[15] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y);
			largeWallTiles[16] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y + 1);
			largeWallTiles[17] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y + 2);
			largeWallTiles[18] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 15);
			largeWallTiles[19] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 14);
			largeWallTiles[20] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 13);
			largeWallTiles[21] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 12);
			largeWallTiles[22] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 11);
			largeWallTiles[23] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 10);
			largeWallTiles[24] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 9);
			largeWallTiles[25] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 8);
			largeWallTiles[26] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 7);
			largeWallTiles[27] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 6);
			largeWallTiles[28] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 5);
			largeWallTiles[29] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 4);
			largeWallTiles[30] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 3);
			largeWallTiles[31] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 2);
			largeWallTiles[32] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 1);
			largeWallTiles[33] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y);
			largeWallTiles[34] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y + 1);
			largeWallTiles[35] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y + 2);
			bool behindLargeWall = false;
			foreach (var tile in largeWallTiles)
			{
				if (tile.WallType > 0)
				{
					behindLargeWall = true;
				}
				else
				{
					behindLargeWall = false;
					break;
				}
			}
			bool hasCeilingTile = false;
			Vector2 playerLocation = new Vector2(Player.Center.X / 16, Player.Center.Y / 16);
			for (int i = 0; i < 60; i++)
			{
				Tile ceilingTile = Main.tile[(int)playerLocation.X, (int)playerLocation.Y];
				if (ceilingTile != null && Main.tileSolid[ceilingTile.TileType] && ceilingTile.HasUnactuatedTile)
				{
					hasCeilingTile = true;
				}
				if (playerLocation.Y > 0)
				{
					playerLocation.Y -= 1;
				}
			}
			bool hasCeilingAbove = false;
			if (behindLargeWall || hasCeilingTile)
			{
				hasCeilingAbove = true;
			}
			else
			{
				hasCeilingAbove = false;
			}
			if (Player.mount.Type != MountType<StealthBat>())
			{
				return (!hasCeilingAbove || !behindWall) && !((double)Player.Center.Y > Main.worldSurface * 16.0) && Main.dayTime && !(Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir));
			}
			else
			{
				return (!hasCeilingAbove || !behindSmallWall) && !((double)Player.Center.Y > Main.worldSurface * 16.0) && Main.dayTime && !(Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir));
			}
		}
    }

    public enum LightTiers
    {
        Off,
        BatOnly,
        NotBat,
        On
    }

    public class VampireConfig : ModConfig
    {
        public static VampireConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Vampire")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> vampireStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%
            [RacialStatType.statLifeMax2] = RacialStatPercentageModifier.n25,
            [RacialStatType.moveSpeed] = RacialStatPercentageModifier.p10,
            [RacialStatType.endurance] = RacialStatPercentageModifier.n15
        };
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool vampireAbility1;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool vampireAbility2;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier vampireStealthDamage;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier vampireBatVelocity;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier vampireBatFlightDuration;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier vampireLifeSteal;
        [DefaultValue(LightTiers.BatOnly)]
        [BackgroundColor(110, 141, 255)]
        public LightTiers vampireLight;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool vampireHealingPotionDenial;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool vampireNoRegen;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool vampireSunlightWeakness;
    }
}