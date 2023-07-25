using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.ModLoader.UI;
using Terraria;

namespace MrPlagueRaces.Common.UI.Elements
{
	internal class UIFocusInputTextField_mp : UIElement
	{
		public delegate void EventHandler(object sender, EventArgs e);

		internal bool Focused;

		internal string CurrentString = "";

		private readonly string _hintText;

		private int _textBlinkerCount;

		private int _textBlinkerState;

		public bool UnfocusOnTab
		{
			get;
			internal set;
		}

		public event EventHandler OnTextChange;

		public event EventHandler OnUnfocus;

		public event EventHandler OnTab;

		public UIFocusInputTextField_mp(string hintText)
		{
			_hintText = hintText;
		}

		public void SetText(string text)
		{
			if (text == null)
			{
				text = "";
			}
			if (CurrentString != text)
			{
				CurrentString = text;
				this.OnTextChange?.Invoke(this, new EventArgs());
			}
		}

		public override void LeftClick(UIMouseEvent evt)
		{
			Main.clrInput();
			Focused = true;
		}

		public override void Update(GameTime gameTime)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			Vector2 MousePosition = default(Vector2);
			MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			if (!ContainsPoint(MousePosition) && Main.mouseLeft)
			{
				Focused = false;
				this.OnUnfocus?.Invoke(this, new EventArgs());
			}
			base.Update(gameTime);
		}

		private static bool JustPressed(Keys key)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (((KeyboardState)(Main.inputText)).IsKeyDown(key))
			{
				return !((KeyboardState)(Main.oldInputText)).IsKeyDown(key);
			}
			return false;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			if (Focused)
			{
				PlayerInput.WritingText = true;
				Main.instance.HandleIME();
				string newString = Main.GetInputText(CurrentString);
				if (!newString.Equals(CurrentString))
				{
					CurrentString = newString;
					this.OnTextChange?.Invoke(this, new EventArgs());
				}
				else
				{
					CurrentString = newString;
				}
				if (JustPressed((Keys)9))
				{
					if (UnfocusOnTab)
					{
						Focused = false;
						this.OnUnfocus?.Invoke(this, new EventArgs());
					}
					this.OnTab?.Invoke(this, new EventArgs());
				}
				if (++_textBlinkerCount >= 20)
				{
					_textBlinkerState = (_textBlinkerState + 1) % 2;
					_textBlinkerCount = 0;
				}
			}
			string displayString = CurrentString;
			if (_textBlinkerState == 1 && Focused)
			{
				displayString += "|";
			}
			CalculatedStyle space = GetDimensions();
			if (CurrentString.Length == 0 && !Focused)
			{
				Utils.DrawBorderString(spriteBatch, _hintText, new Vector2(space.X, space.Y), Color.Gray);
			}
			else
			{
				Utils.DrawBorderString(spriteBatch, displayString, new Vector2(space.X, space.Y), Color.White);
			}
		}
	}
}
