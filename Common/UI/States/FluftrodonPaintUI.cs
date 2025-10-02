using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using ReLogic.OS;
using System;
using System.Globalization;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.Initializers;
using Terraria.UI;
using MrPlagueRaces.Common.Systems;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.Races.Fluftrodon;
using MrPlagueRaces.Common.UI.Elements;
using MrPlagueRaces.Content.Projectiles;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.UI.States
{
    internal class FluftrodonPaintUI : UIState
    {
        public UIPanel baseElement;
        public UIImage previousOption;
        public UIImage currentOption;
        public UIImage nextOption;
        public UIImage previousOptionBackground;
        public UIImage currentOptionBackground;
        public UIImage nextOptionBackground;
        public UIImage currentOptionBorder;
        public Asset<Texture2D> imageTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_FluftrodonPaintColor");
        public Asset<Texture2D> backgroundTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_FluftrodonPaintBackground");
        public Asset<Texture2D> borderTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_FluftrodonPaintBorder");
        public UIText currentOptionDescription;

        public float uITransparency = 1f;

        float baseElementWidth = 150f;

        public override void OnInitialize()
        {
            baseElement = new UIPanel()
            {
                Width = StyleDimension.FromPixelsAndPercent(baseElementWidth, 0f),
                Height = StyleDimension.FromPixelsAndPercent(50f, 0f),
                Left = StyleDimension.FromPixelsAndPercent(Main.mouseX - (baseElementWidth / 2f) + 12f, 0f),
                Top = StyleDimension.FromPixelsAndPercent(Main.mouseY + 20, 0f),
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            Append(baseElement);
            previousOptionBackground = new UIImage(backgroundTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Color = Color.White * 0.25f * uITransparency
            };
            baseElement.Append(previousOptionBackground);
            currentOptionBackground = new UIImage(backgroundTexture)
            {
                HAlign = 0.5f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Color = Color.White * uITransparency
            };
            baseElement.Append(currentOptionBackground);
            nextOptionBackground = new UIImage(backgroundTexture)
            {
                HAlign = 1f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Color = Color.White * 0.25f * uITransparency
            };
            baseElement.Append(nextOptionBackground);
            currentOptionBorder = new UIImage(borderTexture)
            {
                HAlign = 0.5f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Color = Color.White * uITransparency
            };
            baseElement.Append(currentOptionBorder);
            previousOption = new UIImage(imageTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Color = Color.White * 0.25f * uITransparency
            };
            previousOptionBackground.Append(previousOption);
            currentOption = new UIImage(imageTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Color = Color.White * uITransparency
            };
            currentOptionBackground.Append(currentOption);
            nextOption = new UIImage(imageTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Color = Color.White * 0.25f * uITransparency
            };
            nextOptionBackground.Append(nextOption);
            currentOptionDescription = new UIText("Red", 1f)
            {
                HAlign = 0.5f,
                VAlign = 4f,
                Left = StyleDimension.FromPixelsAndPercent(2f, 0f),
            };
            baseElement.Append(currentOptionDescription);
            Append(baseElement);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            var fluftrodonPlayer = Main.LocalPlayer.GetModPlayer<FluftrodonPlayer>();
            Main.LocalPlayer.mouseInterface = true;
            baseElement.Remove();
            previousOptionBackground.Remove();
            currentOptionBackground.Remove();
            nextOptionBackground.Remove();
            previousOption.Remove();
            currentOption.Remove();
            nextOption.Remove();
            currentOptionDescription.Remove();

            baseElement = new UIPanel()
            {
                Width = StyleDimension.FromPixelsAndPercent(baseElementWidth, 0f),
                Height = StyleDimension.FromPixelsAndPercent(50f, 0f),
                Left = StyleDimension.FromPixelsAndPercent(Main.mouseX - (baseElementWidth / 2f) + 12f, 0f),
                Top = StyleDimension.FromPixelsAndPercent(Main.mouseY + 20, 0f),
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            Append(baseElement);
            previousOptionBackground = new UIImage(backgroundTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Color = Color.White * 0.25f * uITransparency
            };
            baseElement.Append(previousOptionBackground);
            currentOptionBackground = new UIImage(backgroundTexture)
            {
                HAlign = 0.5f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Color = Color.White * uITransparency
            };
            baseElement.Append(currentOptionBackground);
            nextOptionBackground = new UIImage(backgroundTexture)
            {
                HAlign = 1f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Color = Color.White * 0.25f * uITransparency
            };
            baseElement.Append(nextOptionBackground);
            previousOption = new UIImage(imageTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Color = new Color(fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint == 0 ? 30 : fluftrodonPlayer.selectedPaint - 1].ToVector4()) * 0.25f * uITransparency
            };
            previousOptionBackground.Append(previousOption);
            currentOption = new UIImage(imageTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Color = new Color(fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint].ToVector4()) * uITransparency
            };
            currentOptionBackground.Append(currentOption);
            currentOptionBorder = new UIImage(borderTexture)
            {
                HAlign = 0.5f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Color = Color.White * uITransparency
            };
            baseElement.Append(currentOptionBorder);
            nextOption = new UIImage(imageTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Color = new Color(fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint > 29 ? 0 : fluftrodonPlayer.selectedPaint + 1].ToVector4()) * 0.25f * uITransparency
            };
            nextOptionBackground.Append(nextOption);
            currentOptionDescription = new UIText(fluftrodonPlayer.paintName[fluftrodonPlayer.selectedPaint], 1f)
            {
                HAlign = 0.5f,
                VAlign = 4f,
                Left = StyleDimension.FromPixelsAndPercent(2f, 0f),
                TextColor = new Color(fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint].ToVector4()) * uITransparency
            };
            baseElement.Append(currentOptionDescription);
            Append(baseElement);
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            var fluftrodonPlayer = Main.LocalPlayer.GetModPlayer<FluftrodonPlayer>();
            base.Update(gameTime);
            if ((PlayerInput.MouseInfo.ScrollWheelValue - PlayerInput.MouseInfoOld.ScrollWheelValue) > 0)
            {
                fluftrodonPlayer.selectedPaint -= 1;
                if (fluftrodonPlayer.selectedPaint < 0)
                {
                    fluftrodonPlayer.selectedPaint = 30;
                }
            }
            if ((PlayerInput.MouseInfo.ScrollWheelValue - PlayerInput.MouseInfoOld.ScrollWheelValue) < 0)
            {
                fluftrodonPlayer.selectedPaint += 1;
                if (fluftrodonPlayer.selectedPaint > 30)
                {
                    fluftrodonPlayer.selectedPaint = 0;
                }
            }
            if (Main.mouseLeft)
            {
                WorldGen.paintTile(Player.tileTargetX, Player.tileTargetY, (byte)fluftrodonPlayer.selectedPaint, true);
                if (uITransparency > 0.25f)
                {
                    uITransparency -= 0.1f;
                }
            }
            if (Main.mouseRight)
            {
                WorldGen.paintWall(Player.tileTargetX, Player.tileTargetY, (byte)fluftrodonPlayer.selectedPaint, true);
                if (uITransparency > 0.25f)
                {
                    uITransparency -= 0.1f;
                }
            }
            if (!Main.mouseLeft && !Main.mouseRight)
            {
                if (uITransparency < 1f)
                {
                    uITransparency += 0.1f;
                }
            }
        }
    }
}