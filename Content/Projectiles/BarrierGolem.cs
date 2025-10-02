using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.Races.Lihzahrd;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class BarrierGolem : ModProjectile
	{
		// {T} Added this instead of using ai[0].
		public bool creationFlag = false;

		// {T} Removed left and right barrier vars, now using ai[0] and ai[1] respectively.

		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("BarrierGolem");
		}

		public override void SetDefaults() {
			Projectile.width = 48;
			Projectile.height = 48;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
		}

		public override void AI() {
			Projectile.timeLeft++;
			Player player = Main.player[Projectile.owner];
			var lihzahrdPlayer = player.GetModPlayer<LihzahrdPlayer>();
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            Projectile.velocity.Y += 0.5f;
			if (!creationFlag) {
				Projectile.position = new Vector2(mrPlagueRacesPlayer.mouseWorld.X - 22, mrPlagueRacesPlayer.mouseWorld.Y - 42);
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

				if (Main.myPlayer == Projectile.owner) // {T} Don't run projectile spawning on non-owner clients.
				{
					Projectile.ai[0] = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y - 15, 0f, 0f, ProjectileType<Barrier>(), (5 + (player.statDefense < 20 ? player.statDefense / 3 : player.statDefense < 40 ? player.statDefense / 2 : player.statDefense < 60 ? player.statDefense : player.statDefense * 1.25f)) * (int)(ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.BarrierGolem) ? (1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems[LihzahrdGolemType.BarrierGolem]]) : 1f), 5f, Projectile.owner);
					Projectile.ai[1] = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y - 15, 0f, 0f, ProjectileType<Barrier>(), (5 + (player.statDefense < 20 ? player.statDefense / 3 : player.statDefense < 40 ? player.statDefense / 2 : player.statDefense < 60 ? player.statDefense : player.statDefense * 1.25f)) * (int)(ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.BarrierGolem) ? (1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems[LihzahrdGolemType.BarrierGolem]]) : 1f), 5f, Projectile.owner);
					Projectile.netUpdate = true;
				}

				creationFlag = true;
            }

            // {T} Just using these to reduce calls to Main.projectile.
            var leftBarrier = Main.projectile[(int)Projectile.ai[0]];
            var rightBarrier = Main.projectile[(int)Projectile.ai[1]];

            leftBarrier.direction = 1;
			rightBarrier.direction = -1;
			if (player.whoAmI == Main.myPlayer)
			{
				if (!leftBarrier.active || !rightBarrier.active || (leftBarrier.position.X - Projectile.position.X) < -500 || (rightBarrier.position.X - Projectile.position.X) > 500)
				{
					Projectile.Kill();
				}
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile projectile = Main.projectile[i];
					if (projectile.active && projectile.type == ProjectileType<Barrier>() && projectile.owner == player.whoAmI && projectile.whoAmI != Projectile.ai[0] && projectile.whoAmI != Projectile.ai[1])
						projectile.Kill();
				}
			} // {T} Shifted this bracket to encompass both kill conditions, since they only need to run on the owner's side.
        }

		public override void OnKill(int timeLeft) {
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
            // {T} Just using these to reduce calls to Main.projectile.
            var leftBarrier = Main.projectile[(int)Projectile.ai[0]];
            var rightBarrier = Main.projectile[(int)Projectile.ai[1]];

            Vector2 leftBarrierCenter = new Vector2(leftBarrier.Center.X, leftBarrier.Center.Y + 16);
			Vector2 rightBarrierCenter = new Vector2(rightBarrier.Center.X, rightBarrier.Center.Y + 16);

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
