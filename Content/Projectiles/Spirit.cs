using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Common.Races.Skeleton;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class Spirit : ModProjectile
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Spirit");
			Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults() {
			Player player = Main.player[Projectile.owner];
			Projectile.width = 28;
			Projectile.height = 34;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
			Projectile.rotation = Projectile.velocity.ToRotation() - 1.575f;
			Projectile.ai[0] += 1f;
			Projectile.Center = player.Center;
			Projectile.gfxOffY = player.gfxOffY;
			Projectile.frameCounter++;
			if (Projectile.frameCounter > 3)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame > 2)
				{
					Projectile.frame = 0;
				}
			}
			if (skeletonPlayer.spirit == 0) {
				Projectile.Kill();
			}
			int num2536 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 261, Projectile.velocity.X, Projectile.velocity.Y);
			Dust dust76 = Main.dust[num2536];
			Dust dust138 = dust76;
			dust138.velocity *= 0.1f;
			Main.dust[num2536].scale = 1.3f;
			Main.dust[num2536].noGravity = true;
			Main.dust[num2536].color = player.eyeColor;
			Lighting.AddLight(Projectile.Center, player.eyeColor.ToVector3());
			if (Main.myPlayer == Projectile.owner)
			{
				Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * 1f;
				Projectile.velocity = velocity;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			return new Color(player.eyeColor.R, player.eyeColor.G, player.eyeColor.B, 0);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
			player.ghost = false;
			for (int num884 = 0; num884 < 50; num884++)
			{
				int num885 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 261, Projectile.velocity.X, Projectile.velocity.Y);
				Dust dust210 = Main.dust[num885];
				Dust dust218 = dust210;
				dust218.velocity *= 2f;
				Main.dust[num885].noGravity = true;
				Main.dust[num885].scale = 1.4f;
				Main.dust[num885].color = player.eyeColor;
			}
			switch (Main.rand.Next(4))
			{
				case 0:
					player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " had their soul torn asunder."), 10.0, 0, false);
					break;
				case 1:
					player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " was spiritually destroyed."), 10.0, 0, false);
					break;
				case 2:
					player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " did not achieve a second wind."), 10.0, 0, false);
					break;
				default:
					player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " faded away."), 10.0, 0, false);
					break;
			}
			SoundEngine.PlaySound(SoundID.NPCDeath39, Projectile.Center);
			Projectile.Kill();
		}

		public override bool? CanDamage()
		{
			return Projectile.ai[0] > 60;
		}
	}
}
