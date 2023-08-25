using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class HealingSpores : ModProjectile
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Healing Spores");
			Main.projFrames[Projectile.type] = 5;
		}

		public override void SetDefaults() {
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.scale = Main.rand.Next(2) + 1;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Projectile.frame = Main.rand.Next(5);
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
			if (Projectile.frameCounter > 5)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame > 4)
				{
					Projectile.frame = 0;
				}
			}
			if (Projectile.alpha <= 230) {
				for (int i = 0; i < Main.player.Length; i++)
				{
					Player playerAny = Main.player[i];
					if (!playerAny.dead && Projectile.getRect().Intersects(playerAny.getRect()))
					{
						playerAny.AddBuff(BuffType<Mycobuffer>(), 180);
					}
				}
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			return new Color(player.hairColor.ToVector4() * lightColor.ToVector4() * Projectile.Opacity);
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
