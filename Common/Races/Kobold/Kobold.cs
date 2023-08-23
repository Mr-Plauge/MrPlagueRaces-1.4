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

namespace MrPlagueRaces.Common.Races.Kobold
{
	public class Kobold : Race
	{
		public override void Load()
        {
			Description = "Adapted for living in subterranean environments, Kobolds are talented miners.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z to breathe out a sparkmine.\n[c/4DBF60:{"+"}] Press X to fire a cluster of mines.\n[c/4DBF60:{"+"}] Press C to detonate any mines within range. Clustermines have an unlimited activation radius.\n[c/FF3640:{"-"}] Sunlight slows and weakens you.";
			CensorClothing = false;
			StarterShirt = true;
			ClothStyle = 2;
			HairColor = new Color(217, 196, 196);
			SkinColor = new Color(171, 87, 80);
			DetailColor = new Color(255, 125, 90);
			EyeColor = new Color(255, 180, 92);
			ShirtColor = new Color(216, 156, 95);
			UnderShirtColor = new Color(119, 115, 157);
		}

		public override void ResetEffects(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				player.pickSpeed -= 0.75f;
				player.moveSpeed += 0.05f;
				player.statLifeMax2 -= (player.statLifeMax2 / 10);
				player.endurance -= 0.05f;
			}
		}
	}
}
