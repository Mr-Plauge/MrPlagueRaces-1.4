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
using MrPlagueRaces.Common.UI.States;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.Races.Fluftrodon
{
	public class Fluftrodon : Race
	{
		public override void Load()
        {
			Description = "Capable of generating paint via an arcane process, Fluftrodons highly value the arts.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z to paint. Left click and right click to paint tiles and walls, scroll to cycle color.\n[c/4DBF60:{"+"}] Hold X to charge up a powerful leap. Scales with max health.\n[c/4DBF60:{"+"}] Hold A or D against a wall and press space to walljump. Scales with max health.";
			CensorClothing = false;
			HairColor = new Color(91, 115, 177);
			SkinColor = new Color(190, 233, 255);
			DetailColor = new Color(91, 115, 177);
			EyeColor = new Color(81, 135, 255);
		}

		public override void ResetEffects(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				player.tileSpeed += 0.5f;
				player.blockRange += 10;
				player.pickSpeed -= 0.25f;
				player.GetDamage(DamageClass.Generic) -= 0.15f;
			}
		}
	}
}
