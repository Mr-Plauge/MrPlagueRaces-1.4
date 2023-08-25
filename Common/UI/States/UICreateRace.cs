using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Core;
using Terraria.UI;
using Terraria.UI.Gamepad;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria;
using MrPlagueRaces.Common.UI.Elements;

namespace MrPlagueRaces.Common.UI.States
{
	public class UICreateRace : UIState, IHaveBackButtonCommand
	{
		private UIElement _baseElement;

		private UITextPanel<string> _messagePanel;

		private UIFocusInputTextField_mp _modName;

		private UIFocusInputTextField_mp _modDisplayName;

		private UIFocusInputTextField_mp _modAuthor;

		private UIFocusInputTextField_mp _raceName;

		private UIFocusInputTextField_mp _raceDisplayName;

		private string lastKnownMessage = "";

		public UIState PreviousUIState
		{
			get;
			set;
		}

		public override void OnInitialize()
		{
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
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
			UITextPanel<string> uITextPanel = new UITextPanel<string>("Create Race", 0.8f, large: true)
			{
				HAlign = 0.5f,
				Top = 
				{
					Pixels = -35f
				},
				BackgroundColor = UICommon.DefaultUIBlue
			}.WithPadding(15f);
			_baseElement.Append(uITextPanel);
			_messagePanel = new UITextPanel<string>(Language.GetTextValue(""))
			{
				Width = 
				{
					Percent = 1f
				},
				Height = 
				{
					Pixels = 25f
				},
				VAlign = 1f,
				Top = 
				{
					Pixels = -20f
				}
			};
			UITextPanel<string> buttonBack = new UITextPanel<string>(Language.GetTextValue("UI.Back"))
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
			buttonBack.OnLeftClick += BackClick;
			_baseElement.Append(buttonBack);
			UITextPanel<string> buttonCreate = new UITextPanel<string>(Language.GetTextValue("LegacyMenu.28"));
			buttonCreate.CopyStyle(buttonBack);
			buttonCreate.HAlign = 1f;
			buttonCreate.WithFadedMouseOver();
			buttonCreate.OnLeftClick += OKClick;
			_baseElement.Append(buttonCreate);
			float top = 16f;
			_modName = createAndAppendTextInputWithLabel("Internal Mod Name (no spaces)", "Type here");
			_modName.OnTextChange += delegate
			{
				_modName.SetText(_modName.CurrentString.Replace(" ", ""));
			};
			_raceName = createAndAppendTextInputWithLabel("Internal Race Name (no spaces)", "Type Here");
			_modDisplayName = createAndAppendTextInputWithLabel("Mod Display Name", "Type here");
			_raceDisplayName = createAndAppendTextInputWithLabel("Race Display Name", "Leave Blank to Skip");
			_modAuthor = createAndAppendTextInputWithLabel("Mod Author", "Type here");
			_modName.OnTab += delegate
			{
				_modDisplayName.Focused = true;
			};
			_modDisplayName.OnTab += delegate
			{
				_raceName.Focused = true;
			};
			_raceName.OnTab += delegate
			{
				_raceDisplayName.Focused = true;
			};
			_raceDisplayName.OnTab += delegate
			{
				_modAuthor.Focused = true;
			};
			_modAuthor.OnTab += delegate
			{
				_modName.Focused = true;
			};
			UIFocusInputTextField_mp createAndAppendTextInputWithLabel(string label, string hint)
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
				panel.Append(modNameText);
				UIPanel textBoxBackground = new UIPanel();
				textBoxBackground.SetPadding(0f);
				textBoxBackground.Top.Set(6f, 0f);
				textBoxBackground.Left.Set(0f, 0.5f);
				textBoxBackground.Width.Set(0f, 0.5f);
				textBoxBackground.Height.Set(30f, 0f);
				panel.Append(textBoxBackground);
				UIFocusInputTextField_mp uIInputTextField = new UIFocusInputTextField_mp(hint)
				{
					UnfocusOnTab = true
				};
				uIInputTextField.Top.Set(5f, 0f);
				uIInputTextField.Left.Set(10f, 0f);
				uIInputTextField.Width.Set(-20f, 1f);
				uIInputTextField.Height.Set(20f, 0f);
				textBoxBackground.Append(uIInputTextField);
				mainPanel.Append(panel);
				return uIInputTextField;
			}
		}

		public override void OnActivate()
		{
			base.OnActivate();
			_modName.SetText("");
			_modDisplayName.SetText("");
			_modAuthor.SetText("");
			_raceName.SetText("");
			_raceDisplayName.SetText("");
			_messagePanel.SetText("");
			_modName.Focused = true;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (lastKnownMessage != _messagePanel.Text)
			{
				lastKnownMessage = _messagePanel.Text;
				if (string.IsNullOrEmpty(_messagePanel.Text))
				{
					_baseElement.RemoveChild(_messagePanel);
				}
				else
				{
					_baseElement.Append(_messagePanel);
				}
			}
		}

