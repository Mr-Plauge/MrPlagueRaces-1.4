using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace MrPlagueRaces.Common.UI.States
{
	public class UIRaceStatPanel : UIElement
	{
		private string _hoverText;
		public UIRaceStatPanel(Asset<Texture2D> texture, string text, string hoverText = "", float textScale = 1f, bool large = false)
		{
			_hoverText = hoverText;
			UISlicedImage statBackground = new UISlicedImage(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/CategoryPanelHighlight", (AssetRequestMode)1))
			{
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f)
			};
			statBackground.SetSliceDepths(10);
			statBackground.Color = Color.LightGray * 0.7f;
			Append(statBackground);
			UIImage statImage = new UIImage(texture)
			{
				HAlign = 0f,
				VAlign = 0.5f,
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f)
			};
			statBackground.Append(statImage);
			UIText statText = new UIText(text)
			{
				HAlign = 0f,
				VAlign = 0.5f,
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Left = StyleDimension.FromPixelsAndPercent(34f, 0f),
				Top = StyleDimension.FromPixelsAndPercent(6f, 0f)
			};
			statBackground.Append(statText);
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			MrPlagueUICharacterCreation.hoverText = _hoverText;
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			MrPlagueUICharacterCreation.hoverText = "";
		}
	}
}
