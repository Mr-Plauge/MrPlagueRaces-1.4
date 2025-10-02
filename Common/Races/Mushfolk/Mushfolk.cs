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

namespace MrPlagueRaces.Common.Races.Mushfolk
{
	public class Mushfolk : Race
	{
		public override void Load()
        {
			Description = "Luminous and mysterious, Mushfolk feed on health to fuel their healing abilities.";
            DisplayName = "[c/006EFF:Mushfolk]";
            CensorClothing = false;
			HairColor = new Color(138, 159, 255);
			SkinColor = new Color(239, 222, 202);
			DetailColor = new Color(239, 222, 202);
			EyeColor = new Color(138, 159, 255);
        }

        public override void PreRaceChange(Player player) // Called before the player's race is changed
        {
            var mushfolkPlayer = player.GetModPlayer<MushfolkPlayer>();
            mushfolkPlayer.growingMushrooms = false;
            mushfolkPlayer.sporeless = 0;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && (projectile.type == ProjectileType<Trapshroom>() || projectile.type == ProjectileType<Nymphshroom>()) && projectile.owner == player.whoAmI)
                    projectile.Kill();
            }
        }

        public override void ResetEffects(Player player)
        {
            RegisterAbilityDescription(ModContent.GetInstance<MushfolkConfig>().mushfolkAbility1, $"[c/4DBF60:+] Hold Z to release trapshroom spores, which release healing clouds upon contact with an enemy. Healing scales with the individual's max health.");
            RegisterAbilityDescription(ModContent.GetInstance<MushfolkConfig>().mushfolkAbility2, $"[c/4DBF60:+] Press X to create a Nymphshroom, which teleports you to it upon contact with an enemy.");

            RaceStatDictionary = ModContent.GetInstance<MushfolkConfig>().mushfolkStats;
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				/*player.statLifeMax2 += (player.statLifeMax2 / 10);
				player.moveSpeed += 0.05f;
				player.GetDamage(DamageClass.Generic) -= 0.1f;
				player.endurance -= 0.25f;*/
			}
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var mushfolkPlayer = player.GetModPlayer<MushfolkPlayer>();
			mushfolkPlayer.growingMushrooms = false;
			mushfolkPlayer.sporeless = 0;
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var mushfolkPlayer = player.GetModPlayer<MushfolkPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
				{
					if (!player.HasBuff(BuffType<Sporeless>())) {
                        if (ModContent.GetInstance<MushfolkConfig>().mushfolkAbility1) 
						{
                            if (MrPlagueRaces.RaceAbilityKeybind1.Current)
							{
								mushfolkPlayer.growingMushrooms = true;
								if (mushfolkPlayer.sporeless < 180) {
									mushfolkPlayer.sporeless++;
								}
								player.eyeHelper.BlinkBecausePlayerGotHurt();
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.SyncBlink(-1, Main.myPlayer);
                                }
                            }
							else
							{
								mushfolkPlayer.growingMushrooms = false;
							}
						}
						if (ModContent.GetInstance<MushfolkConfig>().mushfolkAbility2)
						{
							if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed)
							{
								Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), mrPlagueRacesPlayer.mouseWorld.X, mrPlagueRacesPlayer.mouseWorld.Y, 0f, 0f, ProjectileType<Nymphshroom>(), (int)((1f + (player.ConsumedLifeFruit * 0.075f)) * (1 + player.statLifeMax2 / 30)) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<MushfolkConfig>().mushfolkNymphshroomDamage]), 0, player.whoAmI);
								mushfolkPlayer.sporeless += 30;
							}
						}
					}
				}
			}
		}
		
		public override void PreUpdate(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var mushfolkPlayer = player.GetModPlayer<MushfolkPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead) {
					for (int i = 0; i < mushfolkPlayer.sporeless; i++) {
						if (Main.rand.Next(80) == 1) {
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y,  Main.rand.Next(3) - Main.rand.Next(3),  Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<BurstSpores>(), 0, 0, player.whoAmI);
						}
					}
					if (!mushfolkPlayer.growingMushrooms && mushfolkPlayer.sporeless > 0) {
						mushfolkPlayer.sporeless--;
					}
					if (mushfolkPlayer.sporeless >= 180) {
						if (!player.HasBuff(BuffType<Sporeless>())) {
							SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, player.Center);
                        }
						player.AddBuff(BuffType<Sporeless>(), 120);
						mushfolkPlayer.growingMushrooms = false;
					}
					if (mushfolkPlayer.growingMushrooms) {
						for (int i = 0; i < 3; i++) {
							if (Main.rand.Next(18) == 1) {
								Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), mrPlagueRacesPlayer.mouseWorld.X + Main.rand.Next(60) - Main.rand.Next(60), mrPlagueRacesPlayer.mouseWorld.Y + Main.rand.Next(60) - Main.rand.Next(60), 0f, 0f, ProjectileType<Trapshroom>(), (int)((1f + (player.ConsumedLifeFruit * 0.075f)) * (1 + player.statLifeMax2 / 30)) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<MushfolkConfig>().mushfolkTrapshroomDamage]), 0, player.whoAmI);
							}
						}
					}
				}
			}
        }

        public override void PostUpdate(Player player)
        {
            var mushfolkPlayer = player.GetModPlayer<MushfolkPlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					mushfolkPlayer.SyncPlayer(-1, Main.myPlayer, false);
				}

                var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
                mrPlagueRacesPlayer.SyncRotationsAndOffsets(-1, Main.myPlayer);
            }
        }
    }

	public class MushfolkPlayer : ModPlayer
	{
		public bool growingMushrooms = false;
		public int sporeless = 0;

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.MushfolkSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(growingMushrooms);
			packet.Write(sporeless);

            packet.Send(toWho, fromWho);
        }
    }

    public class MushfolkConfig : ModConfig
    {
        public static MushfolkConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Mushfolk")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> mushfolkStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%
            [RacialStatType.statLifeMax2] = RacialStatPercentageModifier.p10,
            [RacialStatType.allDamage] = RacialStatPercentageModifier.n10,
            [RacialStatType.moveSpeed] = RacialStatPercentageModifier.p4,
            [RacialStatType.endurance] = RacialStatPercentageModifier.n25
        };
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool mushfolkAbility1;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool mushfolkAbility2;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier mushfolkTrapshroomDamage;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier mushfolkNymphshroomDamage;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier mushfolkHealing;
    }
}