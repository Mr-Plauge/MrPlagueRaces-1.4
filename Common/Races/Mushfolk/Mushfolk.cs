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

namespace MrPlagueRaces.Common.Races.Mushfolk
{
	public class Mushfolk : Race
	{
		public override void Load()
        {
			Description = "Luminous and mysterious, the Mushfolk feed on health to fuel their healing abilities.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Hold Z to release trapshroom spores, which release healing clouds upon contact with an enemy. Healing scales with the individual's max health.\n[c/4DBF60:{"+"}] Press X to create a Nymphshroom, which teleports you to it upon contact with an enemy.";
			CensorClothing = false;
			HairColor = new Color(138, 159, 255);
			SkinColor = new Color(239, 222, 202);
			DetailColor = new Color(239, 222, 202);
			EyeColor = new Color(138, 159, 255);
		}
	}
}
