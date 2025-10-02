using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;

namespace Terraria.GameContent.UI.Elements
{
    public class UIColoredImageButtonSmallFix : UIElement
    {
        private Asset<Texture2D> _backPanelTexture;

        private Asset<Texture2D> _texture;
        private Asset<Texture2D> _texture1;
        private Asset<Texture2D> _texture2;
        private Asset<Texture2D> _texture3;

        private Asset<Texture2D> _middleTexture;

        private Asset<Texture2D> _backPanelHighlightTexture;

        private Asset<Texture2D> _backPanelBorderTexture;

        private Color _color;
        private Color _color1;
        private Color _color2;
        private Color _color3;

        private float _visibilityActive = 1f;

        private float _visibilityInactive = 0.4f;

        private bool _selected;

        private bool _hovered;

        private bool _isSplitIntoFour;

        public UIColoredImageButtonSmallFix(Asset<Texture2D> texture, bool isSmall = false, bool isSplitIntoFour = false)
        {
            _color = Color.White;
            _texture = texture;
            _isSplitIntoFour = isSplitIntoFour;
            if (isSmall)
            {
                _backPanelTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/SmallPanel");
            }
            else
            {
                _backPanelTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel");
            }
            Width.Set(_backPanelTexture.Width(), 0f);
            Height.Set(_backPanelTexture.Height(), 0f);
            if (isSmall)
            {
                _backPanelHighlightTexture = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/CategoryPanelHighlightSmall", (AssetRequestMode)1);
            }
            else
            {
                _backPanelHighlightTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight");
            }
            if (isSmall)
            {
                _backPanelBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/SmallPanelBorder");
            }
            else
            {
                _backPanelBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder");
            }
            if (isSplitIntoFour)
            {
                _texture1 = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/ColorDetailSmall_2", (AssetRequestMode)1);
                _texture2 = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/ColorDetailSmall_3", (AssetRequestMode)1);
                _texture3 = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/ColorDetailSmall_4", (AssetRequestMode)1);
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
            spriteBatch.Draw(_backPanelTexture.Value, position, (Rectangle?)null, Color.White * (base.IsMouseHovering ? _visibilityActive : _visibilityInactive), 0f, _backPanelTexture.Size() / 2f, 1f, (SpriteEffects)0, 0f);
            if (_hovered)
            {
                spriteBatch.Draw(_backPanelBorderTexture.Value, position, (Rectangle?)null, Color.White, 0f, _backPanelBorderTexture.Size() / 2f, 1f, (SpriteEffects)0, 0f);
            }
            if (_selected)
            {
                spriteBatch.Draw(_backPanelHighlightTexture.Value, position, (Rectangle?)null, Color.White, 0f, _backPanelHighlightTexture.Size() / 2f, 1f, (SpriteEffects)0, 0f);
            }
            if (_middleTexture != null)
            {
                spriteBatch.Draw(_middleTexture.Value, position, (Rectangle?)null, Color.White, 0f, _middleTexture.Size() / 2f, 1f, (SpriteEffects)0, 0f);
            }
            spriteBatch.Draw(_texture.Value, position, (Rectangle?)null, _color, 0f, _texture.Size() / 2f, 1f, (SpriteEffects)0, 0f);
            if (_isSplitIntoFour)
            {
                spriteBatch.Draw(_texture1.Value, new Vector2(position.X - 0.4f, position.Y), (Rectangle?)null, _color1, 0f, _texture.Size() / 2f, 1f, (SpriteEffects)0, 0f);
                spriteBatch.Draw(_texture2.Value, new Vector2(position.X, position.Y - 0.4f), (Rectangle?)null, _color2, 0f, _texture.Size() / 2f, 1f, (SpriteEffects)0, 0f);
                spriteBatch.Draw(_texture3.Value, new Vector2(position.X - 0.4f, position.Y - 0.4f), (Rectangle?)null, _color3, 0f, _texture.Size() / 2f, 1f, (SpriteEffects)0, 0f);
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
            _hovered = true;
        }

        public void SetVisibility(float whenActive, float whenInactive)
        {
            _visibilityActive = MathHelper.Clamp(whenActive, 0f, 1f);
            _visibilityInactive = MathHelper.Clamp(whenInactive, 0f, 1f);
        }

        public Color GetColor()
        {
            return _color;
        }

        public void SetColor(Color color)
        {
            _color = color;
        }

        public void SetColors(Color color, Color color1, Color color2, Color color3, int detailColorCount)
        {
            _color = color;
            switch (detailColorCount)
            {
                case 1:
                    _color1 = color;
                    _color2 = color;
                    _color3 = color;
                    break;
                case 2:
                    _color1 = color1;
                    _color2 = color;
                    _color3 = color1;
                    break;
                case 3:
                    _color1 = color1;
                    _color2 = color2;
                    _color3 = color2;
                    break;
                case 4:
                    _color1 = color1;
                    _color2 = color2;
                    _color3 = color3;
                    break;
            }
        }

        public void SetMiddleTexture(Asset<Texture2D> texAsset)
        {
            _middleTexture = texAsset;
        }

        public void SetSelected(bool selected)
        {
            _selected = selected;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            _hovered = false;
        }
    }
}
