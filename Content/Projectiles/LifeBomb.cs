using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Common.Races.Lihzahrd;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class LifeBomb : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("LifeBomb");
		}

		public override void SetDefaults() {
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.aiStyle = 14;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 100;
		}

		public override bool PreAI()
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6);
			return true;
		}

		public override void Kill(int timeLeft) {
			Player player = Main.player[Projectile.owner];
			for (int i = 0; i < 20; i++) {
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 10f;
			}
			for (int k = 0; k < Main.maxPlayers; k++) {
				Player target = Main.player[k];
				if (target.active) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
					if (sqrDistanceToTarget / 16 < 150) {
						SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, target.Center);
						target.HealEffect(player.statDefense / 5);
						target.statLife += player.statDefense / 5;
					}
				}
			}
			SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Projectile.Center);
		}

		public override void PostDraw(Color lightColor) {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (Projectile.spriteDirection == -1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Player player = Main.player[Projectile.owner];
			Texture2D glowmask = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/LifeBomb_Glowmask");

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
