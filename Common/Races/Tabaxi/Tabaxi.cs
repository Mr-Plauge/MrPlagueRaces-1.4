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

namespace MrPlagueRaces.Common.Races.Tabaxi
{
	public class Tabaxi : Race
	{
		public override void Load()
        {
			Description = "Nomadic and agile, Tabaxi are motivated by ambition and curiosity.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Hold Z to charge up an intangible dash.\n[c/4DBF60:{"+"}] Press X to set a rewind point, press C to return to it.";
			ClothStyle = 4;
			CensorClothing = false;
			StarterShirt = true;
			StarterPants = true;
			AlwaysDrawHair = true;
			HairColor = new Color(255, 247, 200);
			SkinColor = new Color(237, 208, 165);
			DetailColor = new Color(239, 119, 157);
			EyeColor = new Color(150, 255, 194);
			ShirtColor = new Color(180, 112, 101);
			UnderShirtColor = new Color(108, 74, 61);
			PantsColor = new Color(245, 213, 193);
			ShoeColor = new Color(180, 112, 101);
		}
	}
}
