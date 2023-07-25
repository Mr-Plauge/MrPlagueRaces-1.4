using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MrPlagueRaces.Content.NPCs
{
	public class StylistMenuOverride : GlobalNPC
	{
		public override bool AppliesToEntity(NPC npc, bool lateInstatiation) 
		{
			return npc.type == NPCID.Stylist;
		}

		public override void OnChatButtonClicked(NPC npc, bool firstButton = false)
		{
			Main.NewText("Hair Window has some visual bugs due to being incompatible with custom drawlayers, but should otherwise function properly. Caused by MrPlagueRaces", 77, 191, 96);
		}
	}
}
