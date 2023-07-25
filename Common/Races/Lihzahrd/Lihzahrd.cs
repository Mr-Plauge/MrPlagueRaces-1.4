using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using MrPlagueRaces.Common.UI.States;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.Races.Lihzahrd
{
	public class Lihzahrd : Race
	{
		public override void Load()
        {
			Description = "Reclusive and highly advanced, Lihzahrds are known for the quality of their creations.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z to open a golem placing menu. Scroll to cycle through selected golems. Damage scales with defense.\n[c/4DBF60:{"+"}] Hold X to crawl, allowing you to climb up walls and fit into small gaps.\n[c/FF3640:{"-"}] You lose defense and regeneration outside of the sun.";
			CensorClothing = false;
			StarterShirt = true;
			StarterPants = true;
			ClothStyle = 4;
			HairColor = new Color(216, 255, 93);
			SkinColor = new Color(216, 255, 93);
			DetailColor = new Color(241, 244, 156);
			EyeColor = new Color(115, 107, 0);
			ShirtColor = new Color(201, 110, 75);
			UnderShirtColor = new Color(137, 161, 214);
		}
	}
}
