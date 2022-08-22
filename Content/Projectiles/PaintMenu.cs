using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Common.Races.Fluftrodon;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class PaintMenu : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Paint Menu");
		}

		public override void SetDefaults() {
			Projectile.width = 36;
			Projectile.height = 36;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ownerHitCheck = true;
		}

		public override void AI() {
			Projectile.timeLeft++;
			Player player = Main.player[Projectile.owner];
			var fluftrodonPlayer = player.GetModPlayer<FluftrodonPlayer>();
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (Main.myPlayer == Projectile.owner)
			{
				Projectile.position = new Vector2(Main.MouseWorld.X + 9, Main.MouseWorld.Y + 11).Floor();
			}
			if (fluftrodonPlayer.closeMenu) {
				Projectile.Kill();
				fluftrodonPlayer.closeMenu = false;
			}
		}

		public override bool PreDraw(ref Color lightColor) {
			Player player = Main.player[Projectile.owner];
			var fluftrodonPlayer = player.GetModPlayer<FluftrodonPlayer>();
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			Texture2D textureSkin = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/PaintMenu_Skin");
			Texture2D textureHair = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/PaintMenu_Hair");
			Texture2D texturePaint = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/PaintMenu_Paint");

			int frameHeight = texturePaint.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, texturePaint.Width, frameHeight);

			Vector2 origin = sourceRectangle.Size() / 2f;
			Main.EntitySpriteDraw(textureSkin,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, new Color(player.skinColor.ToVector4() * Projectile.Opacity), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(textureHair,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, new Color(player.hairColor.ToVector4() * Projectile.Opacity), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(texturePaint,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, new Color(fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint].ToVector4() * Projectile.Opacity), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}
	}
}
