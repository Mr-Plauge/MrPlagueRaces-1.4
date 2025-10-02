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
using MrPlagueRaces.Common.UI.States;

namespace MrPlagueRaces.Common.UI.Elements
{
	public class UIRaceButton : UIElement
	{
		public Player _player;

		public Player _clonePlayer;

        public readonly int RaceId;

		public readonly bool IsChangeButton;

        public readonly bool IsClothStyleButton;

        public readonly Asset<Texture2D> _BasePanelTexture;

		public readonly Asset<Texture2D> _selectedBorderTexture;

		public readonly Asset<Texture2D> _hoveredBorderTexture;

		public readonly UICharacter _char;

		public bool _hovered;

		public bool _soundedHover;

		public Race _realRace;
		public int _realSkinVariant;
		public int _realHair;
        public int _realHairAux1;
        public int _realHairAux2;
        public int _realHairAux3;
        public Color _realHairColor;
		public Color _realSkinColor;
		public Color _realDetailColor;
        public Color _realDetailColorAux1;
        public Color _realDetailColorAux2;
        public Color _realDetailColorAux3;
        public Color _realEyeColor;
		public Color _realShirtColor;
		public Color _realUnderShirtColor;
		public Color _realPantsColor;
		public Color _realShoeColor;

		public UIRaceButton(Player player, int raceId, bool isChangeButton = false, bool isClothStyleButton = false)
		{
			_player = player;
            _clonePlayer = new Player();
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            var cloneModPlayer = _clonePlayer.GetModPlayer<MrPlagueRacesPlayer>();
            _clonePlayer.skinVariant = _player.skinVariant;
            _clonePlayer.skinColor = _player.skinColor;
            _clonePlayer.hairColor = _player.hairColor;
            _clonePlayer.eyeColor = _player.eyeColor;
            _clonePlayer.hair = _player.hair;
            cloneModPlayer.noShadows = true;
            cloneModPlayer.detailColor = mrPlagueRacesPlayer.detailColor;
            cloneModPlayer.auxilaryDetailColor1 = mrPlagueRacesPlayer.auxilaryDetailColor1;
            cloneModPlayer.auxilaryDetailColor2 = mrPlagueRacesPlayer.auxilaryDetailColor2;
            cloneModPlayer.auxilaryDetailColor3 = mrPlagueRacesPlayer.auxilaryDetailColor3;
            cloneModPlayer.auxilaryHairstyle1 = mrPlagueRacesPlayer.auxilaryHairstyle1;
            cloneModPlayer.auxilaryHairstyle2 = mrPlagueRacesPlayer.auxilaryHairstyle2;
            cloneModPlayer.auxilaryHairstyle3 = mrPlagueRacesPlayer.auxilaryHairstyle3;
            cloneModPlayer.race = mrPlagueRacesPlayer.race;
            RaceId = raceId;
			IsChangeButton = isChangeButton;
            IsClothStyleButton = isClothStyleButton;
            Width = StyleDimension.FromPixels(44f);
			Height = StyleDimension.FromPixels(80f);
			_BasePanelTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel", (AssetRequestMode)1);
			_selectedBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight", (AssetRequestMode)1);
			_hoveredBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder", (AssetRequestMode)1);
			_char = new UICharacter(_clonePlayer, animated: false, hasBackPanel: false)
			{
				HAlign = 0.5f,
				VAlign = 0.5f
			};
			Append(_char);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
            if (IsChangeButton || IsClothStyleButton)
            {
                _player = Main.LocalPlayer;
            }
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            var cloneModPlayer = _clonePlayer.GetModPlayer<MrPlagueRacesPlayer>();
            _realRace = mrPlagueRacesPlayer.race;
            cloneModPlayer.race = RaceLoader.Races[RaceId];
			SetRaceValues(_clonePlayer, _clonePlayer); // set the clone's values
            base.Draw(spriteBatch);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            var cloneModPlayer = _clonePlayer.GetModPlayer<MrPlagueRacesPlayer>();
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
            if (IsClothStyleButton)
            {
                _clonePlayer.Male = _player.Male;
                _clonePlayer.skinVariant = _player.skinVariant;
                _clonePlayer.skinColor = _player.skinColor;
                _clonePlayer.hairColor = _player.hairColor;
                _clonePlayer.eyeColor = _player.eyeColor;
                _clonePlayer.hair = _player.hair;
                _clonePlayer.shirtColor = _player.shirtColor;
                _clonePlayer.underShirtColor = _player.underShirtColor;
                _clonePlayer.pantsColor = _player.pantsColor;
                _clonePlayer.shoeColor = _player.shoeColor;
                cloneModPlayer.noShadows = true;
                cloneModPlayer.detailColor = mrPlagueRacesPlayer.detailColor;
                cloneModPlayer.auxilaryDetailColor1 = mrPlagueRacesPlayer.auxilaryDetailColor1;
                cloneModPlayer.auxilaryDetailColor2 = mrPlagueRacesPlayer.auxilaryDetailColor2;
                cloneModPlayer.auxilaryDetailColor3 = mrPlagueRacesPlayer.auxilaryDetailColor3;
                cloneModPlayer.auxilaryHairstyle1 = mrPlagueRacesPlayer.auxilaryHairstyle1;
                cloneModPlayer.auxilaryHairstyle2 = mrPlagueRacesPlayer.auxilaryHairstyle2;
                cloneModPlayer.auxilaryHairstyle3 = mrPlagueRacesPlayer.auxilaryHairstyle3;
                cloneModPlayer.race = mrPlagueRacesPlayer.race;
            }
		}

