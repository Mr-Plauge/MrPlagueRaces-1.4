using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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

namespace MrPlagueRaces.Common.Races.Kenku
{
	public class Kenku : Race
	{
		public override void Load()
        {
			Description = "Naturally capable of flight, Kenku possess greater control over aerial movement.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] While airborne, press Z to swoop forwards.\n[c/4DBF60:{"+"}] Press X to fire a volley of feathers. Fly within range of the feathers to launch yourself.\n[c/4DBF60:{"+"}] You are equipped with natural wings by default. Their power scales with max health.\n[c/4DBF60:{"+"}] At above 160 max health, you no longer take fall damage.";
			CensorClothing = false;
			StarterShirt = true;
			HairColor = new Color(120, 96, 172);
			SkinColor = new Color(120, 96, 172);
			DetailColor = new Color(171, 173, 212);
			EyeColor = new Color(151, 73, 0);
			ShirtColor = new Color(201, 180, 177);
			UnderShirtColor = new Color(199, 122, 156);
		}
	}
}
