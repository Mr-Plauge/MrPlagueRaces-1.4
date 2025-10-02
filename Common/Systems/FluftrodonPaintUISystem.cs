using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.GameContent.UI.States;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.UI.States;

namespace MrPlagueRaces.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    public class FluftrodonPaintUISystem : ModSystem
    {
        internal FluftrodonPaintUI fluftrodonPaintUI;

        public void ShowMyUI()
        {
            IngameFancyUI.OpenUIState(fluftrodonPaintUI);
        }

        public void HideMyUI()
        {
            if (Main.InGameUI.CurrentState == fluftrodonPaintUI)
            {
                IngameFancyUI.Close();
            }
        }

        public void ToggleMyUI()
        {
            if (Main.InGameUI.CurrentState != fluftrodonPaintUI)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                ShowMyUI();
            }
            else
            {
                HideMyUI();
            }
        }

        public override void PostSetupContent()
        {
            fluftrodonPaintUI = new FluftrodonPaintUI();
            fluftrodonPaintUI.Activate();
        }
        public override void OnWorldLoad()
        {
            HideMyUI();
        }
    }
}