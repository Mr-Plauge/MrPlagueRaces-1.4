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

namespace MrPlagueRaces.Common.UI.States
{
	public class UIRaceNameButton : UIElement
	{
		private readonly Player _player;

		private readonly Asset<Texture2D> _BasePanelTexture;

		private readonly Asset<Texture2D> _selectedBorderTexture;

		private readonly Asset<Texture2D> _hoveredBorderTexture;

		private bool _hovered;

		private bool _soundedHover;

		private UIText _title;

		private UIText uIText2;

		public readonly LocalizedText Description;

		public float DistanceFromTitleToOption = 20f;

		public UIRaceNameButton(Player player)
		{
			_player = player;
			var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
			Width = StyleDimension.FromPixels(400f);
			Height = StyleDimension.FromPixels(40f);
			_BasePanelTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel", (AssetRequestMode)1);
			_selectedBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight", (AssetRequestMode)1);
			_hoveredBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder", (AssetRequestMode)1);
			float textScale = 1f;
			UIText uIText = new UIText("Race:", textScale)
			{
				HAlign = 0f,
				VAlign = 0.5f,
				Left = StyleDimension.FromPixels(10f)
			};
			Append(uIText);
			_title = uIText;
			uIText2 = new UIText(mrPlagueRacesPlayer.race.DisplayName == null ? mrPlagueRacesPlayer.race.Name : mrPlagueRacesPlayer.race.DisplayName, textScale)
			{
				HAlign = 0f,
				VAlign = 0.5f,
				Left = StyleDimension.FromPixels(60f),
				TextOriginX = 0f
			};
			Append(uIText2);
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
			if (_hovered)
			{
				Utils.DrawSplicedPanel(spriteBatch, _hoveredBorderTexture.Value, (int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, (int)dimensions.Height, 10, 10, 10, 10, Color.White);
			}
			uIText2.SetText(mrPlagueRacesPlayer.race.DisplayName == null ? mrPlagueRacesPlayer.race.Name : mrPlagueRacesPlayer.race.DisplayName);
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
			_hovered = true;
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);
			_hovered = false;
		}
	}
}
