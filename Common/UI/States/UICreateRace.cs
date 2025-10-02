using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using ReLogic.Graphics;
using ReLogic.OS;
using System;
using System.Collections.Generic;
using System.Globalization;
using Terraria.Utilities.FileBrowser;
using System.Linq;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.GameContent;
using Terraria.GameContent.UI.States;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Initializers;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader.Core;
using Terraria.UI;
using Terraria.UI.Gamepad;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.UI.Elements;

namespace MrPlagueRaces.Common.UI.States
{
    public class UICreateRace : UIState, IHaveBackButtonCommand
    {
        private enum CategoryId
        {
            CharInfo,
            RaceSelect,
            Clothing,
            HairStyle,
            HairColor,
            Eye,
            Skin,
            Detail,
            Shirt,
            Undershirt,
            Pants,
            Shoes,
            DetailAux1,
            DetailAux2,
            DetailAux3,
            Count
        }
        private enum HSLSliderId
        {
            Hue,
            Saturation,
            Luminance
        }

        private int[] _validClothStyles = new int[10]
        {
            0,
            2,
            1,
            3,
            8,
            4,
            6,
            5,
            7,
            9
        };
        private CategoryId _selectedPicker;

        private Vector3 _currentColorHSL;

        private int detailColorCount = 1;

        private UIRaceColoredImageButton[] _colorPickers;

        private UIRaceColoredImageButton _clothingStylesCategoryButton;

        private UIRaceColoredImageButton _hairStylesCategoryButton;

        public static Player _player;

        private UIElement _hslContainer;

        private UIElement _hairstylesContainer;

        private UIElement _clothStylesContainer;

        private UIText _hslHexText;

        private UIElement _copyHexButton;

        private UIElement _pasteHexButton;

        private UIElement _randomColorButton;

        private UIElement _copyTemplateButton;

        private UIElement _pasteTemplateButton;

        private UIColoredImageButtonSmallFix _familiarShirtButton;

        private UIColoredImageButtonSmallFix _familiarPantsButton;

        private UIColoredImageButtonSmallFix _censorClothingButton;

        private UIColoredImageButtonSmallFix _detailCountButton;

        private UIColoredImageButtonSmallFix _genderMale;

        private UIColoredImageButtonSmallFix _genderFemale;

        private UIElement _randomizePlayerButton;

        private UIElement _baseElement;

        private UIPanel _modNamePanel;

        private UIPanel _modDisplayNamePanel;

        private UIPanel _modAuthorPanel;

        private UIPanel _raceNamePanel;

        private UIPanel _raceDisplayNamePanel;

        private UIPanel _modNameSubPanel;

        private UIPanel _modDisplayNameSubPanel;

        private UIPanel _modAuthorSubPanel;

        private UIPanel _raceNameSubPanel;

        private UIPanel _raceDisplayNameSubPanel;

        private bool HasModifiedBodySheet = false;
        private bool HasModifiedLegsSheet = false;

        private bool censorClothingEnabled = true;
        private bool familiarShirtEnabled = true;
        private bool familiarPantsEnabled = true;

        private UITextPanel<string> buttonBack;
        private UITextPanel<string> buttonCreate;

        private UIPanel mainColorPalettePanel;
        private static UIPanel mainAppearancePanel;
        private UIPanel mainAbilitiesPanel;
        private UIPanel mainRaceInfoPanel;

        private UIPanel colorPickerPanel;

        public static int[] raceStats = new int[32];

        private UIFocusInputTextField_mp _modName;

        private UIFocusInputTextField_mp _modDisplayName;

        private UIFocusInputTextField_mp _modAuthor;

        private UIFocusInputTextField_mp _raceName;

        private UIFocusInputTextField_mp _raceDisplayName;

        private UIRaceColoredImageButton colorPaletteButton;

        private UIRaceColoredImageButton appearanceButton;

        private UIRaceColoredImageButton abilitiesButton;

        private UIRaceColoredImageButton raceInfoButton;

        public static UIList modularList;
        public static UIRaceSheetPanel[] sheetList = new UIRaceSheetPanel[1000];
        public static int sheetCount = 0;

        public static string maleHurtPath;
        public static string femaleHurtPath;
        public static string deathPath;

        public static bool enableDefaultAbility = true;
        public static bool enableDefaultAttribute = true;
        public static bool drawTitleOnIcon = true;
        public static bool drawPlayerOnIcon = true;

        public static UIText _descriptionText;

        private string lastKnownMessage = "";

        private Item familiarPants;
        private Item familiarShirt;
        private Item blankItem;

        private Action _cancelAction;

        private int myStat;

        public UIState PreviousUIState
        {
            get;
            set;
        }

        public UICreateRace(Action cancelAction)
        {
            _cancelAction = cancelAction;
        }

        // Pushes the sheets below this index down by 1. Adds a new blank one
        public static void AddSheetAtIndex(int index)
        {
            if (sheetCount < 1000)
            {
                UIRaceSheetPanel newSheet = new UIRaceSheetPanel(_player, index + 1, ref _descriptionText, null, SheetCategory.Head, 5, -1, 0, 0)
                {
                    Width = StyleDimension.FromPixels(540f),
                    Height = StyleDimension.FromPixels(60f)
                };
                for (int i = 0; i < sheetCount; i++)
                {
                    sheetList[i]._sheetIndex = i;
                }
                for (int i = sheetCount - 1; i > index; i--)
                {
                    sheetList[i + 1] = sheetList[i];
                }
                sheetList[index + 1] = newSheet;
                for (int i = 0; i < sheetCount; i++)
                {
                    sheetList[i]._sheetIndex = i;
                }
                sheetCount++;
                ReloadSheetList();
            }
            else
            {
                _descriptionText.SetText("Sheet capacity reached. To add more sheets, create your race's Mod Source and add files manually.");
            }
        }
        // Pushes the sheets below this index up by 1. Deletes the last remaining index
        public static void RemoveSheetAtIndex(int index)
        {
            for (int i = index; i < sheetCount - 1; i++)
            {
                sheetList[i] = sheetList[i + 1];
            }
            for (int i = 0; i < sheetCount; i++)
            {
                sheetList[i]._sheetIndex = i;
            }
            sheetList[sheetCount - 1].Remove();
            sheetList[sheetCount - 1] = null;
            sheetCount--;
            ReloadSheetList();
        }

        public static void ResetSheetList()
        {
            for (int i = 0; i < sheetCount; i++)
            {
                sheetList[i].Remove();
                sheetList[i] = null;
            }
            sheetCount = 0;
        }

        public static void ReapplyAllSheets()
        {
            for (int i = 0; i < sheetCount; i++)
            {
                sheetList[i].ReapplySheet();
            }
        }

        public static void ReloadSheetList()
        {
            mainAppearancePanel.RemoveAllChildren();
            UIRaceSoundPanel uIRaceSoundPanel = new UIRaceSoundPanel(_player, ref _descriptionText)
            {
                Width = StyleDimension.FromPixels(540f),
                Height = StyleDimension.FromPixels(60f),
                Top = StyleDimension.FromPixelsAndPercent(4f, 0f)
            };
            mainAppearancePanel.Append(uIRaceSoundPanel);
            mainAppearancePanel.Height.Set((sheetCount * 65f) + 60f + 4f + 4f + 4f, 0f);
            for (int i = 0; i < sheetCount; i++)
            {
                sheetList[i]._sheetIndex = i;
                sheetList[i].Top.Set((i * 65f) + 65f + 4f + 4f, 0f);
                mainAppearancePanel.Append(sheetList[i]);
            }
        }

        public static bool DuplicateSheetExists(SheetCategory sheetType, int colorType, int hairType, int trackType, int genderType)
        {
            int sheetsOfType = 0;
            for (int i = 0; i < sheetCount; i++)
            {
                if (sheetList[i]._sheetType == sheetType && sheetList[i]._colorType == colorType && sheetList[i]._hairType == hairType && sheetList[i]._trackType == trackType && sheetList[i]._genderType == genderType)
                {
                    sheetsOfType++;
                }
            }
            return sheetsOfType > 1;
        }

        public static int GetDuplicateSheet(int index, SheetCategory sheetType, int colorType, int hairType, int trackType, int genderType)
        {
            int duplicateSheetIndex = 0;
            for (int i = 0; i < sheetCount; i++)
            {
                if (sheetList[i]._sheetType == sheetType && sheetList[i]._colorType == colorType && sheetList[i]._hairType == hairType && sheetList[i]._trackType == trackType && sheetList[i]._genderType == genderType)
                {
                    if (index != sheetList[i]._sheetIndex)
                    {
                        duplicateSheetIndex = sheetList[i]._sheetIndex;
                    }
                }
            }
            return duplicateSheetIndex;
        }

        public void AddSheetToPlayerAndUI(int index, string filePath, SheetCategory sheetType, int colorType, int hairType, int trackType, int genderType)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            mrPlagueRacesPlayer.race.SetOverrideTextureFromModContent(filePath, sheetType, colorType, hairType, trackType, genderType);

            UIRaceSheetPanel uIRaceSheetPanel = new UIRaceSheetPanel(_player, index, ref _descriptionText, filePath, sheetType, colorType, hairType, trackType, genderType)
            {
                Width = StyleDimension.FromPixels(540f),
                Height = StyleDimension.FromPixels(60f)
            };
            sheetList[sheetCount] = uIRaceSheetPanel;
            sheetCount++;
        }

        public override void OnInitialize()
        {
            enableDefaultAbility = true;
            enableDefaultAttribute = true;
            drawTitleOnIcon = true;
            drawPlayerOnIcon = true;
            int[] male = { 0, 2, 1, 3, 8 };
            blankItem = new Item();
            blankItem.SetDefaults(0);
            familiarShirt = new Item();
            familiarShirt.SetDefaults(ItemID.FamiliarShirt);
            familiarPants = new Item();
            familiarPants.SetDefaults(ItemID.FamiliarPants);
            _player = new Player();
            _player.difficulty = 0;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            RaceLoader.TryGetRace("MrPlagueRaces/Human", out var newRace);
            mrPlagueRacesPlayer.race = newRace; // The player displayed in this UI is set to the Human race
            mrPlagueRacesPlayer.race.ClearSheets(false, false); // ... but with every sheet cleared

            if (mrPlagueRacesPlayer.race.StarterShirt)
            {
                _player.armor[11] = familiarShirt;
            }
            if (mrPlagueRacesPlayer.race.StarterPants)
            {
                _player.armor[12] = familiarPants;
            }
            _player.skinVariant = male[mrPlagueRacesPlayer.race.ClothStyle - 1];
            _player.hair = mrPlagueRacesPlayer.race.HairStyle;
            mrPlagueRacesPlayer.auxilaryHairstyle1 = mrPlagueRacesPlayer.race.AuxilaryHairstyle1;
            mrPlagueRacesPlayer.auxilaryHairstyle2 = mrPlagueRacesPlayer.race.AuxilaryHairstyle2;
            mrPlagueRacesPlayer.auxilaryHairstyle3 = mrPlagueRacesPlayer.race.AuxilaryHairstyle3;
            _player.hairColor = mrPlagueRacesPlayer.race.HairColor;
            _player.skinColor = mrPlagueRacesPlayer.race.SkinColor;
            mrPlagueRacesPlayer.detailColor = mrPlagueRacesPlayer.race.DetailColor;
            mrPlagueRacesPlayer.auxilaryDetailColor1 = Color.Red;
            mrPlagueRacesPlayer.auxilaryDetailColor2 = Color.Green;
            mrPlagueRacesPlayer.auxilaryDetailColor3 = Color.Blue;
            _player.eyeColor = mrPlagueRacesPlayer.race.EyeColor;
            _player.shirtColor = mrPlagueRacesPlayer.race.ShirtColor;
            _player.underShirtColor = mrPlagueRacesPlayer.race.UnderShirtColor;
            _player.pantsColor = mrPlagueRacesPlayer.race.PantsColor;
            _player.shoeColor = mrPlagueRacesPlayer.race.ShoeColor;

            _baseElement = new UIElement
            {
                Width =
                {
                    Percent = 0.8f
                },
                MaxWidth = UICommon.MaxPanelWidth,
                Top =
                {
                    Pixels = 220f
                },
                Height =
                {
                    Pixels = -220f,
                    Percent = 1f
                },
                HAlign = 0.5f
            };
            Append(_baseElement);
            UIPanel mainPanel = new UIPanel
            {
                Width =
                {
                    Percent = 1f
                },
                Height =
                {
                    Pixels = -110f,
                    Percent = 1f
                },
                BackgroundColor = UICommon.MainPanelBackground,
                PaddingTop = 0f
            };
            _baseElement.Append(mainPanel);
            buttonBack = new UITextPanel<string>(Language.GetTextValue("UI.Back"))
            {
                Width =
                {
                    Pixels = -10f,
                    Percent = 0.5f
                },
                Height =
                {
                    Pixels = 25f
                },
                VAlign = 1f,
                Top =
                {
                    Pixels = -65f
                }
            }.WithFadedMouseOver();
            buttonBack.OnMouseOver += ShowOptionDescription;
            buttonBack.OnMouseOut += ClearOptionDescription;
            buttonBack.OnLeftClick += BackClick;
            _baseElement.Append(buttonBack);
            buttonCreate = new UITextPanel<string>(Language.GetTextValue("LegacyMenu.28"));
            buttonCreate.CopyStyle(buttonBack);
            buttonCreate.HAlign = 1f;
            buttonCreate.WithFadedMouseOver();
            buttonCreate.OnMouseOver += ShowOptionDescription;
            buttonCreate.OnMouseOut += ClearOptionDescription;
            buttonCreate.OnLeftClick += OKClick;
            _baseElement.Append(buttonCreate);

            UIHorizontalSeparator firstSeparator = new UIHorizontalSeparator
            {
                Left = StyleDimension.FromPixelsAndPercent(0f, 0.01f),
                Top = StyleDimension.FromPixels(52f),
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.98f),
                Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
            };
            mainPanel.Append(firstSeparator);

            colorPaletteButton = new UIRaceColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/ColorPalette", (AssetRequestMode)1), false, false, false);
            colorPaletteButton.VAlign = 0f;
            colorPaletteButton.HAlign = 0f;
            colorPaletteButton.Top = StyleDimension.FromPixels(4f);
            colorPaletteButton.Left.Set(-94f + 0 * 48f, 0.5f);
            colorPaletteButton.OnMouseOver += ShowOptionDescription;
            colorPaletteButton.OnMouseOut += ClearOptionDescription;
            colorPaletteButton.OnLeftClick += Click_ColorPalette;
            mainPanel.Append(colorPaletteButton);

            appearanceButton = new UIRaceColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Appearance", (AssetRequestMode)1), false, false, false);
            appearanceButton.VAlign = 0f;
            appearanceButton.HAlign = 0f;
            appearanceButton.Top = StyleDimension.FromPixels(4f);
            appearanceButton.Left.Set(-94f + 1 * 48f, 0.5f);
            appearanceButton.OnMouseOver += ShowOptionDescription;
            appearanceButton.OnMouseOut += ClearOptionDescription;
            appearanceButton.OnLeftClick += Click_Appearance;
            mainPanel.Append(appearanceButton);

            abilitiesButton = new UIRaceColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Abilities", (AssetRequestMode)1), false, false, false);
            abilitiesButton.VAlign = 0f;
            abilitiesButton.HAlign = 0f;
            abilitiesButton.Top = StyleDimension.FromPixels(4f);
            abilitiesButton.Left.Set(-94f + 2 * 48f, 0.5f);
            abilitiesButton.OnMouseOver += ShowOptionDescription;
            abilitiesButton.OnMouseOut += ClearOptionDescription;
            abilitiesButton.OnLeftClick += Click_Abilities;
            mainPanel.Append(abilitiesButton);

            raceInfoButton = new UIRaceColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/RaceInfo", (AssetRequestMode)1), false, false, false);
            raceInfoButton.VAlign = 0f;
            raceInfoButton.HAlign = 0f;
            raceInfoButton.Top = StyleDimension.FromPixels(4f);
            raceInfoButton.Left.Set(-94f + 3 * 48f, 0.5f);
            raceInfoButton.OnMouseOver += ShowOptionDescription;
            raceInfoButton.OnMouseOut += ClearOptionDescription;
            raceInfoButton.OnLeftClick += Click_RaceInfo;
            mainPanel.Append(raceInfoButton);

