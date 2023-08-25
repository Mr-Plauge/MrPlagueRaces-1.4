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
	public class Impactful : ModPrefix
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

	public class ImpactfulItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.prefix == PrefixType<Impactful>())
			{
				var linePower = new TooltipLine(Mod, "Impactful", "Landing creates a shockwave that damages and knocks back nearby enemies")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				var lineStat = new TooltipLine(Mod, "Impactful", "+1% damage")
				{
					IsModifier = true
				};
				tooltips.Add(linePower);
				tooltips.Add(lineStat);
			}
		}
	}

	public class ImpactfulPlayer : ModPlayer
	{
		int impactfulFactor;
		float lastVelocityY;

		public override void ResetEffects() {
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Combustible>())
				{
					Player.GetDamage(DamageClass.Generic) += 0.01f;
				}
			}
		}

		public override void PreUpdate() {
			impactfulFactor = 0;
			if (Player.velocity.Y != 0) {
				lastVelocityY = Player.velocity.Y;
			}
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Impactful>())
				{
					impactfulFactor += 1;
				}
				if (lastVelocityY > 0 && Player.velocity.Y == 0 && impactfulFactor > 0) {
					Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Player.Center.X + Main.rand.Next(15) - Main.rand.Next(15), Player.Center.Y + Main.rand.Next(15) - Main.rand.Next(15), Player.direction, 0, ProjectileType<ImpactExplosion>(), (Player.statLifeMax2 / 15) * impactfulFactor, 10, Player.whoAmI);
					lastVelocityY = Player.velocity.Y;
				}
			}
		}
	}
}