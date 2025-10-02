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
	public class RaceHead : PlayerDrawLayer // RaceHead is responsible for drawing the player's head and hair. The eyelid sheet is handled here
	{
		public Asset<Texture2D> EyeLids_Texture = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/EyeLids");

        public override bool IsHeadLayer => true;
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) 
		{
			return (drawInfo.skinVar < 10);
		}

		protected override void Draw(ref PlayerDrawSet drawInfo) 
		{
			Player drawPlayer = drawInfo.drawPlayer;
			Vector2 helmetOffset = drawInfo.helmetOffset;

			var mrPlagueRacesPlayer = drawPlayer.GetModPlayer<MrPlagueRacesPlayer>();
			
			if (mrPlagueRacesPlayer.race != null) 
            {
				TextureAssets.PlayerHair[drawPlayer.hair] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.PlayerHairAlt[drawPlayer.hair] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 0] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 1] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 2] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
				TextureAssets.Players[drawInfo.skinVar, 15] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");

                Vector2 headPosition = new Vector2((float)(int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawPlayer.bodyFrame.Width / 2) + (float)(drawPlayer.width / 2)), (float)(int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.headPosition + drawInfo.headVect;
				if ((!drawInfo.hideHair || mrPlagueRacesPlayer.race.AlwaysDrawHair) && drawInfo.backHairDraw)
				{
					if (drawPlayer.head == -1 || drawInfo.fullHair || drawInfo.drawsBackHairWithoutHeadgear || mrPlagueRacesPlayer.race.AlwaysDrawHair)
					{
						PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hair_StyleSheet, headPosition, drawInfo.hairBackFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
                        PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hair_Auxilary1_StyleSheet, headPosition, drawInfo.hairBackFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 1);
                        PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hair_Auxilary2_StyleSheet, headPosition, drawInfo.hairBackFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 2);
                        PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hair_Auxilary3_StyleSheet, headPosition, drawInfo.hairBackFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 3);
                    }
					else if (drawInfo.hatHair || mrPlagueRacesPlayer.race.AlwaysDrawHair)
					{
						PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hair_StyleSheet, headPosition, drawInfo.hairBackFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
                        PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hair_Auxilary1_StyleSheet, headPosition, drawInfo.hairBackFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 1);
                        PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hair_Auxilary2_StyleSheet, headPosition, drawInfo.hairBackFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 2);
                        PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hair_Auxilary3_StyleSheet, headPosition, drawInfo.hairBackFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 3);
                    }
				}

				Vector2 eyelidOffset = Main.OffsetsPlayerHeadgear[drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height];
				eyelidOffset.Y -= 2f;
				Rectangle eyelidFrame = EyeLids_Texture.Frame(1, 3, 0, (int)drawPlayer.eyeHelper.CurrentEyeFrame);
				PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.Head_Sheet, null, headPosition, drawPlayer.bodyFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Head_StyleSheet, headPosition, drawPlayer.bodyFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Head_Auxilary1_StyleSheet, headPosition, drawPlayer.bodyFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 1);
                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Head_Auxilary2_StyleSheet, headPosition, drawPlayer.bodyFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 2);
                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Head_Auxilary3_StyleSheet, headPosition, drawPlayer.bodyFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 3);

                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.Eyes_Sheet, null, headPosition, drawPlayer.bodyFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Eyes_StyleSheet, headPosition, drawPlayer.bodyFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Eyes_Auxilary1_StyleSheet, headPosition, drawPlayer.bodyFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 1);
                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Eyes_Auxilary2_StyleSheet, headPosition, drawPlayer.bodyFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 2);
                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Eyes_Auxilary3_StyleSheet, headPosition, drawPlayer.bodyFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 3);

                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, mrPlagueRacesPlayer.race.EyeLids_Sheet, null, headPosition + eyelidOffset, eyelidFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.EyeLids_StyleSheet, headPosition + eyelidOffset, eyelidFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.EyeLids_Auxilary1_StyleSheet, headPosition + eyelidOffset, eyelidFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 1);
                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.EyeLids_Auxilary2_StyleSheet, headPosition + eyelidOffset, eyelidFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 2);
                PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.EyeLids_Auxilary3_StyleSheet, headPosition + eyelidOffset, eyelidFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 3);


                if (!drawPlayer.invis && drawInfo.hatHair || mrPlagueRacesPlayer.race.AlwaysDrawHair)
				{
					PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.HairAlt_StyleSheet, headPosition, drawInfo.hairFrontFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.HairAlt_Auxilary1_StyleSheet, headPosition, drawInfo.hairFrontFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 1);
                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.HairAlt_Auxilary2_StyleSheet, headPosition, drawInfo.hairFrontFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 2);
                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.HairAlt_Auxilary3_StyleSheet, headPosition, drawInfo.hairFrontFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 3);
                }
				if (!drawPlayer.invis && (!drawInfo.hatHair || mrPlagueRacesPlayer.race.AlwaysDrawHair) && (((drawPlayer.face < 0 || !ArmorIDs.Face.Sets.PreventHairDraw[drawPlayer.face]) && (drawPlayer.head < 0 || ArmorIDs.Head.Sets.DrawFullHair[drawPlayer.head] || (drawPlayer.armor[0].type == ItemID.FamiliarWig || drawPlayer.armor[10].type == ItemID.FamiliarWig))) || mrPlagueRacesPlayer.race.AlwaysDrawHair))
				{
					PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hair_StyleSheet, headPosition, drawInfo.hairFrontFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hair_Auxilary1_StyleSheet, headPosition, drawInfo.hairFrontFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 1);
                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hair_Auxilary2_StyleSheet, headPosition, drawInfo.hairFrontFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 2);
                    PlayerLayerHelpers.MakeColoredDrawDatas(ref drawInfo, null, mrPlagueRacesPlayer.race.Hair_Auxilary3_StyleSheet, headPosition, drawInfo.hairFrontFrame, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0, 3);
                }
			}
		}
    }
}