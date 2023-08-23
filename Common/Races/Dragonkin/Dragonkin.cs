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

namespace MrPlagueRaces.Common.Races.Dragonkin
{
	public class Dragonkin : Race
	{
		public override void Load()
        {
			Description = "Built for living in arid environments, Dragonkin are strong and endurant.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Hold Z to breathe burning smoke. Damage scales with defense.\n[c/4DBF60:{"+"}] Press X to release a molotov fireball. Damage scales with defense.";
			CensorClothing = false;
			StarterShirt = true;
			StarterPants = true;
			ClothStyle = 5;
			AlwaysDrawHair = true;
			HairColor = new Color(214, 207, 199);
			SkinColor = new Color(106, 138, 110);
			DetailColor = new Color(182, 215, 126);
			EyeColor = new Color(255, 180, 92);
			ShirtColor = new Color(119, 115, 157);
			UnderShirtColor = new Color(216, 156, 95);
		}

		public override void ResetEffects(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				player.endurance += 0.2f;
				player.jumpSpeedBoost -= 0.1f;
				player.moveSpeed -= 0.2f;
			}
		}
	}
}
