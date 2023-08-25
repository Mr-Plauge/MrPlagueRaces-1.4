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

namespace MrPlagueRaces.Common.Races.Tabaxi
{
	public class Tabaxi : Race
	{
		public override void Load()
        {
			Description = "Nomadic and agile, Tabaxi are motivated by ambition and curiosity.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Hold Z to charge up an intangible dash.\n[c/4DBF60:{"+"}] Press X to set a rewind point, press C to return to it.";
			ClothStyle = 4;
			CensorClothing = false;
			StarterShirt = true;
			StarterPants = true;
			AlwaysDrawHair = true;
			HairColor = new Color(255, 247, 200);
			SkinColor = new Color(237, 208, 165);
			DetailColor = new Color(239, 119, 157);
			EyeColor = new Color(150, 255, 194);
			ShirtColor = new Color(180, 112, 101);
			UnderShirtColor = new Color(108, 74, 61);
			PantsColor = new Color(245, 213, 193);
			ShoeColor = new Color(180, 112, 101);
		}

		public override void ResetEffects(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				player.moveSpeed += 0.15f;
				player.jumpSpeedBoost += 0.1f;
				player.pickSpeed -= 0.15f;
				player.GetCritChance(DamageClass.Generic) -= 15;
				player.endurance -= 0.1f;
				player.noFallDmg = true;
			}
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var tabaxiPlayer = player.GetModPlayer<TabaxiPlayer>();
			tabaxiPlayer.phased = false;
			tabaxiPlayer.phaseChargeCounter = 0;
			tabaxiPlayer.phaseActiveCounter = 0;
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var tabaxiPlayer = player.GetModPlayer<TabaxiPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
				{
					if (MrPlagueRaces.RaceAbilityKeybind1.Current && !player.HasBuff(BuffType<ParticleDeacceleration>()))
					{
						tabaxiPlayer.phased = true;
						if (tabaxiPlayer.phaseChargeCounter == 0) {
							SoundEngine.PlaySound(SoundID.Item162, player.Center);
						}
						if (tabaxiPlayer.phaseChargeCounter < 100) {
							tabaxiPlayer.phaseChargeCounter++;
						}
					}
					else {
						tabaxiPlayer.phased = false;
					}
					if (MrPlagueRaces.RaceAbilityKeybind1.JustReleased && !player.HasBuff(BuffType<ParticleDeacceleration>())) {
						player.AddBuff(BuffType<ParticleDeacceleration>(), tabaxiPlayer.phaseChargeCounter * 2);
						SoundEngine.PlaySound(SoundID.Item163, player.Center);
					}
					if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed)
					{
						if (player.position != tabaxiPlayer.TabaxiSpawn)
						{
							Main.NewText("Rewind point set!", 143, 255, 255);
							for (int i = 0; i < 25; i++)
							{
								int dust = Dust.NewDust(player.position, player.width, player.height, 180);
								Main.dust[dust].noGravity = true;
								Dust obj6 = Main.dust[dust];
								obj6.velocity *= 0.75f;
							}
							tabaxiPlayer.TabaxiSpawn = player.position;
							SoundEngine.PlaySound(SoundID.Item105, player.Center);
						}
						else
						{
							Main.NewText("Rewind point removed!", 143, 255, 255);
							tabaxiPlayer.TabaxiSpawn = new Vector2(-1, -1);
						}
					}
					if (MrPlagueRaces.RaceAbilityKeybind3.JustPressed)
					{
						if (tabaxiPlayer.TabaxiSpawn != new Vector2(-1, -1) && !player.HasBuff(BuffType<Rematerializing>()))
						{
							player.Teleport(tabaxiPlayer.TabaxiSpawn, 3);
							player.AddBuff(BuffType<Rematerializing>(), 180);
							SoundEngine.PlaySound(SoundID.Item165, player.Center);
						}
						else if (!player.HasBuff(BuffType<Rematerializing>()))
						{
							Main.NewText("No rewind point found!", 143, 255, 255);
						}
					}
					if (tabaxiPlayer.phaseActiveCounter > 0) {
						if (player.controlUp) {
							player.controlUp = false;
						}
						if (player.controlDown) {
							player.controlDown = false;
						}
						if (player.controlJump) {
							player.controlJump = false;
						}
						if (player.controlLeft) {
							player.controlLeft = false;
						}
						if (player.controlRight) {
							player.controlRight = false;
						}
					}
				}
			}
		}

		public override void PreUpdate(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var tabaxiPlayer = player.GetModPlayer<TabaxiPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead) {
					if (!tabaxiPlayer.phased && tabaxiPlayer.phaseChargeCounter > 0) {
						player.direction = Main.MouseWorld.X >= player.Center.X ? 1 : -1;
						Vector2 offset = Main.MouseWorld - player.Center;
						Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * (tabaxiPlayer.phaseChargeCounter);
						player.velocity = velocity;
						player.fallStart = (int)(player.position.Y / 16f);
						tabaxiPlayer.phaseActiveCounter = 30;
						tabaxiPlayer.phaseChargeCounter = 0;
					}
					if (tabaxiPlayer.phaseActiveCounter > 0) {
						player.ghost = true;
						tabaxiPlayer.phaseActiveCounter--;
					}
					else {
						player.ghost = false;
					}
				}
				if (tabaxiPlayer.phaseActiveCounter > 0) {
					for (int i = 0; i < 25; i++)
					{
						int dust = Dust.NewDust(new Vector2(player.position.X + Main.rand.Next(10) - Main.rand.Next(10), player.position.Y + Main.rand.Next(10) - Main.rand.Next(10)), player.width + Main.rand.Next(10) - Main.rand.Next(10), player.width + Main.rand.Next(10) - Main.rand.Next(10), 180);
						Main.dust[dust].noGravity = true;
						Dust obj6 = Main.dust[dust];
						obj6.velocity *= 0.75f;
					}
				}
				if (tabaxiPlayer.phased) {
					for (int i = 0; i < tabaxiPlayer.phaseChargeCounter / 2; i++)
					{
						if (Main.rand.Next(40) == 1) {
							int dust = Dust.NewDust(player.position, player.width, player.height, 180);
							Main.dust[dust].noGravity = true;
							Dust obj6 = Main.dust[dust];
							obj6.velocity *= 0.75f;
						}
					}
				}
			}
		}
	}

	public class TabaxiPlayer : ModPlayer
	{
		public Vector2 TabaxiSpawn = new Vector2(-1, -1);
		public bool phased;
		public int phaseChargeCounter;
		public int phaseActiveCounter;

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.TabaxiSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(TabaxiSpawn.X);
			packet.Write(TabaxiSpawn.Y);
			packet.Write(phased);
			packet.Write(phaseChargeCounter);
			packet.Write(phaseActiveCounter);
		}
	}
}