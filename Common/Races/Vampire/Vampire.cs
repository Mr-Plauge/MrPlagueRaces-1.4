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

namespace MrPlagueRaces.Common.Races.Vampire
{
	public class Vampire : Race
	{
		public override void Load()
        {
			Description = "Naturally gifted with a form of soul magic, Vampires can morph into a bat-like state.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z to transform into a bat. You enter stealth after a few seconds, making enemies ignore you.\n[c/4DBF60:{"+"}] Press X while in batform to release your abnormally long tongue, which latches onto enemies and drains their health.\n[c/FF3640:{"-"}] You cannot use healing potions.\n[c/FF3640:{"-"}] Getting attacked in sunlight burns you.";
			CensorClothing = false;
			HairColor = new Color(91, 86, 94);
			SkinColor = new Color(91, 86, 94);
			DetailColor = new Color(175, 165, 140);
			EyeColor = new Color(255, 81, 81);
		}

		public override void ResetEffects(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				player.moveSpeed += 0.1f;
				/*if (player.mount.Type == MountType<StealthBat>()) {
					player.endurance -= 0.5f;
				}
				else {
					player.endurance -= 0.15f;
				}*/
				player.statLifeMax2 -= (player.statLifeMax2 / 4);
			}
		}
	}
}
