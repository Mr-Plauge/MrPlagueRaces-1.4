﻿using Microsoft.Xna.Framework;
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
	public class Revitalizing : ModPrefix
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
			damageMult = 1.08f;
			critBonus = 5;
		}
	}

	public class RevitalizingItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			int index = tooltips.FindIndex(tt => tt.Name.Equals("PrefixDamage") && tt.Mod == "Terraria");
			if (item.prefix == PrefixType<Revitalizing>())
			{
				var linePower = new TooltipLine(Mod, "Revitalizing", "Killing enemies restores health and regenerates you")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				tooltips.Insert(index, linePower);
			}
		}
	}

	public class RevitalizingPlayer : ModPlayer
	{
		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (Player.HeldItem.prefix == PrefixType<Revitalizing>() && !target.active) {
				Player.AddBuff(BuffType<RevitalizingBuff>(), 60);
				Player.HealEffect(Player.statLifeMax2 / 100);
				Player.statLife += Player.statLifeMax2 / 100;
			}
		}
		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (Player.HeldItem.prefix == PrefixType<Revitalizing>() && !target.active) {
				Player.AddBuff(BuffType<RevitalizingBuff>(), 60);
				Player.HealEffect(Player.statLifeMax2 / 100);
				Player.statLife += Player.statLifeMax2 / 100;
			}
		}
		public override void OnHitPvp(Item item, Player target, int damage, bool crit)
		{
			if (Player.HeldItem.prefix == PrefixType<Revitalizing>() && !target.active) {
				Player.AddBuff(BuffType<RevitalizingBuff>(), 60);
				Player.HealEffect(Player.statLifeMax2 / 100);
				Player.statLife += Player.statLifeMax2 / 100;
			}
		}
	}
}