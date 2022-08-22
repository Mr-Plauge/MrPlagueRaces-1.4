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
	public class Harvested: ModBuff
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Harvested");
			Description.SetDefault("Defense is quartered");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<HarvestedPlayer>().harvested = true;
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<HarvestedNPC>().harvested = true;
		}
	}

	public class HarvestedPlayer : ModPlayer
	{
		public bool harvested;

		public override void ResetEffects() {
			harvested = false;
			Player.statDefense /= 4;
		}

		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright) {
			if (harvested && Main.rand.Next(5) < 4 && !Player.dead) {
				Dust dust19 = Dust.NewDustDirect(new Vector2(Player.position.X - 2f, Player.position.Y - 2f), Player.width + 4, Player.height + 4, 27, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 180, default(Color), 1.95f);
				dust19.noGravity = true;
				dust19.velocity *= 0.75f;
				dust19.velocity.X *= 0.75f;
				dust19.velocity.Y -= 1f;
				if (Main.rand.Next(4) == 0)
				{
					dust19.noGravity = false;
					dust19.scale *= 0.5f;
				}
			}
		}
	}

	public class HarvestedNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public bool harvested;

		public override void ResetEffects(NPC npc) {
			harvested = false;
			npc.defense /= 4;
		}

		public override void DrawEffects(NPC npc, ref Color drawColor) {
			if (harvested && Main.rand.Next(5) < 4) {
				Dust dust19 = Dust.NewDustDirect(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, 27, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 180, default(Color), 1.95f);
				dust19.noGravity = true;
				dust19.velocity *= 0.75f;
				dust19.velocity.X *= 0.75f;
				dust19.velocity.Y -= 1f;
				if (Main.rand.Next(4) == 0)
				{
					dust19.noGravity = false;
					dust19.scale *= 0.5f;
				}
			}
		}
	}
}
