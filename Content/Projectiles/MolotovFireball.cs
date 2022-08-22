using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class MolotovFireball : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Molotov Fireball");
		}

		public override void SetDefaults() {
			Projectile.width = 32;
			Projectile.height = 18;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
		}

		public override void AI() {
			Projectile.rotation = Projectile.velocity.ToRotation();
			Player player = Main.player[Projectile.owner];
			Projectile.ai[0]++;
			Projectile.velocity.Y += 0.5f;
			if (Projectile.alpha > 200 && Projectile.ai[0] < 60f) 
			{
				Projectile.alpha--;
			}
			if (Projectile.alpha < 255)
			{
				Projectile.alpha += 3;
			}
			if (Projectile.ai[0] >= 200f || Projectile.alpha >= 255)
			{
				Projectile.Kill();
			}
			if (Main.rand.Next(3) == 1) {
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6);
			}
		}

		public override bool? CanDamage()
		{
			return Projectile.alpha <= 230;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			return new Color(255, 255, 255, 0);
		}

		public override void Kill(int timeLeft)
		{
			Player player = Main.player[Projectile.owner];
			SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
			for (int i = 0; i < 15; i++) {
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6);
				Main.dust[dust].velocity *= 5;
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y,  Main.rand.Next(6) - Main.rand.Next(6),  Main.rand.Next(6) - Main.rand.Next(6), ProjectileType<MolotovFlame>(), 1 + (player.statDefense * 4), 0, Projectile.owner);
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Player player = Main.player[Projectile.owner];
			for (int i = 0; i < 100; i++) {
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6);
			}
			Projectile.Kill();
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
