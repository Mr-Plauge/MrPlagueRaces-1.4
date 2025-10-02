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
using Terraria.Utilities.FileBrowser;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.UI.Elements;

namespace MrPlagueRaces.Common.UI.States
{
	public class UIRaceSoundPanel : UIElement
    {
        public UIPanel soundPanel;
        public UITextPanel<string> maleHurtText;
        public UITextPanel<string> femaleHurtText;
        public UITextPanel<string> deathText;

        public UITextPanel<string> maleHurtButton;
        public UITextPanel<string> femaleHurtButton;
        public UITextPanel<string> deathButton;

        public Player _player;
        public UIText _hoverText;

        public string _maleHurtName;
        public string _femaleHurtName;
        public string _deathName;

        public UIRaceColoredImageButton templateDownloadButton;
        public UIRaceSoundPanel(Player player, ref UIText hoverText)
        {
            _player = player;
            _hoverText = hoverText;

            _maleHurtName = Path.GetFileName(UICreateRace.maleHurtPath);
            _femaleHurtName = Path.GetFileName(UICreateRace.femaleHurtPath);
            _deathName = Path.GetFileName(UICreateRace.deathPath);

            UIPanel soundPanel = new UIPanel()
            {
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixels(60f),
                Top = StyleDimension.FromPixelsAndPercent(4f, 0f)
            };
            Append(soundPanel);
            soundPanel.SetPadding(0f);

            maleHurtText = new UITextPanel<string>("Hurt (Male)");
            maleHurtText.TextColor = Color.White;
            maleHurtText.TextScale = 0.8f;
            maleHurtText.SetPadding(7f);
            maleHurtText.Width.Set(105f, 0f);
            maleHurtText.Height.Set(30f, 0f);
            maleHurtText.BackgroundColor = Color.Transparent;
            maleHurtText.BorderColor = Color.Transparent;
            maleHurtText.Left = StyleDimension.FromPixelsAndPercent(7f, 0f);
            maleHurtText.Top = StyleDimension.FromPixelsAndPercent(0f, 0f);
            maleHurtText.TextHAlign = 0f;
            soundPanel.Append(maleHurtText);

            femaleHurtText = new UITextPanel<string>("Hurt (Female)");
            femaleHurtText.TextColor = Color.White;
            femaleHurtText.TextScale = 0.8f;
            femaleHurtText.SetPadding(7f);
            femaleHurtText.Width.Set(105f, 0f);
            femaleHurtText.Height.Set(30f, 0f);
            femaleHurtText.BackgroundColor = Color.Transparent;
            femaleHurtText.BorderColor = Color.Transparent;
            femaleHurtText.Left = StyleDimension.FromPixelsAndPercent(7f + 160f, 0f);
            femaleHurtText.Top = StyleDimension.FromPixelsAndPercent(0, 0f);
            femaleHurtText.TextHAlign = 0f;
            soundPanel.Append(femaleHurtText);

            deathText = new UITextPanel<string>("Killed");
            deathText.TextColor = Color.White;
            deathText.TextScale = 0.8f;
            deathText.SetPadding(7f);
            deathText.Width.Set(105f, 0f);
            deathText.Height.Set(30f, 0f);
            deathText.BackgroundColor = Color.Transparent;
            deathText.BorderColor = Color.Transparent;
            deathText.Left = StyleDimension.FromPixelsAndPercent(7f + 160f + 160f, 0f);
            deathText.Top = StyleDimension.FromPixelsAndPercent(0f, 0f);
            deathText.TextHAlign = 0f;
            soundPanel.Append(deathText);

            maleHurtButton = new UITextPanel<string>(_maleHurtName == null ? "Upload (wav)" : _maleHurtName);
            maleHurtButton.TextColor = Color.Gray;
            maleHurtButton.SetPadding(7f);
            maleHurtButton.Width.Set(155f, 0f);
            maleHurtButton.Height.Set(30f, 0f);
            maleHurtButton.BackgroundColor = new Color(31, 41, 76);
            maleHurtButton.BorderColor = new Color(82, 96, 145);
            maleHurtButton.Left = StyleDimension.FromPixelsAndPercent(7f, 0f);
            maleHurtButton.Top = StyleDimension.FromPixelsAndPercent(23f, 0f);
            maleHurtButton.TextHAlign = 0.1f;
            maleHurtButton.OnMouseOver += FadedMouseOver;
            maleHurtButton.OnMouseOut += FadedMouseOut;
            maleHurtButton.OnLeftClick += Click_UploadMaleHurt;
            maleHurtButton.OnRightClick += Click_ResetMaleHurt;
            maleHurtButton.OnLeftClick += FadedMouseOver;
            maleHurtButton.OnRightClick += FadedMouseOver;
            soundPanel.Append(maleHurtButton);

            femaleHurtButton = new UITextPanel<string>(_femaleHurtName == null ? "Upload (wav)" : _femaleHurtName);
            femaleHurtButton.TextColor = Color.Gray;
            femaleHurtButton.SetPadding(7f);
            femaleHurtButton.Width.Set(155f, 0f);
            femaleHurtButton.Height.Set(30f, 0f);
            femaleHurtButton.BackgroundColor = new Color(31, 41, 76);
            femaleHurtButton.BorderColor = new Color(82, 96, 145);
            femaleHurtButton.Left = StyleDimension.FromPixelsAndPercent(7f + 160f, 0f);
            femaleHurtButton.Top = StyleDimension.FromPixelsAndPercent(23f, 0f);
            femaleHurtButton.TextHAlign = 0.1f;
            femaleHurtButton.OnMouseOver += FadedMouseOver;
            femaleHurtButton.OnMouseOut += FadedMouseOut;
            femaleHurtButton.OnLeftClick += Click_UploadFemaleHurt;
            femaleHurtButton.OnRightClick += Click_ResetFemaleHurt;
            femaleHurtButton.OnLeftClick += FadedMouseOver;
            femaleHurtButton.OnRightClick += FadedMouseOver;
            soundPanel.Append(femaleHurtButton);

            deathButton = new UITextPanel<string>(_deathName == null ? "Upload (wav)" : _deathName);
            deathButton.TextColor = Color.Gray;
            deathButton.SetPadding(7f);
            deathButton.Width.Set(155f, 0f);
            deathButton.Height.Set(30f, 0f);
            deathButton.BackgroundColor = new Color(31, 41, 76);
            deathButton.BorderColor = new Color(82, 96, 145);
            deathButton.Left = StyleDimension.FromPixelsAndPercent(7f + 160f + 160f, 0f);
            deathButton.Top = StyleDimension.FromPixelsAndPercent(23f, 0f);
            deathButton.TextHAlign = 0.1f;
            deathButton.OnMouseOver += FadedMouseOver;
            deathButton.OnMouseOut += FadedMouseOut;
            deathButton.OnLeftClick += Click_UploadDeath;
            deathButton.OnRightClick += Click_ResetDeath;
            deathButton.OnLeftClick += FadedMouseOver;
            deathButton.OnRightClick += FadedMouseOver;
            soundPanel.Append(deathButton);

            templateDownloadButton = new UIRaceColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/TemplateDownload", (AssetRequestMode)1), false, false, false);
            templateDownloadButton.Left.Set(7f + 160f + 160f + 160f + 2f, 0f);
            templateDownloadButton.VAlign = 0.5f;
            templateDownloadButton.OnMouseOver += FadedMouseOver;
            templateDownloadButton.OnMouseOut += FadedMouseOut;
            templateDownloadButton.OnLeftClick += Click_DownloadSheets;
            soundPanel.Append(templateDownloadButton);
        }

