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

namespace MrPlagueRaces.Common.Races.Skeleton
{
	public class Skeleton : Race
	{
		public override void Load()
        {
			Description = "Reborn through a variety of rituals, most Skeletons go insane upon reanimation.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z, X, and C to switch between bodies.\n[c/4DBF60:{"+"}] When you die, your skeleton releases its spirit. If you can survive without being hit for long enough, you reform a new skeleton and return to life.";
			ClothStyle = 3;
			CensorClothing = false;
			StarterShirt = true;
			StarterPants = true;
			AlwaysDrawHair = true;
			HairColor = new Color(237, 208, 165);
			SkinColor = new Color(237, 208, 165);
			DetailColor = new Color(237, 208, 165);
			EyeColor = new Color(255, 91, 119);
			ShirtColor = new Color(203, 177, 155);
			UnderShirtColor = new Color(210, 111, 111);
			ShoeColor = new Color(146, 119, 97);
		}

		public override void ResetEffects(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				player.moveSpeed += 0.15f;
				player.GetDamage(DamageClass.Generic) += 0.1f;
				player.endurance -= 0.5f;
				player.gills = true;
			}
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
			skeletonPlayer.teleportOne = false;
			skeletonPlayer.teleportTwo = false;
			skeletonPlayer.teleportThree = false;
			skeletonPlayer.spirit = 0;
			skeletonPlayer.currentBody = 1;
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				if (!player.dead)
				{
					if (skeletonPlayer.spirit == 0 && !player.HasBuff(BuffType<Bodyswapped>())) {
						if (MrPlagueRaces.RaceAbilityKeybind1.JustPressed)
						{
							if (skeletonPlayer.currentBody != 1) {
								if (player.ownedProjectileCounts[ProjectileType<Skeleton_One>()] == 0) {
									Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.position.X, player.position.Y + 30, 0f, 0f, ProjectileType<Skeleton_One>(), 0, 0, player.whoAmI);
									SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, player.Center);
									for (int i = 0; i < 6; i++) {
										Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y,  Main.rand.Next(3) - Main.rand.Next(3),  Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, player.whoAmI);
									}
								}
								else {
									DropBody(player);
									skeletonPlayer.teleportOne = true;
									skeletonPlayer.currentBody = 1;
								}
							}
						}
						if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed)
						{
							if (skeletonPlayer.currentBody != 2) {
								if (player.ownedProjectileCounts[ProjectileType<Skeleton_Two>()] == 0) {
									Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.position.X, player.position.Y + 30, 0f, 0f, ProjectileType<Skeleton_Two>(), 0, 0, player.whoAmI);
									SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, player.Center);
									for (int i = 0; i < 6; i++) {
										Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y,  Main.rand.Next(3) - Main.rand.Next(3),  Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, player.whoAmI);
									}
								}
								else {
									DropBody(player);
									skeletonPlayer.teleportTwo = true;
									skeletonPlayer.currentBody = 2;
								}
							}
						}
						if (MrPlagueRaces.RaceAbilityKeybind3.JustPressed)
						{
							if (skeletonPlayer.currentBody != 3) {
								if (player.ownedProjectileCounts[ProjectileType<Skeleton_Three>()] == 0) {
									Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.position.X, player.position.Y + 30, 0f, 0f, ProjectileType<Skeleton_Three>(), 0, 0, player.whoAmI);
									SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, player.Center);
									for (int i = 0; i < 6; i++) {
										Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y,  Main.rand.Next(3) - Main.rand.Next(3),  Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, player.whoAmI);
									}
								}
								else {
									DropBody(player);
									skeletonPlayer.teleportThree = true;
									skeletonPlayer.currentBody = 3;
								}
							}
						}
					}
					if (skeletonPlayer.spirit > 0) {
						if (player.controlUp) {
							player.velocity.Y -= 0.5f;
							player.controlUp = false;
						}
						if (player.controlDown) {
							player.velocity.Y += 0.5f;
							player.controlDown = false;
						}
						if (player.controlJump) {
							player.velocity.Y -= 0.5f;
							player.controlJump = false;
						}
						if (player.controlLeft) {
							player.velocity.X -= 0.5f;
							player.controlLeft = false;
						}
						if (player.controlRight) {
							player.velocity.X += 0.5f;
							player.controlRight = false;
						}
					}
				}
			}
		}

		public override bool PreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource) {
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				if (!player.HasBuff(BuffType<Reanimated>()) && skeletonPlayer.spirit == 0) {
					skeletonPlayer.spirit = 600;
					if (player.ownedProjectileCounts[ProjectileType<Spirit>()] == 0) {
						Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, 0, 0, ProjectileType<Spirit>(), 1, 0, player.whoAmI);
					}
					for (int num884 = 0; num884 < 25; num884++)
					{
						int num885 = Dust.NewDust(player.position, player.width, player.height, 261, player.velocity.X, player.velocity.Y);
						Dust dust210 = Main.dust[num885];
						Dust dust218 = dust210;
						dust218.velocity *= 2f;
						Main.dust[num885].noGravity = true;
						Main.dust[num885].scale = 1.4f;
						Main.dust[num885].color = player.eyeColor;
					}
					for (int i = 0; i < 6; i++) {
						Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y,  Main.rand.Next(3) - Main.rand.Next(3),  Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, player.whoAmI);
					}
					SoundEngine.PlaySound(SoundID.Zombie53, player.Center);
					SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, player.Center);
					player.statLife = player.statLifeMax2 / 2;
					return false;
				}
				else {
					return true;
				}
			}
			else {
				return true;
			}
		}

		public override void PreUpdate(Player player) {
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				if (!player.dead) {
					player.breath = player.breathMax + 100;
					player.buffImmune[BuffID.Bleeding] = true;
					player.buffImmune[BuffID.Poisoned] = true;
					player.buffImmune[BuffID.Venom] = true;
					player.buffImmune[BuffID.Rabies] = true;
					player.buffImmune[BuffID.Frostburn] = true;
					player.buffImmune[BuffID.Suffocation] = true;
					player.ClearBuff(BuffID.Bleeding);
					player.ClearBuff(BuffID.Poisoned);
					player.ClearBuff(BuffID.Venom);
					player.ClearBuff(BuffID.Rabies);
					player.ClearBuff(BuffID.Frostburn);
					player.ClearBuff(BuffID.Suffocation);
					if (skeletonPlayer.spirit > 0) {
						player.ghost = true;
						skeletonPlayer.spirit--;
					}
					else {
						if (player.ghost) {
							for (int num884 = 0; num884 < 25; num884++)
							{
								int num885 = Dust.NewDust(player.position, player.width, player.height, 261, player.velocity.X, player.velocity.Y);
								Dust dust210 = Main.dust[num885];
								Dust dust218 = dust210;
								dust218.velocity *= 2f;
								Main.dust[num885].noGravity = true;
								Main.dust[num885].scale = 1.4f;
								Main.dust[num885].color = player.eyeColor;
							}
							for (int i = 0; i < 6; i++) {
								Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y,  Main.rand.Next(3) - Main.rand.Next(3),  Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, player.whoAmI);
							}
							SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, player.Center);
							player.ghost = false;
							player.AddBuff(BuffType<Reanimated>(), 900);
							player.AddBuff(BuffType<Bodyswapped>(), 320);
						}
					}
				}
			}
		}
		
		public void DropBody(Player player) {
			var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
			if (player.ownedProjectileCounts[ProjectileType<Skeleton_One>()] == 0 && skeletonPlayer.currentBody == 1) {
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.position.X, player.position.Y + 30, 0f, 0f, ProjectileType<Skeleton_One>(), 0, 0, player.whoAmI);
			}
			if (player.ownedProjectileCounts[ProjectileType<Skeleton_Two>()] == 0 && skeletonPlayer.currentBody == 2) {
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.position.X, player.position.Y + 30, 0f, 0f, ProjectileType<Skeleton_Two>(), 0, 0, player.whoAmI);
			}
			if (player.ownedProjectileCounts[ProjectileType<Skeleton_Three>()] == 0 && skeletonPlayer.currentBody == 3) {
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.position.X, player.position.Y + 30, 0f, 0f, ProjectileType<Skeleton_Three>(), 0, 0, player.whoAmI);
			}
			player.AddBuff(BuffType<Bodyswapped>(), 60);
		}
	}

	public class SkeletonPlayer : ModPlayer
	{
		public bool teleportOne;
		public bool teleportTwo;
		public bool teleportThree;
		public int spirit;
		public int currentBody = 1;

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.SkeletonSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(teleportOne);
			packet.Write(teleportTwo);
			packet.Write(teleportThree);
			packet.Write(spirit);
			packet.Write(currentBody);
		}
	}
}
