﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class BreathSmoke : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Breath Smoke");
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults() {
			Projectile.width = 28;
			Projectile.height = 28;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 30;
			Projectile.penetrate = -1;
			Projectile.alpha = 100;
			Projectile.frame = Main.rand.Next(4);
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			Projectile.rotation += Projectile.velocity.X * 0.02f;
			Projectile.scale += 0.2f;
			Projectile.alpha += 10;
			if (Projectile.alpha >= 255) {
				Projectile.Kill();
			}
		}

		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = Projectile.GetAlpha(lightColor);
			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
