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
	// {T} targetNPC has now been changed to use Projectile.ai[1]. This way, the target is networked!
	// Also, since we're using ai[1], instead of storing the actual NPC reference, we're just storing its whoAmI value (AKA it's index in Main.npc).
	// As a consequence of this change, we must set ai[1] to -1 when creating the projectile or else the tongue will fly off into nowhere immediately.
	public class LeechTongue : ModProjectile
	{
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
			if (Projectile.ai[1] < 0) {
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
				NPC targetNPC = Main.npc[(int)Projectile.ai[1]];
				Projectile.Center = targetNPC.Center;
				if (!targetNPC.active || targetNPC.life <= (player.statLifeMax2 / 25) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<VampireConfig>().vampireLifeSteal])) {
					Projectile.ai[1] = -1;
				}
				if (player.ownedProjectileCounts[ProjectileType<HealingClot>()] == 0 && targetNPC.life > (player.statLifeMax2 / 25) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<VampireConfig>().vampireLifeSteal])) {
					Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<HealingClot>(), 0, 0, Projectile.owner);
					if (targetNPC.life > (player.statLifeMax2 / 25) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<VampireConfig>().vampireLifeSteal])) {
						CombatText.NewText(new Rectangle((int)targetNPC.position.X, (int)targetNPC.position.Y, targetNPC.width, targetNPC.height), CombatText.LifeRegenNegative, 5);
						targetNPC.life -= (player.statLifeMax2 / 25) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<VampireConfig>().vampireLifeSteal]);
					}
				}
				vampirePlayer.Leeching = true;
			}
			if (player.mount.Type != MountType<StealthBat>() || player.dead) {
				Projectile.Kill();
			}
			if (Projectile.ai[0] >= 60 && Projectile.ai[1] >= 0) {
                NPC targetNPC = Main.npc[(int)Projectile.ai[1]];
                if (targetNPC.active) {
					SoundEngine.PlaySound(SoundID.NPCHit13, Projectile.Center);
				}
				Projectile.ai[1] = -1;
			}
		}

		public override void OnKill(int timeLeft) {
			Player player = Main.player[Projectile.owner];
            // var vampirePlayer = player.GetModPlayer<VampirePlayer>(); // {T} This line isn't used.
            Projectile.ai[1] = -1;
			SoundEngine.PlaySound(SoundID.Item112, player.Center);
		}

		public override bool? CanDamage()
		{
			return Projectile.ai[0] < 30;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			if (target.life > (player.statLifeMax2 / 25) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<VampireConfig>().vampireLifeSteal])) {
                Projectile.ai[1] = target.whoAmI;
			}
			Projectile.ai[0] = 30;

            // {T} Force a network update! This is very important any time we change ai values in client-only functions.
            // The alternative, FYI, is to use NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
            // where Projectile.whoAmI will be whatever the index of the projectile is in the owner's Main.projectile array.
            Projectile.netUpdate = true;
        }
    }
}
