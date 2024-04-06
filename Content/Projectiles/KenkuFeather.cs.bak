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
	public class KenkuFeather : ModProjectile
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Kenku Feather");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults() {
			Projectile.width = 38;
			Projectile.height = 18;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.aiStyle = 0;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 100;
		}

		public override bool PreAI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (Main.rand.Next(3) == 1) {
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 16);
			}
			if (Projectile.timeLeft > 50) {
				Projectile.velocity *= new Vector2(0.9f, 0.9f);
			}

			if (Projectile.timeLeft < 3) {
				for (int k = 0; k < Main.maxPlayers; k++) {
					Player target = Main.player[k];
					if (target.active) {
						float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
						if (sqrDistanceToTarget / 16 < 300) {
							SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, target.Center);
							target.fallStart = (int)(target.position.Y / 16f);
							target.velocity = Projectile.velocity * (60 + ((player.statLifeMax2 / 2) < 120 ? (player.statLifeMax2 / 2) : 120));
						}
					}
				}
			}
			return true;
		}

		public override void Kill(int timeLeft) {
			for (int i = 0; i < 12; i++) {
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 16);
				Main.dust[dust].velocity *= 3f;
			}
			SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);
		}

		public override void PostDraw(Color lightColor) {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (Projectile.spriteDirection == -1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Player player = Main.player[Projectile.owner];
			Texture2D glowmask = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/KenkuFeather");

			int frameHeight = glowmask.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, glowmask.Width, frameHeight);

			Vector2 origin = sourceRectangle.Size() / 2f;
			Main.EntitySpriteDraw(glowmask,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, new Color(player.skinColor.ToVector4() * lightColor.ToVector4() * Projectile.Opacity), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
		}
	}
}
