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
	public class RevitalizingBuff: ModBuff
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Revitalizing");
			// Description.SetDefault("Regeneration is increased");
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<RevitalizingBuffPlayer>().Revitalizing = true;
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<RevitalizingBuffNPC>().Revitalizing = true;
		}
	}

	public class RevitalizingBuffPlayer : ModPlayer
	{
		public bool Revitalizing;

		public override void UpdateLifeRegen()
		{
			if (Revitalizing) {
				Player.lifeRegen += 20;
				if (Player.statLife < Player.statLifeMax2) {
					if (Main.rand.Next(50) == 1) {
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
		}
	}

	public class RevitalizingBuffNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public bool Revitalizing;

		public override void ResetEffects(NPC npc) {
			Revitalizing = false;
		}
	}
}
