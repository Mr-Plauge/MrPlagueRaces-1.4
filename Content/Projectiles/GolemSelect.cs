using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Common.Races.Lihzahrd;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class GolemSelect : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Golem Select");
			Main.projFrames[Projectile.type] = 6;
		}

		public override void SetDefaults() {
			Projectile.width = 52;
			Projectile.height = 52;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ownerHitCheck = true;
			Projectile.alpha = 100;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * Projectile.Opacity;
		}

		public override void AI() {
			Projectile.timeLeft++;
			Player player = Main.player[Projectile.owner];
			var lihzahrdPlayer = player.GetModPlayer<LihzahrdPlayer>();
			if (Main.myPlayer == Projectile.owner)
			{
				Projectile.position = new Vector2(Main.MouseWorld.X - 22, Main.MouseWorld.Y - 46).Floor();
			}
			Projectile.frame = lihzahrdPlayer.selectedGolem;
			if (lihzahrdPlayer.closeMenu) {
				Projectile.Kill();
				lihzahrdPlayer.closeMenu = false;
			}
			Projectile.direction = Projectile.spriteDirection = lihzahrdPlayer.direction == 1 ? 1 : -1;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}
	}
}
