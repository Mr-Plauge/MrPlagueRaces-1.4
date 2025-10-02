using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using MrPlagueRaces.Common.Races;

namespace MrPlagueRaces.Common.UI.States
{
	public class UIRaceSheetPanel : UIElement
    {
        public UIPanel backPanel;

        public UITextPanel<string> fileText;
        public UITextPanel<string> typeText;
        public UITextPanel<string> colorText;
        public UITextPanel<string> hairText;
        public UITextPanel<string> trackText;
        public UITextPanel<string> genderText;

        public UITextPanel<string> fileButton;
        public UITextPanel<string> typeButton;
        public UITextPanel<string> colorButton;
        public UITextPanel<string> hairButton;
        public UITextPanel<string> trackButton;
        public UITextPanel<string> genderButton;

        public Asset<Texture2D> buttonAddTexture;
        public UIImageButton addButton;
        public Asset<Texture2D> buttonDeleteTexture;
        public UIImageButton deleteButton;
        public string _filePath;
        public SheetCategory _sheetType;
        public int _colorType;
        public int _hairType;
        public int _trackType;
        public int _genderType;
        public string _fileName;
        public UIText _hoverText;
        public Player _player;
        public int _sheetIndex;
        public string originalFilePath;
        public bool sheetEnabled = true;
        public Color origColor;

        public bool hasDuplicateSheet;

        public bool isReceivingFileFromModContent = true;

        public static string[] ColorNames = { "Skin", "Detail", "Detail1", "Detail2", "Detail3", "None", "Eyes", "Hair", "GloSkin", "GloDetail", "GloDetail1", "GloDetail2", "GloDetail3", "GloNone", "GloEyes", "GloHair" };
        public static string[] IsFemale = { "No", "Yes" };

