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
    public class LihzahrdGolemUISystem : ModSystem
    {
        internal LihzahrdGolemUI lihzahrdGolemUI;

        public void ShowMyUI()
        {
            IngameFancyUI.OpenUIState(lihzahrdGolemUI);
        }

        public void HideMyUI()
        {
            if (Main.InGameUI.CurrentState == lihzahrdGolemUI)
            {
                IngameFancyUI.Close();
            }
        }

        public void ToggleMyUI()
        {
            if (Main.InGameUI.CurrentState != lihzahrdGolemUI)
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
            lihzahrdGolemUI = new LihzahrdGolemUI();
            lihzahrdGolemUI.Activate();
        }
        public override void OnWorldLoad()
        {
            HideMyUI();
        }
    }
}