            UIPanel charPreviewAnchor = new UIPanel
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPixels(_baseElement.Height.Pixels - 150f - (float)4),
                Top = StyleDimension.FromPixels(40f),
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            charPreviewAnchor.SetPadding(0f);
            _baseElement.Append(charPreviewAnchor);
            MakeCharPreview(charPreviewAnchor);
            AddDescriptionPanel(mainPanel, 284 + 8 + 6 + 6, 100 - 12 - 12 - 8, "desc");

            UIPanel modularPanel = new UIPanel
            {
                Width = StyleDimension.FromPixelsAndPercent(-10f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(-130f, 1f),
                HAlign = 0.5f,
                VAlign = 0f,
                Top = StyleDimension.FromPixels(52f),
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            mainPanel.Append(modularPanel);
            modularPanel.SetPadding(0f);
            modularList = new UIList
            {
                Width = StyleDimension.FromPixelsAndPercent(-18f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(-6f, 1f)
            };
            modularList.SetPadding(4f);
            modularPanel.Append(modularList);
            UIScrollbar modularScrollbar = new UIScrollbar
            {
                HAlign = 1f,
                Height = StyleDimension.FromPixelsAndPercent(-30f, 1f),
                Top = StyleDimension.FromPixels(15f)
            };
            modularScrollbar.SetView(100f, 1000f);
            modularList.SetScrollbar(modularScrollbar);
            modularPanel.Append(modularScrollbar);

            SetUpColorPalette();
            SetUpAppearance();
            SetUpAbilities();
            SetUpRaceInfo();

            UpdateSelectedGender();
            colorPaletteButton.SetSelected(true);
            SwitchMenu(mainColorPalettePanel);
            _familiarShirtButton.SetColor(_player.shirtColor);
            _familiarPantsButton.SetColor(_player.pantsColor);
            _censorClothingButton.SetColor(_player.pantsColor);
            _detailCountButton.SetColors(mrPlagueRacesPlayer.detailColor, mrPlagueRacesPlayer.auxilaryDetailColor1, mrPlagueRacesPlayer.auxilaryDetailColor2, mrPlagueRacesPlayer.auxilaryDetailColor3, detailColorCount);
            _familiarShirtButton.SetSelected(familiarShirtEnabled);
            _familiarPantsButton.SetSelected(familiarPantsEnabled);
            _censorClothingButton.SetSelected(censorClothingEnabled);
            SwitchMenu(mainColorPalettePanel);
        }


        private void Click_CharClothStyle(UIMouseEvent evt, UIElement listeningElement)
        {
            _clothingStylesCategoryButton.SetImageWithoutSettingSize(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/" + (_player.Male ? "ClothStyleMale" : "ClothStyleFemale"), (AssetRequestMode)1));
            UpdateSelectedGender();
        }

        private void Click_CharGenderMale(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            _player.Male = true;
            Click_CharClothStyle(evt, listeningElement);
            UpdateSelectedGender();
        }

        private void Click_CharGenderFemale(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            _player.Male = false;
            Click_CharClothStyle(evt, listeningElement);
            UpdateSelectedGender();
        }

        private void UpdateSelectedGender()
        {
            _genderMale.SetSelected(_player.Male);
            _genderFemale.SetSelected(!_player.Male);
        }

        private void Click_CopyHex(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            Platform.Get<IClipboard>().Value = (_hslHexText.Text);
        }

        private void Click_PasteHex(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            string value = Platform.Get<IClipboard>().Value;
            if (GetHexColor(value, out Vector3 hsl))
            {
                ApplyPendingColor(ScaledHslToRgb(hsl.X, hsl.Y, hsl.Z));
                _currentColorHSL = hsl;
                UpdateHexText(ScaledHslToRgb(hsl.X, hsl.Y, hsl.Z));
                UpdateColorPickers();
            }
        }

        private void Click_CopyPlayerTemplate(UIMouseEvent evt, UIElement listeningElement)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            SoundEngine.PlaySound(SoundID.MenuTick);
            Dictionary<string, object> obj = new Dictionary<string, object>
            {
                {
                    "version",
                    1
                },
                {
                    "hairStyle",
                    _player.hair
                },
                {
                    "clothingStyle",
                    _player.skinVariant
                },
                {
                    "hairColor",
                    GetHexText(_player.hairColor)
                },
                {
                    "eyeColor",
                    GetHexText(_player.eyeColor)
                },
                {
                    "skinColor",
                    GetHexText(_player.skinColor)
                },
                {
                    "detailColor",
                    GetHexText(mrPlagueRacesPlayer.detailColor)
                },
                {
                    "shirtColor",
                    GetHexText(_player.shirtColor)
                },
                {
                    "underShirtColor",
                    GetHexText(_player.underShirtColor)
                },
                {
                    "pantsColor",
                    GetHexText(_player.pantsColor)
                },
                {
                    "shoeColor",
                    GetHexText(_player.shoeColor)
                },
                {
                    "auxilaryDetailColor1",
                    GetHexText(mrPlagueRacesPlayer.auxilaryDetailColor1)
                },
                {
                    "auxilaryDetailColor2",
                    GetHexText(mrPlagueRacesPlayer.auxilaryDetailColor2)
                },
                {
                    "auxilaryDetailColor3",
                    GetHexText(mrPlagueRacesPlayer.auxilaryDetailColor3)
                },
            };
            JsonSerializerSettings val = new JsonSerializerSettings();
            val.TypeNameHandling = ((TypeNameHandling)4);
            val.MetadataPropertyHandling = ((MetadataPropertyHandling)1);
            val.Formatting = ((Formatting)1);
            string text = JsonConvert.SerializeObject((object)obj, (JsonSerializerSettings)(object)val);
            PlayerInput.PrettyPrintProfiles(ref text);
            Platform.Get<IClipboard>().Value = (text);
            _familiarShirtButton.SetColor(_player.shirtColor);
            _familiarPantsButton.SetColor(_player.pantsColor);
            _censorClothingButton.SetColor(_player.pantsColor);
            _detailCountButton.SetColors(mrPlagueRacesPlayer.detailColor, mrPlagueRacesPlayer.auxilaryDetailColor1, mrPlagueRacesPlayer.auxilaryDetailColor2, mrPlagueRacesPlayer.auxilaryDetailColor3, detailColorCount);
        }

        private void Click_PastePlayerTemplate(UIMouseEvent evt, UIElement listeningElement)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            SoundEngine.PlaySound(SoundID.MenuTick);
            try
            {
                string color3 = Platform.Get<IClipboard>().Value;
                int i = color3.IndexOf("{");
                if (i != -1)
                {
                    color3 = color3.Substring(i);
                    int num = color3.LastIndexOf("}");
                    if (num != -1)
                    {
                        color3 = color3.Substring(0, num + 1);
                        Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(color3);
                        if (dictionary != null)
                        {
                            Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
                            foreach (KeyValuePair<string, object> item in dictionary)
                            {
                                dictionary2[item.Key.ToLower()] = item.Value;
                            }
                            if (dictionary2.TryGetValue("version", out object value))
                            {
                                _ = (long)value;
                            }
                            if (dictionary2.TryGetValue("hairstyle", out value))
                            {
                                int num2 = (int)(long)value;
                                if (Main.Hairstyles.AvailableHairstyles.Contains(num2))
                                {
                                    _player.hair = num2;
                                }
                            }
                            if (dictionary2.TryGetValue("clothingstyle", out value))
                            {
                                int num3 = (int)(long)value;
                                if (_validClothStyles.Contains(num3))
                                {
                                    _player.skinVariant = num3;
                                }
                            }
                            if (dictionary2.TryGetValue("haircolor", out value) && GetHexColor((string)value, out Vector3 flag))
                            {
                                _player.hairColor = ScaledHslToRgb(flag);
                            }
                            if (dictionary2.TryGetValue("eyecolor", out value) && GetHexColor((string)value, out flag))
                            {
                                _player.eyeColor = ScaledHslToRgb(flag);
                            }
                            if (dictionary2.TryGetValue("skincolor", out value) && GetHexColor((string)value, out flag))
                            {
                                _player.skinColor = ScaledHslToRgb(flag);
                            }
                            if (dictionary2.TryGetValue("detailcolor", out value) && GetHexColor((string)value, out flag))
                            {
                                mrPlagueRacesPlayer.detailColor = ScaledHslToRgb(flag);
                            }
                            if (dictionary2.TryGetValue("shirtcolor", out value) && GetHexColor((string)value, out flag))
                            {
                                _player.shirtColor = ScaledHslToRgb(flag);
                            }
                            if (dictionary2.TryGetValue("undershirtcolor", out value) && GetHexColor((string)value, out flag))
                            {
                                _player.underShirtColor = ScaledHslToRgb(flag);
                            }
                            if (dictionary2.TryGetValue("pantscolor", out value) && GetHexColor((string)value, out flag))
                            {
                                _player.pantsColor = ScaledHslToRgb(flag);
                            }
                            if (dictionary2.TryGetValue("shoecolor", out value) && GetHexColor((string)value, out flag))
                            {
                                _player.shoeColor = ScaledHslToRgb(flag);
                            }
                            if (dictionary2.TryGetValue("auxilarydetailcolor1", out value) && GetHexColor((string)value, out flag))
                            {
                                mrPlagueRacesPlayer.auxilaryDetailColor1 = ScaledHslToRgb(flag);
                            }
                            if (dictionary2.TryGetValue("auxilarydetailcolor2", out value) && GetHexColor((string)value, out flag))
                            {
                                mrPlagueRacesPlayer.auxilaryDetailColor2 = ScaledHslToRgb(flag);
                            }
                            if (dictionary2.TryGetValue("auxilarydetailcolor3", out value) && GetHexColor((string)value, out flag))
                            {
                                mrPlagueRacesPlayer.auxilaryDetailColor3 = ScaledHslToRgb(flag);
                            }
                            Click_CharClothStyle(null, null);
                            UpdateColorPickers();
                        }
                    }
                }
            }
            catch
            {
            }
            _familiarShirtButton.SetColor(_player.shirtColor);
            _familiarPantsButton.SetColor(_player.pantsColor);
            _censorClothingButton.SetColor(_player.pantsColor);
            _detailCountButton.SetColors(mrPlagueRacesPlayer.detailColor, mrPlagueRacesPlayer.auxilaryDetailColor1, mrPlagueRacesPlayer.auxilaryDetailColor2, mrPlagueRacesPlayer.auxilaryDetailColor3, detailColorCount);
        }

        private void Click_RandomizePlayer(UIMouseEvent evt, UIElement listeningElement)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            SoundEngine.PlaySound(SoundID.MenuTick);
            Player player = _player;
            player.hair = Main.rand.Next(mrPlagueRacesPlayer.GetRaceHairCount(_player));
            mrPlagueRacesPlayer.auxilaryHairstyle1 = Main.rand.Next(mrPlagueRacesPlayer.GetRaceHairCount(_player, 1));
            mrPlagueRacesPlayer.auxilaryHairstyle2 = Main.rand.Next(mrPlagueRacesPlayer.GetRaceHairCount(_player, 2));
            mrPlagueRacesPlayer.auxilaryHairstyle3 = Main.rand.Next(mrPlagueRacesPlayer.GetRaceHairCount(_player, 3));
            player.eyeColor = ScaledHslToRgb(GetRandomColorVector());
            while (player.eyeColor.R + player.eyeColor.G + player.eyeColor.B > 300)
            {
                player.eyeColor = ScaledHslToRgb(GetRandomColorVector());
            }
            float num = (float)Main.rand.Next(60, 120) * 0.01f;
            if (num > 1f)
            {
                num = 1f;
            }
            player.skinColor = ScaledHslToRgb(GetRandomColorVector());
            mrPlagueRacesPlayer.detailColor = ScaledHslToRgb(GetRandomColorVector());
            player.hairColor = ScaledHslToRgb(GetRandomColorVector());
            player.shirtColor = ScaledHslToRgb(GetRandomColorVector());
            player.underShirtColor = ScaledHslToRgb(GetRandomColorVector());
            player.pantsColor = ScaledHslToRgb(GetRandomColorVector());
            player.shoeColor = ScaledHslToRgb(GetRandomColorVector());
            mrPlagueRacesPlayer.auxilaryDetailColor1 = ScaledHslToRgb(GetRandomColorVector());
            mrPlagueRacesPlayer.auxilaryDetailColor2 = ScaledHslToRgb(GetRandomColorVector());
            mrPlagueRacesPlayer.auxilaryDetailColor3 = ScaledHslToRgb(GetRandomColorVector());
            player.skinVariant = _validClothStyles[Main.rand.Next(_validClothStyles.Length)];
            switch (player.hair + 1)
            {
                case 5:
                case 6:
                case 7:
                case 10:
                case 12:
                case 19:
                case 22:
                case 23:
                case 26:
                case 27:
                case 30:
                case 33:
                    player.Male = false;
                    break;
                default:
                    player.Male = true;
                    break;
            }
            Click_CharClothStyle(null, null);
            UpdateSelectedGender();
            UpdateColorPickers();
            _familiarShirtButton.SetColor(_player.shirtColor);
            _familiarPantsButton.SetColor(_player.pantsColor);
            _censorClothingButton.SetColor(_player.pantsColor);
            _detailCountButton.SetColors(mrPlagueRacesPlayer.detailColor, mrPlagueRacesPlayer.auxilaryDetailColor1, mrPlagueRacesPlayer.auxilaryDetailColor2, mrPlagueRacesPlayer.auxilaryDetailColor3, detailColorCount);
        }

        private void Click_ToggleFamiliarShirt(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            familiarShirtEnabled = !familiarShirtEnabled;
            _familiarShirtButton.SetSelected(familiarShirtEnabled);
        }

        private void Click_ToggleFamiliarPants(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            familiarPantsEnabled = !familiarPantsEnabled;
            _familiarPantsButton.SetSelected(familiarPantsEnabled);
        }

