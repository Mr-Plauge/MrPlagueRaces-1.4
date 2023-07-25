using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using ReLogic.Content;
using ReLogic.Graphics;
using ReLogic.OS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Initializers;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Gamepad;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.UI.Elements;

namespace MrPlagueRaces.Common.UI.States
{
	public class MrPlagueUICharacterCreation : UIState
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

		private readonly Player _player;

		private UIColoredImageButton[] _colorPickers;

		private CategoryId _selectedPicker;

		private Vector3 _currentColorHSL;

		private UIColoredImageButton _clothingStylesCategoryButton;

		private UIColoredImageButton _hairStylesCategoryButton;

		private UIColoredImageButton _charInfoCategoryButton;

		private UIColoredImageButton _raceSelectCategoryButton;

		private UIElement _topContainer;

		private UIElement _middleContainer;

		private UIElement _hslContainer;

		private UIElement _hairstylesContainer;

		private UIElement _clothStylesContainer;

		private UIElement _raceSelectContainer;

		private UIElement _infoContainer;

		private UIText _hslHexText;

		private UIText _difficultyDescriptionText;

		private UIElement _copyHexButton;

		private UIElement _pasteHexButton;

		private UIElement _randomColorButton;

		private UIElement _copyTemplateButton;

		private UIElement _pasteTemplateButton;

		private UIElement _randomizePlayerButton;

		private UIElement statContainer;

		private UIColoredImageButton _genderMale;

		private UIColoredImageButton _genderFemale;

		private UICharacterNameButton _charName;

		private UIText _helpGlyphLeft;

		private UIText _helpGlyphRight;

		private UIRaceInfoPanel uIRaceInfoPanel;

		public const int MAX_NAME_LENGTH = 20;

		public bool raceStatMenu = false;

		private UIGamepadHelper _helper;

		private List<int> _foundPoints = new List<int>();

		private UITextPanel<string> statHoverText;

		public static string hoverText = "";

		public MrPlagueUICharacterCreation(Player player)
		{
			int[] male = { 0, 2, 1, 3, 8 };
			Item familiarShirt = new Item();
			familiarShirt.SetDefaults(ItemID.FamiliarShirt);
			Item familiarPants = new Item();
			familiarPants.SetDefaults(ItemID.FamiliarPants);
			_player = player;
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
			_player.skinVariant = male[mrPlagueRacesPlayer.race.ClothStyle - 1];
			_player.hair = mrPlagueRacesPlayer.race.HairStyle;
			_player.hairColor = mrPlagueRacesPlayer.race.HairColor;
			_player.skinColor = mrPlagueRacesPlayer.race.SkinColor;
			mrPlagueRacesPlayer.detailColor = mrPlagueRacesPlayer.race.DetailColor;
			_player.eyeColor = mrPlagueRacesPlayer.race.EyeColor;
			_player.shirtColor = mrPlagueRacesPlayer.race.ShirtColor;
			_player.underShirtColor = mrPlagueRacesPlayer.race.UnderShirtColor;
			_player.pantsColor = mrPlagueRacesPlayer.race.PantsColor;
			_player.shoeColor = mrPlagueRacesPlayer.race.ShoeColor;
			BuildPage();
		}

		private void BuildPage()
		{
			RemoveAllChildren();
			int path = 4;
			UIElement s = new UIElement
			{
				Width = StyleDimension.FromPixels(588f),
				Height = StyleDimension.FromPixels(380 + path),
				Top = StyleDimension.FromPixels(220f),
				HAlign = 0.5f,
				VAlign = 0f
			};
			s.SetPadding(0f);
			Append(s);
			UIPanel listenPort = new UIPanel
			{
				Width = StyleDimension.FromPercent(1f),
				Height = StyleDimension.FromPixels(s.Height.Pixels - 150f - (float)path),
				Top = StyleDimension.FromPixels(50f),
				BackgroundColor = new Color(33, 43, 79) * 0.8f
			};
			listenPort.SetPadding(0f);
			s.Append(listenPort);
			MakeBackAndCreatebuttons(s);
			MakeCharPreview(listenPort);
			UIElement modPack = new UIElement
			{
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(50f, 0f)
			};
			modPack.SetPadding(0f);
			modPack.PaddingTop = 4f;
			modPack.PaddingBottom = 0f;
			listenPort.Append(modPack);
			UIElement uIElement = new UIElement
			{
				Top = StyleDimension.FromPixelsAndPercent(modPack.Height.Pixels + 6f, 0f),
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(listenPort.Height.Pixels - 70f, 0f)
			};
			statHoverText = new UITextPanel<string>(hoverText)
			{
				Width = StyleDimension.FromPixelsAndPercent(1f, 0f),
				Height = StyleDimension.FromPixelsAndPercent(1f, 0f),
				BackgroundColor = Color.Transparent,
				BorderColor = Color.Transparent
			};
			Append(statHoverText);
			uIElement.SetPadding(0f);
			uIElement.PaddingTop = 3f;
			uIElement.PaddingBottom = 0f;
			listenPort.Append(uIElement);
			_topContainer = modPack;
			_middleContainer = uIElement;
			MakeInfoMenu(uIElement);
			MakeHSLMenu(uIElement);
			MakeHairstylesMenu(uIElement);
			MakeClothStylesMenu(uIElement);
			MakeRaceSelectMenu(uIElement);
			MakeCategoriesBar(modPack);
			Click_CharInfo(null, null);
		}

