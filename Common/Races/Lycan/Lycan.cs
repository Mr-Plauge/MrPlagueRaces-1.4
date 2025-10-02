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
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace MrPlagueRaces.Common.Races.Lycan
{
	public class Lycan : Race
	{
		public override void Load()
        {
			Description = "Born from temporal magic, Lycans can rewind themselves into alternate timelines.";
            DisplayName = "[c/FF386D:Lycan]";
            CensorClothing = false;
			StarterShirt = true;
			HairColor = new Color(117, 144, 167);
			SkinColor = new Color(117, 144, 167);
			DetailColor = new Color(70, 82, 111);
            AuxilaryDetailColor1 = new Color(244, 245, 246);
            EyeColor = new Color(255, 61, 114);
			ShirtColor = new Color(198, 173, 158);
			UnderShirtColor = new Color(244, 109, 109);
        }

        public override void PreRaceChange(Player player) // Called before the player's race is changed
        {
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var rewindPlayer = player.GetModPlayer<RewindPlayer>();
            rewindPlayer.chargingRewind = false;
            rewindPlayer.chargingTimeline = false;
        }

        public override void ResetEffects(Player player)
        {
            RegisterAbilityDescription(ModContent.GetInstance<LycanConfig>().lycanAbility1 && !ModContent.GetInstance<LycanConfig>().lycanRewindNPCs, $"[c/4DBF60:+] Hold Z to record time. Let go to rewind time, rewinding yourself to your previous location. While rewinding, you are invincible.");
            RegisterAbilityDescription(ModContent.GetInstance<LycanConfig>().lycanAbility1 && ModContent.GetInstance<LycanConfig>().lycanRewindNPCs, $"[c/4DBF60:+] Hold Z to record time. Let go to rewind time, rewinding all nearby entities to their previous locations. While rewinding, you are invincible.");
            RegisterAbilityDescription(ModContent.GetInstance<LycanConfig>().lycanAbility2, $"[c/4DBF60:+] Hold X to replay your last rewound timeline. Let go to teleport to the current position in the replay.");

            RaceStatDictionary = ModContent.GetInstance<LycanConfig>().lycanStats;
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				/*player.GetDamage(DamageClass.Generic) += 0.15f;
				player.endurance += 0.1f;
				player.moveSpeed += 0.15f;
				player.jumpSpeedBoost += 0.05f;
				player.tileSpeed -= 0.1f;
				player.wallSpeed -= 0.1f;*/
			}
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var rewindPlayer = player.GetModPlayer<RewindPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				float sqrMaxDetectDistance = 600 * 600;
				bool nearTimeline = false;

				if (!player.dead)
				{
                    if (ModContent.GetInstance<LycanConfig>().lycanAbility1)
					{
                        // {T} Add syncing functions (and checks to not sync if the value is already correct).
                        if (MrPlagueRaces.RaceAbilityKeybind1.Current && !player.HasBuff(BuffType<TemporalRecoil>()) && rewindPlayer.timelineCounter == 0)
						{
							if (!rewindPlayer.chargingRewind)
							{
								rewindPlayer.chargingRewind = true;
                                rewindPlayer.SyncPlayer(-1, Main.myPlayer, false);
                            }
						}
						else
						{
                            if (rewindPlayer.chargingRewind)
                            {
                                rewindPlayer.chargingRewind = false;
                                rewindPlayer.SyncPlayer(-1, Main.myPlayer, false);
                            }
                        }
                    }
                    float sqrDistanceToTarget = Vector2.DistanceSquared(rewindPlayer.rewindPosition[0], player.Center);
                    if (sqrDistanceToTarget < sqrMaxDetectDistance || rewindPlayer.timelineCounter > 0)
                    {
                        nearTimeline = true;
                    }
                    if (ModContent.GetInstance<LycanConfig>().lycanAbility2)
					{
						if (nearTimeline)
						{
                            // {T} Add syncing functions (and checks to not sync if the value is already correct).
                            if (MrPlagueRaces.RaceAbilityKeybind2.Current && !player.HasBuff(BuffType<Rebounded>()) && rewindPlayer.rewindCounter == 0 && rewindPlayer.rewindPosition[0] != Vector2.Zero)
							{
                                if (!rewindPlayer.chargingTimeline)
                                {
                                    rewindPlayer.chargingTimeline = true;
                                    rewindPlayer.SyncPlayer(-1, Main.myPlayer, false);
                                }
                            }
							else
							{
                                if (rewindPlayer.chargingTimeline)
                                {
                                    rewindPlayer.chargingTimeline = false;
                                    rewindPlayer.SyncPlayer(-1, Main.myPlayer, false);
                                }
							}
						}
					}
				}
			}
        }

        public override bool FreeDodge(Player player, Player.HurtInfo info)
        {
            var rewindPlayer = player.GetModPlayer<RewindPlayer>();
            if (!rewindPlayer.chargingRewind && rewindPlayer.rewindCounter > 0)
            {
                return true;
            }
            return false;
        }
    }

	public class RewindPlayer : ModPlayer
	{
		public int rewindCounter = 0;
		public int timelineCounter = 0;
		public bool chargingRewind = false;
		public bool chargingTimeline = false;
		public bool initializedRewind;
		public int counter = 0;
		public Vector2[] rewindPosition = new Vector2[401];
		public int[] rewindDirection = new int[401];
		public Rectangle[] rewindHeadFrame = new Rectangle[401];
		public Rectangle[] rewindBodyFrame = new Rectangle[401];
		public Rectangle[] rewindLegFrame = new Rectangle[401];
		public float[] rewindHeadRotation = new float[401];
		public float[] rewindBodyRotation = new float[401];
		public float[] rewindLegRotation = new float[401];
		public float[] rewindFullRotation = new float[401];
		public Vector2[] rewindFullRotationOrigin = new Vector2[401];

        // {T} Added syncing for networked games.
        // Call it like SyncPlayer(-1, Main.myPlayer, false);
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)MrPlagueRacesMessageType.LycanSyncPlayer);
				packet.Write((byte)Player.whoAmI);
				packet.Write(chargingRewind);
				packet.Write(chargingTimeline);

				packet.Send(toWho, fromWho);
			}
        }

        // {T} Changed this from ModifyDrawInfo to PostUpdate.
        // You should NEVER use a drawing function for gameplay code. Try running this mod with uncapped FPS mods- it WILL explode.
        // Easy fix though ;)
        public override void PostUpdate()
        {
            var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
            if (!Player.dead)
			{
				if (chargingTimeline == true) {
					if (timelineCounter == 0) {
						SoundEngine.PlaySound(SoundID.Item159, Player.Center);
                        if (Player.whoAmI == Main.myPlayer)
                        {
                            mrPlagueRacesPlayer.LycanChargeSound(-1, Main.myPlayer);
                        }
                    }
					counter++;
					if (counter >= 20) {
						counter = 0;
						for (int i = 0; i < 40; i++) {
							if (timelineCounter >= i * 10) {
								int dustMark = Dust.NewDust(new Vector2(rewindPosition[i * 10].X + (Player.direction == 1 ? 8 : 4), rewindPosition[i * 10].Y + 8), 0, 0, 66);
								Main.dust[dustMark].color = new Color(255, 0, 0);
								Main.dust[dustMark].velocity = Vector2.Zero;
								Main.dust[dustMark].scale = 0.5f;
								Main.dust[dustMark].noGravity = true;
                                if (Player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.LycanRedDust(-1, Main.myPlayer, rewindPosition[i * 10].X, rewindPosition[i * 10].Y, 0.5f);
                                }
                            }
						}
						SoundEngine.PlaySound(SoundID.MenuTick, Player.Center);
                        if (Player.whoAmI == Main.myPlayer)
                        {
                            mrPlagueRacesPlayer.LycanChargeTickSound(-1, Main.myPlayer);
                        }
                    }
					int dustForward = Dust.NewDust(new Vector2(rewindPosition[timelineCounter].X + (Player.direction == 1 ? 8 : 4), rewindPosition[timelineCounter].Y + 8), 0, 0, 66);
					Main.dust[dustForward].color = new Color(255, 0, 0);
					Main.dust[dustForward].velocity = Vector2.Zero;
					Main.dust[dustForward].scale = 0.8f;
					Main.dust[dustForward].noGravity = true;
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        mrPlagueRacesPlayer.LycanRedDust(-1, Main.myPlayer, rewindPosition[timelineCounter].X, rewindPosition[timelineCounter].Y, 0.8f);
                    }
                    if (rewindPosition[timelineCounter + 1] != Vector2.Zero) {
						timelineCounter++;
					}
				}
				else if (timelineCounter > 0) {
					SoundEngine.PlaySound(SoundID.Item117, Player.Center);
					SoundEngine.PlaySound(SoundID.Item130, Player.Center);
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        mrPlagueRacesPlayer.LycanTeleportSound(-1, Main.myPlayer);
                    }
                    Player.Teleport(rewindPosition[timelineCounter], 15);
					Player.direction = rewindDirection[timelineCounter];
					for (int i = 0; i < 40; i++) {
						int dustRewind = Dust.NewDust(new Vector2(Player.position.X + (Player.direction == 1 ? 8 : 4), Player.position.Y + 8), 0, 0, 66);
						Main.dust[dustRewind].color = new Color(0, 0, 255);
						Main.dust[dustRewind].velocity *= 5f;
						Main.dust[dustRewind].scale = 0.9f;
						Main.dust[dustRewind].noGravity = true;
                    }
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        mrPlagueRacesPlayer.LycanTeleportBurstDust(-1, Main.myPlayer, Player.position.X, Player.position.Y);
                    }
                    timelineCounter = 0;
					Player.AddBuff(BuffType<Rebounded>(), 300);
				}
				if (rewindCounter > 0 || chargingRewind) {
					Player.eyeHelper.BlinkBecausePlayerGotHurt();
				}
				if (chargingRewind == true) {
					float sqrMaxDetectDistance = 600 * 600;
					if (rewindCounter == 0) {
						for (int i = 0; i < 400; i++) {
							rewindPosition[i] = Vector2.Zero;
							rewindDirection[i] = 0;
							rewindHeadFrame[i] = new Rectangle();
							rewindBodyFrame[i] = new Rectangle();
							rewindLegFrame[i] = new Rectangle();
							rewindHeadRotation[i] = 0;
							rewindBodyRotation[i] = 0;
							rewindLegRotation[i] = 0;
							rewindFullRotation[i] = 0;
							rewindFullRotationOrigin[i] = Vector2.Zero;
						}
						if (ModContent.GetInstance<LycanConfig>().lycanRewindNPCs)
						{
                            // {T} Changed the logic here to not target inactive NPCs, as it was throwing exceptions.
                            foreach (NPC target in Main.ActiveNPCs)
							{
                                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Player.Center);
                                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                                {
                                    var rewindNPC = target.GetGlobalNPC<RewindNPC>();
                                    rewindNPC.rewindHost = Player;
                                }
                            }
                        }
						if (ModContent.GetInstance<LycanConfig>().lycanRewindProjectiles)
						{
                            // {T} Changed the logic here to not target inactive projectiles, as it was throwing exceptions.
                            foreach (Projectile target in Main.ActiveProjectiles) {
								float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Player.Center);
								if (sqrDistanceToTarget < sqrMaxDetectDistance)
								{
									var rewindProjectile = target.GetGlobalProjectile<RewindProjectile>();
									rewindProjectile.rewindHost = Player;
								}
							}
						}
						SoundEngine.PlaySound(SoundID.Item159, Player.Center);
                        if (Player.whoAmI == Main.myPlayer)
                        {
                            mrPlagueRacesPlayer.LycanChargeSound(-1, Main.myPlayer);
                        }
                    }
					initializedRewind = false;
					counter++;
					if (counter >= 20) {
						counter = 0;
						for (int i = 0; i < 40; i++) {
							if (rewindCounter >= i * 10) {
								int dustMark = Dust.NewDust(new Vector2(rewindPosition[i * 10].X + (Player.direction == 1 ? 8 : 4), rewindPosition[i * 10].Y + 8), 0, 0, 66);
								Main.dust[dustMark].color = new Color(255, 0, 0);
								Main.dust[dustMark].velocity = Vector2.Zero;
								Main.dust[dustMark].scale = 0.5f;
								Main.dust[dustMark].noGravity = true;
                                if (Player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.LycanRedDust(-1, Main.myPlayer, rewindPosition[i * 10].X, rewindPosition[i * 10].Y, 0.5f);
                                }
                            }
						}
						SoundEngine.PlaySound(SoundID.MenuTick, Player.Center);
                        if (Player.whoAmI == Main.myPlayer)
                        {
                            mrPlagueRacesPlayer.LycanChargeTickSound(-1, Main.myPlayer);
                        }
                    }
					if (rewindCounter < 400) {
						rewindPosition[rewindCounter] = Player.position;
						rewindDirection[rewindCounter] = Player.direction;
						rewindHeadFrame[rewindCounter] = Player.headFrame;
						rewindBodyFrame[rewindCounter] = Player.bodyFrame;
						rewindLegFrame[rewindCounter] = Player.legFrame;
						rewindHeadRotation[rewindCounter] = Player.headRotation;
						rewindBodyRotation[rewindCounter] = Player.bodyRotation;
						rewindLegRotation[rewindCounter] = Player.legRotation;
						rewindFullRotation[rewindCounter] = Player.fullRotation;
						rewindFullRotationOrigin[rewindCounter] = Player.fullRotationOrigin;
						int dustForward = Dust.NewDust(new Vector2(rewindPosition[rewindCounter].X + (Player.direction == 1 ? 8 : 4), rewindPosition[rewindCounter].Y + 8), 0, 0, 66);
						Main.dust[dustForward].color = new Color(255, 0, 0);
						Main.dust[dustForward].velocity = Vector2.Zero;
						Main.dust[dustForward].scale = 0.8f;
						Main.dust[dustForward].noGravity = true;
                        if (Player.whoAmI == Main.myPlayer)
                        {
                            mrPlagueRacesPlayer.LycanRedDust(-1, Main.myPlayer, rewindPosition[rewindCounter].X, rewindPosition[rewindCounter].Y, 0.8f);
                        }
                        rewindCounter++;
					}
					else {
						for (int i = 0; i < 400; i++) {
							if (i < 399) {
								rewindPosition[i] = rewindPosition[i + 1];
								rewindDirection[i] = rewindDirection[i + 1];
								rewindHeadFrame[i] = rewindHeadFrame[i + 1];
								rewindBodyFrame[i] = rewindBodyFrame[i + 1];
								rewindLegFrame[i] = rewindLegFrame[i + 1];
								rewindHeadRotation[i] = rewindHeadRotation[i + 1];
								rewindBodyRotation[i] = rewindBodyRotation[i + 1];
								rewindLegRotation[i] = rewindLegRotation[i + 1];
								rewindFullRotation[i] = rewindFullRotation[i + 1];
								rewindFullRotationOrigin[i] = rewindFullRotationOrigin[i + 1];
							}
							else {
								rewindPosition[i] = Player.position;
								rewindDirection[i] = Player.direction;
								rewindHeadFrame[i] = Player.headFrame;
								rewindBodyFrame[i] = Player.bodyFrame;
								rewindLegFrame[i] = Player.legFrame;
								rewindHeadRotation[i] = Player.headRotation;
								rewindBodyRotation[i] = Player.bodyRotation;
								rewindLegRotation[i] = Player.legRotation;
								rewindFullRotation[i] = Player.fullRotation;
								rewindFullRotationOrigin[i] = Player.fullRotationOrigin;
								int dustForward = Dust.NewDust(new Vector2(rewindPosition[i].X + (Player.direction == 1 ? 8 : 4), rewindPosition[i].Y + 8), 0, 0, 66);
								Main.dust[dustForward].color = new Color(255, 0, 0);
								Main.dust[dustForward].velocity = Vector2.Zero;
								Main.dust[dustForward].scale = 0.8f;
								Main.dust[dustForward].noGravity = true;
                                if (Player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.LycanRedDust(-1, Main.myPlayer, rewindPosition[i].X, rewindPosition[i].Y, 0.8f);
                                }
                            }
						}
					}
					int dustStart = Dust.NewDust(new Vector2(rewindPosition[0].X + (Player.direction == 1 ? 8 : 4), rewindPosition[0].Y + 8), 0, 0, 66);
					Main.dust[dustStart].color = new Color(255, 0, 0);
					Main.dust[dustStart].velocity = Vector2.Zero;
					Main.dust[dustStart].scale = 0.8f;
					Main.dust[dustStart].noGravity = true;
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        mrPlagueRacesPlayer.LycanRedDust(-1, Main.myPlayer, rewindPosition[0].X, rewindPosition[0].Y, 0.8f);
                    }
                }
				else if (rewindCounter > 0) {
					if (!initializedRewind) {
						for (int i = 0; i < 40; i++) {
							int dustRewind = Dust.NewDust(new Vector2(Player.position.X + (Player.direction == 1 ? 8 : 4), Player.position.Y + 8), 0, 0, 66);
							Main.dust[dustRewind].color = new Color(0, 0, 255);
							Main.dust[dustRewind].velocity *= 5f;
							Main.dust[dustRewind].scale = 0.9f;
							Main.dust[dustRewind].noGravity = true;
                            if (Player.whoAmI == Main.myPlayer)
                            {
                                mrPlagueRacesPlayer.LycanBlueDust(-1, Main.myPlayer, Player.position.X, Player.position.Y, 0.9f);
                            }
                            SoundEngine.PlaySound(SoundID.Item164, Player.Center);
                            if (Player.whoAmI == Main.myPlayer)
                            {
                                mrPlagueRacesPlayer.LycanRewindSound(-1, Main.myPlayer);
                            }
                            Player.AddBuff(BuffType<TemporalRecoil>(), rewindCounter);
						}
						initializedRewind = true;
					}
					for (int i = 0; i < 5; i++) {
						if (rewindCounter > 0) {
                            // {T} Safety. I found you could crash the game by just using the ability for a single tick and releasing.
                            if (rewindPosition[rewindCounter] != Vector2.Zero)
                            {
								Player.position = rewindPosition[rewindCounter];
								Player.direction = rewindDirection[rewindCounter];
								Player.headFrame = rewindHeadFrame[rewindCounter];
								Player.bodyFrame = rewindBodyFrame[rewindCounter];
								Player.legFrame = rewindLegFrame[rewindCounter];
								Player.headRotation = rewindHeadRotation[rewindCounter];
								Player.bodyRotation = rewindBodyRotation[rewindCounter];
								Player.legRotation = rewindLegRotation[rewindCounter];
								Player.fullRotation = rewindFullRotation[rewindCounter];
								Player.fullRotationOrigin = rewindFullRotationOrigin[rewindCounter];
							}
							int dustRewind = Dust.NewDust(new Vector2(Player.position.X + (Player.direction == 1 ? 8 : 4), Player.position.Y + 8), 0, 0, 66);
							Main.dust[dustRewind].color = new Color(0, 0, 255);
							Main.dust[dustRewind].velocity = Vector2.Zero;
							Main.dust[dustRewind].scale = 0.9f;
							Main.dust[dustRewind].noGravity = true;
                            if (Player.whoAmI == Main.myPlayer)
                            {
                                mrPlagueRacesPlayer.LycanBlueDust(-1, Main.myPlayer, Player.position.X, Player.position.Y, 0.9f);
                            }
                            rewindCounter--;
						}
					}
					counter++;
					if (counter >= 10) {
						counter = 0;
						SoundEngine.PlaySound(SoundID.MenuTick, Player.Center);
                        if (Player.whoAmI == Main.myPlayer)
                        {
                            mrPlagueRacesPlayer.LycanChargeTickSound(-1, Main.myPlayer);
                        }
                    }
				}
			}
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			rewindCounter = 0;
			for (int i = 0; i < 400; i++) {
				rewindPosition[i] = Vector2.Zero;
				rewindDirection[i] = 0;
				rewindHeadFrame[i] = new Rectangle();
				rewindBodyFrame[i] = new Rectangle();
				rewindLegFrame[i] = new Rectangle();
				rewindHeadRotation[i] = 0;
				rewindBodyRotation[i] = 0;
				rewindLegRotation[i] = 0;
				rewindFullRotation[i] = 0;
				rewindFullRotationOrigin[i] = Vector2.Zero;
			}
		}
	}

	public class RewindNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public int rewindCounter = 0;
		public int counter = 0;
		public Rectangle[] rewindFrame = new Rectangle[401];
		public float[] rewindRotation = new float[401];
		public Vector2[] rewindPosition = new Vector2[401];
		public Player rewindHost;

		public override void PostAI(NPC npc)
		{
			if (rewindHost != null) {
				var rewindPlayer = rewindHost.GetModPlayer<RewindPlayer>();
				if (rewindPlayer.chargingRewind) {
					counter++;
					if (counter >= 20) {
						counter = 0;
						for (int i = 0; i < 40; i++) {
							if (rewindCounter >= i * 10) {
								int dustMark = Dust.NewDust(new Vector2(rewindPosition[i * 10].X, rewindPosition[i * 10].Y + 8), 0, 0, 66);
								Main.dust[dustMark].color = new Color(255, 0, 0);
								Main.dust[dustMark].velocity = Vector2.Zero;
								Main.dust[dustMark].scale = 0.5f;
								Main.dust[dustMark].noGravity = true;
							}
						}
					}
					if (rewindCounter < 400) {
						rewindFrame[rewindCounter] = npc.frame;
						rewindRotation[rewindCounter] = npc.rotation;
						rewindPosition[rewindCounter] = npc.position;
						int dustForward = Dust.NewDust(new Vector2(rewindPosition[rewindCounter].X, rewindPosition[rewindCounter].Y + 8), 0, 0, 66);
						Main.dust[dustForward].color = new Color(255, 0, 0);
						Main.dust[dustForward].velocity = Vector2.Zero;
						Main.dust[dustForward].scale = 0.8f;
						Main.dust[dustForward].noGravity = true;
						rewindCounter++;
					}
					else {
						for (int i = 0; i < 400; i++) {
							if (i < 399) {
								rewindFrame[i] = rewindFrame[i + 1];
								rewindRotation[i] = rewindRotation[i + 1];
								rewindPosition[i] = rewindPosition[i + 1];
							}
							else {
								rewindFrame[i] = npc.frame;
								rewindRotation[i] = npc.rotation;
								rewindPosition[i] = npc.position;
								int dustForward = Dust.NewDust(new Vector2(rewindPosition[i].X, rewindPosition[i].Y + 8), 0, 0, 66);
								Main.dust[dustForward].color = new Color(255, 0, 0);
								Main.dust[dustForward].velocity = Vector2.Zero;
								Main.dust[dustForward].scale = 0.8f;
								Main.dust[dustForward].noGravity = true;
							}
						}
					}
					int dustStart = Dust.NewDust(new Vector2(rewindPosition[0].X, rewindPosition[0].Y + 8), 0, 0, 66);
					Main.dust[dustStart].color = new Color(255, 0, 0);
					Main.dust[dustStart].velocity = Vector2.Zero;
					Main.dust[dustStart].scale = 0.8f;
					Main.dust[dustStart].noGravity = true;
				}
				else if (rewindCounter > 0) {
					for (int i = 0; i < 5; i++) {
						if (rewindCounter > 0) {
							npc.frame = rewindFrame[rewindCounter];
							npc.rotation = rewindRotation[rewindCounter];
							npc.position = rewindPosition[rewindCounter];
							int dustRewind = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 8), 0, 0, 66);
							Main.dust[dustRewind].color = new Color(0, 0, 255);
							Main.dust[dustRewind].velocity = Vector2.Zero;
							Main.dust[dustRewind].scale = 0.9f;
							Main.dust[dustRewind].noGravity = true;
							rewindCounter--;
						}
					}
				}
				else if (rewindCounter == 0) {
					rewindHost = null;
				}
			}
		}
	}

	public class RewindProjectile : GlobalProjectile
	{
		public override bool InstancePerEntity => true;
		public int rewindCounter = 0;
		public int counter = 0;
		public int[] rewindFrame = new int[401];
		public float[] rewindRotation = new float[401];
		public Vector2[] rewindPosition = new Vector2[401];
		public Player rewindHost;

		public override void PostAI(Projectile projectile)
		{
			if (rewindHost != null) {
				var rewindPlayer = rewindHost.GetModPlayer<RewindPlayer>();
				if (rewindPlayer.chargingRewind) {
					counter++;
					if (counter >= 20) {
						counter = 0;
						for (int i = 0; i < 40; i++) {
							if (rewindCounter >= i * 10) {
								int dustMark = Dust.NewDust(new Vector2(rewindPosition[i * 10].X, rewindPosition[i * 10].Y + 8), 0, 0, 66);
								Main.dust[dustMark].color = new Color(255, 0, 0);
								Main.dust[dustMark].velocity = Vector2.Zero;
								Main.dust[dustMark].scale = 0.5f;
								Main.dust[dustMark].noGravity = true;
							}
						}
					}
					if (rewindCounter < 400) {
						rewindFrame[rewindCounter] = projectile.frame;
						rewindRotation[rewindCounter] = projectile.rotation;
						rewindPosition[rewindCounter] = projectile.position;
						int dustForward = Dust.NewDust(new Vector2(rewindPosition[rewindCounter].X, rewindPosition[rewindCounter].Y + 8), 0, 0, 66);
						Main.dust[dustForward].color = new Color(255, 0, 0);
						Main.dust[dustForward].velocity = Vector2.Zero;
						Main.dust[dustForward].scale = 0.8f;
						Main.dust[dustForward].noGravity = true;
						rewindCounter++;
					}
					else {
						for (int i = 0; i < 400; i++) {
							if (i < 399) {
								rewindFrame[i] = rewindFrame[i + 1];
								rewindRotation[i] = rewindRotation[i + 1];
								rewindPosition[i] = rewindPosition[i + 1];
							}
							else {
								rewindFrame[i] = projectile.frame;
								rewindRotation[i] = projectile.rotation;
								rewindPosition[i] = projectile.position;
								int dustForward = Dust.NewDust(new Vector2(rewindPosition[i].X, rewindPosition[i].Y + 8), 0, 0, 66);
								Main.dust[dustForward].color = new Color(255, 0, 0);
								Main.dust[dustForward].velocity = Vector2.Zero;
								Main.dust[dustForward].scale = 0.8f;
								Main.dust[dustForward].noGravity = true;
							}
						}
					}
					int dustStart = Dust.NewDust(new Vector2(rewindPosition[0].X, rewindPosition[0].Y + 8), 0, 0, 66);
					Main.dust[dustStart].color = new Color(255, 0, 0);
					Main.dust[dustStart].velocity = Vector2.Zero;
					Main.dust[dustStart].scale = 0.8f;
					Main.dust[dustStart].noGravity = true;
				}
				else if (rewindCounter > 0) {
					for (int i = 0; i < 5; i++) {
						if (rewindCounter > 0) {
							projectile.frame = rewindFrame[rewindCounter];
							projectile.rotation = rewindRotation[rewindCounter];
							projectile.position = rewindPosition[rewindCounter];
							int dustRewind = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 8), 0, 0, 66);
							Main.dust[dustRewind].color = new Color(0, 0, 255);
							Main.dust[dustRewind].velocity = Vector2.Zero;
							Main.dust[dustRewind].scale = 0.9f;
							Main.dust[dustRewind].noGravity = true;
							rewindCounter--;
						}
					}
				}
				else if (rewindCounter == 0) {
					rewindHost = null;
				}
			}
		}
    }

    public class LycanConfig : ModConfig
    {
        public static LycanConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Lycan")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> lycanStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%
            [RacialStatType.allDamage] = RacialStatPercentageModifier.p15,
            [RacialStatType.moveSpeed] = RacialStatPercentageModifier.p15,
            [RacialStatType.jumpSpeedBoost] = RacialStatPercentageModifier.p5,
            [RacialStatType.endurance] = RacialStatPercentageModifier.p10,
            [RacialStatType.tileSpeed] = RacialStatPercentageModifier.n10,
            [RacialStatType.wallSpeed] = RacialStatPercentageModifier.n10
        };
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool lycanAbility1;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool lycanAbility2;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool lycanRewindNPCs;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool lycanRewindProjectiles;
    }
}