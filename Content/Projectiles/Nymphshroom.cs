using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class Nymphshroom : ModProjectile
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Nymphshroom");
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults() {
			Player player = Main.player[Projectile.owner];
			Projectile.width = 30;
			Projectile.height = 38;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Projectile.timeLeft = 900;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			Projectile.ai[0] += 1f;
			Projectile.frameCounter++;
			if (Projectile.alpha > 0) {
				Projectile.alpha -= 15;
			}
			if (Projectile.alpha < 0) {
				Projectile.alpha = 0;
			}
			Projectile.velocity = new Vector2(0f, (float)Math.Sin((double)((float)Math.PI * 2f * Projectile.ai[0] / 180f)) * 0.15f);
			if (Projectile.frameCounter > 4)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame > 3)
				{
					Projectile.frame = 0;
				}
			}
			if (Projectile.ai[0] >= 180f)
			{
				Projectile.ai[0] = 0f;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			player.AddBuff(BuffType<Mycobulwark>(), 120);
			player.Teleport(Projectile.position, 15);
			SoundEngine.PlaySound(SoundID.NPCDeath58, Projectile.Center);
			SoundEngine.PlaySound(SoundID.NPCDeath55, Projectile.Center);
			for (int i = 0; i < 3; i++) {
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X + Main.rand.Next(60) - Main.rand.Next(60), Projectile.Center.Y + Main.rand.Next(60) - Main.rand.Next(60), 0f, 0f, ProjectileType<HealingSpores>(), 0, 0, Projectile.owner);
			}
			Projectile.Kill();
		}

		public override void Kill(int timeLeft) {
			for (int i = 0; i < 6; i++) {
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Projectile.Center.X, Projectile.Center.Y,  Main.rand.Next(3) - Main.rand.Next(3),  Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<BurstSpores>(), 0, 0, Projectile.owner);
			}
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
		}

		public override bool PreDraw(ref Color lightColor) {
			Player player = Main.player[Projectile.owner];
			Texture2D textureHair = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/Nymphshroom_Hair");
			Texture2D textureSkin = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/Nymphshroom_Skin");
			Texture2D textureEyes = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/Nymphshroom_Eyes");

			int frameHeight = textureHair.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, textureHair.Width, frameHeight);

			Vector2 origin = sourceRectangle.Size() / 2f;
			Main.EntitySpriteDraw(textureHair,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, new Color(player.hairColor.ToVector4() * Projectile.Opacity), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(textureSkin,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, new Color(player.skinColor.ToVector4() * lightColor.ToVector4() * Projectile.Opacity), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(textureEyes,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, new Color(player.eyeColor.ToVector4() * Projectile.Opacity), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