        public UIRaceSheetPanel(Player player, int sheetIndex, ref UIText hoverText, string filePath, SheetCategory sheetType, int colorType, int hairType, int trackType, int genderType)
        {
            _player = player;
            _sheetIndex = sheetIndex;
            _hoverText = hoverText;
            _filePath = filePath;
            _sheetType = sheetType; 
            _colorType = colorType;
            _hairType = hairType;
            _trackType = trackType;
            _genderType = genderType;
            _fileName = Path.GetFileName(_filePath);
            originalFilePath = _filePath;

            buttonAddTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay");
            buttonDeleteTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete");

            backPanel = new UIPanel();
            backPanel.SetPadding(0f);
            backPanel.Width.Set(0f, 1f);
            backPanel.Height.Set(60f, 0f);
            backPanel.BorderColor = new Color(89, 116, 213) * 0.7f;
            backPanel.OnMouseOver += BackPanelMouseOver;
            backPanel.OnMouseOut += BackPanelMouseOut;
            backPanel.OnLeftClick += Click_ToggleSheet;
            Append(backPanel);
            origColor = backPanel.BackgroundColor;

            fileText = new UITextPanel<string>("Sheet " + (_sheetIndex + 1).ToString());
            fileText.TextColor = Color.White;
            fileText.TextScale = 0.8f;
            fileText.SetPadding(7f);
            fileText.Width.Set(105f, 0f);
            fileText.Height.Set(30f, 0f);
            fileText.BackgroundColor = Color.Transparent;
            fileText.BorderColor = Color.Transparent;
            fileText.Left = StyleDimension.FromPixelsAndPercent(7f, 0f);
            fileText.Top = StyleDimension.FromPixelsAndPercent(0f, 0f);
            fileText.TextHAlign = 0f;
            backPanel.Append(fileText);

            typeText = new UITextPanel<string>("Replaces");
            typeText.TextColor = Color.White;
            typeText.TextScale = 0.8f;
            typeText.SetPadding(7f);
            typeText.Width.Set(85f, 0f);
            typeText.Height.Set(30f, 0f);
            typeText.BackgroundColor = Color.Transparent;
            typeText.BorderColor = Color.Transparent;
            typeText.Left = StyleDimension.FromPixelsAndPercent(121f, 0f);
            typeText.Top = StyleDimension.FromPixelsAndPercent(0f, 0f);
            typeText.TextHAlign = 0f;
            backPanel.Append(typeText);

            colorText = new UITextPanel<string>("Colored By");
            colorText.TextColor = Color.White;
            colorText.TextScale = 0.8f;
            colorText.SetPadding(7f);
            colorText.Width.Set(105f, 0f);
            colorText.Height.Set(30f, 0f);
            colorText.BackgroundColor = Color.Transparent;
            colorText.BorderColor = Color.Transparent;
            colorText.Left = StyleDimension.FromPixelsAndPercent(216f, 0f);
            colorText.Top = StyleDimension.FromPixelsAndPercent(0f, 0f);
            colorText.TextHAlign = 0f;
            backPanel.Append(colorText);

            hairText = new UITextPanel<string>("Style");
            hairText.TextColor = Color.White;
            hairText.TextScale = 0.8f;
            hairText.SetPadding(7f);
            hairText.Width.Set(46f, 0f);
            hairText.Height.Set(30f, 0f);
            hairText.BackgroundColor = Color.Transparent;
            hairText.BorderColor = Color.Transparent;
            hairText.Left = StyleDimension.FromPixelsAndPercent(332f - 5f, 0f);
            hairText.Top = StyleDimension.FromPixelsAndPercent(0f, 0f);
            hairText.TextHAlign = 0f;
            backPanel.Append(hairText);

            trackText = new UITextPanel<string>("Track");
            trackText.TextColor = Color.White;
            trackText.TextScale = 0.8f;
            trackText.SetPadding(7f);
            trackText.Width.Set(46f, 0f);
            trackText.Height.Set(30f, 0f);
            trackText.BackgroundColor = Color.Transparent;
            trackText.BorderColor = Color.Transparent;
            trackText.Left = StyleDimension.FromPixelsAndPercent(392f - 5f, 0f);
            trackText.Top = StyleDimension.FromPixelsAndPercent(0f, 0f);
            trackText.TextHAlign = 0f;
            backPanel.Append(trackText);

            genderText = new UITextPanel<string>("Female");
            genderText.TextColor = Color.White;
            genderText.TextScale = 0.8f;
            genderText.SetPadding(7f);
            genderText.Width.Set(52f, 0f);
            genderText.Height.Set(30f, 0f);
            genderText.BackgroundColor = Color.Transparent;
            genderText.BorderColor = Color.Transparent;
            genderText.Left = StyleDimension.FromPixelsAndPercent(450f - 5f, 0f);
            genderText.Top = StyleDimension.FromPixelsAndPercent(0f, 0f);
            genderText.TextHAlign = 0f;
            backPanel.Append(genderText);

            fileButton = new UITextPanel<string>(_filePath == null ? "Upload" : _fileName + ".png");
            fileButton.TextColor = Color.Gray;
            fileButton.SetPadding(7f);
            fileButton.Width.Set(105f, 0f);
            fileButton.Height.Set(30f, 0f);
            fileButton.BackgroundColor = new Color(31, 41, 76);
            fileButton.BorderColor = new Color(82, 96, 145);
            fileButton.Left = StyleDimension.FromPixelsAndPercent(7f, 0f);
            fileButton.Top = StyleDimension.FromPixelsAndPercent(23f, 0f);
            fileButton.OnMouseOver += FadedMouseOver;
            fileButton.OnMouseOut += FadedMouseOut;
            fileButton.OnLeftClick += Click_File;
            fileButton.OnRightClick += Click_ResetFile;
            fileButton.OnLeftClick += FadedMouseOver;
            fileButton.OnRightClick += FadedMouseOver;
            fileButton.TextHAlign = 0.1f;
            backPanel.Append(fileButton);

            typeButton = new UITextPanel<string>(_sheetType.ToString());
            typeButton.TextColor = Color.Gray;
            typeButton.SetPadding(7f);
            typeButton.Width.Set(85f, 0f);
            typeButton.Height.Set(30f, 0f);
            typeButton.BackgroundColor = new Color(31, 41, 76);
            typeButton.BorderColor = new Color(82, 96, 145);
            typeButton.Left = StyleDimension.FromPixelsAndPercent(121f, 0f);
            typeButton.Top = StyleDimension.FromPixelsAndPercent(23f, 0f);
            typeButton.OnMouseOver += FadedMouseOver;
            typeButton.OnMouseOut += FadedMouseOut;
            typeButton.OnLeftClick += Click_Type;
            typeButton.OnRightClick += Click_Type_Right;
            typeButton.OnLeftClick += FadedMouseOver;
            typeButton.OnRightClick += FadedMouseOver;
            typeButton.TextHAlign = 0.1f;
            backPanel.Append(typeButton);

            colorButton = new UITextPanel<string>(ColorNames[_colorType]);
            colorButton.TextColor = Color.Gray;
            colorButton.SetPadding(7f);
            colorButton.Width.Set(105f, 0f);
            colorButton.Height.Set(30f, 0f);
            colorButton.BackgroundColor = new Color(31, 41, 76);
            colorButton.BorderColor = new Color(82, 96, 145);
            colorButton.Left = StyleDimension.FromPixelsAndPercent(216f, 0f);
            colorButton.Top = StyleDimension.FromPixelsAndPercent(23f, 0f);
            colorButton.OnMouseOver += FadedMouseOver;
            colorButton.OnMouseOut += FadedMouseOut;
            colorButton.OnLeftClick += Click_Color;
            colorButton.OnRightClick += Click_Color_Right;
            colorButton.OnLeftClick += FadedMouseOver;
            colorButton.OnRightClick += FadedMouseOver;
            colorButton.TextHAlign = 0.1f;
            backPanel.Append(colorButton);

            hairButton = new UITextPanel<string>((_hairType + 1).ToString());
            hairButton.TextColor = Color.Gray;
            hairButton.SetPadding(7f);
            hairButton.Width.Set(46f, 0f);
            hairButton.Height.Set(30f, 0f);
            hairButton.BackgroundColor = new Color(31, 41, 76);
            hairButton.BorderColor = new Color(82, 96, 145);
            hairButton.Left = StyleDimension.FromPixelsAndPercent(332f, 0f);
            hairButton.Top = StyleDimension.FromPixelsAndPercent(23f, 0f);
            hairButton.OnMouseOver += FadedMouseOver;
            hairButton.OnMouseOut += FadedMouseOut;
            hairButton.OnLeftClick += Click_Hair;
            hairButton.OnRightClick += Click_Hair_Right;
            hairButton.OnLeftClick += FadedMouseOver;
            hairButton.OnRightClick += FadedMouseOver;
            hairButton.TextHAlign = 0.1f;
            backPanel.Append(hairButton);

            trackButton = new UITextPanel<string>((_trackType + 1).ToString());
            trackButton.TextColor = Color.Gray;
            trackButton.SetPadding(7f);
            trackButton.Width.Set(46f, 0f);
            trackButton.Height.Set(30f, 0f);
            trackButton.BackgroundColor = new Color(31, 41, 76);
            trackButton.BorderColor = new Color(82, 96, 145);
            trackButton.Left = StyleDimension.FromPixelsAndPercent(392f, 0f);
            trackButton.Top = StyleDimension.FromPixelsAndPercent(23f, 0f);
            trackButton.OnMouseOver += FadedMouseOver;
            trackButton.OnMouseOut += FadedMouseOut;
            trackButton.OnLeftClick += Click_Track;
            trackButton.OnRightClick += Click_Track_Right;
            trackButton.OnLeftClick += FadedMouseOver;
            trackButton.OnRightClick += FadedMouseOver;
            trackButton.TextHAlign = 0.1f;
            backPanel.Append(trackButton);

            genderButton = new UITextPanel<string>(IsFemale[_genderType]);
            genderButton.TextColor = Color.Gray;
            genderButton.SetPadding(7f);
            genderButton.Width.Set(52f, 0f);
            genderButton.Height.Set(30f, 0f);
            genderButton.BackgroundColor = new Color(31, 41, 76);
            genderButton.BorderColor = new Color(82, 96, 145);
            genderButton.Left = StyleDimension.FromPixelsAndPercent(450f, 0f);
            genderButton.Top = StyleDimension.FromPixelsAndPercent(23f, 0f);
            genderButton.OnMouseOver += FadedMouseOver;
            genderButton.OnMouseOut += FadedMouseOut;
            genderButton.OnLeftClick += Click_Gender;
            genderButton.OnRightClick += Click_Gender;
            genderButton.OnLeftClick += FadedMouseOver;
            genderButton.OnRightClick += FadedMouseOver;
            genderButton.TextHAlign = 0.1f;
            backPanel.Append(genderButton);

            addButton = new UIImageButton(buttonAddTexture);
            addButton.Left = StyleDimension.FromPixelsAndPercent(510f, 0f);
            addButton.Top = StyleDimension.FromPixelsAndPercent(5f, 0f);
            addButton.OnMouseOver += AddDeleteOver;
            addButton.OnMouseOut += AddDeleteOut;
            addButton.OnLeftClick += Click_Add;
            backPanel.Append(addButton);

            deleteButton = new UIImageButton(buttonDeleteTexture);
            deleteButton.Left = StyleDimension.FromPixelsAndPercent(510f, 0f);
            deleteButton.Top = StyleDimension.FromPixelsAndPercent(30f, 0f);
            deleteButton.OnMouseOver += AddDeleteOver;
            deleteButton.OnMouseOut += AddDeleteOut;
            deleteButton.OnLeftClick += Click_Delete;
            backPanel.Append(deleteButton);
        }

