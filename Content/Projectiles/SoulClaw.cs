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
	public class SoulClaw : ModProjectile
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Soul Claw");
			Main.projFrames[Projectile.type] = 25;
		}

		public override void SetDefaults() {
			Projectile.width = 188;
			Projectile.height = 126;
			Projectile.aiStyle = 75;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ownerHitCheck = true;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			Vector2 vector31 = player.RotatedRelativePoint(player.MountedCenter);
			Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : 1;
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (++Projectile.frame >= Main.projFrames[Projectile.type])
			{
				Projectile.frame = 0;
			}
			Projectile.soundDelay--;
			if (Projectile.soundDelay <= 0)
			{
				SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce, Projectile.Center);
				Projectile.soundDelay = 12;
			}
			if (Main.myPlayer == Projectile.owner)
			{
				if (MrPlagueRaces.RaceAbilityKeybind2.Current && !MrPlagueRaces.RaceAbilityKeybind1.Current && !player.dead)
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

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			target.AddBuff(BuffType<SoulFracture>(), 360);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			Player player = Main.player[Projectile.owner];
			target.AddBuff(BuffType<SoulFracture>(), 360);
		}
	}
}
