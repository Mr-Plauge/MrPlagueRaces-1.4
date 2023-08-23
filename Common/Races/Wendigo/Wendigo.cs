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

namespace MrPlagueRaces.Common.Races.Wendigo
{
	public class Wendigo : Race
	{
		public override void Load()
        {
			Description = "Made from an amalgamation of souls, Wendigoes can shred through the fabric of space.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Hold Z to tear through reality, turning you into an intangible missile. Maximum duration scales with max health.\n[c/4DBF60:{"+"}] Hold X to attack with your claws, shredding enemy defense. Damage scales with max health.";
			CensorClothing = false;
			HairColor = new Color(255, 255, 255);
			SkinColor = new Color(57, 59, 70);
			DetailColor = new Color(57, 59, 70);
			EyeColor = new Color(118, 194, 255);
		}

		public override void ResetEffects(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				player.moveSpeed += 0.25f;
				player.GetDamage(DamageClass.Generic) += 0.15f;
				player.endurance -= 0.1f;
			}
		}
	}
}
