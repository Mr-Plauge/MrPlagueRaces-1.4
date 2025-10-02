using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
using MrPlagueRaces.Common.Races.Lihzahrd;
using MrPlagueRaces.Common.UI.Elements;
using MrPlagueRaces.Content.Projectiles;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.UI.States
{
    internal class LihzahrdGolemUI : UIState
    {
        public UIPanel baseElement;
        public UIImage previousOption;
        public UIImage currentOption;
        public UIImage nextOption;
        public UIImage previousOptionBackground;
        public UIImage currentOptionBackground;
        public UIImage currentOptionBorder;
        public UIImage nextOptionBackground;
        public Asset<Texture2D> golemTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_BoulderGolem");
        public Asset<Texture2D> backgroundTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_LihzahrdGolemBackground");
        public Asset<Texture2D> borderTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_LihzahrdGolemBorder");
        public UIText currentOptionDescription;

        public float uITransparency = 1f;

        float baseElementWidth = 150f;
        bool newClick = false;

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
            currentOptionBorder = new UIImage(borderTexture)
            {
                HAlign = 0.5f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Color = Color.White * uITransparency
            };
            baseElement.Append(currentOptionBorder);
            nextOptionBackground = new UIImage(backgroundTexture)
            {
                HAlign = 1f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Color = Color.White * 0.25f * uITransparency
            };
            baseElement.Append(nextOptionBackground);
            previousOption = new UIImage(golemTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Color = Color.White * 0.25f * uITransparency
            };
            previousOptionBackground.Append(previousOption);
            currentOption = new UIImage(golemTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Color = Color.White * uITransparency
            };
            currentOptionBackground.Append(currentOption);
            nextOption = new UIImage(golemTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Color = Color.White * 0.25f * uITransparency
            };
            nextOptionBackground.Append(nextOption);
            currentOptionDescription = new UIText("Boulder Golem", 1f)
            {
                HAlign = 0.5f,
                VAlign = 4f,
                TextColor = Color.Orange * uITransparency
            };
            baseElement.Append(currentOptionDescription);
            Append(baseElement);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            var lihzahrdPlayer = Main.LocalPlayer.GetModPlayer<LihzahrdPlayer>();
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
            currentOptionBorder = new UIImage(borderTexture)
            {
                HAlign = 0.5f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 0.25f),
                Color = Color.White * uITransparency
            };
            baseElement.Append(currentOptionBorder);
            previousOption = new UIImage(lihzahrdPlayer.GolemIcons[lihzahrdPlayer.selectedGolemIndex > lihzahrdPlayer.Golems.Length - 1 ? 0 : lihzahrdPlayer.selectedGolemIndex == 0 ? lihzahrdPlayer.GolemIcons.Length - 1 : lihzahrdPlayer.selectedGolemIndex - 1])
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Color = Color.White * 0.25f * uITransparency
            };
            previousOptionBackground.Append(previousOption);
            currentOption = new UIImage(lihzahrdPlayer.GolemIcons[lihzahrdPlayer.selectedGolemIndex > lihzahrdPlayer.Golems.Length - 1 ? 0 : lihzahrdPlayer.selectedGolemIndex])
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Color = Color.White * uITransparency
            };
            currentOptionBackground.Append(currentOption);
            nextOption = new UIImage(lihzahrdPlayer.GolemIcons[lihzahrdPlayer.selectedGolemIndex > lihzahrdPlayer.Golems.Length - 1 ? 0 : lihzahrdPlayer.selectedGolemIndex == lihzahrdPlayer.GolemIcons.Length - 1 ? 0 : lihzahrdPlayer.selectedGolemIndex + 1])
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Color = Color.White * 0.25f * uITransparency
            };
            nextOptionBackground.Append(nextOption);
            currentOptionDescription = new UIText(lihzahrdPlayer.GolemNames[lihzahrdPlayer.selectedGolemIndex > lihzahrdPlayer.Golems.Length - 1 ? 0 : lihzahrdPlayer.selectedGolemIndex], 1f)
            {
                HAlign = 0.5f,
                VAlign = 4f,
                TextColor = Color.Orange * uITransparency
            };
            baseElement.Append(currentOptionDescription);
            Append(baseElement);
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            var mrPlagueRacesPlayer = Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            var lihzahrdPlayer = Main.LocalPlayer.GetModPlayer<LihzahrdPlayer>();
            base.Update(gameTime);

            if (lihzahrdPlayer.Golems.Length > 0)
            {
                if ((PlayerInput.MouseInfo.ScrollWheelValue - PlayerInput.MouseInfoOld.ScrollWheelValue) > 0)
                {
                    if (lihzahrdPlayer.selectedGolem == lihzahrdPlayer.Golems[0])
                    {
                        lihzahrdPlayer.selectedGolemIndex = lihzahrdPlayer.Golems.Length - 1;
                    }
                    else
                    {
                        lihzahrdPlayer.selectedGolemIndex--;
                    }
                }
                if ((PlayerInput.MouseInfo.ScrollWheelValue - PlayerInput.MouseInfoOld.ScrollWheelValue) < 0)
                {
                    if (lihzahrdPlayer.selectedGolem == lihzahrdPlayer.Golems[lihzahrdPlayer.Golems.Length - 1])
                    {
                        lihzahrdPlayer.selectedGolemIndex = 0;
                    }
                    else
                    {
                        lihzahrdPlayer.selectedGolemIndex++;
                    }
                }
                lihzahrdPlayer.selectedGolem = (lihzahrdPlayer.selectedGolemIndex > lihzahrdPlayer.Golems.Length - 1) ? lihzahrdPlayer.Golems[0] : lihzahrdPlayer.Golems[lihzahrdPlayer.selectedGolemIndex];
            }
            else
            {
                lihzahrdPlayer.selectedGolem = ProjectileType<BoulderGolem>();
                lihzahrdPlayer.selectedGolemIndex = 0;
            }

            if (Main.mouseLeft)
            {
                if (newClick)
                {
                    newClick = false;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.active && projectile.type == lihzahrdPlayer.selectedGolem && projectile.owner == Main.LocalPlayer.whoAmI)
                            projectile.Kill();
                    }
                    Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), mrPlagueRacesPlayer.mouseWorld.X - 22, mrPlagueRacesPlayer.mouseWorld.Y - 42, 0, 0, lihzahrdPlayer.selectedGolem, 0, 0, Main.LocalPlayer.whoAmI);
                    if (ModContent.GetInstance<LihzahrdConfig>().lihzahrdDisableUIAfterPlacement)
                    {
                        ModContent.GetInstance<LihzahrdGolemUISystem>().HideMyUI();
                    }
                }
                if (uITransparency > 0.25f)
                {
                    uITransparency -= 0.1f;
                }
            }
            if (Main.mouseLeftRelease)
            {
                newClick = true;
            }
            if (!Main.mouseLeft)
            {
                if (uITransparency < 1f)
                {
                    uITransparency += 0.1f;
                }
            }
        }
    }
}