        public string FilePathFromUser()
        {
            ExtensionFilter[] extensions = new ExtensionFilter[1]
            {
                new ExtensionFilter("Audio files", "wav")
            };
            string myAudioFile = FileBrowser.OpenFilePanel("Select audio file", extensions);
            if (myAudioFile != null)
            {
                return myAudioFile;
            }
            return null;
        }

        public void Click_UploadMaleHurt (UIMouseEvent evt, UIElement listeningElement)
        {
            UICreateRace.maleHurtPath = FilePathFromUser();
            _maleHurtName = Path.GetFileName(UICreateRace.maleHurtPath);
            maleHurtButton.SetText(_maleHurtName);
        }

        public void Click_UploadFemaleHurt(UIMouseEvent evt, UIElement listeningElement)
        {
            UICreateRace.femaleHurtPath = FilePathFromUser();
            _femaleHurtName = Path.GetFileName(UICreateRace.femaleHurtPath);
            femaleHurtButton.SetText(_femaleHurtName);
        }

        public void Click_UploadDeath(UIMouseEvent evt, UIElement listeningElement)
        {
            UICreateRace.deathPath = FilePathFromUser();
            _deathName = Path.GetFileName(UICreateRace.deathPath);
            deathButton.SetText(_deathName);
        }

        public void Click_ResetMaleHurt(UIMouseEvent evt, UIElement listeningElement)
        {
            UICreateRace.maleHurtPath = null;
            maleHurtButton.SetText("Upload (wav)");
        }

