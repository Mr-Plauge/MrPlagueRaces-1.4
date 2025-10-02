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
	public class RaceFrontArm : PlayerDrawLayer // RaceFrontArm is responsible for drawing the player's front arm
	{
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

			if (mrPlagueRacesPlayer.race != null)
            {
                TextureAssets.Players[drawInfo.skinVar, 6] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 7] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 8] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 9] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 13] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 14] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");

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
					if (!drawPlayer.invis || PlayerLayerHelpers.IsArmorDrawnWhenInvisible(drawPlayer.body))
					{
						Texture2D value3 = TextureAssets.ArmorBodyComposite[drawPlayer.body].Value;
						for (int j = 0; j < 2; j++)
						{
							if ((!drawPlayer.invis && j == num4) & !drawInfo.hidesTopSkin)
							{
								if (!drawInfo.armorHidesArms)
								{
									PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.Arm_Sheet, null, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0);
                                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0);
                                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary1_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0, 1);
                                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary2_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0, 2);
                                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary3_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0, 3);
                                }
								if (!drawInfo.armorHidesHands)
								{
									PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.Hand_Sheet, null, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0);
                                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hand_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0);
                                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hand_Auxilary1_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0, 1);
                                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hand_Auxilary2_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0, 2);
                                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hand_Auxilary3_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0, 3);
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
								PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.Arm_Sheet, null, frontShoulderPosition, drawInfo.compFrontShoulderFrame, bodyRotation, bodyVect, 1f, drawInfo.playerEffect, 0);
                                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_StyleSheet, frontShoulderPosition, drawInfo.compFrontShoulderFrame, bodyRotation, bodyVect, 1f, drawInfo.playerEffect, 0);
                                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary1_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0, 1);
                                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary2_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0, 2);
                                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary3_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0, 3);
                            }
							if (drawPlayer.armor[1].type == ItemID.FamiliarShirt || drawPlayer.armor[11].type == ItemID.FamiliarShirt)
							{
								drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.Undershirt_Sheet[drawInfo.skinVar].Texture[0].Value, frontShoulderPosition, drawInfo.compFrontShoulderFrame, drawInfo.colorUnderShirt, bodyRotation, bodyVect, 1f, drawInfo.playerEffect, 0));
								drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.ShirtAddition_Sheet[drawInfo.skinVar].Texture[0].Value, frontShoulderPosition, drawInfo.compFrontShoulderFrame, drawInfo.colorShirt, bodyRotation, bodyVect, 1f, drawInfo.playerEffect, 0));
								drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.Shirt_Sheet[drawInfo.skinVar].Texture[0].Value, frontShoulderPosition, drawInfo.compFrontShoulderFrame, drawInfo.colorShirt, bodyRotation, bodyVect, 1f, drawInfo.playerEffect, 0));
							}
						}
						if (j == num5)
						{
							if (!drawInfo.hidesTopSkin)
							{
								PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.Arm_Sheet, null, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0);
                                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0);
                                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary1_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0, 1);
                                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary2_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0, 2);
                                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary3_StyleSheet, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect, 0, 3);
                            }
							if (drawPlayer.armor[1].type == ItemID.FamiliarShirt || drawPlayer.armor[11].type == ItemID.FamiliarShirt)
							{
								drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.Undershirt_Sheet[drawInfo.skinVar].Texture[0].Value, frontArmPosition, drawInfo.compFrontArmFrame, drawInfo.colorUnderShirt, rotation, bodyVect, 1f, drawInfo.playerEffect, 0));
								drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.ShirtAddition_Sheet[drawInfo.skinVar].Texture[0].Value, frontArmPosition, drawInfo.compFrontArmFrame, drawInfo.colorShirt, rotation, bodyVect, 1f, drawInfo.playerEffect, 0));
								drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.Shirt_Sheet[drawInfo.skinVar].Texture[0].Value, frontArmPosition, drawInfo.compFrontArmFrame, drawInfo.colorShirt, rotation, bodyVect, 1f, drawInfo.playerEffect, 0));
							}
						}
					}
				}
			}
		}
	}
}