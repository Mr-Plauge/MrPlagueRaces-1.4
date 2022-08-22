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
	public class ResilientBuff: ModBuff
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Resilient");
			Description.SetDefault("Regeneration and defense are increased");
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<ResilientBuffPlayer>().resilient = true;
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<ResilientBuffNPC>().resilient = true;
		}
	}

	public class ResilientBuffPlayer : ModPlayer
	{
		public bool resilient;

		public override void ResetEffects() {
			int defenseFactor = 0;
			if (resilient) {
				for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
				{
					if (Player.armor[i].prefix == PrefixType<Resilient>())
					{
						defenseFactor += 3;
					}
				}
				Player.statDefense += defenseFactor;
			}
			resilient = false;
		}

		public override void UpdateLifeRegen()
		{
			int healFactor = 0;
			if (resilient) {
				for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
				{
					if (Player.armor[i].prefix == PrefixType<Resilient>())
					{
						healFactor += 2;
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
				Player.lifeRegen += healFactor;
			}
		}
	}

	public class ResilientBuffNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public bool resilient;

		public override void ResetEffects(NPC npc) {
			resilient = false;
		}
	}
}
