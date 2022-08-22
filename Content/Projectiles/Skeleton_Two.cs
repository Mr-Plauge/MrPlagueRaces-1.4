using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Common.Races.Skeleton;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class Skeleton_Two : ModProjectile
	{
		private Texture2D[] Skull_Texture = new Texture2D[165];
		private Texture2D[] Eyes_Texture = new Texture2D[165];
		private float eyeGlow = 1f;

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Skeleton");
		}

		public override void SetDefaults() {
			Player player = Main.player[Projectile.owner];
			Projectile.width = 52;
			Projectile.height = 24;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
			Projectile.ai[0] += 1f;
			Projectile.velocity.Y += 0.5f;
			if (skeletonPlayer.teleportTwo) {
				skeletonPlayer.teleportTwo = false;
				SoundEngine.PlaySound(SoundID.DD2_SkeletonSummoned, Projectile.Center);
				player.Teleport(new Vector2(Projectile.position.X + 26, Projectile.position.Y - 18), 15);
				for (int i = 0; i < 6; i++) {
					Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y,  Main.rand.Next(3) - Main.rand.Next(3),  Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, Projectile.owner);
				}
				Projectile.Kill();
			}
			if (Projectile.ai[0] >= 180f)
			{
				Projectile.ai[0] = 180f;
			}
			if (Projectile.ai[0] > 90)
			{
				eyeGlow -= 0.005f * (Projectile.ai[0] / 10);
			}
			else {
				eyeGlow = 1f;
			}
			Projectile.timeLeft++;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override void Kill(int timeLeft) {
			for (int i = 0; i < 6; i++) {
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y,  Main.rand.Next(3) - Main.rand.Next(3),  Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, Projectile.owner);
			}
			SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.Center);
		}

		public override bool PreDraw(ref Color lightColor) {
			Player player = Main.player[Projectile.owner];
			Skull_Texture[player.hair] = (Texture2D)ModContent.Request<Texture2D>($"MrPlagueRaces/Assets/Textures/Players/Races/Skeleton/Male/ColorSkin/Hairstyles/Hair_{player.hair + 1}");
			Eyes_Texture[player.hair] = (Texture2D)ModContent.Request<Texture2D>($"MrPlagueRaces/Assets/Textures/Players/Races/Skeleton/Male/ColorEyes/Glowmask/Hairstyles/Hair_{player.hair + 1}");
			Texture2D textureSkin = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/Skeleton_Two");

			int frameHeight = textureSkin.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, textureSkin.Width, frameHeight);

			int frameHeightHead = Skull_Texture[player.hair].Height / 14;
			Rectangle sourceRectangleHead = new Rectangle(0, 0, Skull_Texture[player.hair].Width, frameHeightHead);

			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = Projectile.GetAlpha(lightColor);
			Main.EntitySpriteDraw(textureSkin,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, new Color(player.skinColor.ToVector4() * lightColor.ToVector4() * Projectile.Opacity), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(Skull_Texture[player.hair],
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangleHead, new Color(player.skinColor.ToVector4() * lightColor.ToVector4() * Projectile.Opacity), 1.5753f, new Vector2(origin.X - 10, origin.Y + 26), Projectile.scale, SpriteEffects.FlipHorizontally, 0);
			Main.EntitySpriteDraw(Eyes_Texture[player.hair],
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangleHead, new Color(player.eyeColor.ToVector4() * Projectile.Opacity * eyeGlow), 1.5753f, new Vector2(origin.X - 10, origin.Y + 26), Projectile.scale, SpriteEffects.FlipHorizontally, 0);
			return false;
		}
	}
}
