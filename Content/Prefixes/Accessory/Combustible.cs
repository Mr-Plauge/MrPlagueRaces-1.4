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
	public class Combustible : ModPrefix
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

	public class CombustibleItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.prefix == PrefixType<Combustible>())
			{
				var linePower = new TooltipLine(Mod, "Combustible", "Nearby enemies are set on fire")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				var lineStat = new TooltipLine(Mod, "Combustible", "+3% movement speed")
				{
					IsModifier = true
				};
				tooltips.Add(linePower);
				tooltips.Add(lineStat);
			}
		}
	}

	public class CombustiblePlayer : ModPlayer
	{
		float combustibleFactor;

		public override void ResetEffects() {
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Impactful>())
				{
					Player.moveSpeed += 0.03f;
				}
			}
		}

		public override void PreUpdate() {
			combustibleFactor = 0f;
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Combustible>())
				{
					combustibleFactor += 60f;
				}
			}
			float sqrcombustibleFactor = combustibleFactor * combustibleFactor;

			for (int k = 0; k < Main.maxNPCs; k++) {
				NPC target = Main.npc[k];
				if (target.CanBeChasedBy() && !target.HasBuff(BuffType<DarkInferno>())) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Player.Center);

					if (sqrDistanceToTarget < sqrcombustibleFactor) {
						sqrcombustibleFactor = sqrDistanceToTarget;
						target.AddBuff(BuffType<DarkInferno>(), 60);
					}
				}
			}
		}
	}
}