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
	public class Constructive : ModPrefix
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

	public class ConstructiveItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.prefix == PrefixType<Constructive>())
			{
				var linePower = new TooltipLine(Mod, "Constructive", "Mining speed and placement speed are increased")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				var lineStat = new TooltipLine(Mod, "Constructive", "+2% movement speed")
				{
					IsModifier = true
				};
				tooltips.Add(linePower);
				tooltips.Add(lineStat);
			}
		}
	}

	public class ConstructivePlayer : ModPlayer
	{
		public override void ResetEffects() {
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Constructive>())
				{
					Player.pickSpeed -= 0.25f;
					Player.wallSpeed += 0.25f;
					Player.tileSpeed += 0.25f;
					Player.moveSpeed += 0.02f;
				}
			}
		}
	}
}