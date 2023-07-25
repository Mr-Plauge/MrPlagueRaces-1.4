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

namespace MrPlagueRaces.Common.Races.Derpkin
{
	public class Derpkin : Race
	{
		public override void Load()
        {
			Description = "Native to the jungle surface, Derpkin are aerodynamic and deadly.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z to leap towards your cursor.\n[c/4DBF60:{"+"}] Press X to spin, gaining temporary invincibility.";
			CensorClothing = false;
			HairColor = new Color(82, 179, 255);
			SkinColor = new Color(82, 179, 255);
			DetailColor = new Color(48, 76, 128);
			EyeColor = new Color(99, 122, 207);
		}
	}
}
