using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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

namespace MrPlagueRaces.Common.Races.Kenku
{
	public class Kenku : Race
	{
		public override void Load()
        {
			Description = "Naturally capable of flight, Kenku possess greater control over aerial movement.";
            DisplayName = "[c/6F5BF0:Kenku]";
            CensorClothing = false;
			StarterShirt = true;
			HairColor = new Color(120, 96, 172);
			SkinColor = new Color(120, 96, 172);
			DetailColor = new Color(171, 173, 212);
			EyeColor = new Color(151, 73, 0);
			ShirtColor = new Color(201, 180, 177);
			UnderShirtColor = new Color(199, 122, 156);
        }

        public override void ResetEffects(Player player)
        {
            RegisterAbilityDescription(ModContent.GetInstance<KenkuConfig>().kenkuAbility1, $"[c/4DBF60:+] While airborne, press Z to swoop forwards.");
            RegisterAbilityDescription(ModContent.GetInstance<KenkuConfig>().kenkuAbility2, $"[c/4DBF60:+] Press X to fire a volley of feathers. Fly within range of the feathers to launch yourself.");
            RegisterAbilityDescription(ModContent.GetInstance<KenkuConfig>().kenkuWings, $"[c/4DBF60:+] You are equipped with natural wings by default. Their power scales with max health.");
            RegisterAbilityDescription(ModContent.GetInstance<KenkuConfig>().kenkuNoFallDmg, $"[c/4DBF60:+] At above 160 max health, you no longer take fall damage.");

            RaceStatDictionary = ModContent.GetInstance<KenkuConfig>().kenkuStats;
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				/*player.moveSpeed += 0.2f;
				player.jumpSpeedBoost += 0.15f;
				player.statLifeMax2 -= (player.statLifeMax2 / 3);
				player.endurance -= 0.3f;*/
				player.rocketTime = 0;
				player.rocketTimeMax = 0;
				if (ModContent.GetInstance<KenkuConfig>().kenkuNoFallDmg)
				{
                    if (player.statLifeMax2 > 160)
                    {
                        player.noFallDmg = true;
                    }
                }
			}
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var kenkuPlayer = player.GetModPlayer<KenkuPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				float anchor = player.statLifeMax2;
                int maxVelocity = (6 + (player.statLifeMax2 / 100));
                if (player.statLifeMax2 > 120) {
					anchor = 120;
				}
				if (!player.dead && player.active)
                {
					if (ModContent.GetInstance<KenkuConfig>().kenkuAbility1)
					{
						if (MrPlagueRaces.RaceAbilityKeybind1.JustPressed && player.velocity.Y != 0 && kenkuPlayer.wingTime > 0 && !player.HasBuff(BuffType<Dashed>()))
						{
							player.velocity.X = (16 * player.direction) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<KenkuConfig>().kenkuSwoopVelocity]);
							SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, player.Center);
                            if (player.whoAmI == Main.myPlayer)
                            {
                                mrPlagueRacesPlayer.KenkuDashSound(-1, Main.myPlayer);
                            }
                            kenkuPlayer.wingFrame = 2;
							kenkuPlayer.wingFrameCounter = 0;
							kenkuPlayer.dashTime = 32;
							player.AddBuff(BuffType<Dashed>(), 220);
						}
                    }
					if (ModContent.GetInstance<KenkuConfig>().kenkuAbility2)
					{
						if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed && kenkuPlayer.wingTime > 0 && !player.HasBuff(BuffType<Fatigued>()))
						{
							Vector2 velocity = Vector2.Normalize(mrPlagueRacesPlayer.mouseWorld - player.Center) * 10f;
							player.direction = mrPlagueRacesPlayer.mouseWorld.X >= player.Center.X ? 1 : -1;
							player.fallStart = (int)(player.position.Y / 16f);
							player.velocity.X = 6 * -player.direction;
							SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash, player.Center);
                            if (player.whoAmI == Main.myPlayer)
                            {
                                mrPlagueRacesPlayer.KenkuSummonFeathersSound(-1, Main.myPlayer);
                            }
                            kenkuPlayer.wingFrame = 2;
							kenkuPlayer.wingFrameCounter = 0;
							kenkuPlayer.dashTime = 32;
							for (int i = 0; i < 12; i++)
							{
                                // {T} Moved the multiplication of velocity into the NewProjectile call instead of a separate line.
								// It is NOT network-friendly to modify a projectile directly after it has been spawned, as those changes will not be
								// synced across the network. Such changes should only be done in the projectile's AI code.
                                Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X + (float)(player.width / 2) / 16, player.Center.Y, (velocity.X + Main.rand.Next(3) - Main.rand.Next(3)) * 3f, (velocity.Y + Main.rand.Next(3) - Main.rand.Next(3)) * 3f, ProjectileType<KenkuFeather>(), (int)((1f + (player.ConsumedLifeFruit * 0.075f)) * (3 * (player.statLifeMax2 / 40))) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<KenkuConfig>().kenkuFeatherDamage]), 0, player.whoAmI);
							}
							player.AddBuff(BuffType<Fatigued>(), 320);
						}
                    }
					if (ModContent.GetInstance<KenkuConfig>().kenkuWings)
					{
						if (player.wings == 0 && !player.stoned && !player.frozen && !player.webbed && !player.shimmering)
						{
							if (player.controlJump)
							{
								kenkuPlayer.dashTime = 0;
								player.fallStart = (int)(player.position.Y / 16f);
								if (player.velocity.Y != 0)
								{
									kenkuPlayer.flying = true;
								}
								if (kenkuPlayer.wingTime > 0)
								{
									kenkuPlayer.wingFrameCounter++;
									if (kenkuPlayer.wingFrameCounter > 4)
									{
										kenkuPlayer.wingFrame++;
										kenkuPlayer.wingFrameCounter = 0;
										if (kenkuPlayer.wingFrame >= 4)
										{
											kenkuPlayer.wingFrame = 0;
											SoundEngine.PlaySound(SoundID.Item32, player.Center);
                                            if (player.whoAmI == Main.myPlayer)
                                            {
                                                mrPlagueRacesPlayer.KenkuFlapSound(-1, Main.myPlayer);
                                            }
                                        }
									}
									float ascentWhenFalling = (float)(anchor / 100);
									float ascentWhenRising = (float)(anchor / 420);
									float maxCanAscendMultiplier = (float)(anchor / 100);
									float maxAscentMultiplier = (float)(anchor / 25);
									float constantAscend = (float)(anchor / 500);

									player.velocity.Y -= ascentWhenFalling * player.gravDir;

									if (player.gravDir == 1f)
									{
										if (player.velocity.Y > 0f)
										{
											player.velocity.Y -= ascentWhenRising;
										}
										else if (player.velocity.Y > (0f - 5) * maxAscentMultiplier)
										{
											player.velocity.Y -= constantAscend;
										}
										if (player.velocity.Y < (0f - 5) * maxCanAscendMultiplier)
										{
											player.velocity.Y = (0f - 5) * maxCanAscendMultiplier;
										}
									}
									else
									{
										if (player.velocity.Y < 0f)
										{
											player.velocity.Y += ascentWhenRising;
										}
										else if (player.velocity.Y < 5 * maxAscentMultiplier)
										{
											player.velocity.Y += constantAscend;
										}
										if (player.velocity.Y > 5 * maxCanAscendMultiplier)
										{
											player.velocity.Y = 5 * maxCanAscendMultiplier;
										}
									}
									kenkuPlayer.wingTime -= 1f;
								}
								else
								{
									if (player.velocity.Y > 3f)
									{
										player.velocity.Y = 3f;
									}
									if (player.velocity.Y > 1)
									{
										kenkuPlayer.wingFrameCounter = 0;
										kenkuPlayer.wingFrame = 2;
									}
								}
								if (player.controlLeft && player.velocity.X > ((player.maxRunSpeed * 2) * -1))
								{
									player.velocity.X += -0.1f;
								}
								if (player.controlRight && player.velocity.X < (player.maxRunSpeed * 2))
								{
									player.velocity.X += 0.1f;
								}
							}
							else if (kenkuPlayer.dashTime == 0)
							{
								kenkuPlayer.wingFrameCounter = 0;
								kenkuPlayer.wingFrame = 0;
							}
							if (player.empressBrooch && kenkuPlayer.wingTime != 0f)
							{
								kenkuPlayer.wingTime = player.statLifeMax2 * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<KenkuConfig>().kenkuWingsPower]);
							}
						}
						if (player.velocity.Y == 0)
						{
							kenkuPlayer.wingTime = player.statLifeMax2 * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<KenkuConfig>().kenkuWingsPower]);
						}
					}
				}
				if (!player.controlJump || player.velocity.Y == 0) {
					kenkuPlayer.flying = false;
				}
			}
		}
		
		public override void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var kenkuPlayer = player.GetModPlayer<KenkuPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (kenkuPlayer.flying && !player.controlUseItem) {
					player.bodyFrame.Y = player.bodyFrame.Height * 6;
				}
				if (player.velocity.Y != 0) {
					player.fullRotationOrigin = new Vector2((player.width / 2), (player.height / 2));
					player.fullRotation = player.velocity.X * 0.1f;
					if ((double)player.fullRotation < -0.2)
					{
						player.fullRotation = -0.2f;
					}
					if ((double)player.fullRotation > 0.2)
					{
						player.fullRotation = 0.2f;
					}
				}
				else if (!player.sleeping.isSleeping) {
					player.fullRotation = 0f;
				}
			}
		}

		public override void PreUpdate(Player player) 
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var kenkuPlayer = player.GetModPlayer<KenkuPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (player.velocity.Y == 0) {
					kenkuPlayer.wingFrameCounter = 0;
					kenkuPlayer.wingFrame = 0;
				}
				else if (player.velocity.Y != 0 && !player.controlJump && kenkuPlayer.dashTime == 0) {
					kenkuPlayer.wingFrameCounter = 0;
					kenkuPlayer.wingFrame = 1;
				}
				if (kenkuPlayer.dashTime > 0) {
					kenkuPlayer.dashTime--;
					kenkuPlayer.wingFrameCounter++;
					if (kenkuPlayer.wingFrameCounter > 6)
					{
						kenkuPlayer.wingFrame++;
						kenkuPlayer.wingFrameCounter = 0;
						if (kenkuPlayer.wingFrame >= 3)
						{
							kenkuPlayer.wingFrame = 0;
							SoundEngine.PlaySound(SoundID.Item32, player.Center);
                            if (player.whoAmI == Main.myPlayer)
                            {
                                mrPlagueRacesPlayer.KenkuFlapSound(-1, Main.myPlayer);
                            }
                        }
					}
				}
			}
        }

        public override void PostUpdate(Player player)
        {
            var kenkuPlayer = player.GetModPlayer<KenkuPlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					kenkuPlayer.SyncPlayer(-1, Main.myPlayer, false);
				}

                var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
                mrPlagueRacesPlayer.SyncRotationsAndOffsets(-1, Main.myPlayer);
            }
        }
    }

	public class KenkuPlayer : ModPlayer
	{
		public float wingTime;
		public bool flying;
		public int wingFrame;
		public int wingFrameCounter;
		public int dashTime;

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.KenkuSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(wingTime);
			packet.Write(flying);
			packet.Write(wingFrame);
			packet.Write(wingFrameCounter);
			packet.Write(dashTime);

            packet.Send(toWho, fromWho);
        }
	}

	public class KenkuWings : PlayerDrawLayer
	{
		private Asset<Texture2D>[] Wings_Texture = new Asset<Texture2D>[10];
		private string[] PlayerColors = { "ColorSkin", "ColorDetail", "Colorless", "ColorEyes", "ColorHair", "ColorSkin/Glowmask", "ColorDetail/Glowmask", "Colorless/Glowmask", "ColorEyes/Glowmask", "ColorHair/Glowmask" };

		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Wings);

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) 
		{
			Player drawPlayer = drawInfo.drawPlayer;
			return (drawInfo.skinVar < 10 && drawPlayer.wings == 0);
		}

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			Vector2 helmetOffset = drawInfo.helmetOffset;

			var mrPlagueRacesPlayer = drawPlayer.GetModPlayer<MrPlagueRacesPlayer>();
			var kenkuPlayer = drawPlayer.GetModPlayer<KenkuPlayer>();
			if (mrPlagueRacesPlayer.race != null) {
				for (int i = 0; i < 10; i++)
				{
					Wings_Texture[i] = mrPlagueRacesPlayer.GetRaceTexture(drawPlayer, $"{PlayerColors[i]}/Wings");
				}
				Vector2 bodyPosition = new Vector2((float)(int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawPlayer.bodyFrame.Width / 2) + (float)(drawPlayer.width / 2)), (float)(int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2((float)(drawPlayer.bodyFrame.Width / 2), (float)(drawPlayer.bodyFrame.Height / 2));
				MakeColoredDrawDatas(ref drawInfo, Wings_Texture, null, new Vector2(bodyPosition.X - (9 * drawPlayer.direction), bodyPosition.Y - 9), new Rectangle(0, Wings_Texture[0].Height() / 4 * kenkuPlayer.wingFrame, Wings_Texture[0].Width(), Wings_Texture[0].Height() / 4), drawPlayer.bodyRotation, new Vector2((float)(Wings_Texture[0].Width() / 2), (float)(Wings_Texture[0].Height() / 14)), 1f, drawInfo.playerEffect, 0);
			}
		}

		private void MakeColoredDrawDatas(ref PlayerDrawSet drawInfo, Asset<Texture2D>[] texture, Asset<Texture2D>[,] textureHair, Vector2 position, Rectangle? sourceRect, float rotation, Vector2 origin, float scale, SpriteEffects effect, int inactiveLayerDepth)
		{
			DrawData drawData;
			Player drawPlayer = drawInfo.drawPlayer;
			int index;
			for (index = 0; index < 10; index++)
			{
				if (textureHair != null && textureHair[index, drawPlayer.hair] != ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank"))
				{
					drawData = new DrawData(textureHair[index, drawPlayer.hair].Value, position, sourceRect, PlayerColor(ref drawInfo, index), rotation, origin, scale, effect, 0);
					drawData.shader = PlayerShader(ref drawInfo, index);
					drawInfo.DrawDataCache.Add(drawData);
				}
				if (texture != null && texture[index] != ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank"))
				{
					drawData = new DrawData(texture[index].Value, position, sourceRect, PlayerColor(ref drawInfo, index), rotation, origin, scale, effect, 0);
					drawData.shader = PlayerShader(ref drawInfo, index);
					drawInfo.DrawDataCache.Add(drawData);
				}
			}
		}

		private Color PlayerColor(ref PlayerDrawSet drawInfo, int index)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			var mrPlagueRacesPlayer = drawPlayer.GetModPlayer<MrPlagueRacesPlayer>();
			Color color = (index == 0 ? drawInfo.colorHead : index == 1 ? mrPlagueRacesPlayer.colorDetail : index == 2 ? drawInfo.colorEyeWhites : index == 3 ? drawInfo.colorEyes : index == 4 ? drawInfo.colorHair : index == 5 ? drawPlayer.GetImmuneAlpha(drawPlayer.skinColor, 0f) : index == 6 ? drawPlayer.GetImmuneAlpha(mrPlagueRacesPlayer.detailColor, 0f) : index == 7 ? drawPlayer.GetImmuneAlpha(Color.White, 0f) : index == 8 ? drawPlayer.GetImmuneAlpha(drawPlayer.eyeColor, 0f) : drawPlayer.GetImmuneAlpha(drawPlayer.GetHairColor(useLighting: false), 0f));
			return color;
		}

		private int PlayerShader(ref PlayerDrawSet drawInfo, int index)
		{
			int shader = (index == 0 ? drawInfo.skinDyePacked : index == 1 ? drawInfo.skinDyePacked : index == 2 ? 0 : index == 3 ? 0 : index == 4 ? drawInfo.hairDyePacked : index == 5 ? drawInfo.skinDyePacked : index == 6 ? drawInfo.skinDyePacked : index == 7 ? 0 : index == 8 ? 0 : drawInfo.hairDyePacked);
			return shader;
		}
    }

    public class KenkuConfig : ModConfig
    {
        public static KenkuConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Kenku")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> kenkuStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%
            [RacialStatType.statLifeMax2] = RacialStatPercentageModifier.n33,
            [RacialStatType.moveSpeed] = RacialStatPercentageModifier.p20,
            [RacialStatType.jumpSpeedBoost] = RacialStatPercentageModifier.p15,
            [RacialStatType.endurance] = RacialStatPercentageModifier.n30
        };
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool kenkuAbility1;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool kenkuAbility2;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool kenkuWings;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier kenkuWingsPower;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier kenkuFeatherDamage;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier kenkuSwoopVelocity;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier kenkuFeatherVelocity;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool kenkuNoFallDmg;
    }
}