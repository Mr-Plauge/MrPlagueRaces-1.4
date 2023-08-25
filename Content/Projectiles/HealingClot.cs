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
	public class HealingClot : ModProjectile
	{
		float distance = 1f;

		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Healing Clot");
		}

		public override void SetDefaults() {
			Projectile.width = 6;
			Projectile.height = 6;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			var vampirePlayer = player.GetModPlayer<VampirePlayer>();
			Vector2 velocity = Main.projectile[vampirePlayer.LeechTongue].Center - new Vector2(player.Center.X, player.Center.Y - 1);
			float distanceToPlayer = velocity.Length();
			float distanceForm = 2f / distanceToPlayer;
			if (distanceForm > 0.05f) {
				distanceForm = 0.05f;
			}
			distance -= distanceForm;
			Projectile.Center = new Vector2(player.Center.X, player.Center.Y - 1) + (velocity * distance);
			if (Projectile.Center.X - player.Center.X < 5 && Projectile.Center.X - player.Center.X > -5 && Projectile.Center.Y - player.Center.Y < 5 && Projectile.Center.Y - player.Center.Y > -5) {
				Projectile.Kill();
				SoundEngine.PlaySound(SoundID.Item3, Projectile.Center);
				player.HealEffect(player.statLifeMax2 / 25);
				player.statLife += player.statLifeMax2 / 25;
			}
			for (int i = 0; i < 2; i++) {
				int dust = Dust.NewDust(Projectile.position, 0, 0, 278);
				Main.dust[dust].color = player.eyeColor;
				Main.dust[dust].noGravity = true;
			}
			if (player.mount.Type != MountType<StealthBat>() || player.dead) {
				Projectile.Kill();
			}
		}
	}
}
