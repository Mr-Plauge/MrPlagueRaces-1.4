using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Content.Projectiles;
using MrPlagueRaces.Content.Prefixes;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Prefixes
{
	public class Fortunate : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override bool CanRoll(Item item) {
			return false;
		}

		public override void ModifyValue(ref float valueMult) {
			valueMult *= 1.75f;
		}

		public override void Apply(Item item)
		{
			if (item.rare > -12) {
				if (item.rare >= ItemRarityID.LightRed) {
					item.rare = ItemRarityID.Cyan;
				}
				else {
					item.rare = ItemRarityID.LightRed;
				}
			}
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
		{
			critBonus = 1;
			useTimeMult = 0.9f;
		}
	}

	public class FortunateItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			int index = tooltips.FindIndex(tt => tt.Name.Equals("PrefixSpeed") && tt.Mod == "Terraria");
			if (item.prefix == PrefixType<Fortunate>())
			{
				var linePower = new TooltipLine(Mod, "Fortunate", "You have a chance to unearth treasures when digging")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				tooltips.Insert(index, linePower);
			}
		}

		public override bool? UseItem(Item item, Player player)
		{
			var fortunatePlayer = player.GetModPlayer<FortunatePlayer>();
			if (item.prefix == PrefixType<Fortunate>())
			{
				fortunatePlayer.usingItem = true;
			}
			return base.UseItem(item, player);
		}

		public override void UseAnimation(Item item, Player player)
		{
			var fortunatePlayer = player.GetModPlayer<FortunatePlayer>();
			if (item.prefix == PrefixType<Fortunate>())
			{
				fortunatePlayer.animating = true;
			}
		}
	}

	public class FortunatePlayer : ModPlayer
	{
		public bool usingItem = false;
		public bool animating = false;
		
		public override void ResetEffects() {
			usingItem = false;
			animating = false;
		}

		public override void PreUpdate() {
			if (usingItem == false && animating == true) {
				switch (Main.rand.Next(8))
				{
					case 0:
						if (Main.rand.Next(2) == 1) {
							Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.CopperCoin), ItemID.CopperCoin, 1);
							SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Player.Center);
						}
						break;
					case 1:
						if (Main.rand.Next(5) == 1) {
							Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.SilverCoin), ItemID.SilverCoin, 1);
							SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Player.Center);
						}
						break;
					case 2:
						if (Main.rand.Next(25) == 1) {
							Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.GoldCoin), ItemID.GoldCoin, 1);
							SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Player.Center);
						}
						break;
					case 3:
						if (Main.rand.Next(4) == 1) {
							switch (Main.rand.Next(6))
							{
								case 0:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.Amethyst), ItemID.Amethyst, 1);
									break;
								case 1:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.Topaz), ItemID.Topaz, 1);
									break;
								case 2:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.Sapphire), ItemID.Sapphire, 1);
									break;
								case 3:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.Emerald), ItemID.Emerald, 1);
									break;
								case 4:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.Ruby), ItemID.Ruby, 1);
									break;
								default:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.Diamond), ItemID.Diamond, 1);
									break;
							}
							SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Player.Center);
						}
						break;
					case 4:
						if (Main.rand.Next(2) == 1) {
							switch (Main.rand.Next(1))
							{
								case 0:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.CopperOre), ItemID.CopperOre, 1);
									break;
								default:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.TinOre), ItemID.TinOre, 1);
									break;
							}
							SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Player.Center);
						}
						break;
					case 5:
						if (Main.rand.Next(4) == 1) {
							switch (Main.rand.Next(1))
							{
								case 0:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.IronOre), ItemID.IronOre, 1);
									break;
								default:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.LeadOre), ItemID.LeadOre, 1);
									break;
							}
							SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Player.Center);
						}
						break;
					case 6:
						if (Main.rand.Next(6) == 1) {
							switch (Main.rand.Next(1))
							{
								case 0:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.SilverOre), ItemID.SilverOre, 1);
									break;
								default:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.TungstenOre), ItemID.TungstenOre, 1);
									break;
							}
							SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Player.Center);
						}
						break;
					default:
						if (Main.rand.Next(8) == 1) {
							switch (Main.rand.Next(1))
							{
								case 0:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.GoldOre), ItemID.GoldOre, 1);
									break;
								default:
									Player.QuickSpawnItem(Player.GetSource_OpenItem(ItemID.PlatinumOre), ItemID.PlatinumOre, 1);
									break;
							}
							SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Player.Center);
						}
						break;
				}
			}
		}
	}
}