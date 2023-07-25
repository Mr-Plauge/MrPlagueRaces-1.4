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

namespace MrPlagueRaces.Common.Races.Merfolk
{
	public class Merfolk : Race
	{
		public override void Load()
        {
			Description = "Excellent at swimming and fishing, Merfolk breathe water instead of air.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] While underwater, hold Z to streamline-swim. Improves control and speed.\n[c/4DBF60:{"+"}] While underwater, press X to kick forward for a boost of momentum.";
			CensorClothing = false;
			HairColor = new Color(108, 255, 61);
			SkinColor = new Color(58, 188, 116);
			DetailColor = new Color(108, 255, 61);
			EyeColor = new Color(255, 81, 81);
		}
	}
}