		private void BackClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HandleBackButtonUsage();
		}

		public void HandleBackButtonUsage()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			SoundEngine.PlaySound(in SoundID.MenuClose);
			Main.menuMode = 10001;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			UILinkPointNavigator.Shortcuts.BackButtonCommand = 7;
		}

		private void OKClick(UIMouseEvent evt, UIElement listeningElement)
		{
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				string modNameTrimmed = _modName.CurrentString.Trim();
				string raceNameTrimmed = _raceName.CurrentString.Trim();
				string sourceFolder = Path.Combine(Path.Combine(Program.SavePathShared, "ModSources"), modNameTrimmed);
				CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
				if (Directory.Exists(sourceFolder))
				{
					_messagePanel.SetText("Folder already exists");
				}
				else if (!provider.IsValidIdentifier(modNameTrimmed))
				{
					_messagePanel.SetText("ModName is invalid C# identifier. Remove spaces.");
				}
				else if (modNameTrimmed.Equals("Mod", StringComparison.InvariantCultureIgnoreCase) || modNameTrimmed.Equals("ModLoader", StringComparison.InvariantCultureIgnoreCase) || modNameTrimmed.Equals("tModLoader", StringComparison.InvariantCultureIgnoreCase))
				{
					_messagePanel.SetText("ModName is a reserved mod name. Choose a different name.");
				}
				else if (!provider.IsValidIdentifier(raceNameTrimmed))
				{
					_messagePanel.SetText("RaceName is invalid C# identifier. Remove spaces.");
				}
				else if (string.IsNullOrWhiteSpace(_modDisplayName.CurrentString))
				{
					_messagePanel.SetText("DisplayName can't be empty");
				}
				else if (string.IsNullOrWhiteSpace(_modAuthor.CurrentString))
				{
					_messagePanel.SetText("Author can't be empty");
				}
				else if (string.IsNullOrWhiteSpace(_raceName.CurrentString))
				{
					_messagePanel.SetText("RaceName can't be empty");
				}
				else
				{
					Directory.CreateDirectory(sourceFolder);
					File.WriteAllText(Path.Combine(sourceFolder, "build.txt"), GetModBuild());
					File.WriteAllText(Path.Combine(sourceFolder, "description.txt"), GetModDescription());

					string commonPath = Path.Combine(sourceFolder, "Common/Races/" + raceNameTrimmed);
					string assetPath = Path.Combine(sourceFolder, "Assets/Textures/Players/Races/" + raceNameTrimmed);
					string malePath = Path.Combine(assetPath, "Male");
					string femalePath = Path.Combine(assetPath, "Female");

					Directory.CreateDirectory(commonPath);
					Directory.CreateDirectory(malePath);
					Directory.CreateDirectory(femalePath);

					string colorDetail_male = Path.Combine(malePath, "ColorDetail");
					Directory.CreateDirectory(colorDetail_male);
					string colorEyes_male = Path.Combine(malePath, "ColorEyes");
					Directory.CreateDirectory(colorEyes_male);
					string colorHair_male = Path.Combine(malePath, "ColorHair/Hairstyles");
					Directory.CreateDirectory(colorHair_male);
					string colorSkin_male = Path.Combine(malePath, "ColorSkin");
					Directory.CreateDirectory(colorSkin_male);
					string colorless_male = Path.Combine(malePath, "Colorless");
					Directory.CreateDirectory(colorless_male);
					string colorSkin_female = Path.Combine(femalePath, "ColorSkin");
					Directory.CreateDirectory(colorSkin_female);

					
					Texture2D[] sheetIndex = new Texture2D[12];
					FileStream stream = null;
					string[] sheetNames = { "ColorSkin/Arms", "ColorSkin/Body", "ColorSkin/EyeLids", "ColorSkin/Hands", "ColorSkin/Head", "ColorSkin/Legs", "Colorless/Eyes", "ColorEyes/Eyes", "ColorHair/Hairstyles/Hair_1", "ColorHair/Hairstyles/Hair_2", "ColorHair/Hairstyles/Hair_3"};
					for (int i = 0; i < 11; i++)
					{
						sheetIndex[i] = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Players/Races/Human/Male/" + sheetNames[i], AssetRequestMode.ImmediateLoad).Value;
						using (stream = File.OpenWrite(Path.Combine(malePath, sheetNames[i] + ".png"))) {
							sheetIndex[i].SaveAsPng(stream, sheetIndex[i].Width, sheetIndex[i].Height);
						}
					}
					Texture2D icon_blank = ModContent.Request<Texture2D>("MrPlagueRaces/icon_blank", AssetRequestMode.ImmediateLoad).Value;
					using (stream = File.OpenWrite(Path.Combine(sourceFolder, "icon.png"))) {
						icon_blank.SaveAsPng(stream, icon_blank.Width, icon_blank.Height);
					}
					Texture2D legs_female = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Players/Races/Human/Female/ColorSkin/Legs", AssetRequestMode.ImmediateLoad).Value;
					using (stream = File.OpenWrite(Path.Combine(femalePath, "ColorSkin/Legs.png"))) {
						legs_female.SaveAsPng(stream, legs_female.Width, legs_female.Height);
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
					Main.menuMode = 10001;
				}
			}
			catch (Exception e)
			{
				//Logging.tML.Error((object)e);
				_messagePanel.SetText("There was an issue. Check client.log" + e);
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
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(789, 3);
			defaultInterpolatedStringHandler.AppendLiteral("using Microsoft.Xna.Framework;\r\nusing Microsoft.Xna.Framework.Graphics;\r\nusing System;\r\nusing System.Collections.Generic;\r\nusing Terraria;\r\nusing Terraria.DataStructures;\r\nusing Terraria.ID;\r\nusing Terraria.ModLoader;\r\nusing Terraria.ModLoader.IO;\r\nusing MrPlagueRaces.Common.Races;\r\n\r\nnamespace ");
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
			defaultInterpolatedStringHandler.AppendLiteral("Description = \"This is the default description of the ");
			defaultInterpolatedStringHandler.AppendFormatted(!string.IsNullOrEmpty(_raceDisplayName.CurrentString) ? _raceDisplayName.CurrentString : raceNameTrimmed);
			defaultInterpolatedStringHandler.AppendLiteral(" race.\";\r\n\t\t\tAbilitiesDescription = \"Describe unique racial abilities here.\";\r\n\t\t\tStarterShirt = true;\r\n\t\t\tStarterPants = true;\r\n\t\t\tHairColor = new Color(255, 255, 255);\r\n\t\t\tSkinColor = new Color(255, 255, 255);\r\n\t\t\tDetailColor = new Color(255, 255, 255);\r\n\t\t\tEyeColor = new Color(255, 255, 255);\r\n\t\t\tShirtColor = new Color(255, 255, 255);\r\n\t\t\tUnderShirtColor = new Color(255, 255, 255);\r\n\t\t\tPantsColor = new Color(255, 255, 255);\r\n\t\t\tShoeColor = new Color(255, 255, 255);\r\n\t\t}\r\n\t}\r\n}");
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
			if (string.IsNullOrEmpty(raceNameTrimmed))
			{
				return "# This file will automatically update with entries for new content after a build and reload";
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(151, 2);
			defaultInterpolatedStringHandler.AppendLiteral("# This file will automatically update with entries for new content after a build and reload\r\n\r\nItems: {\r\n\t");
			defaultInterpolatedStringHandler.AppendFormatted(raceNameTrimmed);
			defaultInterpolatedStringHandler.AppendLiteral(": {\r\n\t\tDisplayName: ");
			defaultInterpolatedStringHandler.AppendFormatted(Regex.Replace(raceNameTrimmed, "([A-Z])", " $1").Trim());
			defaultInterpolatedStringHandler.AppendLiteral("\r\n\t\tTooltip: \"\"\r\n\t}\r\n} \r\n");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		/*internal string GetBasicRace(string modNameTrimmed, string basicSwordName)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(789, 3);
			defaultInterpolatedStringHandler.AppendLiteral("using Terraria;\r\nusing Terraria.ID;\r\nusing Terraria.ModLoader;\r\n\r\nnamespace ");
			defaultInterpolatedStringHandler.AppendFormatted(modNameTrimmed);
			defaultInterpolatedStringHandler.AppendLiteral(".Items\r\n{\r\n\tpublic class ");
			defaultInterpolatedStringHandler.AppendFormatted(basicSwordName);
			defaultInterpolatedStringHandler.AppendLiteral(" : ModItem\r\n\t{\r\n        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.");
			defaultInterpolatedStringHandler.AppendFormatted(modNameTrimmed);
			defaultInterpolatedStringHandler.AppendLiteral(".hjson file.\r\n\r\n\t\tpublic override void SetDefaults()\r\n\t\t{\r\n\t\t\tItem.damage = 50;\r\n\t\t\tItem.DamageType = DamageClass.Melee;\r\n\t\t\tItem.width = 40;\r\n\t\t\tItem.height = 40;\r\n\t\t\tItem.useTime = 20;\r\n\t\t\tItem.useAnimation = 20;\r\n\t\t\tItem.useStyle = 1;\r\n\t\t\tItem.knockBack = 6;\r\n\t\t\tItem.value = 10000;\r\n\t\t\tItem.rare = 2;\r\n\t\t\tItem.UseSound = SoundID.Item1;\r\n\t\t\tItem.autoReuse = true;\r\n\t\t}\r\n\r\n\t\tpublic override void AddRecipes()\r\n\t\t{\r\n\t\t\tRecipe recipe = CreateRecipe();\r\n\t\t\trecipe.AddIngredient(ItemID.DirtBlock, 10);\r\n\t\t\trecipe.AddTile(TileID.WorkBenches);\r\n\t\t\trecipe.Register();\r\n\t\t}\r\n\t}\r\n}");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}*/
	}
}
