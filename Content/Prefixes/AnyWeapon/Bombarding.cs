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
	public class Bombarding : ModPrefix
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

	public class BombardingItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			int index = tooltips.FindIndex(tt => tt.Name.Equals("PrefixDamage") && tt.Mod == "Terraria");
			if (item.prefix == PrefixType<Bombarding>())
			{
				var linePower = new TooltipLine(Mod, "Bombarding", "Attacking unleashes a volley of lingering flames")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				tooltips.Insert(index, linePower);
			}
		}

		public override bool? UseItem(Item item, Player player)
		{
			if (item.prefix == PrefixType<Bombarding>() && !player.HasBuff(BuffType<Bombarded>()))
			{
				Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * 10f;
				for (int i = 0; i < 5; i++) {
					Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, velocity.X + Main.rand.Next(6) - Main.rand.Next(6), velocity.Y + Main.rand.Next(6) - Main.rand.Next(6), ProjectileType<BombardingFlame>(), item.damage / 2, 5, player.whoAmI);
				}
				SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash, player.Center);
				player.AddBuff(BuffType<Bombarded>(), 40);
			}
			return base.UseItem(item, player);
		}
	}

	public class BombardingPlayer : ModPlayer
	{
		
	}
}