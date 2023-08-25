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
using MrPlagueRaces.Common.Races.Lihzahrd;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Mounts
{
	public class Crawl : ModMount
	{
		public override void SetStaticDefaults() {
			MountData.heightBoost = -28;
			MountData.fallDamage = 1f;
			MountData.fatigueMax = 0;
			MountData.fallDamage = 1f;
			MountData.runSpeed = 1f;
			MountData.acceleration = 0.16f;
			MountData.jumpHeight = 10;
			MountData.jumpSpeed = 4f;
			MountData.totalFrames = 20;
			int[] array = new int[MountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = 0;
			}
			MountData.playerYOffsets = array;

			MountData.standingFrameCount = 1;
			MountData.standingFrameDelay = 0;
			MountData.standingFrameStart = 0;

			MountData.runningFrameCount = 14;
			MountData.runningFrameDelay = 4;
			MountData.runningFrameStart = 6;

			MountData.flyingFrameCount = 0;
			MountData.flyingFrameDelay = 0;
			MountData.flyingFrameStart = 0;

			MountData.inAirFrameCount = 1;
			MountData.inAirFrameDelay = 0;
			MountData.inAirFrameStart = 5;

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
		
		public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow) {
			var lihzahrdPlayer = drawPlayer.GetModPlayer<LihzahrdPlayer>();
			if (lihzahrdPlayer.crawlFrame == -1) {
				drawPlayer.headFrame.Y = frame.Y;
				MountData.bodyFrame = frame.Y / 56;
				drawPlayer.legFrame.Y = frame.Y;
			}
			else {
				drawPlayer.headFrame.Y = drawPlayer.headFrame.Y * lihzahrdPlayer.crawlFrame;
				MountData.bodyFrame = lihzahrdPlayer.crawlFrame;
				drawPlayer.legFrame.Y = lihzahrdPlayer.legFrame * 56;
			}
			return false;
		}

		public override void UpdateEffects(Player player)
		{
			MountData.runSpeed = (player.maxRunSpeed / 3) + player.accRunSpeed;
		}
	}
}