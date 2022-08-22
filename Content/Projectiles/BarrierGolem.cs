using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Common.Races.Lihzahrd;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class BarrierGolem : ModProjectile
	{
		public int leftBarrier = 0;
		public int rightBarrier = 0;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("BarrierGolem");
		}

		public override void SetDefaults() {
			Projectile.width = 48;
			Projectile.height = 48;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.ownerHitCheck = true;
		}

		public override void AI() {
			Projectile.timeLeft++;
			Player player = Main.player[Projectile.owner];
			var lihzahrdPlayer = player.GetModPlayer<LihzahrdPlayer>();
			Projectile.velocity.Y += 0.5f;
			if (Projectile.ai[0] == 0 && Main.myPlayer == Projectile.owner) {
				Projectile.position = new Vector2(Main.MouseWorld.X - 22, Main.MouseWorld.Y - 42);
				SoundEngine.PlaySound(SoundID.DD2_DefenseTowerSpawn, Projectile.Center);
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
				for (int i = 0; i < 10; i++) {
					Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6);
				}
				Projectile.direction = Projectile.spriteDirection = lihzahrdPlayer.direction == 1 ? 1 : -1;
				leftBarrier = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y - 15, 0f, 0f, ProjectileType<Barrier>(), 1 + player.statDefense, 0, Projectile.owner);
				rightBarrier = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y - 15, 0f, 0f, ProjectileType<Barrier>(), 1 + player.statDefense, 0, Projectile.owner);
			}
			Projectile.ai[0]++;
			Main.projectile[leftBarrier].direction = 1;
			Main.projectile[rightBarrier].direction = -1;
			if (!Main.projectile[leftBarrier].active || !Main.projectile[rightBarrier].active || (Main.projectile[leftBarrier].position.X - Projectile.position.X) < -500 || (Main.projectile[rightBarrier].position.X - Projectile.position.X) > 500) {
				Projectile.Kill();
			}
		}

		public override void Kill(int timeLeft) {
			Player player = Main.player[Projectile.owner];
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
			for (int i = 0; i < 10; i++) {
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6);
			}
			for (int i = 0; i < Main.maxProjectiles; i++) {
				Projectile projectile = Main.projectile[i];
				if (projectile.active && projectile.type == ProjectileType<Barrier>() && projectile.owner == player.whoAmI)
					projectile.Kill();
			}
			SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		private static Asset<Texture2D> chainTexture;

		public override void Load() {
			chainTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/GolemChain");
		}

		public override bool PreDrawExtras() {
			Vector2 leftBarrierCenter = new Vector2(Main.projectile[leftBarrier].Center.X, Main.projectile[leftBarrier].Center.Y + 16);
			Vector2 rightBarrierCenter = new Vector2(Main.projectile[rightBarrier].Center.X, Main.projectile[rightBarrier].Center.Y + 16);

			Vector2 centerToLeft = new Vector2(Projectile.Center.X, Projectile.Center.Y + 10);
			Vector2 centerToRight = new Vector2(Projectile.Center.X, Projectile.Center.Y + 10);

			Vector2 directionToLeftBarrier = leftBarrierCenter - new Vector2(Projectile.Center.X, Projectile.Center.Y + 10);
			Vector2 directionToRightBarrier = rightBarrierCenter - new Vector2(Projectile.Center.X, Projectile.Center.Y + 10);

			float leftChainRotation = directionToLeftBarrier.ToRotation() - MathHelper.PiOver2;
			float rightChainRotation = directionToRightBarrier.ToRotation() - MathHelper.PiOver2;

			float distanceToLeftBarrier = directionToLeftBarrier.Length();
			float distanceToRightBarrier = directionToRightBarrier.Length();

			while (distanceToLeftBarrier > 10f && !float.IsNaN(distanceToLeftBarrier)) {
				directionToLeftBarrier /= distanceToLeftBarrier;
				directionToLeftBarrier *= chainTexture.Height();

				centerToLeft += directionToLeftBarrier;
				directionToLeftBarrier = leftBarrierCenter - centerToLeft;
				distanceToLeftBarrier = directionToLeftBarrier.Length();

				Color drawColor = Lighting.GetColor((int)centerToLeft.X / 16, (int)(centerToLeft.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, centerToLeft - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, leftChainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}

			while (distanceToRightBarrier > 10f && !float.IsNaN(distanceToRightBarrier)) {
				directionToRightBarrier /= distanceToRightBarrier;
				directionToRightBarrier *= chainTexture.Height();

				centerToRight += directionToRightBarrier;
				directionToRightBarrier = rightBarrierCenter - centerToRight;
				distanceToRightBarrier = directionToRightBarrier.Length();

				Color drawColor = Lighting.GetColor((int)centerToRight.X / 16, (int)(centerToRight.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, centerToRight - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, rightChainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
			return false;
		}

		public override void PostDraw(Color lightColor) {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (Projectile.spriteDirection == -1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Player player = Main.player[Projectile.owner];
			Texture2D glowmask = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/BarrierGolem_Glowmask");

			int frameHeight = glowmask.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, glowmask.Width, frameHeight);

			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = Color.White;
			Main.EntitySpriteDraw(glowmask,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
		}
	}
}
