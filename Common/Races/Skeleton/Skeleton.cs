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

namespace MrPlagueRaces.Common.Races.Skeleton
{
	public class Skeleton : Race
	{
		public override void Load()
        {
			Description = "Reborn through a variety of rituals, most Skeletons go insane upon reanimation.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z, X, and C to switch between bodies.\n[c/4DBF60:{"+"}] When you die, your skeleton releases its spirit. If you can survive without being hit for long enough, you reform a new skeleton and return to life.";
			ClothStyle = 3;
			CensorClothing = false;
			StarterShirt = true;
			StarterPants = true;
			AlwaysDrawHair = true;
			HairColor = new Color(237, 208, 165);
			SkinColor = new Color(237, 208, 165);
			DetailColor = new Color(237, 208, 165);
			EyeColor = new Color(255, 91, 119);
			ShirtColor = new Color(203, 177, 155);
			UnderShirtColor = new Color(210, 111, 111);
			ShoeColor = new Color(146, 119, 97);
		}

		public override void ResetEffects(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				player.moveSpeed += 0.15f;
				player.GetDamage(DamageClass.Generic) += 0.1f;
				player.endurance -= 0.5f;
				player.gills = true;
			}
		}
	}
}
