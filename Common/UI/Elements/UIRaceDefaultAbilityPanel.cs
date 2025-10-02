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
	public class UIRaceDefaultAbilityPanel : UIElement
	{
		private UIText _hoverText;
		private UIPanel backingPanel;
		private UIText statText;
        private UIText titleText;
        private UISlicedImage statBackground;
        public UIRaceDefaultAbilityPanel(ref UIText hoverText)
		{
			_hoverText = hoverText;
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
			statText = new UIText(UICreateRace.enableDefaultAbility ? "Yes" : "No")
            {
				HAlign = 0f,
				VAlign = 0.5f,
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Left = StyleDimension.FromPixelsAndPercent(64f, 0f),
				Top = StyleDimension.FromPixelsAndPercent(6f, 0f),
				TextColor = Color.Gray
			};
			statBackground.Append(statText);
            titleText = new UIText("Example Ability:")
            {
                HAlign = 0f,
                VAlign = 0.5f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Left = StyleDimension.FromPixelsAndPercent(-20f, 0f),
                Top = StyleDimension.FromPixelsAndPercent(6f, 0f),
                TextColor = Color.White
            };
            statBackground.Append(titleText);
            UIPanel panelListeningElement = new UIPanel() // Prevents the menutick noise from playing multiple times when hovering over the panel's elements
            {
                HAlign = 0f,
                VAlign = 0.5f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            statBackground.Append(panelListeningElement);
        }

		public override void LeftMouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
			UICreateRace.enableDefaultAbility = !UICreateRace.enableDefaultAbility;
            base.LeftMouseDown(evt);
        }

        public override void RightMouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            UICreateRace.enableDefaultAbility = !UICreateRace.enableDefaultAbility;
            base.RightMouseDown(evt);
        }

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Color textColor = Color.Gray;
            if (UICreateRace.enableDefaultAbility)
			{
				textColor = new Color(77, 191, 96);
			}
			else
			{
                textColor = new Color(255, 54, 64);
            }
            statText.SetText(UICreateRace.enableDefaultAbility ? "Yes" : "No");
            statText.TextColor = textColor;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
			if (backingPanel.BorderColor != Colors.FancyUIFatButtonMouseOver)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            _hoverText.SetText("Include an example ability in your race.cs file.");
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
