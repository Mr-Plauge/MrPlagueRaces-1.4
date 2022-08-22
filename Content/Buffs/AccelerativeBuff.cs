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
	public class AccelerativeBuff: ModBuff
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Accelerative");
			Description.SetDefault("You accelerate to unbearable degrees");
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<AccelerativeBuffPlayer>().accelerative = true;
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<AccelerativeBuffNPC>().accelerative = true;
		}
	}

	public class AccelerativeBuffPlayer : ModPlayer
	{
		public bool accelerative;

		public override void ResetEffects() {
			if (accelerative) {
				Player.maxRunSpeed += 5f;
				Player.runAcceleration += 0.05f;
			}
			accelerative = false;
		}

		public override void PreUpdate() {
			if (accelerative)
			{
				if (Player.velocity.X > 6 || Player.velocity.X < -6)
				{
					for (int j = 0; j < 50; j++) {
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
	}

	public class AccelerativeBuffNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public bool accelerative;

		public override void ResetEffects(NPC npc) {
			accelerative = false;
		}
	}
}
