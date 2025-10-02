using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using MrPlagueRaces.Common.UI;
using MrPlagueRaces.Content.Items;
using MrPlagueRaces.Common.Systems;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.Races.Human;
using MrPlagueRaces.Common.Races.Tabaxi;
using MrPlagueRaces.Common.UI.States;

namespace MrPlagueRaces
{
	public class MrPlagueRacesPlayer : ModPlayer
	{
		public Race race; // the player's race. Change this value to change the player's race

		public Color detailColor = new Color(255, 255, 255); // the player's saved detail color
        public Color auxilaryDetailColor1 = new Color(255, 255, 255); // the player's saved detail_1 color
        public Color auxilaryDetailColor2 = new Color(255, 255, 255); // the player's saved detail_2 color
        public Color auxilaryDetailColor3 = new Color(255, 255, 255); // the player's saved detail_3 color

        public Color colorDetail = new Color(255, 255, 255); // detail color with ingame lighting applied
        public Color colorAuxilaryDetail1 = new Color(255, 255, 255); // detail_1 color with ingame lighting applied
        public Color colorAuxilaryDetail2 = new Color(255, 255, 255); // detail_2 color with ingame lighting applied
        public Color colorAuxilaryDetail3 = new Color(255, 255, 255); // detail_3 color with ingame lighting applied
        public Color colorEyes = new Color(255, 255, 255); // eye color with ingame lighting applied
        public Color colorSkin = new Color(255, 255, 255); // skin color with ingame lighting applied
        public Color colorHair = new Color(255, 255, 255); // hair color with ingame lighting applied

        public int auxilaryHairstyle1 = 0; // The player's stored hairstyle_1 value
        public int auxilaryHairstyle2 = 0; // The player's stored hairstyle_2 value
        public int auxilaryHairstyle3 = 0; // The player's stored hairstyle_3 value

        public bool noShadows = false; // boolean for disabling the player's ingame lighting
		public int strandAnimationTime = 0; // variable for triggering the Entangled Strands animation
        public int reverseStrandAnimationTime = 15; // variable for triggering the Entangled Strands animation (reversed)

        public int isNewPlayer = 0; // variable used to apply post-0.4 color value redistributions to Tabaxi, Lihzahrd, Soulbeast, Lycan, Vampire, and Fluftrodon

        public Vector2 mouseWorld = new Vector2(0, 0);

        public Rectangle headFrame = new Rectangle(0,0,40,56);
        public Rectangle bodyFrame = new Rectangle(0, 0, 40, 56);
        public Rectangle legFrame = new Rectangle(0, 0, 40, 56);
        public Rectangle hairFrame = new Rectangle(0, 0, 40, 56);

