using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
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
    public class RaceChangeCameraModifier : ICameraModifier
    {
        private int framesToLast = 15;
        private int framesElapsed = 0;
        public static bool MovingOutward = false;

        public string UniqueIdentity { get; private set; }
        public bool Finished { get; private set; }

        public RaceChangeCameraModifier(int frames, string uniqueIdentity = null)
        {
            framesToLast = frames;
            UniqueIdentity = uniqueIdentity;
        }

        public void Update(ref CameraInfo cameraInfo)
        {
            float progress = Utils.GetLerpValue(0, framesToLast, framesElapsed);

            cameraInfo.CameraPosition.Y = MathHelper.SmoothStep(cameraInfo.CameraPosition.Y, cameraInfo.CameraPosition.Y + 300f + ((Main.screenHeight / Main.screenWidth) * 6f) + ((Main.screenWidth / Main.screenHeight) < 2f && Main.screenHeight > 1000 ? -120f : (Main.screenHeight < 775 ? -50f : 0f) + (Main.screenHeight / 8) - (Main.screenHeight / 4)), progress);

            if (!Main.gameInactive && !Main.gamePaused)
            {
                if (MovingOutward && framesElapsed <= framesToLast)
                {
                    framesElapsed++;
                }
                else if (framesElapsed > 0)
                {
                    framesElapsed--;
                }
            }

            if (framesElapsed == 0 && !MovingOutward)
            {
                Finished = true;
            }
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class RaceChangeUISystem : ModSystem
    {
        internal RaceChangeUI raceChangeUI;

        public void ShowMyUI()
        {
            IngameFancyUI.OpenUIState(raceChangeUI);
            raceChangeUI.StoreOriginalValues();
            raceChangeUI.SetDefaultRace();
            Main.instance.CameraModifiers.Add(new RaceChangeCameraModifier(30, "RaceChangeModifier"));
            RaceChangeCameraModifier.MovingOutward = true;
        }

        public void HideMyUI()
        {
            raceChangeUI.strandDelayTime = 15;
            RaceChangeCameraModifier.MovingOutward = false;
            if (Main.InGameUI.CurrentState == raceChangeUI)
            {
                IngameFancyUI.Close();
            }
        }

        public override void PostSetupContent()
        {
            raceChangeUI = new RaceChangeUI();
            raceChangeUI.Activate();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.InGameUI.CurrentState != raceChangeUI && RaceChangeCameraModifier.MovingOutward)
            {
                HideMyUI();
            }
        }
        public override void OnWorldLoad()
        {
            raceChangeUI.SetDefaultRace();
            HideMyUI();
        }
    }
}