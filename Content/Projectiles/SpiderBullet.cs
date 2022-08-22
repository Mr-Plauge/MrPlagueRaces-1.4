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
	public class SpiderBullet : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Spider Bullet");
		}

		public override void SetDefaults() {
			Projectile.width = 22;
			Projectile.height = 6;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.aiStyle = 0;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 100;
		}

		public override bool PreAI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			return true;
		}

		public override void Kill(int timeLeft) {
			for (int i = 0; i < 10; i++) {
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6);
			}
			SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);
		}

		public override void PostDraw(Color lightColor) {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (Projectile.spriteDirection == -1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Player player = Main.player[Projectile.owner];
			Texture2D glowmask = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/SpiderBullet_Glowmask");

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
