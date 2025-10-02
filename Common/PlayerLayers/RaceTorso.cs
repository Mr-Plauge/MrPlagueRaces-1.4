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
	public class RaceTorso : PlayerDrawLayer // RaceTorso is responsible for drawing the player's body and legs. The sitting animation is handled here
	{
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
			
			if (mrPlagueRacesPlayer.race != null)
            {
                TextureAssets.Players[drawInfo.skinVar, 3] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 4] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 6] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 10] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 11] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 12] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");

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

					PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.Body_Sheet, null, bodyPosition, drawInfo.compTorsoFrame, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Body_StyleSheet, bodyPosition, drawInfo.compTorsoFrame, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Body_Auxilary1_StyleSheet, bodyPosition, drawInfo.compTorsoFrame, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0, 1);
                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Body_Auxilary2_StyleSheet, bodyPosition, drawInfo.compTorsoFrame, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0, 2);
                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Body_Auxilary3_StyleSheet, bodyPosition, drawInfo.compTorsoFrame, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0, 3);
                }
				if (!drawInfo.hidesBottomSkin && !drawPlayer.invis && !drawInfo.isBottomOverriden)
				{
					if (drawInfo.isSitting)
                    {
                        int playerGender = drawPlayer.Male ? 0 : 1;
                        for (int i = 0; i < 16; i++)
						{
							PlayerLayerHelpers.DrawSittingLegs(ref drawInfo, mrPlagueRacesPlayer.race.Legs_Sheet[i].Texture[playerGender].Value, PlayerLayerHelpers.PlayerColor(ref drawInfo, i), PlayerLayerHelpers.PlayerShader(ref drawInfo, i));
                            PlayerLayerHelpers.DrawSittingLegs(ref drawInfo, mrPlagueRacesPlayer.race.Legs_StyleSheet[i, drawPlayer.hair].Texture[playerGender].Value, PlayerLayerHelpers.PlayerColor(ref drawInfo, i), PlayerLayerHelpers.PlayerShader(ref drawInfo, i));
                            PlayerLayerHelpers.DrawSittingLegs(ref drawInfo, mrPlagueRacesPlayer.race.Legs_Auxilary1_StyleSheet[i, mrPlagueRacesPlayer.auxilaryHairstyle1].Texture[playerGender].Value, PlayerLayerHelpers.PlayerColor(ref drawInfo, i), PlayerLayerHelpers.PlayerShader(ref drawInfo, i));
                            PlayerLayerHelpers.DrawSittingLegs(ref drawInfo, mrPlagueRacesPlayer.race.Legs_Auxilary2_StyleSheet[i, mrPlagueRacesPlayer.auxilaryHairstyle2].Texture[playerGender].Value, PlayerLayerHelpers.PlayerColor(ref drawInfo, i), PlayerLayerHelpers.PlayerShader(ref drawInfo, i));
                            PlayerLayerHelpers.DrawSittingLegs(ref drawInfo, mrPlagueRacesPlayer.race.Legs_Auxilary3_StyleSheet[i, mrPlagueRacesPlayer.auxilaryHairstyle3].Texture[playerGender].Value, PlayerLayerHelpers.PlayerColor(ref drawInfo, i), PlayerLayerHelpers.PlayerShader(ref drawInfo, i));
                        }
						if (drawPlayer.armor[2].type == ItemID.FamiliarPants || drawPlayer.armor[12].type == ItemID.FamiliarPants)
						{
							PlayerLayerHelpers.DrawSittingLegs(ref drawInfo, mrPlagueRacesPlayer.race.Pants_Sheet[drawInfo.skinVar].Texture[0].Value, drawInfo.colorPants);
							PlayerLayerHelpers.DrawSittingLegs(ref drawInfo, mrPlagueRacesPlayer.race.Shoes_Sheet[drawInfo.skinVar].Texture[0].Value, drawInfo.colorShoes);
						}
					}
					else
					{
						PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.Legs_Sheet, null, legPosition, drawPlayer.legFrame, drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
                        PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Legs_StyleSheet, legPosition, drawPlayer.legFrame, drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
                        PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Legs_Auxilary1_StyleSheet, legPosition, drawPlayer.legFrame, drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0, 1);
                        PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Legs_Auxilary2_StyleSheet, legPosition, drawPlayer.legFrame, drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0, 2);
                        PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Legs_Auxilary3_StyleSheet, legPosition, drawPlayer.legFrame, drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0, 3);

                        if (drawPlayer.armor[2].type == ItemID.FamiliarPants || drawPlayer.armor[12].type == ItemID.FamiliarPants)
						{
							drawData = new DrawData(mrPlagueRacesPlayer.race.Pants_Sheet[drawInfo.skinVar].Texture[0].Value, legPosition, drawPlayer.legFrame, drawInfo.colorPants, drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
							drawInfo.DrawDataCache.Add(drawData);
							drawData = new DrawData(mrPlagueRacesPlayer.race.Shoes_Sheet[drawInfo.skinVar].Texture[0].Value, legPosition, drawPlayer.legFrame, drawInfo.colorShoes, drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
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
						drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.CensorShirt_Sheet.Texture[0].Value, backArmPosition, drawInfo.compTorsoFrame, drawInfo.colorPants, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0));
					}
					if (!(drawPlayer.armor[2].type == ItemID.FamiliarPants || drawPlayer.armor[12].type == ItemID.FamiliarPants) && mrPlagueRacesPlayer.race.CensorClothing)
					{
						if (drawInfo.isSitting)
						{
							PlayerLayerHelpers.DrawSittingLegs(ref drawInfo, mrPlagueRacesPlayer.race.CensorPants_Sheet.Texture[0].Value, drawInfo.colorShirt);
							return;
						}
						else
						{
							drawData = new DrawData(mrPlagueRacesPlayer.race.CensorPants_Sheet.Texture[0].Value, legPosition, drawPlayer.legFrame, drawInfo.colorPants, drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0);
							drawInfo.DrawDataCache.Add(drawData);
						}
					}
					if (drawPlayer.armor[1].type == ItemID.FamiliarShirt || drawPlayer.armor[11].type == ItemID.FamiliarShirt)
					{
						drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.Undershirt_Sheet[drawInfo.skinVar].Texture[0].Value, bodyPosition, drawInfo.compTorsoFrame, drawInfo.colorUnderShirt, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0));
						drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.Shirt_Sheet[drawInfo.skinVar].Texture[0].Value, bodyPosition, drawInfo.compTorsoFrame, drawInfo.colorShirt, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0));
						if ((drawInfo.skinVar == 3 || drawInfo.skinVar == 8 || drawInfo.skinVar == 7) && drawPlayer.body <= 0 && !drawPlayer.invis)
						{
							if (drawInfo.isSitting)
							{
								PlayerLayerHelpers.DrawSittingLegs(ref drawInfo, mrPlagueRacesPlayer.race.PantsAddition_Sheet[drawInfo.skinVar].Texture[0].Value, drawInfo.colorShirt);
								return;
							}
							drawData = new DrawData(mrPlagueRacesPlayer.race.PantsAddition_Sheet[drawInfo.skinVar].Texture[0].Value, pantsAdditionPosition, drawPlayer.legFrame, drawInfo.colorShirt, drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect, 0);
							drawInfo.DrawDataCache.Add(drawData);
						}
					}
				}
			}
        }
	}
}