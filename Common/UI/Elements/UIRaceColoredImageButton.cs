using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.Players;

namespace MrPlagueRaces.Common.UI.Elements
{
	public class UIRaceColoredImageButton : UIElement
	{
		private Asset<Texture2D> _backPanelTexture;

		private Asset<Texture2D> _texture;

		private Asset<Texture2D> _middleTexture;

		private Asset<Texture2D> _backPanelHighlightTexture;

		private Asset<Texture2D> _backPanelBorderTexture;

		private Color _color;

		private float _visibilityActive = 1f;

		private float _visibilityInactive = 0.4f;

		private bool _selected;

		private bool _hovered;
		private bool _internallyHovered;

		private bool _xInvert;
		private bool _yInvert;

		private bool _isSmall;


        public UIRaceColoredImageButton(Asset<Texture2D> texture, bool isSmall = false, bool xInvert = false, bool yInvert = false)
		{
			_color = Color.White;
			_texture = texture;
			_xInvert = xInvert;
            _yInvert = yInvert;
			_isSmall = isSmall;

            if (isSmall)
			{
				if (_xInvert)
				{
                    _backPanelTexture = ModContent.Request<Texture2D>(_yInvert ? "MrPlagueRaces/Assets/Textures/UI/CategoryPanelQuarterBottomOpposite" : "MrPlagueRaces/Assets/Textures/UI/CategoryPanelQuarterOpposite", (AssetRequestMode)1);
                }
				else
				{
                    _backPanelTexture = ModContent.Request<Texture2D>(_yInvert ? "MrPlagueRaces/Assets/Textures/UI/CategoryPanelQuarterBottom" : "MrPlagueRaces/Assets/Textures/UI/CategoryPanelQuarter", (AssetRequestMode)1);
                }
			}
			else
			{
				_backPanelTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel", (AssetRequestMode)1);
			}
			Width.Set(_backPanelTexture.Width(), 0f);
			Height.Set(_backPanelTexture.Height(), 0f);
			if (isSmall)
			{
                _backPanelHighlightTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/CategoryPanelHighlightQuarter", (AssetRequestMode)1);
            }
			else
			{
                _backPanelHighlightTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight", (AssetRequestMode)1);
            }
			if (isSmall)
			{
				_backPanelBorderTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/CategoryPanelBorderQuarter", (AssetRequestMode)1);
			}
			else
			{
				_backPanelBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder", (AssetRequestMode)1);
			}
		}

		public void SetImage(Asset<Texture2D> texture)
		{
			_texture = texture;
			Width.Set(_texture.Width(), 0f);
			Height.Set(_texture.Height(), 0f);
		}

		public void SetImageWithoutSettingSize(Asset<Texture2D> texture)
		{
			_texture = texture;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			Vector2 position = dimensions.Position() + new Vector2(dimensions.Width, dimensions.Height) / 2f;
			spriteBatch.Draw(_backPanelTexture.Value, position, (Rectangle?)null, Color.White * (_hovered ? _visibilityActive : _visibilityInactive), 0f, _backPanelTexture.Size() / 2f, 1f, (_xInvert ? SpriteEffects.FlipHorizontally : 0) | (_yInvert ? SpriteEffects.FlipVertically : 0), 0f);
			if (_hovered)
			{
				spriteBatch.Draw(_backPanelBorderTexture.Value, position, (Rectangle?)null, Color.White, 0f, _backPanelBorderTexture.Size() / 2f, 1f, (_xInvert ? SpriteEffects.FlipHorizontally : 0) | (_yInvert ? SpriteEffects.FlipVertically : 0), 0f);
			}
			if (_selected)
			{
				spriteBatch.Draw(_backPanelHighlightTexture.Value, position, (Rectangle?)null, Color.White, 0f, _backPanelHighlightTexture.Size() / 2f, 1f, (_xInvert ? SpriteEffects.FlipHorizontally : 0) | (_yInvert ? SpriteEffects.FlipVertically : 0), 0f);
			}
			if (_middleTexture != null)
			{
				spriteBatch.Draw(_middleTexture.Value, position, (Rectangle?)null, Color.White, 0f, _middleTexture.Size() / 2f, 1f, (_xInvert ? SpriteEffects.FlipHorizontally : 0) | (_yInvert ? SpriteEffects.FlipVertically : 0), 0f);
			}
			spriteBatch.Draw(_texture.Value, position, (Rectangle?)null, _color, 0f, _texture.Size() / 2f, 1f, (_xInvert ? SpriteEffects.FlipHorizontally : 0) | (_yInvert ? SpriteEffects.FlipVertically : 0), 0f);
            if (!base.IsMouseHovering)
			{
				_hovered = false;
			}
        }

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
            if (!_isSmall)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
			else
			{
				if (!_internallyHovered)
				{
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
			}
            _hovered = true;
		}

		public void SetVisibility(float whenActive, float whenInactive)
		{
			_visibilityActive = MathHelper.Clamp(whenActive, 0f, 1f);
			_visibilityInactive = MathHelper.Clamp(whenInactive, 0f, 1f);
		}

		public void SetColor(Color color)
		{
			_color = color;
		}

		public void SetMiddleTexture(Asset<Texture2D> texAsset)
		{
			_middleTexture = texAsset;
		}

		public bool IsHovered()
		{
			return _hovered;
        }
        public bool IsSelected()
        {
            return _selected;
        }
        public Color GetColor()
        {
            return _color;
        }

        public void SetSelected(bool selected, bool shouldStartHovering = false)
		{
			_selected = selected;
			_hovered = shouldStartHovering;
			_internallyHovered = shouldStartHovering;
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);
			_hovered = false;
		}
	}
}
