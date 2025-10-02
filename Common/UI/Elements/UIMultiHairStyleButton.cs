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
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.Players;

namespace MrPlagueRaces.Common.UI.Elements
{
	public class UIMultiHairStyleButton : UIImageButton
	{
		private readonly Player _player;

		public readonly int HairStyleId;

		private readonly Asset<Texture2D> _selectedBorderTexture;

		private readonly Asset<Texture2D> _hoveredBorderTexture;

		private bool _hovered;

		private bool _soundedHover;

		private int _framesToSkip;

        private int hairMode;

        private Player clonePlayer;

		public UIMultiHairStyleButton(Player player, int hairStyleId, int hairMode = 0)
			: base(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel"))
		{
            this.hairMode = hairMode;
			_player = player;
            clonePlayer = new Player();
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            var cloneModPlayer = clonePlayer.GetModPlayer<MrPlagueRacesPlayer>();
            clonePlayer.Male = _player.Male;
            clonePlayer.skinColor = _player.skinColor;
            clonePlayer.hairColor = _player.hairColor;
            clonePlayer.eyeColor = _player.eyeColor;
            clonePlayer.hair = _player.hair;
            cloneModPlayer.noShadows = true;
            cloneModPlayer.detailColor = mrPlagueRacesPlayer.detailColor;
            cloneModPlayer.auxilaryDetailColor1 = mrPlagueRacesPlayer.auxilaryDetailColor1;
            cloneModPlayer.auxilaryDetailColor2 = mrPlagueRacesPlayer.auxilaryDetailColor2;
            cloneModPlayer.auxilaryDetailColor3 = mrPlagueRacesPlayer.auxilaryDetailColor3;
            cloneModPlayer.auxilaryHairstyle1 = mrPlagueRacesPlayer.auxilaryHairstyle1;
            cloneModPlayer.auxilaryHairstyle2 = mrPlagueRacesPlayer.auxilaryHairstyle2;
            cloneModPlayer.auxilaryHairstyle3 = mrPlagueRacesPlayer.auxilaryHairstyle3;
            cloneModPlayer.race = mrPlagueRacesPlayer.race;
            HairStyleId = hairStyleId;
			Width = StyleDimension.FromPixels(44f);
			Height = StyleDimension.FromPixels(44f);
			_selectedBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight");
			_hoveredBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder");
			UseImmediateMode = true;
		}

		public void SkipRenderingContent(int timeInFrames)
		{
			_framesToSkip = timeInFrames;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            var cloneModPlayer = clonePlayer.GetModPlayer<MrPlagueRacesPlayer>();
            clonePlayer.Male = _player.Male;
            clonePlayer.skinColor = _player.skinColor;
            clonePlayer.hairColor = _player.hairColor;
            clonePlayer.eyeColor = _player.eyeColor;
            clonePlayer.hair = _player.hair;
            cloneModPlayer.noShadows = true;
            cloneModPlayer.detailColor = mrPlagueRacesPlayer.detailColor;
            cloneModPlayer.auxilaryDetailColor1 = mrPlagueRacesPlayer.auxilaryDetailColor1;
            cloneModPlayer.auxilaryDetailColor2 = mrPlagueRacesPlayer.auxilaryDetailColor2;
            cloneModPlayer.auxilaryDetailColor3 = mrPlagueRacesPlayer.auxilaryDetailColor3;
            cloneModPlayer.auxilaryHairstyle1 = mrPlagueRacesPlayer.auxilaryHairstyle1;
            cloneModPlayer.auxilaryHairstyle2 = mrPlagueRacesPlayer.auxilaryHairstyle2;
            cloneModPlayer.auxilaryHairstyle3 = mrPlagueRacesPlayer.auxilaryHairstyle3;
            cloneModPlayer.race = mrPlagueRacesPlayer.race;
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
			Vector2 vector = new Vector2(-5f, -5f);
			base.DrawSelf(spriteBatch);

            switch (hairMode)
            {
                case 0:
                    if (_player.hair == HairStyleId)
                    {
                        spriteBatch.Draw(_selectedBorderTexture.Value, GetDimensions().Center() - _selectedBorderTexture.Size() / 2f, Color.White);
                    }
                    break;
                case 1:
                    if (mrPlagueRacesPlayer.auxilaryHairstyle1 == HairStyleId)
                    {
                        spriteBatch.Draw(_selectedBorderTexture.Value, GetDimensions().Center() - _selectedBorderTexture.Size() / 2f, Color.White);
                    }
                    break;
                case 2:
                    if (mrPlagueRacesPlayer.auxilaryHairstyle2 == HairStyleId)
                    {
                        spriteBatch.Draw(_selectedBorderTexture.Value, GetDimensions().Center() - _selectedBorderTexture.Size() / 2f, Color.White);
                    }
                    break;
                default:
                    if (mrPlagueRacesPlayer.auxilaryHairstyle3 == HairStyleId)
                    {
                        spriteBatch.Draw(_selectedBorderTexture.Value, GetDimensions().Center() - _selectedBorderTexture.Size() / 2f, Color.White);
                    }
                    break;
            }
			if (_hovered)
			{
				spriteBatch.Draw(_hoveredBorderTexture.Value, GetDimensions().Center() - _hoveredBorderTexture.Size() / 2f, Color.White);
			}
			if (_framesToSkip > 0)
			{
				_framesToSkip--;
				return;
			}

            int hair;

            switch (hairMode)
            {
                case 0:
                    hair = clonePlayer.hair;
                    clonePlayer.hair = HairStyleId;
                    Main.PlayerRenderer.DrawPlayerHead(Main.Camera, clonePlayer, GetDimensions().Center() + vector);
                    clonePlayer.hair = hair;
                    break;
                case 1:
                    hair = cloneModPlayer.auxilaryHairstyle1;
                    cloneModPlayer.auxilaryHairstyle1 = HairStyleId;
                    Main.PlayerRenderer.DrawPlayerHead(Main.Camera, clonePlayer, GetDimensions().Center() + vector);
                    cloneModPlayer.auxilaryHairstyle1 = hair;
                    break;
                case 2:
                    hair = cloneModPlayer.auxilaryHairstyle2;
                    cloneModPlayer.auxilaryHairstyle2 = HairStyleId;
                    Main.PlayerRenderer.DrawPlayerHead(Main.Camera, clonePlayer, GetDimensions().Center() + vector);
                    cloneModPlayer.auxilaryHairstyle2 = hair;
                    break;
                default:
                    hair = cloneModPlayer.auxilaryHairstyle3;
                    cloneModPlayer.auxilaryHairstyle3 = HairStyleId;
                    Main.PlayerRenderer.DrawPlayerHead(Main.Camera, clonePlayer, GetDimensions().Center() + vector);
                    cloneModPlayer.auxilaryHairstyle3 = hair;
                    break;
            }
        }

		public override void LeftMouseDown(UIMouseEvent evt)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
			switch (hairMode)
			{
				case 0:
                    _player.hair = HairStyleId;
                    break;
                case 1:
                    mrPlagueRacesPlayer.auxilaryHairstyle1 = HairStyleId;
                    break;
                case 2:
                    mrPlagueRacesPlayer.auxilaryHairstyle2 = HairStyleId;
                    break;
                default:
                    mrPlagueRacesPlayer.auxilaryHairstyle3 = HairStyleId;
                    break;
            }
            if (_player.whoAmI == Main.myPlayer)
            {
                mrPlagueRacesPlayer.SyncPlayerAppearance(-1, Main.myPlayer);
            }
            SoundEngine.PlaySound(SoundID.MenuTick);
            base.LeftMouseDown(evt);
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
