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
	public class Hexed : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Accessory;

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
	}

	public class HexedItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.prefix == PrefixType<Hexed>())
			{
				var linePower = new TooltipLine(Mod, "Hexed", "You summon shadowflame spectres when attacked")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				var lineStat = new TooltipLine(Mod, "Hexed", "+2 defense")
				{
					IsModifier = true
				};
				tooltips.Add(linePower);
				tooltips.Add(lineStat);
			}
		}
	}

	public class HexedPlayer : ModPlayer
	{
		public override void ResetEffects() {
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Hexed>())
				{
					Player.statDefense += 2;
				}
			}
		}

		public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
		{
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Hexed>())
				{
					Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Player.Center.X, Player.Center.Y, 4 * Player.direction, Main.rand.Next(4) - Main.rand.Next(4), ProjectileType<ShadowflameSeeker>(), Player.statLifeMax2 / 25, 5, Player.whoAmI);
					SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Player.Center);
				}
			}
		}
	}
}