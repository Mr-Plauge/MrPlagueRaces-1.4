using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using MrPlagueRaces.Common.UI.States;

namespace MrPlagueRaces.Common.Systems
{
	public class UIRedirectionSystem : ModSystem
	{

		public override void Load() => On.Terraria.Main.Draw += InterceptCharacterCreationMenu;
		public override void Unload() => On.Terraria.Main.Draw -= InterceptCharacterCreationMenu;

		private void InterceptCharacterCreationMenu(On.Terraria.Main.orig_Draw orig, Main self, GameTime gameTime)
		{
			UserInterface userInterface = Main.gameMenu ? Main.MenuUI : Main.InGameUI;
			if(userInterface.CurrentState is UICharacterCreation) {
				Main.PendingPlayer = new Player();
				Main.menuMode = 888;
				Main.MenuUI.SetState(new MrPlagueUICharacterCreation(Main.PendingPlayer));
			}
			orig(self, gameTime);
		}
	}
}