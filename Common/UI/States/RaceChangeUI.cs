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
using MrPlagueRaces.Common.UI.Elements;
using MrPlagueRaces.Content.Projectiles;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.UI.States
{
    internal class RaceChangeUI : UIState
    {
        private enum CategoryId
        {
            HairStyle,
            HairColor,
            Eye,
            Skin,
            Detail,
            DetailAux1,
            DetailAux2,
            DetailAux3,
            Shirt,
            Undershirt,
            Pants,
            Shoes
        }

        private enum HSLSliderId
        {
            Hue,
            Saturation,
            Luminance
        }

        private static Vector3 RgbToScaledHsl(Color color)
        {
            Vector3 modPrefix = Main.rgbToHsl(color);
            modPrefix.Z = (modPrefix.Z - 0.15f) / 0.85f;
            modPrefix = Vector3.Clamp(modPrefix, Vector3.Zero, Vector3.One);
            return modPrefix;
        }

        private static Color ScaledHslToRgb(Vector3 hsl)
        {
            return ScaledHslToRgb(hsl.X, hsl.Y, hsl.Z);
        }

        private static Color ScaledHslToRgb(float hue, float saturation, float luminosity)
        {
            return Main.hslToRgb(hue, saturation, luminosity * 0.85f + 0.15f);
        }

        private static string GetHexText(Color pendingColor)
        {
            return "#" + pendingColor.Hex3().ToUpper();
        }

        private static Vector3 GetRandomColorVector()
        {
            return new Vector3(Main.rand.NextFloat(), Main.rand.NextFloat(), Main.rand.NextFloat());
        }

        private bool GetHexColor(string hexString, out Vector3 hsl)
        {
            if (hexString.StartsWith("#"))
            {
                hexString = hexString.Substring(1);
            }
            if (hexString.Length <= 6 && uint.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out uint result))
            {
                uint b = result & 0xFF;
                uint g = (result >> 8) & 0xFF;
                uint r = (result >> 16) & 0xFF;
                hsl = RgbToScaledHsl(new Color((int)r, (int)g, (int)b));
                return true;
            }
            hsl = Vector3.Zero;
            return false;
        }

        private Vector3 _currentColorHSL;
        public UIPanel RaceChangePanel;
        public UIPanel RaceChangeBaseElement;
        public Player _player;
        private UIRaceColoredImageButton[] _colorPickers;

        private Race _originalRace;
        private int _originalSkinVariant;
        private int _originalHair;
        private int _originalHairAux1;
        private int _originalHairAux2;
        private int _originalHairAux3;
        private Color _originalHairColor;
        private Color _originalSkinColor;
        private Color _originalDetailColor;
        private Color _originalAuxilaryDetailColor1;
        private Color _originalAuxilaryDetailColor2;
        private Color _originalAuxilaryDetailColor3;
        private Color _originalEyeColor;
        private Color _originalShirtColor;
        private Color _originalUnderShirtColor;
        private Color _originalPantsColor;
        private Color _originalShoeColor;
        private UITextPanel<LocalizedText> paletteNotifier;
        private int detailColorCount = 1;

        private UIColoredImageButton _copyHexButton;
        private UIColoredImageButton _pasteHexButton;
        private UIColoredImageButton _randomColorButton;

        private UITextPanel<string> statHoverText;
        public static string hoverText = "";

        private UIPanel ModifiablePanel;

        private CategoryId _selectedPicker;

        public int strandDelayTime = 15;

        public override void OnDeactivate()
        {
            ReinstateOriginalValues();
        }

        public override void OnInitialize()
        {
            int[] male = { 0, 2, 1, 3, 8 };
            Main.PendingPlayer = new Player();
            Item familiarShirt = new Item();
            familiarShirt.SetDefaults(ItemID.FamiliarShirt);
            Item familiarPants = new Item();
            familiarPants.SetDefaults(ItemID.FamiliarPants);
            _player = Main.PendingPlayer;
            _player.difficulty = 0;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            int randomRace = Main.rand.Next(RaceLoader.Races.Count);
            mrPlagueRacesPlayer.race = RaceLoader.Races[randomRace];
            if (mrPlagueRacesPlayer.race.StarterShirt)
            {
                _player.armor[11] = familiarShirt;
            }
            if (mrPlagueRacesPlayer.race.StarterPants)
            {
                _player.armor[12] = familiarPants;
            }

            RaceChangeBaseElement = new UIPanel()
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Width = StyleDimension.FromPixelsAndPercent(600f, 0f),
                Height = StyleDimension.FromPixelsAndPercent(300f, 0f),
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };

            RaceChangePanel = new UIPanel()
            {
                HAlign = 0.5f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0, 0.75f),
                BackgroundColor = new Color(33, 43, 79) * 0.8f
            };
            RaceChangeBaseElement.Append(RaceChangePanel);

            UIPanel PlayerColorPanel = new UIPanel()
            {
                HAlign = 0f,
                VAlign = 0.5f,
                Width = StyleDimension.FromPixelsAndPercent(0, 0.515f),
                Height = StyleDimension.FromPixelsAndPercent(0, 1f),
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            PlayerColorPanel.SetPadding(0f);
            RaceChangePanel.Append(PlayerColorPanel);

            ModifiablePanel = new UIPanel()
            {
                HAlign = 0f,
                VAlign = 0.94f,
                Width = StyleDimension.FromPixelsAndPercent(0, 0.515f + 0.01f),
                Height = StyleDimension.FromPixelsAndPercent(0, 0.6f + 0.01f),
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            ModifiablePanel.SetPadding(5f);
            RaceChangePanel.Append(ModifiablePanel);

            _colorPickers = new UIRaceColoredImageButton[8];

            PlayerColorPanel.Append(CreateColorPicker(CategoryId.HairStyle, "MrPlagueRaces/Assets/Textures/UI/Style_Hair", 0.05f, 0f));
            _colorPickers[0].SetMiddleTexture(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Style_Arrow", (AssetRequestMode)1));
            PlayerColorPanel.Append(CreateColorPicker(CategoryId.HairColor, "MrPlagueRaces/Assets/Textures/UI/ColorHair", 0.225f + 0.05f, 0f));
            PlayerColorPanel.Append(CreateColorPicker(CategoryId.Eye, "MrPlagueRaces/Assets/Textures/UI/ColorEye", 0.45f + 0.05f, 0f));
            _colorPickers[2].SetMiddleTexture(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/ColorEyeBack", (AssetRequestMode)1));
            PlayerColorPanel.Append(CreateColorPicker(CategoryId.Skin, "MrPlagueRaces/Assets/Textures/UI/ColorSkin", 0.675f + 0.05f, 0f));
            PlayerColorPanel.Append(CreateColorPicker(CategoryId.Detail, "MrPlagueRaces/Assets/Textures/UI/ColorDetailQuarter", 0.955f - 0.084f, 0f, true));
            PlayerColorPanel.Append(CreateColorPicker(CategoryId.DetailAux1, "MrPlagueRaces/Assets/Textures/UI/ColorDetailQuarterOpposite", 0.955f, 0f, true, true));
            PlayerColorPanel.Append(CreateColorPicker(CategoryId.DetailAux2, "MrPlagueRaces/Assets/Textures/UI/ColorDetailQuarterBottom", 0.955f - 0.084f, 0.135f, true, false, true));
            PlayerColorPanel.Append(CreateColorPicker(CategoryId.DetailAux3, "MrPlagueRaces/Assets/Textures/UI/ColorDetailQuarterBottomOpposite", 0.955f, 0.135f, true, true, true));

            UIHorizontalSeparator element = new UIHorizontalSeparator
            {
                HAlign = 0.5f,
                VAlign = 0.325f,
                Width = StyleDimension.FromPixelsAndPercent(255f, 0f),
                Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
            };
            PlayerColorPanel.Append(element);

            EnableColorPicker();

            UIPanel RaceSelectPanel = new UIPanel()
            {
                HAlign = 1f,
                VAlign = 0.5f,
                Width = StyleDimension.FromPixelsAndPercent(0, 0.485f),
                Height = StyleDimension.FromPixelsAndPercent(0, 1f),
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent,
            };
            RaceSelectPanel.SetPadding(0f);
            RaceChangePanel.Append(RaceSelectPanel);

            UIList raceSelectList = new UIList
            {
                Width = StyleDimension.FromPixelsAndPercent(-18f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(-6f, 1f)
            };
            RaceSelectPanel.Append(raceSelectList);
            UIScrollbar raceSelectScrollbar = new UIScrollbar
            {
                HAlign = 1f,
                Height = StyleDimension.FromPixelsAndPercent(-30f, 1f),
                Top = StyleDimension.FromPixels(10f)
            };
            raceSelectScrollbar.SetView(100f, 1000f);
            raceSelectList.SetScrollbar(raceSelectScrollbar);
            RaceSelectPanel.Append(raceSelectScrollbar);
            int raceCount = RaceLoader.Races.Count;
            UIElement selectContainer = new UIElement
            {
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent((82 * (raceCount / 5 + ((raceCount % 5 != 0) ? 1 : 0))) + (8 + 52 - 8), 0f)
            };
            raceSelectList.Add(selectContainer);
            selectContainer.SetPadding(0f);

            for (int raceId = 0; raceId < raceCount; raceId++)
            {
                UIRaceButton uIRaceButton = new UIRaceButton(_player, raceId, true)
                {
                    Left = StyleDimension.FromPixels((float)(raceId % 5) * 48f),
                    Top = StyleDimension.FromPixels((float)(raceId / 5) * 84f)
                };
                uIRaceButton.OnLeftMouseDown += Click_SelectRace;
                selectContainer.Append(uIRaceButton);
            }

            paletteNotifier = new UITextPanel<LocalizedText>(Language.GetText("Mods.MrPlagueRaces.UI.ColorPaletteNotifier"), 1f);
            paletteNotifier.Width.Set(-10f, 0.5f);
            paletteNotifier.Height.Set(50f, 0f);
            paletteNotifier.HAlign = 0.5f;
            paletteNotifier.VAlign = -0.15f;
            paletteNotifier.BackgroundColor = Color.Transparent;
            paletteNotifier.BorderColor = Color.Transparent;

            UITextPanel<LocalizedText> cancelButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Cancel"), 0.7f, large: true);
            cancelButton.Width.Set(-10f, 0.5f);
            cancelButton.Height.Set(50f, 0f);
            cancelButton.HAlign = 1f;
            cancelButton.VAlign = 1f;
            cancelButton.OnMouseOver += FadedMouseOver;
            cancelButton.OnMouseOut += FadedMouseOut;
            cancelButton.OnLeftClick += CloseButtonClicked;
            RaceChangeBaseElement.Append(cancelButton);

            UITextPanel<LocalizedText> confirmButton = new UITextPanel<LocalizedText>(Language.GetText("Mods.MrPlagueRaces.UI.Confirm"), 0.7f, large: true);
            confirmButton.Width.Set(-10f, 0.5f);
            confirmButton.Height.Set(50f, 0f);
            confirmButton.HAlign = 0f;
            confirmButton.VAlign = 1f;
            confirmButton.OnMouseOver += FadedMouseOver;
            confirmButton.OnMouseOut += FadedMouseOut;
            confirmButton.OnLeftClick += ConfirmButtonClicked;
            RaceChangeBaseElement.Append(confirmButton);

            Append(RaceChangeBaseElement);

            statHoverText = new UITextPanel<string>(hoverText)
            {
                Width = StyleDimension.FromPixelsAndPercent(1f, 0f),
                Height = StyleDimension.FromPixelsAndPercent(1f, 0f),
                Left = StyleDimension.FromPixelsAndPercent(50f, 0f),
                Top = StyleDimension.FromPixelsAndPercent(50f, 0f),
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            Append(statHoverText);
        }

        public override void Update(GameTime gameTime)
        {
            var mrPlagueRacesPlayer = Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            base.Update(gameTime);
            Main.LocalPlayer.cursorItemIconID = 0;
            if (strandDelayTime > 0)
            {
                strandDelayTime--;
            }
            if (strandDelayTime == 0)
            {
                mrPlagueRacesPlayer.StrandAnimation(0);
            }
            if (PlayerInput.Triggers.Current.Down)
            {
                paletteNotifier.Remove();
                RaceChangeBaseElement.Append(paletteNotifier);
            }
            else
            {
                paletteNotifier.Remove();
            }
        }

        public void EnableColorPicker()
        {
            ModifiablePanel.RemoveAllChildren();

            UIElement playerColorSlider = new UIPanel
            {
                Width = StyleDimension.FromPixelsAndPercent(220f, 0f),
                Height = StyleDimension.FromPixelsAndPercent(104f, 0f),
                HAlign = 0.8f,
                VAlign = 0.94f
            };
            playerColorSlider.SetPadding(0f);
            playerColorSlider.PaddingTop = 3f;
            ModifiablePanel.Append(playerColorSlider);
            playerColorSlider.Append(CreateHSLSlider(HSLSliderId.Hue));
            playerColorSlider.Append(CreateHSLSlider(HSLSliderId.Saturation));
            playerColorSlider.Append(CreateHSLSlider(HSLSliderId.Luminance));

            _copyHexButton = new UIColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Copy", (AssetRequestMode)1), isSmall: true)
            {
                VAlign = 0.99f - 0.5f - 0.5f,
                HAlign = 0.03f,
            };
            _copyHexButton.OnLeftMouseDown += Click_CopyHex;
            ModifiablePanel.Append(_copyHexButton);

            _pasteHexButton = new UIColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Paste", (AssetRequestMode)1), isSmall: true)
            {
                VAlign = 0.99f - 0.5f,
                HAlign = 0.03f
            };
            _pasteHexButton.OnLeftMouseDown += Click_PasteHex;
            ModifiablePanel.Append(_pasteHexButton);

            _randomColorButton = new UIColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Randomize", (AssetRequestMode)1), isSmall: true)
            {
                VAlign = 0.99f,
                HAlign = 0.03f
            };
            _randomColorButton.OnLeftMouseDown += Click_RandomizeSingleColor;

            ModifiablePanel.Append(_randomColorButton);
        }

        public void EnableHairStyles()
        {
            var mrPlagueRacesPlayer = Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            int rowSize = 5;
            ModifiablePanel.RemoveAllChildren();
            UIList hairStyleList = new UIList
            {
                Width = StyleDimension.FromPixelsAndPercent(-18f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(-6f, 1f)
            };
            hairStyleList.SetPadding(0f);
            ModifiablePanel.Append(hairStyleList);
            UIScrollbar hairStyleScrollbar = new UIScrollbar
            {
                HAlign = 0.955f,
                Height = StyleDimension.FromPixelsAndPercent(-30f, 1f),
                Top = StyleDimension.FromPixels(10f)
            };
            hairStyleScrollbar.SetView(100f, 1000f);
            hairStyleList.SetScrollbar(hairStyleScrollbar);
            ModifiablePanel.Append(hairStyleScrollbar);

            int hairCount = mrPlagueRacesPlayer.GetRaceHairCount(Main.LocalPlayer, 0);
            int hairCountAux1 = mrPlagueRacesPlayer.GetRaceHairCount(Main.LocalPlayer, 1);
            int hairCountAux2 = mrPlagueRacesPlayer.GetRaceHairCount(Main.LocalPlayer, 2);
            int hairCountAux3 = mrPlagueRacesPlayer.GetRaceHairCount(Main.LocalPlayer, 3);
            int[] hairCategories = { hairCount, hairCountAux1, hairCountAux2, hairCountAux3 };
            int totalHair = (int)(Math.Ceiling((double)hairCount / rowSize) * rowSize) + (int)(Math.Ceiling((double)hairCountAux1 / rowSize) * rowSize) + (int)(Math.Ceiling((double)hairCountAux2 / rowSize) * rowSize) + (int)(Math.Ceiling((double)hairCountAux3 / rowSize) * rowSize);
            int activeHairAuxilaries = (Math.Clamp(hairCountAux1, 0, 1)) + (Math.Clamp(hairCountAux2, 0, 1)) + (Math.Clamp(hairCountAux3, 0, 1));
            int currentHairAuxilary = 0;
            UIElement hairPanel = new UIElement
            {
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(48 * (totalHair / rowSize + ((totalHair % rowSize != 0) ? 1 : 0)) + (24f * activeHairAuxilaries), 0f)
            };
            hairStyleList.Add(hairPanel);
            hairPanel.SetPadding(0f);
            int totalCounter = 0;
            for (int hairCategory = 0; hairCategory < 1 + activeHairAuxilaries; hairCategory++)
            {
                for (int currentCounter = 0; currentCounter < hairCategories[hairCategory]; currentCounter++)
                {
                    UIMultiHairStyleButton hairButton = new UIMultiHairStyleButton(Main.LocalPlayer, currentCounter, hairCategory)
                    {
                        Left = StyleDimension.FromPixels((float)(totalCounter % rowSize) * 48f + 6f),
                        Top = StyleDimension.FromPixels((float)(totalCounter / rowSize) * 48f + 1f + (24f * currentHairAuxilary))
                    };
                    hairButton.SetSnapPoint("Middle", totalCounter);
                    hairPanel.Append(hairButton);
                    totalCounter++;
                }
                totalCounter += (int)(Math.Ceiling((double)totalCounter / rowSize) * rowSize) - totalCounter;
                currentHairAuxilary++;
                if (currentHairAuxilary <= activeHairAuxilaries)
                {
                    UIHorizontalSeparator element = new UIHorizontalSeparator
                    {
                        Left = StyleDimension.FromPixelsAndPercent(0f, 0.037f),
                        Top = StyleDimension.FromPixels((float)(totalCounter / rowSize) * 48f + 1f + (24f * currentHairAuxilary) - 12f),
                        Width = StyleDimension.FromPixelsAndPercent(0f, 0.87f),
                        Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
                    };
                    hairPanel.Append(element);
                }
            }
        }

        private UIColoredSlider CreateHSLSlider(HSLSliderId id)
        {
            UIColoredSlider uIColoredSlider = CreateHSLSliderButtonBase(id);
            uIColoredSlider.VAlign = 0f;
            uIColoredSlider.HAlign = 0f;
            uIColoredSlider.Width = StyleDimension.FromPixelsAndPercent(-10f, 1f);
            uIColoredSlider.Top.Set(30 * (int)id, 0f);
            return uIColoredSlider;
        }

        private UIColoredSlider CreateHSLSliderButtonBase(HSLSliderId id)
        {
            switch (id)
            {
                case HSLSliderId.Saturation:
                    return new UIColoredSlider(LocalizedText.Empty, () => GetHSLSliderPosition(HSLSliderId.Saturation), delegate (float x)
                    {
                        UpdateHSLValue(HSLSliderId.Saturation, x);
                    }, UpdateHSL_S, (float x) => GetHSLSliderColorAt(HSLSliderId.Saturation, x), Color.Transparent);
                case HSLSliderId.Luminance:
                    return new UIColoredSlider(LocalizedText.Empty, () => GetHSLSliderPosition(HSLSliderId.Luminance), delegate (float x)
                    {
                        UpdateHSLValue(HSLSliderId.Luminance, x);
                    }, UpdateHSL_L, (float x) => GetHSLSliderColorAt(HSLSliderId.Luminance, x), Color.Transparent);
                default:
                    return new UIColoredSlider(LocalizedText.Empty, () => GetHSLSliderPosition(HSLSliderId.Hue), delegate (float x)
                    {
                        UpdateHSLValue(HSLSliderId.Hue, x);
                    }, UpdateHSL_H, (float x) => GetHSLSliderColorAt(HSLSliderId.Hue, x), Color.Transparent);
            }
        }

        private void UpdateHSL_H()
        {
            float value = UILinksInitializer.HandleSliderHorizontalInput(_currentColorHSL.X, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
            UpdateHSLValue(HSLSliderId.Hue, value);
        }

        private void UpdateHSL_S()
        {
            float value = UILinksInitializer.HandleSliderHorizontalInput(_currentColorHSL.Y, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
            UpdateHSLValue(HSLSliderId.Saturation, value);
        }

        private void UpdateHSL_L()
        {
            float value = UILinksInitializer.HandleSliderHorizontalInput(_currentColorHSL.Z, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
            UpdateHSLValue(HSLSliderId.Luminance, value);
        }

        private float GetHSLSliderPosition(HSLSliderId id)
        {
            switch (id)
            {
                case HSLSliderId.Hue:
                    return _currentColorHSL.X;
                case HSLSliderId.Saturation:
                    return _currentColorHSL.Y;
                case HSLSliderId.Luminance:
                    return _currentColorHSL.Z;
                default:
                    return 1f;
            }
        }

        private void UpdateHSLValue(HSLSliderId id, float value)
        {
            switch (id)
            {
                case HSLSliderId.Hue:
                    _currentColorHSL.X = value;
                    break;
                case HSLSliderId.Saturation:
                    _currentColorHSL.Y = value;
                    break;
                case HSLSliderId.Luminance:
                    _currentColorHSL.Z = value;
                    break;
            }
            Color color = ScaledHslToRgb(_currentColorHSL.X, _currentColorHSL.Y, _currentColorHSL.Z);
            ApplyPendingColor(color);
            _colorPickers[(int)_selectedPicker]?.SetColor(color);
            if ((int)_selectedPicker == 4)
            {
                switch (detailColorCount)
                {
                    case 1:
                        _colorPickers[5]?.SetColor(color);
                        _colorPickers[6]?.SetColor(color);
                        _colorPickers[7]?.SetColor(color);
                        break;
                    case 2:
                        _colorPickers[6]?.SetColor(color);
                        break;
                }
            }
            else if ((int)_selectedPicker == 5)
            {
                switch (detailColorCount)
                {
                    case 2:
                        _colorPickers[7]?.SetColor(color);
                        break;
                }
            }
            else if ((int)_selectedPicker == 6)
            {
                switch (detailColorCount)
                {
                    case 3:
                        _colorPickers[7]?.SetColor(color);
                        break;
                }
            }
            if ((int)_selectedPicker == 1)
            {
                _colorPickers[(int)0].SetColor(color);
            }
        }

        private void ApplyPendingColor(Color pendingColor)
        {
            var mrPlagueRacesPlayer = Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            switch (_selectedPicker)
            {
                case CategoryId.HairStyle:
                    Main.LocalPlayer.hairColor = pendingColor;
                    break;
                case CategoryId.HairColor:
                    Main.LocalPlayer.hairColor = pendingColor;
                    break;
                case CategoryId.Eye:
                    Main.LocalPlayer.eyeColor = pendingColor;
                    break;
                case CategoryId.Skin:
                    Main.LocalPlayer.skinColor = pendingColor;
                    break;
                case CategoryId.Detail:
                    mrPlagueRacesPlayer.detailColor = pendingColor;
                    break;
                case CategoryId.DetailAux1:
                    switch (detailColorCount)
                    {
                        case 1:
                            mrPlagueRacesPlayer.detailColor = pendingColor;
                            break;
                        case 2:
                            mrPlagueRacesPlayer.auxilaryDetailColor1 = pendingColor;
                            break;
                        case 3:
                            mrPlagueRacesPlayer.auxilaryDetailColor1 = pendingColor;
                            break;
                        case 4:
                            mrPlagueRacesPlayer.auxilaryDetailColor1 = pendingColor;
                            break;
                    }
                    break;
                case CategoryId.DetailAux2:
                    switch (detailColorCount)
                    {
                        case 1:
                            mrPlagueRacesPlayer.detailColor = pendingColor;
                            break;
                        case 2:
                            mrPlagueRacesPlayer.detailColor = pendingColor;
                            break;
                        case 3:
                            mrPlagueRacesPlayer.auxilaryDetailColor2 = pendingColor;
                            break;
                        case 4:
                            mrPlagueRacesPlayer.auxilaryDetailColor2 = pendingColor;
                            break;
                    }
                    break;
                case CategoryId.DetailAux3:
                    switch (detailColorCount)
                    {
                        case 1:
                            mrPlagueRacesPlayer.detailColor = pendingColor;
                            break;
                        case 2:
                            mrPlagueRacesPlayer.auxilaryDetailColor1 = pendingColor;
                            break;
                        case 3:
                            mrPlagueRacesPlayer.auxilaryDetailColor2 = pendingColor;
                            break;
                        case 4:
                            mrPlagueRacesPlayer.auxilaryDetailColor3 = pendingColor;
                            break;
                    }
                    break;
                case CategoryId.Shirt:
                    Main.LocalPlayer.shirtColor = pendingColor;
                    break;
                case CategoryId.Undershirt:
                    Main.LocalPlayer.underShirtColor = pendingColor;
                    break;
                case CategoryId.Pants:
                    Main.LocalPlayer.pantsColor = pendingColor;
                    break;
                case CategoryId.Shoes:
                    Main.LocalPlayer.shoeColor = pendingColor;
                    break;
            }
            if (_player.whoAmI == Main.myPlayer)
            {
                mrPlagueRacesPlayer.SyncPlayerAppearance(-1, Main.myPlayer);
            }
        }

        private Color GetHSLSliderColorAt(HSLSliderId id, float pointAt)
        {
            switch (id)
            {
                case HSLSliderId.Hue:
                    return ScaledHslToRgb(pointAt, 1f, 0.5f);
                case HSLSliderId.Saturation:
                    return ScaledHslToRgb(_currentColorHSL.X, pointAt, _currentColorHSL.Z);
                case HSLSliderId.Luminance:
                    return ScaledHslToRgb(_currentColorHSL.X, _currentColorHSL.Y, pointAt);
                default:
                    return Color.White;
            }
        }

        private UIRaceColoredImageButton CreateColorPicker(CategoryId id, string texturePath, float hAlign = 0f, float vAlign = 0f, bool shouldBeSmall = false, bool xInvert = false, bool yInvert = false)
        {
            UIRaceColoredImageButton uIColoredImageButton = new UIRaceColoredImageButton(ModContent.Request<Texture2D>(texturePath, (AssetRequestMode)1), shouldBeSmall, xInvert, yInvert);
            _colorPickers[(int)id] = uIColoredImageButton;
            uIColoredImageButton.VAlign = vAlign;
            uIColoredImageButton.HAlign = hAlign;
            uIColoredImageButton.OnLeftMouseDown += Click_ColorPicker;
            return uIColoredImageButton;
        }

        private void Click_ColorPicker(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            int previousSelectedPicker = (int)_selectedPicker;
            int text = 0;
            while (true)
            {
                if (text < _colorPickers.Length)
                {
                    if (_colorPickers[text] == evt.Target)
                    {
                        break;
                    }
                    text++;
                    continue;
                }
                return;
            }
            if (text == 0)
            {
                SelectPlayerHair((CategoryId)text);
            }
            else
            {
                if (text == 5)
                {
                    switch (detailColorCount)
                    {
                        case 1:
                            SelectColorPicker((CategoryId)4);
                            break;
                        default:
                            SelectColorPicker((CategoryId)text);
                            break;
                    }
                }
                else if (text == 6)
                {
                    switch (detailColorCount)
                    {
                        case 1:
                            SelectColorPicker((CategoryId)4);
                            break;
                        case 2:
                            SelectColorPicker((CategoryId)4);
                            break;
                        default:
                            SelectColorPicker((CategoryId)text);
                            break;
                    }
                }
                else if (text == 7)
                {
                    switch (detailColorCount)
                    {
                        case 1:
                            SelectColorPicker((CategoryId)4);
                            break;
                        case 2:
                            SelectColorPicker((CategoryId)5);
                            break;
                        case 3:
                            SelectColorPicker((CategoryId)6);
                            break;
                        default:
                            SelectColorPicker((CategoryId)text);
                            break;
                    }
                }
                else
                {
                    SelectColorPicker((CategoryId)text);
                }
            }
            ResetColorPickers();
            _colorPickers[(int)text].SetSelected(true, true);
        }

        private void SelectPlayerHair(CategoryId selection)
        {
            EnableHairStyles();
            _selectedPicker = selection;
            var mrPlagueRacesPlayer = Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        private void Click_SelectRace(UIMouseEvent evt, UIElement listeningElement)
        {
            if ((int)_selectedPicker == 4 || (int)_selectedPicker == 5 || (int)_selectedPicker == 6 || (int)_selectedPicker == 7)
            {
                ResetColorPickers();
                _colorPickers[4].SetSelected(true);
                SelectColorPicker((CategoryId)4);
            }
            UpdateColorPickers();
        }

        private void Click_CopyHex(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            Platform.Get<IClipboard>().Value = GetHexText(ScaledHslToRgb(_currentColorHSL));
        }

        private void Click_PasteHex(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            string value = Platform.Get<IClipboard>().Value;
            if (GetHexColor(value, out Vector3 hsl))
            {
                ApplyPendingColor(ScaledHslToRgb(hsl.X, hsl.Y, hsl.Z));
                _currentColorHSL = hsl;
                UpdateColorPickers();
            }
        }

        private void Click_RandomizeSingleColor(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            Vector3 parts = GetRandomColorVector();
            ApplyPendingColor(ScaledHslToRgb(parts.X, parts.Y, parts.Z));
            _currentColorHSL = parts;
            UpdateColorPickers();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            int LinesCount(string s)
            {
                int count = 0;
                int position = 0;
                while ((position = s.IndexOf('\n', position)) != -1)
                {
                    count++;
                    position++;
                }
                return count;
            }
            statHoverText.Remove();
            statHoverText = new UITextPanel<string>(hoverText)
            {
                Width = StyleDimension.FromPixelsAndPercent(1f, 0f),
                Height = StyleDimension.FromPixelsAndPercent(LinesCount(hoverText) == 1 ? 67.5f : (50f * LinesCount(hoverText)) - (22f * (LinesCount(hoverText) > 2 ? LinesCount(hoverText) - 2 : 0)), 0f),
                Left = StyleDimension.FromPixelsAndPercent(Main.mouseX + 10f, 0f),
                Top = StyleDimension.FromPixelsAndPercent(Main.mouseY + 12f, 0f),
                BackgroundColor = new Color(33, 43, 79) * 0.95f
            };
            if (hoverText == "")
            {
                statHoverText.BorderColor = Color.Transparent;
                statHoverText.BackgroundColor = Color.Transparent;
            }
            Append(statHoverText);

            Main.LocalPlayer.mouseInterface = true;
            _player.Male = Main.LocalPlayer.Male;
            if (detailColorCount == 1)
            {
                _colorPickers[4].SetSelected(_colorPickers[4].IsSelected() || _colorPickers[5].IsSelected() || _colorPickers[6].IsSelected() || _colorPickers[7].IsSelected(), _colorPickers[4].IsHovered() || _colorPickers[5].IsHovered() || _colorPickers[6].IsHovered() || _colorPickers[7].IsHovered());
                _colorPickers[5].SetSelected(_colorPickers[4].IsSelected() || _colorPickers[5].IsSelected() || _colorPickers[6].IsSelected() || _colorPickers[7].IsSelected(), _colorPickers[4].IsHovered() || _colorPickers[5].IsHovered() || _colorPickers[6].IsHovered() || _colorPickers[7].IsHovered());
                _colorPickers[6].SetSelected(_colorPickers[4].IsSelected() || _colorPickers[5].IsSelected() || _colorPickers[6].IsSelected() || _colorPickers[7].IsSelected(), _colorPickers[4].IsHovered() || _colorPickers[5].IsHovered() || _colorPickers[6].IsHovered() || _colorPickers[7].IsHovered());
                _colorPickers[7].SetSelected(_colorPickers[4].IsSelected() || _colorPickers[5].IsSelected() || _colorPickers[6].IsSelected() || _colorPickers[7].IsSelected(), _colorPickers[4].IsHovered() || _colorPickers[5].IsHovered() || _colorPickers[6].IsHovered() || _colorPickers[7].IsHovered());

            }
            else if (detailColorCount == 2)
            {
                _colorPickers[4].SetSelected(_colorPickers[4].IsSelected() || _colorPickers[6].IsSelected(), _colorPickers[4].IsHovered() || _colorPickers[6].IsHovered());
                _colorPickers[6].SetSelected(_colorPickers[4].IsSelected() || _colorPickers[6].IsSelected(), _colorPickers[4].IsHovered() || _colorPickers[6].IsHovered());
                _colorPickers[5].SetSelected(_colorPickers[5].IsSelected() || _colorPickers[7].IsSelected(), _colorPickers[5].IsHovered() || _colorPickers[7].IsHovered());
                _colorPickers[7].SetSelected(_colorPickers[5].IsSelected() || _colorPickers[7].IsSelected(), _colorPickers[5].IsHovered() || _colorPickers[7].IsHovered());

            }
            else if (detailColorCount == 3)
            {
                _colorPickers[4].SetSelected(_colorPickers[4].IsSelected(), _colorPickers[4].IsHovered());
                _colorPickers[5].SetSelected(_colorPickers[5].IsSelected(), _colorPickers[5].IsHovered());
                _colorPickers[6].SetSelected(_colorPickers[6].IsSelected() || _colorPickers[7].IsSelected(), _colorPickers[6].IsHovered() || _colorPickers[7].IsHovered());
                _colorPickers[7].SetSelected(_colorPickers[6].IsSelected() || _colorPickers[7].IsSelected(), _colorPickers[6].IsHovered() || _colorPickers[7].IsHovered());

            }
            else
            {
                _colorPickers[4].SetSelected(_colorPickers[4].IsSelected(), _colorPickers[4].IsHovered());
                _colorPickers[5].SetSelected(_colorPickers[5].IsSelected(), _colorPickers[5].IsHovered());
                _colorPickers[6].SetSelected(_colorPickers[6].IsSelected(), _colorPickers[6].IsHovered());
                _colorPickers[7].SetSelected(_colorPickers[7].IsSelected(), _colorPickers[7].IsHovered());
            }
            base.DrawSelf(spriteBatch);
            if (_copyHexButton.IsMouseHovering)
            {
                Main.instance.MouseText(Language.GetTextValue("UI.CopyColorToClipboard"));
            }
            if (_pasteHexButton.IsMouseHovering)
            {
                Main.instance.MouseText(Language.GetTextValue("UI.PasteColorFromClipboard"));
            }
            if (_randomColorButton.IsMouseHovering)
            {
                Main.instance.MouseText(Language.GetTextValue("UI.RandomizeColor"));
            }
        }


        private void SelectColorPicker(CategoryId selection)
        {
            EnableColorPicker();
            var mrPlagueRacesPlayer = Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            _selectedPicker = selection;
            Vector3 currentColorHSL = Vector3.One;
            switch (_selectedPicker)
            {
                case CategoryId.HairStyle:
                    currentColorHSL = RgbToScaledHsl(Main.LocalPlayer.hairColor);
                    break;
                case CategoryId.HairColor:
                    currentColorHSL = RgbToScaledHsl(Main.LocalPlayer.hairColor);
                    break;
                case CategoryId.Eye:
                    currentColorHSL = RgbToScaledHsl(Main.LocalPlayer.eyeColor);
                    break;
                case CategoryId.Skin:
                    currentColorHSL = RgbToScaledHsl(Main.LocalPlayer.skinColor);
                    break;
                case CategoryId.Detail:
                    currentColorHSL = RgbToScaledHsl(mrPlagueRacesPlayer.detailColor);
                    break;
                case CategoryId.DetailAux1:
                    switch (detailColorCount)
                    {
                        case 1:
                            _colorPickers[5].SetColor(mrPlagueRacesPlayer.detailColor);
                            _colorPickers[6].SetColor(mrPlagueRacesPlayer.detailColor);
                            _colorPickers[7].SetColor(mrPlagueRacesPlayer.detailColor);
                            break;
                        case 2:
                            _colorPickers[5].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                            _colorPickers[6].SetColor(mrPlagueRacesPlayer.detailColor);
                            _colorPickers[7].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                            break;
                        case 3:
                            _colorPickers[5].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                            _colorPickers[6].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor2);
                            _colorPickers[7].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor2);
                            break;
                        case 4:
                            _colorPickers[5].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                            _colorPickers[6].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor2);
                            _colorPickers[7].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor3);
                            break;
                    }
                    currentColorHSL = RgbToScaledHsl(mrPlagueRacesPlayer.auxilaryDetailColor1);
                    break;
                case CategoryId.DetailAux2:
                    currentColorHSL = RgbToScaledHsl(mrPlagueRacesPlayer.auxilaryDetailColor2);
                    break;
                case CategoryId.DetailAux3:
                    currentColorHSL = RgbToScaledHsl(mrPlagueRacesPlayer.auxilaryDetailColor3);
                    break;
                case CategoryId.Shirt:
                    currentColorHSL = RgbToScaledHsl(Main.LocalPlayer.shirtColor);
                    break;
                case CategoryId.Undershirt:
                    currentColorHSL = RgbToScaledHsl(Main.LocalPlayer.underShirtColor);
                    break;
                case CategoryId.Pants:
                    currentColorHSL = RgbToScaledHsl(Main.LocalPlayer.pantsColor);
                    break;
                case CategoryId.Shoes:
                    currentColorHSL = RgbToScaledHsl(Main.LocalPlayer.shoeColor);
                    break;
            }
            _currentColorHSL = currentColorHSL;
        }

        private void UpdateColorPickers()
        {
            var mrPlagueRacesPlayer = Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            detailColorCount = mrPlagueRacesPlayer.GetDetailColorCount(_player);
            _colorPickers[0].SetColor(Main.LocalPlayer.hairColor);
            _colorPickers[1].SetColor(Main.LocalPlayer.hairColor);
            _colorPickers[2].SetColor(Main.LocalPlayer.eyeColor);
            _colorPickers[3].SetColor(Main.LocalPlayer.skinColor);
            _colorPickers[4].SetColor(mrPlagueRacesPlayer.detailColor);
            switch (detailColorCount)
            {
                case 1:
                    _colorPickers[5].SetColor(mrPlagueRacesPlayer.detailColor);
                    _colorPickers[6].SetColor(mrPlagueRacesPlayer.detailColor);
                    _colorPickers[7].SetColor(mrPlagueRacesPlayer.detailColor);
                    break;
                case 2:
                    _colorPickers[5].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                    _colorPickers[6].SetColor(mrPlagueRacesPlayer.detailColor);
                    _colorPickers[7].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                    break;
                case 3:
                    _colorPickers[5].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                    _colorPickers[6].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor2);
                    _colorPickers[7].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor2);
                    break;
                case 4:
                    _colorPickers[5].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                    _colorPickers[6].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor2);
                    _colorPickers[7].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor3);
                    break;
            }
            if (_selectedPicker != 0)
            {
                SelectColorPicker(_selectedPicker);
            }
            else
            {
                SelectPlayerHair(_selectedPicker);
            }
        }

        private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            ((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
            ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.8f;
            ((UIPanel)evt.Target).BorderColor = Color.Black;
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            var mrPlagueRacesPlayer = Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            SoundEngine.PlaySound(SoundID.MenuClose);
            ModContent.GetInstance<RaceChangeUISystem>().HideMyUI();
            UpdateColorPickers();
        }

        private void ConfirmButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            var mrPlagueRacesPlayer = Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            //SoundEngine.PlaySound(SoundID.MenuClose);
            StoreOriginalValues();
            ModContent.GetInstance<RaceChangeUISystem>().HideMyUI();
            mrPlagueRacesPlayer.reverseStrandAnimationTime = 0;
            SoundEngine.PlaySound(SoundID.Item176);
            SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact);
            if (Main.LocalPlayer.whoAmI == Main.myPlayer)
            {
                mrPlagueRacesPlayer.EntangledStrandsConfirmSound(-1, Main.myPlayer);
            }
            UpdateColorPickers();
        }

        private void ResetColorPickers()
        {
            for (int i = 0; i < 8; i++)
            {
                _colorPickers[i].SetSelected(selected: false);
            }
        }

        public void SetDefaultRace()
        {
            var mrPlagueRacesPlayer = Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            _player.GetModPlayer<MrPlagueRacesPlayer>().race = mrPlagueRacesPlayer.race;
            UpdateColorPickers();
            ResetColorPickers();
            SelectColorPicker((CategoryId)1);
            _colorPickers[1].SetSelected(selected: true);
        }

        public void StoreOriginalValues()
        {
            var mrPlagueRacesPlayer = Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            _originalRace = mrPlagueRacesPlayer.race;
            _originalSkinVariant = Main.LocalPlayer.skinVariant;
            _originalHair = Main.LocalPlayer.hair;
            _originalHairAux1 = mrPlagueRacesPlayer.auxilaryHairstyle1;
            _originalHairAux2 = mrPlagueRacesPlayer.auxilaryHairstyle2;
            _originalHairAux3 = mrPlagueRacesPlayer.auxilaryHairstyle3;
            _originalHairColor = Main.LocalPlayer.hairColor;
            _originalSkinColor = Main.LocalPlayer.skinColor;
            _originalDetailColor = mrPlagueRacesPlayer.detailColor;
            _originalAuxilaryDetailColor1 = mrPlagueRacesPlayer.auxilaryDetailColor1;
            _originalAuxilaryDetailColor2 = mrPlagueRacesPlayer.auxilaryDetailColor2;
            _originalAuxilaryDetailColor3 = mrPlagueRacesPlayer.auxilaryDetailColor3;
            _originalEyeColor = Main.LocalPlayer.eyeColor;
            _originalShirtColor = Main.LocalPlayer.shirtColor;
            _originalUnderShirtColor = Main.LocalPlayer.underShirtColor;
            _originalPantsColor = Main.LocalPlayer.pantsColor;
            _originalShoeColor = Main.LocalPlayer.shoeColor;
        }

        public void ReinstateOriginalValues()
        {
            var mrPlagueRacesPlayer = Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>();
            mrPlagueRacesPlayer.race = _originalRace;
            Main.LocalPlayer.skinVariant = _originalSkinVariant;
            Main.LocalPlayer.hair = _originalHair;
            mrPlagueRacesPlayer.auxilaryHairstyle1 = _originalHairAux1;
            mrPlagueRacesPlayer.auxilaryHairstyle2 = _originalHairAux2;
            mrPlagueRacesPlayer.auxilaryHairstyle3 = _originalHairAux3;
            Main.LocalPlayer.hairColor = _originalHairColor;
            Main.LocalPlayer.skinColor = _originalSkinColor;
            mrPlagueRacesPlayer.detailColor = _originalDetailColor;
            mrPlagueRacesPlayer.auxilaryDetailColor1 = _originalAuxilaryDetailColor1;
            mrPlagueRacesPlayer.auxilaryDetailColor2 = _originalAuxilaryDetailColor2;
            mrPlagueRacesPlayer.auxilaryDetailColor3 = _originalAuxilaryDetailColor3;
            Main.LocalPlayer.eyeColor = _originalEyeColor;
            Main.LocalPlayer.shirtColor = _originalShirtColor;
            Main.LocalPlayer.underShirtColor = _originalUnderShirtColor;
            Main.LocalPlayer.pantsColor = _originalPantsColor;
            Main.LocalPlayer.shoeColor = _originalShoeColor;
            if (Main.LocalPlayer.whoAmI == Main.myPlayer)
            {
                mrPlagueRacesPlayer.SyncRace(-1, Main.myPlayer);
                mrPlagueRacesPlayer.SyncPlayerAppearance(-1, Main.myPlayer);
            }
        }
    }
}