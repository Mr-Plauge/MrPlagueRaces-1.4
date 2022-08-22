using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class Trapshroom : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Trapshroom");
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults() {
			Player player = Main.player[Projectile.owner];
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Projectile.timeLeft = 900;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			Projectile.ai[0] += 1f;
			Projectile.frameCounter++;
			if (Projectile.alpha > 0) {
				Projectile.alpha -= 15;
			}
			if (Projectile.alpha < 0) {
				Projectile.alpha = 0;
			}
			Projectile.velocity = new Vector2(0f, (float)Math.Sin((double)((float)Math.PI * 2f * Projectile.ai[0] / 180f)) * 0.15f);
			if (Projectile.frameCounter > 4)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame > 3)
				{
					Projectile.frame = 0;
				}
			}
			if (Projectile.ai[0] >= 180f)
			{
				Projectile.ai[0] = 0f;
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Player player = Main.player[Projectile.owner];
			for (int i = 0; i < 9; i++) {
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X + Main.rand.Next(60) - Main.rand.Next(60), Projectile.Center.Y + Main.rand.Next(60) - Main.rand.Next(60), 0f, 0f, ProjectileType<HealingSpores>(), 0, 0, Projectile.owner);
			}
			Projectile.Kill();
		}

		public override void Kill(int timeLeft) {
			for (int i = 0; i < 6; i++) {
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y,  Main.rand.Next(3) - Main.rand.Next(3),  Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<BurstSpores>(), 0, 0, Projectile.owner);
			}
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
		}

		public override bool PreDraw(ref Color lightColor) {
			Player player = Main.player[Projectile.owner];
			Texture2D textureHair = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/Trapshroom_Hair");
			Texture2D textureSkin = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/Trapshroom_Skin");

			int frameHeight = textureHair.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, textureHair.Width, frameHeight);

			Vector2 origin = sourceRectangle.Size() / 2f;
			Main.EntitySpriteDraw(textureHair,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, new Color(player.hairColor.ToVector4() * Projectile.Opacity), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(textureSkin,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, new Color(player.skinColor.ToVector4() * lightColor.ToVector4() * Projectile.Opacity), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
