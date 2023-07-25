using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MrPlagueRaces.Common.Races.Human
{
	public class Human : Race
	{
		public override void Load()
        {
			Description = "Surprisingly durable and resilient, Humans can adapt to any situation.";
			AbilitiesDescription = $"placeholder description. Contact _mrplague on discord if you see this.";
			StarterShirt = true;
			StarterPants = true;
			HairColor = new Color(215, 90, 55);
			SkinColor = new Color(255, 125, 90);
			DetailColor = new Color(255, 125, 90);
			EyeColor = new Color(105, 90, 75);
			ShirtColor = new Color(175, 165, 140);
			UnderShirtColor = new Color(160, 180, 215);
			PantsColor = new Color(255, 230, 175);
			ShoeColor = new Color(160, 105, 60);
		}
	}
}
