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

namespace MrPlagueRaces.Common.Races.Dragonkin
{
	public class Dragonkin : Race
	{
		public override void Load()
        {
			Description = "Built for living in arid environments, Dragonkin are strong and endurant.";
            DisplayName = "[c/309665:Dragonkin]";
            CensorClothing = false;
			StarterShirt = true;
			StarterPants = true;
			ClothStyle = 5;
			AlwaysDrawHair = true;
			HairColor = new Color(214, 207, 199);
			SkinColor = new Color(106, 138, 110);
			DetailColor = new Color(182, 215, 126);
			EyeColor = new Color(255, 180, 92);
			ShirtColor = new Color(119, 115, 157);
			UnderShirtColor = new Color(216, 156, 95);
        }

		public override void ResetEffects(Player player)
        {
            RegisterAbilityDescription(ModContent.GetInstance<DragonkinConfig>().dragonkinAbility1, $"[c/4DBF60:+] Hold Z to breathe burning smoke. Damage scales with defense.");
            RegisterAbilityDescription(ModContent.GetInstance<DragonkinConfig>().dragonkinAbility2, $"[c/4DBF60:+] Press X to release a molotov fireball. Damage scales with defense.");

            RaceStatDictionary = ModContent.GetInstance<DragonkinConfig>().dragonkinStats;
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats)
            {
                /*player.endurance += 0.2f;
				player.jumpSpeedBoost -= 0.1f;
				player.moveSpeed -= 0.2f;*/
            }
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var dragonkinPlayer = player.GetModPlayer<DragonkinPlayer>();
			dragonkinPlayer.breathingSmoke = false;
			dragonkinPlayer.firingSmoke = 0;
			dragonkinPlayer.burningOut = 0;
			dragonkinPlayer.soundInterval = 0;
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var dragonkinPlayer = player.GetModPlayer<DragonkinPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
				{
					if (!player.HasBuff(BuffType<BurnedOut>()))
                    {
						if (ModContent.GetInstance<DragonkinConfig>().dragonkinAbility1)
						{
							if (MrPlagueRaces.RaceAbilityKeybind1.JustPressed)
							{
								dragonkinPlayer.soundInterval = 20;
							}
							if (MrPlagueRaces.RaceAbilityKeybind1.Current)
							{
								dragonkinPlayer.breathingSmoke = true;
								if (dragonkinPlayer.burningOut < 90)
								{
									dragonkinPlayer.burningOut++;
								}
							}
							else
							{
								dragonkinPlayer.breathingSmoke = false;
							}
                        }
						if (ModContent.GetInstance<DragonkinConfig>().dragonkinAbility2)
						{
							if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed)
							{
								player.ChangeDir(dragonkinPlayer.direction);
								SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, player.Center);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.DragonkinFireballSound(-1, Main.myPlayer);
                                }
                                Vector2 velocity = Vector2.Normalize(mrPlagueRacesPlayer.mouseWorld - player.Center) * 15f;
								Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X + (float)(player.width / 2) / 16, player.Center.Y, velocity.X, velocity.Y, ProjectileType<MolotovFireball>(), (5 + (player.statDefense < 20 ? player.statDefense / 5 : player.statDefense < 40 ? player.statDefense / 2 : player.statDefense < 60 ? player.statDefense : player.statDefense * 1.25f)) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<DragonkinConfig>().dragonkinFireDamage]), 8, player.whoAmI);
								dragonkinPlayer.burningOut += 30;
								dragonkinPlayer.firingSmoke = 20;
							}
						}
					}
				}
			}
		}

		public override void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var dragonkinPlayer = player.GetModPlayer<DragonkinPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead && !player.sleeping.isSleeping) {
					if (dragonkinPlayer.breathingSmoke || dragonkinPlayer.firingSmoke > 0)
                    {
                        player.ChangeDir(dragonkinPlayer.direction);
                        Vector2 offset = mrPlagueRacesPlayer.mouseWorld - player.Center;
						dragonkinPlayer.targetHeadRotation = (offset * player.direction).ToRotation() * 0.55f;
					} 
					else 
					{
						dragonkinPlayer.targetHeadRotation = 0;
					}
					dragonkinPlayer.headRotation = MathHelper.Lerp(dragonkinPlayer.headRotation, dragonkinPlayer.targetHeadRotation, 16f / 60);
					player.headRotation = dragonkinPlayer.headRotation;
                }
            }
		}

		public override void PreUpdate(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var dragonkinPlayer = player.GetModPlayer<DragonkinPlayer>();
            if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats)
			{
				if (!player.dead)
				{
					for (int i = 0; i < dragonkinPlayer.burningOut; i++)
					{
						if (Main.rand.Next(80) == 1)
						{
							Dust.NewDust(player.position, player.width, player.height, 6);
						}
					}
					if (!dragonkinPlayer.breathingSmoke && dragonkinPlayer.burningOut > 0)
					{
						dragonkinPlayer.burningOut--;
					}
					if (dragonkinPlayer.burningOut >= 90)
					{
						if (!player.HasBuff(BuffType<BurnedOut>()))
						{
							SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, player.Center);
						}
						player.AddBuff(BuffType<BurnedOut>(), 120);
						dragonkinPlayer.breathingSmoke = false;
					}
					if (dragonkinPlayer.breathingSmoke)
					{
						Vector2 velocity = Vector2.Normalize(mrPlagueRacesPlayer.mouseWorld - player.Center) * 10f;
						Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X + (float)(player.width / 2) / 16, player.Center.Y, velocity.X, velocity.Y, ProjectileType<BreathSmoke>(), 0, 0, player.whoAmI);
						if (dragonkinPlayer.soundInterval >= 20)
						{
							SoundEngine.PlaySound(SoundID.Item34, player.Center);
							dragonkinPlayer.soundInterval = 0;
						}
						else
						{
							dragonkinPlayer.soundInterval++;
						}
						for (int i = 0; i < 3; i++)
						{
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), mrPlagueRacesPlayer.mouseWorld.X + Main.rand.Next(60) - Main.rand.Next(60), mrPlagueRacesPlayer.mouseWorld.Y + Main.rand.Next(60) - Main.rand.Next(60), 0f, 0f, ProjectileType<BurningSmoke>(), (5 + (player.statDefense < 20 ? player.statDefense / 5 : player.statDefense < 40 ? player.statDefense / 2 : player.statDefense < 60 ? player.statDefense : player.statDefense * 1.1f)) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<DragonkinConfig>().dragonkinSmokeDamage]), 0, player.whoAmI);
						}
					}
					if (dragonkinPlayer.firingSmoke > 0)
					{
						dragonkinPlayer.firingSmoke--;
					}
					if (dragonkinPlayer.firingSmoke < 0)
					{
						dragonkinPlayer.firingSmoke = 0;
					}
				}
			}
		}

		public override void PostUpdate(Player player)
        {
            var dragonkinPlayer = player.GetModPlayer<DragonkinPlayer>();
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
                dragonkinPlayer.direction = mrPlagueRacesPlayer.mouseWorld.X >= player.Center.X ? 1 : -1;

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					dragonkinPlayer.SyncPlayer(-1, Main.myPlayer, false);
				}

                mrPlagueRacesPlayer.SyncRotationsAndOffsets(-1, Main.myPlayer);
            }
        }
    }

	public class DragonkinPlayer : ModPlayer
	{
		public float headRotation;
		public float targetHeadRotation;
		public bool breathingSmoke = false;
		public int firingSmoke;
		public int burningOut;
		public int soundInterval;
        public int direction = 1;

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.DragonkinSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(headRotation);
			packet.Write(targetHeadRotation);
			packet.Write(breathingSmoke);
			packet.Write(firingSmoke);
			packet.Write(burningOut);
			packet.Write(soundInterval);
            packet.Write(direction);

            packet.Send(toWho, fromWho);
        }
	}

    public class DragonkinConfig : ModConfig
    {
        public static DragonkinConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Dragonkin")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> dragonkinStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%
            [RacialStatType.endurance] = RacialStatPercentageModifier.p20,
            [RacialStatType.moveSpeed] = RacialStatPercentageModifier.n20,
            [RacialStatType.jumpSpeedBoost] = RacialStatPercentageModifier.n10
        };
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool dragonkinAbility1;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool dragonkinAbility2;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier dragonkinSmokeDamage;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier dragonkinFireDamage;
    }
}