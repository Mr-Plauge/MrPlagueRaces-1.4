using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Content.Mounts;
using MrPlagueRaces.Common.Races.Vampire;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class LeechTongue : ModProjectile
	{
		public NPC targetNPC = null;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Leech Tongue");
		}

		public override void SetDefaults() {
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			var vampirePlayer = player.GetModPlayer<VampirePlayer>();
			if (targetNPC == null) {
				Projectile.ai[0] += 1f;
				if (Projectile.ai[0] > 30) {
					Vector2 velocity = Vector2.Normalize(player.Center - Projectile.Center) * 10f;
					Projectile.velocity = velocity;
					if (Projectile.Center.X - player.Center.X < 5 && Projectile.Center.X - player.Center.X > -5 && Projectile.Center.Y - player.Center.Y < 5 && Projectile.Center.Y - player.Center.Y > -5) {
						Projectile.Kill();
					}
				}
				vampirePlayer.Leeching = false;
			}
			else {
				Projectile.Center = targetNPC.Center;
				if (!targetNPC.active || targetNPC.life < 5) {
					targetNPC = null;
				}
				if (player.ownedProjectileCounts[ProjectileType<HealingClot>()] == 0 && targetNPC.life > 5) {
					Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<HealingClot>(), 0, 0, Projectile.owner);
					if (targetNPC.life > 5) {
						CombatText.NewText(new Rectangle((int)targetNPC.position.X, (int)targetNPC.position.Y, targetNPC.width, targetNPC.height), CombatText.LifeRegenNegative, 5);
						targetNPC.life -= 5;
					}
				}
				vampirePlayer.Leeching = true;
			}
			if (player.mount.Type != MountType<StealthBat>() || player.dead) {
				Projectile.Kill();
			}
			if (Projectile.ai[0] >= 60 && targetNPC != null) {
				if (targetNPC.active) {
					SoundEngine.PlaySound(SoundID.NPCHit13, Projectile.Center);
				}
				targetNPC = null;
			}
		}

		public override void Kill(int timeLeft) {
			Player player = Main.player[Projectile.owner];
			var vampirePlayer = player.GetModPlayer<VampirePlayer>();
			targetNPC = null;
			SoundEngine.PlaySound(SoundID.Item112, player.Center);
		}

		public override bool? CanDamage()
		{
			return Projectile.ai[0] < 30;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			if (target.life > 5) {
				targetNPC = target;
			}
			Projectile.ai[0] = 30;
		}
	}
}
