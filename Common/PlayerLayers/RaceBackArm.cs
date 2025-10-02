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
	public class RaceBackArm : PlayerDrawLayer // RaceBackArm is responsible for drawing the player's back arm
	{
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

			if (mrPlagueRacesPlayer.race != null) 
			{
                TextureAssets.Players[drawInfo.skinVar, 5] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 7] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				
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
							PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.Arm_Sheet, null, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary1_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0, 1);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary2_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0, 2);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary3_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0, 3);
                        }
						if (!flag && !drawInfo.hidesTopSkin)
						{
							PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.Hand_Sheet, null, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hand_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hand_Auxilary1_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0, 1);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hand_Auxilary2_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0, 2);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hand_Auxilary3_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0, 3);
                            flag = true;
						}
						if (drawPlayer.armor[1].type == ItemID.FamiliarShirt || drawPlayer.armor[11].type == ItemID.FamiliarShirt)
						{
							drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.Undershirt_Sheet[drawInfo.skinVar].Texture[0].Value, backArmPosition, drawInfo.compBackArmFrame, drawInfo.colorUnderShirt, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0));
							drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.ShirtAddition_Sheet[drawInfo.skinVar].Texture[0].Value, backArmPosition, drawInfo.compBackArmFrame, drawInfo.colorShirt, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0));
							drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.Undershirt_Sheet[drawInfo.skinVar].Texture[0].Value, bodyPosition, drawInfo.compBackShoulderFrame, drawInfo.colorUnderShirt, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0));
							drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.Shirt_Sheet[drawInfo.skinVar].Texture[0].Value, bodyPosition, drawInfo.compBackShoulderFrame, drawInfo.colorShirt, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0));
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
							PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.Arm_Sheet, null, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary1_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0, 1);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary2_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0, 2);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Arm_Auxilary3_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0, 3);
                        }
						if (!flag && !drawInfo.hidesTopSkin)
						{
							PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.Hand_Sheet, null, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hand_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hand_Auxilary1_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0, 1);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hand_Auxilary2_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0, 2);
                            PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hand_Auxilary3_StyleSheet, backArmPosition, drawInfo.compBackArmFrame, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0, 3);
                            flag = true;
						}
						if (drawPlayer.armor[1].type == ItemID.FamiliarShirt || drawPlayer.armor[11].type == ItemID.FamiliarShirt)
						{
							drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.Undershirt_Sheet[drawInfo.skinVar].Texture[0].Value, backArmPosition, drawInfo.compBackArmFrame, drawInfo.colorUnderShirt, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0));
							drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.ShirtAddition_Sheet[drawInfo.skinVar].Texture[0].Value, backArmPosition, drawInfo.compBackArmFrame, drawInfo.colorShirt, rotation, bodyVect2, 1f, drawInfo.playerEffect, 0));
							drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.Undershirt_Sheet[drawInfo.skinVar].Texture[0].Value, bodyPosition, drawInfo.compBackShoulderFrame, drawInfo.colorUnderShirt, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0));
							drawInfo.DrawDataCache.Add(new DrawData(mrPlagueRacesPlayer.race.Shirt_Sheet[drawInfo.skinVar].Texture[0].Value, bodyPosition, drawInfo.compBackShoulderFrame, drawInfo.colorShirt, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0));
						}
					}
				}
			}
		}
	}
}