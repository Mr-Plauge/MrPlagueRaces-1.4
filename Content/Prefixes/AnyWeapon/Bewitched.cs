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
	public class Bewitched : ModPrefix
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
			damageMult = 1.1f;
			critBonus = 2;
		}
	}

	public class BewitchedItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			int index = tooltips.FindIndex(tt => tt.Name.Equals("PrefixDamage") && tt.Mod == "Terraria");
			if (item.prefix == PrefixType<Bewitched>())
			{
				var linePower = new TooltipLine(Mod, "Bewitched", "Attacking releases shadowflame seekers")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				tooltips.Insert(index, linePower);
			}
		}

		public override bool? UseItem(Item item, Player player)
		{
			if (item.prefix == PrefixType<Bewitched>() && !player.HasBuff(BuffType<Invoked>()))
			{
				Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * 10f;
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, velocity.X, velocity.Y, ProjectileType<ShadowflameSeeker>(), item.damage / 2, 5, player.whoAmI);
				SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, player.Center);
				player.AddBuff(BuffType<Invoked>(), 40);
			}
			return base.UseItem(item, player);
		}
	}

	public class BewitchedPlayer : ModPlayer
	{

	}
}