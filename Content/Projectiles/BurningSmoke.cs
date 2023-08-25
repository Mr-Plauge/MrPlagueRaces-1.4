using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MrPlagueRaces.Content.Projectiles
{
	public class BurningSmoke : ModProjectile
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Burning Smoke");
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults() {
			Projectile.width = 28;
			Projectile.height = 28;
			Projectile.scale = Main.rand.Next(2) + 1;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Projectile.frame = Main.rand.Next(4);
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			Projectile.ai[0] += 1f;
			if (Projectile.alpha > 200 && Projectile.ai[0] < 60f) 
			{
				Projectile.alpha--;
			}
			if (Projectile.alpha < 255 && Projectile.ai[0] >= 360f)
			{
				Projectile.alpha += 3;
			}
			if (Projectile.ai[0] >= 400f || Projectile.alpha >= 255)
			{
				Projectile.Kill();
			}
			if (Projectile.ai[0] <= 200f) {
				Projectile.scale += 0.01f;
			}
			Projectile.frameCounter++;
			if (Projectile.frameCounter > 4)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame > 3)
				{
					Projectile.frame = 0;
				}
			}
			if (Main.rand.Next(800) == 1 && Projectile.alpha <= 230) {
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6);
			}
		}

		public override bool? CanDamage()
		{
			return Projectile.alpha <= 230;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Dust.NewDust(target.position, target.width, target.height, 6);
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
