using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using Terraria.Audio;
using Terraria.ID;
using MrPlagueRaces.Common.UI.States;

namespace MrPlagueRaces.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    public class UIRedirectionSystem : ModSystem
	{
		public override void Load() => Terraria.On_Main.Draw += InterceptMenus;
		public override void Unload() => Terraria.On_Main.Draw -= InterceptMenus;

        internal RaceDresserUI raceDresserUI;
        internal RaceHairstyleUI raceHairstyleUI;

        private void InterceptMenus(Terraria.On_Main.orig_Draw orig, Main self, GameTime gameTime)
		{
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceSelectionInPlayerCreationUI && Main.gameMenu)
			{
                // Intercept the Player Creation UI
                UserInterface userInterface = Main.gameMenu ? Main.MenuUI : Main.InGameUI;
                if (userInterface.CurrentState is UICharacterCreation)
                {
                    Main.PendingPlayer = new Player();
                    Main.menuMode = 888;
                    Main.MenuUI.SetState(new MrPlagueUICharacterCreation(Main.PendingPlayer));
                }
            }
            if (!Main.gameMenu)
            {
                // Intercept the Dresser UI
                if (ModContent.GetInstance<MrPlagueRacesConfig>().overrideDresserUI && Main.clothesWindow)
                {
                    Main.CancelClothesWindow(true);
                    HideHairstyleUI();
                    ShowDresserUI();
                }
                // Intercept the Hairstyle UI
                if (ModContent.GetInstance<MrPlagueRacesConfig>().overrideHairstyleUI && Main.hairWindow)
                {
                    Main.hairWindow = false;
                    if (Main.LocalPlayer.talkNPC > -1 && Main.npc[Main.LocalPlayer.talkNPC].type == 353)
                    {
                        Main.LocalPlayer.SetTalkNPC(-1);
                    }
                    HideDresserUI();
                    ShowHairstyleUI();
                }
            }
            orig(self, gameTime);
        }

        public void ShowDresserUI()
        {
            IngameFancyUI.OpenUIState(raceDresserUI);
            raceDresserUI.StoreOriginalValues();
            raceDresserUI.SetDefaultRace();
        }

        public void ShowHairstyleUI()
        {
            IngameFancyUI.OpenUIState(raceHairstyleUI);
            raceHairstyleUI.StoreOriginalValues();
            raceHairstyleUI.SetDefaultRace();
        }

        public void HideDresserUI()
        {
            if (Main.InGameUI.CurrentState == raceDresserUI)
            {
                raceDresserUI.ReinstateOriginalValues();
                IngameFancyUI.Close();
            }
        }

        public void HideHairstyleUI()
        {
            if (Main.InGameUI.CurrentState == raceHairstyleUI)
            {
                raceHairstyleUI.ReinstateOriginalValues();
                Main.LocalPlayer.SetTalkNPC(-1);
                IngameFancyUI.Close();
            }
        }

        public void ToggleDresserUI()
        {
            if (Main.InGameUI.CurrentState != raceDresserUI)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                ShowDresserUI();
            }
            else
            {
                HideDresserUI();
            }
        }

        public void ToggleHairstyleUI()
        {
            if (Main.InGameUI.CurrentState != raceHairstyleUI)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                ShowHairstyleUI();
            }
            else
            {
                HideHairstyleUI();
            }
        }

        public override void PostSetupContent()
        {
            raceDresserUI = new RaceDresserUI();
            raceDresserUI.Activate();

            raceHairstyleUI = new RaceHairstyleUI();
            raceHairstyleUI.Activate();
        }
        public override void OnWorldLoad()
        {
            HideDresserUI();
            HideHairstyleUI();
        }
    }
}