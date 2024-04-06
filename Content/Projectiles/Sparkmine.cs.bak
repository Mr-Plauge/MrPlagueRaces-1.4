using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Common.Races.Kobold;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class Sparkmine : ModProjectile
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Sparkmine");
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults() {
			Player player = Main.player[Projectile.owner];
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Projectile.timeLeft = 900;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			var koboldPlayer = player.GetModPlayer<KoboldPlayer>();
			Projectile.ai[0] += 1f;
			Projectile.frameCounter++;
			if (Projectile.alpha > 50) {
				Projectile.alpha -= 15;
			}
			if (Projectile.alpha < 50) {
				Projectile.alpha = 50;
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
			if (koboldPlayer.triggeringMine > 0 && ((int)(player.position.X - Projectile.position.X) / 16 < 16 && (player.position.X - Projectile.position.X) / 16 > -16 && (player.position.Y - Projectile.position.Y) / 16 < 16 && (player.position.Y - Projectile.position.Y) / 16 > -16)) {
				Explode();
			}
			Lighting.AddLight(Projectile.Center, player.eyeColor.ToVector3());
		}

		public override void Kill(int timeLeft) 
		{
			Player player = Main.player[Projectile.owner];
			for (int i = 0; i < 6; i++) {
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y,  Main.rand.Next(3) - Main.rand.Next(3),  Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<Breathmine>(), 0, 0, Projectile.owner);
			}
		}

		public override bool? CanDamage()
		{
			return Projectile.width == 260;
		}

		public void Explode() {
			Player player = Main.player[Projectile.owner];
			Vector2 velocity = Vector2.Normalize(Projectile.position - player.Center) * (10 + (player.statLifeMax2 / 80));
			player.velocity = -velocity;
			player.fallStart = (int)(player.position.Y / 16f);
			SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
			if (player.HeldItem.pick > 0) {
				int explosionRadius = 5;
				int minTileX = (int)(Projectile.position.X / 16f - (float)explosionRadius);
				int maxTileX = (int)(Projectile.position.X / 16f + (float)explosionRadius);
				int minTileY = (int)(Projectile.position.Y / 16f - (float)explosionRadius);
				int maxTileY = (int)(Projectile.position.Y / 16f + (float)explosionRadius);
				if (minTileX < 0) {
					minTileX = 0;
				}
				if (maxTileX > Main.maxTilesX) {
					maxTileX = Main.maxTilesX;
				}
				if (minTileY < 0) {
					minTileY = 0;
				}
				if (maxTileY > Main.maxTilesY) {
					maxTileY = Main.maxTilesY;
				}
				for (int x = minTileX; x <= maxTileX; x++) {
					for (int y = minTileY; y <= maxTileY; y++) {
						float diffX = Math.Abs((float)x - Projectile.position.X / 16f);
						float diffY = Math.Abs((float)y - Projectile.position.Y / 16f);
						double distanceToTile = Math.Sqrt((double)(diffX * diffX + diffY * diffY));
						if (distanceToTile < (double)explosionRadius) {
							player.PickTile(x, y, player.HeldItem.pick);
						}
					}
				}
			}
			int goreIndex = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
			Main.gore[goreIndex].scale = 0.6f;
			Main.gore[goreIndex].alpha = 100;
			Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
			Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
			goreIndex = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
			Main.gore[goreIndex].scale = 0.6f;
			Main.gore[goreIndex].alpha = 100;
			Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
			Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
			goreIndex = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
			Main.gore[goreIndex].scale = 0.6f;
			Main.gore[goreIndex].alpha = 100;
			Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
			Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
			goreIndex = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
			Main.gore[goreIndex].scale = 0.6f;
			Main.gore[goreIndex].alpha = 100;
			Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
			Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
			for (int i = 0; i < 20; i++) {
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 278);
				Main.dust[dust].color = player.eyeColor;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 3f;
				dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 278);
				Main.dust[dust].color = player.eyeColor;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 2f;
			}
			Projectile.position = Projectile.Center;
			Projectile.width = 260;
			Projectile.height = 260;
			Projectile.Center = Projectile.position;
			Projectile.alpha = 255;
			Projectile.timeLeft = 3;
		}

		public override void PostDraw(Color lightColor) {
			Player player = Main.player[Projectile.owner];
			Texture2D textureEyes = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/Sparkmine_Eyes");

			int frameHeight = textureEyes.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, textureEyes.Width, frameHeight);

			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = Projectile.GetAlpha(lightColor);
			Main.EntitySpriteDraw(textureEyes,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, new Color(player.eyeColor.ToVector4() * Projectile.Opacity), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
		}
	}
}
