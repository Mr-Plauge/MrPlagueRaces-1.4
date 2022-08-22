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
	public class TetherGolem : ModProjectile
	{
		public int leftTether = 0;
		public int middleTether = 0;
		public int rightTether = 0;
		public int leftTetherTime = 400;
		public int middleTetherTime = 400;
		public int rightTetherTime = 400;
		public NPC leftNPC;
		public NPC middleNPC;
		public NPC rightNPC;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("TetherGolem");
		}

		public override void SetDefaults() {
			Projectile.width = 48;
			Projectile.height = 48;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Main.projFrames[Projectile.type] = 2;
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
				leftTether = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<Tether>(), 1 + player.statDefense / 6, 0, Projectile.owner);
				middleTether = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<Tether>(), 1 + player.statDefense / 6, 0, Projectile.owner);
				rightTether = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<Tether>(), 1 + player.statDefense / 6, 0, Projectile.owner);
			}
			Projectile.ai[0]++;
			if (leftNPC != null || middleNPC != null || rightNPC != null) {
				Projectile.frame = 1;
			}
			else {
				Projectile.frame = 0;
			}
			
			if (leftNPC == null || !leftNPC.active || leftTetherTime == 0) {
				if (leftTetherTime < 400) {
					leftTetherTime++;
					leftNPC = null;
				}
				else {
					leftNPC = FindClosestNPC(350f);
				}
				Main.projectile[leftTether].velocity = (Projectile.Center - Main.projectile[leftTether].Center) * 0.5f;
				Main.projectile[leftTether].ai[0] = 0;
			}
			if (leftNPC != null) {
				if (leftTetherTime > 0) {
					leftTetherTime--;
				}
				if (Main.projectile[leftTether].ai[0] == 0) {
					Main.projectile[leftTether].ai[0] = 1;
				}
				Projectile.direction = Projectile.spriteDirection = leftNPC.Center.X > Projectile.Center.X ? 1 : -1;
				leftNPC.AddBuff(BuffType<Tethered>(), 60);
				Main.projectile[leftTether].velocity = (leftNPC.Center - Main.projectile[leftTether].Center) * 0.5f;
				if (leftNPC.Center.X - Projectile.Center.X > 150) {
					if (leftNPC.velocity.X > 0) {
						leftNPC.velocity.X -= 0.5f;
					}
				}
				if (leftNPC.Center.X - Projectile.Center.X < -150) {
					if (leftNPC.velocity.X < 0) {
						leftNPC.velocity.X += 0.5f;
					}
				}
				if (leftNPC.Center.Y - Projectile.Center.Y > 150) {
					if (leftNPC.velocity.Y > 0) {
						leftNPC.velocity.Y -= 0.5f;
					}
				}
				if (leftNPC.Center.Y - Projectile.Center.Y < -150) {
					if (leftNPC.velocity.Y < 0) {
						leftNPC.velocity.Y += 0.5f;
					}
				}
			}

			if (middleNPC == null || !middleNPC.active | middleTetherTime == 0) {
				if (middleTetherTime < 400) {
					middleTetherTime++;
					middleNPC = null;
				}
				else {
					middleNPC = FindClosestNPC(350f);
				}
				Main.projectile[middleTether].velocity = (Projectile.Center - Main.projectile[middleTether].Center) * 0.5f;
				Main.projectile[middleTether].ai[0] = 0;
			}
			if (middleNPC != null) {
				if (middleTetherTime > 0) {
					middleTetherTime--;
				}
				if (Main.projectile[middleTether].ai[0] == 0) {
					Main.projectile[middleTether].ai[0] = 1;
				}
				Projectile.direction = Projectile.spriteDirection = middleNPC.Center.X > Projectile.Center.X ? 1 : -1;
				middleNPC.AddBuff(BuffType<Tethered>(), 60);
				Main.projectile[middleTether].velocity = (middleNPC.Center - Main.projectile[middleTether].Center) * 0.5f;
				if (middleNPC.Center.X - Projectile.Center.X > 150) {
					if (middleNPC.velocity.X > 0) {
						middleNPC.velocity.X -= 0.5f;
					}
				}
				if (middleNPC.Center.X - Projectile.Center.X < -150) {
					if (middleNPC.velocity.X < 0) {
						middleNPC.velocity.X += 0.5f;
					}
				}
				if (middleNPC.Center.Y - Projectile.Center.Y > 150) {
					if (middleNPC.velocity.Y > 0) {
						middleNPC.velocity.Y -= 0.5f;
					}
				}
				if (middleNPC.Center.Y - Projectile.Center.Y < -150) {
					if (middleNPC.velocity.Y < 0) {
						middleNPC.velocity.Y += 0.5f;
					}
				}
			}

			if (rightNPC == null || !rightNPC.active || rightTetherTime == 0) {
				if (rightTetherTime < 400) {
					rightTetherTime++;
					rightNPC = null;
				}
				else {
					rightNPC = FindClosestNPC(350f);
				}
				Main.projectile[rightTether].velocity = (Projectile.Center - Main.projectile[rightTether].Center) * 0.5f;
				Main.projectile[rightTether].ai[0] = 0;
			}
			if (rightNPC != null) {
				if (rightTetherTime > 0) {
					rightTetherTime--;
				}
				if (Main.projectile[rightTether].ai[0] == 0) {
					Main.projectile[rightTether].ai[0] = 1;
				}
				Projectile.direction = Projectile.spriteDirection = rightNPC.Center.X > Projectile.Center.X ? 1 : -1;
				Main.projectile[rightTether].velocity = (rightNPC.Center - Main.projectile[rightTether].Center) * 0.5f;
				rightNPC.AddBuff(BuffType<Tethered>(), 60);
				if (rightNPC.Center.X - Projectile.Center.X > 150) {
					if (rightNPC.velocity.X > 0) {
						rightNPC.velocity.X -= 0.5f;
					}
				}
				if (rightNPC.Center.X - Projectile.Center.X < -150) {
					if (rightNPC.velocity.X < 0) {
						rightNPC.velocity.X += 0.5f;
					}
				}
				if (rightNPC.Center.Y - Projectile.Center.Y > 150) {
					if (rightNPC.velocity.Y > 0) {
						rightNPC.velocity.Y -= 0.5f;
					}
				}
				if (rightNPC.Center.Y - Projectile.Center.Y < -150) {
					if (rightNPC.velocity.Y < 0) {
						rightNPC.velocity.Y += 0.5f;
					}
				}
			}
		}

		public NPC FindClosestNPC(float maxDetectDistance) {
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			for (int k = 0; k < Main.maxNPCs; k++) {
				NPC target = Main.npc[k];
				if (target.CanBeChasedBy() && !target.HasBuff(BuffType<Tethered>())) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
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
				if (projectile.active && projectile.type == ProjectileType<Tether>() && projectile.owner == player.whoAmI)
					projectile.Kill();
			}
			SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		private static Asset<Texture2D> chainTexture;
		private static Asset<Texture2D> endTexture;
		private static Asset<Texture2D> endTexture_Glowmask;

		public override void Load() {
			chainTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/GolemChain");
			endTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/TetherEnd");
			endTexture_Glowmask = ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/TetherEnd_Glowmask");
		}

		public override bool PreDrawExtras() {
			Vector2 leftTetherCenter = new Vector2(Main.projectile[leftTether].Center.X, Main.projectile[leftTether].Center.Y);
			Vector2 middleTetherCenter = new Vector2(Main.projectile[middleTether].Center.X, Main.projectile[middleTether].Center.Y);
			Vector2 rightTetherCenter = new Vector2(Main.projectile[rightTether].Center.X, Main.projectile[rightTether].Center.Y);

			Vector2 centerToLeft = new Vector2(Projectile.Center.X, Projectile.Center.Y + 16);
			Vector2 centerToMiddle = new Vector2(Projectile.Center.X, Projectile.Center.Y + 16);
			Vector2 centerToRight = new Vector2(Projectile.Center.X, Projectile.Center.Y + 16);

			Vector2 directionToLeftTether = leftTetherCenter - new Vector2(Projectile.Center.X, Projectile.Center.Y + 16);
			Vector2 directionToMiddleTether = middleTetherCenter - new Vector2(Projectile.Center.X, Projectile.Center.Y + 16);
			Vector2 directionToRightTether = rightTetherCenter - new Vector2(Projectile.Center.X, Projectile.Center.Y + 16);

			float leftChainRotation = directionToLeftTether.ToRotation() - MathHelper.PiOver2;
			float middleChainRotation = directionToMiddleTether.ToRotation() - MathHelper.PiOver2;
			float rightChainRotation = directionToRightTether.ToRotation() - MathHelper.PiOver2;

			float distanceToLeftTether = directionToLeftTether.Length();
			float distanceToMiddleTether = directionToMiddleTether.Length();
			float distanceToRightTether = directionToRightTether.Length();

			while (distanceToLeftTether > 10f && !float.IsNaN(distanceToLeftTether)) {
				directionToLeftTether /= distanceToLeftTether;
				directionToLeftTether *= chainTexture.Height();

				centerToLeft += directionToLeftTether;
				directionToLeftTether = leftTetherCenter - centerToLeft;
				distanceToLeftTether = directionToLeftTether.Length();

				Color drawColor = Lighting.GetColor((int)centerToLeft.X / 16, (int)(centerToLeft.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, centerToLeft - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, leftChainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

				Main.EntitySpriteDraw(endTexture.Value, Main.projectile[leftTether].Center - Main.screenPosition,
					endTexture.Value.Bounds, drawColor, leftChainRotation,
					endTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

				Main.EntitySpriteDraw(endTexture_Glowmask.Value, Main.projectile[leftTether].Center - Main.screenPosition,
					endTexture.Value.Bounds, Color.White, leftChainRotation,
					endTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}

			while (distanceToMiddleTether > 10f && !float.IsNaN(distanceToMiddleTether)) {
				directionToMiddleTether /= distanceToMiddleTether;
				directionToMiddleTether *= chainTexture.Height();

				centerToMiddle += directionToMiddleTether;
				directionToMiddleTether = middleTetherCenter - centerToMiddle;
				distanceToMiddleTether = directionToMiddleTether.Length();

				Color drawColor = Lighting.GetColor((int)centerToMiddle.X / 16, (int)(centerToMiddle.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, centerToMiddle - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, middleChainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

				Main.EntitySpriteDraw(endTexture.Value, Main.projectile[middleTether].Center - Main.screenPosition,
					endTexture.Value.Bounds, drawColor, middleChainRotation,
					endTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

				Main.EntitySpriteDraw(endTexture_Glowmask.Value, Main.projectile[middleTether].Center - Main.screenPosition,
					endTexture.Value.Bounds, Color.White, middleChainRotation,
					endTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}

			while (distanceToRightTether > 10f && !float.IsNaN(distanceToRightTether)) {
				directionToRightTether /= distanceToRightTether;
				directionToRightTether *= chainTexture.Height();

				centerToRight += directionToRightTether;
				directionToRightTether = rightTetherCenter - centerToRight;
				distanceToRightTether = directionToRightTether.Length();

				Color drawColor = Lighting.GetColor((int)centerToRight.X / 16, (int)(centerToRight.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, centerToRight - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, rightChainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

				Main.EntitySpriteDraw(endTexture.Value, Main.projectile[rightTether].Center - Main.screenPosition,
					endTexture.Value.Bounds, drawColor, rightChainRotation,
					endTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

				Main.EntitySpriteDraw(endTexture_Glowmask.Value, Main.projectile[rightTether].Center - Main.screenPosition,
					endTexture.Value.Bounds, Color.White, rightChainRotation,
					endTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
			return false;
		}
		
		public override bool PreDraw(ref Color lightColor) {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (Projectile.spriteDirection == -1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Player player = Main.player[Projectile.owner];
			Texture2D body = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/TetherGolem");
			Texture2D glowmask = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/TetherGolem_Glowmask");

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
