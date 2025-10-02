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
using MrPlagueRaces.Common.Races;

namespace MrPlagueRaces.Common
{
	public class PlayerLayerHelpers
	{
		// An index of player colors in the order they are rendered by the MakeColoredDrawDatas function (IE, colorhair will be rendered above colorskin)
		public static string[] PlayerColors = { "ColorSkin", "ColorDetail", "ColorDetail_1", "ColorDetail_2", "ColorDetail_3", "Colorless", "ColorEyes", "ColorHair", "ColorSkin/Glowmask", "ColorDetail/Glowmask", "ColorDetail_1/Glowmask", "ColorDetail_2/Glowmask", "ColorDetail_3/Glowmask", "Colorless/Glowmask", "ColorEyes/Glowmask", "ColorHair/Glowmask" };

        // Arrays representing the familiar clothing styles' numerical IDs
        public static int[] MaleClothingIDs = { 0, 2, 1, 3, 8 };
        public static int[] FemaleClothingIDs = { 4, 6, 5, 7, 9 };

        // A function for drawing a playerlayer using all 16 player colors. If no corresponding file is found in one of the color folders, that color is skipped
        public static void MakeColoredDrawDatas(ref PlayerDrawSet drawInfo, RaceSheet[] raceSheet, RaceSheet[,] raceStyleSheet, Vector2 position, Rectangle? sourceRect, float rotation, Vector2 origin, float scale, SpriteEffects effect, int inactiveLayerDepth, int auxilaryHairstyle = 0)
        {
            DrawData drawData;
            Player drawPlayer = drawInfo.drawPlayer;
            var mrPlagueRacesPlayer = drawPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            int chosenHairIndex;
            int playerGender = drawPlayer.Male ? 0 : 1;

            switch (auxilaryHairstyle)
            {
                case 0:
                    chosenHairIndex = drawPlayer.hair;
                    break;
                case 1:
                    chosenHairIndex = mrPlagueRacesPlayer.auxilaryHairstyle1;
                    break;
                case 2:
                    chosenHairIndex = mrPlagueRacesPlayer.auxilaryHairstyle2;
                    break;
                default:
                    chosenHairIndex = mrPlagueRacesPlayer.auxilaryHairstyle3;
                    break;
            }

            int index;
            for (index = 0; index < 16; index++)
            {
                if (raceStyleSheet != null && (raceStyleSheet[index, chosenHairIndex].OverrideTexture[playerGender] != null ? !raceStyleSheet[index, chosenHairIndex].IsOverrideTextureBlank(playerGender) : !raceStyleSheet[index, chosenHairIndex].IsBlank(playerGender)))
                {
                    drawData = new DrawData(raceStyleSheet[index, chosenHairIndex].OverrideTexture[playerGender] != null ? raceStyleSheet[index, chosenHairIndex].OverrideTexture[playerGender] : raceStyleSheet[index, chosenHairIndex].Texture[playerGender].Value, position, sourceRect, PlayerColor(ref drawInfo, index), rotation, origin, scale, effect, 0);
                    drawData.shader = PlayerShader(ref drawInfo, index);
                    drawInfo.DrawDataCache.Add(drawData);
                }
                else if (raceSheet != null && (raceSheet[index].OverrideTexture[playerGender] != null ? !raceSheet[index].IsOverrideTextureBlank(playerGender) : !raceSheet[index].IsBlank(playerGender)))
                {
                    drawData = new DrawData(raceSheet[index].OverrideTexture[playerGender] != null ? raceSheet[index].OverrideTexture[playerGender] : raceSheet[index].Texture[playerGender].Value, position, sourceRect, PlayerColor(ref drawInfo, index), rotation, origin, scale, effect, 0);
                    drawData.shader = PlayerShader(ref drawInfo, index);
                    drawInfo.DrawDataCache.Add(drawData);
                }
            }
        }

