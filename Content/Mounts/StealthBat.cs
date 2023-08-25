using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using MrPlagueRaces.Common.Races.Vampire;
using MrPlagueRaces.Content.Projectiles;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Mounts
{
	public class StealthBat : ModMount
	{
		public override void SetStaticDefaults() {
			MountData.heightBoost = -28;
			MountData.fallDamage = 1f;
			MountData.flightTimeMax = 100;
			MountData.fatigueMax = 0;
			MountData.fallDamage = 0f;
			MountData.runSpeed = 1f;
			MountData.acceleration = 0.16f;
			MountData.jumpHeight = 10;
			MountData.jumpSpeed = 4f;
			MountData.totalFrames = 6;
			int[] array = new int[MountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = -8;
			}
			MountData.playerYOffsets = array;
			MountData.bodyFrame = 8;
			
			MountData.xOffset = 0;
			MountData.yOffset = -2;
			MountData.playerHeadOffset = -18;

			MountData.standingFrameCount = 1;
			MountData.standingFrameDelay = 0;
			MountData.standingFrameStart = 4;

			MountData.runningFrameCount = 2;
			MountData.runningFrameDelay = 12;
			MountData.runningFrameStart = 4;

			MountData.flyingFrameCount = 4;
			MountData.flyingFrameDelay = 6;
			MountData.flyingFrameStart = 0;

			MountData.inAirFrameCount = 1;
			MountData.inAirFrameDelay = 0;
			MountData.inAirFrameStart = 1;

			MountData.idleFrameCount = 0;
			MountData.idleFrameDelay = 0;
			MountData.idleFrameStart = 0;
			
			MountData.swimFrameCount = MountData.inAirFrameCount;
			MountData.swimFrameDelay = MountData.inAirFrameDelay;
			MountData.swimFrameStart = MountData.inAirFrameStart;
			
			if (!Main.dedServ) {
				MountData.textureWidth = MountData.backTexture.Width();
				MountData.textureHeight = MountData.backTexture.Height();
			}
		}

		public override void SetMount(Player player, ref bool skipDust) {
			skipDust = true;
		}

		public override void Dismount(Player player, ref bool skipDust) {
			skipDust = true;
		}

		public override void UpdateEffects(Player player)
		{
			if (player.velocity.Y != 0)
			{
				MountData.runSpeed = (player.moveSpeed) + (player.accRunSpeed) + (player.wingTimeMax / 30);
			}
			else
			{
				MountData.runSpeed = 1f;
			}
			MountData.flightTimeMax = player.wingTimeMax + 100;
		}

		public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow) {
			var mrPlagueRacesPlayer = drawPlayer.GetModPlayer<MrPlagueRacesPlayer>();
			var vampirePlayer = drawPlayer.GetModPlayer<VampirePlayer>();
			float rotate;
			rotate = drawPlayer.velocity.X * 0.1f;
			if ((double)rotate < -0.3)
			{
				rotate = -0.3f;
			}
			if ((double)rotate > 0.3)
			{
				rotate = 0.3f;
			}
			Texture2D textureEyes = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Mounts/StealthBat_Eyes");
			Texture2D textureSkin = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Mounts/StealthBat_Skin");
			Texture2D textureDetail = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Mounts/StealthBat_Detail");
			Asset<Texture2D> textureLeech = ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/LeechTongueChain");
			Asset<Texture2D> textureLeechHead = ModContent.Request<Texture2D>("MrPlagueRaces/Content/Projectiles/LeechTongueHook");
			Main.EntitySpriteDraw(textureEyes, drawPosition, frame, drawPlayer.eyeColor, rotation + rotate, drawOrigin, drawScale, spriteEffects, 0);
			Main.EntitySpriteDraw(textureSkin, drawPosition, frame,  mrPlagueRacesPlayer.colorSkin, rotation + rotate, drawOrigin, drawScale, spriteEffects, 0);
			Main.EntitySpriteDraw(textureDetail, drawPosition, frame, mrPlagueRacesPlayer.colorDetail, rotation + rotate, drawOrigin, drawScale, spriteEffects, 0);
			if (drawPlayer.ownedProjectileCounts[ProjectileType<LeechTongue>()] != 0) {
				Vector2 playerCenter = Main.projectile[vampirePlayer.LeechTongue].Center;
				Vector2 center = new Vector2(drawPlayer.Center.X, drawPlayer.Center.Y - 1f);
				Vector2 directionToPlayer = playerCenter - new Vector2(drawPlayer.Center.X, drawPlayer.Center.Y - 1f);
				float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
				float distanceToPlayer = directionToPlayer.Length();
				if (directionToPlayer.Length() > 420) {
					Main.projectile[vampirePlayer.LeechTongue].ai[0] = 60;
				}
				while (distanceToPlayer > 10f && !float.IsNaN(distanceToPlayer)) {
					directionToPlayer /= distanceToPlayer; // get unit vector
					directionToPlayer *= textureLeech.Height(); // multiply by chain link length

					center += directionToPlayer; // update draw position
					directionToPlayer = playerCenter - center; // update distance
					distanceToPlayer = directionToPlayer.Length();

					Main.EntitySpriteDraw(textureLeech.Value, center - Main.screenPosition, textureLeech.Value.Bounds, mrPlagueRacesPlayer.colorEyes, chainRotation, new Vector2(textureLeech.Size().X * 0.5f, textureLeech.Size().Y * 0.5f), 1f, SpriteEffects.None, 0);
					Main.EntitySpriteDraw(textureLeechHead.Value, Main.projectile[vampirePlayer.LeechTongue].Center - Main.screenPosition, textureLeechHead.Value.Bounds, mrPlagueRacesPlayer.colorEyes, chainRotation, new Vector2(textureLeechHead.Size().X * 0.5f, textureLeechHead.Size().Y * 0.5f), 1f, SpriteEffects.None, 0);
				}
			}
			return false;
		}
	}
}