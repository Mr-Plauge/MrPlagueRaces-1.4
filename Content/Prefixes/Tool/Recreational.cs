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
	public class Recreational : ModPrefix
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

	public class RecreationalItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			int index = tooltips.FindIndex(tt => tt.Name.Equals("PrefixSpeed") && tt.Mod == "Terraria");
			if (item.prefix == PrefixType<Recreational>())
			{
				var linePower = new TooltipLine(Mod, "Recreational", "Digging restores your health and mana")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				tooltips.Insert(index, linePower);
			}
		}

		public override bool? UseItem(Item item, Player player)
		{
			var recreationalPlayer = player.GetModPlayer<RecreationalPlayer>();
			if (item.prefix == PrefixType<Recreational>())
			{
				recreationalPlayer.usingItem = true;
			}
			return base.UseItem(item, player);
		}

		public override void UseAnimation(Item item, Player player)
		{
			var recreationalPlayer = player.GetModPlayer<RecreationalPlayer>();
			if (item.prefix == PrefixType<Recreational>())
			{
				recreationalPlayer.animating = true;
			}
		}
	}

	public class RecreationalPlayer : ModPlayer
	{
		public bool usingItem = false;
		public bool animating = false;
		
		public override void ResetEffects() {
			usingItem = false;
			animating = false;
		}
		
		public override void PreUpdate() {
			if (usingItem == false && animating == true) {
				Player.HealEffect(1);
				Player.ManaEffect(2);
				Player.statLife += 1;
				Player.statMana += 2;
				SoundEngine.PlaySound(SoundID.Item154, Player.Center);
			}
		}
	}
}