        private void Click_ToggleCensorClothing(UIMouseEvent evt, UIElement listeningElement)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            if (HasModifiedBodySheet && HasModifiedLegsSheet)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                censorClothingEnabled = !censorClothingEnabled;
                _censorClothingButton.SetSelected(censorClothingEnabled);
            }
            else if (HasModifiedLegsSheet)
            {
                _descriptionText.SetText("You must change the body sheet before disabling censor clothing.");
            }
            else if (HasModifiedBodySheet)
            {
                _descriptionText.SetText("You must change both the male & female leg sheets before disabling censor clothing.");
            }
            else
            {
                _descriptionText.SetText("You must change the body and leg sheets before disabling censor clothing.");
            }
        }

        private void Click_ChangeDetailCount(UIMouseEvent evt, UIElement listeningElement)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (detailColorCount > 3)
            {
                detailColorCount = 1;
            }
            else
            {
                detailColorCount += 1;
            }
            _detailCountButton.SetMiddleTexture(ModContent.Request<Texture2D>($"MrPlagueRaces/Assets/Textures/UI/CategoryPanelHighlightSmall_{detailColorCount}", (AssetRequestMode)1));
            _descriptionText.SetText(Language.GetText($"Mods.MrPlagueRaces.UI.DetailCount_{detailColorCount}"));
            UpdateColorPickers();
            _detailCountButton.SetColors(mrPlagueRacesPlayer.detailColor, mrPlagueRacesPlayer.auxilaryDetailColor1, mrPlagueRacesPlayer.auxilaryDetailColor2, mrPlagueRacesPlayer.auxilaryDetailColor3, detailColorCount);
        }

        private void MakeHairstylesMenu(UIElement middleInnerPanel)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            Main.Hairstyles.UpdateUnlocks();
            UIElement s2 = new UIElement
            {
                Width = StyleDimension.FromPixelsAndPercent(-10f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                HAlign = 0.5f,
                VAlign = 0.5f,
                Top = StyleDimension.FromPixels(6f)
            };
            middleInnerPanel.Append(s2);
            s2.SetPadding(0f);
            UIList hairList = new UIList
            {
                Width = StyleDimension.FromPixelsAndPercent(-18f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(-6f, 1f)
            };
            hairList.SetPadding(4f);
            s2.Append(hairList);
            UIScrollbar hairScrollbar = new UIScrollbar
            {
                HAlign = 1f,
                Height = StyleDimension.FromPixelsAndPercent(-30f, 1f),
                Top = StyleDimension.FromPixels(10f)
            };
            hairScrollbar.SetView(100f, 1000f);
            hairList.SetScrollbar(hairScrollbar);
            s2.Append(hairScrollbar);
            int hairCount = mrPlagueRacesPlayer.GetRaceHairCount(_player, 0);
            int hairCountAux1 = mrPlagueRacesPlayer.GetRaceHairCount(_player, 1);
            int hairCountAux2 = mrPlagueRacesPlayer.GetRaceHairCount(_player, 2);
            int hairCountAux3 = mrPlagueRacesPlayer.GetRaceHairCount(_player, 3);
            int[] hairCategories = { hairCount, hairCountAux1, hairCountAux2, hairCountAux3 };
            int totalHair = (int)(Math.Ceiling((double)hairCount / 10) * 10) + (int)(Math.Ceiling((double)hairCountAux1 / 10) * 10) + (int)(Math.Ceiling((double)hairCountAux2 / 10) * 10) + (int)(Math.Ceiling((double)hairCountAux3 / 10) * 10);
            int activeHairAuxilaries = (Math.Clamp(hairCountAux1, 0, 1)) + (Math.Clamp(hairCountAux2, 0, 1)) + (Math.Clamp(hairCountAux3, 0, 1));
            int currentHairAuxilary = 0;
            UIElement hairPanel = new UIElement
            {
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(48 * (totalHair / 10 + ((totalHair % 10 != 0) ? 1 : 0)) + (24f * activeHairAuxilaries), 0f)
            };
            hairList.Add(hairPanel);
            hairPanel.SetPadding(0f);
            int totalCounter = 0;
            for (int hairCategory = 0; hairCategory < 1 + activeHairAuxilaries; hairCategory++)
            {
                for (int currentCounter = 0; currentCounter < hairCategories[hairCategory]; currentCounter++)
                {
                    UIMultiHairStyleButton hairButton = new UIMultiHairStyleButton(_player, currentCounter, hairCategory)
                    {
                        Left = StyleDimension.FromPixels((float)(totalCounter % 10) * 48f + 6f + 4f),
                        Top = StyleDimension.FromPixels((float)(totalCounter / 10) * 48f + 1f + (24f * currentHairAuxilary))
                    };
                    hairButton.SetSnapPoint("Middle", totalCounter);
                    hairPanel.Append(hairButton);
                    totalCounter++;
                }
                totalCounter += (int)(Math.Ceiling((double)totalCounter / 10) * 10) - totalCounter;
                currentHairAuxilary++;
                if (currentHairAuxilary <= activeHairAuxilaries)
                {
                    UIHorizontalSeparator element = new UIHorizontalSeparator
                    {
                        Left = StyleDimension.FromPixelsAndPercent(0f, 0.01f),
                        Top = StyleDimension.FromPixels((float)(totalCounter / 10) * 48f + 1f + (24f * currentHairAuxilary) - 12f),
                        Width = StyleDimension.FromPixelsAndPercent(0f, 0.961f),
                        Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
                    };
                    hairPanel.Append(element);
                }
            }
            _hairstylesContainer = s2;
        }

        private void MakeClothStylesMenu(UIElement middleInnerPanel)
        {
            UIElement i = new UIElement
            {
                Width = StyleDimension.FromPixelsAndPercent(-10f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            middleInnerPanel.Append(i);
            i.SetPadding(0f);
            int num = 15;
            for (int j = 0; j < _validClothStyles.Length; j++)
            {
                int num2 = 0;
                if (j >= _validClothStyles.Length / 2)
                {
                    num2 = 18;
                }
                UIClothStyleButton uIClothStyleButton = new UIClothStyleButton(_player, _validClothStyles[j])
                {
                    Left = StyleDimension.FromPixels((float)j * 46f + (float)num2 + 30f + 6f),
                    Top = StyleDimension.FromPixels(num)
                };
                uIClothStyleButton.OnLeftMouseDown += Click_CharClothStyle;
                uIClothStyleButton.SetSnapPoint("Middle", j);
                i.Append(uIClothStyleButton);
            }
            for (int k = 0; k < 2; k++)
            {
                int num3 = 0;
                if (k >= 1)
                {
                    num3 = 18;
                }
                UIHorizontalSeparator element = new UIHorizontalSeparator
                {
                    Left = StyleDimension.FromPixels((float)k * 230f + (float)num3 + 30f + 6f),
                    Top = StyleDimension.FromPixels(num + 86),
                    Width = StyleDimension.FromPixelsAndPercent(228f, 0f),
                    Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
                };
                i.Append(element);
                UIColoredImageButtonSmallFix UIColoredImageButtonSmallFix = CreatePickerWithoutClick_Original(CategoryId.Clothing, "MrPlagueRaces/Assets/Textures/UI/" + ((k == 0) ? "ClothStyleMale" : "ClothStyleFemale"), 0f, 0f);
                UIColoredImageButtonSmallFix.Top = StyleDimension.FromPixelsAndPercent(num + 92, 0f);
                UIColoredImageButtonSmallFix.Left = StyleDimension.FromPixels((float)k * 230f + 126f + (float)num3 + 6f - 4);
                UIColoredImageButtonSmallFix.HAlign = 0f;
                UIColoredImageButtonSmallFix.VAlign = 0f;
                i.Append(UIColoredImageButtonSmallFix);
                if (k == 0)
                {
                    UIColoredImageButtonSmallFix.OnLeftMouseDown += Click_CharGenderMale;
                    _genderMale = UIColoredImageButtonSmallFix;
                }
                else
                {
                    UIColoredImageButtonSmallFix.OnLeftMouseDown += Click_CharGenderFemale;
                    _genderFemale = UIColoredImageButtonSmallFix;
                }
                UIColoredImageButtonSmallFix.SetSnapPoint("Low", k * 4);
            }
            UIElement uIElement = new UIElement
            {
                Width = StyleDimension.FromPixels(130f),
                Height = StyleDimension.FromPixels(50f),
                HAlign = 0.5f,
                Left = StyleDimension.FromPixelsAndPercent(9f, 0f),
                Top = StyleDimension.FromPixelsAndPercent(num + 92, 0f)
            };
            i.Append(uIElement);
            UIElement uIElement2 = new UIElement
            {
                Width = StyleDimension.FromPixels(174f),
                Height = StyleDimension.FromPixels(50f),
                HAlign = 0.5f,
                Left = StyleDimension.FromPixelsAndPercent(9f, 0f),
                Top = StyleDimension.FromPixelsAndPercent(num + 92 + 40, 0f)
            };
            i.Append(uIElement2);
            UIColoredImageButtonSmallFix UIColoredImageButtonSmallFix2 = new UIColoredImageButtonSmallFix(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Copy", (AssetRequestMode)1), isSmall: true)
            {
                VAlign = 0.5f,
                HAlign = 0f,
                Left = StyleDimension.FromPixelsAndPercent(0f, 0f)
            };
            UIColoredImageButtonSmallFix2.OnLeftMouseDown += Click_CopyPlayerTemplate;
            uIElement.Append(UIColoredImageButtonSmallFix2);
            _copyTemplateButton = UIColoredImageButtonSmallFix2;
            UIColoredImageButtonSmallFix UIColoredImageButtonSmallFix3 = new UIColoredImageButtonSmallFix(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Paste", (AssetRequestMode)1), isSmall: true)
            {
                VAlign = 0.5f,
                HAlign = 0.5f
            };
            UIColoredImageButtonSmallFix3.OnLeftMouseDown += Click_PastePlayerTemplate;
            uIElement.Append(UIColoredImageButtonSmallFix3);
            _pasteTemplateButton = UIColoredImageButtonSmallFix3;
            UIColoredImageButtonSmallFix UIColoredImageButtonSmallFix4 = new UIColoredImageButtonSmallFix(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Randomize", (AssetRequestMode)1), isSmall: true)
            {
                VAlign = 0.5f,
                HAlign = 1f
            };
            UIColoredImageButtonSmallFix4.OnLeftMouseDown += Click_RandomizePlayer;
            uIElement.Append(UIColoredImageButtonSmallFix4);
            _randomizePlayerButton = UIColoredImageButtonSmallFix4;


            _familiarShirtButton = new UIColoredImageButtonSmallFix(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/FamiliarShirt", (AssetRequestMode)1), isSmall: true)
            {
                VAlign = 0.5f,
                HAlign = 0.05f,
                Left = StyleDimension.FromPixelsAndPercent(0f, 0f)
            };
            _familiarShirtButton.OnLeftMouseDown += Click_ToggleFamiliarShirt;
            _familiarShirtButton.OnMouseOver += ShowOptionDescription;
            _familiarShirtButton.OnMouseOut += ClearOptionDescription;
            uIElement2.Append(_familiarShirtButton);
            _familiarPantsButton = new UIColoredImageButtonSmallFix(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/FamiliarPants", (AssetRequestMode)1), isSmall: true)
            {
                VAlign = 0.5f,
                HAlign = 0.35f
            };
            _familiarPantsButton.OnLeftMouseDown += Click_ToggleFamiliarPants;
            _familiarPantsButton.OnMouseOver += ShowOptionDescription;
            _familiarPantsButton.OnMouseOut += ClearOptionDescription;
            uIElement2.Append(_familiarPantsButton);
            _censorClothingButton = new UIColoredImageButtonSmallFix(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/CensorPants", (AssetRequestMode)1), isSmall: true)
            {
                VAlign = 0.5f,
                HAlign = 0.65f
            };
            _censorClothingButton.OnLeftMouseDown += Click_ToggleCensorClothing;
            _censorClothingButton.OnMouseOver += ShowOptionDescription;
            _censorClothingButton.OnMouseOut += ClearOptionDescription;
            uIElement2.Append(_censorClothingButton);
            _detailCountButton = new UIColoredImageButtonSmallFix(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/ColorDetailSmall_1", (AssetRequestMode)1), isSmall: true, isSplitIntoFour: true)
            {
                VAlign = 0.5f,
                HAlign = 0.95f
            };
            _detailCountButton.OnLeftMouseDown += Click_ChangeDetailCount;
            _detailCountButton.OnMouseOver += ShowOptionDescription;
            _detailCountButton.OnMouseOut += ClearOptionDescription;
            uIElement2.Append(_detailCountButton);
            _detailCountButton.SetMiddleTexture(ModContent.Request<Texture2D>($"MrPlagueRaces/Assets/Textures/UI/CategoryPanelHighlightSmall_{detailColorCount}", (AssetRequestMode)1));

            UIColoredImageButtonSmallFix2.SetSnapPoint("Low", 1);
            UIColoredImageButtonSmallFix3.SetSnapPoint("Low", 2);
            UIColoredImageButtonSmallFix4.SetSnapPoint("Low", 3);
            _clothStylesContainer = i;
        }

        private void MakeHSLMenu(UIElement parentContainer)
        {
            UIElement uIElement = new UIElement
            {
                Width = StyleDimension.FromPixelsAndPercent(220f, 0f),
                Height = StyleDimension.FromPixelsAndPercent(158f, 0f),
                Left = StyleDimension.FromPixelsAndPercent(9f, 0f),
                HAlign = 0.5f,
                VAlign = 0f
            };
            uIElement.SetPadding(0f);
            parentContainer.Append(uIElement);
            UIElement uIElement2 = new UIPanel
            {
                Width = StyleDimension.FromPixelsAndPercent(220f, 0f),
                Height = StyleDimension.FromPixelsAndPercent(104f, 0f),
                HAlign = 0.5f,
                VAlign = 0f,
                Top = StyleDimension.FromPixelsAndPercent(10f, 0f)
            };
            uIElement2.SetPadding(0f);
            uIElement2.PaddingTop = 3f;
            uIElement.Append(uIElement2);
            uIElement2.Append(CreateHSLSlider(HSLSliderId.Hue));
            uIElement2.Append(CreateHSLSlider(HSLSliderId.Saturation));
            uIElement2.Append(CreateHSLSlider(HSLSliderId.Luminance));
            UIPanel uIPanel = new UIPanel
            {
                VAlign = 1f,
                HAlign = 1f,
                Width = StyleDimension.FromPixelsAndPercent(100f, 0f),
                Height = StyleDimension.FromPixelsAndPercent(32f, 0f)
            };
            UIText uIText = new UIText("FFFFFF")
            {
                VAlign = 0.5f,
                HAlign = 0.5f
            };
            uIPanel.Append(uIText);
            uIElement.Append(uIPanel);
            UIColoredImageButtonSmallFix UIColoredImageButtonSmallFix = new UIColoredImageButtonSmallFix(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Copy", (AssetRequestMode)1), isSmall: true)
            {
                VAlign = 1f,
                HAlign = 0f,
                Left = StyleDimension.FromPixelsAndPercent(0f, 0f)
            };
            UIColoredImageButtonSmallFix.OnLeftMouseDown += Click_CopyHex;
            uIElement.Append(UIColoredImageButtonSmallFix);
            _copyHexButton = UIColoredImageButtonSmallFix;
            UIColoredImageButtonSmallFix UIColoredImageButtonSmallFix2 = new UIColoredImageButtonSmallFix(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Paste", (AssetRequestMode)1), isSmall: true)
            {
                VAlign = 1f,
                HAlign = 0f,
                Left = StyleDimension.FromPixelsAndPercent(40f, 0f)
            };
            UIColoredImageButtonSmallFix2.OnLeftMouseDown += Click_PasteHex;
            uIElement.Append(UIColoredImageButtonSmallFix2);
            _pasteHexButton = UIColoredImageButtonSmallFix2;
            UIColoredImageButtonSmallFix UIColoredImageButtonSmallFix3 = new UIColoredImageButtonSmallFix(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Randomize", (AssetRequestMode)1), isSmall: true)
            {
                VAlign = 1f,
                HAlign = 0f,
                Left = StyleDimension.FromPixelsAndPercent(80f, 0f)
            };
            UIColoredImageButtonSmallFix3.OnLeftMouseDown += Click_RandomizeSingleColor;
            uIElement.Append(UIColoredImageButtonSmallFix3);
            _randomColorButton = UIColoredImageButtonSmallFix3;
            _hslContainer = uIElement;
            _hslHexText = uIText;
            UIColoredImageButtonSmallFix.SetSnapPoint("Low", 0);
            UIColoredImageButtonSmallFix2.SetSnapPoint("Low", 1);
            UIColoredImageButtonSmallFix3.SetSnapPoint("Low", 2);
        }

        private UIColoredSlider CreateHSLSlider(HSLSliderId id)
        {
            UIColoredSlider uIColoredSlider = CreateHSLSliderButtonBase(id);
            uIColoredSlider.VAlign = 0f;
            uIColoredSlider.HAlign = 0f;
            uIColoredSlider.Width = StyleDimension.FromPixelsAndPercent(-10f, 1f);
            uIColoredSlider.Top.Set(30 * (int)id, 0f);
            uIColoredSlider.OnLeftMouseDown += Click_ColorPicker;
            uIColoredSlider.SetSnapPoint("Middle", (int)id, null, new Vector2(0f, 20f));
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
            if ((int)_selectedPicker == 7)
            {
                switch (detailColorCount)
                {
                    case 1:
                        _colorPickers[12]?.SetColor(color);
                        _colorPickers[13]?.SetColor(color);
                        _colorPickers[14]?.SetColor(color);
                        break;
                    case 2:
                        _colorPickers[13]?.SetColor(color);
                        break;
                }
            }
            else if ((int)_selectedPicker == 12)
            {
                switch (detailColorCount)
                {
                    case 2:
                        _colorPickers[14]?.SetColor(color);
                        break;
                }
            }
            else if ((int)_selectedPicker == 13)
            {
                switch (detailColorCount)
                {
                    case 3:
                        _colorPickers[14]?.SetColor(color);
                        break;
                }
            }
            if (_selectedPicker == CategoryId.HairColor)
            {
                _hairStylesCategoryButton.SetColor(color);
            }
            UpdateHexText(color);
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

        private void ApplyPendingColor(Color pendingColor)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            switch (_selectedPicker)
            {
                case CategoryId.HairColor:
                    _player.hairColor = pendingColor;
                    break;
                case CategoryId.Eye:
                    _player.eyeColor = pendingColor;
                    break;
                case CategoryId.Skin:
                    _player.skinColor = pendingColor;
                    break;
                case CategoryId.Detail:
                    mrPlagueRacesPlayer.detailColor = pendingColor;
                    break;
                case CategoryId.Shirt:
                    _player.shirtColor = pendingColor;
                    break;
                case CategoryId.Undershirt:
                    _player.underShirtColor = pendingColor;
                    break;
                case CategoryId.Pants:
                    _player.pantsColor = pendingColor;
                    break;
                case CategoryId.Shoes:
                    _player.shoeColor = pendingColor;
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
            }
        }

        private void UpdateHexText(Color pendingColor)
        {
            _hslHexText.SetText(GetHexText(pendingColor));
        }

        private static string GetHexText(Color pendingColor)
        {
            return "#" + pendingColor.Hex3().ToUpper();
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

        private void Click_RandomizeSingleColor(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            Vector3 parts = GetRandomColorVector();
            ApplyPendingColor(ScaledHslToRgb(parts.X, parts.Y, parts.Z));
            _currentColorHSL = parts;
            UpdateHexText(ScaledHslToRgb(parts.X, parts.Y, parts.Z));
            UpdateColorPickers();
        }

        private static Vector3 GetRandomColorVector()
        {
            return new Vector3(Main.rand.NextFloat(), Main.rand.NextFloat(), Main.rand.NextFloat());
        }

        private void UpdateColorPickers()
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            _colorPickers[4].SetColor(_player.hairColor);
            _hairStylesCategoryButton.SetColor(_player.hairColor);
            _colorPickers[5].SetColor(_player.eyeColor);
            _colorPickers[6].SetColor(_player.skinColor);
            _colorPickers[7].SetColor(mrPlagueRacesPlayer.detailColor);
            _colorPickers[8].SetColor(_player.shirtColor);
            _colorPickers[9].SetColor(_player.underShirtColor);
            _colorPickers[10].SetColor(_player.pantsColor);
            _colorPickers[11].SetColor(_player.shoeColor);
            switch (detailColorCount)
            {
                case 1:
                    _colorPickers[12].SetColor(mrPlagueRacesPlayer.detailColor);
                    _colorPickers[13].SetColor(mrPlagueRacesPlayer.detailColor);
                    _colorPickers[14].SetColor(mrPlagueRacesPlayer.detailColor);
                    break;
                case 2:
                    _colorPickers[12].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                    _colorPickers[13].SetColor(mrPlagueRacesPlayer.detailColor);
                    _colorPickers[14].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                    break;
                case 3:
                    _colorPickers[12].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                    _colorPickers[13].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor2);
                    _colorPickers[14].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor2);
                    break;
                case 4:
                    _colorPickers[12].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                    _colorPickers[13].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor2);
                    _colorPickers[14].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor3);
                    break;
            }
        }

        private UIRaceColoredImageButton CreateColorPicker(CategoryId id, string texturePath, float xPositionStart, float xPositionPerId, bool shouldBeSmall = false, bool xInvert = false, bool yInvert = false)
        {
            UIRaceColoredImageButton UIRaceColoredImageButton = new UIRaceColoredImageButton(ModContent.Request<Texture2D>(texturePath, (AssetRequestMode)1), shouldBeSmall, xInvert, yInvert);
            _colorPickers[(int)id] = UIRaceColoredImageButton;
            UIRaceColoredImageButton.VAlign = 0f;
            UIRaceColoredImageButton.HAlign = 0f;
            UIRaceColoredImageButton.Left.Set(xPositionStart + (float)id * xPositionPerId, 0.5f);
            UIRaceColoredImageButton.Top.Set(4f, 0f);
            UIRaceColoredImageButton.OnMouseOver += ShowOptionDescription;
            UIRaceColoredImageButton.OnMouseOut += ClearOptionDescription;
            UIRaceColoredImageButton.OnLeftMouseDown += Click_ColorPicker;
            UIRaceColoredImageButton.SetSnapPoint("Top", (int)id);
            if (shouldBeSmall)
            {
                if (xInvert)
                {
                    UIRaceColoredImageButton.Left = StyleDimension.FromPixelsAndPercent(364f - 61f, 0f);
                    UIRaceColoredImageButton.Top = StyleDimension.FromPixelsAndPercent(yInvert ? 21.99f + 4f : 4f, 0f);
                }
                else
                {
                    UIRaceColoredImageButton.Left = _colorPickers[(int)CategoryId.Detail].Left;
                    UIRaceColoredImageButton.Top = StyleDimension.FromPixelsAndPercent(yInvert ? 21.99f + 4f : 4f, 0f);
                }
            }
            return UIRaceColoredImageButton;
        }

        private UIRaceColoredImageButton CreatePickerWithoutClick(CategoryId id, string texturePath, float xPositionStart, float xPositionPerId)
        {
            UIRaceColoredImageButton UIRaceColoredImageButton = new UIRaceColoredImageButton(ModContent.Request<Texture2D>(texturePath, (AssetRequestMode)1));
            UIRaceColoredImageButton.VAlign = 0f;
            UIRaceColoredImageButton.HAlign = 0f;
            UIRaceColoredImageButton.Left.Set(xPositionStart + (float)id * xPositionPerId, 0.5f);
            UIRaceColoredImageButton.OnMouseOver += ShowOptionDescription;
            UIRaceColoredImageButton.OnMouseOut += ClearOptionDescription;
            return UIRaceColoredImageButton;
        }

        private UIColoredImageButtonSmallFix CreatePickerWithoutClick_Original(CategoryId id, string texturePath, float xPositionStart, float xPositionPerId)
        {
            UIColoredImageButtonSmallFix UIColoredImageButtonSmallFix = new UIColoredImageButtonSmallFix(ModContent.Request<Texture2D>(texturePath, (AssetRequestMode)1));
            UIColoredImageButtonSmallFix.VAlign = 0f;
            UIColoredImageButtonSmallFix.HAlign = 0f;
            UIColoredImageButtonSmallFix.Left.Set(xPositionStart + (float)id * xPositionPerId, 0.5f);
            return UIColoredImageButtonSmallFix;
        }

        private void Click_ClothStyles(UIMouseEvent evt, UIElement listeningElement)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            SoundEngine.PlaySound(SoundID.MenuTick);
            UnselectAllCategories();
            _selectedPicker = CategoryId.Clothing;
            colorPickerPanel.Append(_clothStylesContainer);
            _familiarShirtButton.SetColor(_player.shirtColor);
            _familiarPantsButton.SetColor(_player.pantsColor);
            _censorClothingButton.SetColor(_player.pantsColor);
            _detailCountButton.SetColors(mrPlagueRacesPlayer.detailColor, mrPlagueRacesPlayer.auxilaryDetailColor1, mrPlagueRacesPlayer.auxilaryDetailColor2, mrPlagueRacesPlayer.auxilaryDetailColor3, detailColorCount);
            _clothingStylesCategoryButton.SetSelected(selected: true, shouldStartHovering: true);
        }

        private void Click_HairStyles(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            UnselectAllCategories();
            MakeHairstylesMenu(colorPickerPanel);
            colorPickerPanel.Append(_hairstylesContainer);
            _selectedPicker = CategoryId.HairStyle;
            _hairStylesCategoryButton.SetSelected(selected: true, shouldStartHovering: true);
        }

        private void Click_ColorPicker(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            UnselectAllCategories();
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
            if (text == 12)
            {
                switch (detailColorCount)
                {
                    case 1:
                        SelectColorPicker((CategoryId)7);
                        break;
                    default:
                        SelectColorPicker((CategoryId)text);
                        break;
                }
            }
            else if (text == 13)
            {
                switch (detailColorCount)
                {
                    case 1:
                        SelectColorPicker((CategoryId)7);
                        break;
                    case 2:
                        SelectColorPicker((CategoryId)7);
                        break;
                    default:
                        SelectColorPicker((CategoryId)text);
                        break;
                }
            }
            else if (text == 14)
            {
                switch (detailColorCount)
                {
                    case 1:
                        SelectColorPicker((CategoryId)7);
                        break;
                    case 2:
                        SelectColorPicker((CategoryId)12);
                        break;
                    case 3:
                        SelectColorPicker((CategoryId)13);
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

        private static Color ScaledHslToRgb(Vector3 hsl)
        {
            return ScaledHslToRgb(hsl.X, hsl.Y, hsl.Z);
        }

        private static Color ScaledHslToRgb(float hue, float saturation, float luminosity)
        {
            return Main.hslToRgb(hue, saturation, luminosity * 0.85f + 0.15f);
        }

        private static Vector3 RgbToScaledHsl(Color color)
        {
            Vector3 modPrefix = Main.rgbToHsl(color);
            modPrefix.Z = (modPrefix.Z - 0.15f) / 0.85f;
            modPrefix = Vector3.Clamp(modPrefix, Vector3.Zero, Vector3.One);
            return modPrefix;
        }

        private void UnselectAllCategories()
        {
            UIRaceColoredImageButton[] colorPickers = _colorPickers;
            for (int i = 0; i < colorPickers.Length; i++)
            {
                colorPickers[i]?.SetSelected(selected: false);
            }
            _clothingStylesCategoryButton.SetSelected(selected: false);
            _hairStylesCategoryButton.SetSelected(selected: false);
            _hslContainer.Remove();
            _hairstylesContainer.Remove();
            _clothStylesContainer.Remove();
        }

        private void SelectColorPicker(CategoryId selection, bool shouldStartHovering = true)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            _selectedPicker = selection;
            switch (selection)
            {
                case CategoryId.CharInfo:
                    return;
                case CategoryId.Clothing:
                    Click_ClothStyles(null, null);
                    return;
                case CategoryId.HairStyle:
                    Click_HairStyles(null, null);
                    return;
            }
            colorPickerPanel.Append(_hslContainer);
            for (int i = 0; i < _colorPickers.Length; i++)
            {
                if (_colorPickers[i] != null)
                {
                    _colorPickers[i].SetSelected(i == (int)selection, i == (int)selection && shouldStartHovering);
                }
            }
            Vector3 currentColorHSL = Vector3.One;
            switch (selection)
            {
                case CategoryId.HairColor:
                    currentColorHSL = RgbToScaledHsl(_player.hairColor);
                    break;
                case CategoryId.Eye:
                    currentColorHSL = RgbToScaledHsl(_player.eyeColor);
                    break;
                case CategoryId.Skin:
                    currentColorHSL = RgbToScaledHsl(_player.skinColor);
                    break;
                case CategoryId.Detail:
                    currentColorHSL = RgbToScaledHsl(mrPlagueRacesPlayer.detailColor);
                    break;
                case CategoryId.Shirt:
                    currentColorHSL = RgbToScaledHsl(_player.shirtColor);
                    break;
                case CategoryId.Undershirt:
                    currentColorHSL = RgbToScaledHsl(_player.underShirtColor);
                    break;
                case CategoryId.Pants:
                    currentColorHSL = RgbToScaledHsl(_player.pantsColor);
                    break;
                case CategoryId.Shoes:
                    currentColorHSL = RgbToScaledHsl(_player.shoeColor);
                    break;
                case CategoryId.DetailAux1:
                    switch (detailColorCount)
                    {
                        case 1:
                            _colorPickers[12].SetColor(mrPlagueRacesPlayer.detailColor);
                            _colorPickers[13].SetColor(mrPlagueRacesPlayer.detailColor);
                            _colorPickers[14].SetColor(mrPlagueRacesPlayer.detailColor);
                            break;
                        case 2:
                            _colorPickers[12].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                            _colorPickers[13].SetColor(mrPlagueRacesPlayer.detailColor);
                            _colorPickers[14].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                            break;
                        case 3:
                            _colorPickers[12].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                            _colorPickers[13].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor2);
                            _colorPickers[14].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor2);
                            break;
                        case 4:
                            _colorPickers[12].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor1);
                            _colorPickers[13].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor2);
                            _colorPickers[14].SetColor(mrPlagueRacesPlayer.auxilaryDetailColor3);
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
            }
            _currentColorHSL = currentColorHSL;
            UpdateHexText(ScaledHslToRgb(currentColorHSL.X, currentColorHSL.Y, currentColorHSL.Z));
        }

        public void SetUpColorPalette()
        {
            mainColorPalettePanel = new UIPanel();
            mainColorPalettePanel.SetPadding(0f);
            mainColorPalettePanel.Width.Set(0f, 1f);
            mainColorPalettePanel.Height.Set(400f, 0f);
            mainColorPalettePanel.BackgroundColor = Color.Transparent;
            mainColorPalettePanel.BorderColor = Color.Transparent;

            colorPickerPanel = new UIPanel();
            colorPickerPanel.SetPadding(0f);
            colorPickerPanel.Width.Set(0f, 1f);
            colorPickerPanel.Height.Set(350f, 0f);
            colorPickerPanel.Top = StyleDimension.FromPixels(44f);
            colorPickerPanel.BackgroundColor = Color.Transparent;
            colorPickerPanel.BorderColor = Color.Transparent;
            mainColorPalettePanel.Append(colorPickerPanel);

            MakeHSLMenu(colorPickerPanel);
            MakeHairstylesMenu(colorPickerPanel);
            MakeClothStylesMenu(colorPickerPanel);

            float i = -288f - 78f + 41f;
            float text = 48f;
            _colorPickers = new UIRaceColoredImageButton[15];
            mainColorPalettePanel.Append(CreateColorPicker(CategoryId.HairColor, "MrPlagueRaces/Assets/Textures/UI/ColorHair", i, text));
            mainColorPalettePanel.Append(CreateColorPicker(CategoryId.Eye, "MrPlagueRaces/Assets/Textures/UI/ColorEye", i, text));
            mainColorPalettePanel.Append(CreateColorPicker(CategoryId.Skin, "MrPlagueRaces/Assets/Textures/UI/ColorSkin", i, text));
            mainColorPalettePanel.Append(CreateColorPicker(CategoryId.Detail, "MrPlagueRaces/Assets/Textures/UI/ColorDetailQuarter", i, text, true));
            mainColorPalettePanel.Append(CreateColorPicker(CategoryId.Shirt, "MrPlagueRaces/Assets/Textures/UI/ColorShirt", i, text));
            mainColorPalettePanel.Append(CreateColorPicker(CategoryId.Undershirt, "MrPlagueRaces/Assets/Textures/UI/ColorUndershirt", i, text));
            mainColorPalettePanel.Append(CreateColorPicker(CategoryId.Pants, "MrPlagueRaces/Assets/Textures/UI/ColorPants", i, text));
            mainColorPalettePanel.Append(CreateColorPicker(CategoryId.Shoes, "MrPlagueRaces/Assets/Textures/UI/ColorShoes", i, text));
            mainColorPalettePanel.Append(CreateColorPicker(CategoryId.DetailAux1, "MrPlagueRaces/Assets/Textures/UI/ColorDetailQuarterOpposite", i, text, true, true));
            mainColorPalettePanel.Append(CreateColorPicker(CategoryId.DetailAux2, "MrPlagueRaces/Assets/Textures/UI/ColorDetailQuarterBottom", i, text, true, false, true));
            mainColorPalettePanel.Append(CreateColorPicker(CategoryId.DetailAux3, "MrPlagueRaces/Assets/Textures/UI/ColorDetailQuarterBottomOpposite", i, text, true, true, true));
            _colorPickers[5].SetMiddleTexture(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/ColorEyeBack", (AssetRequestMode)1));
            _clothingStylesCategoryButton = CreatePickerWithoutClick(CategoryId.Clothing, _player.Male ? "MrPlagueRaces/Assets/Textures/UI/ClothStyleMale" : "MrPlagueRaces/Assets/Textures/UI/ClothStyleFemale", i, text);
            _clothingStylesCategoryButton.OnLeftMouseDown += Click_ClothStyles;
            _clothingStylesCategoryButton.SetSnapPoint("Top", 2);
            _clothingStylesCategoryButton.Top.Set(4f, 0f);
            mainColorPalettePanel.Append(_clothingStylesCategoryButton);
            _hairStylesCategoryButton = CreatePickerWithoutClick(CategoryId.HairStyle, "MrPlagueRaces/Assets/Textures/UI/Style_Hair", i, text);
            _hairStylesCategoryButton.OnLeftMouseDown += Click_HairStyles;
            _hairStylesCategoryButton.SetMiddleTexture(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Style_Arrow", (AssetRequestMode)1));
            _hairStylesCategoryButton.SetSnapPoint("Top", 3);
            _hairStylesCategoryButton.Top.Set(4f, 0f);
            mainColorPalettePanel.Append(_hairStylesCategoryButton);

            UnselectAllCategories();
            UpdateColorPickers();
            _selectedPicker = CategoryId.Clothing;
            colorPickerPanel.Append(_clothStylesContainer);
            _clothingStylesCategoryButton.SetSelected(selected: true, shouldStartHovering: false);
        }

        public void SetUpAppearance()
        {
            ResetSheetList();

            mainAppearancePanel = new UIPanel();
            mainAppearancePanel.SetPadding(0f);
            mainAppearancePanel.Width.Set(0f, 1f);
            mainAppearancePanel.Height.Set(285f, 0f);
            mainAppearancePanel.BackgroundColor = Color.Transparent;
            mainAppearancePanel.BorderColor = Color.Transparent;
            AddSheetToPlayerAndUI(0, "MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/Arms", SheetCategory.Arms, 0, -1, 0, 0);
            AddSheetToPlayerAndUI(1, "MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/Body", SheetCategory.Body, 0, -1, 0, 0);
            AddSheetToPlayerAndUI(2, "MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/EyeLids", SheetCategory.EyeLids, 0, -1, 0, 0);
            AddSheetToPlayerAndUI(3, "MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/Hands", SheetCategory.Hands, 0, -1, 0, 0);
            AddSheetToPlayerAndUI(4, "MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/Head", SheetCategory.Head, 0, -1, 0, 0);
            AddSheetToPlayerAndUI(5, "MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/Legs", SheetCategory.Legs, 0, -1, 0, 0);

            AddSheetToPlayerAndUI(6, "MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorEyes/Eyes", SheetCategory.Eyes, 6, -1, 0, 0);
            AddSheetToPlayerAndUI(7, "MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/Colorless/Eyes", SheetCategory.Eyes, 5, -1, 0, 0);

            AddSheetToPlayerAndUI(8, "MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorHair/Hairstyles/Hair_1", SheetCategory.Hair, 7, 0, 0, 0);
            AddSheetToPlayerAndUI(9, "MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorHair/Hairstyles/Hair_2", SheetCategory.Hair, 7, 1, 0, 0);
            AddSheetToPlayerAndUI(10, "MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorHair/Hairstyles/Hair_3", SheetCategory.Hair, 7, 2, 0, 0);

            AddSheetToPlayerAndUI(11, "MrPlagueRaces/Assets/Textures/Players/Races/Human/Female/ColorSkin/Legs", SheetCategory.Legs, 0, -1, 0, 1);
        }

        public void SetUpAbilities()
        {
            Asset<Texture2D>[] statImage =
            {
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/Health"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/HealthRegen"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/Mana"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/ManaRegen"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/ManaCost"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/Defense"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/Endurance"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/Thorns"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/LavaMax"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MeleeDamage"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MeleeSpeed"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/RangedDamage"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MagicDamage"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/SummonDamage"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MaxMinions"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MinionKnockback"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MaxTurrets"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/AllDamage"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MeleeCrit"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/RangedCrit"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MagicCrit"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/AllCrit"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/ArmorPenetration"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/PickSpeed"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/TileSpeed"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/WallSpeed"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/BlockRange"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MoveSpeed"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/JumpSpeed"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/FallResistance"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/FishingSkill"),
                ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/Aggro")
            };
            string[] statHoverText =
            {
                $"[c/3BC9EF:Health.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Health Regeneration.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Mana.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Mana Regeneration.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Mana Cost.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Defense.] LMB/RMB to increment & decrement. WARNING: This stat is buggy, modify Endurance instead!",
                $"[c/3BC9EF:Endurance.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Thorns.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Lava Immunity Time.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Melee Damage.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Melee Speed.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Ranged Damage.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Magic Damage.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Summon Damage.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Minions.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Minion Knockback.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Turrets.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Damage.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Melee Critical Strike Chance.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Ranged Critical Strike Chance.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Magic Critical Strike Chance.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Critical Strike Chance.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Armor Penetration.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Mining Delay.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Tile Placement Speed.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Wall Placement Speed.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Placement Range.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Movement Speed.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Jump Speed.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Fall Damage Resistance.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Fishing Skill.] LMB/RMB to increment & decrement.",
                $"[c/3BC9EF:Aggro.] LMB/RMB to increment & decrement."
            };
            bool[] statIsNegative =
            {
                false,
                false,
                false,
                false,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false
            };
            mainAbilitiesPanel = new UIPanel();
            mainAbilitiesPanel.SetPadding(0f);
            mainAbilitiesPanel.Width.Set(0f, 1f);
            mainAbilitiesPanel.Height.Set(285f + 50f + 35f, 0f);
            mainAbilitiesPanel.BackgroundColor = Color.Transparent;
            mainAbilitiesPanel.BorderColor = Color.Transparent;

            int buttonCount = 0;
            for (int i = 0; i < 32; i++)
            {
                UIRaceStatConfigurePanel statPanel = new UIRaceStatConfigurePanel(statImage[i], ref _descriptionText, statHoverText[i], i, statIsNegative[i])
                {
                    Width = StyleDimension.FromPixels(130f),
                    Height = StyleDimension.FromPixels(30f),
                    Left = StyleDimension.FromPixels(2f + ((float)(buttonCount % 4) * 134f)),
                    Top = StyleDimension.FromPixels(4f + ((float)(buttonCount / 4) * 35f))
                };
                mainAbilitiesPanel.Append(statPanel);
                buttonCount += 1;
            }

            UIRaceDefaultAbilityPanel defaultAbilityPanel = new UIRaceDefaultAbilityPanel(ref _descriptionText)
            {
                Width = StyleDimension.FromPixels(180f),
                Height = StyleDimension.FromPixels(30f),
                HAlign = 0.5f,
                Top = StyleDimension.FromPixels(4f + ((float)(buttonCount / 4) * 35f))
            };
            mainAbilitiesPanel.Append(defaultAbilityPanel);

            UIRaceDefaultAttributePanel defaultAttributePanel = new UIRaceDefaultAttributePanel(ref _descriptionText)
            {
                Width = StyleDimension.FromPixels(275f),
                Height = StyleDimension.FromPixels(30f),
                HAlign = 0.5f,
                Top = StyleDimension.FromPixels((4f + ((float)(buttonCount / 4) * 35f) + 35f))
            };
            mainAbilitiesPanel.Append(defaultAttributePanel);
        }

        public void SetUpRaceInfo()
        {
            float top = 4f;

            mainRaceInfoPanel = new UIPanel();
            mainRaceInfoPanel.SetPadding(0f);
            mainRaceInfoPanel.Width.Set(0f, 1f);
            mainRaceInfoPanel.Height.Set(230f + 46f + 36f + 4f, 0f);
            mainRaceInfoPanel.BackgroundColor = Color.Transparent;
            mainRaceInfoPanel.BorderColor = Color.Transparent;

            _modName = createAndAppendTextInputWithLabel("Internal Mod Name (no spaces)", "Type here", ref _modNamePanel, ref _modNameSubPanel);
            _modName.OnTextChange += delegate
            {
                _modName.SetText(_modName.CurrentString.Replace(" ", ""));
            };
            _raceName = createAndAppendTextInputWithLabel("Internal Race Name (no spaces)", "Type Here", ref _raceNamePanel, ref _raceNameSubPanel);
            _modDisplayName = createAndAppendTextInputWithLabel("Mod Display Name", "Type here", ref _modDisplayNamePanel, ref _modDisplayNameSubPanel);
            _raceDisplayName = createAndAppendTextInputWithLabel("Race Display Name", "Leave Blank to Skip", ref _raceDisplayNamePanel, ref _raceDisplayNameSubPanel);
            _modAuthor = createAndAppendTextInputWithLabel("Mod Author", "Type here", ref _modAuthorPanel, ref _modAuthorSubPanel);
            _modName.OnTab += delegate
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                _modDisplayName.Focused = true;
            };
            _modDisplayName.OnTab += delegate
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                _modAuthor.Focused = true;
            };
            _modAuthor.OnTab += delegate
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                _raceName.Focused = true;
            };
            _raceName.OnTab += delegate
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                _raceDisplayName.Focused = true;
            };
            _raceDisplayName.OnTab += delegate
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                _modName.Focused = true;
            };

            UIRaceDrawTitlePanel drawTitlePanel = new UIRaceDrawTitlePanel(ref _descriptionText)
            {
                Width = StyleDimension.FromPixels(260f),
                Height = StyleDimension.FromPixels(30f),
                HAlign = 0.5f,
                Top = StyleDimension.FromPixels(top)
            };
            mainRaceInfoPanel.Append(drawTitlePanel);
            UIRaceDrawPlayerPanel drawPlayerPanel = new UIRaceDrawPlayerPanel(ref _descriptionText)
            {
                Width = StyleDimension.FromPixels(270f),
                Height = StyleDimension.FromPixels(30f),
                HAlign = 0.5f,
                Top = StyleDimension.FromPixels(top + 36f)
            };
            mainRaceInfoPanel.Append(drawPlayerPanel);
            UIFocusInputTextField_mp createAndAppendTextInputWithLabel(string label, string hint, ref UIPanel backgroundPanel, ref UIPanel textBoxPanel)
            {
                UIPanel panel = new UIPanel();
                panel.SetPadding(0f);
                panel.Width.Set(0f, 1f);
                panel.Height.Set(40f, 0f);
                panel.Top.Set(top, 0f);
                top += 46f;
                UIText modNameText = new UIText(label)
                {
                    Left =
                    {
                        Pixels = 10f
                    },
                    Top =
                    {
                        Pixels = 10f
                    }
                };
                panel.BorderColor = new Color(89, 116, 213) * 0.7f;
                panel.Append(modNameText);
                UIPanel textBoxBackground = new UIPanel();
                textBoxBackground.SetPadding(0f);
                textBoxBackground.Top.Set(6f, 0f);
                textBoxBackground.Left.Set(0f, 0.5f);
                textBoxBackground.Width.Set(0f, 0.5f);
                textBoxBackground.Height.Set(30f, 0f);
                textBoxBackground.BackgroundColor = new Color(31, 41, 76);
                textBoxBackground.BorderColor = new Color(82, 96, 145);
                textBoxPanel = textBoxBackground;
                panel.Append(textBoxBackground);
                UIFocusInputTextField_mp uIInputTextField = new UIFocusInputTextField_mp(hint)
                {
                    UnfocusOnTab = true
                };
                uIInputTextField.Top.Set(5f, 0f);
                uIInputTextField.Left.Set(10f, 0f);
                uIInputTextField.Width.Set(-20f, 1f);
                uIInputTextField.Height.Set(20f, 0f);
                uIInputTextField.OnMouseOver += ShowOptionDescription;
                uIInputTextField.OnMouseOut += ClearOptionDescription;
                uIInputTextField.OnMouseOver += FadedMouseOver;
                uIInputTextField.OnMouseOut += FadedMouseOut;
                uIInputTextField.OnLeftMouseDown += PlayMenuTick;
                textBoxBackground.Append(uIInputTextField);
                mainRaceInfoPanel.Append(panel);
                backgroundPanel = panel;
                return uIInputTextField;
            }
        }

        public void ShowOptionDescription(UIMouseEvent evt, UIElement listeningElement)
        {
            LocalizedText localizedText = null;
            if (listeningElement == _familiarShirtButton)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.FamiliarShirt");
            }
            if (listeningElement == _familiarPantsButton)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.FamiliarPants");
            }
            if (listeningElement == _censorClothingButton)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.CensorClothing");
            }
            if (listeningElement == _detailCountButton)
            {
                localizedText = Language.GetText($"Mods.MrPlagueRaces.UI.DetailCount_{detailColorCount}");
            }
            if (listeningElement == _clothingStylesCategoryButton)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ClothingStyles");
            }
            if (listeningElement == _hairStylesCategoryButton)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.HairStyles");
            }
            if (listeningElement == _colorPickers[4])
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ColorPickers4");
            }
            if (listeningElement == _colorPickers[5])
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ColorPickers5");
            }
            if (listeningElement == _colorPickers[6])
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ColorPickers6");
            }
            if (listeningElement == _colorPickers[7])
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ColorPickers7");
            }
            if (listeningElement == _colorPickers[8])
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ColorPickers8");
            }
            if (listeningElement == _colorPickers[9])
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ColorPickers9");
            }
            if (listeningElement == _colorPickers[10])
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ColorPickers10");
            }
            if (listeningElement == _colorPickers[11])
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ColorPickers11");
            }
            if (listeningElement == _colorPickers[12])
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ColorPickers12");
            }
            if (listeningElement == _colorPickers[13])
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ColorPickers13");
            }
            if (listeningElement == _colorPickers[14])
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ColorPickers14");
            }
            if (listeningElement == buttonBack)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ButtonBack");
            }
            if (listeningElement == buttonCreate)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ButtonCreate");
            }
            if (listeningElement == colorPaletteButton)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ColorPalette");
            }
            if (listeningElement == appearanceButton)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.Appearance");
            }
            if (listeningElement == abilitiesButton)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.Abilities");
            }
            if (listeningElement == raceInfoButton)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.RaceInfo");
            }
            if (listeningElement == _modName)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ModName");
            }
            if (listeningElement == _modDisplayName)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ModDisplayName");
            }
            if (listeningElement == _modAuthor)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.ModAuthor");
            }
            if (listeningElement == _raceName)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.RaceName");
            }
            if (listeningElement == _raceDisplayName)
            {
                localizedText = Language.GetText("Mods.MrPlagueRaces.UI.RaceDisplayName");
            }
            if (localizedText != null)
            {
                _descriptionText.SetText(localizedText);
            }
        }

        private void ResetButtons()
        {
            colorPaletteButton.SetSelected(false);
            appearanceButton.SetSelected(false);
            abilitiesButton.SetSelected(false);
            raceInfoButton.SetSelected(false);
        }

        private void Click_ColorPalette(UIMouseEvent evt, UIElement listeningElement)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            SoundEngine.PlaySound(SoundID.MenuTick);
            ResetButtons();
            colorPaletteButton.SetSelected(true, true);
            SwitchMenu(mainColorPalettePanel);
            _familiarShirtButton.SetColor(_player.shirtColor);
            _familiarPantsButton.SetColor(_player.pantsColor);
            _censorClothingButton.SetColor(_player.pantsColor);
            _detailCountButton.SetColors(mrPlagueRacesPlayer.detailColor, mrPlagueRacesPlayer.auxilaryDetailColor1, mrPlagueRacesPlayer.auxilaryDetailColor2, mrPlagueRacesPlayer.auxilaryDetailColor3, detailColorCount);
            _familiarShirtButton.SetSelected(familiarShirtEnabled);
            _familiarPantsButton.SetSelected(familiarPantsEnabled);
            _censorClothingButton.SetSelected(censorClothingEnabled);
            if (_selectedPicker == CategoryId.HairStyle)
            {
                UnselectAllCategories();
                MakeHairstylesMenu(colorPickerPanel);
                _hairStylesCategoryButton.SetSelected(selected: true, shouldStartHovering: false);
            }
        }

        private void Click_Appearance(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            ResetButtons();
            ReloadSheetList();
            appearanceButton.SetSelected(true, true);
            SwitchMenu(mainAppearancePanel);
        }

        private void Click_Abilities(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            ResetButtons();
            abilitiesButton.SetSelected(true, true);
            SwitchMenu(mainAbilitiesPanel);
        }

        private void Click_RaceInfo(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            ResetButtons();
            raceInfoButton.SetSelected(true, true);
            SwitchMenu(mainRaceInfoPanel);
        }

        private void SwitchMenu(UIPanel uIPanel)
        {
            modularList.Clear();
            modularList.Add(uIPanel);
        }

        private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (listeningElement == _modName)
            {
                _modNameSubPanel.BorderColor = Colors.FancyUIFatButtonMouseOver;
            }
            else if (listeningElement == _modDisplayName)
            {
                _modDisplayNameSubPanel.BorderColor = Colors.FancyUIFatButtonMouseOver;
            }
            else if (listeningElement == _modAuthor)
            {
                _modAuthorSubPanel.BorderColor = Colors.FancyUIFatButtonMouseOver;
            }
            else if (listeningElement == _raceName)
            {
                _raceNameSubPanel.BorderColor = Colors.FancyUIFatButtonMouseOver;
            }
            else if (listeningElement == _raceDisplayName)
            {
                _raceDisplayNameSubPanel.BorderColor = Colors.FancyUIFatButtonMouseOver;
            }
            else
            {
                ((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
                ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
            }
        }

        private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            if (listeningElement == _modName)
            {
                _modNameSubPanel.BorderColor = new Color(82, 96, 145);
            }
            else if (listeningElement == _modDisplayName)
            {
                _modDisplayNameSubPanel.BorderColor = new Color(82, 96, 145);
            }
            else if (listeningElement == _modAuthor)
            {
                _modAuthorSubPanel.BorderColor = new Color(82, 96, 145);
            }
            else if (listeningElement == _raceName)
            {
                _raceNameSubPanel.BorderColor = new Color(82, 96, 145);
            }
            else if (listeningElement == _raceDisplayName)
            {
                _raceDisplayNameSubPanel.BorderColor = new Color(82, 96, 145);
            }
            else
            {
                ((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.8f;
                ((UIPanel)evt.Target).BorderColor = Color.Black;
            }
        }

        private void PlayMenuTick(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        }


        public void ClearOptionDescription(UIMouseEvent evt, UIElement listeningElement)
        {
            _descriptionText.SetText(Language.GetText("Workshop.HubDescriptionDefault"));
        }

        private void MakeCharPreview(UIPanel container)
        {
            // {T} Removed the loop here as well, since it was just creating duplicate UICharacter elements with the exact same parameters.
            UICharacter element = new UICharacter(_player, animated: true, hasBackPanel: false, 1.5f)
            {
                Width = StyleDimension.FromPixels(80f),
                Height = StyleDimension.FromPixelsAndPercent(80f, 0f),
                Top = StyleDimension.FromPixelsAndPercent(-70f, 0f),
                VAlign = 0f,
                HAlign = 0.5f
            };
            container.Append(element);
        }

        private void AddDescriptionPanel(UIElement container, float accumulatedHeight, float height, string tagGroup)
        {
            float num = 0f;
            UISlicedImage uISlicedImage = new UISlicedImage(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight"))
            {
                HAlign = 0.5f,
                VAlign = 1f,
                Width = StyleDimension.FromPixelsAndPercent((0f - num) * 2f, 1f),
                Left = StyleDimension.FromPixels(0f - num),
                Height = StyleDimension.FromPixelsAndPercent(height, 0f),
                Top = StyleDimension.FromPixels(2f)
            };
            uISlicedImage.SetSliceDepths(10);
            uISlicedImage.Color = Color.LightGray * 0.7f;
            container.Append(uISlicedImage);
            UIText uIText = new UIText(Language.GetText("Workshop.HubDescriptionDefault"))
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(5f, 0f)
            };
            uIText.PaddingLeft = 20f;
            uIText.PaddingRight = 20f;
            uIText.PaddingTop = 6f;
            uIText.IsWrapped = true;
            uISlicedImage.Append(uIText);
            _descriptionText = uIText;
            UIHorizontalSeparator secondSeparator = new UIHorizontalSeparator
            {
                Left = StyleDimension.FromPixelsAndPercent(0f, 0.01f),
                VAlign = 1f,
                Top = StyleDimension.FromPixels(2f - height - 8f),
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.98f),
                Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
            };
            container.Append(secondSeparator);
        }

        public override void OnActivate()
        {
            base.OnActivate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        private void BackClick(UIMouseEvent evt, UIElement listeningElement)
        {
            HandleBackButtonUsage();
        }

        public void HandleBackButtonUsage()
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            mrPlagueRacesPlayer.race.CensorClothing = true;
            mrPlagueRacesPlayer.race.ReloadSheetsFromDefaultFilepath();
            SoundEngine.PlaySound(in SoundID.MenuClose);
            _cancelAction();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            HasModifiedLegsSheet = mrPlagueRacesPlayer.race.GetRaceSheetFromIDs(SheetCategory.Legs, 0, -1, 0).OverrideTexture[0] != ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/Legs").Value && mrPlagueRacesPlayer.race.GetRaceSheetFromIDs(SheetCategory.Legs, 0, -1, 0).OverrideTexture[1] != ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Players/Races/Human/Female/ColorSkin/Legs").Value;
            HasModifiedBodySheet = mrPlagueRacesPlayer.race.GetRaceSheetFromIDs(SheetCategory.Body, 0, -1, 0).OverrideTexture[0] != ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/Body").Value;
            if (!HasModifiedBodySheet || !HasModifiedLegsSheet)
            {
                censorClothingEnabled = true;
                _censorClothingButton.SetSelected(censorClothingEnabled);

            }
            if (censorClothingEnabled)
            {
                mrPlagueRacesPlayer.race.CensorClothing = true;
            }
            else
            {
                mrPlagueRacesPlayer.race.CensorClothing = false;
            }
            if (familiarShirtEnabled)
            {
                _player.armor[11] = familiarShirt;
            }
            else
            {
                _player.armor[11] = blankItem;
            }
            if (familiarPantsEnabled)
            {
                _player.armor[12] = familiarPants;
            }
            else
            {
                _player.armor[12] = blankItem;
            }
            if (detailColorCount == 1)
            {
                _colorPickers[7].SetSelected(_colorPickers[7].IsSelected() || _colorPickers[12].IsSelected() || _colorPickers[13].IsSelected() || _colorPickers[14].IsSelected(), _colorPickers[7].IsHovered() || _colorPickers[12].IsHovered() || _colorPickers[13].IsHovered() || _colorPickers[14].IsHovered());
                _colorPickers[12].SetSelected(_colorPickers[7].IsSelected() || _colorPickers[12].IsSelected() || _colorPickers[13].IsSelected() || _colorPickers[14].IsSelected(), _colorPickers[7].IsHovered() || _colorPickers[12].IsHovered() || _colorPickers[13].IsHovered() || _colorPickers[14].IsHovered());
                _colorPickers[13].SetSelected(_colorPickers[7].IsSelected() || _colorPickers[12].IsSelected() || _colorPickers[13].IsSelected() || _colorPickers[14].IsSelected(), _colorPickers[7].IsHovered() || _colorPickers[12].IsHovered() || _colorPickers[13].IsHovered() || _colorPickers[14].IsHovered());
                _colorPickers[14].SetSelected(_colorPickers[7].IsSelected() || _colorPickers[12].IsSelected() || _colorPickers[13].IsSelected() || _colorPickers[14].IsSelected(), _colorPickers[7].IsHovered() || _colorPickers[12].IsHovered() || _colorPickers[13].IsHovered() || _colorPickers[14].IsHovered());

            }
            else if (detailColorCount == 2)
            {
                _colorPickers[7].SetSelected(_colorPickers[7].IsSelected() || _colorPickers[13].IsSelected(), _colorPickers[7].IsHovered() || _colorPickers[13].IsHovered());
                _colorPickers[13].SetSelected(_colorPickers[7].IsSelected() || _colorPickers[13].IsSelected(), _colorPickers[7].IsHovered() || _colorPickers[13].IsHovered());
                _colorPickers[12].SetSelected(_colorPickers[12].IsSelected() || _colorPickers[14].IsSelected(), _colorPickers[12].IsHovered() || _colorPickers[14].IsHovered());
                _colorPickers[14].SetSelected(_colorPickers[12].IsSelected() || _colorPickers[14].IsSelected(), _colorPickers[12].IsHovered() || _colorPickers[14].IsHovered());

            }
            else if (detailColorCount == 3)
            {
                _colorPickers[7].SetSelected(_colorPickers[7].IsSelected(), _colorPickers[7].IsHovered());
                _colorPickers[12].SetSelected(_colorPickers[12].IsSelected(), _colorPickers[12].IsHovered());
                _colorPickers[13].SetSelected(_colorPickers[13].IsSelected() || _colorPickers[14].IsSelected(), _colorPickers[13].IsHovered() || _colorPickers[14].IsHovered());
                _colorPickers[14].SetSelected(_colorPickers[13].IsSelected() || _colorPickers[14].IsSelected(), _colorPickers[13].IsHovered() || _colorPickers[14].IsHovered());

            }
            else
            {
                _colorPickers[7].SetSelected(_colorPickers[7].IsSelected(), _colorPickers[7].IsHovered());
                _colorPickers[12].SetSelected(_colorPickers[12].IsSelected(), _colorPickers[12].IsHovered());
                _colorPickers[13].SetSelected(_colorPickers[13].IsSelected(), _colorPickers[13].IsHovered());
                _colorPickers[14].SetSelected(_colorPickers[14].IsSelected(), _colorPickers[14].IsHovered());
            }
            UILinkPointNavigator.Shortcuts.BackButtonCommand = 7;
        }

        private void OKClick(UIMouseEvent evt, UIElement listeningElement)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            try
            {
                string modNameTrimmed = _modName.CurrentString.Trim();
                string raceNameTrimmed = _raceName.CurrentString.Trim();
                string sourceFolder = Path.Combine(Path.Combine(Program.SavePathShared, "ModSources"), modNameTrimmed);
                CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
                if (Directory.Exists(sourceFolder))
                {
                    _descriptionText.SetText("folder with ModName's name already exists.");
                }
                else if (!provider.IsValidIdentifier(modNameTrimmed))
                {
                    _descriptionText.SetText("ModName is invalid. Remove spaces and numbers.");
                }
                else if (modNameTrimmed.Equals("Mod", StringComparison.InvariantCultureIgnoreCase) || modNameTrimmed.Equals("ModLoader", StringComparison.InvariantCultureIgnoreCase) || modNameTrimmed.Equals("tModLoader", StringComparison.InvariantCultureIgnoreCase))
                {
                    _descriptionText.SetText("ModName is a reserved mod name. Choose a different name.");
                }
                else if (!provider.IsValidIdentifier(raceNameTrimmed))
                {
                    _descriptionText.SetText("RaceName is invalid. Remove spaces and numbers.");
                }
                else if (string.IsNullOrWhiteSpace(_modDisplayName.CurrentString))
                {
                    _descriptionText.SetText("DisplayName can't be empty.");
                }
                else if (string.IsNullOrWhiteSpace(_modAuthor.CurrentString))
                {
                    _descriptionText.SetText("Author can't be empty.");
                }
                else if (string.IsNullOrWhiteSpace(raceNameTrimmed))
                {
                    _descriptionText.SetText("RaceName can't be empty.");
                }
                else
                {
                    mrPlagueRacesPlayer.race.CensorClothing = true;
                    Directory.CreateDirectory(sourceFolder);
                    File.WriteAllText(Path.Combine(sourceFolder, "build.txt"), GetModBuild());
                    File.WriteAllText(Path.Combine(sourceFolder, "description.txt"), GetModDescription());

                    string commonPath = Path.Combine(sourceFolder, "Common/Races/" + raceNameTrimmed);
                    string assetPath = Path.Combine(sourceFolder, "Assets/Textures/Players/Races/" + raceNameTrimmed);
                    string soundPath = Path.Combine(sourceFolder, "Assets/Sounds/Players/Races/" + raceNameTrimmed);
                    string maleAssetPath = Path.Combine(assetPath, "Male");
                    string femaleAssetPath = Path.Combine(assetPath, "Female");
                    string maleSoundPath = Path.Combine(soundPath, "Male");
                    string femaleSoundPath = Path.Combine(soundPath, "Female");

                    Directory.CreateDirectory(commonPath);
                    Directory.CreateDirectory(maleAssetPath);
                    Directory.CreateDirectory(femaleAssetPath);
                    Directory.CreateDirectory(maleSoundPath);
                    Directory.CreateDirectory(femaleSoundPath);
                    // Set up an empty mod icon. This icon will be filled any registered player sheets below
                    Texture2D icon_blank = ModContent.Request<Texture2D>("MrPlagueRaces/icon_blank", AssetRequestMode.ImmediateLoad).Value;

                    // Using this unmodified texture2d is necessary to reset the mod icon. Otherwise, it retains drawdata from the previous icon(s)
                    Texture2D icon_blank_unmodified = ModContent.Request<Texture2D>("MrPlagueRaces/icon_blank_unmodified", AssetRequestMode.ImmediateLoad).Value;
                    icon_blank = icon_blank_unmodified;
                    // In this nightmare-inducing cliff face of code, every racial sprite sheet currently applied to the player is written to the mod source. It is a little overwhelming, but it works!
                    FileStream stream = null;
                    for (int k = 0; k < 2; k++) // loop through both genders
                    {
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            if (!mrPlagueRacesPlayer.race.Hand_Sheet[i].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hand_Sheet[i].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hand_Sheet[i].OverrideTexture[0]) || k == 0))
                            {
                                Texture2D sheet = mrPlagueRacesPlayer.race.Hand_Sheet[i].OverrideTexture[k]; // Renders backhand on icon
                                icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 320);
                            }
                        }

                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Hand_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hand_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hand_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Hand_StyleSheet[i, j].OverrideTexture[k]; // Renders backHand on icon
                                    if (j == _player.hair)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 320);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Hand_Auxilary1_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hand_Auxilary1_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hand_Auxilary1_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Hand_Auxilary1_StyleSheet[i, j].OverrideTexture[k]; // Renders backHand on icon
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle1)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 320);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Hand_Auxilary2_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hand_Auxilary2_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hand_Auxilary2_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Hand_Auxilary2_StyleSheet[i, j].OverrideTexture[k]; // Renders backHand on icon
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle2)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 320);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Hand_Auxilary3_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hand_Auxilary3_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hand_Auxilary3_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Hand_Auxilary3_StyleSheet[i, j].OverrideTexture[k]; // Renders backHand on icon
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle3)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 320);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            if (!mrPlagueRacesPlayer.race.Legs_Sheet[i].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Legs_Sheet[i].OverrideTexture[1] != mrPlagueRacesPlayer.race.Legs_Sheet[i].OverrideTexture[0]) || k == 0))
                            {
                                Texture2D sheet = mrPlagueRacesPlayer.race.Legs_Sheet[i].OverrideTexture[k];
                                string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                Directory.CreateDirectory(colorPath);
                                using (stream = File.OpenWrite(Path.Combine(colorPath, "Legs" + ".png")))
                                {
                                    sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                }
                                icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Legs_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Legs_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Legs_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Legs_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Legs_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == _player.hair)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Legs_Auxilary1_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Legs_Auxilary1_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Legs_Auxilary1_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Legs_Auxilary1_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_1");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Legs_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle1)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Legs_Auxilary2_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Legs_Auxilary2_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Legs_Auxilary2_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Legs_Auxilary2_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_2");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Legs_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle2)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Legs_Auxilary3_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Legs_Auxilary3_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Legs_Auxilary3_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Legs_Auxilary3_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_3");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Legs_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle3)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        if (k == 0)
                        {
                            int clothStyle = _player.Male ? Array.IndexOf(PlayerLayerHelpers.MaleClothingIDs, _player.skinVariant) : Array.IndexOf(PlayerLayerHelpers.FemaleClothingIDs, _player.skinVariant);
                            if (censorClothingEnabled)
                            {
                                icon_blank = CombineTexture2Ds(icon_blank, ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Players/Clothes/CensorPants", (AssetRequestMode)1).Value, -1, 20, 18);
                            }
                            if (familiarPantsEnabled)
                            {
                                icon_blank = CombineTexture2Ds(icon_blank, ModContent.Request<Texture2D>($"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{clothStyle + 1}/Pants", (AssetRequestMode)1).Value, -1, 20, 18);
                                icon_blank = CombineTexture2Ds(icon_blank, ModContent.Request<Texture2D>($"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{clothStyle + 1}/Shoes", (AssetRequestMode)1).Value, -4, 20, 18);
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            if (!mrPlagueRacesPlayer.race.Body_Sheet[i].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Body_Sheet[i].OverrideTexture[1] != mrPlagueRacesPlayer.race.Body_Sheet[i].OverrideTexture[0]) || k == 0))
                            {
                                Texture2D sheet = mrPlagueRacesPlayer.race.Body_Sheet[i].OverrideTexture[k];
                                string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                Directory.CreateDirectory(colorPath);
                                using (stream = File.OpenWrite(Path.Combine(colorPath, "Body" + ".png")))
                                {
                                    sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                }
                                icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Body_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Body_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Body_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Body_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Body_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == _player.hair)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Body_Auxilary1_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Body_Auxilary1_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Body_Auxilary1_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Body_Auxilary1_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_1");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Body_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle1)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Body_Auxilary2_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Body_Auxilary2_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Body_Auxilary2_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Body_Auxilary2_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_2");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Body_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle2)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Body_Auxilary3_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Body_Auxilary3_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Body_Auxilary3_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Body_Auxilary3_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_3");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Body_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle3)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        if (k == 0)
                        {
                            int clothStyle = _player.Male ? Array.IndexOf(PlayerLayerHelpers.MaleClothingIDs, _player.skinVariant) : Array.IndexOf(PlayerLayerHelpers.FemaleClothingIDs, _player.skinVariant);
                            if (familiarShirtEnabled)
                            {

                                icon_blank = CombineTexture2Ds(icon_blank, ModContent.Request<Texture2D>($"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{clothStyle + 1}/PantsAddition", (AssetRequestMode)1).Value, -3, 20, 18);
                                icon_blank = CombineTexture2Ds(icon_blank, ModContent.Request<Texture2D>($"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{clothStyle + 1}/Undershirt", (AssetRequestMode)1).Value, -2, 20, 18);
                                icon_blank = CombineTexture2Ds(icon_blank, ModContent.Request<Texture2D>($"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{clothStyle + 1}/Shirt", (AssetRequestMode)1).Value, -3, 20, 18);
                                icon_blank = CombineTexture2Ds(icon_blank, ModContent.Request<Texture2D>($"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{clothStyle + 1}/ShirtAddition", (AssetRequestMode)1).Value, -3, 20, 18, 0, 56);
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            if (!mrPlagueRacesPlayer.race.Head_Sheet[i].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Head_Sheet[i].OverrideTexture[1] != mrPlagueRacesPlayer.race.Head_Sheet[i].OverrideTexture[0]) || k == 0))
                            {
                                Texture2D sheet = mrPlagueRacesPlayer.race.Head_Sheet[i].OverrideTexture[k];
                                string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                Directory.CreateDirectory(colorPath);
                                using (stream = File.OpenWrite(Path.Combine(colorPath, "Head" + ".png")))
                                {
                                    sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                }
                                icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Head_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Head_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Head_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Head_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Head_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == _player.hair)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Head_Auxilary1_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Head_Auxilary1_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Head_Auxilary1_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Head_Auxilary1_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_1");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Head_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle1)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Head_Auxilary2_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Head_Auxilary2_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Head_Auxilary2_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Head_Auxilary2_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_2");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Head_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle2)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Head_Auxilary3_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Head_Auxilary3_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Head_Auxilary3_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Head_Auxilary3_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_3");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Head_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle3)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            if (!mrPlagueRacesPlayer.race.Eyes_Sheet[i].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Eyes_Sheet[i].OverrideTexture[1] != mrPlagueRacesPlayer.race.Eyes_Sheet[i].OverrideTexture[0]) || k == 0))
                            {
                                Texture2D sheet = mrPlagueRacesPlayer.race.Eyes_Sheet[i].OverrideTexture[k];
                                string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                Directory.CreateDirectory(colorPath);
                                using (stream = File.OpenWrite(Path.Combine(colorPath, "Eyes" + ".png")))
                                {
                                    sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                }
                                icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Eyes_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Eyes_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Eyes_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Eyes_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Eyes_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == _player.hair)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Eyes_Auxilary1_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Eyes_Auxilary1_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Eyes_Auxilary1_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Eyes_Auxilary1_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_1");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Eyes_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle1)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Eyes_Auxilary2_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Eyes_Auxilary2_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Eyes_Auxilary2_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Eyes_Auxilary2_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_2");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Eyes_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle2)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Eyes_Auxilary3_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Eyes_Auxilary3_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Eyes_Auxilary3_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Eyes_Auxilary3_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_3");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Eyes_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle3)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            if (!mrPlagueRacesPlayer.race.EyeLids_Sheet[i].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.EyeLids_Sheet[i].OverrideTexture[1] != mrPlagueRacesPlayer.race.EyeLids_Sheet[i].OverrideTexture[0]) || k == 0))
                            {
                                Texture2D sheet = mrPlagueRacesPlayer.race.EyeLids_Sheet[i].OverrideTexture[k];
                                string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                Directory.CreateDirectory(colorPath);
                                using (stream = File.OpenWrite(Path.Combine(colorPath, "EyeLids" + ".png")))
                                {
                                    sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                }
                                icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.EyeLids_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.EyeLids_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.EyeLids_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.EyeLids_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "EyeLids_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == _player.hair)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.EyeLids_Auxilary1_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.EyeLids_Auxilary1_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.EyeLids_Auxilary1_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.EyeLids_Auxilary1_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_1");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "EyeLids_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle1)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.EyeLids_Auxilary2_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.EyeLids_Auxilary2_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.EyeLids_Auxilary2_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.EyeLids_Auxilary2_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_2");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "EyeLids_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle2)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.EyeLids_Auxilary3_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.EyeLids_Auxilary3_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.EyeLids_Auxilary3_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.EyeLids_Auxilary3_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_3");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "EyeLids_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle3)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Hair_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hair_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hair_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Hair_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Hair_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == _player.hair)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Hair_Auxilary1_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hair_Auxilary1_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hair_Auxilary1_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Hair_Auxilary1_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_1");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Hair_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle1)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Hair_Auxilary2_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hair_Auxilary2_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hair_Auxilary2_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Hair_Auxilary2_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_2");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Hair_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle2)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Hair_Auxilary3_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hair_Auxilary3_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hair_Auxilary3_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Hair_Auxilary3_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_3");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Hair_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle3)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.HairAlt_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.HairAlt_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.HairAlt_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.HairAlt_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "HairAlt_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.HairAlt_Auxilary1_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.HairAlt_Auxilary1_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.HairAlt_Auxilary1_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.HairAlt_Auxilary1_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_1");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "HairAlt_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.HairAlt_Auxilary2_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.HairAlt_Auxilary2_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.HairAlt_Auxilary2_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.HairAlt_Auxilary2_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_2");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "HairAlt_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.HairAlt_Auxilary3_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.HairAlt_Auxilary3_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.HairAlt_Auxilary3_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.HairAlt_Auxilary3_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_3");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "HairAlt_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            if (!mrPlagueRacesPlayer.race.Arm_Sheet[i].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Arm_Sheet[i].OverrideTexture[1] != mrPlagueRacesPlayer.race.Arm_Sheet[i].OverrideTexture[0]) || k == 0))
                            {
                                Texture2D sheet = mrPlagueRacesPlayer.race.Arm_Sheet[i].OverrideTexture[k];
                                string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                Directory.CreateDirectory(colorPath);
                                using (stream = File.OpenWrite(Path.Combine(colorPath, "Arms" + ".png")))
                                {
                                    sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                }
                                icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 80);
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Arm_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Arm_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Arm_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Arm_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Arms_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == _player.hair)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 80);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Arm_Auxilary1_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Arm_Auxilary1_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Arm_Auxilary1_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Arm_Auxilary1_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_1");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Arms_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle1)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 80);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Arm_Auxilary2_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Arm_Auxilary2_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Arm_Auxilary2_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Arm_Auxilary2_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_2");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Arms_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle2)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 80);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Arm_Auxilary3_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Arm_Auxilary3_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Arm_Auxilary3_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Arm_Auxilary3_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_3");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Arms_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle3)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 80);
                                    }
                                }
                            }
                        }
                        if (k == 0)
                        {
                            int clothStyle = _player.Male ? Array.IndexOf(PlayerLayerHelpers.MaleClothingIDs, _player.skinVariant) : Array.IndexOf(PlayerLayerHelpers.FemaleClothingIDs, _player.skinVariant);
                            if (familiarShirtEnabled)
                            {
                                icon_blank = CombineTexture2Ds(icon_blank, ModContent.Request<Texture2D>($"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{clothStyle + 1}/Undershirt", (AssetRequestMode)1).Value, -2, 20, 18, 80);
                                icon_blank = CombineTexture2Ds(icon_blank, ModContent.Request<Texture2D>(clothStyle == 3 ? $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{clothStyle + 1}/Shirt" : $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{clothStyle + 1}/ShirtAddition", (AssetRequestMode)1).Value, -3, 20, 18, 80, clothStyle == 3 || clothStyle == 1 ? 0 : 56);
                                icon_blank = CombineTexture2Ds(icon_blank, ModContent.Request<Texture2D>($"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{clothStyle + 1}/ShirtAddition", (AssetRequestMode)1).Value, -3, 20, 18, 0, 56);
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            if (!mrPlagueRacesPlayer.race.Hand_Sheet[i].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hand_Sheet[i].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hand_Sheet[i].OverrideTexture[0]) || k == 0))
                            {
                                Texture2D sheet = mrPlagueRacesPlayer.race.Hand_Sheet[i].OverrideTexture[k];
                                string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                Directory.CreateDirectory(colorPath);
                                using (stream = File.OpenWrite(Path.Combine(colorPath, "Hands" + ".png")))
                                {
                                    sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                }
                                icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 80);
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Hand_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hand_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hand_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Hand_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Hands_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == _player.hair)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 80);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Hand_Auxilary1_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hand_Auxilary1_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hand_Auxilary1_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Hand_Auxilary1_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_1");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Hands_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle1)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 80);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Hand_Auxilary2_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hand_Auxilary2_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hand_Auxilary2_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Hand_Auxilary2_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_2");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Hands_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle2)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 80);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < 16; i++) // loop through all 16 player colors
                        {
                            for (int j = 0; j < 165; j++)
                            {
                                if (!mrPlagueRacesPlayer.race.Hand_Auxilary3_StyleSheet[i, j].IsOverrideTextureBlank(k) && ((k > 0 && mrPlagueRacesPlayer.race.Hand_Auxilary3_StyleSheet[i, j].OverrideTexture[1] != mrPlagueRacesPlayer.race.Hand_Auxilary3_StyleSheet[i, j].OverrideTexture[0]) || k == 0))
                                {
                                    Texture2D sheet = mrPlagueRacesPlayer.race.Hand_Auxilary3_StyleSheet[i, j].OverrideTexture[k];
                                    string colorPath = Path.Combine(k == 0 ? maleAssetPath : femaleAssetPath, PlayerLayerHelpers.PlayerColors[i]);
                                    Directory.CreateDirectory(colorPath);
                                    string stylePath = Path.Combine(colorPath, "Hairstyles_3");
                                    Directory.CreateDirectory(stylePath);
                                    using (stream = File.OpenWrite(Path.Combine(stylePath, "Hands_" + (j + 1) + ".png")))
                                    {
                                        sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
                                    }
                                    if (j == mrPlagueRacesPlayer.auxilaryHairstyle3)
                                    {
                                        icon_blank = (k == 1) ? icon_blank : CombineTexture2Ds(icon_blank, sheet, i, 20, 18, 80);
                                    }
                                }
                            }
                        }
                    }
                    if (!drawPlayerOnIcon)
                    {
                        icon_blank = icon_blank_unmodified;
                    }
                    if (drawTitleOnIcon)
                    {
                        icon_blank = CombineTexture2Ds(icon_blank, ModContent.Request<Texture2D>("MrPlagueRaces/icon_blank_title", AssetRequestMode.ImmediateLoad).Value, 5);
                    }
                    using (stream = File.OpenWrite(Path.Combine(sourceFolder, "icon.png"))) {
                        icon_blank.SaveAsPng(stream, icon_blank.Width, icon_blank.Height);
                    }

                    if (maleHurtPath != null)
                    {
                        File.WriteAllBytes(Path.Combine(maleSoundPath, "Hurt.wav"), File.ReadAllBytes(maleHurtPath));
                    }
                    if (femaleHurtPath != null)
                    {
                        File.WriteAllBytes(Path.Combine(femaleSoundPath, "Hurt.wav"), File.ReadAllBytes(femaleHurtPath));
                    }
                    if (deathPath != null)
                    {
                        File.WriteAllBytes(Path.Combine(maleSoundPath, "Killed.wav"), File.ReadAllBytes(deathPath));
                    }

                    File.WriteAllText(Path.Combine(sourceFolder, modNameTrimmed + ".cs"), GetModClass(modNameTrimmed));
                    File.WriteAllText(Path.Combine(sourceFolder, modNameTrimmed + ".csproj"), GetModCsproj(modNameTrimmed));
                    string text = Path.Combine(sourceFolder, "Properties");
                    Directory.CreateDirectory(text);
                    File.WriteAllText(Path.Combine(text, "launchSettings.json"), GetLaunchSettings());
                    File.WriteAllText(Path.Combine(commonPath, raceNameTrimmed + ".cs"), GetBasicRace(modNameTrimmed, raceNameTrimmed));
                    string text3 = Path.Combine(sourceFolder, "Localization");
                    Directory.CreateDirectory(text3);
                    File.WriteAllText(Path.Combine(text3, "en-US_Mods." + modNameTrimmed + ".hjson"), GetLocalizationFile(modNameTrimmed, raceNameTrimmed));
                    Utils.OpenFolder(sourceFolder);
                    SoundEngine.PlaySound(in SoundID.MenuOpen);
                    SoundEngine.PlaySound(in SoundID.ResearchComplete);
                    Main.menuMode = 10001;
                    mrPlagueRacesPlayer.race.ReloadSheetsFromDefaultFilepath();
                }
            }
            catch (Exception e)
            {
                _descriptionText.SetText("There was an issue. Check client.log" + e);
            }
        }

        public Texture2D CombineTexture2Ds(Texture2D baseTexture, Texture2D textureToPaste, int colorToApply = 0, int drawOffsetX = 0, int drawOffsetY = 0, int offsetX = 0, int offsetY = 0)
        {
            Texture2D resultTexture = ModContent.Request<Texture2D>("MrPlagueRaces/icon_blank", AssetRequestMode.ImmediateLoad).Value;
            Color[] combinedPixels = new Color[baseTexture.Width * baseTexture.Height];
            baseTexture.GetData<Color>(combinedPixels);

            // Get pixel data from base texture
            Color[] basePixels = new Color[baseTexture.Width * baseTexture.Height];
            baseTexture.GetData<Color>(basePixels);

            // Get pixel data from texture to paste
            Color[] pastePixels = new Color[textureToPaste.Width * textureToPaste.Height];
            textureToPaste.GetData<Color>(pastePixels);
            int drawOffset = 20;
            // Paste the texture
            for (int y = 0; y < baseTexture.Height; y++)
            {
                for (int x = 0; x < baseTexture.Width; x++)
                {
                    int basePixelX = x + drawOffsetX;
                    int basePixelY = y + drawOffsetY;
                    Color clothingColor = Color.White;
                    switch (colorToApply)
                    {
                        case -1: // Pants
                            clothingColor = _player.pantsColor;
                            break;
                        case -2: // Undershirt
                            clothingColor = _player.underShirtColor;
                            break;
                        case -3: // Shirt
                            clothingColor = _player.shirtColor;
                            break;
                        case -4: // Shoes
                            clothingColor = _player.shoeColor;
                            break;
                    }
                    // Check bounds to prevent errors if pasting outside the base texture
                    if (basePixelX >= 0 && basePixelX < baseTexture.Width && basePixelY >= 0 && basePixelY < baseTexture.Height)
                    {
                        if (x + offsetX < textureToPaste.Width && y + offsetY < textureToPaste.Height)
                        {
                            Color pasteColor = MultiplyBlendMode(pastePixels[(y + offsetY) * textureToPaste.Width + (x + offsetX)], colorToApply >= 0 ? PlayerLayerHelpers.PlayerColor(_player, colorToApply) : clothingColor);
                            combinedPixels[basePixelY * baseTexture.Width + basePixelX] = Color.Lerp(basePixels[basePixelY * baseTexture.Width + basePixelX], pasteColor, (x > 40 || y > 56) && drawOffsetX != 0 && drawOffsetY != 0 ? 0 : pasteColor.A);
                        }
                        else
                        {
                            combinedPixels[basePixelY * baseTexture.Width + basePixelX] = basePixels[basePixelY * baseTexture.Width + basePixelX];
                        }
                    }
                }
            }

            resultTexture.SetData(combinedPixels);
            return resultTexture;
        }

        public Color MultiplyBlendMode(Color baseColor, Color blendColor)
        {
            Vector3 normalizedBase = new Vector3((float)baseColor.R / 255, (float)baseColor.G / 255, (float)baseColor.B / 255);
            Vector3 normalizedBlend = new Vector3((float)blendColor.R / 255, (float)blendColor.G / 255, (float)blendColor.B / 255);
            return new Color(normalizedBase.X * normalizedBlend.X, normalizedBase.Y * normalizedBlend.Y, normalizedBase.Z * normalizedBlend.Z) * (baseColor.A / 255);
        }

        public byte[] StreamToBytes(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private string GetModBuild()
        {
            return "displayName = " + _modDisplayName.CurrentString + Environment.NewLine + "author = " + _modAuthor.CurrentString + Environment.NewLine + "modReferences = MrPlagueRaces" + Environment.NewLine + "version = 0.1";
        }

        private string GetModDescription()
        {
            return _modDisplayName.CurrentString + " is a custom race mod. Made with MrPlague's Authentic Races.";
        }

        private string GetModClass(string modNameTrimmed)
        {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(75, 2);
            defaultInterpolatedStringHandler.AppendLiteral("using Terraria.ModLoader;\r\n\r\nnamespace ");
            defaultInterpolatedStringHandler.AppendFormatted(modNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral("\r\n{\r\n\tpublic class ");
            defaultInterpolatedStringHandler.AppendFormatted(modNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral(" : Mod\r\n\t{\r\n\t}\r\n}");
            return defaultInterpolatedStringHandler.ToStringAndClear();
        }

        internal string GetBasicRace(string modNameTrimmed, string raceNameTrimmed)
        {
            bool[] statIsNegative =
            {
                false,
                false,
                false,
                false,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false
            };

            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(789, 3);
            defaultInterpolatedStringHandler.AppendLiteral("using Microsoft.Xna.Framework;\r\nusing Microsoft.Xna.Framework.Graphics;\r\nusing System;\r\nusing System.Collections.Generic;\r\nusing System.ComponentModel;\r\nusing Terraria;\r\nusing Terraria.DataStructures;\r\nusing Terraria.GameInput;\r\nusing Terraria.ID;\r\nusing Terraria.ModLoader;\r\nusing Terraria.ModLoader.Config;\r\nusing Terraria.ModLoader.IO;\r\nusing MrPlagueRaces.Common.Races;\r\n\r\nnamespace ");
            defaultInterpolatedStringHandler.AppendFormatted(modNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral(".Common.Races.");
            defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral("\r\n{\r\n\tpublic class ");
            defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral(" : Race\r\n\t{");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\tpublic override void Load()\r\n\t\t{\r\n\t\t\t");
            if (!string.IsNullOrEmpty(_raceDisplayName.CurrentString)) {
                defaultInterpolatedStringHandler.AppendLiteral("DisplayName = \"");
                defaultInterpolatedStringHandler.AppendFormatted(_raceDisplayName.CurrentString);
                defaultInterpolatedStringHandler.AppendLiteral("\";\r\n\t\t\t");
            }
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            int clothStyle = _player.Male ? Array.IndexOf(PlayerLayerHelpers.MaleClothingIDs, _player.skinVariant) : Array.IndexOf(PlayerLayerHelpers.FemaleClothingIDs, _player.skinVariant);

            defaultInterpolatedStringHandler.AppendLiteral("Description = \"This is the default description of the ");
			defaultInterpolatedStringHandler.AppendFormatted(!string.IsNullOrEmpty(_raceDisplayName.CurrentString) ? _raceDisplayName.CurrentString : raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral(" race.\";\r\n\t\t\tClothStyle = ");
            defaultInterpolatedStringHandler.AppendFormatted(clothStyle + 1);
            defaultInterpolatedStringHandler.AppendLiteral(";\r\n\t\t\tHairStyle = ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.hair);
            if (mrPlagueRacesPlayer.GetRaceHairCount(_player, 1) > 0)
            {
                defaultInterpolatedStringHandler.AppendLiteral(";\r\n\t\t\tAuxilaryHairstyle1 = ");
                defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.auxilaryHairstyle1);
                if (mrPlagueRacesPlayer.GetRaceHairCount(_player, 2) > 0)
                {
                    defaultInterpolatedStringHandler.AppendLiteral(";\r\n\t\t\tAuxilaryHairstyle2 = ");
                    defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.auxilaryHairstyle2);
                    if (mrPlagueRacesPlayer.GetRaceHairCount(_player, 3) > 0)
                    {
                        defaultInterpolatedStringHandler.AppendLiteral(";\r\n\t\t\tAuxilaryHairstyle3 = ");
                        defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.auxilaryHairstyle3);
                    }
                }
            }
            defaultInterpolatedStringHandler.AppendLiteral(";\r\n\t\t\tCensorClothing = ");
            defaultInterpolatedStringHandler.AppendFormatted(censorClothingEnabled ? "true" : "false");
            defaultInterpolatedStringHandler.AppendLiteral(";\r\n\t\t\tStarterShirt = ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.armor[11] == familiarShirt ? "true" : "false");
            defaultInterpolatedStringHandler.AppendLiteral(";\r\n\t\t\tStarterPants = ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.armor[12] == familiarPants ? "true" : "false");
            defaultInterpolatedStringHandler.AppendLiteral(";\r\n\t\t\tHairColor = new Color(");
            defaultInterpolatedStringHandler.AppendFormatted(_player.hairColor.R);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.hairColor.G);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.hairColor.B);
            defaultInterpolatedStringHandler.AppendLiteral(");\r\n\t\t\tSkinColor = new Color(");
            defaultInterpolatedStringHandler.AppendFormatted(_player.skinColor.R);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.skinColor.G);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.skinColor.B);
            defaultInterpolatedStringHandler.AppendLiteral(");\r\n\t\t\tDetailColor = new Color(");
            defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.detailColor.R);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.detailColor.G);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.detailColor.B);
            if (detailColorCount > 1)
            {
                defaultInterpolatedStringHandler.AppendLiteral(");\r\n\t\t\tAuxilaryDetailColor1 = new Color(");
                defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.auxilaryDetailColor1.R);
                defaultInterpolatedStringHandler.AppendLiteral(", ");
                defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.auxilaryDetailColor1.G);
                defaultInterpolatedStringHandler.AppendLiteral(", ");
                defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.auxilaryDetailColor1.B);
                if (detailColorCount > 2)
                {
                    defaultInterpolatedStringHandler.AppendLiteral(");\r\n\t\t\tAuxilaryDetailColor2 = new Color(");
                    defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.auxilaryDetailColor2.R);
                    defaultInterpolatedStringHandler.AppendLiteral(", ");
                    defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.auxilaryDetailColor2.G);
                    defaultInterpolatedStringHandler.AppendLiteral(", ");
                    defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.auxilaryDetailColor2.B);
                    if (detailColorCount > 2)
                    {
                        defaultInterpolatedStringHandler.AppendLiteral(");\r\n\t\t\tAuxilaryDetailColor3 = new Color(");
                        defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.auxilaryDetailColor3.R);
                        defaultInterpolatedStringHandler.AppendLiteral(", ");
                        defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.auxilaryDetailColor3.G);
                        defaultInterpolatedStringHandler.AppendLiteral(", ");
                        defaultInterpolatedStringHandler.AppendFormatted(mrPlagueRacesPlayer.auxilaryDetailColor3.B);
                    }
                }
            }
            defaultInterpolatedStringHandler.AppendLiteral(");\r\n\t\t\tEyeColor = new Color(");
            defaultInterpolatedStringHandler.AppendFormatted(_player.eyeColor.R);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.eyeColor.G);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.eyeColor.B);
            defaultInterpolatedStringHandler.AppendLiteral(");\r\n\t\t\tShirtColor = new Color(");
            defaultInterpolatedStringHandler.AppendFormatted(_player.shirtColor.R);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.shirtColor.G);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.shirtColor.B);
            defaultInterpolatedStringHandler.AppendLiteral(");\r\n\t\t\tUnderShirtColor = new Color(");
            defaultInterpolatedStringHandler.AppendFormatted(_player.underShirtColor.R);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.underShirtColor.G);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.underShirtColor.B);
            defaultInterpolatedStringHandler.AppendLiteral(");\r\n\t\t\tPantsColor = new Color(");
            defaultInterpolatedStringHandler.AppendFormatted(_player.pantsColor.R);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.pantsColor.G);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.pantsColor.B);
            defaultInterpolatedStringHandler.AppendLiteral(");\r\n\t\t\tShoeColor = new Color(");
            defaultInterpolatedStringHandler.AppendFormatted(_player.shoeColor.R);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.shoeColor.G);
            defaultInterpolatedStringHandler.AppendLiteral(", ");
            defaultInterpolatedStringHandler.AppendFormatted(_player.shoeColor.B);
            defaultInterpolatedStringHandler.AppendLiteral(");\r\n\t\t}\r\n\r\n\t\tpublic override void ResetEffects(Player player)\r\n\t\t{");
            if (enableDefaultAbility)
            {
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tRegisterAbilityDescription(ModContent.GetInstance<");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Config>().");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Ability1, $\"[c/4DBF60:+] Press Z to make a hurt noise and become entangled in webs. Replace this default ability with one of your own!\"); // this description is only displayed when ");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Ability1 is set to 'true' in ");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Config");
            }
            if (enableDefaultAttribute)
            {
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tRegisterAbilityDescription(ModContent.GetInstance<");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Config>().");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("NoFallDmg, $\"[c/4DBF60:+] You do not take fall damage.\"); // this description is only displayed when ");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("NoFallDmg is set to 'true' in ");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Config");
            }
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\r\n\t\t\tRaceStatDictionary = ModContent.GetInstance<");
            defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral("Config>().");
            defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral("Stats; // this line automatically links the stat dictionary at the bottom of the file (");
            defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral("Stats) to your race");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\r\n\t\t\tvar mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRaces.MrPlagueRacesPlayer>();\r\n\t\t\tif (ModContent.GetInstance<MrPlagueRaces.MrPlagueRacesConfig>().raceStats)\r\n\t\t\t{\r\n\t\t\t\t// Enable player attributes here. A library of player attributes can be found on the tmodloader docs: https://docs.tmodloader.net/docs/stable/class_player.html");
            if (enableDefaultAttribute)
            {
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\tif (ModContent.GetInstance<");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Config>().");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("NoFallDmg) // this line makes sure ");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("NoFallDmg is enabled in ");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Config before applying the attribute below");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t{");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\t// This player attribute prevents ");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("from taking fall damage");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\tplayer.noFallDmg = true;");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t}");
            }
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t}\r\n\t\t}\r\n\t\t");
            if (enableDefaultAbility)
            {
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\tpublic override void ProcessTriggers(Player player, TriggersSet triggersSet)");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t{");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tvar mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRaces.MrPlagueRacesPlayer>();");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tif (ModContent.GetInstance<MrPlagueRaces.MrPlagueRacesConfig>().raceStats && !player.dead) // this line makes sure racial stats are enabled and the player is not dead before executing the ability");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t{");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\tif (ModContent.GetInstance<");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Config>().");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Ability1) // this line makes sure ");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Ability1 is enabled in ");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Config before executing the ability below");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t{");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\tif (MrPlagueRaces.MrPlagueRaces.RaceAbilityKeybind1.JustPressed)");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\t{");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\t\t// Write code for a single ability here. \r\n\t\t\t\t\t\t// This ability plays ");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("'s hurt sound and gives the player 5 seconds of Webbed");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\t\tmrPlagueRacesPlayer.PlayRaceSound(player, \"Hurt\");");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\t\tplayer.AddBuff(BuffID.Webbed, 5 * 60);");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\t}");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t}");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t}");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t}");
            }
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\r\n\t\tpublic class ");
            defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral("Config : ModConfig\r\n\t\t{");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t// Modify ");
            defaultInterpolatedStringHandler.AppendFormatted(modNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral("/Localization/en-US_Mods.");
            defaultInterpolatedStringHandler.AppendFormatted(modNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral(".hjson to change your mod's config descriptions");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tpublic static ");
            defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral("Config Instance;\r\n\t\t\tpublic override ConfigScope Mode => ConfigScope.ServerSide;\r\n\t\t\t\r\n\t\t\t[BackgroundColor(110, 141, 255)]\r\n\t\t\tpublic Dictionary<MrPlagueRaces.RacialStatType, MrPlagueRaces.RacialStatPercentageModifier> ");
            defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral("Stats = new Dictionary<MrPlagueRaces.RacialStatType, MrPlagueRaces.RacialStatPercentageModifier>()\r\n\t\t\t{");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t// Insert player stats here. p20 is equivalent to +20%. n20 is equivalent to -20%. etc");
            for (int i = 0; i < raceStats.Length; i++)
            {
                if (raceStats[i] != 0)
                {
                    defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t[MrPlagueRaces.RacialStatType.");
                    defaultInterpolatedStringHandler.AppendFormatted(Enum.GetName(typeof(RacialStatType), i + 1));
                    defaultInterpolatedStringHandler.AppendLiteral("] = MrPlagueRaces.RacialStatPercentageModifier.");
                    defaultInterpolatedStringHandler.AppendFormatted(Enum.GetName(typeof(RacialStatPercentageModifier), statIsNegative[i] ? 100 - raceStats[i] : 100 + raceStats[i]));
                    defaultInterpolatedStringHandler.AppendLiteral(", ");
                }
            }
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t};");
            if (enableDefaultAbility)
            {
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\r\n\t\t\t[DefaultValue(true)]");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t[BackgroundColor(110, 141, 255)]");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tpublic bool ");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Ability1;");
            }
            if (enableDefaultAttribute)
            {
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\r\n\t\t\t[DefaultValue(true)]");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t[BackgroundColor(110, 141, 255)]");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tpublic bool ");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("NoFallDmg;");
            }
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t}\r\n\t}\r\n}");

            return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		internal string GetModCsproj(string modNameTrimmed)
		{
			return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Project Sdk=\"Microsoft.NET.Sdk\">\r\n  <Import Project=\"..\\tModLoader.targets\" />\r\n  <PropertyGroup>\r\n    <AssemblyName>" + modNameTrimmed + "</AssemblyName>\r\n    <TargetFramework>net6.0</TargetFramework>\r\n    <PlatformTarget>AnyCPU</PlatformTarget>\r\n    <LangVersion>latest</LangVersion>\r\n  </PropertyGroup>\r\n  <ItemGroup>\r\n    <PackageReference Include=\"tModLoader.CodeAssist\" Version=\"0.1.*\" />\r\n  </ItemGroup>\r\n</Project>";
		}

		internal bool CsprojUpdateNeeded(string fileContents)
		{
			if (!fileContents.Contains("..\\tModLoader.targets"))
			{
				return true;
			}
			if (!fileContents.Contains("<TargetFramework>net6.0</TargetFramework>"))
			{
				return true;
			}
			return false;
		}

		internal string GetLaunchSettings()
		{
			return "{\r\n  \"profiles\": {\r\n    \"Terraria\": {\r\n      \"commandName\": \"Executable\",\r\n      \"executablePath\": \"dotnet\",\r\n      \"commandLineArgs\": \"$(tMLPath)\",\r\n      \"workingDirectory\": \"$(tMLSteamPath)\"\r\n    },\r\n    \"TerrariaServer\": {\r\n      \"commandName\": \"Executable\",\r\n      \"executablePath\": \"dotnet\",\r\n      \"commandLineArgs\": \"$(tMLServerPath)\",\r\n      \"workingDirectory\": \"$(tMLSteamPath)\"\r\n    }\r\n  }\r\n}";
		}

		internal string GetLocalizationFile(string modNameTrimmed, string raceNameTrimmed)
        {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(789, 3);
            defaultInterpolatedStringHandler.AppendLiteral("Configs: {\r\n\t");
            defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral("Config: {");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\tDisplayName: ");
            defaultInterpolatedStringHandler.AppendFormatted(!string.IsNullOrEmpty(_raceDisplayName.CurrentString) ? _raceDisplayName.CurrentString : raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t");
            defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral("Stats: {");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tLabel: Stat Modifiers (");
            defaultInterpolatedStringHandler.AppendFormatted(!string.IsNullOrEmpty(_raceDisplayName.CurrentString) ? _raceDisplayName.CurrentString : raceNameTrimmed);
            defaultInterpolatedStringHandler.AppendLiteral(")");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tTooltip: ");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\'\'\'");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\tModify this race's stat changes");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t[c/F26E0A:To add a new stat modifier, delete/reassign the key of any existing modifiers containing \"Key: None\"]");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t[c/FF3640:WARNING: Setting stats too high or too low may result in unexpected ingame behavior!]");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\'\'\'");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t}");
            if (enableDefaultAbility)
            {
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("Ability1: {");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tLabel: \"[i/s1:2703] Primary Ability (Example Webs)\"");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tTooltip: ");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\'\'\'");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\tOn: Enables this ability");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\tOff: Disables this ability");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\'\'\'");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t}");
            }
            if (enableDefaultAttribute)
            {
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t");
                defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral("NoFallDmg: {");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tLabel: \"[i/s1:158] Fall Damage Immunity\"");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\tTooltip: ");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\'\'\'");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\tOn: ");
                defaultInterpolatedStringHandler.AppendFormatted(!string.IsNullOrEmpty(_raceDisplayName.CurrentString) ? _raceDisplayName.CurrentString : raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral(" is immune to fall damage");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\tOff: ");
                defaultInterpolatedStringHandler.AppendFormatted(!string.IsNullOrEmpty(_raceDisplayName.CurrentString) ? _raceDisplayName.CurrentString : raceNameTrimmed);
                defaultInterpolatedStringHandler.AppendLiteral(" is not immune to fall damage");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t\t\t\'\'\'");
                defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\t}");
            }
            defaultInterpolatedStringHandler.AppendLiteral("\r\n\t}");
            defaultInterpolatedStringHandler.AppendLiteral("\r\n}");
            return defaultInterpolatedStringHandler.ToStringAndClear();
        }
	}
}
