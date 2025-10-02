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

namespace MrPlagueRaces.Common.Races.Derpkin
{
	public class Derpkin : Race
	{
		public override void Load()
		{
			Description = "Native to the jungle surface, Derpkin are aerodynamic and deadly.";
            DisplayName = "[c/007BFF:Derpkin]";
            CensorClothing = false;
			HairColor = new Color(82, 179, 255);
			SkinColor = new Color(82, 179, 255);
			DetailColor = new Color(48, 76, 128);
			EyeColor = new Color(99, 122, 207);
        }

        public override void PreRaceChange(Player player) // Called before the player's race is changed
        {
            var derpkinPlayer = player.GetModPlayer<DerpkinPlayer>();
            derpkinPlayer.headRotation = 0;
            derpkinPlayer.targetHeadRotation = 0;
            derpkinPlayer.counterSpin = 0;
        }

        public override void ResetEffects(Player player)
        {
            RegisterAbilityDescription(ModContent.GetInstance<DerpkinConfig>().derpkinAbility1, $"[c/4DBF60:+] Press Z to leap towards your cursor.");
            RegisterAbilityDescription(ModContent.GetInstance<DerpkinConfig>().derpkinAbility2, $"[c/4DBF60:+] Press X to spin, gaining temporary invincibility.");

            RaceStatDictionary = ModContent.GetInstance<DerpkinConfig>().derpkinStats;
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats)
            {
                /*player.moveSpeed += 0.2f;
				player.jumpSpeedBoost += 0.3f;
				player.statLifeMax2 -= (player.statLifeMax2 / 5);
				player.endurance -= 0.2f;*/
            }
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var derpkinPlayer = player.GetModPlayer<DerpkinPlayer>();
			derpkinPlayer.headRotation = 0;
			derpkinPlayer.targetHeadRotation = 0;
			derpkinPlayer.counterSpin = 0;
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var derpkinPlayer = player.GetModPlayer<DerpkinPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats)
			{
				if (!player.dead)
				{
					if (!player.HasBuff(BuffType<Outstretched>()))
					{
						if (ModContent.GetInstance<DerpkinConfig>().derpkinAbility1)
						{
                            if (MrPlagueRaces.RaceAbilityKeybind1.JustPressed)
                            {
                                SoundEngine.PlaySound(SoundID.Item39, player.Center);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.DerpkinLeapSound(-1, Main.myPlayer);
                                }
                                for (int i = 0; i < 18; i++)
                                {
                                    Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, Main.rand.Next(3) - Main.rand.Next(3), Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, player.whoAmI);
                                }
                                player.direction = mrPlagueRacesPlayer.mouseWorld.X >= player.Center.X ? 1 : -1;
                                Vector2 offset = mrPlagueRacesPlayer.mouseWorld - player.Center;
                                derpkinPlayer.targetHeadRotation = (offset * player.direction).ToRotation() * 0.55f;
                                Vector2 velocity = Vector2.Normalize(mrPlagueRacesPlayer.mouseWorld - player.Center) * (15 + (player.statLifeMax2 / 80)) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<DerpkinConfig>().derpkinLeapVelocity]);
                                player.velocity = velocity;
                                player.fallStart = (int)(player.position.Y / 16f);
                                player.AddBuff(BuffType<Outstretched>(), 180 - (player.statLifeMax2 / 10));
                            }
                        }
					}
					if (!player.HasBuff(BuffType<Unravelled>()))
                    {
						if (ModContent.GetInstance<DerpkinConfig>().derpkinAbility2)
						{
							if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed)
							{
								for (int i = 0; i < 9; i++)
								{
									Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, Main.rand.Next(3) - Main.rand.Next(3), Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, player.whoAmI);
								}
								derpkinPlayer.counterSpin = 30;
								player.velocity.Y -= (10 + (player.statLifeMax2 / 80)) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<DerpkinConfig>().derpkinSpinVelocity]);
								player.fallStart = (int)(player.position.Y / 16f);
								player.AddBuff(BuffType<Unravelled>(), 180 - (player.statLifeMax2 / 10));
							}
						}
					}
				}
			}
		}

		public override void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var derpkinPlayer = player.GetModPlayer<DerpkinPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats)
			{
				if (!player.dead && !player.sleeping.isSleeping)
				{
					derpkinPlayer.headRotation = MathHelper.Lerp(derpkinPlayer.headRotation, derpkinPlayer.targetHeadRotation, 16f / 60);
					player.fullRotationOrigin = new Vector2((player.width / 2), (player.height / 2));
					player.fullRotation = -derpkinPlayer.headRotation / 2;
					player.headRotation = derpkinPlayer.headRotation;
				}
			}
		}

		public override void PreUpdate(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var derpkinPlayer = player.GetModPlayer<DerpkinPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats)
			{
				if (!player.dead)
				{
					if (player.velocity.Y == 0)
					{
						derpkinPlayer.targetHeadRotation = 0;
					}
					if (derpkinPlayer.counterSpin > 0)
					{
						if (derpkinPlayer.counterSpin % 5 == 0)
						{
							SoundEngine.PlaySound(SoundID.Item1, player.Center);
							if (player.whoAmI == Main.myPlayer)
							{
								mrPlagueRacesPlayer.DerpkinSpinSound(-1, Main.myPlayer);
                            }
                            player.direction = player.direction == 1 ? -1 : 1;
							for (int i = 0; i < 2; i++)
							{
								Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, Main.rand.Next(3) - Main.rand.Next(3), Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, player.whoAmI);
							}
						}
						derpkinPlayer.counterSpin--;
					}
				}
			}
		}

        public override bool FreeDodge(Player player, Player.HurtInfo info)
        {
            var derpkinPlayer = player.GetModPlayer<DerpkinPlayer>();
            if (derpkinPlayer.counterSpin > 0)
            {
                return true;
            }
            return false;
        }
    }

	public class DerpkinPlayer : ModPlayer
	{
		public float headRotation;
		public float targetHeadRotation;
		public int counterSpin;
	}

	public class DerpkinConfig : ModConfig
	{
		public static DerpkinConfig Instance;
		public override ConfigScope Mode => ConfigScope.ServerSide;

		//[Header("Derpkin")]
		[BackgroundColor(110, 141, 255)]
		public Dictionary<RacialStatType, RacialStatPercentageModifier> derpkinStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
		{
			//n describes negative, p describes positive. IE, n20 is equivalent to -20%
			[RacialStatType.statLifeMax2] = RacialStatPercentageModifier.n20,
			[RacialStatType.moveSpeed] = RacialStatPercentageModifier.p20,
			[RacialStatType.jumpSpeedBoost] = RacialStatPercentageModifier.p30,
			[RacialStatType.endurance] = RacialStatPercentageModifier.n20
		};
		[DefaultValue(true)]
		[BackgroundColor(110, 141, 255)]
		public bool derpkinAbility1;
		[DefaultValue(true)]
		[BackgroundColor(110, 141, 255)]
		public bool derpkinAbility2;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier derpkinLeapVelocity;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier derpkinSpinVelocity;
    }
}