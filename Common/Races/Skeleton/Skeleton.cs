using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Config;
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
            DisplayName = "[c/E3C7AC:Skeleton]";
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

        public override void PreRaceChange(Player player) // Called before the player's race is changed
        {
            var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
            skeletonPlayer.teleportOne = false;
            skeletonPlayer.teleportTwo = false;
            skeletonPlayer.teleportThree = false;
            skeletonPlayer.spirit = 0;
            skeletonPlayer.currentBody = 1;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && (projectile.type == ProjectileType<Spirit>() || projectile.type == ProjectileType<Skeleton_One>() || projectile.type == ProjectileType<Skeleton_Two>() || projectile.type == ProjectileType<Skeleton_Three>()) && projectile.owner == player.whoAmI)
                    projectile.Kill();
            }
        }

        public override void ResetEffects(Player player)
        {
            RegisterAbilityDescription(ModContent.GetInstance<SkeletonConfig>().skeletonAbility1, $"[c/4DBF60:+] Press Z, X, and C to switch between bodies.");
            RegisterAbilityDescription(ModContent.GetInstance<SkeletonConfig>().skeletonAbility1, $"[c/4DBF60:+] Press V to desummon all existing bodies.");
            RegisterAbilityDescription(ModContent.GetInstance<SkeletonConfig>().skeletonGhost && ModContent.GetInstance<SkeletonConfig>().skeletonIntangibility != IntangibilityProgressionTiers.Disabled, $"[c/4DBF60:+] When you die, your body releases its intangible spirit. If you can survive without being hit for long enough, you reform and return to life.");
            RegisterAbilityDescription(ModContent.GetInstance<SkeletonConfig>().skeletonGhost && ModContent.GetInstance<SkeletonConfig>().skeletonIntangibility == IntangibilityProgressionTiers.Disabled, $"[c/4DBF60:+] When you die, your body releases its spirit. If you can survive without being hit for long enough, you reform and return to life.");
            RegisterAbilityDescription(ModContent.GetInstance<SkeletonConfig>().skeletonGills, $"[c/4DBF60:+] You do not need to breathe underwater.");
            RegisterAbilityDescription(ModContent.GetInstance<SkeletonConfig>().skeletonDebuffImmunity, $"[c/4DBF60:+] You are immune to poison, burning, bleeding, and suffocation.");

            RaceStatDictionary = ModContent.GetInstance<SkeletonConfig>().skeletonStats;
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
            if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				/*player.moveSpeed += 0.15f;
				player.GetDamage(DamageClass.Generic) += 0.1f;
				player.endurance -= 0.5f;*/
				if (ModContent.GetInstance<SkeletonConfig>().skeletonGills)
				{
					player.gills = true;
				}
                if (skeletonPlayer.spirit > 0)
                {
                    player.statLifeMax2 = 0;
                    if (skeletonPlayer.meetsIntangibilityRequirement(ModContent.GetInstance<SkeletonConfig>().skeletonIntangibility))
                    {
                        player.shimmering = true;
                        player.ClearBuff(BuffID.Shimmer);
                    }
                    if (player.shimmerTransparency < 1)
                    {
                        player.shimmerTransparency += 0.25f;
                    }
                }
                else if (!player.HasBuff(BuffID.Shimmer))
                {
                    if (player.shimmerTransparency > 0)
                    {
                        player.shimmerTransparency -= 0.25f;
                    }
                }
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
            if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
				{
					if (skeletonPlayer.spirit == 0 && !player.HasBuff(BuffType<Bodyswapped>())) {
						if (ModContent.GetInstance<SkeletonConfig>().skeletonAbility1)
						{
							if (MrPlagueRaces.RaceAbilityKeybind1.JustPressed)
							{
								if (skeletonPlayer.currentBody != 1)
								{
									if (player.ownedProjectileCounts[ProjectileType<Skeleton_One>()] == 0)
									{
										Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.position.X, player.position.Y + 30, 0f, ProjectileType<Skeleton_One>(), 0, 0, player.whoAmI);
										SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, player.Center);
                                        if (player.whoAmI == Main.myPlayer)
                                        {
                                            mrPlagueRacesPlayer.SkeletonSummonSound(-1, Main.myPlayer);
                                        }
                                        for (int i = 0; i < 6; i++)
										{
											Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, Main.rand.Next(3) - Main.rand.Next(3), Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, player.whoAmI);
										}
									}
									else
									{
										DropBody(player);
										skeletonPlayer.teleportOne = true;
										skeletonPlayer.currentBody = 1;
									}
								}
							}
							if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed)
							{
								if (skeletonPlayer.currentBody != 2)
								{
									if (player.ownedProjectileCounts[ProjectileType<Skeleton_Two>()] == 0)
									{
										Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.position.X, player.position.Y + 30, 0f, 0f, ProjectileType<Skeleton_Two>(), 0, 0, player.whoAmI);
										SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, player.Center);
                                        if (player.whoAmI == Main.myPlayer)
                                        {
                                            mrPlagueRacesPlayer.SkeletonSummonSound(-1, Main.myPlayer);
                                        }
                                        for (int i = 0; i < 6; i++)
										{
											Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, Main.rand.Next(3) - Main.rand.Next(3), Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, player.whoAmI);
										}
									}
									else
									{
										DropBody(player);
										skeletonPlayer.teleportTwo = true;
										skeletonPlayer.currentBody = 2;
									}
								}
							}
							if (MrPlagueRaces.RaceAbilityKeybind3.JustPressed)
							{
								if (skeletonPlayer.currentBody != 3)
								{
									if (player.ownedProjectileCounts[ProjectileType<Skeleton_Three>()] == 0)
									{
										Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.position.X, player.position.Y + 30, 0f, 0f, ProjectileType<Skeleton_Three>(), 0, 0, player.whoAmI);
										SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, player.Center);
                                        if (player.whoAmI == Main.myPlayer)
                                        {
                                            mrPlagueRacesPlayer.SkeletonSummonSound(-1, Main.myPlayer);
                                        }
                                        for (int i = 0; i < 6; i++)
										{
											Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, Main.rand.Next(3) - Main.rand.Next(3), Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, player.whoAmI);
										}
									}
									else
									{
										DropBody(player);
										skeletonPlayer.teleportThree = true;
										skeletonPlayer.currentBody = 3;
									}
								}
							}
						}
                    }
					if (ModContent.GetInstance<SkeletonConfig>().skeletonAbility1)
					{
						if (MrPlagueRaces.RaceAbilityKeybind4.JustPressed)
						{
							for (int i = 0; i < Main.maxProjectiles; i++)
							{
								skeletonPlayer.teleportOne = false;
								skeletonPlayer.teleportTwo = false;
								skeletonPlayer.teleportThree = false;
								skeletonPlayer.currentBody = 1;
								Projectile projectile = Main.projectile[i];
								if (projectile.active && (projectile.type == ProjectileType<Skeleton_One>() || projectile.type == ProjectileType<Skeleton_Two>() || projectile.type == ProjectileType<Skeleton_Three>()) && projectile.owner == player.whoAmI)
									projectile.Kill();
							}
						}
					}

                    // {T} Important gameplay logic that isn't input-dependent has been taken out of here since it won't run while the window isn't in focus!
                    // The code is now in PreUpdate.
                    if (skeletonPlayer.spirit > 0) {
                        if (!player.controlLeft && !player.controlRight)
                        {
                            skeletonPlayer.targetVelocityX = 0f;
                        }
                        if (!player.controlUp && !player.controlDown && !player.controlJump)
                        {
                            skeletonPlayer.targetVelocityY = 0f;
                        }
                        if (player.controlUp)
                        {
                            skeletonPlayer.targetVelocityY = -15f * (skeletonPlayer.meetsIntangibilityRequirement(ModContent.GetInstance<SkeletonConfig>().skeletonIntangibility) ? 1f : 1f);
                            player.controlUp = false;
                        }
                        if (player.controlDown)
                        {
                            skeletonPlayer.targetVelocityY = 15f * (skeletonPlayer.meetsIntangibilityRequirement(ModContent.GetInstance<SkeletonConfig>().skeletonIntangibility) ? 1f : 1f);
                            player.controlDown = false;
                        }
                        if (player.controlJump)
                        {
                            skeletonPlayer.targetVelocityY = -15f * (skeletonPlayer.meetsIntangibilityRequirement(ModContent.GetInstance<SkeletonConfig>().skeletonIntangibility) ? 1f : 1f);
                            player.controlJump = false;
                        }
                        if (player.controlLeft)
                        {
                            skeletonPlayer.targetVelocityX = -15f * (skeletonPlayer.meetsIntangibilityRequirement(ModContent.GetInstance<SkeletonConfig>().skeletonIntangibility) ? 1f : 1f);
                            player.controlLeft = false;
                        }
                        if (player.controlRight)
                        {
                            skeletonPlayer.targetVelocityX = 15f * (skeletonPlayer.meetsIntangibilityRequirement(ModContent.GetInstance<SkeletonConfig>().skeletonIntangibility) ? 1f : 1f);
                            player.controlRight = false;
                        }
                        player.controlUseItem = false;
                    }
				}
			}
		}

		public override bool PreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource) {
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats && ModContent.GetInstance<SkeletonConfig>().skeletonGhost) {
				if (!player.HasBuff(BuffType<Reanimated>()) && skeletonPlayer.spirit == 0) {
					skeletonPlayer.spirit = 600;
					skeletonPlayer.lastUnobstructedPosition = player.position;
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

        public override bool FreeDodge(Player player, Player.HurtInfo info)
        {
            var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
            if (skeletonPlayer.spirit > 0)
            {
                return true;
            }
            return false;
        }

        public override void PreUpdate(Player player) {
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
				{
                    if (ModContent.GetInstance<SkeletonConfig>().skeletonGills)
                    {
                        player.breath = player.breathMax + 100;
                    }
                    if (ModContent.GetInstance<SkeletonConfig>().skeletonDebuffImmunity)
                    {
                        player.buffImmune[BuffID.Bleeding] = true;
                        player.buffImmune[BuffID.Poisoned] = true;
                        player.buffImmune[BuffID.Venom] = true;
                        player.buffImmune[BuffID.Rabies] = true;
                        player.buffImmune[BuffID.Frostburn] = true;
                        player.buffImmune[BuffID.Suffocation] = true;
                        player.buffImmune[BuffID.Burning] = true;
                        player.buffImmune[BuffID.OnFire] = true;
                        player.ClearBuff(BuffID.Bleeding);
                        player.ClearBuff(BuffID.Poisoned);
                        player.ClearBuff(BuffID.Venom);
                        player.ClearBuff(BuffID.Rabies);
                        player.ClearBuff(BuffID.Frostburn);
                        player.ClearBuff(BuffID.Suffocation);
                        player.ClearBuff(BuffID.Burning);
                        player.ClearBuff(BuffID.OnFire);
                    }
                    if (skeletonPlayer.spirit > 0)
					{
                        // {T} Moved some code out of ProcessTriggers so that it can properly run while the game is not in focus.
                        if (player.shimmering && !skeletonPlayer.isInBlocks())
                        {
                            skeletonPlayer.lastUnobstructedPosition = player.position;
                        }
                        if (skeletonPlayer.velocityX > skeletonPlayer.targetVelocityX)
                        {
                            skeletonPlayer.velocityX -= 0.5f * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<SkeletonConfig>().skeletonGhostVelocity]);
                        }
                        if (skeletonPlayer.velocityX < skeletonPlayer.targetVelocityX)
                        {
                            skeletonPlayer.velocityX += 0.5f * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<SkeletonConfig>().skeletonGhostVelocity]);
                        }
                        if (skeletonPlayer.velocityY > skeletonPlayer.targetVelocityY)
                        {
                            skeletonPlayer.velocityY -= 0.5f * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<SkeletonConfig>().skeletonGhostVelocity]);
                        }
                        if (skeletonPlayer.velocityY < skeletonPlayer.targetVelocityY)
                        {
                            skeletonPlayer.velocityY += 0.5f * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<SkeletonConfig>().skeletonGhostVelocity]);
                        }
                        player.maxFallSpeed = 1000000f;
                        player.gravity = 0f;
                        player.velocity.X = skeletonPlayer.velocityX * (skeletonPlayer.meetsIntangibilityRequirement(ModContent.GetInstance<SkeletonConfig>().skeletonIntangibility) ? 1f : 0.4f);
                        player.velocity.Y = skeletonPlayer.velocityY * (skeletonPlayer.meetsIntangibilityRequirement(ModContent.GetInstance<SkeletonConfig>().skeletonIntangibility) ? 1f : 0.4f);

                        if (skeletonPlayer.spirit == 1)
                        {
                            player.velocity *= 0.45f;
                            player.AddBuff(BuffType<Reanimated>(), 900);
                            player.AddBuff(BuffType<Bodyswapped>(), 320);
                            if (skeletonPlayer.isInBlocks())
                            {
                                if (ModContent.GetInstance<SkeletonConfig>().skeletonStuckPrevention == StuckPreventionTiers.Low || ModContent.GetInstance<SkeletonConfig>().skeletonStuckPrevention == StuckPreventionTiers.High)
                                {
									skeletonPlayer.SeekNearestAirPocket();
                                    player.velocity *= 0f;
                                }
                                if (ModContent.GetInstance<SkeletonConfig>().skeletonStuckPrevention == StuckPreventionTiers.High && skeletonPlayer.isInBlocks())
                                {
                                    Vector2 tileAlignedPosition = new Vector2(skeletonPlayer.lastUnobstructedPosition.X / 16, skeletonPlayer.lastUnobstructedPosition.Y / 16) * 16;
                                    player.Teleport(new Vector2(tileAlignedPosition.X, tileAlignedPosition.Y - 2), -1);
                                    player.velocity *= 0f;
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
                            }
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                Projectile projectile = Main.projectile[i];
                                if (projectile.active && projectile.type == ProjectileType<Spirit>() && projectile.owner == player.whoAmI)
                                    projectile.Kill();
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
							for (int i = 0; i < 6; i++)
							{
								Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, Main.rand.Next(3) - Main.rand.Next(3), Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<PuffDust>(), 0, 0, player.whoAmI);
							}
							SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, player.Center);
                            if (player.whoAmI == Main.myPlayer)
                            {
                                mrPlagueRacesPlayer.SkeletonSummonSound(-1, Main.myPlayer);
                            }
                        }
						skeletonPlayer.spirit--;
					}
                    if (skeletonPlayer.isInTempleEarly() && player.HasBuff(BuffType<Reanimated>()) && ModContent.GetInstance<SkeletonConfig>().skeletonNoEarlyTemple)
                    {
                        player.statLife = 0;
                        switch (Main.rand.Next(2))
                        {
                            case 0:
                                player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " tried to phase through impenetrable material."), 10.0, 0, false);
                                break;
                            case 1:
                                player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " still needs to defeat Plantera."), 10.0, 0, false);
                                break;
                            default:
                                player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " left organs behind in the temple wall."), 10.0, 0, false);
                                break;
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

        public override void PostUpdate(Player player)
        {
            var skeletonPlayer = player.GetModPlayer<SkeletonPlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    skeletonPlayer.SyncPlayer(-1, Main.myPlayer, false);
                }

                var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
                mrPlagueRacesPlayer.SyncRotationsAndOffsets(-1, Main.myPlayer);
            }
        }
    }

    public enum StuckPreventionTiers
    {
        Off,
        Low,
        High
    }

    public enum IntangibilityProgressionTiers
    {
        Enabled,
        PostEOC,
        PostEvilBoss,
        PostSkeletron,
        Hardmode,
        PostMechBosses,
        PostPlantera,
        PostGolem,
        PostMoonLord,
        Disabled
    }

    public class SkeletonPlayer : ModPlayer
	{
		public bool teleportOne;
		public bool teleportTwo;
		public bool teleportThree;
        public int spirit;
		public int currentBody = 1;
        public float velocityX = 0f;
        public float velocityY = 0f;
        public float targetVelocityX = 0f;
        public float targetVelocityY = 0f;
        public Vector2 lastUnobstructedPosition = new Vector2(-1, -1);

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
			packet.Write(velocityX);
			packet.Write(velocityY);
			packet.Write(targetVelocityX);
			packet.Write(targetVelocityY);
            packet.Write(lastUnobstructedPosition.X);
            packet.Write(lastUnobstructedPosition.Y);

            packet.Send(toWho, fromWho);
        }

        public bool meetsIntangibilityRequirement(IntangibilityProgressionTiers intangibilityCriteria)
        {
            if (intangibilityCriteria == IntangibilityProgressionTiers.Enabled)
            {
                return true;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostEOC)
            {
                return NPC.downedBoss1;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostEvilBoss)
            {
                return NPC.downedBoss2;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostSkeletron)
            {
                return NPC.downedBoss3;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.Hardmode)
            {
                return Main.hardMode;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostMechBosses)
            {
                return NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostPlantera)
            {
                return NPC.downedPlantBoss;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostGolem)
            {
                return NPC.downedGolemBoss;
            }
            else if (intangibilityCriteria == IntangibilityProgressionTiers.PostMoonLord)
            {
                return NPC.downedMoonlord;
            }
            else
            {
                return false;
            }
        }

        public bool isInTempleEarly()
        {
            Tile[] wallTiles = new Tile[6];
            Point playerTilePoint = (Main.LocalPlayer.position / 16).ToPoint();
            wallTiles[0] = Framing.GetTileSafely(playerTilePoint.X, playerTilePoint.Y);
            wallTiles[1] = Framing.GetTileSafely(playerTilePoint.X, playerTilePoint.Y + 1);
            wallTiles[2] = Framing.GetTileSafely(playerTilePoint.X, playerTilePoint.Y + 2);
            wallTiles[3] = Framing.GetTileSafely(playerTilePoint.X + 1, playerTilePoint.Y);
            wallTiles[4] = Framing.GetTileSafely(playerTilePoint.X + 1, playerTilePoint.Y + 1);
            wallTiles[5] = Framing.GetTileSafely(playerTilePoint.X + 1, playerTilePoint.Y + 2);
            bool behindTempleWall = false;
            foreach (var tile in wallTiles)
            {
                if (tile.WallType == 87)
                {
                    behindTempleWall = true;
                    break;
                }
            }
            return behindTempleWall && !NPC.downedPlantBoss;
        }

        public bool isInBlocks()
        {
            int inBlocks = 0;
            Vector2 playerLocation = new Vector2(Player.position.X / 16, Player.position.Y / 16);
            for (int i = 0; i < 3; i++)
            {
                Tile myTile1 = Main.tile[(int)playerLocation.X + 1, (int)playerLocation.Y];
                Tile myTile2 = Main.tile[(int)playerLocation.X, (int)playerLocation.Y];
                if (myTile1 != null && Main.tileSolid[myTile1.TileType] && myTile1.HasUnactuatedTile)
                {
                    inBlocks++;
                }
                if (myTile2 != null && Main.tileSolid[myTile2.TileType] && myTile2.HasUnactuatedTile)
                {
                    inBlocks++;
                }
                if (playerLocation.Y > 0)
                {
                    playerLocation.Y -= 1;
                }
            }
            return inBlocks > 0;
        }

        public bool isPositionObstructed(Vector2 checkPosition)
        {
            int inBlocks = 0;
            Vector2 playerLocation = checkPosition;
            for (int i = 0; i < 3; i++)
            {
                Tile myTile1 = Main.tile[(int)playerLocation.X + 1, (int)playerLocation.Y];
                Tile myTile2 = Main.tile[(int)playerLocation.X, (int)playerLocation.Y];
                if (myTile1 != null && Main.tileSolid[myTile1.TileType] && myTile1.HasUnactuatedTile)
                {
                    inBlocks++;
                }
                if (myTile2 != null && Main.tileSolid[myTile2.TileType] && myTile2.HasUnactuatedTile)
                {
                    inBlocks++;
                }
                if (playerLocation.Y > 0)
                {
                    playerLocation.Y -= 1;
                }
            }
            return inBlocks > 0;
        }

        public void SeekNearestAirPocket()
        {
            Vector2 playerLocation = new Vector2(Player.position.X / 16, Player.position.Y / 16);
            int SeekRange = 50;
            Vector2 closestPosition = new Vector2(playerLocation.X - SeekRange, playerLocation.Y - SeekRange);
            for (int i = -SeekRange; i < SeekRange; i++)
            {
                for (int j = -SeekRange; j < SeekRange; j++)
                {
                    Vector2 currentPosition = new Vector2((int)playerLocation.X + i, (int)playerLocation.Y + j);
                    if (Vector2.Distance(currentPosition, playerLocation) < Vector2.Distance(closestPosition, playerLocation))
                    {
                        if (!isPositionObstructed(currentPosition))
                        {
                            closestPosition = currentPosition;
                        }
                    }
                }
            }
            if (!isPositionObstructed(closestPosition))
            {
                Player.Teleport(new Vector2(closestPosition.X * 16, (closestPosition.Y - 2) * 16), -1);
            }
        }
    }

    public class SkeletonConfig : ModConfig
    {
        public static SkeletonConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Skeleton")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> skeletonStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%
            [RacialStatType.allDamage] = RacialStatPercentageModifier.p10,
            [RacialStatType.moveSpeed] = RacialStatPercentageModifier.p15,
            [RacialStatType.endurance] = RacialStatPercentageModifier.n50
        };
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool skeletonAbility1;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool skeletonGhost;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier skeletonGhostVelocity;
        [DefaultValue(IntangibilityProgressionTiers.Enabled)]
        [BackgroundColor(110, 141, 255)]
        public IntangibilityProgressionTiers skeletonIntangibility;
        [DefaultValue(StuckPreventionTiers.High)]
        [BackgroundColor(110, 141, 255)]
        public StuckPreventionTiers skeletonStuckPrevention;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool skeletonNoEarlyTemple;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool skeletonGills;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool skeletonDebuffImmunity;
    }
}