        public void ReapplySheet()
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            if (sheetEnabled && mrPlagueRacesPlayer.race.GetRaceSheetFromIDs(_sheetType, _colorType, _hairType, _trackType).OverrideTexture[_genderType] == null)
            {
                if (_filePath != null)
                {
                    if (isReceivingFileFromModContent)
                    {
                        mrPlagueRacesPlayer.race.SetOverrideTextureFromModContent(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                    }
                    else
                    {
                        mrPlagueRacesPlayer.race.SetOverrideTextureFromFilePath(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                    }
                }
            }
        }

        public void Click_Add(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            UICreateRace.AddSheetAtIndex(_sheetIndex);
        }
        public void Click_Delete(UIMouseEvent evt, UIElement listeningElement)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            if (UICreateRace.sheetCount > 1)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                mrPlagueRacesPlayer.race.ClearOverrideTexture(_sheetType, _colorType, _hairType, _trackType, _genderType);
                UICreateRace.RemoveSheetAtIndex(_sheetIndex);
            }
            else
            {
                _hoverText.SetText("Can't remove the last sheet.");
            }
            UICreateRace.ReapplyAllSheets();
        }
        public void Click_File(UIMouseEvent evt, UIElement listeningElement)
        {
            sheetEnabled = true;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            _filePath = mrPlagueRacesPlayer.race.FilePathFromUser();
            if (_filePath != null && _filePath != originalFilePath)
            {
                mrPlagueRacesPlayer.race.SetOverrideTextureFromFilePath(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                _fileName = Path.GetFileName(_filePath);
                fileButton.SetText(_fileName);
                isReceivingFileFromModContent = false;
            }
        }
        public void Click_ResetFile(UIMouseEvent evt, UIElement listeningElement)
        {
            sheetEnabled = true;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            _filePath = null;
            mrPlagueRacesPlayer.race.ClearOverrideTexture(_sheetType, _colorType, _hairType, _trackType, _genderType);
            fileButton.SetText("Upload");
            isReceivingFileFromModContent = false;
        }
        public void Click_Type(UIMouseEvent evt, UIElement listeningElement)
        {
            sheetEnabled = true;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            if (_filePath != null)
            {
                mrPlagueRacesPlayer.race.ClearOverrideTexture(_sheetType, _colorType, _hairType, _trackType, _genderType);
            }
            SheetCategory sheetCategory = SheetCategory.Arms;
            switch (_sheetType)
            {
                case SheetCategory.Arms:
                    sheetCategory = SheetCategory.Hands;
                    break;
                case SheetCategory.Hands:
                    sheetCategory = SheetCategory.Body;
                    break;
                case SheetCategory.Body:
                    sheetCategory = SheetCategory.Legs;
                    break;
                case SheetCategory.Legs:
                    sheetCategory = SheetCategory.Head;
                    break;
                case SheetCategory.Head:
                    sheetCategory = SheetCategory.EyeLids;
                    break;
                case SheetCategory.EyeLids:
                    sheetCategory = SheetCategory.Eyes;
                    break;
                case SheetCategory.Eyes:
                    sheetCategory = SheetCategory.Hair;
                    break;
                case SheetCategory.Hair:
                    sheetCategory = SheetCategory.HairAlt;
                    break;
                case SheetCategory.HairAlt:
                    sheetCategory = SheetCategory.Arms;
                    break;
            }
            _sheetType = sheetCategory;
            typeButton.SetText(_sheetType.ToString());
            if (_filePath != null)
            {
                if (isReceivingFileFromModContent)
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromModContent(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
                else
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromFilePath(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
            }
            UICreateRace.ReapplyAllSheets();
        }
        public void Click_Type_Right(UIMouseEvent evt, UIElement listeningElement)
        {
            sheetEnabled = true;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            if (_filePath != null)
            {
                mrPlagueRacesPlayer.race.ClearOverrideTexture(_sheetType, _colorType, _hairType, _trackType, _genderType);
            }
            SheetCategory sheetCategory = SheetCategory.Arms;
            switch (_sheetType)
            {
                case SheetCategory.Arms:
                    sheetCategory = SheetCategory.HairAlt;
                    break;
                case SheetCategory.Hands:
                    sheetCategory = SheetCategory.Arms;
                    break;
                case SheetCategory.Body:
                    sheetCategory = SheetCategory.Hands;
                    break;
                case SheetCategory.Legs:
                    sheetCategory = SheetCategory.Body;
                    break;
                case SheetCategory.Head:
                    sheetCategory = SheetCategory.Legs;
                    break;
                case SheetCategory.EyeLids:
                    sheetCategory = SheetCategory.Head;
                    break;
                case SheetCategory.Eyes:
                    sheetCategory = SheetCategory.EyeLids;
                    break;
                case SheetCategory.Hair:
                    sheetCategory = SheetCategory.Eyes;
                    break;
                case SheetCategory.HairAlt:
                    sheetCategory = SheetCategory.Hair;
                    break;
            }
            _sheetType = sheetCategory;
            typeButton.SetText(_sheetType.ToString());
            if (_filePath != null)
            {
                if (isReceivingFileFromModContent)
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromModContent(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
                else
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromFilePath(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
            }
            UICreateRace.ReapplyAllSheets();
        }
        public void Click_Color(UIMouseEvent evt, UIElement listeningElement)
        {
            sheetEnabled = true;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            if (_filePath != null)
            {
                mrPlagueRacesPlayer.race.ClearOverrideTexture(_sheetType, _colorType, _hairType, _trackType, _genderType);
            }
            _colorType = _colorType >= 15 ? 0 : _colorType + 1;
            colorButton.SetText(ColorNames[_colorType]);
            if (_filePath != null)
            {
                if (isReceivingFileFromModContent)
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromModContent(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
                else
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromFilePath(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
            }
            UICreateRace.ReapplyAllSheets();
        }
        public void Click_Color_Right(UIMouseEvent evt, UIElement listeningElement)
        {
            sheetEnabled = true;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            if (_filePath != null)
            {
                mrPlagueRacesPlayer.race.ClearOverrideTexture(_sheetType, _colorType, _hairType, _trackType, _genderType);
            }
            _colorType = _colorType <= 0 ? 15 : _colorType - 1;
            colorButton.SetText(ColorNames[_colorType]);
            if (_filePath != null)
            {
                if (isReceivingFileFromModContent)
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromModContent(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
                else
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromFilePath(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
            }
            UICreateRace.ReapplyAllSheets();
        }
        public void Click_Hair(UIMouseEvent evt, UIElement listeningElement)
        {
            sheetEnabled = true;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            if (_filePath != null)
            {
                mrPlagueRacesPlayer.race.ClearOverrideTexture(_sheetType, _colorType, _hairType, _trackType, _genderType);
            }
            _hairType = _hairType >= 164 ? -1 : _hairType + 1;
            hairButton.SetText((_hairType + 1).ToString());
            if (_filePath != null && _filePath != originalFilePath)
            {
                if (isReceivingFileFromModContent)
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromModContent(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
                else
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromFilePath(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
            }
            UICreateRace.ReapplyAllSheets();
        }
        public void Click_Hair_Right(UIMouseEvent evt, UIElement listeningElement)
        {
            sheetEnabled = true;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            if (_filePath != null)
            {
                mrPlagueRacesPlayer.race.ClearOverrideTexture(_sheetType, _colorType, _hairType, _trackType, _genderType);
            }
            _hairType = _hairType <= -1 ? 164 : _hairType - 1;
            hairButton.SetText((_hairType + 1).ToString());
            if (_filePath != null && _filePath != originalFilePath)
            {
                if (isReceivingFileFromModContent)
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromModContent(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
                else
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromFilePath(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
            }
            UICreateRace.ReapplyAllSheets();
        }
        public void Click_Track(UIMouseEvent evt, UIElement listeningElement)
        {
            sheetEnabled = true;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            if (_filePath != null)
            {
                mrPlagueRacesPlayer.race.ClearOverrideTexture(_sheetType, _colorType, _hairType, _trackType, _genderType);
            }
            _trackType = _trackType >= 3 ? 0 : _trackType + 1;
            trackButton.SetText((_trackType + 1).ToString());
            if (_filePath != null)
            {
                if (isReceivingFileFromModContent)
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromModContent(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
                else
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromFilePath(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
            }
            UICreateRace.ReapplyAllSheets();
        }
        public void Click_Track_Right(UIMouseEvent evt, UIElement listeningElement)
        {
            sheetEnabled = true;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            if (_filePath != null)
            {
                mrPlagueRacesPlayer.race.ClearOverrideTexture(_sheetType, _colorType, _hairType, _trackType, _genderType);
            }
            _trackType = _trackType <= 0 ? 3 : _trackType - 1;
            trackButton.SetText((_trackType + 1).ToString());
            if (_filePath != null)
            {
                if (isReceivingFileFromModContent)
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromModContent(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
                else
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromFilePath(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
            }
            UICreateRace.ReapplyAllSheets();
        }
        public void Click_Gender(UIMouseEvent evt, UIElement listeningElement)
        {
            sheetEnabled = true;
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            if (_filePath != null)
            {
                mrPlagueRacesPlayer.race.ClearOverrideTexture(_sheetType, _colorType, _hairType, _trackType, _genderType);
            }
            _genderType = _genderType >= 1 ? 0 : 1;
            genderButton.SetText(IsFemale[_genderType]);
            if (_filePath != null)
            {
                if (isReceivingFileFromModContent)
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromModContent(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
                else
                {
                    mrPlagueRacesPlayer.race.SetOverrideTextureFromFilePath(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                }
            }
            UICreateRace.ReapplyAllSheets();
        }

        public void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (listeningElement == fileButton)
            {
                string fileHoverText = "Upload a new ";
                if (_genderType == 1)
                {
                    fileHoverText = fileHoverText +  "female ";
                }
                string appendixText = "";
                switch (_sheetType)
                {
                    case SheetCategory.Arms:
                        appendixText = "arm sheet (360x224px).";
                        break;
                    case SheetCategory.Hands:
                        appendixText = "hand sheet (360x224px).";
                        break;
                    case SheetCategory.Body:
                        appendixText = "body sheet (360x224px).";
                        break;
                    case SheetCategory.Legs:
                        appendixText = "leg sheet (40x1120px).";
                        break;
                    case SheetCategory.Head:
                        appendixText = "head sheet (40x1120px).";
                        break;
                    case SheetCategory.EyeLids:
                        appendixText = "eyelid sheet (40x168px).";
                        break;
                    case SheetCategory.Eyes:
                        appendixText = "eye sheet (40x1120px).";
                        break;
                    case SheetCategory.Hair:
                        appendixText = "hair sheet (40x784px).";
                        break;
                    case SheetCategory.HairAlt:
                        appendixText = "hairAlt sheet (40x784px).";
                        break;
                }
                fileHoverText = fileHoverText + appendixText;
                _hoverText.SetText(fileHoverText);
            }
            if (listeningElement == typeButton)
            {
                switch (_sheetType)
                {
                    case SheetCategory.Arms:
                        _hoverText.SetText("This sheet replaces the player's arms.");
                        break;
                    case SheetCategory.Hands:
                        _hoverText.SetText("This sheet replaces the player's hands.");
                        break;
                    case SheetCategory.Body:
                        _hoverText.SetText("This sheet replaces the player's body.");
                        break;
                    case SheetCategory.Legs:
                        _hoverText.SetText("This sheet replaces the player's legs.");
                        break;
                    case SheetCategory.Head:
                        _hoverText.SetText("This sheet replaces the player's head.");
                        break;
                    case SheetCategory.EyeLids:
                        _hoverText.SetText("This sheet replaces the player's eyelids.");
                        break;
                    case SheetCategory.Eyes:
                        _hoverText.SetText("This sheet replaces the player's eyes.");
                        break;
                    case SheetCategory.Hair:
                        if (_hairType == -1)
                        {
                            _hoverText.SetText("Set the style parameter to a value above 0 to use this sheet type.");
                        }
                        else
                        {
                            _hoverText.SetText("This sheet replaces the player's hair.");
                        }
                        break;
                    case SheetCategory.HairAlt:
                        if (_hairType == -1)
                        {
                            _hoverText.SetText("Set the style parameter to a value above 0 to use this sheet type.");
                        }
                        else
                        {
                            _hoverText.SetText("This sheet replaces the player's hair while wearing certain hats.");
                        }
                        break;
                }
            }
            if (listeningElement == colorButton)
            {
                switch (_colorType)
                {
                    case 0:
                        _hoverText.SetText("This sheet adopts the player's skin color.");
                        break;
                    case 1:
                        _hoverText.SetText("This sheet adopts the player's first detail color.");
                        break;
                    case 2:
                        _hoverText.SetText("This sheet adopts the player's second detail color.");
                        break;
                    case 3:
                        _hoverText.SetText("This sheet adopts the player's third detail color.");
                        break;
                    case 4:
                        _hoverText.SetText("This sheet adopts the player's fourth detail color.");
                        break;
                    case 5:
                        _hoverText.SetText("This sheet is unaffected by player colors.");
                        break;
                    case 6:
                        _hoverText.SetText("This sheet adopts the player's eye color.");
                        break;
                    case 7:
                        _hoverText.SetText("This sheet adopts the player's hair color.");
                        break;
                    case 8:
                        _hoverText.SetText("This sheet glows & adopts the player's skin color.");
                        break;
                    case 9:
                        _hoverText.SetText("This sheet glows & adopts the player's first detail color.");
                        break;
                    case 10:
                        _hoverText.SetText("This sheet glows & adopts the player's second detail color.");
                        break;
                    case 11:
                        _hoverText.SetText("This sheet glows & adopts the player's third detail color.");
                        break;
                    case 12:
                        _hoverText.SetText("This sheet glows & adopts the player's fourth detail color.");
                        break;
                    case 13:
                        _hoverText.SetText("This sheet glows & is unaffected by player colors.");
                        break;
                    case 14:
                        _hoverText.SetText("This sheet glows & adopts the player's eye color.");
                        break;
                    case 15:
                        _hoverText.SetText("This sheet glows & adopts the player's hair color.");
                        break;
                }
            }
            if (listeningElement == hairButton)
            {
                if (_hairType == -1)
                {
                    _hoverText.SetText("Set this parameter to a value above 0 to tie this sheet to a hairstyle.");
                }
                else
                {
                    _hoverText.SetText("This sheet is only rendered when hairstyle " + (_hairType + 1) + " is selected on hair track #" + (_trackType + 1) + ".");
                }
            }
            if (listeningElement == trackButton)
            {
                if (_hairType == -1)
                {
                    _hoverText.SetText("Set the style parameter to a value above 0 to tie this sheet to a hair track.");
                }
                else
                {
                    _hoverText.SetText("This sheet is only rendered when hairstyle " + (_hairType + 1) + " is selected on hair track #" + (_trackType + 1) + ".");
                }
            }
            if (listeningElement == genderButton)
            {
                if (_genderType == 0)
                {
                    _hoverText.SetText("This sheet can be overridden on female players by a matching sheet with female set to 'yes'.");
                }
                else
                {
                    _hoverText.SetText("This sheet replaces its male variant on female players.");
                }
            }
            ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        public void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            _hoverText.SetText(Language.GetText("Workshop.HubDescriptionDefault"));
            ((UIPanel)evt.Target).BorderColor = new Color(82, 96, 145);
        }

        public void BackPanelMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            if ((evt.Target) == backPanel || (evt.Target) == fileText || (evt.Target) == typeText || (evt.Target) == colorText || (evt.Target) == hairText || (evt.Target) == trackText || (evt.Target) == genderText)
            {
                _hoverText.SetText(sheetEnabled ? "Click to disable this sheet." : "Click to enable this sheet.");
                if (UICreateRace.DuplicateSheetExists(_sheetType, _colorType, _hairType, _trackType, _genderType))
                {
                    string overwriteWarning = $"WARNING: This sheet & sheet {UICreateRace.GetDuplicateSheet(_sheetIndex, _sheetType, _colorType, _hairType, _trackType, _genderType) + 1}'s parameters are identical. Change any parameter on either sheet to avoid an overwrite.";
                    _hoverText.SetText(overwriteWarning);
                }
                backPanel.BorderColor = new Color(89, 116, 213);
                backPanel.BackgroundColor = new Color(73, 94, 171);
            }
            else
            {
                backPanel.BorderColor = new Color(89, 116, 213) * 0.7f;
                backPanel.BackgroundColor = origColor;
            }
        }

        public void BackPanelMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            if ((evt.Target) == backPanel || (evt.Target) == fileText || (evt.Target) == typeText || (evt.Target) == colorText || (evt.Target) == hairText || (evt.Target) == trackText || (evt.Target) == genderText)
            {
                _hoverText.SetText(Language.GetText("Workshop.HubDescriptionDefault"));
                backPanel.BackgroundColor = origColor;
                backPanel.BorderColor = new Color(89, 116, 213) * 0.7f;
            }
        }
        public void Click_ToggleSheet(UIMouseEvent evt, UIElement listeningElement)
        {
            if ((evt.Target) == backPanel || (evt.Target) == fileText || (evt.Target) == typeText || (evt.Target) == colorText || (evt.Target) == hairText || (evt.Target) == trackText || (evt.Target) == genderText)
            {
                var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
                SoundEngine.PlaySound(SoundID.MenuTick);
                sheetEnabled = !sheetEnabled;
                _hoverText.SetText(sheetEnabled ? "Click to disable this sheet." : "Click to enable this sheet.");
                if (UICreateRace.DuplicateSheetExists(_sheetType, _colorType, _hairType, _trackType, _genderType))
                {
                    string overwriteWarning = $"WARNING: This sheet & sheet {UICreateRace.GetDuplicateSheet(_sheetIndex, _sheetType, _colorType, _hairType, _trackType, _genderType) + 1}'s parameters are identical. Change a parameter on either sheet to prevent an overwrite.";
                    _hoverText.SetText(overwriteWarning);
                }
                if (sheetEnabled)
                {
                    if (_filePath != null)
                    {
                        if (isReceivingFileFromModContent)
                        {
                            mrPlagueRacesPlayer.race.SetOverrideTextureFromModContent(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                        }
                        else
                        {
                            mrPlagueRacesPlayer.race.SetOverrideTextureFromFilePath(_filePath, _sheetType, _colorType, _hairType, _trackType, _genderType);
                        }
                    }
                }
                else
                {
                    mrPlagueRacesPlayer.race.ClearOverrideTexture(_sheetType, _colorType, _hairType, _trackType, _genderType);
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            fileText.SetText("Sheet " + (_sheetIndex + 1).ToString());
            hasDuplicateSheet = UICreateRace.DuplicateSheetExists(_sheetType, _colorType, _hairType, _trackType, _genderType);
            if (backPanel.BorderColor != new Color(89, 116, 213) && backPanel.BorderColor != new Color(133, 53, 46) && backPanel.BorderColor != new Color(165, 70, 170))
            {
                if (sheetEnabled)
                {

                    if (hasDuplicateSheet)
                    {
                        backPanel.BackgroundColor = new Color(121, 52, 125) * 0.7f;
                        backPanel.BorderColor = new Color(165, 70, 170) * 0.7f;
                    }
                    else
                    {
                        backPanel.BackgroundColor = origColor;
                        backPanel.BorderColor = new Color(89, 116, 213) * 0.7f;
                    }
                }
                else
                {
                    backPanel.BackgroundColor = new Color(99, 34, 29) * 0.7f;
                    backPanel.BorderColor = new Color(133, 53, 46) * 0.7f;
                }
            }
            else
            {
                if (sheetEnabled)
                {
                    if (hasDuplicateSheet)
                    {
                        backPanel.BackgroundColor = new Color(121, 52, 125);
                        backPanel.BorderColor = new Color(165, 70, 170);
                    }
                    else
                    {
                        backPanel.BackgroundColor = new Color(73, 94, 171);
                        backPanel.BorderColor = new Color(89, 116, 213);
                    }
                }
                else
                {
                    backPanel.BackgroundColor = new Color(99, 34, 29);
                    backPanel.BorderColor = new Color(133, 53, 46);
                }
            }
                base.DrawSelf(spriteBatch);
        }

        public void AddDeleteOver(UIMouseEvent evt, UIElement listeningElement)
        {
            if (listeningElement == addButton)
            {
                _hoverText.SetText("Add a new sheet below this one.");
            }
            if (listeningElement == deleteButton)
            {
                if (UICreateRace.sheetCount > 1)
                {
                    _hoverText.SetText("Delete this sheet.");
                }
                else
                {
                    _hoverText.SetText("Can't remove the last sheet.");
                }
            }
        }

        public void AddDeleteOut(UIMouseEvent evt, UIElement listeningElement)
        {
            _hoverText.SetText(Language.GetText("Workshop.HubDescriptionDefault"));
        }
    }
}
