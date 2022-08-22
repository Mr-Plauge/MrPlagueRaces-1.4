using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.UI;
using MrPlagueRaces.Common.Races;

namespace MrPlagueRaces.Common.UI.Elements
{
	public class UIRaceButton : UIElement
	{
		private readonly Player _player;

		public readonly int RaceId;

		private readonly Asset<Texture2D> _BasePanelTexture;

		private readonly Asset<Texture2D> _selectedBorderTexture;

		private readonly Asset<Texture2D> _hoveredBorderTexture;

		private readonly UICharacter _char;

		private bool _hovered;

		private bool _soundedHover;

		private Race _realRace;
		private int _realSkinVariant;
		private int _realHair;
		private Color _realHairColor;
		private Color _realSkinColor;
		private Color _realDetailColor;
		private Color _realEyeColor;
		private Color _realShirtColor;
		private Color _realUnderShirtColor;
		private Color _realPantsColor;
		private Color _realShoeColor;

		public UIRaceButton(Player player, int raceId)
		{
			_player = player;
			RaceId = raceId;
			Width = StyleDimension.FromPixels(44f);
			Height = StyleDimension.FromPixels(80f);
			_BasePanelTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel", (AssetRequestMode)1);
			_selectedBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight", (AssetRequestMode)1);
			_hoveredBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder", (AssetRequestMode)1);
			_char = new UICharacter(_player, animated: false, hasBackPanel: false)
			{
				HAlign = 0.5f,
				VAlign = 0.5f
			};
			Append(_char);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
			GetRealValues();
			mrPlagueRacesPlayer.race = RaceLoader.Races[RaceId];
			SetRaceValues();
			base.Draw(spriteBatch);
			mrPlagueRacesPlayer.race = _realRace;
			SetRealValues();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
			if (_hovered)
			{
				if (!_soundedHover)
				{
					SoundEngine.PlaySound(SoundID.MenuTick);
				}
				_soundedHover = true;
			}
			else
			{
				_soundedHover = false;
			}
			CalculatedStyle dimensions = GetDimensions();
			Utils.DrawSplicedPanel(spriteBatch, _BasePanelTexture.Value, (int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, (int)dimensions.Height, 10, 10, 10, 10, Color.White * 0.5f);
			if (_realRace == RaceLoader.Races[RaceId])
			{
				Utils.DrawSplicedPanel(spriteBatch, _selectedBorderTexture.Value, (int)dimensions.X + 3, (int)dimensions.Y + 3, (int)dimensions.Width - 6, (int)dimensions.Height - 6, 10, 10, 10, 10, Color.White);
			}
			if (_hovered)
			{
				Utils.DrawSplicedPanel(spriteBatch, _hoveredBorderTexture.Value, (int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, (int)dimensions.Height, 10, 10, 10, 10, Color.White);
			}
		}

		public override void MouseDown(UIMouseEvent evt)
		{
			var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
			mrPlagueRacesPlayer.race = RaceLoader.Races[RaceId];
			SetRaceValues();
			SoundEngine.PlaySound(SoundID.MenuTick);
			base.MouseDown(evt);
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
			_hovered = true;
			_char.SetAnimated(animated: true);
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);
			_hovered = false;
			_char.SetAnimated(animated: false);
		}

		public void GetRealValues()
		{
			var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
			_realRace = mrPlagueRacesPlayer.race;
			_realSkinVariant = _player.skinVariant;
			_realHair = _player.hair;
			_realHairColor = _player.hairColor;
			_realSkinColor = _player.skinColor;
			_realDetailColor = mrPlagueRacesPlayer.detailColor;
			_realEyeColor = _player.eyeColor;
			_realShirtColor = _player.shirtColor;
			_realUnderShirtColor = _player.underShirtColor;
			_realPantsColor = _player.pantsColor;
			_realShoeColor = _player.shoeColor;
		}

		public void SetRealValues()
		{
			Item blankItem = new Item();
			blankItem.SetDefaults(0);
			Item familiarShirt = new Item();
			familiarShirt.SetDefaults(ItemID.FamiliarShirt);
			Item familiarPants = new Item();
			familiarPants.SetDefaults(ItemID.FamiliarPants);
			var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();

			_player.skinVariant = _realSkinVariant;
			_player.armor[11] = blankItem;
			_player.armor[12] = blankItem;
			if (mrPlagueRacesPlayer.race.StarterShirt)
			{
				_player.armor[11] = familiarShirt;
			}
			if (mrPlagueRacesPlayer.race.StarterPants)
			{
				_player.armor[12] = familiarPants;
			}
			_player.hair = _realHair;
			_player.hairColor = _realHairColor;
			_player.skinColor = _realSkinColor;
			mrPlagueRacesPlayer.detailColor = _realDetailColor;
			_player.eyeColor = _realEyeColor;
			_player.shirtColor = _realShirtColor;
			_player.underShirtColor = _realUnderShirtColor;
			_player.pantsColor = _realPantsColor;
			_player.shoeColor = _realShoeColor;
		}

		public void SetRaceValues()
		{
			int[] male = { 0, 2, 1, 3, 8 };
			int[] female = { 4, 6, 5, 7, 9 };

			Item blankItem = new Item();
			blankItem.SetDefaults(0);
			Item familiarShirt = new Item();
			familiarShirt.SetDefaults(ItemID.FamiliarShirt);
			Item familiarPants = new Item();
			familiarPants.SetDefaults(ItemID.FamiliarPants);
			var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();

			_player.skinVariant = (_player.Male ? male[mrPlagueRacesPlayer.race.ClothStyle - 1] : female[mrPlagueRacesPlayer.race.ClothStyle - 1]);
			_player.armor[11] = blankItem;
			_player.armor[12] = blankItem;
			if (mrPlagueRacesPlayer.race.StarterShirt)
			{
				_player.armor[11] = familiarShirt;
			}
			if (mrPlagueRacesPlayer.race.StarterPants)
			{
				_player.armor[12] = familiarPants;
			}
			_player.hair = mrPlagueRacesPlayer.race.HairStyle;
			_player.hairColor = mrPlagueRacesPlayer.race.HairColor;
			_player.skinColor = mrPlagueRacesPlayer.race.SkinColor;
			mrPlagueRacesPlayer.detailColor = mrPlagueRacesPlayer.race.DetailColor;
			_player.eyeColor = mrPlagueRacesPlayer.race.EyeColor;
			_player.shirtColor = mrPlagueRacesPlayer.race.ShirtColor;
			_player.underShirtColor = mrPlagueRacesPlayer.race.UnderShirtColor;
			_player.pantsColor = mrPlagueRacesPlayer.race.PantsColor;
			_player.shoeColor = mrPlagueRacesPlayer.race.ShoeColor;
		}
	}
}
