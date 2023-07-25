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
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.Races.Goblin
{
	public class Goblin : Race
	{
		public override void Load()
        {
			Description = "Known for their effective tinkering, Goblins forge their equipment with dark flames.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z to shadowforge your held item, granting it unique powers. Costs mana.\n[c/4DBF60:{"+"}] Press X to unleash a shadowflame harvester, which burns enemies and grants you improved mana regeneration.";
			ClothStyle = 2;
			HairStyle = 15;
			StarterShirt = true;
			StarterPants = true;
			HairColor = new Color(85, 96, 123);
			SkinColor = new Color(182, 215, 126);
			DetailColor = new Color(182, 215, 126);
			EyeColor = new Color(105, 90, 75);
			ShirtColor = new Color(182, 91, 91);
			UnderShirtColor = new Color(166, 113, 93);
			PantsColor = new Color(175, 227, 255);
			ShoeColor = new Color(160, 105, 60);
		}
	}
}
