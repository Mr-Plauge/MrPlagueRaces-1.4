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

namespace MrPlagueRaces.Common.Races.Merfolk
{
	public class Merfolk : Race
	{
		public override void Load()
        {
			Description = "Excellent at swimming and fishing, Merfolk breathe water instead of air.";
            DisplayName = "[c/00FF9D:Merfolk]";
            CensorClothing = false;
			HairColor = new Color(108, 255, 61);
			SkinColor = new Color(58, 188, 116);
			DetailColor = new Color(108, 255, 61);
			EyeColor = new Color(255, 81, 81);
        }

        public override void PreRaceChange(Player player) // Called before the player's race is changed
        {
            var merfolkPlayer = player.GetModPlayer<MerfolkPlayer>();
            merfolkPlayer.fullRotation = 0;
            merfolkPlayer.targetFullRotation = 0;
            merfolkPlayer.headRotation = 0;
            merfolkPlayer.targetHeadRotation = 0;
            merfolkPlayer.swimming = false;
            merfolkPlayer.diveCount = 0;
            merfolkPlayer.breathHurt = 0;
            merfolkPlayer.breathInterval = 7;
            merfolkPlayer.breathMeter = 200;
        }

        public override void ResetEffects(Player player)
        {
            RegisterAbilityDescription(ModContent.GetInstance<MerfolkConfig>().merfolkAbility1, $"[c/4DBF60:+] While underwater, hold Z to streamline-swim. Improves control and speed.");
            RegisterAbilityDescription(ModContent.GetInstance<MerfolkConfig>().merfolkAbility2, $"[c/4DBF60:+] While underwater, press X to kick forward for a boost of momentum.");
            RegisterAbilityDescription(ModContent.GetInstance<MerfolkConfig>().merfolkFishBowl && !ModContent.GetInstance<MerfolkConfig>().merfolkRain, $"[c/4DBF60:+] Equip a Fish Bowl to breathe outside of water.");
            RegisterAbilityDescription(!ModContent.GetInstance<MerfolkConfig>().merfolkFishBowl && ModContent.GetInstance<MerfolkConfig>().merfolkRain, $"[c/4DBF60:+] Stand in rain to breathe outside of water.");
            RegisterAbilityDescription(ModContent.GetInstance<MerfolkConfig>().merfolkFishBowl && ModContent.GetInstance<MerfolkConfig>().merfolkRain, $"[c/4DBF60:+] Equip a Fish Bowl or stand in rain to breathe outside of water.");
            RegisterAbilityDescription(ModContent.GetInstance<MerfolkConfig>().merfolkNoAirBreathing, $"[c/FF3640:-] You cannot breathe air.");

            RaceStatDictionary = ModContent.GetInstance<MerfolkConfig>().merfolkStats;
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				/*player.pickSpeed -= 0.1f;
				player.tileSpeed += 0.1f;*/
                player.merman = false;
                player.gills = false;
                if (ModContent.GetInstance<MerfolkConfig>().merfolkIgnoreWater)
				{
					player.ignoreWater = true;
				}
				if (ModContent.GetInstance<MerfolkConfig>().merfolkAccFlipper)
				{
					player.accFlipper = true;
				}
			}
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var merfolkPlayer = player.GetModPlayer<MerfolkPlayer>();
			merfolkPlayer.fullRotation = 0;
			merfolkPlayer.targetFullRotation = 0;
			merfolkPlayer.headRotation = 0;
			merfolkPlayer.targetHeadRotation = 0;
			merfolkPlayer.swimming = false;
			merfolkPlayer.diveCount = 0;
			merfolkPlayer.breathHurt = 0;
			merfolkPlayer.breathInterval = 7;
			merfolkPlayer.breathMeter = 200;
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var merfolkPlayer = player.GetModPlayer<MerfolkPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead && player.active)
                {
					if (ModContent.GetInstance<MerfolkConfig>().merfolkAbility1)
					{
						if (MrPlagueRaces.RaceAbilityKeybind1.Current && player.wet && merfolkPlayer.diveCount == 0)
						{
							Vector2 velocity = Vector2.Normalize(mrPlagueRacesPlayer.mouseWorld - player.Center) * (5 + (player.statLifeMax2 / 100));
							player.velocity = velocity * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<MerfolkConfig>().merfolkSwimVelocity]);
							merfolkPlayer.swimming = true;
							if (player.swimTime <= 10)
							{
								player.swimTime = 30;
							}
							player.controlUp = false;
							player.controlLeft = false;
							player.controlDown = false;
							player.controlRight = false;
							player.controlJump = false;
							player.fallStart = (int)(player.position.Y / 16f);
							player.AddBuff(BuffType<FluidGrace>(), 340);
						}
						else if (merfolkPlayer.diveCount == 0)
						{
							merfolkPlayer.swimming = false;
						}
					}
					if (ModContent.GetInstance<MerfolkConfig>().merfolkAbility2)
					{
						if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed && player.wet && merfolkPlayer.diveCount == 0)
						{
							merfolkPlayer.diveCount = 60;
						}
					}
				}
			}
		}

		public override void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var merfolkPlayer = player.GetModPlayer<MerfolkPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead && !player.sleeping.isSleeping) {
					if (merfolkPlayer.swimming) {
						Vector2 offset = mrPlagueRacesPlayer.mouseWorld - player.Center;
						merfolkPlayer.targetFullRotation = ((offset * merfolkPlayer.direction).ToRotation() * 0.55f) + (merfolkPlayer.direction == 1 ? 1.575f : -1.575f);
						merfolkPlayer.targetHeadRotation = ((offset * merfolkPlayer.direction).ToRotation() * 0.55f) + (merfolkPlayer.direction == 1 ? -1.575f : 1.575f);
						player.ChangeDir(merfolkPlayer.direction);
                    } 
					else 
					{
						merfolkPlayer.targetFullRotation = 0;
						merfolkPlayer.targetHeadRotation = 0;
					}
					merfolkPlayer.fullRotation = MathHelper.Lerp(merfolkPlayer.fullRotation, merfolkPlayer.targetFullRotation, 16f / 60);
					merfolkPlayer.headRotation = MathHelper.Lerp(merfolkPlayer.headRotation, merfolkPlayer.targetHeadRotation, 16f / 60);
					player.fullRotationOrigin = new Vector2((player.width / 2), (player.height / 2));
					player.fullRotation = (merfolkPlayer.swimming ? merfolkPlayer.targetFullRotation : merfolkPlayer.fullRotation);
                    player.headRotation = (merfolkPlayer.swimming ? merfolkPlayer.targetHeadRotation : merfolkPlayer.headRotation);
					if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) && !merfolkPlayer.swimming && merfolkPlayer.diveCount == 0)
					{
						player.headRotation = player.velocity.Y * (float)merfolkPlayer.direction * 0.1f;
						if ((double)player.headRotation < -0.3)
						{
							player.headRotation = -0.3f;
						}
						if ((double)player.headRotation > 0.3)
						{
							player.headRotation = 0.3f;
						}
                    }
                }
            }
        }

		public override void PreUpdate(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var merfolkPlayer = player.GetModPlayer<MerfolkPlayer>();
            if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (merfolkPlayer.diveCount > 0) {
					merfolkPlayer.swimming = true;
					if (merfolkPlayer.diveCount == 60) {
						Vector2 velocity = -Vector2.Normalize(mrPlagueRacesPlayer.mouseWorld - player.Center) * 5;
						player.velocity = velocity;
						player.swimTime = 0;
					}
					if (merfolkPlayer.diveCount == 45) {
						player.swimTime = 30;
					}
					if (merfolkPlayer.diveCount == 40) {
						Vector2 velocity = Vector2.Normalize(mrPlagueRacesPlayer.mouseWorld - player.Center) * (15 + (player.statLifeMax2 / 100));
						player.velocity = velocity * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<MerfolkConfig>().merfolkLungeVelocity]);
						player.AddBuff(BuffType<FluidGrace>(), 120);
					}
					merfolkPlayer.diveCount--;
				}
				if (player.dead)
				{
					merfolkPlayer.breathInterval = 0;
					merfolkPlayer.breathMeter = 200;
				}

                if (ModContent.GetInstance<MerfolkConfig>().merfolkNoAirBreathing)
                {
                    if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) || (ModContent.GetInstance<MerfolkConfig>().merfolkFishBowl && (player.armor[0].type == ItemID.FishBowl) || (player.armor[0].type == ItemID.GoldGoldfishBowl)) || (ModContent.GetInstance<MerfolkConfig>().merfolkRain && Main.raining && merfolkPlayer.ExposedToSky()))
					{
						merfolkPlayer.breathInterval = 0;
						if (merfolkPlayer.breathMeter < 200)
						{
							merfolkPlayer.breathMeter += 3;
						}
						if (merfolkPlayer.breathMeter > 200)
						{
							merfolkPlayer.breathMeter = 200;
						}
						player.breath = (merfolkPlayer.breathMeter + 2);
						merfolkPlayer.breathHurt = 0;
                    }
					else
					{
						merfolkPlayer.breathInterval += 1;
						if (merfolkPlayer.breathInterval >= 7)
						{
							merfolkPlayer.breathMeter -= 1;
							merfolkPlayer.breathInterval = 0;
						}
						player.breath = (merfolkPlayer.breathMeter - 2);
                    }
					if (player.breath == 0)
					{
						SoundEngine.PlaySound(SoundID.Drown, player.Center);
					}
					if (player.breath <= 0)
					{
						player.lifeRegenTime = 0;
						player.breath = 0;
						merfolkPlayer.breathHurt += 1;
						if (merfolkPlayer.breathHurt >= 7)
						{
							player.statLife -= 2;
							merfolkPlayer.breathHurt = 0;
						}
						if (player.statLife <= 0)
						{
							player.statLife = 0;
							switch (Main.rand.Next(8))
							{
								case 0:
									player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " is sleeping with the airbreathers."), 10.0, 0, false);
									break;
								case 1:
									player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " didn't make it to the water."), 10.0, 0, false);
									break;
								case 2:
									player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " was out of their element."), 10.0, 0, false);
									break;
								case 3:
									player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " suffocated."), 10.0, 0, false);
									break;
								case 4:
									player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " couldn't breathe."), 10.0, 0, false);
									break;
								case 5:
									player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " is food for the land dwellers."), 10.0, 0, false);
									break;
								case 6:
									player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " tried breathing air."), 10.0, 0, false);
									break;
								default:
									player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " had gills instead of lungs."), 10.0, 0, false);
									break;
							}
						}
					}
				}
                else
                {
                    merfolkPlayer.breathInterval = 0;
                    if (merfolkPlayer.breathMeter < 200)
                    {
                        merfolkPlayer.breathMeter += 3;
                    }
                    if (merfolkPlayer.breathMeter > 200)
                    {
                        merfolkPlayer.breathMeter = 200;
                    }
                    player.breath = (merfolkPlayer.breathMeter + 2);
                    merfolkPlayer.breathHurt = 0;
                }
            }
        }

        public override void PostUpdate(Player player)
        {
            var merfolkPlayer = player.GetModPlayer<MerfolkPlayer>();
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
                merfolkPlayer.direction = mrPlagueRacesPlayer.mouseWorld.X >= player.Center.X ? 1 : -1;

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					merfolkPlayer.SyncPlayer(-1, Main.myPlayer, false);
				}

                if (player.whoAmI == Main.myPlayer)
                {
                    mrPlagueRacesPlayer.SyncRotationsAndOffsets(-1, Main.myPlayer);
                }
            }
        }
    }

	public class MerfolkPlayer : ModPlayer
	{
		public float fullRotation;
		public float targetFullRotation;
		public float headRotation;
		public float targetHeadRotation;
		public bool swimming = false;
		public int diveCount;
		public int breathHurt;
		public int breathInterval = 7;
		public int breathMeter = 200;
        public int direction = 1;

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.MerfolkSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(fullRotation);
			packet.Write(targetFullRotation);
			packet.Write(headRotation);
			packet.Write(targetHeadRotation);
			packet.Write(swimming);
			packet.Write(diveCount);
			packet.Write(breathHurt);
			packet.Write(breathInterval);
			packet.Write(breathMeter);
            packet.Write(direction);

            packet.Send(toWho, fromWho);
        }

        public bool ExposedToSky()
        {
            bool hasCeilingTile = false;
            Point playerTileCoordinate = Player.Center.ToTileCoordinates();
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
            return !(playerTileCoordinate.Y <= Main.maxTilesY - 200 && (double)playerTileCoordinate.Y > Main.rockLayer) && !hasCeilingTile;
        }
    }

    public class MerfolkConfig : ModConfig
    {
        public static MerfolkConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Merfolk")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> merfolkStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%
            [RacialStatType.pickSpeed] = RacialStatPercentageModifier.p10,
            [RacialStatType.tileSpeed] = RacialStatPercentageModifier.p10
        };
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool merfolkAbility1;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool merfolkAbility2;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier merfolkSwimVelocity;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier merfolkLungeVelocity;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool merfolkIgnoreWater;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool merfolkAccFlipper;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool merfolkFishBowl;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool merfolkRain;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool merfolkNoAirBreathing;
    }
}