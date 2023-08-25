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
	public class SoulFracture: ModBuff
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Soul Fracture");
			// Description.SetDefault("A wound so deep it cuts through the spirit\nDefense is reduced to 1%");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<SoulFracturePlayer>().SoulFracture = true;
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<SoulFractureNPC>().SoulFracture = true;
		}
	}

	public class SoulFracturePlayer : ModPlayer
	{
		public bool SoulFracture;

		public override void ResetEffects() {
			SoulFracture = false;
			if (SoulFracture) {
				Player.statDefense /= 100;
			}
		}

		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright) {
			if (SoulFracture && Main.rand.Next(5) < 4) {
				int dust = Dust.NewDust(new Vector2(Player.position.X - 2f, Player.position.Y - 2f), Player.width + 4, Player.height + 4, 264);
				Main.dust[dust].color = new Color(221, 65, 65);
				Main.dust[dust].noGravity = true;
			}
		}
	}

	public class SoulFractureNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public bool SoulFracture;

		public override void ResetEffects(NPC npc) {
			SoulFracture = false;
			if (SoulFracture) {
				npc.defense /= 100;
			}
		}

		public override void DrawEffects(NPC npc, ref Color drawColor) {
			if (SoulFracture && Main.rand.Next(5) < 4) {
				int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, 264);
				Main.dust[dust].color = new Color(221, 65, 65);
				Main.dust[dust].noGravity = true;
			}
		}
	}
}