        public void Click_ResetFemaleHurt(UIMouseEvent evt, UIElement listeningElement)
        {
            UICreateRace.femaleHurtPath = null;
            femaleHurtButton.SetText("Upload (wav)");
        }

        public void Click_ResetDeath(UIMouseEvent evt, UIElement listeningElement)
        {
            UICreateRace.deathPath = null;
            deathButton.SetText("Upload (wav)");
        }

        public void Click_DownloadSheets(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            string sourceFolder = Path.Combine(Program.SavePathShared, "TemplateRaceSheets");
            Directory.CreateDirectory(sourceFolder);
            File.WriteAllText(Path.Combine(sourceFolder, "INSTRUCTIONS.txt"), GetTemplateInstructions());

            WriteSheetToFolder("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/Arms", sourceFolder);
            WriteSheetToFolder("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/Body", sourceFolder);
            WriteSheetToFolder("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/EyeLids", sourceFolder);
            WriteSheetToFolder("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/Hands", sourceFolder);
            WriteSheetToFolder("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/Head", sourceFolder);
            WriteSheetToFolder("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorSkin/Legs", sourceFolder);

            WriteSheetToFolder("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorEyes/Eyes", sourceFolder);
            WriteSheetToFolder("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/Colorless/Eyes", sourceFolder, "_Whites");

            WriteSheetToFolder("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorHair/Hairstyles/Hair_1", sourceFolder);
            WriteSheetToFolder("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorHair/Hairstyles/Hair_2", sourceFolder);
            WriteSheetToFolder("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/ColorHair/Hairstyles/Hair_3", sourceFolder);

            WriteSheetToFolder("MrPlagueRaces/Assets/Textures/Players/Races/Human/Female/ColorSkin/Legs", sourceFolder, "_Female");

            File.WriteAllBytes(Path.Combine(sourceFolder, "Hurt.wav"), ModContent.GetFileBytes("MrPlagueRaces/Assets/Sounds/Players/Races/Tabaxi/Male/Hurt.wav"));
            File.WriteAllBytes(Path.Combine(sourceFolder, "Killed.wav"), ModContent.GetFileBytes("MrPlagueRaces/Assets/Sounds/Players/Races/Derpkin/Male/Killed.wav"));

            File.WriteAllBytes(Path.Combine(sourceFolder, "Hurt_Female.wav"), ModContent.GetFileBytes("MrPlagueRaces/Assets/Sounds/Players/Races/Tabaxi/Female/Hurt.wav"));

            Utils.OpenFolder(sourceFolder);
        }

