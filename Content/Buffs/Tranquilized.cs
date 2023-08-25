using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Buffs
{
	public class Tranquilized: ModBuff
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Tranquilized");
			// Description.SetDefault("You are on fire");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<TranquilizedPlayer>().tranquilized = true;
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<TranquilizedNPC>().tranquilized = true;
		}
	}

	public class TranquilizedPlayer : ModPlayer
	{
		public bool tranquilized;

		public override void ResetEffects() {
			if (tranquilized) {
				Player.velocity.X *= 0.5f;
			}
			tranquilized = false;
		}
	}

	public class TranquilizedNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public bool tranquilized;

		public override void ResetEffects(NPC npc) {
			if (tranquilized) {
				npc.velocity.X *= 0.5f;
			}
			tranquilized = false;
		}
	}
}