		// Returns a player color value (0-15) that receives ingame lighting.
        public static Color PlayerColor(ref PlayerDrawSet drawInfo, int index)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            var mrPlagueRacesPlayer = drawPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            Color color = (index == 0 ? drawInfo.colorHead : index == 1 ? Main.quickAlpha(mrPlagueRacesPlayer.colorDetail, (float)(drawInfo.colorHead.A / 255f)) : index == 2 ? Main.quickAlpha(mrPlagueRacesPlayer.colorAuxilaryDetail1, (float)(drawInfo.colorHead.A / 255f)) : index == 3 ? Main.quickAlpha(mrPlagueRacesPlayer.colorAuxilaryDetail2, (float)(drawInfo.colorHead.A / 255f)) : index == 4 ? Main.quickAlpha(mrPlagueRacesPlayer.colorAuxilaryDetail3, (float)(drawInfo.colorHead.A / 255f)) : index == 5 ? drawInfo.colorEyeWhites : index == 6 ? drawInfo.colorEyes : index == 7 ? drawInfo.colorHair : index == 8 ? drawPlayer.GetImmuneAlpha(drawPlayer.skinColor, 0f) : index == 9 ? drawPlayer.GetImmuneAlpha(mrPlagueRacesPlayer.detailColor, 0f) : index == 10 ? drawPlayer.GetImmuneAlpha(mrPlagueRacesPlayer.auxilaryDetailColor1, 0f) : index == 11 ? drawPlayer.GetImmuneAlpha(mrPlagueRacesPlayer.auxilaryDetailColor2, 0f) : index == 12 ? drawPlayer.GetImmuneAlpha(mrPlagueRacesPlayer.auxilaryDetailColor3, 0f) : index == 13 ? drawPlayer.GetImmuneAlpha(Color.White, 0f) : index == 14 ? drawPlayer.GetImmuneAlpha(drawPlayer.eyeColor, 0f) : drawPlayer.GetImmuneAlpha(drawPlayer.GetHairColor(useLighting: false), 0f));
            return color;
        }

        // Returns a player color value (0-15) that receives ingame lighting.
        public static Color PlayerColor(Player player, int index)
        {
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            Color color = ((index == 0 || index == 8) ? player.skinColor : (index == 1 || index == 9) ? mrPlagueRacesPlayer.detailColor : (index == 2 || index == 10) ? mrPlagueRacesPlayer.auxilaryDetailColor1 : (index == 3 || index == 11) ? mrPlagueRacesPlayer.auxilaryDetailColor2 : (index == 4 || index == 12) ? mrPlagueRacesPlayer.auxilaryDetailColor3 : (index == 5 || index == 13) ? Color.White : (index == 6 || index == 14) ? player.eyeColor : (index == 7 || index == 15) ? player.hairColor : Color.White);
            return color;
        }

        // Returns a player shader corresponding to one of the player colors (0-15). This is used to apply hair dye to hair layers, skin dyes to skin layers, etc.
        public static int PlayerShader(ref PlayerDrawSet drawInfo, int index)
        {
            int shader = (index == 0 ? drawInfo.skinDyePacked : index == 1 ? drawInfo.skinDyePacked : index == 2 ? drawInfo.skinDyePacked : index == 3 ? drawInfo.skinDyePacked : index == 4 ? drawInfo.skinDyePacked : index == 5 ? 0 : index == 6 ? 0 : index == 7 ? drawInfo.hairDyePacked : index == 8 ? drawInfo.skinDyePacked : index == 9 ? drawInfo.skinDyePacked : index == 10 ? drawInfo.skinDyePacked : index == 11 ? drawInfo.skinDyePacked : index == 12 ? drawInfo.skinDyePacked : index == 13 ? 0 : index == 14 ? 0 : drawInfo.hairDyePacked);
            return shader;
        }

		// A function for drawing a pair of player legs that have been bent into a sitting position. This function draws the legs using a single specified player color. To render sitting legs in all 16 colors, use a loop (demonstrated in RaceTorso).
        public static void DrawSittingLegs(ref PlayerDrawSet drawInfo, Texture2D textureToDraw, Color matchingColor, int shaderIndex = 0, bool glowmask = false)
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

		// A boolean used in RaceFrontArm to determine if armor is drawn when the player is invisible
        public static bool IsArmorDrawnWhenInvisible(int torsoID)
        {
            if ((uint)(torsoID - 21) <= 1u)
            {
                return false;
            }
            return true;
        }
    }
}