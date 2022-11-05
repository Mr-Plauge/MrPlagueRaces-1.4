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
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.Races.Mushfolk
{
	public class Mushfolk : Race
	{
		public override void Load()
        {
			Description = "Luminous and mysterious, the Mushfolk feed on health to fuel their healing abilities.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Hold Z to release trapshroom spores, which release healing clouds upon contact with an enemy. Healing scales with the individual's max health.\n[c/4DBF60:{"+"}] Press X to create a Nymphshroom, which teleports you to it upon contact with an enemy.";
			CensorClothing = false;
			HairColor = new Color(138, 159, 255);
			SkinColor = new Color(239, 222, 202);
			DetailColor = new Color(239, 222, 202);
			EyeColor = new Color(138, 159, 255);
		}

		public override void ResetEffects(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				player.statLifeMax2 += (player.statLifeMax2 / 10);
				player.moveSpeed += 0.05f;
				player.GetDamage(DamageClass.Generic) -= 0.1f;
				player.endurance -= 0.25f;
			}
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var mushfolkPlayer = player.GetModPlayer<MushfolkPlayer>();
			mushfolkPlayer.growingMushrooms = false;
			mushfolkPlayer.sporeless = 0;
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var mushfolkPlayer = player.GetModPlayer<MushfolkPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				if (!player.dead)
				{
					if (!player.HasBuff(BuffType<Sporeless>())) {
						if (MrPlagueRaces.RaceAbilityKeybind1.Current)
						{
							mushfolkPlayer.growingMushrooms = true;
							if (mushfolkPlayer.sporeless < 180) {
								mushfolkPlayer.sporeless++;
							}
							player.eyeHelper.BlinkBecausePlayerGotHurt();
						}
						else
						{
							mushfolkPlayer.growingMushrooms = false;
						}
						if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed)
						{
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Main.MouseWorld.X, Main.MouseWorld.Y, 0f, 0f, ProjectileType<Nymphshroom>(), 1 + player.statLifeMax2 / 10, 0, player.whoAmI);
							mushfolkPlayer.sporeless += 30;
						}
					}
				}
			}
		}
		
		public override void PreUpdate(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var mushfolkPlayer = player.GetModPlayer<MushfolkPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				if (!player.dead) {
					for (int i = 0; i < mushfolkPlayer.sporeless; i++) {
						if (Main.rand.Next(80) == 1) {
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y,  Main.rand.Next(3) - Main.rand.Next(3),  Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<BurstSpores>(), 0, 0, player.whoAmI);
						}
					}
					if (!mushfolkPlayer.growingMushrooms && mushfolkPlayer.sporeless > 0) {
						mushfolkPlayer.sporeless--;
					}
					if (mushfolkPlayer.sporeless >= 180) {
						if (!player.HasBuff(BuffType<Sporeless>())) {
							SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, player.Center);
						}
						player.AddBuff(BuffType<Sporeless>(), 120);
						mushfolkPlayer.growingMushrooms = false;
					}
					if (mushfolkPlayer.growingMushrooms) {
						for (int i = 0; i < 3; i++) {
							if (Main.rand.Next(18) == 1) {
								Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Main.MouseWorld.X + Main.rand.Next(60) - Main.rand.Next(60), Main.MouseWorld.Y + Main.rand.Next(60) - Main.rand.Next(60), 0f, 0f, ProjectileType<Trapshroom>(), 1 + player.statLifeMax2 / 10, 0, player.whoAmI);
							}
						}
					}
				}
			}
		}
	}

	public class MushfolkPlayer : ModPlayer
	{
		public bool growingMushrooms = false;
		public int sporeless = 0;

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.MushfolkSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(growingMushrooms);
			packet.Write(sporeless);
		}
	}
}
