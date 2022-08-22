using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Common.Races.Kobold;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class BombardingFlame : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Bombarding Flame");
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults() {
			Player player = Main.player[Projectile.owner];
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Projectile.aiStyle = 14;
			Projectile.timeLeft = 300;
		}

		public override bool PreAI()
		{
			if (Main.rand.Next(5) < 4) {
				Dust dust19 = Dust.NewDustDirect(Projectile.Center, 0, 0, 27, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 180, default(Color), 1.95f);
				dust19.noGravity = true;
				dust19.velocity *= 0.75f;
				dust19.velocity.X *= 0.75f;
				dust19.velocity.Y -= 1f;
				if (Main.rand.Next(4) == 0)
				{
					dust19.noGravity = false;
					dust19.scale *= 0.5f;
				}
			}
			return true;
		}
	}
}