		private void MakeCharPreview(UIPanel container)
		{
			float iP = 70f;
			for (float serverPassword = 0f; serverPassword <= 1f; serverPassword += 1f)
			{
				UICharacter element = new UICharacter(_player, animated: true, hasBackPanel: false, 1.5f)
				{
					Width = StyleDimension.FromPixels(80f),
					Height = StyleDimension.FromPixelsAndPercent(80f, 0f),
					Top = StyleDimension.FromPixelsAndPercent(0f - iP, 0f),
					VAlign = 0f,
					HAlign = 0.5f
				};
				container.Append(element);
			}
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
			UIList netPlayers = new UIList
			{
				Width = StyleDimension.FromPixelsAndPercent(-18f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(-6f, 1f)
			};
			netPlayers.SetPadding(4f);
			s2.Append(netPlayers);
			UIScrollbar serverPassword = new UIScrollbar
			{
				HAlign = 1f,
				Height = StyleDimension.FromPixelsAndPercent(-30f, 1f),
				Top = StyleDimension.FromPixels(10f)
			};
			serverPassword.SetView(100f, 1000f);
			netPlayers.SetScrollbar(serverPassword);
			s2.Append(serverPassword);
			int text = mrPlagueRacesPlayer.GetRaceHairCount(_player);
			UIElement language = new UIElement
			{
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(48 * (text / 10 + ((text % 10 != 0) ? 1 : 0)), 0f)
			};
			netPlayers.Add(language);
			language.SetPadding(0f);
			for (int worldName = 0; worldName < text; worldName++)
			{
				UIHairStyleButton newMOTD = new UIHairStyleButton(_player, worldName)
				{
					Left = StyleDimension.FromPixels((float)(worldName % 10) * 54f + 6f),
					Top = StyleDimension.FromPixels((float)(worldName / 10) * 48f + 1f)
				};
				newMOTD.SetSnapPoint("Middle", worldName);
				language.Append(newMOTD);
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
					num2 = 20;
				}
				UIClothStyleButton uIClothStyleButton = new UIClothStyleButton(_player, _validClothStyles[j])
				{
					Left = StyleDimension.FromPixels((float)j * 46f + (float)num2 + 50f),
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
					num3 = 20;
				}
				UIHorizontalSeparator element = new UIHorizontalSeparator
				{
					Left = StyleDimension.FromPixels((float)k * 230f + (float)num3 + 50f),
					Top = StyleDimension.FromPixels(num + 86),
					Width = StyleDimension.FromPixelsAndPercent(230f, 0f),
					Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
				};
				i.Append(element);
				UIColoredImageButton uIColoredImageButton = CreatePickerWithoutClick(CategoryId.Clothing, "MrPlagueRaces/Assets/Textures/UI/" + ((k == 0) ? "ClothStyleMale" : "ClothStyleFemale"), 0f, 0f);
				uIColoredImageButton.Top = StyleDimension.FromPixelsAndPercent(num + 92, 0f);
				uIColoredImageButton.Left = StyleDimension.FromPixels((float)k * 230f + 126f + (float)num3 + 6f);
				uIColoredImageButton.HAlign = 0f;
				uIColoredImageButton.VAlign = 0f;
				i.Append(uIColoredImageButton);
				if (k == 0)
				{
					uIColoredImageButton.OnLeftMouseDown += Click_CharGenderMale;
					_genderMale = uIColoredImageButton;
				}
				else
				{
					uIColoredImageButton.OnLeftMouseDown += Click_CharGenderFemale;
					_genderFemale = uIColoredImageButton;
				}
				uIColoredImageButton.SetSnapPoint("Low", k * 4);
			}
			UIElement uIElement = new UIElement
			{
				Width = StyleDimension.FromPixels(130f),
				Height = StyleDimension.FromPixels(50f),
				HAlign = 0.5f,
				VAlign = 1f
			};
			i.Append(uIElement);
			UIColoredImageButton uIColoredImageButton2 = new UIColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Copy", (AssetRequestMode)1), isSmall: true)
			{
				VAlign = 0.5f,
				HAlign = 0f,
				Left = StyleDimension.FromPixelsAndPercent(0f, 0f)
			};
			uIColoredImageButton2.OnLeftMouseDown += Click_CopyPlayerTemplate;
			uIElement.Append(uIColoredImageButton2);
			_copyTemplateButton = uIColoredImageButton2;
			UIColoredImageButton uIColoredImageButton3 = new UIColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Paste", (AssetRequestMode)1), isSmall: true)
			{
				VAlign = 0.5f,
				HAlign = 0.5f
			};
			uIColoredImageButton3.OnLeftMouseDown += Click_PastePlayerTemplate;
			uIElement.Append(uIColoredImageButton3);
			_pasteTemplateButton = uIColoredImageButton3;
			UIColoredImageButton uIColoredImageButton4 = new UIColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Randomize", (AssetRequestMode)1), isSmall: true)
			{
				VAlign = 0.5f,
				HAlign = 1f
			};
			uIColoredImageButton4.OnLeftMouseDown += Click_RandomizePlayer;
			uIElement.Append(uIColoredImageButton4);
			_randomizePlayerButton = uIColoredImageButton4;
			uIColoredImageButton2.SetSnapPoint("Low", 1);
			uIColoredImageButton3.SetSnapPoint("Low", 2);
			uIColoredImageButton4.SetSnapPoint("Low", 3);
			_clothStylesContainer = i;
		}