		public override void LeftMouseDown(UIMouseEvent evt)
        {
            var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
            var raceHookPlayer = _player.GetModPlayer<RaceHookPlayer>();

            if (IsChangeButton)
            {
                raceHookPlayer.PreRaceChange(); // preracechange is set here
                mrPlagueRacesPlayer.race = RaceLoader.Races[RaceId];
                raceHookPlayer.PostRaceChange(); // preracechange is set here
                if (PlayerInput.Triggers.Current.Down)
				{
                    SetRaceValues(_player, _clonePlayer, true, false, false, false);
                }
				else
				{
                    SetRaceValues(_player, _clonePlayer, true, false, false);
                }
                if (_player.whoAmI == Main.myPlayer)
                {
                    mrPlagueRacesPlayer.SyncRace(-1, Main.myPlayer);
                    mrPlagueRacesPlayer.SyncPlayerAppearance(-1, Main.myPlayer);
                }
            }
            else if (IsClothStyleButton)
            {
                SetRaceValues(_player, _clonePlayer, false, true, false, false);
                if (_player.whoAmI == Main.myPlayer)
                {
                    mrPlagueRacesPlayer.SyncPlayerAppearance(-1, Main.myPlayer);
                }
            }
			else
            {
                raceHookPlayer.PreRaceChange(); // preracechange is set here
                mrPlagueRacesPlayer.race = RaceLoader.Races[RaceId];
                raceHookPlayer.PostRaceChange(); // preracechange is set here
                SetRaceValues(_player, _clonePlayer);
            }
            SoundEngine.PlaySound(SoundID.MenuTick);
            base.LeftMouseDown(evt);
        }

        public override void MouseOver(UIMouseEvent evt)
		{
			_hovered = true;
			_char.SetAnimated(animated: true);
            if (IsChangeButton)
            {
                var cloneModPlayer = _clonePlayer.GetModPlayer<MrPlagueRacesPlayer>();
                if (cloneModPlayer.race.Description != null && cloneModPlayer.race.Description != "")
                {
                    RaceChangeUI.hoverText = cloneModPlayer.race.Description + (cloneModPlayer.race.AbilitiesDescription != null && cloneModPlayer.race.AbilitiesDescription != "" ? ("\n" + cloneModPlayer.race.AbilitiesDescription) : "");
                }
            }
            base.MouseOver(evt);
        }

		public override void MouseOut(UIMouseEvent evt)
		{
			_hovered = false;
			_char.SetAnimated(animated: false); 
            if (IsChangeButton)
            {
                RaceChangeUI.hoverText = "";
            }
            base.MouseOut(evt);
        }

