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
	public class Undying : ModPrefix
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

	public class UndyingItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.prefix == PrefixType<Undying>())
			{
				var linePower = new TooltipLine(Mod, "Undying", "Fatal damage is absorbed, giving you a second chance")
				{
					OverrideColor = new Color(212, 163, 255)
				};
				var lineStat = new TooltipLine(Mod, "Undying", "+3% damage")
				{
					IsModifier = true
				};
				tooltips.Add(linePower);
				tooltips.Add(lineStat);
			}
		}
	}

	public class UndyingPlayer : ModPlayer
	{
		int undyingFactor;

		public override void ResetEffects() {
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Undying>())
				{
					Player.GetDamage(DamageClass.Generic) += 0.03f;
				}
			}
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			undyingFactor = 0;
			for (int i = 0; i <= 7 + Player.extraAccessorySlots; i++)
			{
				if (Player.armor[i].prefix == PrefixType<Undying>() && !Player.HasBuff(BuffType<Reincarnated>())) {
					undyingFactor += 1;
				}
			}
			if (undyingFactor > 0) {
				Player.statLife = Player.statLifeMax2 / 25;
				Player.AddBuff(BuffType<Reincarnated>(), 1800 - undyingFactor * 30);
				SoundEngine.PlaySound(SoundID.DD2_BetsySummon, Player.Center);
				SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, Player.Center);
				for (int j = 0; j < 800; j++) {
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
				return false;
			}
			return true;
		}
	}
}