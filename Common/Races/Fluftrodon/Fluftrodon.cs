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
using MrPlagueRaces.Common.UI.States;
using MrPlagueRaces.Common.Systems;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Content.Projectiles;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.Races.Fluftrodon
{
	public class Fluftrodon : Race
	{
		public override void Load()
        {
			Description = "Capable of generating paint via an arcane process, Fluftrodons highly value the arts.";
            DisplayName = "[c/A1E7FF:Fluftrodon]";
            CensorClothing = false;
			HairColor = new Color(190, 233, 255);
			SkinColor = new Color(190, 233, 255);
			DetailColor = new Color(91, 115, 177);
            AuxilaryDetailColor1 = new Color(91, 115, 177);
            AuxilaryDetailColor2 = new Color(190, 192, 255);
            AuxilaryDetailColor3 = new Color(124, 158, 234);
            EyeColor = new Color(81, 135, 255);
        }

        public override void PreRaceChange(Player player) // Called before the player's race is changed
        {
            if (player.whoAmI == Main.myPlayer)
            {
                ModContent.GetInstance<FluftrodonPaintUISystem>().HideMyUI();
            }
        }

        public override void ResetEffects(Player player)
        {
            RegisterAbilityDescription(ModContent.GetInstance<FluftrodonConfig>().fluftrodonAbility1, $"[c/4DBF60:+] Press Z to paint. Left click and right click to paint tiles and walls, scroll to cycle color.");
			RegisterAbilityDescription(ModContent.GetInstance<FluftrodonConfig>().fluftrodonAbility2, $"[c/4DBF60:+] Hold X to charge up a powerful leap. Scales with max health.");
            RegisterAbilityDescription(ModContent.GetInstance<FluftrodonConfig>().fluftrodonWallJump, $"[c/4DBF60:+] Hold A or D against a wall and press space to walljump. Scales with max health.");

            RaceStatDictionary = ModContent.GetInstance<FluftrodonConfig>().fluftrodonStats;
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				/*player.tileSpeed += 0.5f;
				player.blockRange += 10;
				player.pickSpeed -= 0.25f;
				player.GetDamage(DamageClass.Generic) -= 0.15f;*/
			}
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var fluftrodonPlayer = player.GetModPlayer<FluftrodonPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
				{
					if (ModContent.GetInstance<FluftrodonConfig>().fluftrodonAbility1)
					{
						if (MrPlagueRaces.RaceAbilityKeybind1.JustPressed)
                        {
                            if (player.whoAmI == Main.myPlayer)
                            {
                                ModContent.GetInstance<FluftrodonPaintUISystem>().ToggleMyUI();
                            }
						}
                    }
					if (ModContent.GetInstance<FluftrodonConfig>().fluftrodonAbility2)
					{
						if (MrPlagueRaces.RaceAbilityKeybind2.Current && !player.HasBuff(BuffType<Airborne>()))
						{
							if (fluftrodonPlayer.jumpCharge == 0)
							{
								SoundEngine.PlaySound(SoundID.Item105, player.Center);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.FluftrodonChargeSound(-1, Main.myPlayer);
                                }
                                for (int i = 0; i < 10; i++)
								{
									int dust = Dust.NewDust(player.position, player.width, player.height, 264);
									Main.dust[dust].color = fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint];
									Main.dust[dust].noGravity = true;
									Main.dust[dust].velocity *= 2f;
									dust = Dust.NewDust(player.position, player.width, player.height, 264);
									Main.dust[dust].color = fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint];
									Main.dust[dust].noGravity = true;
									Main.dust[dust].velocity *= 1f;
                                }
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.FluftrodonChargeDust(-1, Main.myPlayer);
                                }
                            }
							if (fluftrodonPlayer.jumpCharge < 40)
							{
								fluftrodonPlayer.jumpCharge++;
							}
						}
						if (MrPlagueRaces.RaceAbilityKeybind2.JustReleased)
						{
							if (fluftrodonPlayer.jumpCharge > 0)
							{
								SoundEngine.PlaySound(SoundID.Item152, player.Center);
								SoundEngine.PlaySound(SoundID.Item39, player.Center);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.FluftrodonJumpSound(-1, Main.myPlayer);
                                }
                                for (int i = 0; i < 20; i++)
								{
									int dust = Dust.NewDust(player.position, player.width, player.height, 264);
									Main.dust[dust].color = fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint];
									Main.dust[dust].noGravity = true;
									Main.dust[dust].velocity *= 5f;
									dust = Dust.NewDust(player.position, player.width, player.height, 264);
									Main.dust[dust].color = fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint];
									Main.dust[dust].noGravity = true;
									Main.dust[dust].velocity *= 4f;
                                }
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.FluftrodonJumpDust(-1, Main.myPlayer);
                                }
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.FluftrodonJumpDust(-1, Main.myPlayer);
                                }
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.FluftrodonJumpDust(-1, Main.myPlayer);
                                }
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.FluftrodonJumpDust(-1, Main.myPlayer);
                                }
                                Vector2 velocity = ((Vector2.Normalize(mrPlagueRacesPlayer.mouseWorld - player.Center) * (fluftrodonPlayer.jumpCharge / 2)) * ((0.5f + ((float)player.statLifeMax2 * 0.001f)))) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<FluftrodonConfig>().fluftrodonLeapVelocity]);
								player.velocity = velocity;
								player.fallStart = (int)(player.position.Y / 16f);
								player.AddBuff(BuffType<Airborne>(), 6000);
								fluftrodonPlayer.jumpCharge = 0;
							}
						}
                    }
					if (ModContent.GetInstance<FluftrodonConfig>().fluftrodonWallJump)
					{
						if (player.controlJump && (player.controlLeft || player.controlRight) && fluftrodonPlayer.isNextToWall() && player.velocity.X == 0 && player.velocity.Y != 0 && fluftrodonPlayer.canWallJump && !player.stoned && !player.frozen && !player.webbed && !player.shimmering)
						{
							SoundEngine.PlaySound(SoundID.Item152, player.Center);
							SoundEngine.PlaySound(SoundID.Item39, player.Center);
                            if (player.whoAmI == Main.myPlayer)
                            {
                                mrPlagueRacesPlayer.FluftrodonJumpSound(-1, Main.myPlayer);
                            }
                            player.fallStart = (int)(player.position.Y / 16f);
                            for (int i = 0; i < 5; i++)
							{
								int dust = Dust.NewDust(player.position, player.width, player.height, 264);
								Main.dust[dust].color = fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint];
								Main.dust[dust].noGravity = true;
								Main.dust[dust].velocity *= 5f;
								dust = Dust.NewDust(player.position, player.width, player.height, 264);
								Main.dust[dust].color = fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint];
								Main.dust[dust].noGravity = true;
								Main.dust[dust].velocity *= 4f;
                            }
                            if (player.whoAmI == Main.myPlayer)
                            {
                                mrPlagueRacesPlayer.FluftrodonJumpDust(-1, Main.myPlayer);
                            }
                            player.velocity.Y = (-10f + player.statLifeMax2 / -100) * (int)(1f + StatConfigHelpers.RacialStatPercentageFloatIndex[(int)ModContent.GetInstance<FluftrodonConfig>().fluftrodonWallJumpVelocity]);
							fluftrodonPlayer.canWallJump = false;
						}
						if (!player.controlJump)
						{
							fluftrodonPlayer.canWallJump = true;
						}
					}
				}
			}
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
            if (player.whoAmI == Main.myPlayer)
            {
                ModContent.GetInstance<FluftrodonPaintUISystem>().HideMyUI();
            }
        }

		public override void PreUpdate(Player player) {
			if (player.velocity.Y == 0) {
				player.ClearBuff(BuffType<Airborne>());
			}
        }

        public override void PostUpdate(Player player)
        {
            var fluftrodonPlayer = player.GetModPlayer<FluftrodonPlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					fluftrodonPlayer.SyncPlayer(-1, Main.myPlayer, false);
				}

                var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
                mrPlagueRacesPlayer.SyncRotationsAndOffsets(-1, Main.myPlayer);
            }
        }
    }

	public class FluftrodonPlayer : ModPlayer
	{
		public Color[] paintColor = { new Color(235, 232, 245), new Color(244, 0, 0), new Color(244, 109, 0), new Color(244, 241, 0), new Color(181, 244, 0), new Color(0, 244, 10), new Color(0, 244, 164), new Color(0, 226, 244), new Color(0, 162, 244), new Color(41, 0, 244), new Color(150, 0, 244), new Color(244, 0, 243), new Color(249, 100, 158), new Color(144, 0, 0), new Color(144, 64, 0), new Color(144, 136, 0), new Color(106, 144, 0), new Color(0, 144, 5), new Color(0, 144, 96), new Color(0, 133, 144), new Color(0, 95, 144), new Color(24, 0, 144), new Color(88, 0, 144), new Color(144, 0, 143), new Color(147, 59, 93), new Color(64, 63, 73), new Color(255, 255, 255), new Color(141, 138, 154), new Color(150, 121, 94), new Color(5, 3, 10), new Color(11, 255, 255), new Color(208, 239, 246)};
        public string[] paintName = { "Remove Paint", "Red", "Orange", "Yellow", "Lime", "Green", "Teal", "Cyan", "Sky Blue", "Blue", "Purple", "Violet", "Pink", "Deep Red", "Deep Orange", "Deep Yellow", "Deep Lime", "Deep Green", "Deep Teal", "Deep Cyan", "Deep Sky Blue", "Deep Blue", "Deep Purple", "Deep Violet", "Deep Pink", "Black", "White", "Gray", "Brown", "Shadow", "Negative" };
        public int selectedPaint = 1;
		public float jumpCharge = 0;
		public bool canWallJump = true;
		public bool closeMenu = false;

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.FluftrodonSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(selectedPaint);
			packet.Write(jumpCharge);
			packet.Write(canWallJump);
			packet.Write(closeMenu);

            packet.Send(toWho, fromWho);
        }

        public bool isNextToWall()
        {
            int adjacentToBlocks = 0;
            Vector2 playerLocation = new Vector2(Player.position.X / 16, Player.position.Y / 16);
            for (int i = 0; i < 3; i++)
            {
                Tile myTile1 = Main.tile[(int)playerLocation.X + 2, (int)playerLocation.Y];
                Tile myTile2 = Main.tile[(int)playerLocation.X - 1, (int)playerLocation.Y];
                if (myTile1 != null && Main.tileSolid[myTile1.TileType] && myTile1.HasUnactuatedTile)
                {
                    adjacentToBlocks++;
                }
                if (myTile2 != null && Main.tileSolid[myTile2.TileType] && myTile2.HasUnactuatedTile)
                {
                    adjacentToBlocks++;
                }
                if (playerLocation.Y > 0)
                {
                    playerLocation.Y -= 1;
                }
            }
            return adjacentToBlocks > 0;
        }
    }

    public class FluftrodonConfig : ModConfig
    {
        public static FluftrodonConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Fluftrodon")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> fluftrodonStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%
            [RacialStatType.allDamage] = RacialStatPercentageModifier.n15,
            [RacialStatType.pickSpeed] = RacialStatPercentageModifier.p25,
            [RacialStatType.tileSpeed] = RacialStatPercentageModifier.p50,
            [RacialStatType.blockRange] = RacialStatPercentageModifier.p10
        };
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool fluftrodonAbility1;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool fluftrodonAbility2;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool fluftrodonWallJump;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier fluftrodonLeapVelocity;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier fluftrodonWallJumpVelocity;
    }
}