		public void SetRaceValues(Player player, Player clonePlayer, bool shouldReplaceHair = true, bool shouldReplaceClothingStyle = true, bool shouldReplaceClothingColors = true, bool shouldReplaceSkinValues = true)
        {
            Item blankItem = new Item();
            blankItem.SetDefaults(0);
            Item familiarShirt = new Item();
            familiarShirt.SetDefaults(ItemID.FamiliarShirt);
            Item familiarPants = new Item();
            familiarPants.SetDefaults(ItemID.FamiliarPants);
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            var cloneModPlayer = clonePlayer.GetModPlayer<MrPlagueRacesPlayer>();

            if (shouldReplaceClothingStyle)
            {
                if (IsClothStyleButton)
                {
                    if (player.Male)
                    {
                        switch (player.skinVariant)
                        {
                            case 0:
                                player.skinVariant = 2;
                                break;
                            case 2:
                                player.skinVariant = 1;
                                break;
                            case 1:
                                player.skinVariant = 3;
                                break;
                            case 3:
                                player.skinVariant = 8;
                                break;
                            case 8:
                                player.skinVariant = 0;
                                break;
                        }
                    }
                    else
                    {
                        switch (player.skinVariant)
                        {
                            case 4:
                                player.skinVariant = 6;
                                break;
                            case 6:
                                player.skinVariant = 5;
                                break;
                            case 5:
                                player.skinVariant = 7;
                                break;
                            case 7:
                                player.skinVariant = 9;
                                break;
                            case 9:
                                player.skinVariant = 4;
                                break;
                        }
                    }
                }
                else
                {
                    player.skinVariant = (player.Male ? PlayerLayerHelpers.MaleClothingIDs[cloneModPlayer.race.ClothStyle - 1] : PlayerLayerHelpers.FemaleClothingIDs[cloneModPlayer.race.ClothStyle - 1]);
                }
            }
            if (shouldReplaceClothingColors)
            {
                player.armor[11] = blankItem;
                player.armor[12] = blankItem;

                if (cloneModPlayer.race.StarterShirt || IsClothStyleButton)
                {
                    player.armor[11] = familiarShirt;
                }
                if (cloneModPlayer.race.StarterPants || IsClothStyleButton)
                {
                    player.armor[12] = familiarPants;
                }
                player.shirtColor = cloneModPlayer.race.ShirtColor;
                player.underShirtColor = cloneModPlayer.race.UnderShirtColor;
                player.pantsColor = cloneModPlayer.race.PantsColor;
                player.shoeColor = cloneModPlayer.race.ShoeColor;
            }
            if (shouldReplaceSkinValues)
            {
                player.hairColor = cloneModPlayer.race.HairColor;
                player.skinColor = cloneModPlayer.race.SkinColor;
                mrPlagueRacesPlayer.detailColor = cloneModPlayer.race.DetailColor;
                mrPlagueRacesPlayer.auxilaryDetailColor1 = cloneModPlayer.race.AuxilaryDetailColor1;
                mrPlagueRacesPlayer.auxilaryDetailColor2 = cloneModPlayer.race.AuxilaryDetailColor2;
                mrPlagueRacesPlayer.auxilaryDetailColor3 = cloneModPlayer.race.AuxilaryDetailColor3;
                player.eyeColor = cloneModPlayer.race.EyeColor;
            }
            if (shouldReplaceHair)
            {
                player.hair = cloneModPlayer.race.HairStyle;
                mrPlagueRacesPlayer.auxilaryHairstyle1 = cloneModPlayer.race.AuxilaryHairstyle1;
                mrPlagueRacesPlayer.auxilaryHairstyle2 = cloneModPlayer.race.AuxilaryHairstyle2;
                mrPlagueRacesPlayer.auxilaryHairstyle3 = cloneModPlayer.race.AuxilaryHairstyle3;
            }
            cloneModPlayer.race.PostSetRaceValues(ref player, ref clonePlayer, ref shouldReplaceHair, ref shouldReplaceClothingStyle, ref shouldReplaceClothingColors, ref shouldReplaceSkinValues);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
