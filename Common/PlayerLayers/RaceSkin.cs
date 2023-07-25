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
	public class RaceTorso : PlayerDrawLayer
	{
		private Asset<Texture2D>[] Shirt_Texture = new Asset<Texture2D>[13];
		private Asset<Texture2D>[] Undershirt_Texture = new Asset<Texture2D>[13];
		private Asset<Texture2D>[] Pants_Texture = new Asset<Texture2D>[13];
		private Asset<Texture2D>[] Shoes_Texture = new Asset<Texture2D>[13];
		private Asset<Texture2D>[] ShirtAddition_Texture = new Asset<Texture2D>[13];
		private Asset<Texture2D>[] PantsAddition_Texture = new Asset<Texture2D>[13];
		private Asset<Texture2D> CensorShirt_Texture;
		private Asset<Texture2D> CensorPants_Texture;

		private Asset<Texture2D>[] Body_Texture = new Asset<Texture2D>[10];
		private Asset<Texture2D>[] Legs_Texture = new Asset<Texture2D>[10];
		private string[] PlayerColors = { "ColorSkin", "ColorDetail", "Colorless", "ColorEyes", "ColorHair", "ColorSkin/Glowmask", "ColorDetail/Glowmask", "Colorless/Glowmask", "ColorEyes/Glowmask", "ColorHair/Glowmask" };

		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Skin);

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) 
		{
			return (drawInfo.skinVar < 10);
		}

		protected override void Draw(ref PlayerDrawSet drawInfo) 
		{
			Player drawPlayer = drawInfo.drawPlayer;

			int[] male = { 0, 2, 1, 3, 8 };
			int[] female = { 4, 6, 5, 7, 9 };

			var mrPlagueRacesPlayer = drawPlayer.GetModPlayer<MrPlagueRacesPlayer>();
			
			if (mrPlagueRacesPlayer.race != null) {
				TextureAssets.Players[drawInfo.skinVar, 3] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 4] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 6] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 10] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 11] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 12] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");

				CensorShirt_Texture = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, "Clothes/CensorShirt", "MrPlagueRaces/Assets/Textures/Players/Clothes/CensorShirt");
				CensorPants_Texture = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, "Clothes/CensorPants", "MrPlagueRaces/Assets/Textures/Players/Clothes/CensorPants");

				for (int i = 0; i < 5; i++)
				{
					Shirt_Texture[male[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Shirt", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Shirt");
					Undershirt_Texture[male[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Undershirt", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Undershirt");
					Pants_Texture[male[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Pants", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Pants");
					Shoes_Texture[male[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Shoes", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Shoes");
					ShirtAddition_Texture[male[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/ShirtAddition", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/ShirtAddition");
					PantsAddition_Texture[male[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/PantsAddition", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/PantsAddition");
				}

				for (int i = 0; i < 5; i++)
				{
					Shirt_Texture[female[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Shirt", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/Shirt");
					Undershirt_Texture[female[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Undershirt", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/Undershirt");
					Pants_Texture[female[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Pants", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/Pants");
					Shoes_Texture[female[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Shoes", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/Shoes");
					ShirtAddition_Texture[female[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/ShirtAddition", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/ShirtAddition");
					PantsAddition_Texture[female[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/PantsAddition", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/PantsAddition");
				}
				
				for (int i = 0; i < 10; i++)
				{
					Body_Texture[i] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"{PlayerColors[i]}/Body");
					Legs_Texture[i] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"{PlayerColors[i]}/Legs");
				}

				DrawData drawData;
				Vector2 bodyPosition = new Vector2((float)(int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawPlayer.bodyFrame.Width / 2) + (float)(drawPlayer.width / 2)), (float)(int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2((float)(drawPlayer.bodyFrame.Width / 2), (float)(drawPlayer.bodyFrame.Height / 2));
				Vector2 legPosition = new Vector2((float)(int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawPlayer.bodyFrame.Width / 2) + (float)(drawPlayer.width / 2)), (float)(int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2((float)(drawPlayer.bodyFrame.Width / 2), (float)(drawPlayer.bodyFrame.Height / 2));
				Vector2 pantsAdditionPosition = new Vector2((float)(int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawPlayer.legFrame.Width / 2) + (float)(drawPlayer.width / 2)), (float)(int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawPlayer.height - (float)drawPlayer.legFrame.Height + 4f)) + drawPlayer.legPosition + drawInfo.legVect;
				Vector2 compositeOffset_BackArm = new Vector2((float)(6 * ((!((Enum)drawInfo.playerEffect).HasFlag((Enum)(object)(SpriteEffects)1)) ? 1 : (-1))), (float)(2 * ((!((Enum)drawInfo.playerEffect).HasFlag((Enum)(object)(SpriteEffects)2)) ? 1 : (-1))));
				Vector2 compositeOffset_FrontArm = new Vector2((float)(-5 * ((!((Enum)drawInfo.playerEffect).HasFlag((Enum)(object)(SpriteEffects)1)) ? 1 : (-1))), 0f);

				if (!drawInfo.hidesTopSkin && !drawPlayer.invis)
				{
					bodyPosition.Y += drawInfo.torsoOffset;
					Vector2 value = Main.OffsetsPlayerHeadgear[drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height];
					value.Y -= 2f;
					bodyPosition += value * (float)(-((Enum)drawInfo.playerEffect).HasFlag((Enum)(object)(SpriteEffects)2).ToDirectionInt());
					float bodyRotation = drawPlayer.bodyRotation;
					MakeColoredDrawDatas(ref drawInfo, Body_Texture, null, bodyPosition, drawInfo.compTorsoFrame, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
				}
				if (!drawInfo.hidesBottomSkin && !drawPlayer.invis && !drawInfo.isBottomOverriden)
				{
					if (drawInfo.isSitting)
					{
						for (int i = 0; i < 5; i++)
						{
							DrawSittingLegs(ref drawInfo, Legs_Texture[i].Value, PlayerColor(ref drawInfo, i), PlayerShader(ref drawInfo, i));
						}
						if (drawPlayer.armor[2].type == ItemID.FamiliarPants || drawPlayer.armor[12].type == ItemID.FamiliarPants)
						{
							DrawSittingLegs(ref drawInfo, Pants_Texture[drawInfo.skinVar].Value, drawInfo.colorPants);
							DrawSittingLegs(ref drawInfo, Shoes_Texture[drawInfo.skinVar].Value, drawInfo.colorShoes);
						}
					}
					else
					{
						MakeColoredDrawDatas(ref drawInfo, Legs_Texture, null, legPosition, drawPlayer.legFrame, drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
						if (drawPlayer.armor[2].type == ItemID.FamiliarPants || drawPlayer.armor[12].type == ItemID.FamiliarPants)
						{
							drawData = new DrawData(Pants_Texture[drawInfo.skinVar].Value, legPosition, drawPlayer.legFrame, drawInfo.colorPants, drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
							drawInfo.DrawDataCache.Add(drawData);
							drawData = new DrawData(Shoes_Texture[drawInfo.skinVar].Value, legPosition, drawPlayer.legFrame, drawInfo.colorShoes, drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
							drawInfo.DrawDataCache.Add(drawData);
						}
					}
				}
				if (!drawPlayer.invis)
				{
					float bodyRotation = drawPlayer.bodyRotation;
					float rotation = bodyRotation + drawInfo.compositeBackArmRotation;
					Vector2 backArmPosition = bodyPosition;
					Vector2 bodyVect2 = drawInfo.bodyVect;
					bodyVect2 += compositeOffset_BackArm;
					backArmPosition += compositeOffset_BackArm;
					if (!(drawPlayer.armor[1].type == ItemID.FamiliarShirt || drawPlayer.armor[11].type == ItemID.FamiliarShirt) && mrPlagueRacesPlayer.race.CensorClothing)
					{
						drawInfo.DrawDataCache.Add(new DrawData(CensorShirt_Texture.Value, backArmPosition, drawInfo.compTorsoFrame, drawInfo.colorPants, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0));
					}
					if (!(drawPlayer.armor[2].type == ItemID.FamiliarPants || drawPlayer.armor[12].type == ItemID.FamiliarPants) && mrPlagueRacesPlayer.race.CensorClothing)
					{
						if (drawInfo.isSitting)
						{
							DrawSittingLegs(ref drawInfo, CensorPants_Texture.Value, drawInfo.colorShirt);
							return;
						}
						else
						{
							drawData = new DrawData(CensorPants_Texture.Value, legPosition, drawPlayer.legFrame, drawInfo.colorPants, drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
							drawInfo.DrawDataCache.Add(drawData);
						}
					}
					if (drawPlayer.armor[1].type == ItemID.FamiliarShirt || drawPlayer.armor[11].type == ItemID.FamiliarShirt)
					{
						drawInfo.DrawDataCache.Add(new DrawData(Undershirt_Texture[drawInfo.skinVar].Value, bodyPosition, drawInfo.compTorsoFrame, drawInfo.colorUnderShirt, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0));
						drawInfo.DrawDataCache.Add(new DrawData(Shirt_Texture[drawInfo.skinVar].Value, bodyPosition, drawInfo.compTorsoFrame, drawInfo.colorShirt, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0));
						if ((drawInfo.skinVar == 3 || drawInfo.skinVar == 8 || drawInfo.skinVar == 7) && drawPlayer.body <= 0 && !drawPlayer.invis)
						{
							if (drawInfo.isSitting)
							{
								DrawSittingLegs(ref drawInfo, PantsAddition_Texture[drawInfo.skinVar].Value, drawInfo.colorShirt);
								return;
							}
							drawData = new DrawData(PantsAddition_Texture[drawInfo.skinVar].Value, pantsAdditionPosition, drawPlayer.legFrame, drawInfo.colorShirt, drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect, 0);
							drawInfo.DrawDataCache.Add(drawData);
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
			Color color = (index == 0 ? drawInfo.colorHead : index == 1 ? new Color(mrPlagueRacesPlayer.colorDetail.R, mrPlagueRacesPlayer.colorDetail.G, mrPlagueRacesPlayer.colorDetail.B, drawPlayer.skinColor.A) : index == 2 ? drawInfo.colorEyeWhites : index == 3 ? drawInfo.colorEyes : index == 4 ? drawInfo.colorHair : index == 5 ? drawPlayer.GetImmuneAlpha(drawPlayer.skinColor, 0f) : index == 6 ? drawPlayer.GetImmuneAlpha(mrPlagueRacesPlayer.detailColor, 0f) : index == 7 ? drawPlayer.GetImmuneAlpha(Color.White, 0f) : index == 8 ? drawPlayer.GetImmuneAlpha(drawPlayer.eyeColor, 0f) : drawPlayer.GetImmuneAlpha(drawPlayer.GetHairColor(useLighting: false), 0f));
			return color;
		}

		private int PlayerShader(ref PlayerDrawSet drawInfo, int index)
		{
			int shader = (index == 0 ? drawInfo.skinDyePacked : index == 1 ? drawInfo.skinDyePacked : index == 2 ? 0 : index == 3 ? 0 : index == 4 ? drawInfo.hairDyePacked : index == 5 ? drawInfo.skinDyePacked : index == 6 ? drawInfo.skinDyePacked : index == 7 ? 0 : index == 8 ? 0 : drawInfo.hairDyePacked);
			return shader;
		}

		private static void DrawSittingLegs(ref PlayerDrawSet drawInfo, Texture2D textureToDraw, Color matchingColor, int shaderIndex = 0, bool glowmask = false)
		{
			DrawData drawData;
			Player drawPlayer = drawInfo.drawPlayer;
			Vector2 legsOffset = drawInfo.legsOffset;
			Vector2 value = new Vector2((float)(int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawPlayer.legFrame.Width / 2) + (float)(drawPlayer.width / 2)), (float)(int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawPlayer.height - (float)drawPlayer.legFrame.Height + 4f)) + drawPlayer.legPosition + drawInfo.legVect;
			Rectangle legFrame = drawPlayer.legFrame;
			value.Y -= 2f;
			value.Y += drawInfo.seatYOffset;
			value += legsOffset;
			int num = 2;
			int num2 = 42;
			int num4 = 2;
			int num8 = 2;
			int num7 = 0;
			int num6 = 0;
			int num5 = 0;
			bool flag = drawPlayer.legs == 101 || drawPlayer.legs == 102 || drawPlayer.legs == 118 || drawPlayer.legs == 99;
			if (drawPlayer.wearsRobe && !flag)
			{
				num = 0;
				num8 = 0;
				num2 = 6;
				value.Y += 4f;
				legFrame.Y = legFrame.Height * 5;
			}
			switch (drawPlayer.legs)
			{
			case 214:
			case 215:
			case 216:
				num = -6;
				num8 = 2;
				num7 = 2;
				num4 = 4;
				num2 = 6;
				legFrame = drawPlayer.legFrame;
				value.Y += 2f;
				break;
			case 106:
			case 143:
			case 226:
				num = 0;
				num8 = 0;
				num2 = 6;
				value.Y += 4f;
				legFrame.Y = legFrame.Height * 5;
				break;
			case 132:
				num = -2;
				num5 = 2;
				break;
			case 193:
			case 194:
				if (drawPlayer.body == 218)
				{
					num = -2;
					num5 = 2;
					value.Y += 2f;
				}
				break;
			case 210:
				if (glowmask)
				{
					Vector2 vector = default(Vector2);
					value += vector;
				}
				break;
			}
			for (int num3 = num4; num3 >= 0; num3--)
			{
				Vector2 position = value + new Vector2((float)num, 2f) * new Vector2((float)drawPlayer.direction, 1f);
				Rectangle value2 = legFrame;
				value2.Y += num3 * 2;
				value2.Y += num2;
				value2.Height -= num2;
				value2.Height -= num3 * 2;
				if (num3 != num4)
				{
					value2.Height = 2;
				}
				position.X += drawPlayer.direction * num8 * num3 + num6 * drawPlayer.direction;
				if (num3 != 0)
				{
					position.X += num5 * drawPlayer.direction;
				}
				position.Y += num2;
				position.Y += num7;
				drawData = new DrawData(textureToDraw, position, value2, matchingColor, drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect, 0);
				drawData.shader = shaderIndex;
				drawInfo.DrawDataCache.Add(drawData);
			}
		}
	}
}