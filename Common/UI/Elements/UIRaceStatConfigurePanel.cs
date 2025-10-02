using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace MrPlagueRaces.Common.UI.States
{
	public class UIRaceStatConfigurePanel : UIElement
	{
		private UIText _hoverText;
		private UIPanel backingPanel;
		private UIText statText;
		private string _textToSet;
		private bool _isNegative;
		private int _myId;
		private UISlicedImage statBackground;
        public UIRaceStatConfigurePanel(Asset<Texture2D> texture, ref UIText hoverText, string textToSet, int myId, bool isNegative = false)
		{
			_hoverText = hoverText;
			_myId = myId;
			_textToSet = textToSet;
			_isNegative = isNegative;
			UICreateRace.raceStats[_myId] = 0;
			statBackground = new UISlicedImage(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/AbilityPanelHighlight", (AssetRequestMode)1))
			{
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f)
			};
			statBackground.SetSliceDepths(10);
			statBackground.Color = Color.White;
			Append(statBackground);
			backingPanel = new UIPanel()
			{
				HAlign = 0f,
				VAlign = 0.5f,
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
				BackgroundColor = new Color(63, 82, 151) * 0.7f,
                BorderColor = new Color(89, 116, 213) * 0.7f
            };
			statBackground.Append(backingPanel);
			statText = new UIText(UICreateRace.raceStats[_myId].ToString())
            {
				HAlign = 0f,
				VAlign = 0.5f,
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Left = StyleDimension.FromPixelsAndPercent(34f, 0f),
				Top = StyleDimension.FromPixelsAndPercent(6f, 0f),
				TextColor = Color.Gray
			};
			statBackground.Append(statText);
            UIImage statImage = new UIImage(texture)
            {
                HAlign = 0f,
                VAlign = 0.5f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f)
            };
            statBackground.Append(statImage);
        }

		public override void LeftMouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
			if (UICreateRace.raceStats[_myId] < 100)
			{
                UICreateRace.raceStats[_myId] += 1;
            }
            base.LeftMouseDown(evt);
        }

        public override void RightMouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (UICreateRace.raceStats[_myId] > -100)
            {
                UICreateRace.raceStats[_myId] -= 1;
            }
            base.RightMouseDown(evt);
        }

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Color textColor = Color.Gray;
			string plusAddition = "+";
			if (UICreateRace.raceStats[_myId] > 0)
			{
				textColor = _isNegative ? new Color(255, 54, 64) : new Color(77, 191, 96);
			}
			else if (UICreateRace.raceStats[_myId] < 0)
			{
                textColor = _isNegative ? new Color(77, 191, 96) : new Color(255, 54, 64);
                plusAddition = "";
            }
			statText.SetText(plusAddition + UICreateRace.raceStats[_myId].ToString() + "%");
			statText.TextColor = textColor;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            _hoverText.SetText(_textToSet);
            backingPanel.BorderColor = Colors.FancyUIFatButtonMouseOver;
            backingPanel.BackgroundColor = new Color(73, 94, 171);
        }

		public override void MouseOut(UIMouseEvent evt)
		{
            _hoverText.SetText(Language.GetText("Workshop.HubDescriptionDefault"));
            backingPanel.BorderColor = new Color(89, 116, 213) * 0.7f;
			backingPanel.BackgroundColor = new Color(63, 82, 151) * 0.7f;
        }
	}
}
