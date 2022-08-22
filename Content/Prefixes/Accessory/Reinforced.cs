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
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Content.Projectiles;
using MrPlagueRaces.Content.Prefixes;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Prefixes
{
	public class Reinforced : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Accessory;

		public override bool CanRoll(Item item) {
			return false;
		}

		public override void ModifyValue(ref float valueMult) {
			valueMult *= 1.75f;
		}

		public override void Apply(Item item)
		{
			if (item.rare > -12) {
				if (item.rare >= ItemRarityID.LightRed) {
					item.rare = ItemRarityID.Cyan;
				}
				else {
					item.rare = ItemRarityID.LightRed;
				}
			}
		}
	}

	public class ReinforcedItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.prefix == PrefixType<Reinforced>())
			{
				var linePower = new TooltipLine(Mod, "Reinforced", "You get a recharging shield that absorbs the damage of one attack")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				var lineStat = new TooltipLine(Mod, "Reinforced", "+3 defense")
				{
					IsModifier = true
				};
				tooltips.Add(linePower);
				tooltips.Add(lineStat);
			}
		}
	}

	public class ReinforcedPlayer : ModPlayer
	{
		int reinforcedFactor;
		public override void ResetEffects() {
			reinforcedFactor = 0;
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Reinforced>())
				{
					Player.statDefense += 3;
					reinforcedFactor += 1;
				}
			}
			if (reinforcedFactor > 0 && !Player.HasBuff(BuffType<Shielded>())) {
				Player.endurance += 0.75f * reinforcedFactor;
			}
		}

		public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
		{
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Reinforced>() && !Player.HasBuff(BuffType<Shielded>()))
				{
					SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Player.Center);
					SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, Player.Center);
					for (int j = 0; j < 300; j++) {
						if (Main.rand.Next(5) == 1) {
							Dust dust19 = Dust.NewDustDirect(new Vector2(Player.position.X - 2f, Player.position.Y - 2f), Player.width + 4, Player.height + 4, 27, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 180, default(Color), 1.95f);
							dust19.noGravity = true;
							dust19.velocity *= 2f;
							dust19.velocity.X *= 0.75f;
							dust19.velocity.Y -= 1f;
							if (Main.rand.Next(4) == 0)
							{
								dust19.noGravity = false;
								dust19.scale *= 0.5f;
							}
						}
					}
					Player.AddBuff(BuffType<Shielded>(), 300 - reinforcedFactor * 10);
				}
			}
		}
	}
}