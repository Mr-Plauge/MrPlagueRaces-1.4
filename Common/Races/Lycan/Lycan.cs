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

namespace MrPlagueRaces.Common.Races.Lycan
{
	public class Lycan : Race
	{
		public override void Load()
        {
			Description = "Temporally anomalous, Lycans rewind and reiterate through the situations they create.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Hold Z to record time. Let go to rewind time, rewinding all nearby entities to their previous locations.\n[c/4DBF60:{"+"}] Hold X to replay your last rewound timeline. Let go to teleport to the current position in the replay.";
			CensorClothing = false;
			StarterShirt = true;
			HairColor = new Color(244, 245, 246);
			SkinColor = new Color(117, 144, 167);
			DetailColor = new Color(70, 82, 111);
			EyeColor = new Color(255, 61, 114);
			ShirtColor = new Color(198, 173, 158);
			UnderShirtColor = new Color(244, 109, 109);
		}
	}
}
