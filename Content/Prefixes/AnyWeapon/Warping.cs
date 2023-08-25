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
	public class Warping : ModPrefix
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

	public class WarpingItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			int index = tooltips.FindIndex(tt => tt.Name.Equals("PrefixDamage") && tt.Mod == "Terraria");
			if (item.prefix == PrefixType<Warping>())
			{
				var linePower = new TooltipLine(Mod, "Warping", "You teleport to slain opponents and generate a shockwave")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				tooltips.Insert(index, linePower);
			}
		}
	}

	public class WarpingPlayer : ModPlayer
	{
		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Projectile, consider using OnHitNPC instead */
		{
			if (Player.HeldItem.prefix == PrefixType<Warping>() && !target.active) {
				Player.Teleport(target.position, 15);
				SoundEngine.PlaySound(SoundID.NPCDeath55, Player.Center);
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Player.Center.X + Main.rand.Next(15) - Main.rand.Next(15), Player.Center.Y + Main.rand.Next(15) - Main.rand.Next(15), Player.direction, 0, ProjectileType<ImpactExplosion>(), Player.HeldItem.damage, 10, Player.whoAmI);
			}
		}
		public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Item, consider using OnHitNPC instead */
		{
			if (Player.HeldItem.prefix == PrefixType<Warping>() && !target.active) {
				Player.Teleport(target.position, 15);
				SoundEngine.PlaySound(SoundID.NPCDeath55, Player.Center);
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Player.Center.X + Main.rand.Next(15) - Main.rand.Next(15), Player.Center.Y + Main.rand.Next(15) - Main.rand.Next(15), Player.direction, 0, ProjectileType<ImpactExplosion>(), Player.HeldItem.damage, 10, Player.whoAmI);
			}
		}
		/*public override void OnHitPvp(Item item, Player target, int damage, bool crit) tModPorter Note: Removed. Use OnHurt on the receiving player and check info.PvP. Use info.DamageSource.SourcePlayerIndex to get the attacking player 
		{
			if (Player.HeldItem.prefix == PrefixType<Warping>() && !target.active) {
				Player.Teleport(target.position, 15);
				SoundEngine.PlaySound(SoundID.NPCDeath55, Player.Center);
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Player.Center.X + Main.rand.Next(15) - Main.rand.Next(15), Player.Center.Y + Main.rand.Next(15) - Main.rand.Next(15), Player.direction, 0, ProjectileType<ImpactExplosion>(), Player.HeldItem.damage, 10, Player.whoAmI);
			}
		}*/
	}
}