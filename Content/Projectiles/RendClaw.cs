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
	public class RendClaw : ModProjectile
	{
		private int frameInterval;

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Rend Claw");
			Main.projFrames[Projectile.type] = 6;
		}

		public override void SetDefaults() {
			Projectile.width = 240;
			Projectile.height = 96;
			Projectile.aiStyle = 75;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ownerHitCheck = true;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			Vector2 vector31 = player.RotatedRelativePoint(player.MountedCenter);
			if (frameInterval >= 2) {
				frameInterval = 0;
				Projectile.frame += 1;
			}
			frameInterval++;
			if (Main.myPlayer == Projectile.owner)
			{
				if (Projectile.frame < 6 && !player.dead)
				{
					float num123 = 1f;
					num123 = 15f * Projectile.scale;
					Vector2 vector34 = Main.MouseWorld - vector31;
					vector34.Normalize();
					if (vector34.HasNaNs())
					{
						vector34 = Vector2.UnitX * (float)player.direction;
					}
					vector34 *= num123;
					Projectile.velocity = vector34;
					Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : 1;
					Projectile.rotation = Projectile.velocity.ToRotation();
				}
				else
				{
					Projectile.Kill();
				}
			}
			Vector2 vector40 = Projectile.Center + Projectile.velocity * 3f;
			Lighting.AddLight(Projectile.Center, player.eyeColor.ToVector3());
		}

		public override Color? GetAlpha(Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			return new Color(player.eyeColor.ToVector4() * Projectile.Opacity);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Player player = Main.player[Projectile.owner];
			target.AddBuff(BuffType<SoulFracture>(), 360);
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			Player player = Main.player[Projectile.owner];
			target.AddBuff(BuffType<SoulFracture>(), 360);
		}
	}
}