		private void MakeRaceSelectMenu(UIElement middleInnerPanel)
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
			UIRaceNameButton uICharacterNameButton = new UIRaceNameButton(_player);
			uICharacterNameButton.Width = StyleDimension.FromPixelsAndPercent(0f, 0.5f);
			uICharacterNameButton.HAlign = 0.5f;
			uICharacterNameButton.Left = StyleDimension.FromPixels(-136);
			i.Append(uICharacterNameButton);
			float num = 4f;
			float num3 = 0.4f;
			UIElement uIElement = new UIElement
			{
				HAlign = 0f,
				VAlign = 1f,
				Width = StyleDimension.FromPixelsAndPercent(0f - num, num3),
				Height = StyleDimension.FromPixelsAndPercent(-50f, 1f)
			};
			uIElement.SetPadding(0f);
			i.Append(uIElement);
			UIPanel raceSelectPanel = new UIPanel
			{
				Width = StyleDimension.FromPixelsAndPercent(-310f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
				HAlign = 0.5f,
				VAlign = 0.5f,
				Left = StyleDimension.FromPixels(0f - num + 158f),
				BackgroundColor = Color.Transparent,
				BorderColor = Color.Transparent
			};
			raceSelectPanel.SetPadding(0f);
			i.Append(raceSelectPanel);
			UIList raceSelectList = new UIList
			{
				Width = StyleDimension.FromPixelsAndPercent(-18f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(-6f, 1f)
			};
			raceSelectPanel.Append(raceSelectList);
			UIScrollbar raceSelectScrollbar = new UIScrollbar
			{
				HAlign = 1f,
				Height = StyleDimension.FromPixelsAndPercent(-30f, 1f),
				Top = StyleDimension.FromPixels(10f)
			};
			raceSelectScrollbar.SetView(100f, 1000f);
			raceSelectList.SetScrollbar(raceSelectScrollbar);
			raceSelectPanel.Append(raceSelectScrollbar);
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
				UIRaceButton uIRaceButton = new UIRaceButton(_player, raceId)
				{
					Left = StyleDimension.FromPixels((float)(raceId % 5) * 48f),
					Top = StyleDimension.FromPixels((float)(raceId / 5) * 84f)
				};
				uIRaceButton.OnLeftMouseDown += Click_SelectRace;
				uIRaceButton.SetSnapPoint("Middle", raceId);
				selectContainer.Append(uIRaceButton);
				if (raceId == raceCount - 1) {
					UIRaceSkeletonButton uIRaceSkeletonButton = new UIRaceSkeletonButton(_player)
					{
						Width = StyleDimension.FromPixelsAndPercent(0f, 0.6f),
						HAlign = 0.5f,
						Left = StyleDimension.FromPixels(-6f),
						Top = StyleDimension.FromPixels((float)(raceId / 5) * 84f + 92f - 8)
					};
					uIRaceSkeletonButton.SetSnapPoint("Middle", raceCount);
					uIRaceSkeletonButton.OnLeftMouseDown += Click_NamingSkeleton;
					selectContainer.Append(uIRaceSkeletonButton);
				}
			}
			UIElement raceStatPanel = new UIElement
			{
				HAlign = 1f,
				VAlign = 1f,
				Width = StyleDimension.FromPixelsAndPercent((0f - num) * 2f, 0.914f - num3),
				Left = StyleDimension.FromPixels(0f - num - 276),
				Height = StyleDimension.FromPixelsAndPercent(uIElement.Height.Pixels, uIElement.Height.Precent)
			};
			i.Append(raceStatPanel);
			raceStatPanel.SetPadding(0f);
			UIList raceStatList = new UIList
			{
				Width = StyleDimension.FromPixelsAndPercent(-18f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(-6f, 1f)
			};
			raceStatPanel.Append(raceStatList);
			UIScrollbar raceStatScrollbar = new UIScrollbar
			{
				HAlign = 1f,
				Height = StyleDimension.FromPixelsAndPercent(-30f, 1f),
				Top = StyleDimension.FromPixels(10f)
			};
			raceStatScrollbar.SetView(100f, 1000f);
			raceStatList.SetScrollbar(raceStatScrollbar);
			raceStatPanel.Append(raceStatScrollbar);
			statContainer = new UIElement
			{
				Width = StyleDimension.FromPixelsAndPercent(-7f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(700f, 0f)
			};
			raceStatList.Add(statContainer);
			statContainer.SetPadding(0f);
			uIRaceInfoPanel = new UIRaceInfoPanel(_player)
			{
				HAlign = 0f,
				VAlign = 0.5f,
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f)
			};
			statContainer.Append(uIRaceInfoPanel);
			uIRaceInfoPanel.UpdateStats();
			/*UIRaceNameButton uIRaceSkeletonButton = new UIRaceNameButton(_player);
			uIRaceSkeletonButton.Width = StyleDimension.FromPixelsAndPercent(0f, 0.25f);
			uIRaceSkeletonButton.HAlign = 0.7f;
			uIRaceSkeletonButton.Left = StyleDimension.FromPixels(-136);
			i.Append(uIRaceSkeletonButton);*/
			statContainer.Height = StyleDimension.FromPixelsAndPercent(uIRaceInfoPanel.totalHeight + 8, 0f);
			_raceSelectContainer = i;
		}

		private void MakeCategoriesBar(UIElement categoryContainer)
		{
			float i = -288f;
			float text = 48f;
			_colorPickers = new UIColoredImageButton[12];
			categoryContainer.Append(CreateColorPicker(CategoryId.HairColor, "MrPlagueRaces/Assets/Textures/UI/ColorHair", i, text));
			categoryContainer.Append(CreateColorPicker(CategoryId.Eye, "MrPlagueRaces/Assets/Textures/UI/ColorEye", i, text));
			categoryContainer.Append(CreateColorPicker(CategoryId.Skin, "MrPlagueRaces/Assets/Textures/UI/ColorSkin", i, text));
			categoryContainer.Append(CreateColorPicker(CategoryId.Detail, "MrPlagueRaces/Assets/Textures/UI/ColorDetail", i, text));
			categoryContainer.Append(CreateColorPicker(CategoryId.Shirt, "MrPlagueRaces/Assets/Textures/UI/ColorShirt", i, text));
			categoryContainer.Append(CreateColorPicker(CategoryId.Undershirt, "MrPlagueRaces/Assets/Textures/UI/ColorUndershirt", i, text));
			categoryContainer.Append(CreateColorPicker(CategoryId.Pants, "MrPlagueRaces/Assets/Textures/UI/ColorPants", i, text));
			categoryContainer.Append(CreateColorPicker(CategoryId.Shoes, "MrPlagueRaces/Assets/Textures/UI/ColorShoes", i, text));
			_colorPickers[5].SetMiddleTexture(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/ColorEyeBack", (AssetRequestMode)1));
			_clothingStylesCategoryButton = CreatePickerWithoutClick(CategoryId.Clothing, "MrPlagueRaces/Assets/Textures/UI/ClothStyleMale", i, text);
			_clothingStylesCategoryButton.OnLeftMouseDown += Click_ClothStyles;
			_clothingStylesCategoryButton.SetSnapPoint("Top", 2);
			categoryContainer.Append(_clothingStylesCategoryButton);
			_hairStylesCategoryButton = CreatePickerWithoutClick(CategoryId.HairStyle, "MrPlagueRaces/Assets/Textures/UI/Style_Hair", i, text);
			_hairStylesCategoryButton.OnLeftMouseDown += Click_HairStyles;
			_hairStylesCategoryButton.SetMiddleTexture(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Style_Arrow", (AssetRequestMode)1));
			_hairStylesCategoryButton.SetSnapPoint("Top", 3);
			categoryContainer.Append(_hairStylesCategoryButton);
			_charInfoCategoryButton = CreatePickerWithoutClick(CategoryId.CharInfo, "MrPlagueRaces/Assets/Textures/UI/CharInfo", i, text);
			_charInfoCategoryButton.OnLeftMouseDown += Click_CharInfo;
			_charInfoCategoryButton.SetSnapPoint("Top", 0);
			categoryContainer.Append(_charInfoCategoryButton);
			_raceSelectCategoryButton = CreatePickerWithoutClick(CategoryId.RaceSelect, "MrPlagueRaces/Assets/Textures/UI/RaceSelect", i, text);
			_raceSelectCategoryButton.OnLeftMouseDown += Click_RaceSelect;
			_raceSelectCategoryButton.SetSnapPoint("Top", 1);
			categoryContainer.Append(_raceSelectCategoryButton);
			UpdateColorPickers();
			UIHorizontalSeparator element = new UIHorizontalSeparator
			{
				Width = StyleDimension.FromPixelsAndPercent(-20f, 1f),
				Top = StyleDimension.FromPixels(6f),
				VAlign = 1f,
				HAlign = 0.5f,
				Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
			};
			categoryContainer.Append(element);
			int num = 21;
			UIText uIText = new UIText(PlayerInput.GenerateInputTag_ForCurrentGamemode(tagForGameplay: false, "HotbarMinus"))
			{
				Left = new StyleDimension(-num, 0f),
				VAlign = 0.5f,
				Top = new StyleDimension(-4f, 0f)
			};
			categoryContainer.Append(uIText);
			UIText uIText2 = new UIText(PlayerInput.GenerateInputTag_ForCurrentGamemode(tagForGameplay: false, "HotbarMinus"))
			{
				HAlign = 1f,
				Left = new StyleDimension(12 + num, 0f),
				VAlign = 0.5f,
				Top = new StyleDimension(-4f, 0f)
			};
			categoryContainer.Append(uIText2);
			_helpGlyphLeft = uIText;
			_helpGlyphRight = uIText2;
			categoryContainer.OnUpdate += UpdateHelpGlyphs;
		}

		private void UpdateHelpGlyphs(UIElement element)
		{
			string text = "";
			string text2 = "";
			if (PlayerInput.UsingGamepad)
			{
				text = PlayerInput.GenerateInputTag_ForCurrentGamemode(tagForGameplay: false, "HotbarMinus");
				text2 = PlayerInput.GenerateInputTag_ForCurrentGamemode(tagForGameplay: false, "HotbarPlus");
			}
			_helpGlyphLeft.SetText(text);
			_helpGlyphRight.SetText(text2);
		}

		private UIColoredImageButton CreateColorPicker(CategoryId id, string texturePath, float xPositionStart, float xPositionPerId)
		{
			UIColoredImageButton uIColoredImageButton = new UIColoredImageButton(ModContent.Request<Texture2D>(texturePath, (AssetRequestMode)1));
			_colorPickers[(int)id] = uIColoredImageButton;
			uIColoredImageButton.VAlign = 0f;
			uIColoredImageButton.HAlign = 0f;
			uIColoredImageButton.Left.Set(xPositionStart + (float)id * xPositionPerId, 0.5f);
			uIColoredImageButton.OnLeftMouseDown += Click_ColorPicker;
			uIColoredImageButton.SetSnapPoint("Top", (int)id);
			return uIColoredImageButton;
		}

		private UIColoredImageButton CreatePickerWithoutClick(CategoryId id, string texturePath, float xPositionStart, float xPositionPerId)
		{
			UIColoredImageButton uIColoredImageButton = new UIColoredImageButton(ModContent.Request<Texture2D>(texturePath, (AssetRequestMode)1));
			uIColoredImageButton.VAlign = 0f;
			uIColoredImageButton.HAlign = 0f;
			uIColoredImageButton.Left.Set(xPositionStart + (float)id * xPositionPerId, 0.5f);
			return uIColoredImageButton;
		}

		private void MakeInfoMenu(UIElement parentContainer)
		{
			UIElement i = new UIElement
			{
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
				HAlign = 0.5f,
				VAlign = 0f
			};
			i.SetPadding(10f);
			i.PaddingBottom = 0f;
			i.PaddingTop = 0f;
			parentContainer.Append(i);
			UICharacterNameButton uICharacterNameButton = new UICharacterNameButton(Language.GetText("UI.WorldCreationName"), Language.GetText("UI.PlayerEmptyName"));
			uICharacterNameButton.Width = StyleDimension.FromPixelsAndPercent(0f, 1f);
			uICharacterNameButton.HAlign = 0.5f;
			i.Append(uICharacterNameButton);
			_charName = uICharacterNameButton;
			uICharacterNameButton.OnLeftMouseDown += Click_Naming;
			uICharacterNameButton.SetSnapPoint("Middle", 0);
			float num = 4f;
			float num2 = 0f;
			float num3 = 0.4f;
			UIElement uIElement = new UIElement
			{
				HAlign = 0f,
				VAlign = 1f,
				Width = StyleDimension.FromPixelsAndPercent(0f - num, num3),
				Height = StyleDimension.FromPixelsAndPercent(-50f, 1f)
			};
			uIElement.SetPadding(0f);
			i.Append(uIElement);
			UISlicedImage uISlicedImage = new UISlicedImage(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/CategoryPanelHighlight", (AssetRequestMode)1))
			{
				HAlign = 1f,
				VAlign = 1f,
				Width = StyleDimension.FromPixelsAndPercent((0f - num) * 2f, 1f - num3),
				Left = StyleDimension.FromPixels(0f - num),
				Height = StyleDimension.FromPixelsAndPercent(uIElement.Height.Pixels, uIElement.Height.Precent)
			};
			uISlicedImage.SetSliceDepths(10);
			uISlicedImage.Color = Color.LightGray * 0.7f;
			i.Append(uISlicedImage);
			float num4 = 4f;
			UIDifficultyButton uIDifficultyButton = new UIDifficultyButton(_player, Language.GetText("LegacyMenu.26"), Language.GetText("LegacyMenu.31"), 0, Color.Cyan)
			{
				HAlign = 0f,
				VAlign = 1f / (num4 - 1f),
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f - num2, 1f / num4)
			};
			UIDifficultyButton uIDifficultyButton2 = new UIDifficultyButton(_player, Language.GetText("LegacyMenu.25"), Language.GetText("LegacyMenu.30"), 1, Main.mcColor)
			{
				HAlign = 0f,
				VAlign = 2f / (num4 - 1f),
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f - num2, 1f / num4)
			};
			UIDifficultyButton uIDifficultyButton3 = new UIDifficultyButton(_player, Language.GetText("LegacyMenu.24"), Language.GetText("LegacyMenu.29"), 2, Main.hcColor)
			{
				HAlign = 0f,
				VAlign = 1f,
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f - num2, 1f / num4)
			};
			UIDifficultyButton uIDifficultyButton4 = new UIDifficultyButton(_player, Language.GetText("UI.Creative"), Language.GetText("UI.CreativeDescriptionPlayer"), 3, Main.creativeModeColor)
			{
				HAlign = 0f,
				VAlign = 0f,
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f - num2, 1f / num4)
			};
			UIText uIText = new UIText(Language.GetText("LegacyMenu.26"))
			{
				HAlign = 0f,
				VAlign = 0.5f,
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Top = StyleDimension.FromPixelsAndPercent(15f, 0f),
				IsWrapped = true
			};
			uIText.PaddingLeft = 3f;
			uIText.PaddingRight = 3f;
			uISlicedImage.Append(uIText);
			uIElement.Append(uIDifficultyButton4);
			uIElement.Append(uIDifficultyButton);
			uIElement.Append(uIDifficultyButton2);
			uIElement.Append(uIDifficultyButton3);
			_infoContainer = i;
			_difficultyDescriptionText = uIText;
			uIDifficultyButton4.OnLeftMouseDown += UpdateDifficultyDescription;
			uIDifficultyButton.OnLeftMouseDown += UpdateDifficultyDescription;
			uIDifficultyButton2.OnLeftMouseDown += UpdateDifficultyDescription;
			uIDifficultyButton3.OnLeftMouseDown += UpdateDifficultyDescription;
			UpdateDifficultyDescription(null, null);
			uIDifficultyButton4.SetSnapPoint("Middle", 1);
			uIDifficultyButton.SetSnapPoint("Middle", 2);
			uIDifficultyButton2.SetSnapPoint("Middle", 3);
			uIDifficultyButton3.SetSnapPoint("Middle", 4);
		}