        public void WriteSheetToFolder(string sheetPath, string sourceFolder, string addition = "")
        {
            FileStream stream = null;
            Texture2D sheet = ModContent.Request<Texture2D>(sheetPath, AssetRequestMode.ImmediateLoad).Value;
            using (stream = File.OpenWrite(Path.Combine(sourceFolder, Path.GetFileName(sheetPath) + addition + ".png")))
            {
                sheet.SaveAsPng(stream, sheet.Width, sheet.Height);
            }
        }

        public string GetTemplateInstructions()
        {
            string instructionText = "";
            instructionText += "DISCLAIMER:";
            instructionText += "\nTAKE EVERYTHING OUT OF THIS FOLDER AND PUT IT SOMEWHERE ELSE!";
            instructionText += "\nOTHERWISE, IT WILL BE OVERRIDDEN WHEN TEMPLATE SHEETS ARE NEXT DOWNLOADED.";
            instructionText += "\n";
            instructionText += "\nThe player is composed of seven sheets that are layered on top of each other.";
            instructionText += "\n";
            instructionText += "\nArms (The player's arms)";
            instructionText += "\nHands (The player's hands)";
            instructionText += "\nHair (The player's hair)";
            instructionText += "\nHairAlt (The player's hair while wearing certain hats)";
            instructionText += "\nBody (The player's body)";
            instructionText += "\nEyes (The player's eyes)";
            instructionText += "\nHead (The player's head)";
            instructionText += "\nLegs (The player's legs)";
            instructionText += "\n";
            instructionText += "\nThese sheets are drawn in grayscale, which allows them to adopt colors from ingame customization.";
            instructionText += "\n";
            instructionText += "\nThe rendering priority of a sheet is based on its assigned color in the following order: Skin Color -> Detail Color(s) -> Colorless -> Eye Color -> Hair Color -> Glowing Skin Color -> Glowing Detail Color(s) -> Glowing Colorless -> Glowing Eye Color -> Glowing Hair Color. Keep this property in mind when layering colored sheets on top of each other.";
            instructionText += "\n";
            instructionText += "\nMultiple sheets of the same type (IE, head) can coexist so long as 1 or more of their parameters are not identical (IE, a detail-colored head sheet and a skin-colored head sheet). This can be used to create separately colored underbellies, beaks, etc. For examples of this layering technique in use, view Dragonkin's textures folder in the extracted source of MrPlagueRaces.";
            instructionText += "\n";
            instructionText += "\nCustom familiar clothing sheets can be inserted once you have compiled your mod source. For examples of how to implement custom familiar clothing sheets, view Kenku's textures folder in the extracted source of MrPlagueRaces.";
            instructionText += "\n";
            instructionText += "\nHair/HairAlt sheet lists must not have any gaps (IE, skipping from Style = 1 to Style = 3). If a gap exists, the list will be previewed incorrectly in the UI.";
            instructionText += "\n";
            instructionText += "\nAseprite ($19.99) or Piskel (free) are recommended programs for designing player sheets.";
            return instructionText;
        }

        public void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (listeningElement == maleHurtButton)
            {
                _hoverText.SetText("Upload a male hurt sound for your race. Leave blank to skip.");
                ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
            }
            if (listeningElement == femaleHurtButton)
            {
                _hoverText.SetText("Upload a female hurt sound for your race. Leave blank to skip (will default to male hurt sound).");
                ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
            }
            if (listeningElement == deathButton)
            {
                _hoverText.SetText("Upload a death sound for your race. Leave blank to skip.");
                ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
            }
            if (listeningElement == templateDownloadButton)
            {
                _hoverText.SetText("Download a set of template sheets & instructions to begin designing your race's appearance.");
            }
        }

        public void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            _hoverText.SetText(Language.GetText("Workshop.HubDescriptionDefault"));
            if (listeningElement == maleHurtButton || listeningElement == femaleHurtButton || listeningElement == deathButton)
            {
                ((UIPanel)evt.Target).BorderColor = new Color(82, 96, 145);
            }
        }
    }
}
