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
using MrPlagueRaces.Common.UI.States;
using MrPlagueRaces.Common.Systems;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Content.Projectiles;
using MrPlagueRaces.Content.Mounts;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.Races.Lihzahrd
{
	public class Lihzahrd : Race
	{
		public override void Load()
        {
			Description = "Reclusive and highly advanced, Lihzahrds are known for the quality of their contraptions.";
            DisplayName = "[c/8CFF00:Lihzahrd]";
            CensorClothing = false;
			StarterShirt = true;
			StarterPants = true;
			ClothStyle = 4;
			HairColor = new Color(241, 244, 156);
            SkinColor = new Color(216, 255, 93);
			DetailColor = new Color(241, 244, 156);
			EyeColor = new Color(115, 107, 0);
			ShirtColor = new Color(201, 110, 75);
			UnderShirtColor = new Color(137, 161, 214);
        }

        public override void PreRaceChange(Player player) // Called before the player's race is changed
        {
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            if (player.mount.Type == MountType<Crawl>())
            {
                player.mount.Dismount(player);
                if (player.whoAmI == Main.myPlayer)
                {
                    mrPlagueRacesPlayer.SyncDismount(-1, Main.myPlayer);
                }
                player.bodyRotation = 0f;
                player.legRotation = 0f;
                player.headPosition = new Vector2(0, 0);
                player.bodyPosition = new Vector2(0, 0);
                player.legPosition = new Vector2(0, 0);
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && (projectile.type == ProjectileType<BoulderGolem>() || projectile.type == ProjectileType<TetherGolem>() || projectile.type == ProjectileType<BoulderGolem>() || projectile.type == ProjectileType<LaserGolem>() || projectile.type == ProjectileType<BarrierGolem>() || projectile.type == ProjectileType<SpiderGolem>() || projectile.type == ProjectileType<LifeGolem>()) && projectile.owner == player.whoAmI)
                    projectile.Kill();
            }

            if (player.whoAmI == Main.myPlayer)
            {
                ModContent.GetInstance<LihzahrdGolemUISystem>().HideMyUI();
            }
        }

        public override void ResetEffects(Player player)
        {
            RegisterAbilityDescription(ModContent.GetInstance<LihzahrdConfig>().lihzahrdAbility1, $"[c/4DBF60:+] Hold Z to crawl, allowing you to climb up walls and fit into small gaps.");
            RegisterAbilityDescription(ModContent.GetInstance<LihzahrdConfig>().lihzahrdAbility2, $"[c/4DBF60:+] Press X to open a golem placing menu. Scroll to cycle through selected golems. Damage scales with defense.");
            RegisterAbilityDescription(ModContent.GetInstance<LihzahrdConfig>().lihzahrdAbility2, $"[c/4DBF60:+] Press C to desummon all existing golems.");
            RegisterAbilityDescription(ModContent.GetInstance<LihzahrdConfig>().lihzahrdCraftingRecipes, $"[c/4DBF60:+] You can craft lihzahrd bricks and traps.");
            RegisterAbilityDescription(ModContent.GetInstance<LihzahrdConfig>().lihzahrdSunlessWeakness, $"[c/FF3640:-] You lose defense and regeneration outside of the sun.");

            RaceStatDictionary = ModContent.GetInstance<LihzahrdConfig>().lihzahrdStats;
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            var lihzahrdPlayer = player.GetModPlayer<LihzahrdPlayer>();
            if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				/*player.tileSpeed += 0.1f;
				player.pickSpeed -= 0.3f;
				player.GetDamage(DamageClass.Generic) -= 0.5f;*/
				if (player.mount.Type == MountType<Crawl>()) {
					player.noKnockback = true;
                }
                lihzahrdPlayer.ReloadGolems();
            }
        }

        public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var lihzahrdPlayer = player.GetModPlayer<LihzahrdPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead)
                {
					if (ModContent.GetInstance<LihzahrdConfig>().lihzahrdAbility1)
					{
						if (MrPlagueRaces.RaceAbilityKeybind1.Current)
						{
							if (player.mount.Type != MountType<Crawl>())
							{
								player.mount.SetMount(MountType<Crawl>(), player, false);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.SyncCrawl(-1, Main.myPlayer);
                                }
                            }
						}
						else
						{
							if (player.mount.Type == MountType<Crawl>())
							{
								player.mount.Dismount(player);
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    mrPlagueRacesPlayer.SyncDismount(-1, Main.myPlayer);
                                }
                            }
						}
					}

					// {T} Crawling animation code (and fallStart resetting) has been moved over to PostUpdate so it can be implicitly synced.
					// Fallstart won't get set if the game window is out of focus, since ProcessTriggers won't run in that scenario, potentially allowing
					// players to die while crawling by tabbing out of the game and falling.
					if (player.mount.Type == MountType<Crawl>()) {
						if (player.controlUseItem) {
							player.controlUseItem = false;
						}
						if ((player.controlLeft || player.controlRight) && lihzahrdPlayer.isNextToWall() && player.velocity.X == 0 && !player.stoned && !player.frozen && !player.webbed && !player.shimmering) {
							if (player.velocity.Y > 0) {
								player.velocity.Y = -((player.maxRunSpeed / 3) + player.accRunSpeed);
							}
							player.velocity.Y -= 1f;
							if (player.velocity.Y < -((player.maxRunSpeed / 3) + player.accRunSpeed)) {
								player.velocity.Y = -((player.maxRunSpeed / 3) + player.accRunSpeed);
                            }
						}
                    }
					if (ModContent.GetInstance<LihzahrdConfig>().lihzahrdAbility2)
					{
						if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed)
                        {
							if (Main.netMode == NetmodeID.SinglePlayer || ModContent.GetInstance<MrPlagueRacesConfig>().experimentalContent)
							{
								if (player.whoAmI == Main.myPlayer && lihzahrdPlayer.Golems.Length > 0)
								{
									ModContent.GetInstance<LihzahrdGolemUISystem>().ToggleMyUI();
								}
							}
							else
                            {
                                SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, player.Center);
                                Main.NewText("Due to syncing issues, Lihzahrd Golems are disabled in multiplayer. To enable Lihzahrd Golems in multiplayer, toggle Experimental Content in the Adjustments (General) Config.", 255, 54, 64);
                            }
                        }
						if (MrPlagueRaces.RaceAbilityKeybind3.JustPressed)
						{
							for (int i = 0; i < Main.maxProjectiles; i++)
							{
								Projectile projectile = Main.projectile[i];
								if (projectile.active && (projectile.type == ProjectileType<BoulderGolem>() || projectile.type == ProjectileType<TetherGolem>() || projectile.type == ProjectileType<BoulderGolem>() || projectile.type == ProjectileType<LaserGolem>() || projectile.type == ProjectileType<BarrierGolem>() || projectile.type == ProjectileType<SpiderGolem>() || projectile.type == ProjectileType<LifeGolem>()) && projectile.owner == player.whoAmI)
									projectile.Kill();
							}
						}
					}
                    if (player.controlLeft) {
						lihzahrdPlayer.direction = -1;
					}
					if (player.controlRight) {
						lihzahrdPlayer.direction = 1;
					}
				}
			}
		}
		public override void PreUpdate(Player player) {
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var lihzahrdPlayer = player.GetModPlayer<LihzahrdPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats)
            {
				if (ModContent.GetInstance<LihzahrdConfig>().lihzahrdSunlessWeakness)
				{
					if (!lihzahrdPlayer.ExposedToSun())
					{
						player.AddBuff(BuffType<Sluggish>(), 2);
					}
				}
			}
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) 
		{
            var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
            if (player.mount.Type == MountType<Crawl>())
            {
                player.mount.Dismount(player);
				if (player.whoAmI == Main.myPlayer)
				{
					mrPlagueRacesPlayer.SyncDismount(-1, Main.myPlayer);
				}
                player.bodyRotation = 0f;
                player.legRotation = 0f;
                player.headPosition = new Vector2(0, 0);
                player.bodyPosition = new Vector2(0, 0);
                player.legPosition = new Vector2(0, 0);
            }
            if (player.whoAmI == Main.myPlayer)
            {
                ModContent.GetInstance<LihzahrdGolemUISystem>().HideMyUI();
            }
        }

		public override void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var lihzahrdPlayer = player.GetModPlayer<LihzahrdPlayer>();
			if (ModContent.GetInstance<MrPlagueRacesConfig>().raceStats) {
				if (!player.dead) {
					if (player.mount.Type == MountType<Crawl>()) {
						player.fullRotationOrigin = new Vector2((player.width / 2), (player.height / 2));
						player.bodyRotation = (player.direction == 1 ? 1.575f : -1.575f);
						player.legRotation = (player.direction == 1 ? 1.575f : -1.575f);
						if (player.bodyFrame.Y == player.bodyFrame.Height * 0 || player.bodyFrame.Y == player.bodyFrame.Height * 5) {
							player.bodyFrame.Y = player.bodyFrame.Height * 14;
							player.legFrame.Y = player.legFrame.Height * 6;
						}
						player.headPosition = new Vector2(player.direction == 1 ? 12 : -12, 25);
						player.bodyPosition = new Vector2(player.direction == 1 ? 9 : -9, 21);
						player.legPosition = new Vector2(player.direction == 1 ? -5 : 5, 7);
						if (player.velocity.Y < 0 && (player.controlLeft || player.controlRight) && lihzahrdPlayer.isNextToWall() && player.velocity.X == 0) {
							player.fullRotation = player.velocity.Y * (float)player.direction * 1f;
							if ((double)player.fullRotation < -1.575f)
							{
								player.fullRotation = -1.575f;
							}
							if ((double)player.fullRotation > 1.575f)
							{
								player.fullRotation = 1.575f;
							}
						}
						else if (player.velocity.Y < 0 && !((player.controlLeft || player.controlRight) && lihzahrdPlayer.isNextToWall() && player.velocity.X == 0)) {
							player.fullRotation = player.velocity.Y * (float)player.direction * 0.5f;
							if ((double)player.fullRotation < -0.78f)
							{
								player.fullRotation = -0.78f;
							}
							if ((double)player.fullRotation > 0.78f)
							{
								player.fullRotation = 0.78f;
							}
						}
						else if (!player.sleeping.isSleeping) {
							player.fullRotation = 0f;
						}
					}
					else if (!player.sleeping.isSleeping) {
						player.bodyRotation = 0f;
						player.legRotation = 0f;
						player.headPosition = new Vector2(0, 0);
						player.bodyPosition = new Vector2(0, 0);
						player.legPosition = new Vector2(0, 0);
                    }
                }
            }
        }

        public override void PostUpdate(Player player)
        {
            var lihzahrdPlayer = player.GetModPlayer<LihzahrdPlayer>();

            // {T} Crawling code moved over from ProcessTriggers
            if (player.mount.Type == MountType<Crawl>())
            {
                if ((player.controlLeft || player.controlRight) && lihzahrdPlayer.isNextToWall() && player.velocity.X == 0 && !player.stoned && !player.frozen && !player.webbed && !player.shimmering)
                {
                    player.fallStart = (int)(player.position.Y / 16f);
                    lihzahrdPlayer.crawlFrameCounter++;
                    if (lihzahrdPlayer.crawlFrameCounter > 4)
                    {
                        lihzahrdPlayer.crawlFrame++;
                        lihzahrdPlayer.crawlFrameCounter = 0;
                        if (lihzahrdPlayer.crawlFrame >= 6)
                        {
                            lihzahrdPlayer.crawlFrame = 0;
                        }
                    }
                    lihzahrdPlayer.legFrameCounter++;
                    if (lihzahrdPlayer.legFrameCounter > 4)
                    {
                        lihzahrdPlayer.legFrame++;
                        lihzahrdPlayer.legFrameCounter = 0;
                        if (lihzahrdPlayer.legFrame >= 20)
                        {
                            lihzahrdPlayer.legFrame = 6;
                        }
                    }
                }
                else
                {
                    lihzahrdPlayer.crawlFrame = -1;
                    lihzahrdPlayer.crawlFrameCounter = 0;
                    lihzahrdPlayer.legFrame = 6;
                    lihzahrdPlayer.legFrameCounter = 0;
                }
            }


            if (player.whoAmI == Main.myPlayer)
            {
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					lihzahrdPlayer.SyncPlayer(-1, Main.myPlayer, false);
				}

                var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
                mrPlagueRacesPlayer.SyncRotationsAndOffsets(-1, Main.myPlayer);
            }
        }
    }

	public enum LihzahrdGolemType
	{
		BoulderGolem,
		TetherGolem,
		LaserGolem,
		BarrierGolem,
		SpiderGolem,
		LifeGolem
	}


	public class LihzahrdPlayer : ModPlayer
	{
        public int[] Golems = { ProjectileType<BoulderGolem>(), ProjectileType<TetherGolem>(), ProjectileType<LaserGolem>(), ProjectileType<BarrierGolem>(), ProjectileType<SpiderGolem>(), ProjectileType<LifeGolem>() };
        public string[] GolemNames = { "Boulder Golem", "Tether Golem", "Laser Golem", "Barrier Golem", "Spider Golem", "Life Golem" };
        public Asset<Texture2D>[] GolemIcons = { ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_BoulderGolem"), ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_TetherGolem"), ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_LaserGolem"), ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_BarrierGolem"), ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_SpiderGolem"), ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_LifeGolem"), };
        public float fullRotation;
		public float targetFullRotation;
		public float headRotation;
		public float targetHeadRotation;
		public int crawlFrame;
		public int crawlFrameCounter;
		public int legFrame;
		public int legFrameCounter;
		public int selectedGolem = ProjectileType<BoulderGolem>();
        public int selectedGolemIndex = 0;
        public int direction;
		public bool closeMenu;

		public int boulderGolemID;
		public int tetherGolemID;
        public int laserGolemID;
        public int barrierGolemID;
		public int spiderGolemID;
		public int lifeGolemID;

		public int boulderGolemLeftBoulderID;
        public int boulderGolemRightBoulderID;

        public int tetherGolemLeftTetherID;
        public int tetherGolemRightTetherID;
        public int tetherGolemMiddleTetherID;

		public int laserGolemLeftGripperID;
        public int laserGolemRightGripperID;
        public int laserGolemLaserID;

		public int barrierGolemLeftBarrierID;
        public int barrierGolemRightBarrierID;

		public int spiderGolemLeftGripperID;
        public int spiderGolemRightGripperID;

        public void ReloadGolems()
		{
			var golemList = new List<int>();
            var golemNamesList = new List<string>();
            var golemIconsList = new List<Asset<Texture2D>>();
            if (ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.BoulderGolem))
            {
                golemList.Add(ProjectileType<BoulderGolem>());
				golemNamesList.Add("Boulder Golem");
				golemIconsList.Add(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_BoulderGolem"));
            }
            if (ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.TetherGolem))
            {
                golemList.Add(ProjectileType<TetherGolem>());
                golemNamesList.Add("Tether Golem");
                golemIconsList.Add(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_TetherGolem"));
            }
            if (ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.LaserGolem))
            {
                golemList.Add(ProjectileType<LaserGolem>());
                golemNamesList.Add("Laser Golem");
                golemIconsList.Add(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_LaserGolem"));
            }
            if (ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.BarrierGolem))
            {
                golemList.Add(ProjectileType<BarrierGolem>());
                golemNamesList.Add("Barrier Golem");
                golemIconsList.Add(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_BarrierGolem"));
            }
            if (ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.SpiderGolem))
            {
                golemList.Add(ProjectileType<SpiderGolem>());
                golemNamesList.Add("Spider Golem");
                golemIconsList.Add(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_SpiderGolem"));
            }
            if (ModContent.GetInstance<LihzahrdConfig>().lihzahrdGolems.ContainsKey(LihzahrdGolemType.LifeGolem))
            {
                golemList.Add(ProjectileType<LifeGolem>());
                golemNamesList.Add("Life Golem");
                golemIconsList.Add(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/UI_LifeGolem"));
            }
            Golems = golemList.ToArray();
            GolemNames = golemNamesList.ToArray();
			GolemIcons = golemIconsList.ToArray();
        }


		// {T} I deleted a bunch of values from here corresponding to the crawling animation, since it's now handled locally on clients.
		// What I would call 'implicitly synced', now.
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.LihzahrdSyncPlayer);
			packet.Write((byte)Player.whoAmI);
            packet.Write(direction);

			/*packet.Write(boulderGolemID);
			packet.Write(tetherGolemID);
			packet.Write(laserGolemID);
			packet.Write(barrierGolemID);
			packet.Write(spiderGolemID);
			packet.Write(lifeGolemID);

			packet.Write(boulderGolemLeftBoulderID);
			packet.Write(boulderGolemRightBoulderID);

			packet.Write(tetherGolemLeftTetherID);
			packet.Write(tetherGolemRightTetherID);
			packet.Write(tetherGolemMiddleTetherID);

			packet.Write(laserGolemLeftGripperID);
			packet.Write(laserGolemRightGripperID);
			packet.Write(laserGolemLaserID);

			packet.Write(barrierGolemLeftBarrierID);
			packet.Write(barrierGolemRightBarrierID);

			packet.Write(spiderGolemLeftGripperID);
			packet.Write(spiderGolemRightGripperID);*/

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

        public bool ExposedToSun()
		{
			Tile[] wallTiles = new Tile[6];
			Point playerTilePoint = (Main.LocalPlayer.position / 16).ToPoint();
			wallTiles[0] = Framing.GetTileSafely(playerTilePoint.X, playerTilePoint.Y);
			wallTiles[1] = Framing.GetTileSafely(playerTilePoint.X, playerTilePoint.Y + 1);
			wallTiles[2] = Framing.GetTileSafely(playerTilePoint.X, playerTilePoint.Y + 2);
			wallTiles[3] = Framing.GetTileSafely(playerTilePoint.X + 1, playerTilePoint.Y);
			wallTiles[4] = Framing.GetTileSafely(playerTilePoint.X + 1, playerTilePoint.Y + 1);
			wallTiles[5] = Framing.GetTileSafely(playerTilePoint.X + 1, playerTilePoint.Y + 2);
			bool behindWall = false;
			foreach (var tile in wallTiles)
			{
				if (tile.WallType > 0)
				{
					behindWall = true;
				}
				else
				{
					behindWall = false;
					break;
				}
			}
			Tile[] largeWallTiles = new Tile[36];
			Point playerTilePointLarge = (Main.LocalPlayer.position / 16).ToPoint();
			largeWallTiles[0] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 15);
			largeWallTiles[1] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 14);
			largeWallTiles[2] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 13);
			largeWallTiles[3] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 12);
			largeWallTiles[4] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 11);
			largeWallTiles[5] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 10);
			largeWallTiles[6] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 9);
			largeWallTiles[7] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 8);
			largeWallTiles[8] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 7);
			largeWallTiles[9] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 6);
			largeWallTiles[10] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 5);
			largeWallTiles[11] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 4);
			largeWallTiles[12] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 3);
			largeWallTiles[13] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 2);
			largeWallTiles[14] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y - 1);
			largeWallTiles[15] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y);
			largeWallTiles[16] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y + 1);
			largeWallTiles[17] = Framing.GetTileSafely(playerTilePointLarge.X, playerTilePointLarge.Y + 2);
			largeWallTiles[18] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 15);
			largeWallTiles[19] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 14);
			largeWallTiles[20] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 13);
			largeWallTiles[21] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 12);
			largeWallTiles[22] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 11);
			largeWallTiles[23] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 10);
			largeWallTiles[24] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 9);
			largeWallTiles[25] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 8);
			largeWallTiles[26] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 7);
			largeWallTiles[27] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 6);
			largeWallTiles[28] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 5);
			largeWallTiles[29] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 4);
			largeWallTiles[30] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 3);
			largeWallTiles[31] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 2);
			largeWallTiles[32] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y - 1);
			largeWallTiles[33] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y);
			largeWallTiles[34] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y + 1);
			largeWallTiles[35] = Framing.GetTileSafely(playerTilePointLarge.X + 1, playerTilePointLarge.Y + 2);
			bool behindLargeWall = false;
			foreach (var tile in largeWallTiles)
			{
				if (tile.WallType > 0)
				{
					behindLargeWall = true;
				}
				else
				{
					behindLargeWall = false;
					break;
				}
			}
			bool hasCeilingTile = false;
			Vector2 playerLocation = new Vector2(Player.Center.X / 16, Player.Center.Y / 16);
			for (int i = 0; i < 60; i++)
			{
				Tile ceilingTile = Main.tile[(int)playerLocation.X, (int)playerLocation.Y];
				if (ceilingTile != null && Main.tileSolid[ceilingTile.TileType] && ceilingTile.HasUnactuatedTile)
				{
					hasCeilingTile = true;
				}
				if (playerLocation.Y > 0)
				{
					playerLocation.Y -= 1;
				}
			}
			bool hasCeilingAbove = false;
			if (behindLargeWall || hasCeilingTile)
			{
				hasCeilingAbove = true;
			}
			else
			{
				hasCeilingAbove = false;
			}
			return (!hasCeilingAbove || !behindWall) && !((double)Player.Center.Y > Main.worldSurface * 16.0) && Main.dayTime && !(Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir));
		}
	}

	public class LihzahrdRecipes : ModSystem
	{
		public override void AddRecipes() {
			Recipe.Create(ItemID.LihzahrdBrick, 10)
				.AddIngredient(ItemID.MudstoneBlock, 10)
				.AddIngredient(ItemID.ChlorophyteOre, 1)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(Language.GetOrRegister("Mods.MrPlagueRaces.Conditions.LowHealth"), () => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd && ModContent.GetInstance<MrPlagueRacesConfig>().raceStats && ModContent.GetInstance<LihzahrdConfig>().lihzahrdCraftingRecipes)
				.Register();
			Recipe.Create(ItemID.SuperDartTrap, 1)
				.AddIngredient(ItemID.DartTrap, 1)
				.AddIngredient(ItemID.LihzahrdBrick, 5)
				.AddIngredient(ItemID.JungleSpores, 3)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(Language.GetOrRegister("Mods.MrPlagueRaces.Conditions.LowHealth"), () => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd && ModContent.GetInstance<MrPlagueRacesConfig>().raceStats && ModContent.GetInstance<LihzahrdConfig>().lihzahrdCraftingRecipes)
				.Register();
			Recipe.Create(ItemID.SpikyBallTrap, 1)
				.AddIngredient(ItemID.GeyserTrap, 1)
				.AddIngredient(ItemID.LihzahrdBrick, 5)
				.AddIngredient(ItemID.JungleSpores, 3)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(Language.GetOrRegister("Mods.MrPlagueRaces.Conditions.LowHealth"), () => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd && ModContent.GetInstance<MrPlagueRacesConfig>().raceStats && ModContent.GetInstance<LihzahrdConfig>().lihzahrdCraftingRecipes)
				.Register();
			Recipe.Create(ItemID.FlameTrap, 1)
				.AddIngredient(ItemID.DartTrap, 1)
				.AddIngredient(ItemID.LihzahrdBrick, 5)
				.AddIngredient(ItemID.Moonglow, 3)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(Language.GetOrRegister("Mods.MrPlagueRaces.Conditions.LowHealth"), () => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd && ModContent.GetInstance<MrPlagueRacesConfig>().raceStats && ModContent.GetInstance<LihzahrdConfig>().lihzahrdCraftingRecipes)
				.Register();
			Recipe.Create(ItemID.SpearTrap, 1)
				.AddIngredient(ItemID.GeyserTrap, 1)
				.AddIngredient(ItemID.LihzahrdBrick, 5)
				.AddIngredient(ItemID.Moonglow, 3)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(Language.GetOrRegister("Mods.MrPlagueRaces.Conditions.LowHealth"), () => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd && ModContent.GetInstance<MrPlagueRacesConfig>().raceStats && ModContent.GetInstance<LihzahrdConfig>().lihzahrdCraftingRecipes)
				.Register();
			Recipe.Create(ItemID.DartTrap, 1)
				.AddIngredient(ItemID.StoneBlock, 15)
				.AddIngredient(ItemID.JungleSpores, 3)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(Language.GetOrRegister("Mods.MrPlagueRaces.Conditions.LowHealth"), () => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd && ModContent.GetInstance<MrPlagueRacesConfig>().raceStats && ModContent.GetInstance<LihzahrdConfig>().lihzahrdCraftingRecipes)
				.Register();
			Recipe.Create(ItemID.GeyserTrap, 1)
				.AddIngredient(ItemID.StoneBlock, 15)
				.AddIngredient(ItemID.Moonglow, 3)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(Language.GetOrRegister("Mods.MrPlagueRaces.Conditions.LowHealth"), () => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd && ModContent.GetInstance<MrPlagueRacesConfig>().raceStats && ModContent.GetInstance<LihzahrdConfig>().lihzahrdCraftingRecipes)
				.Register();
		}
    }

    public class LihzahrdConfig : ModConfig
    {
        public static LihzahrdConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Lihzahrd")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> lihzahrdStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%
            [RacialStatType.allDamage] = RacialStatPercentageModifier.n15,
            [RacialStatType.pickSpeed] = RacialStatPercentageModifier.p30,
            [RacialStatType.tileSpeed] = RacialStatPercentageModifier.p10
        };
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool lihzahrdAbility1;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool lihzahrdAbility2;
        [DefaultValue(RacialStatPercentageModifier.zero)]
        [BackgroundColor(110, 141, 255)]
        public RacialStatPercentageModifier lihzahrdCrawlVelocity;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool lihzahrdSunlessWeakness;
        [DefaultValue(true)]
        [BackgroundColor(110, 141, 255)]
        public bool lihzahrdCraftingRecipes;
		[DefaultValue(true)]
		[BackgroundColor(110, 141, 255)]
		public bool lihzahrdDisableUIAfterPlacement;

        [BackgroundColor(110, 141, 255)]
		public Dictionary<LihzahrdGolemType, RacialStatPercentageModifier> lihzahrdGolems = new Dictionary<LihzahrdGolemType, RacialStatPercentageModifier>()
		{
			[LihzahrdGolemType.BoulderGolem] = RacialStatPercentageModifier.zero,
			[LihzahrdGolemType.TetherGolem] = RacialStatPercentageModifier.zero,
			[LihzahrdGolemType.LaserGolem] = RacialStatPercentageModifier.zero,
			[LihzahrdGolemType.BarrierGolem] = RacialStatPercentageModifier.zero,
			[LihzahrdGolemType.SpiderGolem] = RacialStatPercentageModifier.zero,
			[LihzahrdGolemType.LifeGolem] = RacialStatPercentageModifier.zero
		};
    }
}