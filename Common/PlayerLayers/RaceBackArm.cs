using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace MrPlagueRaces.Common
{
	public class RaceBackArm : PlayerDrawLayer
	{
		private Asset<Texture2D>[] Shirt_Texture = new Asset<Texture2D>[13];
		private Asset<Texture2D>[] Undershirt_Texture = new Asset<Texture2D>[13];
		private Asset<Texture2D>[] ShirtAddition_Texture = new Asset<Texture2D>[13];

		private Asset<Texture2D>[] Arm_Texture = new Asset<Texture2D>[10];
		private Asset<Texture2D>[] Hand_Texture = new Asset<Texture2D>[10];
		private string[] PlayerColors = { "ColorSkin", "ColorDetail", "Colorless", "ColorEyes", "ColorHair", "ColorSkin/Glowmask", "ColorDetail/Glowmask", "Colorless/Glowmask", "ColorEyes/Glowmask", "ColorHair/Glowmask" };

		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Skin);

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) 
		{
			return (drawInfo.skinVar < 10);
		}

		protected override void Draw(ref PlayerDrawSet drawInfo) 
		{
			Player drawPlayer = drawInfo.drawPlayer;
			Vector2 helmetOffset = drawInfo.helmetOffset;

			int[] male = { 0, 2, 1, 3, 8 };
			int[] female = { 4, 6, 5, 7, 9 };

			var mrPlagueRacesPlayer = drawPlayer.GetModPlayer<MrPlagueRacesPlayer>();

			if (mrPlagueRacesPlayer.race != null) {
			
				TextureAssets.Players[drawInfo.skinVar, 5] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 7] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");

				for (int i = 0; i < 5; i++)
				{
					Shirt_Texture[male[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Shirt", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Shirt");
					Undershirt_Texture[male[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Undershirt", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Undershirt");
					ShirtAddition_Texture[male[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/ShirtAddition", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/ShirtAddition");
				}

				for (int i = 0; i < 5; i++)
				{
					Shirt_Texture[female[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Shirt", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/Shirt");
					Undershirt_Texture[female[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Undershirt", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/Undershirt");
					ShirtAddition_Texture[female[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/ShirtAddition", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/ShirtAddition");
				}

				for (int i = 0; i < 10; i++)
				{
					Arm_Texture[i] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"{PlayerColors[i]}/Arms");
					Hand_Texture[i] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"{PlayerColors[i]}/Hands");
				}
				
				Vector2 bodyPosition = new Vector2((float)(int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawPlayer.bodyFrame.Width / 2) + (float)(drawPlayer.width / 2)), (float)(int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2((float)(drawPlayer.bodyFrame.Width / 2), (float)(drawPlayer.bodyFrame.Height / 2));
				Vector2 backArmPosition = new Vector2((float)(int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawPlayer.bodyFrame.Width / 2) + (float)(drawPlayer.width / 2)), (float)(int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2((float)(drawPlayer.bodyFrame.Width / 2), (float)(drawPlayer.bodyFrame.Height / 2));
				Vector2 value = Main.OffsetsPlayerHeadgear[drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height];
				value.Y -= 2f;
				backArmPosition += value * (float)(-((Enum)drawInfo.playerEffect).HasFlag((Enum)(object)(SpriteEffects)2).ToDirectionInt());
				Vector2 compositeOffset_BackArm = new Vector2((float)(6 * ((!((Enum)drawInfo.playerEffect).HasFlag((Enum)(object)(SpriteEffects)1)) ? 1 : (-1))), (float)(2 * ((!((Enum)drawInfo.playerEffect).HasFlag((Enum)(object)(SpriteEffects)2)) ? 1 : (-1))));
				backArmPosition.Y += drawInfo.torsoOffset;
				float bodyRotation = drawPlayer.bodyRotation;
				backArmPosition += compositeOffset_BackArm;
				Vector2 position4 = backArmPosition;
				Vector2 bodyVect2 = drawInfo.bodyVect;
				position4 += drawInfo.backShoulderOffset;
				bodyVect2 += compositeOffset_BackArm;
				float rotation = bodyRotation + drawInfo.compositeBackArmRotation;
				bool flag = false;
				if (drawPlayer.body > 0)
				{
					if (drawInfo.armorHidesArms)
					{
						if (!drawInfo.hidesTopSkin)
						{
							MakeColoredDrawDatas(ref drawInfo, Arm_Texture, null, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0);
						}
						if (!flag && !drawInfo.hidesTopSkin)
						{
							MakeColoredDrawDatas(ref drawInfo, Hand_Texture, null, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0);
							flag = true;
						}
						if (drawPlayer.armor[1].type == ItemID.FamiliarShirt || drawPlayer.armor[11].type == ItemID.FamiliarShirt)
						{
							drawInfo.DrawDataCache.Add(new DrawData(Undershirt_Texture[drawInfo.skinVar].Value, backArmPosition, drawInfo.compBackArmFrame, drawInfo.colorUnderShirt, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0));
							drawInfo.DrawDataCache.Add(new DrawData(ShirtAddition_Texture[drawInfo.skinVar].Value, backArmPosition, drawInfo.compBackArmFrame, drawInfo.colorShirt, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0));
							drawInfo.DrawDataCache.Add(new DrawData(Undershirt_Texture[drawInfo.skinVar].Value, bodyPosition, drawInfo.compBackShoulderFrame, drawInfo.colorUnderShirt, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0));
							drawInfo.DrawDataCache.Add(new DrawData(Shirt_Texture[drawInfo.skinVar].Value, bodyPosition, drawInfo.compBackShoulderFrame, drawInfo.colorShirt, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0));
						}
						drawPlayer.invis = true;
					}
				}
				if (!drawPlayer.invis)
				{
					if (!drawInfo.hidesTopSkin)
					{
						if (!drawPlayer.invis)
						{
							MakeColoredDrawDatas(ref drawInfo, Arm_Texture, null, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0);
						}
						if (!flag && !drawInfo.hidesTopSkin)
						{
							MakeColoredDrawDatas(ref drawInfo, Hand_Texture, null, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0);
							flag = true;
						}
						if (drawPlayer.armor[1].type == ItemID.FamiliarShirt || drawPlayer.armor[11].type == ItemID.FamiliarShirt)
						{
							drawInfo.DrawDataCache.Add(new DrawData(Undershirt_Texture[drawInfo.skinVar].Value, backArmPosition, drawInfo.compBackArmFrame, drawInfo.colorUnderShirt, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0));
							drawInfo.DrawDataCache.Add(new DrawData(ShirtAddition_Texture[drawInfo.skinVar].Value, backArmPosition, drawInfo.compBackArmFrame, drawInfo.colorShirt, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0));
							drawInfo.DrawDataCache.Add(new DrawData(Undershirt_Texture[drawInfo.skinVar].Value, bodyPosition, drawInfo.compBackShoulderFrame, drawInfo.colorUnderShirt, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0));
							drawInfo.DrawDataCache.Add(new DrawData(Shirt_Texture[drawInfo.skinVar].Value, bodyPosition, drawInfo.compBackShoulderFrame, drawInfo.colorShirt, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0));
						}
					}
				}
			}
		}

		private void MakeColoredDrawDatas(ref PlayerDrawSet drawInfo, Asset<Texture2D>[] texture, Asset<Texture2D>[,] textureHair, Vector2 position, Rectangle? sourceRect, float rotation, Vector2 origin, float scale, SpriteEffects effect, int inactiveLayerDepth)
		{
			DrawData drawData;
			Player drawPlayer = drawInfo.drawPlayer;
			int index;
			for (index = 0; index < 10; index++)
			{
				if (textureHair != null && textureHair[index, drawPlayer.hair] != ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank"))
				{
					drawData = new DrawData(textureHair[index, drawPlayer.hair].Value, position, sourceRect, PlayerColor(ref drawInfo, index), rotation, origin, scale, effect, 0);
					drawData.shader = PlayerShader(ref drawInfo, index);
					drawInfo.DrawDataCache.Add(drawData);
				}
				if (texture != null && texture[index] != ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank"))
				{
					drawData = new DrawData(texture[index].Value, position, sourceRect, PlayerColor(ref drawInfo, index), rotation, origin, scale, effect, 0);
					drawData.shader = PlayerShader(ref drawInfo, index);
					drawInfo.DrawDataCache.Add(drawData);
				}
			}
		}

		private Color PlayerColor(ref PlayerDrawSet drawInfo, int index)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			var mrPlagueRacesPlayer = drawPlayer.GetModPlayer<MrPlagueRacesPlayer>();
			Color color = (index == 0 ? drawInfo.colorHead : index == 1 ? mrPlagueRacesPlayer.colorDetail : index == 2 ? drawInfo.colorEyeWhites : index == 3 ? drawInfo.colorEyes : index == 4 ? drawInfo.colorHair : index == 5 ? drawPlayer.GetImmuneAlpha(drawPlayer.skinColor, 0f) : index == 6 ? drawPlayer.GetImmuneAlpha(mrPlagueRacesPlayer.detailColor, 0f) : index == 7 ? drawPlayer.GetImmuneAlpha(Color.White, 0f) : index == 8 ? drawPlayer.GetImmuneAlpha(drawPlayer.eyeColor, 0f) : drawPlayer.GetImmuneAlpha(drawPlayer.GetHairColor(useLighting: false), 0f));
			return color;
		}

		private int PlayerShader(ref PlayerDrawSet drawInfo, int index)
		{
			int shader = (index == 0 ? drawInfo.skinDyePacked : index == 1 ? drawInfo.skinDyePacked : index == 2 ? 0 : index == 3 ? 0 : index == 4 ? drawInfo.hairDyePacked : index == 5 ? drawInfo.skinDyePacked : index == 6 ? drawInfo.skinDyePacked : index == 7 ? 0 : index == 8 ? 0 : drawInfo.hairDyePacked);
			return shader;
		}

		private static bool IsArmorDrawnWhenInvisible(int torsoID)
		{
			if ((uint)(torsoID - 21) <= 1u)
			{
				return false;
			}
			return true;
		}
		
	}
}