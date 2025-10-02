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
	public class SpiderGolem : ModProjectile
	{
		// {T} Added this to replace ai[0].
		public bool creationFlag = false;

		// {T} Removed left and right grip variables. Now using ai[0] and ai[1] respectively.
		public int directionX = 1;
		public int bulletTimer = 0;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("SpiderGolem");
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

				if (Main.myPlayer == Projectile.owner) // {T} Avoid spawning projectiles on non-owner clients.
				{
					Projectile.ai[0] = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<GolemWallGripper>(), 0, 0, Projectile.owner);
					Projectile.ai[1] = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<GolemWallGripper>(), 0, 0, Projectile.owner);
					Projectile.netUpdate = true;
				}

				creationFlag = true;
			}

            // {T} Just easier to use, and reduces calls to Main.projectile.
            var leftGrip = Main.projectile[(int)Projectile.ai[0]];
            var rightGrip = Main.projectile[(int)Projectile.ai[1]];

            leftGrip.direction = -1;
            rightGrip.direction = 1;

			NPC closestNPC = FindClosestNPC(150f);
			if (closestNPC != null && closestNPC.position.Y > Projectile.position.Y) {
				Projectile.velocity.X = (closestNPC.Center.X - Projectile.Center.X) * 0.1f;
				bulletTimer++;
				if (bulletTimer == 5) {
					SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
					if (Main.myPlayer == Projectile.owner) // {T} We shouldn't spawn projectiles on non-owner clients.
					{
						for (int i = 0; i < 5; i++)
						{
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X + Main.rand.Next(15) - Main.rand.Next(15), Projectile.Center.Y + 10 + Main.rand.Next(5) - Main.rand.Next(5), 0f, 15f + Main.rand.Next(5) - Main.rand.Next(5), ProjectileType<SpiderBullet>(), (3 + (player.statDefense < 20 ? player.statDefense / 3 : player.statDefense < 40 ? player.statDefense / 2 : player.statDefense < 60 ? player.statDefense : player.statDefense * 1.25f)) * (int)(ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.SpiderGolem) ? (1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems[LihzahrdGolemType.SpiderGolem]]) : 1f), 0, Projectile.owner);
						}
					}
					bulletTimer = 0;
				}
				if (Projectile.velocity.X < 0) {
					directionX = -1;
				}
				if (Projectile.velocity.X > 0) {
					directionX = 1;
				}
			}
			else {
				Projectile.velocity.X = 1f * (directionX == 1 ? 1 : -1);
            }
			if (player.whoAmI == Main.myPlayer)
			{
				if (!leftGrip.active || !rightGrip.active || (leftGrip.position.X - Projectile.position.X) < -1500 || (rightGrip.position.X - Projectile.position.X) > 1500)
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
				if (projectile.active && projectile.type == ProjectileType<GolemWallGripper>() && projectile.owner == player.whoAmI)
					projectile.Kill();
			}
			SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);
		}

		public NPC FindClosestNPC(float maxDetectDistance) {
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			for (int k = 0; k < Main.maxNPCs; k++) {
				NPC target = Main.npc[k];
				if (target.CanBeChasedBy()) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			directionX = -directionX;
			return false;
		}

		private static Asset<Texture2D> chainTexture;

		public override void Load() {
			chainTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/GolemChain");
		}

		public override bool PreDrawExtras() {
            // {T} Just easier to use, and reduces calls to Main.projectile.
            var leftGrip = Main.projectile[(int)Projectile.ai[0]];
            var rightGrip = Main.projectile[(int)Projectile.ai[1]];

            Vector2 leftGripCenter = new Vector2(leftGrip.Center.X, leftGrip.Center.Y);
			Vector2 rightGripCenter = new Vector2(rightGrip.Center.X, rightGrip.Center.Y);

			Vector2 centerToLeft = new Vector2(Projectile.Center.X, Projectile.Center.Y);
			Vector2 centerToRight = new Vector2(Projectile.Center.X, Projectile.Center.Y);

			Vector2 directionToLeftGrip = leftGripCenter - new Vector2(Projectile.Center.X, Projectile.Center.Y);
			Vector2 directionToRightGrip = rightGripCenter - new Vector2(Projectile.Center.X, Projectile.Center.Y);

			float leftChainRotation = directionToLeftGrip.ToRotation() - MathHelper.PiOver2;
			float rightChainRotation = directionToRightGrip.ToRotation() - MathHelper.PiOver2;

			float distanceToLeftGrip = directionToLeftGrip.Length();
			float distanceToRightGrip = directionToRightGrip.Length();

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
			return false;
		}

		public override void PostDraw(Color lightColor) {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (Projectile.spriteDirection == -1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Player player = Main.player[Projectile.owner];
			Texture2D glowmask = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/SpiderGolem_Glowmask");

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
