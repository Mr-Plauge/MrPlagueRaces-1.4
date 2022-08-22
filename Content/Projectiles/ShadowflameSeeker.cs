using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class ShadowflameSeeker : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Shadowflame Seeker");
			Main.projFrames[Projectile.type] = 6;
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults() {
			Projectile.width = 38;
			Projectile.height = 32;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.alpha = 100;
			Projectile.timeLeft = 120;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			Projectile.ai[0]++;
			Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : -1;
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (Projectile.spriteDirection == -1) {
				Projectile.rotation += MathHelper.Pi;
			}
			if (Projectile.alpha < 100) {
				Projectile.alpha++;
			}
			Projectile.frameCounter++;
			if (Projectile.frameCounter > 6)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame > 5)
				{
					Projectile.frame = 0;
				}
			}
			if (Main.rand.Next(5) < 4) {
				Dust dust19 = Dust.NewDustDirect(new Vector2(Projectile.position.X - 2f, Projectile.position.Y - 2f), Projectile.width + 4, Projectile.height + 4, 27, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 180, default(Color), 1.95f);
				dust19.noGravity = true;
				dust19.velocity *= 0.75f;
				dust19.velocity.X *= 0.75f;
				dust19.velocity.Y -= 1f;
				if (Main.rand.Next(4) == 0)
				{
					dust19.noGravity = false;
					dust19.scale *= 0.5f;
				}
			}

			NPC closestNPC = FindClosestNPC(500f);
			if (closestNPC == null)
				return;
			Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 8f;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Player player = Main.player[Projectile.owner];
			target.AddBuff(BuffType<DarkInferno>(), 10);
		}

		public override void Kill(int timeLeft) {
			for (int i = 0; i < 6; i++) {
				Dust dust19 = Dust.NewDustDirect(new Vector2(Projectile.position.X - 2f, Projectile.position.Y - 2f), Projectile.width + 4, Projectile.height + 4, 27, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 180, default(Color), 1.95f);
				dust19.noGravity = true;
				dust19.velocity *= 0.75f;
				dust19.velocity.X *= 0.75f;
				dust19.velocity.Y -= 1f;
				if (Main.rand.Next(4) == 0)
				{
					dust19.noGravity = false;
					dust19.scale *= 0.5f;
				}
			}
			SoundEngine.PlaySound(SoundID.NPCDeath55, Projectile.Center);
		}

		public NPC FindClosestNPC(float maxDetectDistance) {
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			for (int k = 0; k < Main.maxNPCs; k++) {
				NPC target = Main.npc[k];
				if (target.CanBeChasedBy() && !target.HasBuff(BuffType<DarkInferno>())) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			return new Color(Color.White.ToVector4() * Projectile.Opacity);
		}

		public override bool PreDraw(ref Color lightColor) {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (Projectile.spriteDirection == -1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}

			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = Projectile.GetAlpha(lightColor);
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++) {
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
			}
			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
			return false;
		}
	}
}