		private void UpdateDifficultyDescription(UIMouseEvent evt, UIElement listeningElement)
		{
			LocalizedText i = Language.GetText("LegacyMenu.31");
			switch (_player.difficulty)
			{
			case 0:
				i = Language.GetText("LegacyMenu.31");
				break;
			case 1:
				i = Language.GetText("LegacyMenu.30");
				break;
			case 2:
				i = Language.GetText("LegacyMenu.29");
				break;
			case 3:
				i = Language.GetText("UI.CreativeDescriptionPlayer");
				break;
			}
			_difficultyDescriptionText.SetText(i);
		}

		private void MakeHSLMenu(UIElement parentContainer)
		{
			UIElement uIElement = new UIElement
			{
				Width = StyleDimension.FromPixelsAndPercent(220f, 0f),
				Height = StyleDimension.FromPixelsAndPercent(158f, 0f),
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
			UIColoredImageButton uIColoredImageButton = new UIColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Copy", (AssetRequestMode)1), isSmall: true)
			{
				VAlign = 1f,
				HAlign = 0f,
				Left = StyleDimension.FromPixelsAndPercent(0f, 0f)
			};
			uIColoredImageButton.OnLeftMouseDown += Click_CopyHex;
			uIElement.Append(uIColoredImageButton);
			_copyHexButton = uIColoredImageButton;
			UIColoredImageButton uIColoredImageButton2 = new UIColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Paste", (AssetRequestMode)1), isSmall: true)
			{
				VAlign = 1f,
				HAlign = 0f,
				Left = StyleDimension.FromPixelsAndPercent(40f, 0f)
			};
			uIColoredImageButton2.OnLeftMouseDown += Click_PasteHex;
			uIElement.Append(uIColoredImageButton2);
			_pasteHexButton = uIColoredImageButton2;
			UIColoredImageButton uIColoredImageButton3 = new UIColoredImageButton(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Randomize", (AssetRequestMode)1), isSmall: true)
			{
				VAlign = 1f,
				HAlign = 0f,
				Left = StyleDimension.FromPixelsAndPercent(80f, 0f)
			};
			uIColoredImageButton3.OnLeftMouseDown += Click_RandomizeSingleColor;
			uIElement.Append(uIColoredImageButton3);
			_randomColorButton = uIColoredImageButton3;
			_hslContainer = uIElement;
			_hslHexText = uIText;
			uIColoredImageButton.SetSnapPoint("Low", 0);
			uIColoredImageButton2.SetSnapPoint("Low", 1);
			uIColoredImageButton3.SetSnapPoint("Low", 2);
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
				return new UIColoredSlider(LocalizedText.Empty, () => GetHSLSliderPosition(HSLSliderId.Saturation), delegate(float x)
				{
					UpdateHSLValue(HSLSliderId.Saturation, x);
				}, UpdateHSL_S, (float x) => GetHSLSliderColorAt(HSLSliderId.Saturation, x), Color.Transparent);
			case HSLSliderId.Luminance:
				return new UIColoredSlider(LocalizedText.Empty, () => GetHSLSliderPosition(HSLSliderId.Luminance), delegate(float x)
				{
					UpdateHSLValue(HSLSliderId.Luminance, x);
				}, UpdateHSL_L, (float x) => GetHSLSliderColorAt(HSLSliderId.Luminance, x), Color.Transparent);
			default:
				return new UIColoredSlider(LocalizedText.Empty, () => GetHSLSliderPosition(HSLSliderId.Hue), delegate(float x)
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

		private void MakeBackAndCreatebuttons(UIElement outerContainer)
		{
			UITextPanel<LocalizedText> uITextPanel = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, large: true)
			{
				Width = StyleDimension.FromPixelsAndPercent(-10f, 0.5f),
				Height = StyleDimension.FromPixels(50f),
				VAlign = 1f,
				HAlign = 0f,
				Top = StyleDimension.FromPixels(-45f)
			};
			uITextPanel.OnMouseOver += FadedMouseOver;
			uITextPanel.OnMouseOut += FadedMouseOut;
			uITextPanel.OnLeftMouseDown += Click_GoBack;
			uITextPanel.SetSnapPoint("Back", 0);
			outerContainer.Append(uITextPanel);
			UITextPanel<LocalizedText> uITextPanel2 = new UITextPanel<LocalizedText>(Language.GetText("UI.Create"), 0.7f, large: true)
			{
				Width = StyleDimension.FromPixelsAndPercent(-10f, 0.5f),
				Height = StyleDimension.FromPixels(50f),
				VAlign = 1f,
				HAlign = 1f,
				Top = StyleDimension.FromPixels(-45f)
			};
			uITextPanel2.OnMouseOver += FadedMouseOver;
			uITextPanel2.OnMouseOut += FadedMouseOut;
			uITextPanel2.OnLeftMouseDown += Click_NamingAndCreating;
			uITextPanel2.SetSnapPoint("Create", 0);
			outerContainer.Append(uITextPanel2);
		}

		private void Click_GoBack(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			Main.OpenCharacterSelectUI();
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

		private void Click_SelectRace(UIMouseEvent evt, UIElement listeningElement)
		{
			UpdateColorPickers();
			uIRaceInfoPanel.UpdateStats();
			statContainer.Height = StyleDimension.FromPixelsAndPercent(uIRaceInfoPanel.totalHeight + 8, 0f);
		}

		private void Click_ColorPicker(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
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
			SelectColorPicker((CategoryId)text);
		}

		private void Click_RaceSelect(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			UnselectAllCategories();
			_selectedPicker = CategoryId.RaceSelect;
			_middleContainer.Append(_raceSelectContainer);
			_raceSelectCategoryButton.SetSelected(selected: true);
		}

		private void Click_ClothStyles(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			UnselectAllCategories();
			_selectedPicker = CategoryId.Clothing;
			_middleContainer.Append(_clothStylesContainer);
			_clothingStylesCategoryButton.SetSelected(selected: true);
			UpdateSelectedGender();
		}

		private void Click_HairStyles(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			UnselectAllCategories();
			_selectedPicker = CategoryId.HairStyle;
			MakeHairstylesMenu(_middleContainer);
			_middleContainer.Append(_hairstylesContainer);
			_hairStylesCategoryButton.SetSelected(selected: true);
		}

		private void Click_CharInfo(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			UnselectAllCategories();
			_selectedPicker = CategoryId.CharInfo;
			_middleContainer.Append(_infoContainer);
			_charInfoCategoryButton.SetSelected(selected: true);
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
				}
			};
			JsonSerializerSettings val = new JsonSerializerSettings();
			val.TypeNameHandling = ((TypeNameHandling)4);
			val.MetadataPropertyHandling = ((MetadataPropertyHandling)1);
			val.Formatting = ((Formatting)1);
			string text = JsonConvert.SerializeObject((object)obj, (JsonSerializerSettings)(object)val);
			PlayerInput.PrettyPrintProfiles(ref text);
			Platform.Get<IClipboard>().Value = (text);
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
							Click_CharClothStyle(null, null);
							UpdateColorPickers();
						}
					}
				}
			}
			catch
			{
			}
		}

		private void Click_RandomizePlayer(UIMouseEvent evt, UIElement listeningElement)
		{
			var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
			SoundEngine.PlaySound(SoundID.MenuTick);
			Player player = _player;
			player.hair = Main.rand.Next(mrPlagueRacesPlayer.GetRaceHairCount(_player));
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
		}

		private void Click_Naming(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuOpen);
			_player.name = "";
			Main.clrInput();
			UIVirtualKeyboard uIVirtualKeyboard = new UIVirtualKeyboard(Language.GetText("LegacyMenu.45").Value, "", OnFinishedNaming, OnCancledNaming, 0, allowEmpty: true);
			uIVirtualKeyboard.SetMaxInputLength(20);
			Main.MenuUI.SetState(uIVirtualKeyboard);
		}

		private void Click_NamingSkeleton(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuOpen);
			_player.name = "";
			Main.clrInput();
			UICreateRace uIVirtualKeyboard = new UICreateRace();
			//uIVirtualKeyboard.SetMaxInputLength(20);
			Main.MenuUI.SetState(uIVirtualKeyboard);
		}

		private void Click_NamingAndCreating(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuOpen);
			if (string.IsNullOrEmpty(_player.name))
			{
				_player.name = "";
				Main.clrInput();
				UIVirtualKeyboard uIVirtualKeyboard = new UIVirtualKeyboard(Language.GetText("LegacyMenu.45").Value, "", OnFinishedNamingAndCreating, OnCancledNaming);
				uIVirtualKeyboard.SetMaxInputLength(20);
				Main.MenuUI.SetState(uIVirtualKeyboard);
			}
			else
			{
				FinishCreatingCharacter();
			}
		}

		private void OnFinishedNaming(string name)
		{
			_player.name = name.Trim();
			Main.MenuUI.SetState(this);
			_charName.SetContents(_player.name);
		}

		private void OnCancledNaming()
		{
			Main.MenuUI.SetState(this);
		}

		private void OnFinishedNamingAndCreating(string name)
		{
			_player.name = name.Trim();
			Main.MenuUI.SetState(this);
			_charName.SetContents(_player.name);
			FinishCreatingCharacter();
		}

		private void FinishCreatingCharacter()
		{
			SetupPlayerStatsAndInventoryBasedOnDifficulty();
			PlayerFileData.CreateAndSave(_player);
			Main.LoadPlayers();
			Main.menuMode = 1;
		}

		private void SetupPlayerStatsAndInventoryBasedOnDifficulty()
		{
			int modItem = 0;
			if (_player.difficulty == 3)
			{
				_player.statLife = (_player.statLifeMax = 100);
				_player.statMana = (_player.statManaMax = 20);
				_player.inventory[modItem].SetDefaults(6);
				_player.inventory[modItem++].Prefix(-1);
				_player.inventory[modItem].SetDefaults(1);
				_player.inventory[modItem++].Prefix(-1);
				_player.inventory[modItem].SetDefaults(10);
				_player.inventory[modItem++].Prefix(-1);
				_player.inventory[modItem].SetDefaults(7);
				_player.inventory[modItem++].Prefix(-1);
				_player.inventory[modItem].SetDefaults(4281);
				_player.inventory[modItem++].Prefix(-1);
				_player.inventory[modItem].SetDefaults(8);
				_player.inventory[modItem++].stack = 100;
				_player.inventory[modItem].SetDefaults(965);
				_player.inventory[modItem++].stack = 100;
				_player.inventory[modItem++].SetDefaults(50);
				_player.inventory[modItem++].SetDefaults(84);
				_player.armor[3].SetDefaults(4978);
				_player.armor[3].Prefix(-1);
				_player.AddBuff(216, 3600);
			}
			else
			{
				_player.inventory[modItem].SetDefaults(3507);
				_player.inventory[modItem++].Prefix(-1);
				_player.inventory[modItem].SetDefaults(3509);
				_player.inventory[modItem++].Prefix(-1);
				_player.inventory[modItem].SetDefaults(3506);
				_player.inventory[modItem++].Prefix(-1);
			}
			if (Main.runningCollectorsEdition)
			{
				_player.inventory[modItem++].SetDefaults(603);
			}
			_player.savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules();
			CreativePowerManager.Instance.ResetDataForNewPlayer(_player);
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

		private void UnselectAllCategories()
		{
			UIColoredImageButton[] colorPickers = _colorPickers;
			for (int i = 0; i < colorPickers.Length; i++)
			{
				colorPickers[i]?.SetSelected(selected: false);
			}
			_clothingStylesCategoryButton.SetSelected(selected: false);
			_hairStylesCategoryButton.SetSelected(selected: false);
			_charInfoCategoryButton.SetSelected(selected: false);
			_raceSelectCategoryButton.SetSelected(selected: false);
			_hslContainer.Remove();
			_hairstylesContainer.Remove();
			_clothStylesContainer.Remove();
			_infoContainer.Remove();
			_raceSelectContainer.Remove();
		}

		private void SelectColorPicker(CategoryId selection)
		{
			var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
			_selectedPicker = selection;
			switch (selection)
			{
			case CategoryId.CharInfo:
				Click_CharInfo(null, null);
				return;
			case CategoryId.Clothing:
				Click_ClothStyles(null, null);
				return;
			case CategoryId.HairStyle:
				Click_HairStyles(null, null);
				return;
			}
			UnselectAllCategories();
			_middleContainer.Append(_hslContainer);
			for (int i = 0; i < _colorPickers.Length; i++)
			{
				if (_colorPickers[i] != null)
				{
					_colorPickers[i].SetSelected(i == (int)selection);
				}
			}
			Vector3 currentColorHSL = Vector3.One;
			switch (_selectedPicker)
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
			}
			_currentColorHSL = currentColorHSL;
			UpdateHexText(ScaledHslToRgb(currentColorHSL.X, currentColorHSL.Y, currentColorHSL.Z));
		}

		private void UpdateColorPickers()
		{
			var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
			_ = _selectedPicker;
			_colorPickers[4].SetColor(_player.hairColor);
			_hairStylesCategoryButton.SetColor(_player.hairColor);
			_colorPickers[5].SetColor(_player.eyeColor);
			_colorPickers[6].SetColor(_player.skinColor);
			_colorPickers[7].SetColor(mrPlagueRacesPlayer.detailColor);
			_colorPickers[8].SetColor(_player.shirtColor);
			_colorPickers[9].SetColor(_player.underShirtColor);
			_colorPickers[10].SetColor(_player.pantsColor);
			_colorPickers[11].SetColor(_player.shoeColor);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			string result = null;
			if (_copyHexButton.IsMouseHovering)
			{
				result = Language.GetTextValue("UI.CopyColorToClipboard");
			}
			if (_pasteHexButton.IsMouseHovering)
			{
				result = Language.GetTextValue("UI.PasteColorFromClipboard");
			}
			if (_randomColorButton.IsMouseHovering)
			{
				result = Language.GetTextValue("UI.RandomizeColor");
			}
			if (_copyTemplateButton.IsMouseHovering)
			{
				result = Language.GetTextValue("UI.CopyPlayerToClipboard");
			}
			if (_pasteTemplateButton.IsMouseHovering)
			{
				result = Language.GetTextValue("UI.PastePlayerFromClipboard");
			}
			if (_randomizePlayerButton.IsMouseHovering)
			{
				result = Language.GetTextValue("UI.RandomizePlayer");
			}
			if (result != null)
			{
				float x = FontAssets.MouseText.Value.MeasureString(result).X;
				Vector2 vector = new Vector2(Main.mouseX, Main.mouseY) + new Vector2(16f);
				if (vector.Y > (float)(Main.screenHeight - 30))
				{
					vector.Y = Main.screenHeight - 30;
				}
				if (vector.X > (float)Main.screenWidth - x)
				{
					vector.X = Main.screenWidth - 460;
				}
				Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, result, vector.X, vector.Y, new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), Color.Black, Vector2.Zero);
			}
			statHoverText.Left = StyleDimension.FromPixelsAndPercent(Main.mouseX + 10f, 0f);
			statHoverText.Top = StyleDimension.FromPixelsAndPercent(Main.mouseY + 12f, 0f);
			statHoverText.SetText(hoverText);
			SetupGamepadPoints(spriteBatch);
		}

		private void SetupGamepadPoints(SpriteBatch spriteBatch)
		{
			UILinkPointNavigator.Shortcuts.BackButtonCommand = 1;
			int num = 3000;
			int num2 = num + 20;
			int num3 = num;
			List<SnapPoint> snapPoints = GetSnapPoints();
			SnapPoint snapPoint = snapPoints.First((SnapPoint a) => a.Name == "Back");
			SnapPoint snapPoint2 = snapPoints.First((SnapPoint a) => a.Name == "Create");
			UILinkPoint uILinkPoint = UILinkPointNavigator.Points[num3];
			uILinkPoint.Unlink();
			UILinkPointNavigator.SetPosition(num3, snapPoint.Position);
			num3++;
			UILinkPoint uILinkPoint2 = UILinkPointNavigator.Points[num3];
			uILinkPoint2.Unlink();
			UILinkPointNavigator.SetPosition(num3, snapPoint2.Position);
			num3++;
			uILinkPoint.Right = uILinkPoint2.ID;
			uILinkPoint2.Left = uILinkPoint.ID;
			_foundPoints.Clear();
			_foundPoints.Add(uILinkPoint.ID);
			_foundPoints.Add(uILinkPoint2.ID);
			List<SnapPoint> list = snapPoints.Where((SnapPoint a) => a.Name == "Top").ToList();
			list.Sort(SortPoints);
			for (int i = 0; i < list.Count; i++)
			{
				UILinkPoint uILinkPoint3 = UILinkPointNavigator.Points[num3];
				uILinkPoint3.Unlink();
				UILinkPointNavigator.SetPosition(num3, list[i].Position);
				uILinkPoint3.Left = num3 - 1;
				uILinkPoint3.Right = num3 + 1;
				uILinkPoint3.Down = num2;
				if (i == 0)
				{
					uILinkPoint3.Left = -3;
				}
				if (i == list.Count - 1)
				{
					uILinkPoint3.Right = -4;
				}
				if (_selectedPicker == CategoryId.HairStyle || _selectedPicker == CategoryId.Clothing)
				{
					uILinkPoint3.Down = num2 + i;
				}
				_foundPoints.Add(num3);
				num3++;
			}
			List<SnapPoint> list2 = snapPoints.Where((SnapPoint a) => a.Name == "Middle").ToList();
			list2.Sort(SortPoints);
			num3 = num2;
			switch (_selectedPicker)
			{
			case CategoryId.CharInfo:
			{
				for (int l = 0; l < list2.Count; l++)
				{
					UILinkPoint andSet3 = GetAndSet(num3, list2[l]);
					andSet3.Up = andSet3.ID - 1;
					andSet3.Down = andSet3.ID + 1;
					if (l == 0)
					{
						andSet3.Up = num + 2;
					}
					if (l == list2.Count - 1)
					{
						andSet3.Down = uILinkPoint.ID;
						uILinkPoint.Up = andSet3.ID;
						uILinkPoint2.Up = andSet3.ID;
					}
					_foundPoints.Add(num3);
					num3++;
				}
				break;
			}
			case CategoryId.HairStyle:
			{
				if (list2.Count == 0)
				{
					break;
				}
				_helper.CullPointsOutOfElementArea(spriteBatch, list2, _hairstylesContainer);
				SnapPoint snapPoint3 = list2[list2.Count - 1];
				_ = snapPoint3.Id / 10;
				_ = snapPoint3.Id % 10;
				int count = Main.Hairstyles.AvailableHairstyles.Count;
				for (int m = 0; m < list2.Count; m++)
				{
					SnapPoint snapPoint4 = list2[m];
					UILinkPoint andSet4 = GetAndSet(num3, snapPoint4);
					andSet4.Left = andSet4.ID - 1;
					if (snapPoint4.Id == 0)
					{
						andSet4.Left = -3;
					}
					andSet4.Right = andSet4.ID + 1;
					if (snapPoint4.Id == count - 1)
					{
						andSet4.Right = -4;
					}
					andSet4.Up = andSet4.ID - 10;
					if (m < 10)
					{
						andSet4.Up = num + 2 + m;
					}
					andSet4.Down = andSet4.ID + 10;
					if (snapPoint4.Id + 10 > snapPoint3.Id)
					{
						if (snapPoint4.Id % 10 < 5)
						{
							andSet4.Down = uILinkPoint.ID;
						}
						else
						{
							andSet4.Down = uILinkPoint2.ID;
						}
					}
					if (m == list2.Count - 1)
					{
						uILinkPoint.Up = andSet4.ID;
						uILinkPoint2.Up = andSet4.ID;
					}
					_foundPoints.Add(num3);
					num3++;
				}
				break;
			}
			default:
			{
				List<SnapPoint> list4 = snapPoints.Where((SnapPoint a) => a.Name == "Low").ToList();
				list4.Sort(SortPoints);
				num3 = num2 + 20;
				for (int n = 0; n < list4.Count; n++)
				{
					UILinkPoint andSet5 = GetAndSet(num3, list4[n]);
					andSet5.Up = num2 + 2;
					andSet5.Down = uILinkPoint.ID;
					andSet5.Left = andSet5.ID - 1;
					andSet5.Right = andSet5.ID + 1;
					if (n == 0)
					{
						andSet5.Left = andSet5.ID + 2;
						uILinkPoint.Up = andSet5.ID;
					}
					if (n == list4.Count - 1)
					{
						andSet5.Right = andSet5.ID - 2;
						uILinkPoint2.Up = andSet5.ID;
					}
					_foundPoints.Add(num3);
					num3++;
				}
				num3 = num2;
				for (int num4 = 0; num4 < list2.Count; num4++)
				{
					UILinkPoint andSet6 = GetAndSet(num3, list2[num4]);
					andSet6.Up = andSet6.ID - 1;
					andSet6.Down = andSet6.ID + 1;
					if (num4 == 0)
					{
						andSet6.Up = num + 2 + 5;
					}
					if (num4 == list2.Count - 1)
					{
						andSet6.Down = num2 + 20 + 2;
					}
					_foundPoints.Add(num3);
					num3++;
				}
				break;
			}
			case CategoryId.Clothing:
			{
				List<SnapPoint> list3 = snapPoints.Where((SnapPoint a) => a.Name == "Low").ToList();
				list3.Sort(SortPoints);
				int down = -2;
				int down2 = -2;
				num3 = num2 + 20;
				for (int j = 0; j < list3.Count; j++)
				{
					UILinkPoint andSet = GetAndSet(num3, list3[j]);
					andSet.Up = num2 + j + 2;
					andSet.Down = uILinkPoint.ID;
					if (j >= 3)
					{
						andSet.Up++;
						andSet.Down = uILinkPoint2.ID;
					}
					andSet.Left = andSet.ID - 1;
					andSet.Right = andSet.ID + 1;
					if (j == 0)
					{
						down = andSet.ID;
						andSet.Left = andSet.ID + 4;
						uILinkPoint.Up = andSet.ID;
					}
					if (j == list3.Count - 1)
					{
						down2 = andSet.ID;
						andSet.Right = andSet.ID - 4;
						uILinkPoint2.Up = andSet.ID;
					}
					_foundPoints.Add(num3);
					num3++;
				}
				num3 = num2;
				for (int k = 0; k < list2.Count; k++)
				{
					UILinkPoint andSet2 = GetAndSet(num3, list2[k]);
					andSet2.Up = num + 2 + k;
					andSet2.Left = andSet2.ID - 1;
					andSet2.Right = andSet2.ID + 1;
					if (k == 0)
					{
						andSet2.Left = andSet2.ID + 9;
					}
					if (k == list2.Count - 1)
					{
						andSet2.Right = andSet2.ID - 9;
					}
					andSet2.Down = down;
					if (k >= 5)
					{
						andSet2.Down = down2;
					}
					_foundPoints.Add(num3);
					num3++;
				}
				break;
			}
			}
			if (PlayerInput.UsingGamepadUI && !_foundPoints.Contains(UILinkPointNavigator.CurrentPoint))
			{
				MoveToVisuallyClosestPoint();
			}
		}

		private void MoveToVisuallyClosestPoint()
		{
			Dictionary<int, UILinkPoint> modNPC = UILinkPointNavigator.Points;
			Vector2 mouseScreen = Main.MouseScreen;
			UILinkPoint uILinkPoint = null;
			foreach (int foundPoint in _foundPoints)
			{
				if (!modNPC.TryGetValue(foundPoint, out UILinkPoint value))
				{
					return;
				}
				if (uILinkPoint == null || Vector2.Distance(mouseScreen, uILinkPoint.Position) > Vector2.Distance(mouseScreen, value.Position))
				{
					uILinkPoint = value;
				}
			}
			if (uILinkPoint != null)
			{
				UILinkPointNavigator.ChangePoint(uILinkPoint.ID);
			}
		}

		public void TryMovingCategory(int direction)
		{
			int num = (int)(_selectedPicker + direction) % 10;
			if (num < 0)
			{
				num += 10;
			}
			SelectColorPicker((CategoryId)num);
		}

		private UILinkPoint GetAndSet(int ptid, SnapPoint snap)
		{
			UILinkPoint uILinkPoint = UILinkPointNavigator.Points[ptid];
			uILinkPoint.Unlink();
			UILinkPointNavigator.SetPosition(uILinkPoint.ID, snap.Position);
			return uILinkPoint;
		}

		private bool PointWithName(SnapPoint a, string comp)
		{
			return a.Name == comp;
		}

		private int SortPoints(SnapPoint a, SnapPoint b)
		{
			return a.Id.CompareTo(b.Id);
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
	}
}