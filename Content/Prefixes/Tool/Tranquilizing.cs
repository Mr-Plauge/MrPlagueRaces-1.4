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
	public class Tranquilizing : ModPrefix
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
			critBonus = 3;
			useTimeMult = 0.95f;
		}
	}

	public class TranquilizingItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			int index = tooltips.FindIndex(tt => tt.Name.Equals("PrefixSpeed") && tt.Mod == "Terraria");
			if (item.prefix == PrefixType<Tranquilizing>())
			{
				var linePower = new TooltipLine(Mod, "Tranquilizing", "Hurting enemies temporarily stuns them")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				tooltips.Insert(index, linePower);
			}
		}
	}

	public class TranquilizingPlayer : ModPlayer
	{
		public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Item, consider using OnHitNPC instead */
		{
			if (Player.HeldItem.prefix == PrefixType<Tranquilizing>()) {
				target.AddBuff(BuffType<Tranquilized>(), 60);
			}
		}
		/*public override void OnHitPvp(Item item, Player target, int damage, bool crit)
		{
			if (Player.HeldItem.prefix == PrefixType<Tranquilizing>()) {
				target.AddBuff(BuffType<Tranquilized>(), 60);
			}
		}*/
	}
}