        public override void SaveData(TagCompound tag) // This is where race data is saved
        {
			// Save custom player data (race, detail color, etc)
			if (race != null)
			{
				tag["Race"] = race.FullName;
				tag["detailColorR"] = detailColor.R;
				tag["detailColorG"] = detailColor.G;
				tag["detailColorB"] = detailColor.B;

                tag["auxilaryDetailColor1R"] = auxilaryDetailColor1.R;
                tag["auxilaryDetailColor1G"] = auxilaryDetailColor1.G;
                tag["auxilaryDetailColor1B"] = auxilaryDetailColor1.B;

                tag["auxilaryDetailColor2R"] = auxilaryDetailColor2.R;
                tag["auxilaryDetailColor2G"] = auxilaryDetailColor2.G;
                tag["auxilaryDetailColor2B"] = auxilaryDetailColor2.B;

                tag["auxilaryDetailColor3R"] = auxilaryDetailColor3.R;
                tag["auxilaryDetailColor3G"] = auxilaryDetailColor3.G;
                tag["auxilaryDetailColor3B"] = auxilaryDetailColor3.B;

                tag["auxilaryHairstyle1"] = auxilaryHairstyle1;
                tag["auxilaryHairstyle2"] = auxilaryHairstyle2;
                tag["auxilaryHairstyle3"] = auxilaryHairstyle3;

                tag["isNewPlayer"] = isNewPlayer;

            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        { 
            // Enabling noShadows will prevent the player from receiving ingame lighting. This boolean is enabled for ingame UICharacters, which would otherwise copy the lighting of the client player
            if (noShadows)
			{
                fullBright = true;
            }
        }

        public override void LoadData(TagCompound tag) // This is where saved character data is loaded
        {
			// Apply saved race
			if ((tag.ContainsKey("Race") && RaceLoader.TryGetRace(tag.GetString("Race"), out var loadedRace)))
            {
                race = loadedRace;
			}
			else
            {
                if (tag.GetString("Race") == "MrPlagueRaces/Wendigo") // Catch and swap the Wendigo ID to Soulbeast
                {
                    RaceLoader.TryGetRace("MrPlagueRaces/Soulbeast", out var newRace);
                    race = newRace;
                }
				else
				{
                    RaceLoader.TryGetRace("MrPlagueRaces/Human", out var newRace);
                    race = newRace;
                }
            }

			// Apply saved detail color
			if (tag.ContainsKey("detailColorR"))
			{
				detailColor.R = (byte)tag["detailColorR"];
			}
			if (tag.ContainsKey("detailColorG"))
			{
				detailColor.G = (byte)tag["detailColorG"];
			}
			if (tag.ContainsKey("detailColorB"))
			{
				detailColor.B = (byte)tag["detailColorB"];
            }

			// Apply saved auxilary detail color 1
            if (tag.ContainsKey("auxilaryDetailColor1R"))
            {
                auxilaryDetailColor1.R = (byte)tag["auxilaryDetailColor1R"];
            }
            if (tag.ContainsKey("auxilaryDetailColor1G"))
            {
                auxilaryDetailColor1.G = (byte)tag["auxilaryDetailColor1G"];
            }
            if (tag.ContainsKey("auxilaryDetailColor1B"))
            {
                auxilaryDetailColor1.B = (byte)tag["auxilaryDetailColor1B"];
            }

            // Apply saved auxilary detail color 2
            if (tag.ContainsKey("auxilaryDetailColor2R"))
            {
                auxilaryDetailColor2.R = (byte)tag["auxilaryDetailColor2R"];
            }
            if (tag.ContainsKey("auxilaryDetailColor2G"))
            {
                auxilaryDetailColor2.G = (byte)tag["auxilaryDetailColor2G"];
            }
            if (tag.ContainsKey("auxilaryDetailColor2B"))
            {
                auxilaryDetailColor2.B = (byte)tag["auxilaryDetailColor2B"];
            }

            // Apply saved auxilary detail color 3
            if (tag.ContainsKey("auxilaryDetailColor3R"))
            {
                auxilaryDetailColor3.R = (byte)tag["auxilaryDetailColor3R"];
            }
            if (tag.ContainsKey("auxilaryDetailColor3G"))
            {
                auxilaryDetailColor3.G = (byte)tag["auxilaryDetailColor3G"];
            }
            if (tag.ContainsKey("auxilaryDetailColor3B"))
            {
                auxilaryDetailColor3.B = (byte)tag["auxilaryDetailColor3B"];
            }

            // Apply saved auxilary hairstyles
            if (tag.ContainsKey("auxilaryHairstyle1"))
            {
                auxilaryHairstyle1 = (int)tag["auxilaryHairstyle1"];
            }
            if (tag.ContainsKey("auxilaryHairstyle2"))
            {
                auxilaryHairstyle2 = (int)tag["auxilaryHairstyle2"];
            }
            if (tag.ContainsKey("auxilaryHairstyle3"))
            {
                auxilaryHairstyle3 = (int)tag["auxilaryHairstyle3"];
            }
            if (tag.ContainsKey("isNewPlayer"))
            {
                isNewPlayer = (int)tag["isNewPlayer"];
            }
            if (isNewPlayer == 0) //apply post-0.4 color value redistributions to pre-0.4 Tabaxi, Merfolk, Lihzahrd, Soulbeast, Lycan, Vampire, and Fluftrodon players
            {
                Color originalSkinColor = Player.skinColor;
                Color originalHairColor = Player.hairColor;
                Color originalDetailColor = detailColor;
                if (tag.GetString("Race") == "MrPlagueRaces/Tabaxi")
                {
                    auxilaryDetailColor1 = originalHairColor;
                    Player.hairColor = originalSkinColor;
                }
                if (tag.GetString("Race") == "MrPlagueRaces/Merfolk")
                {
                    Player.hairColor = originalDetailColor;
                }
                if (tag.GetString("Race") == "MrPlagueRaces/Lihzahrd")
                {
                    Player.hairColor = originalDetailColor;
                }
                if (tag.GetString("Race") == "MrPlagueRaces/Soulbeast" || tag.GetString("Race") == "MrPlagueRaces/Wendigo")
                {
                    Player.hairColor = originalSkinColor;
                    detailColor = originalHairColor;
                }
                if (tag.GetString("Race") == "MrPlagueRaces/Lycan")
                {
                    auxilaryDetailColor1 = originalHairColor;
                    Player.hairColor = originalSkinColor;
                }
                if (tag.GetString("Race") == "MrPlagueRaces/Vampire")
                {
                    Player.hairColor = originalSkinColor;
                }
                if (tag.GetString("Race") == "MrPlagueRaces/Fluftrodon")
                {
                    Player.hairColor = originalSkinColor;
                    detailColor = originalHairColor;
                    auxilaryDetailColor1 = originalDetailColor;
                    auxilaryDetailColor2 = originalDetailColor;
                    auxilaryDetailColor3 = originalDetailColor;
                }
                isNewPlayer = 1;
            }

            // If the player has a hairstyle that is outside of their race's hairstyle limit, cap it
            if ((Player.hair + 1) > GetRaceHairCount(Player) && GetRaceHairCount(Player) != 0)
			{
				Player.hair = (GetRaceHairCount(Player) - 1);
            }
            if ((auxilaryHairstyle1 + 1) > GetRaceHairCount(Player, 1) && GetRaceHairCount(Player, 1) != 0)
            {
                auxilaryHairstyle1 = (GetRaceHairCount(Player, 1) - 1);
            }
            if ((auxilaryHairstyle2 + 1) > GetRaceHairCount(Player, 2) && GetRaceHairCount(Player, 2) != 0)
            {
                auxilaryHairstyle2 = (GetRaceHairCount(Player, 2) - 1);
            }
            if ((auxilaryHairstyle3 + 1) > GetRaceHairCount(Player, 3) && GetRaceHairCount(Player, 3) != 0)
            {
                auxilaryHairstyle3 = (GetRaceHairCount(Player, 3) - 1);
            }
        }

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) // This is where multiplayer race information is synced
		{
            // Send player data to server
            SyncRace(toWho, fromWho);
            SyncPlayerAppearance(toWho, fromWho);
        }

        public void SyncPlayerAppearance(int toWho, int fromWho)
        {
            SyncSkinColor(toWho, fromWho);
            SyncHairColor(toWho, fromWho);
            SyncEyeColor(toWho, fromWho);
            SyncDetailColor(toWho, fromWho);
            SyncAuxDetail1Color(toWho, fromWho);
            SyncAuxDetail2Color(toWho, fromWho);
            SyncAuxDetail3Color(toWho, fromWho);
            SyncShirtColor(toWho, fromWho);
            SyncUndershirtColor(toWho, fromWho);
            SyncPantsColor(toWho, fromWho);
            SyncShoeColor(toWho, fromWho);
            SyncHairstyles(toWho, fromWho);
            SyncClothStyle(toWho, fromWho);
        }

        public void SyncRace(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncRace);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(race.Id);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncSkinColor(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncSkinColor);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(Player.skinColor.R);
                    packet.Write(Player.skinColor.G);
                    packet.Write(Player.skinColor.B);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncHairColor(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncHairColor);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(Player.hairColor.R);
                    packet.Write(Player.hairColor.G);
                    packet.Write(Player.hairColor.B);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncEyeColor(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncEyeColor);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(Player.eyeColor.R);
                    packet.Write(Player.eyeColor.G);
                    packet.Write(Player.eyeColor.B);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncDetailColor(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncDetailColor);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(detailColor.R);
                    packet.Write(detailColor.G);
                    packet.Write(detailColor.B);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncAuxDetail1Color(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncAuxDetail1Color);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(auxilaryDetailColor1.R);
                    packet.Write(auxilaryDetailColor1.G);
                    packet.Write(auxilaryDetailColor1.B);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncAuxDetail2Color(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncAuxDetail2Color);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(auxilaryDetailColor2.R);
                    packet.Write(auxilaryDetailColor2.G);
                    packet.Write(auxilaryDetailColor2.B);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncAuxDetail3Color(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncAuxDetail3Color);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(auxilaryDetailColor3.R);
                    packet.Write(auxilaryDetailColor3.G);
                    packet.Write(auxilaryDetailColor3.B);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncShirtColor(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncShirtColor);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(Player.shirtColor.R);
                    packet.Write(Player.shirtColor.G);
                    packet.Write(Player.shirtColor.B);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncUndershirtColor(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncUndershirtColor);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(Player.underShirtColor.R);
                    packet.Write(Player.underShirtColor.G);
                    packet.Write(Player.underShirtColor.B);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncPantsColor(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncPantsColor);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(Player.pantsColor.R);
                    packet.Write(Player.pantsColor.G);
                    packet.Write(Player.pantsColor.B);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncShoeColor(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncShoeColor);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(Player.shoeColor.R);
                    packet.Write(Player.shoeColor.G);
                    packet.Write(Player.shoeColor.B);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncHairstyles(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncHairstyles);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(Player.hair);
                    packet.Write(auxilaryHairstyle1);
                    packet.Write(auxilaryHairstyle2);
                    packet.Write(auxilaryHairstyle3);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncClothStyle(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (race != null)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncClothStyle);
                    packet.Write((byte)Player.whoAmI);

                    packet.Write(Player.skinVariant);

                    packet.Send(toWho, fromWho);
                }
            }
        }

        public void SyncPosition(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncPosition);
                packet.Write((byte)Player.whoAmI);

                packet.Write(Player.position.X);
                packet.Write(Player.position.Y);

                packet.Send(toWho, fromWho);
            }
        }

        public void SyncVelocity(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncVelocity);
                packet.Write((byte)Player.whoAmI);

                packet.Write(Player.velocity.X);
                packet.Write(Player.velocity.Y);

                packet.Send(toWho, fromWho);
            }
        }

        public void SyncRotationsAndOffsets(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncRotationsAndOffsets);
                packet.Write((byte)Player.whoAmI);

                packet.Write(Player.fullRotationOrigin.X);
                packet.Write(Player.fullRotationOrigin.Y);

                packet.Write(Player.fullRotation);

                packet.Write(Player.headRotation);
                packet.Write(Player.bodyRotation);
                packet.Write(Player.legRotation);

                packet.Write(Player.headPosition.X);
                packet.Write(Player.headPosition.Y);

                packet.Write(Player.bodyPosition.X);
                packet.Write(Player.bodyPosition.Y);

                packet.Write(Player.legPosition.X);
                packet.Write(Player.legPosition.Y);

                packet.Send(toWho, fromWho);
            }
        }

        public void SyncDismount(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncDismount);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }

        public void SyncBat(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncBat);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }

        public void SyncCrawl(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncCrawl);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }

        public void SyncMouseWorld(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncMouseWorld);
                packet.Write((byte)Player.whoAmI);

                packet.Write(mouseWorld.X);
                packet.Write(mouseWorld.Y);

                packet.Send(toWho, fromWho);
            }
        }

        public void SyncPlayerFrames(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncPlayerFrames);
                packet.Write((byte)Player.whoAmI);

                packet.Write(headFrame.X);
                packet.Write(headFrame.Y);

                packet.Write(bodyFrame.X);
                packet.Write(bodyFrame.Y);

                packet.Write(legFrame.X);
                packet.Write(legFrame.Y);

                packet.Write(hairFrame.X);
                packet.Write(hairFrame.Y);

                packet.Send(toWho, fromWho);
            }
        }

        public void SyncBlink(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncBlink);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }

        public void DerpkinLeapSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.DerpkinLeapSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void DerpkinSpinSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.DerpkinSpinSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void DragonkinFireballSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.DragonkinFireballSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void FluftrodonChargeSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.FluftrodonChargeSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void FluftrodonJumpSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.FluftrodonJumpSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void GoblinChargeSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.GoblinChargeSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void GoblinShootSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.GoblinShootSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void KenkuDashSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.KenkuDashSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void KenkuSummonFeathersSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.KenkuSummonFeathersSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void KenkuFlapSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.KenkuFlapSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void KoboldMineSummonSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.KoboldMineSummonSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void KoboldClusterMineSummonSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.KoboldClusterMineSummonSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void KoboldExplosionSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.KoboldExplosionSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void LihzahrdSummonGolemSound(int toWho, int fromWho, float X, float Y)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.LihzahrdSummonGolemSound);
                packet.Write((byte)Player.whoAmI);

                packet.Write(X);
                packet.Write(Y);

                packet.Send(toWho, fromWho);
            }
        }
        public void LycanChargeSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.LycanChargeSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void LycanChargeTickSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.LycanChargeTickSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void LycanRewindSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.LycanRewindSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void LycanTeleportSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.LycanTeleportSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void MushfolkTeleportSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.MushfolkTeleportSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void TabaxiDashChargeSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.TabaxiDashChargeSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void TabaxiDashExecuteSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.TabaxiDashExecuteSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void TabaxiSetTeleportSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.TabaxiSetTeleportSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void TabaxiUseTeleportSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.TabaxiUseTeleportSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void VampireTransformSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.VampireTransformSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void VampireExitTransformationSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.VampireExitTransformationSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void VampireShootTongueSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.VampireShootTongueSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void SkeletonSummonSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.SkeletonSummonSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void SkeletonTeleportSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.SkeletonTeleportSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void SoulbeastRendSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.SoulbeastRendSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void SoulbeastRendExitSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.SoulbeastRendExitSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void EntangledStrandsConfirmSound(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.EntangledStrandsConfirmSound);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }

        public void FluftrodonChargeDust(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.FluftrodonChargeDust);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void FluftrodonJumpDust(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.FluftrodonJumpDust);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void GoblinHarvesterFireballDust(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.GoblinHarvesterFireballDust);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void GoblinHarvesterShootDust(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.GoblinHarvesterShootDust);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void KoboldExplosionSparkDust(int toWho, int fromWho, float X, float Y, int width, int height)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.KoboldExplosionSparkDust);
                packet.Write((byte)Player.whoAmI);

                packet.Write(X);
                packet.Write(Y);

                packet.Write(width);
                packet.Write(height);

                packet.Send(toWho, fromWho);
            }
        }
        public void LycanRedDust(int toWho, int fromWho, float X, float Y, float Scale)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.LycanRedDust);
                packet.Write((byte)Player.whoAmI);

                packet.Write(X);
                packet.Write(Y);
                packet.Write(Scale);

                packet.Send(toWho, fromWho);
            }
        }
        public void LycanBlueDust(int toWho, int fromWho, float X, float Y, float Scale)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.LycanBlueDust);
                packet.Write((byte)Player.whoAmI);

                packet.Write(X);
                packet.Write(Y);
                packet.Write(Scale);

                packet.Send(toWho, fromWho);
            }
        }
        public void LycanTeleportBurstDust(int toWho, int fromWho, float X, float Y)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.LycanTeleportBurstDust);
                packet.Write((byte)Player.whoAmI);

                packet.Write(X);
                packet.Write(Y);

                packet.Send(toWho, fromWho);
            }
        }
        public void TabaxiTeleportDust(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.TabaxiTeleportDust);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void VampireTransformDust(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.VampireTransformDust);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void SoulbeastRendDust(int toWho, int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.SoulbeastRendDust);
                packet.Write((byte)Player.whoAmI);

                packet.Send(toWho, fromWho);
            }
        }
        public void EntangledStrandsDust(int toWho, int fromWho, int animTime)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MrPlagueRacesMessageType.EntangledStrandsDust);
                packet.Write((byte)Player.whoAmI);

                packet.Write(animTime);

                packet.Send(toWho, fromWho);
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            // Apply ingame lighting to player colors

            // {T} All of the height multipliers here have been changed from 0.5 to 0.25, so that the colours all change at the same time while the player moves.
            // Previously, the colours would change at different times (eg. colorSkin would not match vanilla's colorHead value, because the offsets were different)
            // resulting in some odd lighting. Subtle, but noticeable!
            colorDetail = Player.GetImmuneAlpha(Lighting.GetColorClamped((int)((double)Player.position.X + (double)Player.width * 0.5) / 16, (int)(((double)Player.position.Y + (double)Player.height * 0.25) / 16.0), detailColor), drawInfo.shadow);

            colorAuxilaryDetail1 = Player.GetImmuneAlpha(Lighting.GetColorClamped((int)((double)Player.position.X + (double)Player.width * 0.5) / 16, (int)(((double)Player.position.Y + (double)Player.height * 0.25) / 16.0), auxilaryDetailColor1), drawInfo.shadow);
            colorAuxilaryDetail2 = Player.GetImmuneAlpha(Lighting.GetColorClamped((int)((double)Player.position.X + (double)Player.width * 0.5) / 16, (int)(((double)Player.position.Y + (double)Player.height * 0.25) / 16.0), auxilaryDetailColor2), drawInfo.shadow);
            colorAuxilaryDetail3 = Player.GetImmuneAlpha(Lighting.GetColorClamped((int)((double)Player.position.X + (double)Player.width * 0.5) / 16, (int)(((double)Player.position.Y + (double)Player.height * 0.25) / 16.0), auxilaryDetailColor3), drawInfo.shadow);

            colorEyes = Player.GetImmuneAlpha(Lighting.GetColorClamped((int)((double)Player.position.X + (double)Player.width * 0.5) / 16, (int)(((double)Player.position.Y + (double)Player.height * 0.25) / 16.0), Player.eyeColor), drawInfo.shadow);
            colorSkin = Player.GetImmuneAlpha(Lighting.GetColorClamped((int)((double)Player.position.X + (double)Player.width * 0.5) / 16, (int)(((double)Player.position.Y + (double)Player.height * 0.25) / 16.0), Player.skinColor), drawInfo.shadow);
            colorHair = Player.GetImmuneAlpha(Lighting.GetColorClamped((int)((double)Player.position.X + (double)Player.width * 0.5) / 16, (int)(((double)Player.position.Y + (double)Player.height * 0.25) / 16.0), Player.hairColor), drawInfo.shadow);
            if (noShadows || drawInfo.headOnlyRender)
            {
                colorDetail = Player.GetImmuneAlpha(detailColor, 0f);
                colorAuxilaryDetail1 = Player.GetImmuneAlpha(auxilaryDetailColor1, 0f);
                colorAuxilaryDetail2 = Player.GetImmuneAlpha(auxilaryDetailColor2, 0f);
                colorAuxilaryDetail3 = Player.GetImmuneAlpha(auxilaryDetailColor3, 0f);
            }
            if (Player.dead)
            {
                TextureAssets.Ghost = GetRaceTexture(Player, "Ghost");
            }
            else
            {
                TextureAssets.Ghost = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
            }
            if (race == null)
            {
                for (int i = 0; i < RaceLoader.Races.Count; i++)
                {
                    if (RaceLoader.Races[i].Name == "Human")
                    {
                        race = RaceLoader.Races[i];
                    }
                }
            }

            // Calamity Mod Fix: Prevents certain playerlayers from becoming invisible when wearing Calamity armor that modifies playerdrawing
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                drawInfo.armorHidesArms = false;
            }

            if (Player.whoAmI != Main.myPlayer && !noShadows)
            {
                Player.headFrame = headFrame;
                Player.bodyFrame = bodyFrame;
                Player.legFrame = legFrame;
            }
        }

        public override void PreUpdate()
        {
            // If the player became a ghost without being in hardcore, unghost them
            if (Player.dead && Player.ghost && Player.difficulty != 2) {
				Player.ghost = false;
            }

            if (Player.whoAmI == Main.myPlayer)
            {
                mouseWorld = Main.MouseWorld;
                headFrame = Player.headFrame;
                bodyFrame = Player.bodyFrame;
                legFrame = Player.legFrame;
                hairFrame = Player.hairFrame;
            }

			// Particle effect timer for Entangled Strands
            if (strandAnimationTime > 0)
            {
                StrandAnimation(strandAnimationTime);
                strandAnimationTime--;
            }
            if (reverseStrandAnimationTime < 15)
            {
                StrandAnimation(reverseStrandAnimationTime);
                reverseStrandAnimationTime++;
            }
        }

        public override void PostUpdate()
        {
            if (Player.whoAmI == Main.myPlayer)
            {
                SyncVelocity(-1, Main.myPlayer);
                SyncPosition(-1, Main.myPlayer);
                SyncMouseWorld(-1, Main.myPlayer);
                SyncPlayerFrames(-1, Main.myPlayer);
            }
        }

        // Displays a swirling pattern around the player
        public void StrandAnimation(int animTime, int blueDustID = 295, int redDustID = 296)
		{
            int itemTime = animTime;
            int itemTimeMax = 15;
            float num21 = itemTimeMax;
            num21 = (num21 - itemTime) / num21;
            float num19 = 44f;
            float num18 = 3.14156f * 3f;
            Vector2 vector4 = Utils.RotatedBy(new Vector2(15f, 0f), (double)(num18 * num21), default(Vector2));
            vector4.X *= Player.direction;
            Vector2 vector5 = default(Vector2);
            for (int num17 = 0; num17 < 2; num17++)
            {
                int dust = blueDustID;
                if (num17 == 1)
                {
                    vector4.X *= -1f;
                    dust = redDustID;
                }
                vector5 = new Vector2(vector4.X, num19 * (1f - num21) - num19 + (float)(Player.height / 2));
                vector5 += Player.Center;
                int num16 = Dust.NewDust(vector5, 0, 0, dust, 0f, 0f, 100);
                Main.dust[num16].position = vector5;
                Main.dust[num16].noGravity = true;
                Main.dust[num16].velocity = Vector2.Zero;
                Main.dust[num16].scale = 1.3f;
                Main.dust[num16].customData = this;
            }
            if (Player.whoAmI == Main.myPlayer)
            {
                EntangledStrandsDust(-1, Main.myPlayer, animTime);
            }
        }

		public override void OnHurt(Player.HurtInfo info)
		{
			// Play the custom hurt sound of the player's race
			if (race != null && ModContent.GetInstance<MrPlagueRacesConfig>().raceHurtSounds)
			{
                // {T} Using ExecuteRaceSound instead of PlayRaceSound because PlayRaceSound will send a networked packet on the server which
                // WILL cause double-ups of the hurt sound on multiplayer games, and is more noticeable with lag. Hurt sounds do not need to be networked
                // as they already run on all clients.
                ExecuteRaceSound(Player, "Hurt");
			}
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers) {
			// Cancel the vanilla hurt sound call
			if (race != null && ModContent.GetInstance<MrPlagueRacesConfig>().raceHurtSounds) {
				modifiers.DisableSound();
			}
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			// Cancel the vanilla death sound call
			if (race != null && ModContent.GetInstance<MrPlagueRacesConfig>().raceHurtSounds)
			{
				playSound = false;
			}
			return true;
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			// If the player dies, close the ingame Race Changing UI
			if (RaceChangeCameraModifier.MovingOutward)
			{
                ModContent.GetInstance<RaceChangeUISystem>().raceChangeUI.ReinstateOriginalValues();
                ModContent.GetInstance<RaceChangeUISystem>().HideMyUI();
            }
            // Play the custom death sound of the player's race
            if (race != null && ModContent.GetInstance<MrPlagueRacesConfig>().raceHurtSounds)
			{
                // {T} Using ExecuteRaceSound instead of PlayRaceSound because PlayRaceSound will send a networked packet on the server which
                // WILL cause double-ups of the death sound on multiplayer games, and is more noticeable with lag. Hurt sounds do not need to be networked
                // as they already run on all clients.
                ExecuteRaceSound(Player, "Killed");
			}
		}

		// A function for grabbing a texture from the player's race's conventional filepath
		public Asset<Texture2D> GetRaceTexture(Player player, string texturePath, string defaultTexturePath = "MrPlagueRaces/Assets/Textures/Blank")
		{
			Asset<Texture2D> Race_Texture = ModContent.Request<Texture2D>($"{defaultTexturePath}");
			if (race != null)
			{
				if (player.Male)
				{
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/{texturePath}"))
					{
						Race_Texture = ModContent.Request<Texture2D>($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/{texturePath}");
					}
				}
				else
				{
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Female/{texturePath}"))
					{
						Race_Texture = ModContent.Request<Texture2D>($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Female/{texturePath}");
					}
					else
					{
						if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/{texturePath}"))
						{
							Race_Texture = ModContent.Request<Texture2D>($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/{texturePath}");
						}
					}
				}
			}
			return Race_Texture;
        }

        // A function for grabbing a texture from any race. if a path parameter is left blank, it will default to the conventional filepath
        public Asset<Texture2D> GetAnyRaceTexture(Player player, string texturePath, string defaultTexturePath = "MrPlagueRaces/Assets/Textures/Blank", string modPath = "", string textureLocationPath = "", string raceNamePath = "")
        {
            modPath = modPath == "" ? $"{race.Mod.Name}" : modPath;
            textureLocationPath = textureLocationPath == "" ? $"{race.TextureLocation}" : textureLocationPath;
            raceNamePath = raceNamePath == "" ? $"{race.Name}" : raceNamePath;

            Asset<Texture2D> Race_Texture = ModContent.Request<Texture2D>($"{defaultTexturePath}");
            if (race != null)
            {
                if (player.Male)
                {
                    if (ModContent.HasAsset($"{modPath}/{textureLocationPath}/{raceNamePath}/Male/{texturePath}"))
                    {
                        Race_Texture = ModContent.Request<Texture2D>($"{modPath}/{textureLocationPath}/{raceNamePath}/Male/{texturePath}");
                    }
                }
                else
                {
                    if (ModContent.HasAsset($"{modPath}/{textureLocationPath}/{raceNamePath}/Female/{texturePath}"))
                    {
                        Race_Texture = ModContent.Request<Texture2D>($"{modPath}/{textureLocationPath}/{raceNamePath}/Female/{texturePath}");
                    }
                    else
                    {
                        if (ModContent.HasAsset($"{modPath}/{textureLocationPath}/{raceNamePath}/Male/{texturePath}"))
                        {
                            Race_Texture = ModContent.Request<Texture2D>($"{modPath}/{textureLocationPath}/{raceNamePath}/Male/{texturePath}");
                        }
                    }
                }
            }
            return Race_Texture;
        }

        // Play a sound using the racial audio filepath
        public void PlayRaceSound(Player player, string soundPath)
		{
			if (race != null)
            {
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MrPlagueRacesMessageType.ExecuteRaceSound);
                    packet.Write((byte)player.whoAmI);
                    packet.Write(soundPath);
                    packet.Send();
                }
                else
                {
                    ExecuteRaceSound(player, soundPath);
                }
			}
		}

        // Play a sound (based on the player's gender) with a random pitch
        public void ExecuteRaceSound(Player player, string soundPath)
        {
            if (race != null)
			{
				if (player.Male)
				{
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Male/{soundPath}"))
					{
						if (Main.rand.Next(3) == 1)
						{
							SoundEngine.PlaySound(new SoundStyle($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Male/{soundPath}") with {Pitch = -0.1f, Volume = 1f}, player.Center);
						}
						else if (Main.rand.Next(3) == 2)
						{
							SoundEngine.PlaySound(new SoundStyle($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Male/{soundPath}") with {Pitch = 0f, Volume = 1f}, player.Center);
						}
						else
						{
							SoundEngine.PlaySound(new SoundStyle($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Male/{soundPath}") with {Pitch = 0.1f, Volume = 1f}, player.Center);
						}
					}
					else
					{
						if (Main.rand.Next(3) == 1)
						{
							SoundEngine.PlaySound(new SoundStyle($"{Mod.Name}/Assets/Sounds/Players/Races/Human/Male/{soundPath}") with {Pitch = -0.1f, Volume = 1f}, player.Center);
						}
						else if (Main.rand.Next(3) == 2)
						{
							SoundEngine.PlaySound(new SoundStyle($"{Mod.Name}/Assets/Sounds/Players/Races/Human/Male/{soundPath}") with {Pitch = 0f, Volume = 1f}, player.Center);
						}
						else
						{
							SoundEngine.PlaySound(new SoundStyle($"{Mod.Name}/Assets/Sounds/Players/Races/Human/Male/{soundPath}") with {Pitch = 0.1f, Volume = 1f}, player.Center);
						}
					}
				}
				else
				{
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Female/{soundPath}"))
					{
						if (Main.rand.Next(3) == 1)
						{
							SoundEngine.PlaySound(new SoundStyle($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Female/{soundPath}") with {Pitch = -0.1f, Volume = 1f}, player.Center);
						}
						else if (Main.rand.Next(3) == 2)
						{
							SoundEngine.PlaySound(new SoundStyle($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Female/{soundPath}") with {Pitch = 0f, Volume = 1f}, player.Center);
						}
						else
						{
							SoundEngine.PlaySound(new SoundStyle($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Female/{soundPath}") with {Pitch = 0.1f, Volume = 1f}, player.Center);
						}
					}
					else if (ModContent.HasAsset($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Male/{soundPath}"))
					{
						if (Main.rand.Next(3) == 1)
						{
							SoundEngine.PlaySound(new SoundStyle($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Male/{soundPath}") with {Pitch = -0.1f, Volume = 1f}, player.Center);
						}
						else if (Main.rand.Next(3) == 2)
						{
							SoundEngine.PlaySound(new SoundStyle($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Male/{soundPath}") with {Pitch = 0f, Volume = 1f}, player.Center);
						}
						else
						{
							SoundEngine.PlaySound(new SoundStyle($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Male/{soundPath}") with {Pitch = 0.1f, Volume = 1f}, player.Center);
						}
					}
					else if (ModContent.HasAsset($"{race.Mod.Name}/Assets/Sounds/Players/Races/Human/Female/{soundPath}"))
					{
						if (Main.rand.Next(3) == 1)
						{
							SoundEngine.PlaySound(new SoundStyle($"{Mod.Name}/Assets/Sounds/Players/Races/Human/Female/{soundPath}") with {Pitch = -0.1f, Volume = 1f}, player.Center);
						}
						else if (Main.rand.Next(3) == 2)
						{
							SoundEngine.PlaySound(new SoundStyle($"{Mod.Name}/Assets/Sounds/Players/Races/Human/Female/{soundPath}") with {Pitch = 0f, Volume = 1f}, player.Center);
						}
						else
						{
							SoundEngine.PlaySound(new SoundStyle($"{Mod.Name}/Assets/Sounds/Players/Races/Human/Female/{soundPath}") with {Pitch = 0.1f, Volume = 1f}, player.Center);
						}
					}
					else
					{
						if (Main.rand.Next(3) == 1)
						{
							SoundEngine.PlaySound(new SoundStyle($"{Mod.Name}/Assets/Sounds/Players/Races/Human/Male/{soundPath}") with {Pitch = -0.1f, Volume = 1f}, player.Center);
						}
						else if (Main.rand.Next(3) == 2)
						{
							SoundEngine.PlaySound(new SoundStyle($"{Mod.Name}/Assets/Sounds/Players/Races/Human/Male/{soundPath}") with {Pitch = 0f, Volume = 1f}, player.Center);
						}
						else
						{
							SoundEngine.PlaySound(new SoundStyle($"{Mod.Name}/Assets/Sounds/Players/Races/Human/Male/{soundPath}") with {Pitch = 0.1f, Volume = 1f}, player.Center);
						}
					}
				}
			}
		}

        // Examines the player's race's file directory for detail color folders. Returns the number of folders found
        public int GetDetailColorCount(Player player)
		{
			int auxilaryCount = 0;
			Color invalidColor = new Color(0, 0, 0) * 0;
            if (race.AuxilaryDetailColor1 != invalidColor)
            {
				auxilaryCount += 1;
            }
            if (race.AuxilaryDetailColor2 != invalidColor)
            {
                auxilaryCount += 1;
            }
            if (race.AuxilaryDetailColor3 != invalidColor)
            {
                auxilaryCount += 1;
            }
			return 1 + auxilaryCount;
        }

        // Examines the player's race's RaceSheet index for hair sheets. Returns the count of the folder with the highest number of hair sheets. Automatically updated if more hair RaceSheets are added at any point
        public int GetRaceHairCount(Player player, int hairMode = 0)
        {
            int largestCount = 0;
            int currentCount = 0;
            if (race != null)
            {
                for (int i = 0; i < 16; i++)
                {
                    currentCount = 0;
                    for (int j = 0; j < 165; j++)
                    {
                        if (hairMode == 0 && (race.Hair_StyleSheet[i, j].OverrideTexture[0] != null ? !race.Hair_StyleSheet[i, j].IsOverrideTextureBlank(0) : race.Hair_StyleSheet[i, j].Texture[0] != ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank")))
                        {
                            currentCount++;
                        }
                        else if (hairMode == 1 && (race.Hair_Auxilary1_StyleSheet[i, j].OverrideTexture[0] != null ? !race.Hair_Auxilary1_StyleSheet[i, j].IsOverrideTextureBlank(0) : race.Hair_Auxilary1_StyleSheet[i, j].Texture[0] != ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank")))
                        {
                            currentCount++;
                        }
                        else if (hairMode == 2 && (race.Hair_Auxilary2_StyleSheet[i, j].OverrideTexture[0] != null ? !race.Hair_Auxilary2_StyleSheet[i, j].IsOverrideTextureBlank(0) : race.Hair_Auxilary2_StyleSheet[i, j].Texture[0] != ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank")))
                        {
                            currentCount++;
                        }
                        else if (hairMode == 3 && (race.Hair_Auxilary3_StyleSheet[i, j].OverrideTexture[0] != null ? !race.Hair_Auxilary3_StyleSheet[i, j].IsOverrideTextureBlank(0) : race.Hair_Auxilary3_StyleSheet[i, j].Texture[0] != ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank")))
                        {
                            currentCount++;
                        }
                    }
                    if (currentCount > largestCount)
                    {
                        largestCount = currentCount;
                    }
                }
            }
			return largestCount;
		}
	}
}