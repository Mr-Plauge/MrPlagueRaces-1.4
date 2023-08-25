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
	public class Volatile : ModPrefix
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

	public class VolatileItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.prefix == PrefixType<Volatile>())
			{
				var linePower = new TooltipLine(Mod, "Volatile", "You generate an explosion when attacked")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				var lineStat = new TooltipLine(Mod, "Volatile", "+2% damage")
				{
					IsModifier = true
				};
				tooltips.Add(linePower);
				tooltips.Add(lineStat);
			}
		}
	}

	public class VolatilePlayer : ModPlayer
	{
		int volatileFactor;

		public override void ResetEffects() {
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Volatile>())
				{
					Player.GetDamage(DamageClass.Generic) += 0.02f;
				}
			}
		}

		public override void PostHurt(Player.HurtInfo info)
		{
			volatileFactor = 0;
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Volatile>())
				{
					volatileFactor += 1;
				}
			}
			if (volatileFactor > 0) {
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Player.Center.X + Main.rand.Next(15) - Main.rand.Next(15), Player.Center.Y + Main.rand.Next(15) - Main.rand.Next(15), 0, 0, ProjectileType<VolatileExplosion>(), (Player.statLifeMax2 / 15) * volatileFactor, 5, Player.whoAmI);
			}
		}
	}
}