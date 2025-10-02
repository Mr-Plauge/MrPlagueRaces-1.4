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
	public class LaserGolem : ModProjectile
	{
		public bool creationFlag = false; // {T} Added this in place of ai[0] since we were never assigning ai[0] in an owner-only code hook.

		public int directionY = 1;

		// {T} Did away with the grip and laser variables- now using .ai.
		// ai[0] is left grip. ai[1] is right grip. ai[2] is laser.

		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("LaserGolem");
		}

		public override void SetDefaults() {
			Projectile.width = 48;
			Projectile.height = 48;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Main.projFrames[Projectile.type] = 2;
		}

		public override void AI() {
			Projectile.timeLeft++;
			Player player = Main.player[Projectile.owner];
			var lihzahrdPlayer = player.GetModPlayer<LihzahrdPlayer>();
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            Projectile.velocity.Y = 1f * (directionY == 1 ? 1 : -1);
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

				// {T} Added check to not try and create projectiles on non-owner clients.
				if (Main.myPlayer == Projectile.owner)
				{
					Projectile.ai[0] = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<GolemCeilingGripper>(), 0, 0, Projectile.owner);
                    Projectile.ai[1] = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<GolemCeilingGripper>(), 0, 0, Projectile.owner);
                    Projectile.ai[2] = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y + 23, 0f, 0f, ProjectileType<Laser>(), (5 + (player.statDefense < 20 ? player.statDefense / 3 : player.statDefense < 40 ? player.statDefense / 2 : player.statDefense < 60 ? player.statDefense : player.statDefense * 1.25f)) * (int)(ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.LaserGolem) ? (1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems[LihzahrdGolemType.LaserGolem]]) : 1f), 0, Projectile.owner);
					Projectile.netUpdate = true;
				}

				SoundEngine.PlaySound(SoundID.Item13, Projectile.Center);
                creationFlag = true;
            }

			// {T} Just some code to help later, reducing repeat calls of Main.projectile.
            var leftGrip = Main.projectile[(int)Projectile.ai[0]];
            var rightGrip = Main.projectile[(int)Projectile.ai[1]];
            var laser = Main.projectile[(int)Projectile.ai[2]];

			if (laser.active) // {T} The laser risks despawning fairly easily, so I'm adding this check to confirm it's still active, to avoid stale references.
			{
				laser.position.Y = Projectile.position.Y + 26; // {T} Adjusted the offset. The laser should be a bit more straight ahead now.
				laser.ai[0] = Projectile.position.X;
				if (laser.ai[1] == 0)
				{
					laser.ai[1] = laser.position.X;
					Projectile.frame = 0;
				}
				else
				{
					Projectile.frame = 1;
				}
				for (int i = 0; i < 10; i++)
				{
					int dust = Dust.NewDust(new Vector2(laser.ai[1] + (Projectile.spriteDirection == 1 ? 5 : -15), laser.position.Y), laser.width, laser.height, 6);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 2f;
				}
				laser.velocity.X += Projectile.spriteDirection * 15;
			}
			leftGrip.direction = -1;
			rightGrip.direction = 1;
			if (player.whoAmI == Main.myPlayer)
			{
				// {T} Also make the projectile destroy itself if the laser despawns.
				if (!leftGrip.active || !rightGrip.active || !laser.active || (leftGrip.position.Y - Projectile.position.Y) < -1500 || (rightGrip.position.Y - Projectile.position.Y) > 1500)
				{
					Projectile.Kill();
				}
            }
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			directionY = -directionY;
			return false;
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
				if (projectile.active && (projectile.type == ProjectileType<GolemCeilingGripper>() || projectile.type == ProjectileType<Laser>()) && projectile.owner == player.whoAmI)
					projectile.Kill();
			}
			SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);
		}

		private static Asset<Texture2D> chainTexture;
		private static Asset<Texture2D> laserTexture;

		public override void Load() {
			chainTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/GolemChain");
			laserTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/GolemLaser");
		}

		public override bool PreDrawExtras() {
            // {T} Just some code to help later, reducing repeat calls of Main.projectile.
            var leftGrip = Main.projectile[(int)Projectile.ai[0]];
            var rightGrip = Main.projectile[(int)Projectile.ai[1]];
            var laser = Main.projectile[(int)Projectile.ai[2]];

            Vector2 leftGripCenter = new Vector2(leftGrip.Center.X, leftGrip.Center.Y);
			Vector2 rightGripCenter = new Vector2(rightGrip.Center.X, rightGrip.Center.Y);
			Vector2 laserCenter = new Vector2(Projectile.Center.X, Projectile.Center.Y + 8);

			Vector2 centerToLeft = new Vector2(Projectile.Center.X, Projectile.Center.Y);
			Vector2 centerToRight = new Vector2(Projectile.Center.X, Projectile.Center.Y);
			Vector2 centerToLaser = new Vector2(laser.ai[1] + (15 * Projectile.spriteDirection), laserCenter.Y);

			Vector2 directionToLeftGrip = leftGripCenter - new Vector2(Projectile.Center.X, Projectile.Center.Y);
			Vector2 directionToRightGrip = rightGripCenter - new Vector2(Projectile.Center.X, Projectile.Center.Y);
			Vector2 directionToLaser = laserCenter - new Vector2(laser.ai[1] + (15 * Projectile.spriteDirection), laserCenter.Y); // {T} Changed this to make the laser look straight.

			float leftChainRotation = directionToLeftGrip.ToRotation() - MathHelper.PiOver2;
			float rightChainRotation = directionToRightGrip.ToRotation() - MathHelper.PiOver2;
			float laserRotation = directionToLaser.ToRotation() - MathHelper.PiOver2;

			float distanceToLeftGrip = directionToLeftGrip.Length();
			float distanceToRightGrip = directionToRightGrip.Length();
			float distanceToLaser = directionToLaser.Length();

			while (distanceToLeftGrip > 10f && !float.IsNaN(distanceToLeftGrip)) {
				directionToLeftGrip /= distanceToLeftGrip;
				directionToLeftGrip *= chainTexture.Height();

				centerToLeft += directionToLeftGrip;
				directionToLeftGrip = leftGripCenter - centerToLeft;
				distanceToLeftGrip = directionToLeftGrip.Length();

				Color drawColor = Lighting.GetColor((int)centerToLeft.X / 16, (int)(centerToLeft.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, centerToLeft - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, leftChainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}

			while (distanceToRightGrip > 10f && !float.IsNaN(distanceToRightGrip)) {
				directionToRightGrip /= distanceToRightGrip;
				directionToRightGrip *= chainTexture.Height();

				centerToRight += directionToRightGrip;
				directionToRightGrip = rightGripCenter - centerToRight;
				distanceToRightGrip = directionToRightGrip.Length();

				Color drawColor = Lighting.GetColor((int)centerToRight.X / 16, (int)(centerToRight.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, centerToRight - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, rightChainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}

			while (distanceToLaser > 10f && !float.IsNaN(distanceToLaser)) {
				directionToLaser /= distanceToLaser;
				directionToLaser *= laserTexture.Height();

				centerToLaser += directionToLaser;
				directionToLaser = laserCenter - centerToLaser;
				distanceToLaser = directionToLaser.Length();

				Main.EntitySpriteDraw(laserTexture.Value, centerToLaser - Main.screenPosition,
					laserTexture.Value.Bounds, new Color(255, 255, 255, 0), laserRotation,
					laserTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
			return false;
		}

		public override bool PreDraw(ref Color lightColor) {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (Projectile.spriteDirection == -1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Player player = Main.player[Projectile.owner];
			Texture2D body = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/LaserGolem");
			Texture2D glowmask = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/LaserGolem_Glowmask");

			int frameHeight = glowmask.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, glowmask.Width, frameHeight);

			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = Color.White;
			Main.EntitySpriteDraw(body,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

			Main.EntitySpriteDraw(glowmask,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
			return false;
		}
	}
}