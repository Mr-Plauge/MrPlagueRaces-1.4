using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Prefixes;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Buffs
{
	public class FlawlessBuff: ModBuff
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Flawless");
			// Description.SetDefault("Critical strike chance and damage are greatly increased");
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<FlawlessBuffPlayer>().flawless = true;
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<FlawlessBuffNPC>().flawless = true;
		}
	}

	public class FlawlessBuffPlayer : ModPlayer
	{
		public bool flawless;

		public override void ResetEffects() {
			int critFactor = 0;
			float damageFactor = 0;
			if (flawless) {
				for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
				{
					if (Player.armor[i].prefix == PrefixType<Flawless>())
					{
						critFactor += 5;
						damageFactor += 0.1f;
					}
				}
				Player.GetCritChance(DamageClass.Generic) += critFactor;
				Player.GetDamage(DamageClass.Generic) += damageFactor;
			}
			flawless = false;
		}
	}

	public class FlawlessBuffNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public bool flawless;

		public override void ResetEffects(NPC npc) {
			flawless = false;
		}
	}
}
