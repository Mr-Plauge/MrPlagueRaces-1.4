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
	public class RaceFrontArm : PlayerDrawLayer
	{
		private Asset<Texture2D>[] Shirt_Texture = new Asset<Texture2D>[13];
		private Asset<Texture2D>[] Undershirt_Texture = new Asset<Texture2D>[13];
		private Asset<Texture2D>[] ShirtAddition_Texture = new Asset<Texture2D>[13];

		private Asset<Texture2D>[] Arm_Texture = new Asset<Texture2D>[10];
		private Asset<Texture2D>[] Hand_Texture = new Asset<Texture2D>[10];
		private string[] PlayerColors = { "ColorSkin", "ColorDetail", "Colorless", "ColorEyes", "ColorHair", "ColorSkin/Glowmask", "ColorDetail/Glowmask", "Colorless/Glowmask", "ColorEyes/Glowmask", "ColorHair/Glowmask" };

		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ArmOverItem);

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
				TextureAssets.Players[drawInfo.skinVar, 6] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 7] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 8] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 9] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 13] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 14] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				
				for (int i = 0; i < 5; i++)
				{
					Shirt_Texture[male[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Shirt", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Shirt");
					Undershirt_Texture[male[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Undershirt", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Undershirt");
					ShirtAddition_Texture[male[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/ShirtAddition", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/ShirtAddition");
				}

				for (int i = 0; i < 5; i++)
				{
					Shirt_Texture[female[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Shirt", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Shirt");
					Undershirt_Texture[female[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/Undershirt", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Undershirt");
					ShirtAddition_Texture[female[i]] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"Clothes/Style_{i + 1}/ShirtAddition", $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/ShirtAddition");
				}
				
				for (int i = 0; i < 10; i++)
				{
					Arm_Texture[i] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"{PlayerColors[i]}/Arms");
					Hand_Texture[i] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"{PlayerColors[i]}/Hands");
				}

				Vector2 frontArmPosition = new Vector2((float)(int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawPlayer.bodyFrame.Width / 2) + (float)(drawPlayer.width / 2)), (float)(int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2((float)(drawPlayer.bodyFrame.Width / 2), (float)(drawPlayer.bodyFrame.Height / 2));
				Vector2 value = Main.OffsetsPlayerHeadgear[drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height];
				value.Y -= 2f;
				frontArmPosition += value * (float)(-((Enum)drawInfo.playerEffect).HasFlag((Enum)(object)(SpriteEffects)2).ToDirectionInt());
				float bodyRotation = drawPlayer.bodyRotation;
				float rotation = drawPlayer.bodyRotation + drawInfo.compositeFrontArmRotation;
				Vector2 bodyVect = drawInfo.bodyVect;
				Vector2 compositeOffset_FrontArm = new Vector2((float)(-5 * ((!((Enum)drawInfo.playerEffect).HasFlag((Enum)(object)(SpriteEffects)1)) ? 1 : (-1))), 0f);
				bodyVect += compositeOffset_FrontArm;
				frontArmPosition += compositeOffset_FrontArm;
				Vector2 frontShoulderPosition = frontArmPosition + drawInfo.frontShoulderOffset;
				if (drawInfo.compFrontArmFrame.X / drawInfo.compFrontArmFrame.Width >= 7)
				{
					frontArmPosition += new Vector2((float)((!((Enum)drawInfo.playerEffect).HasFlag((Enum)(object)(SpriteEffects)1)) ? 1 : (-1)), (float)((!((Enum)drawInfo.playerEffect).HasFlag((Enum)(object)(SpriteEffects)2)) ? 1 : (-1)));
				}
				_ = drawPlayer.invis;
				bool num7 = drawPlayer.body > 0;
				int num6 = drawInfo.compShoulderOverFrontArm ? 1 : 0;
				int num5 = (!drawInfo.compShoulderOverFrontArm) ? 1 : 0;
				int num4 = (!drawInfo.compShoulderOverFrontArm) ? 1 : 0;
				if (num7)
				{
					if (!drawPlayer.invis || IsArmorDrawnWhenInvisible(drawPlayer.body))
					{
						Texture2D value3 = TextureAssets.ArmorBodyComposite[drawPlayer.body].Value;
						for (int j = 0; j < 2; j++)
						{
							if ((!drawPlayer.invis && j == num4) & !drawInfo.hidesTopSkin)
							{
								if (!drawInfo.armorHidesArms)
								{
									MakeColoredDrawDatas(ref drawInfo, Arm_Texture, null, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0);
								}
								if (!drawInfo.armorHidesHands)
								{
									MakeColoredDrawDatas(ref drawInfo, Hand_Texture, null, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0);
								}
							}
						}
					}
				}
				else if (!drawPlayer.invis)
				{
					for (int j = 0; j < 2; j++)
					{
						if (j == num6)
						{
							if (!drawInfo.hidesTopSkin)
							{
								MakeColoredDrawDatas(ref drawInfo, Arm_Texture, null, frontShoulderPosition, drawInfo.compFrontShoulderFrame, bodyRotation, bodyVect, 1f, drawInfo.playerEffect, 0);
							}
							if (drawPlayer.armor[1].type == ItemID.FamiliarShirt || drawPlayer.armor[11].type == ItemID.FamiliarShirt)
							{
								drawInfo.DrawDataCache.Add(new DrawData(Undershirt_Texture[drawInfo.skinVar].Value, frontShoulderPosition, drawInfo.compFrontShoulderFrame, drawInfo.colorUnderShirt, bodyRotation, bodyVect, 1f, drawInfo.playerEffect, 0));
								drawInfo.DrawDataCache.Add(new DrawData(ShirtAddition_Texture[drawInfo.skinVar].Value, frontShoulderPosition, drawInfo.compFrontShoulderFrame, drawInfo.colorShirt, bodyRotation, bodyVect, 1f, drawInfo.playerEffect, 0));
								drawInfo.DrawDataCache.Add(new DrawData(Shirt_Texture[drawInfo.skinVar].Value, frontShoulderPosition, drawInfo.compFrontShoulderFrame, drawInfo.colorShirt, bodyRotation, bodyVect, 1f, drawInfo.playerEffect, 0));
							}
						}
						if (j == num5)
						{
							if (!drawInfo.hidesTopSkin)
							{
								MakeColoredDrawDatas(ref drawInfo, Arm_Texture, null, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0);
							}
							if (drawPlayer.armor[1].type == ItemID.FamiliarShirt || drawPlayer.armor[11].type == ItemID.FamiliarShirt)
							{
								drawInfo.DrawDataCache.Add(new DrawData(Undershirt_Texture[drawInfo.skinVar].Value, frontArmPosition, drawInfo.compFrontArmFrame, drawInfo.colorUnderShirt, rotation, bodyVect, 1f, drawInfo.playerEffect, 0));
								drawInfo.DrawDataCache.Add(new DrawData(ShirtAddition_Texture[drawInfo.skinVar].Value, frontArmPosition, drawInfo.compFrontArmFrame, drawInfo.colorShirt, rotation, bodyVect, 1f, drawInfo.playerEffect, 0));
								drawInfo.DrawDataCache.Add(new DrawData(Shirt_Texture[drawInfo.skinVar].Value, frontArmPosition, drawInfo.compFrontArmFrame, drawInfo.colorShirt, rotation, bodyVect, 1f, drawInfo.playerEffect, 0));
							}
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