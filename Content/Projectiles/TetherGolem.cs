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
using System.IO;

namespace MrPlagueRaces.Content.Projectiles
{
	// {T} Across the board, Projectile.ai[0] has been changed into a local variable since it was never being used in a client-only context, and
	// should not need to be networked.
	// The three ai values will now be used to hold references to the projectile indexes of each tether so tether chains draw correctly.
	public class TetherGolem : ModProjectile
	{
		public bool creationFlag = false; // {T} New value to replace ai[0].

		// {T} Removed the tether variables, now using Projectile.ai[0,1,2]
		public int leftTetherTime = 400;
		public int middleTetherTime = 400;
		public int rightTetherTime = 400;
		public int leftNPC = -1; // {T} These are now just integers holding the index in Main.npc. Better for networking.
		public int middleNPC = -1;
		public int rightNPC = -1;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("TetherGolem");
		}

		public override void SetDefaults() {
			Projectile.width = 48;
			Projectile.height = 48;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Main.projFrames[Projectile.type] = 2;
		}

		// {T} Added these to sync projectile behaviour across clients and server. Since we're pulling NPCs around, it's critical
		// that everything lines up across players, and the server.
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(leftNPC);
            writer.Write(middleNPC);
            writer.Write(rightNPC);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            leftNPC = reader.ReadInt32();
            middleNPC = reader.ReadInt32();
            rightNPC = reader.ReadInt32();
        }

        public override void AI() {
            // {T} Adding this to compare later, and force a sync if any change.
            var prevLeftNPC = leftNPC;
            var prevMiddleNPC = middleNPC;
            var prevRightNPC = rightNPC;

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
				if (Main.myPlayer == Projectile.owner) // {T} For safety reasons, let's not try to spawn projectiles on other client sides, only the owner side. It won't work.
				{
                    Projectile.ai[0] = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<Tether>(), (5 + (player.statDefense < 20 ? player.statDefense / 3 : player.statDefense < 40 ? player.statDefense / 2 : player.statDefense < 60 ? player.statDefense : player.statDefense * 1.25f)) * (int)(ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.TetherGolem) ? (1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems[LihzahrdGolemType.TetherGolem]]) : 1f), 0, Projectile.owner);
                    Projectile.ai[1] = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<Tether>(), (5 + (player.statDefense < 20 ? player.statDefense / 3 : player.statDefense < 40 ? player.statDefense / 2 : player.statDefense < 60 ? player.statDefense : player.statDefense * 1.25f)) * (int)(ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.TetherGolem) ? (1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems[LihzahrdGolemType.TetherGolem]]) : 1f), 0, Projectile.owner);
                    Projectile.ai[2] = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<Tether>(), (5 + (player.statDefense < 20 ? player.statDefense / 3 : player.statDefense < 40 ? player.statDefense / 2 : player.statDefense < 60 ? player.statDefense : player.statDefense * 1.25f)) * (int)(ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.TetherGolem) ? (1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems[LihzahrdGolemType.TetherGolem]]) : 1f), 0, Projectile.owner);
					Projectile.netUpdate = true;
				}
				creationFlag = true;
			}

			if (Main.myPlayer == Projectile.owner) // {T} Don't kill the tether projectiles on non-owner sides.
			{
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile projectile = Main.projectile[i];
					if (projectile.active && projectile.type == ProjectileType<Tether>() && projectile.owner == player.whoAmI && projectile.whoAmI != Projectile.ai[0] && projectile.whoAmI != Projectile.ai[1] && projectile.whoAmI != Projectile.ai[2])
						projectile.Kill();
				}
			}

            if (leftNPC >= 0 || middleNPC >= 0 || rightNPC >= 0) {
				Projectile.frame = 1;
			}
			else {
				Projectile.frame = 0;
			}
			
			if (leftNPC < 0 || !Main.npc[leftNPC].active || leftTetherTime == 0) {
				if (leftTetherTime < 400) {
					leftTetherTime++;
					leftNPC = -1;
				}
                else if (Main.myPlayer == Projectile.owner) // {T} Added owner check. We shouldn't seek targets on clients.
                {
                    var foundNPC = FindClosestNPC(350f); // {T} Tweaks to support using an index instead of an actual NPC object.
					if (foundNPC != null)
					{
						leftNPC = foundNPC.whoAmI;
					}
					else
					{
						leftNPC = -1;
					}
				}
                if (Projectile.ai[0] >= 0) // {T} Check to accomodate my changes involving setting this value to -1 initially and using .ai instead.
                {
                    // {T} Safety check in case of bad references. No guarantees ai value isn't just 0.
                    var leftTether = Main.projectile[(int)Projectile.ai[0]];
					if (leftTether.type == ModContent.ProjectileType<Tether>())
					{
                        leftTether.velocity = (Projectile.Center - leftTether.Center) * 0.5f;
                        leftTether.ai[0] = 0;
                        leftTether.netUpdate = true; // {T} Push a networked event to sync the ai change.
					}
                }
            }
			if (leftNPC >= 0) {
				if (leftTetherTime > 0) {
					leftTetherTime--;
				}
				if (Projectile.ai[0] >= 0) // {T} Check to accomodate my changes involving setting this value to -1 initially and using .ai instead.
                {
                    // {T} Safety check in case of bad references. No guarantees ai value isn't just 0.
                    var leftTether = Main.projectile[(int)Projectile.ai[0]];
					if (leftTether.type == ModContent.ProjectileType<Tether>())
					{
                        leftTether.velocity = (Main.npc[leftNPC].Center - leftTether.Center) * 0.5f; // {T} Moved this up from a few lines below.
						if (leftTether.ai[0] == 0)
						{
                            leftTether.ai[0] = 1;
                            leftTether.netUpdate = true; // {T} Push a networked event to sync the ai change.
						}
					}
				}
                Projectile.direction = Projectile.spriteDirection = Main.npc[leftNPC].Center.X > Projectile.Center.X ? 1 : -1;
                Main.npc[leftNPC].AddBuff(BuffType<Tethered>(), 60);
				if (Main.npc[leftNPC].Center.X - Projectile.Center.X > 150) {
					if (Main.npc[leftNPC].velocity.X > 0) {
                        Main.npc[leftNPC].velocity.X -= 0.5f;
					}
				}
				if (Main.npc[leftNPC].Center.X - Projectile.Center.X < -150) {
					if (Main.npc[leftNPC].velocity.X < 0) {
                        Main.npc[leftNPC].velocity.X += 0.5f;
					}
				}
				if (Main.npc[leftNPC].Center.Y - Projectile.Center.Y > 150) {
					if (Main.npc[leftNPC].velocity.Y > 0) {
                        Main.npc[leftNPC].velocity.Y -= 0.5f;
					}
				}
				if (Main.npc[leftNPC].Center.Y - Projectile.Center.Y < -150) {
					if (Main.npc[leftNPC].velocity.Y < 0) {
                        Main.npc[leftNPC].velocity.Y += 0.5f;
					}
				}
			}

			if (middleNPC < 0 || !Main.npc[middleNPC].active | middleTetherTime == 0) {
				if (middleTetherTime < 400) {
					middleTetherTime++;
					middleNPC = -1;
				}
                else if (Main.myPlayer == Projectile.owner) // {T} Added owner check. We shouldn't seek targets on clients.
                {
                    var foundNPC = FindClosestNPC(350f); // {T} Tweaks to support using an index instead of an actual NPC object.
                    if (foundNPC != null)
                    {
                        middleNPC = foundNPC.whoAmI;
                    }
                    else
                    {
                        middleNPC = -1;
                    }
                }
				if (Projectile.ai[1] >= 0) // {T} Check to accomodate my changes involving setting this value to -1 initially and using .ai instead.
                {
                    // {T} Safety check in case of bad references. No guarantees ai value isn't just 0.
                    var middleTether = Main.projectile[(int)Projectile.ai[1]];
					if (middleTether.type == ModContent.ProjectileType<Tether>())
					{
                        middleTether.velocity = (Projectile.Center - middleTether.Center) * 0.5f;
                        middleTether.ai[0] = 0;
                        middleTether.netUpdate = true; // {T} Push a networked event to sync the ai change.
					}
				}
            }
			if (middleNPC >= 0) {
				if (middleTetherTime > 0) {
					middleTetherTime--;
				}
				if (Projectile.ai[1] >= 0) // {T} Check to accomodate my changes involving setting this value to -1 initially and using .ai instead.
                {
                    // {T} Safety check in case of bad references. No guarantees ai value isn't just 0.
                    var middleTether = Main.projectile[(int)Projectile.ai[1]];
					if (middleTether.type == ModContent.ProjectileType<Tether>())
					{
                        middleTether.velocity = (Main.npc[middleNPC].Center - middleTether.Center) * 0.5f; // {T} Moved this up from a few lines below.
						if (middleTether.ai[0] == 0)
						{
                            middleTether.ai[0] = 1;
                            middleTether.netUpdate = true; // {T} Push a networked event to sync the ai change.
						}
					}
				}
				Projectile.direction = Projectile.spriteDirection = Main.npc[middleNPC].Center.X > Projectile.Center.X ? 1 : -1;
                Main.npc[middleNPC].AddBuff(BuffType<Tethered>(), 60);
				if (Main.npc[middleNPC].Center.X - Projectile.Center.X > 150) {
					if (Main.npc[middleNPC].velocity.X > 0) {
                        Main.npc[middleNPC].velocity.X -= 0.5f;
					}
				}
				if (Main.npc[middleNPC].Center.X - Projectile.Center.X < -150) {
					if (Main.npc[middleNPC].velocity.X < 0) {
                        Main.npc[middleNPC].velocity.X += 0.5f;
					}
				}
				if (Main.npc[middleNPC].Center.Y - Projectile.Center.Y > 150) {
					if (Main.npc[middleNPC].velocity.Y > 0) {
                        Main.npc[middleNPC].velocity.Y -= 0.5f;
					}
				}
				if (Main.npc[middleNPC].Center.Y - Projectile.Center.Y < -150) {
					if (Main.npc[middleNPC].velocity.Y < 0) {
                        Main.npc[middleNPC].velocity.Y += 0.5f;
					}
				}
			}

			if (rightNPC < 0 || !Main.npc[rightNPC].active || rightTetherTime == 0) {
				if (rightTetherTime < 400)
				{
					rightTetherTime++;
					rightNPC = -1;
				}
				else if (Main.myPlayer == Projectile.owner) // {T} Added owner check. We shouldn't seek targets on clients.
                {
					var foundNPC = FindClosestNPC(350f); // {T} Tweaks to support using an index instead of an actual NPC object.
					if (foundNPC != null)
					{
						rightNPC = foundNPC.whoAmI;
					}
					else
					{
						rightNPC = -1;
					}
				}
				if (Projectile.ai[2] >= 0) // {T} Check to accomodate my changes involving setting this value to -1 initially and using .ai instead.
                {
                    // {T} Safety check in case of bad references. No guarantees ai value isn't just 0.
                    var rightTether = Main.projectile[(int)Projectile.ai[2]];
					if (rightTether.type == ModContent.ProjectileType<Tether>())
					{
                        rightTether.velocity = (Projectile.Center - rightTether.Center) * 0.5f;
                        rightTether.ai[0] = 0;
                        rightTether.netUpdate = true; // {T} Push a networked event to sync the ai change.
					}
				}
            }
			if (rightNPC >= 0) {
				if (rightTetherTime > 0) {
					rightTetherTime--;
				}
				if (Projectile.ai[2] >= 0) // {T} Check to accomodate my changes involving setting this value to -1 initially and using .ai instead.
                {
                    var rightTether = Main.projectile[(int)Projectile.ai[2]];
					if (rightTether.type == ModContent.ProjectileType<Tether>())
					{
                        rightTether.velocity = (Main.npc[rightNPC].Center - rightTether.Center) * 0.5f; // {T} Moved this up from a few lines below.
						if (rightTether.ai[0] == 0)
						{
                            rightTether.ai[0] = 1;
                            rightTether.netUpdate = true; // {T} Push a networked event to sync the ai change.
						}
					}
				}
				Projectile.direction = Projectile.spriteDirection = Main.npc[rightNPC].Center.X > Projectile.Center.X ? 1 : -1;
                Main.npc[rightNPC].AddBuff(BuffType<Tethered>(), 60);
				if (Main.npc[rightNPC].Center.X - Projectile.Center.X > 150) {
					if (Main.npc[rightNPC].velocity.X > 0) {
                        Main.npc[rightNPC].velocity.X -= 0.5f;
					}
				}
				if (Main.npc[rightNPC].Center.X - Projectile.Center.X < -150) {
					if (Main.npc[rightNPC].velocity.X < 0) {
                        Main.npc[rightNPC].velocity.X += 0.5f;
					}
				}
				if (Main.npc[rightNPC].Center.Y - Projectile.Center.Y > 150) {
					if (Main.npc[rightNPC].velocity.Y > 0) {
                        Main.npc[rightNPC].velocity.Y -= 0.5f;
					}
				}
				if (Main.npc[rightNPC].Center.Y - Projectile.Center.Y < -150) {
					if (Main.npc[rightNPC].velocity.Y < 0) {
                        Main.npc[rightNPC].velocity.Y += 0.5f;
					}
				}
			}

			// {T} Compare and sync if needed.
			if (Main.myPlayer == Projectile.owner)
			{
				if(prevLeftNPC != leftNPC || prevMiddleNPC != middleNPC || prevRightNPC != rightNPC)
				{
					Projectile.netUpdate = true;
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
            var leftTether = Main.projectile[(int)Projectile.ai[0]]; // {T} These values get reused in some of the code below, in place of the previous Main.projectile[] refs.
            var middleTether = Main.projectile[(int)Projectile.ai[1]];
            var rightTether = Main.projectile[(int)Projectile.ai[2]];

            Vector2 leftTetherCenter = new Vector2(leftTether.Center.X, leftTether.Center.Y);
			Vector2 middleTetherCenter = new Vector2(middleTether.Center.X, middleTether.Center.Y);
			Vector2 rightTetherCenter = new Vector2(rightTether.Center.X, rightTether.Center.Y);

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

				Main.EntitySpriteDraw(endTexture.Value, leftTether.Center - Main.screenPosition,
					endTexture.Value.Bounds, drawColor, leftChainRotation,
					endTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

				Main.EntitySpriteDraw(endTexture_Glowmask.Value, leftTether.Center - Main.screenPosition,
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

				Main.EntitySpriteDraw(endTexture.Value, middleTether.Center - Main.screenPosition,
					endTexture.Value.Bounds, drawColor, middleChainRotation,
					endTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

				Main.EntitySpriteDraw(endTexture_Glowmask.Value, middleTether.Center - Main.screenPosition,
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

				Main.EntitySpriteDraw(endTexture.Value, rightTether.Center - Main.screenPosition,
					endTexture.Value.Bounds, drawColor, rightChainRotation,
					endTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

				Main.EntitySpriteDraw(endTexture_Glowmask.Value, rightTether.Center - Main.screenPosition,
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
