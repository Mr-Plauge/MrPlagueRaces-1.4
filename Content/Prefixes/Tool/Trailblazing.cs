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
	public class Trailblazing : ModPrefix
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
			critBonus = 2;
			useTimeMult = 0.85f;
		}
	}

	public class TrailblazingItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			int index = tooltips.FindIndex(tt => tt.Name.Equals("PrefixSpeed") && tt.Mod == "Terraria");
			if (item.prefix == PrefixType<Trailblazing>())
			{
				var linePower = new TooltipLine(Mod, "Trailblazing", "Using this tool creates a beam of light ahead of you")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				tooltips.Insert(index, linePower);
			}
		}

		public override void UseAnimation(Item item, Player player)
		{
			var trailblazingPlayer = player.GetModPlayer<TrailblazingPlayer>();
			if (item.prefix == PrefixType<Trailblazing>())
			{
				trailblazingPlayer.duration = 30;
			}
		}
	}

	public class TrailblazingPlayer : ModPlayer
	{
		public int duration;
		public override void PreUpdate() {
			if (duration > 0) {
				for (int i = 0; i < 6; i++) {
					Lighting.AddLight(new Vector2(Player.Center.X + ((i * 45) * Player.direction), Player.Center.Y), new Color(162, 96, 195).ToVector3() * 1.25f);
				}
				duration--;
			}
		}
	}
}