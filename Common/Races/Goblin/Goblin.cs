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
using MrPlagueRaces.Content.Prefixes;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.Races.Goblin
{
	public class Goblin : Race
	{
		public override void Load()
        {
			Description = "Known for their effective tinkering, Goblins forge their equipment with dark flames.";
            DisplayName = "[c/E2FF52:Goblin]";
            ClothStyle = 2;
			HairStyle = 15;
			StarterShirt = true;
			StarterPants = true;
			HairColor = new Color(85, 96, 123);
			SkinColor = new Color(182, 215, 126);
			DetailColor = new Color(182, 215, 126);
			EyeColor = new Color(105, 90, 75);
			ShirtColor = new Color(182, 91, 91);
			UnderShirtColor = new Color(166, 113, 93);
			PantsColor = new Color(175, 227, 255);
			ShoeColor = new Color(160, 105, 60);
        }

        public override void PreRaceChange(Player player) // Called before the player's race is changed
        {
            var goblinPlayer = player.GetModPlayer<GoblinPlayer>();
            goblinPlayer.harvesterCounter = 0;
        }

        public override void ResetEffects(Player player)
        {
            RegisterAbilityDescription(ModContent.GetInstance<GoblinConfig>().goblinAbility1, $"[c/4DBF60:+] Press Z to shadowforge your held item, granting it unique powers. Costs mana.");
            RegisterAbilityDescription(ModContent.GetInstance<GoblinConfig>().goblinAbility2, $"[c/4DBF60:+] Press X to unleash a shadowflame harvester, which burns enemies and grants you improved mana regeneration.");

            RaceStatDictionary = ModContent.GetInstance<GoblinConfig>().goblinStats;
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            var goblinPlayer = player.GetModPlayer<GoblinPlayer>();
            if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				/*player.moveSpeed += 0.1f;
				player.maxMinions += 1;
				player.maxTurrets += 1;
				player.statLifeMax2 -= (player.statLifeMax2 / 5);
				player.statManaMax2 += 40;*/
			}
        }

        public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            var goblinPlayer = player.GetModPlayer<GoblinPlayer>();
            goblinPlayer.harvesterCounter = 0;
        }

        public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var goblinPlayer = player.GetModPlayer<GoblinPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
                {
					if (ModContent.GetInstance<GoblinConfig>().goblinAbility1)
					{
						if (MrPlagueRaces.RaceAbilityKeybind1.JustPressed && goblinPlayer.IsEquipment(player.HeldItem) && player.HeldItem.stack == 1 && player.statMana >= 60)
                        {
                            goblinPlayer.ReloadPrefixes();
                            Item reforgeItem = player.inventory[player.selectedItem];
							reforgeItem.ResetPrefix();
							if (goblinPlayer.IsAccessory(player.HeldItem))
							{
								reforgeItem.Prefix(goblinPlayer.AccessoryPrefixes[Main.rand.Next(goblinPlayer.AccessoryPrefixes.Length)]);
							}
							if (goblinPlayer.IsWeapon(player.HeldItem))
							{
								reforgeItem.Prefix(goblinPlayer.AnyWeaponPrefixes[Main.rand.Next(goblinPlayer.AnyWeaponPrefixes.Length)]);
							}
							if (goblinPlayer.IsTool(player.HeldItem))
							{
								reforgeItem.Prefix(goblinPlayer.ToolPrefixes[Main.rand.Next(goblinPlayer.ToolPrefixes.Length)]);
							}
							reforgeItem.position.X = player.position.X + (float)(player.width / 2) - (float)(reforgeItem.width / 2);
							reforgeItem.position.Y = player.position.Y + (float)(player.height / 2) - (float)(reforgeItem.height / 2);
							PopupText.NewText(PopupTextContext.ItemReforge, reforgeItem, reforgeItem.stack, noStack: true);
							SoundEngine.PlaySound(SoundID.Item37);
							SoundEngine.PlaySound(SoundID.DD2_BookStaffCast);
							for (int i = 0; i < 140; i++)
							{
								if (Main.rand.Next(50) == 1)
								{
									Dust dust19 = Dust.NewDustDirect(new Vector2(player.position.X - 2f, player.position.Y - 2f), player.width + 4, player.height + 4, 27, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 180, default(Color), 1.95f);
									dust19.noGravity = true;
									dust19.velocity *= 0.75f;
									dust19.velocity.X *= 0.75f;
									dust19.velocity.Y -= 1f;
									if (Main.rand.Next(4) == 0)
									{
										dust19.noGravity = false;
										dust19.scale *= 0.5f;
									}
								}
							}
							player.statMana -= 60;
						}
                    }
					if (ModContent.GetInstance<GoblinConfig>().goblinAbility2)
					{
						if (MrPlagueRaces.RaceAbilityKeybind2.Current && !player.HasBuff(BuffType<Fathoming>()) && goblinPlayer.harvesterCounter == 0)
						{
							goblinPlayer.harvesterCounter = 30;
							SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryCircle, player.Center);
                            if (player.whoAmI == Main.myPlayer)
                            {
                                mrPlagueRacesPlayer.GoblinChargeSound(-1, Main.myPlayer);
                            }
                        }
						if (goblinPlayer.harvesterCounter > 0)
						{
							player.controlUseItem = false;
						}
					}
				}
			}
		}

		public override void PreUpdate(Player player) {
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var goblinPlayer = player.GetModPlayer<GoblinPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (goblinPlayer.harvesterCounter > 0) {
					goblinPlayer.harvesterCounter--;
					for (int i = 0; i < 30; i++) {
						Dust dust19 = Dust.NewDustDirect(new Vector2(player.Center.X + (player.direction == 1 ? 0 : -10), player.Center.Y - 30), 0, 0, 27, 0f, player.velocity.Y * 0.4f, 180, default(Color), 1.95f);
						dust19.noGravity = true;
						dust19.velocity *= 0.75f;
						dust19.velocity.X *= 0.75f;
						dust19.velocity.Y -= 1f;
						if (Main.rand.Next(4) == 0)
						{
							dust19.noGravity = false;
							dust19.scale *= 0.5f;
						}
                    }
                    if (player.whoAmI == Main.myPlayer)
                    {
                        mrPlagueRacesPlayer.GoblinHarvesterFireballDust(-1, Main.myPlayer);
                    }
                }
				if (goblinPlayer.harvesterCounter == 1) {
					Vector2 velocity = Vector2.Normalize(mrPlagueRacesPlayer.mouseWorld - player.Center) * 10f;
					Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X + (player.direction == 1 ? 0 : -10), player.Center.Y - 30, velocity.X, velocity.Y, ProjectileType<ShadowflameHarvester>(), (int)((1f + (player.ConsumedLifeFruit * 0.075f)) * (player.statLifeMax2 / 25)) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<GoblinConfig>().goblinShadowHarvesterDamage]), 5, player.whoAmI);
					SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, player.Center);
                    if (player.whoAmI == Main.myPlayer)
                    {
                        mrPlagueRacesPlayer.GoblinShootSound(-1, Main.myPlayer);
                    }
                    player.AddBuff(BuffType<Fathoming>(), 240);
					for (int i = 0; i < 30; i++) {
						int dust = Dust.NewDust(new Vector2(player.Center.X + (player.direction == 1 ? 0 : -10), player.Center.Y - 30), 0, 0, 27);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 3f;
						dust = Dust.NewDust(new Vector2(player.Center.X + (player.direction == 1 ? 0 : -10), player.Center.Y - 30), 0, 0, 27);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 2f;
                    }
                    if (player.whoAmI == Main.myPlayer)
                    {
                        mrPlagueRacesPlayer.GoblinHarvesterShootDust(-1, Main.myPlayer);
                    }
                }
			}
		}

		public override void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo) {
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var goblinPlayer = player.GetModPlayer<GoblinPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (goblinPlayer.harvesterCounter > 0) {
					player.bodyFrame.Y = player.bodyFrame.Height * 5;
				}
			}
        }

        public override void PostUpdate(Player player)
        {
            var goblinPlayer = player.GetModPlayer<GoblinPlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
                var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
                mrPlagueRacesPlayer.SyncRotationsAndOffsets(-1, Main.myPlayer);
            }
        }
    }

	public class GoblinPlayer : ModPlayer
    {
		public int[] AccessoryPrefixes = { PrefixType<Combustible>(), PrefixType<Constructive>(), PrefixType<Flawless>(), PrefixType<Impactful>(), PrefixType<Hexed>(), PrefixType<Luminescent>(), PrefixType<Regenerative>(), PrefixType<Reinforced>(), PrefixType<Resilient>(), PrefixType<Streamlined>(), PrefixType<Undying>(), PrefixType<Volatile>() };
		public int[] AnyWeaponPrefixes = { PrefixType<Accelerative>(), PrefixType<Bewitched>(), PrefixType<Bombarding>(), PrefixType<Explosive>(), PrefixType<Immolating>(), PrefixType<Revitalizing>(), PrefixType<Warping>() };
		public int[] ToolPrefixes = { PrefixType<Fortunate>(), PrefixType<Recreational>(), PrefixType<Trailblazing>(), PrefixType<Tranquilizing>() };

		public int harvesterCounter = 0;

		public void ReloadPrefixes()
		{
            var accessoryList = new List<int>();
            var anyWeaponList = new List<int>();
            var toolList = new List<int>();

            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Combustible)) 
			{
				accessoryList.Add(PrefixType<Combustible>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Constructive))
            {
                accessoryList.Add(PrefixType<Constructive>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Flawless))
            {
                accessoryList.Add(PrefixType<Flawless>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Impactful))
            {
                accessoryList.Add(PrefixType<Impactful>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Hexed))
            {
                accessoryList.Add(PrefixType<Hexed>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Luminescent))
            {
                accessoryList.Add(PrefixType<Luminescent>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Regenerative))
            {
                accessoryList.Add(PrefixType<Regenerative>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Reinforced))
            {
                accessoryList.Add(PrefixType<Reinforced>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Resilient))
            {
                accessoryList.Add(PrefixType<Resilient>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Streamlined))
            {
                accessoryList.Add(PrefixType<Streamlined>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Undying))
            {
                accessoryList.Add(PrefixType<Undying>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Volatile))
            {
                accessoryList.Add(PrefixType<Volatile>());
            }


            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Accelerative))
            {
                anyWeaponList.Add(PrefixType<Accelerative>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Bewitched))
            {
                anyWeaponList.Add(PrefixType<Bewitched>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Bombarding))
            {
                anyWeaponList.Add(PrefixType<Bombarding>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Explosive))
            {
                anyWeaponList.Add(PrefixType<Explosive>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Immolating))
            {
                anyWeaponList.Add(PrefixType<Immolating>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Revitalizing))
            {
                anyWeaponList.Add(PrefixType<Revitalizing>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Warping))
            {
                anyWeaponList.Add(PrefixType<Warping>());
            }

            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Fortunate))
            {
                toolList.Add(PrefixType<Fortunate>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Recreational))
            {
                toolList.Add(PrefixType<Recreational>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Trailblazing))
            {
                toolList.Add(PrefixType<Trailblazing>());
            }
            if (ModContent.GetInstance<GoblinConfig>().goblinPrefixes.ContainsKey(GoblinPrefixType.Tranquilizing))
            {
                toolList.Add(PrefixType<Tranquilizing>());
            }

            if (accessoryList?.Count == 0)
            {
                accessoryList.Add(0);
            }
            if (anyWeaponList?.Count == 0)
            {
                anyWeaponList.Add(0);
            }
            if (toolList?.Count == 0)
            {
                toolList.Add(0);
            }

            AccessoryPrefixes = accessoryList.ToArray();
            AnyWeaponPrefixes = anyWeaponList.ToArray();
            ToolPrefixes = toolList.ToArray();
        }

		public bool IsAccessory(Item Item)
		{
			return (Item.accessory == true);
		}
		public bool IsTool(Item Item)
		{
			return (Item.pick > 0 || Item.axe > 0 || Item.hammer > 0);
		}

		public bool IsWeapon(Item Item)
		{
			return (Item.damage > 0);
		}
		public bool IsEquipment(Item Item)
		{
			return ((IsAccessory(Item) || IsTool(Item) || IsWeapon(Item)));
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.GoblinSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(harvesterCounter);
		}
    }

    public enum GoblinPrefixType
    {
        Accelerative,
        Bewitched,
        Bombarding,
        Combustible,
		Constructive,
        Explosive,
        Flawless,
        Fortunate,
        Hexed,
        Immolating,
        Impactful,
        Luminescent,
        Recreational,
        Regenerative,
        Reinforced,
        Resilient,
        Revitalizing,
        Streamlined,
        Trailblazing,
        Tranquilizing,
        Undying,
        Volatile,
        Warping
    }

    public class GoblinConfig : ModConfig
    {
        public static GoblinConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Goblin")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> goblinStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%
            [RacialStatType.statLifeMax2] = RacialStatPercentageModifier.n20,
            [RacialStatType.statManaMax2] = RacialStatPercentageModifier.p40,
            [RacialStatType.moveSpeed] = RacialStatPercentageModifier.p10,
            [RacialStatType.maxMinions] = RacialStatPercentageModifier.p1,
            [RacialStatType.maxTurrets] = RacialStatPercentageModifier.p1
        };
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool goblinAbility1;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool goblinAbility2;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier goblinShadowHarvesterDamage;
        [BackgroundColor(110, 141, 255)]
        public Dictionary<GoblinPrefixType, RacialStatPercentageModifier> goblinPrefixes = new Dictionary<GoblinPrefixType, RacialStatPercentageModifier>()
        {
            [GoblinPrefixType.Combustible] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Constructive] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Flawless] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Hexed] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Impactful] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Luminescent] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Regenerative] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Reinforced] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Resilient] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Streamlined] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Undying] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Volatile] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Accelerative] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Bewitched] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Bombarding] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Explosive] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Immolating] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Revitalizing] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Warping] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Fortunate] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Recreational] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Trailblazing] = RacialStatPercentageModifier.zero,
            [GoblinPrefixType.Tranquilizing] = RacialStatPercentageModifier.zero,
        };
    }
}