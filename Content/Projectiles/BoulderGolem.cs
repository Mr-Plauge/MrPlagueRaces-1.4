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
	public class BoulderGolem : ModProjectile
	{
		// {T} Using this flag instead of ai[0] as ai[0] was never used in an owner-only context.
		public bool creationFlag = false;

		// {T} Did away with the left and right boulder variables, now tracking using ai[0] and ai[1] respectively.
		public float spin;
		public float extend;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("BoulderGolem");
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

				if (Main.myPlayer == Projectile.owner) // {T} Don't spawn projectiles for non-owner clients.
				{
					Projectile.ai[0] = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y + 16, 0f, 0f, ProjectileType<Boulder>(), (5 + (player.statDefense < 20 ? player.statDefense / 3 : player.statDefense < 40 ? player.statDefense / 2 : player.statDefense < 60 ? player.statDefense : player.statDefense * 1.25f)) * (int)(ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.BoulderGolem) ? (1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems[LihzahrdGolemType.BoulderGolem]]) : 1f), 0, Projectile.owner);
                    Projectile.ai[1] = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y + 16, 0f, 0f, ProjectileType<Boulder>(), (5 + (player.statDefense < 20 ? player.statDefense / 3 : player.statDefense < 40 ? player.statDefense / 2 : player.statDefense < 60 ? player.statDefense : player.statDefense * 1.25f)) * (int)(ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.BoulderGolem) ? (1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems[LihzahrdGolemType.BoulderGolem]]) : 1f), 0, Projectile.owner);
					Projectile.netUpdate = true;
				}

				creationFlag = true;
            }

			if (Main.myPlayer == Projectile.owner) // {T} Don't check and kill projectiles unless they're owned by us.
			{
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile projectile = Main.projectile[i];
					if (projectile.active && projectile.type == ProjectileType<Boulder>() && projectile.owner == player.whoAmI && projectile.whoAmI != Projectile.ai[0] && projectile.whoAmI != Projectile.ai[1])
						projectile.Kill();
				}
			}
            spin += 0.1f;
			if (extend < 100) {
				extend += 5;
			}

            // {T} Just reducing the repeated calls to Main.projectile.
            var leftBoulder = Main.projectile[(int)Projectile.ai[0]];
            var rightBoulder = Main.projectile[(int)Projectile.ai[1]];

            leftBoulder.direction = -1;
			rightBoulder.direction = 1;
			leftBoulder.Center = new Vector2(Projectile.Center.X, Projectile.Center.Y + 16) + Vector2.One.RotatedBy(spin) * extend;
			rightBoulder.Center = new Vector2(Projectile.Center.X, Projectile.Center.Y + 16) + Vector2.One.RotatedBy(spin) * -extend;
			leftBoulder.rotation += 0.1f;
			rightBoulder.rotation += 0.1f;
			if (player.whoAmI == Main.myPlayer)
			{
				if (!leftBoulder.active || !rightBoulder.active || (leftBoulder.position.X - Projectile.position.X) < -500 || (rightBoulder.position.X - Projectile.position.X) > 500)
				{
					Projectile.Kill();
				}
			}
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
				if (projectile.active && projectile.type == ProjectileType<Boulder>() && projectile.owner == player.whoAmI)
					projectile.Kill();
			}
			SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);
		}

		private static Asset<Texture2D> chainTexture;

		public override void Load() {
			chainTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/GolemChain");
		}

		public override bool PreDrawExtras() {
            // {T} Just reducing the repeated calls to Main.projectile.
            var leftBoulder = Main.projectile[(int)Projectile.ai[0]];
            var rightBoulder = Main.projectile[(int)Projectile.ai[1]];

            Vector2 leftBoulderCenter = new Vector2(leftBoulder.Center.X, leftBoulder.Center.Y);
			Vector2 rightBoulderCenter = new Vector2(rightBoulder.Center.X, rightBoulder.Center.Y);

			Vector2 centerToLeft = new Vector2(Projectile.Center.X, Projectile.Center.Y + 16);
			Vector2 centerToRight = new Vector2(Projectile.Center.X, Projectile.Center.Y + 16);

			Vector2 directionToLeftBoulder = leftBoulderCenter - new Vector2(Projectile.Center.X, Projectile.Center.Y + 16);
			Vector2 directionToRightBoulder = rightBoulderCenter - new Vector2(Projectile.Center.X, Projectile.Center.Y + 16);

			float leftChainRotation = directionToLeftBoulder.ToRotation() - MathHelper.PiOver2;
			float rightChainRotation = directionToRightBoulder.ToRotation() - MathHelper.PiOver2;

			float distanceToLeftBoulder = directionToLeftBoulder.Length();
			float distanceToRightBoulder = directionToRightBoulder.Length();

			while (distanceToLeftBoulder > 10f && !float.IsNaN(distanceToLeftBoulder)) {
				directionToLeftBoulder /= distanceToLeftBoulder;
				directionToLeftBoulder *= chainTexture.Height();

				centerToLeft += directionToLeftBoulder;
				directionToLeftBoulder = leftBoulderCenter - centerToLeft;
				distanceToLeftBoulder = directionToLeftBoulder.Length();

				Color drawColor = Lighting.GetColor((int)centerToLeft.X / 16, (int)(centerToLeft.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, centerToLeft - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, leftChainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}

			while (distanceToRightBoulder > 10f && !float.IsNaN(distanceToRightBoulder)) {
				directionToRightBoulder /= distanceToRightBoulder;
				directionToRightBoulder *= chainTexture.Height();

				centerToRight += directionToRightBoulder;
				directionToRightBoulder = rightBoulderCenter - centerToRight;
				distanceToRightBoulder = directionToRightBoulder.Length();

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
			Texture2D glowmask = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/BoulderGolem_Glowmask");

			int frameHeight = glowmask.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, glowmask.Width, frameHeight);

			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = Color.White;
			Main.EntitySpriteDraw(glowmask,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}
	}
}
