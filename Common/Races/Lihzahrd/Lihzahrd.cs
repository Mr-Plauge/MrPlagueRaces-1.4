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
using MrPlagueRaces.Common.UI.States;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Content.Mounts;
using MrPlagueRaces.Content.Projectiles;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.Races.Lihzahrd
{
	public class Lihzahrd : Race
	{
		public override void Load()
        {
			Description = "Reclusive and highly advanced, Lihzahrds are known for the quality of their creations.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z to open a golem placing menu. Scroll to cycle through selected golems. Damage scales with defense.\n[c/4DBF60:{"+"}] Hold X to crawl, allowing you to climb up walls and fit into small gaps.\n[c/FF3640:{"-"}] You lose defense and regeneration outside of the sun.";
			CensorClothing = false;
			StarterShirt = true;
			StarterPants = true;
			ClothStyle = 4;
			HairColor = new Color(216, 255, 93);
			SkinColor = new Color(216, 255, 93);
			DetailColor = new Color(241, 244, 156);
			EyeColor = new Color(115, 107, 0);
			ShirtColor = new Color(201, 110, 75);
			UnderShirtColor = new Color(137, 161, 214);
		}

		public override void ResetEffects(Player player)
		{
			player.tileSpeed += 0.1f;
			player.pickSpeed -= 0.3f;
			player.GetDamage(DamageClass.Generic).Base -= 5f;
			if (player.mount.Type == MountType<Crawl>()) {
				player.noKnockback = true;
			}
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var lihzahrdPlayer = player.GetModPlayer<LihzahrdPlayer>();
			if ((PlayerInput.MouseInfo.ScrollWheelValue - PlayerInput.MouseInfoOld.ScrollWheelValue) > 0 && player.ownedProjectileCounts[ProjectileType<GolemSelect>()] != 0) {
				lihzahrdPlayer.selectedGolem -= 1;
				if (lihzahrdPlayer.selectedGolem < 0) {
					lihzahrdPlayer.selectedGolem = 5;
				}
			}
			if ((PlayerInput.MouseInfo.ScrollWheelValue - PlayerInput.MouseInfoOld.ScrollWheelValue) < 0 && player.ownedProjectileCounts[ProjectileType<GolemSelect>()] != 0) {
				lihzahrdPlayer.selectedGolem += 1;
				if (lihzahrdPlayer.selectedGolem > 5) {
					lihzahrdPlayer.selectedGolem = 0;
				}
					
			}
			if (player.ownedProjectileCounts[ProjectileType<GolemSelect>()] != 0) {
				if (player.controlUseItem) {
					player.controlUseItem = false;
				}
			}
			if (!player.dead)
			{
				if (MrPlagueRaces.RaceAbilityKeybind1.Current)
				{
					if (player.mount.Type != MountType<Crawl>()) {
						player.mount.SetMount(MountType<Crawl>(), player, false);
					}
				}
				else
				{
					if (player.mount.Type == MountType<Crawl>()) {
						player.mount.Dismount(player);
					}
				}
				if (player.mount.Type == MountType<Crawl>()) {
					if (player.controlUseItem) {
						player.controlUseItem = false;
					}
					if ((player.controlLeft || player.controlRight) && player.velocity.X == 0) {
						if (player.velocity.Y > 0) {
							player.velocity.Y = -((player.maxRunSpeed / 3) + player.accRunSpeed);
						}
						player.velocity.Y -= 1f;
						if (player.velocity.Y < -((player.maxRunSpeed / 3) + player.accRunSpeed)) {
							player.velocity.Y = -((player.maxRunSpeed / 3) + player.accRunSpeed);
						}
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
					else {
						lihzahrdPlayer.crawlFrame = -1;
						lihzahrdPlayer.crawlFrameCounter = 0;
						lihzahrdPlayer.legFrame = 6;
						lihzahrdPlayer.legFrameCounter = 0;
					}
				}
				if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed)
				{
					if (player.ownedProjectileCounts[ProjectileType<GolemSelect>()] == 0) {
						Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, 0, 0, ProjectileType<GolemSelect>(), 0, 0, player.whoAmI);
					}
					else {
						lihzahrdPlayer.closeMenu = true;
					}
				}
				if (player.controlLeft) {
					lihzahrdPlayer.direction = -1;
				}
				if (player.controlRight) {
					lihzahrdPlayer.direction = 1;
				}
				if (Main.mouseLeft && Main.mouseLeftRelease && player.ownedProjectileCounts[ProjectileType<GolemSelect>()] != 0) {
					switch (lihzahrdPlayer.selectedGolem)
					{
						case 0:
							for (int i = 0; i < Main.maxProjectiles; i++) {
								Projectile projectile = Main.projectile[i];
								if (projectile.active && projectile.type == ProjectileType<BoulderGolem>() && projectile.owner == player.whoAmI)
									projectile.Kill();
							}
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Main.MouseWorld.X - 22, Main.MouseWorld.Y - 42, 0, 0, ProjectileType<BoulderGolem>(), 0, 0, player.whoAmI);
							break;
						case 1:
							for (int i = 0; i < Main.maxProjectiles; i++) {
								Projectile projectile = Main.projectile[i];
								if (projectile.active && projectile.type == ProjectileType<TetherGolem>() && projectile.owner == player.whoAmI)
									projectile.Kill();
							}
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Main.MouseWorld.X - 22, Main.MouseWorld.Y - 42, 0, 0, ProjectileType<TetherGolem>(), 0, 0, player.whoAmI);
							break;
						case 2:
							for (int i = 0; i < Main.maxProjectiles; i++) {
								Projectile projectile = Main.projectile[i];
								if (projectile.active && projectile.type == ProjectileType<LaserGolem>() && projectile.owner == player.whoAmI)
									projectile.Kill();
							}
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Main.MouseWorld.X - 22, Main.MouseWorld.Y - 42, 0, 0, ProjectileType<LaserGolem>(), 0, 0, player.whoAmI);
							break;
						case 3:
							for (int i = 0; i < Main.maxProjectiles; i++) {
								Projectile projectile = Main.projectile[i];
								if (projectile.active && projectile.type == ProjectileType<BarrierGolem>() && projectile.owner == player.whoAmI)
									projectile.Kill();
							}
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Main.MouseWorld.X - 22, Main.MouseWorld.Y - 42, 0, 0, ProjectileType<BarrierGolem>(), 0, 0, player.whoAmI);
							break;
						case 4:
							for (int i = 0; i < Main.maxProjectiles; i++) {
								Projectile projectile = Main.projectile[i];
								if (projectile.active && projectile.type == ProjectileType<SpiderGolem>() && projectile.owner == player.whoAmI)
									projectile.Kill();
							}
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Main.MouseWorld.X - 22, Main.MouseWorld.Y - 42, 0, 0, ProjectileType<SpiderGolem>(), 0, 0, player.whoAmI);
							break;
						default:
							for (int i = 0; i < Main.maxProjectiles; i++) {
								Projectile projectile = Main.projectile[i];
								if (projectile.active && projectile.type == ProjectileType<LifeGolem>() && projectile.owner == player.whoAmI)
									projectile.Kill();
							}
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Main.MouseWorld.X - 22, Main.MouseWorld.Y - 42, 0, 0, ProjectileType<LifeGolem>(), 0, 0, player.whoAmI);
							break;
					}
				}
			}
		}
		public override void PreUpdate(Player player) {
			var lihzahrdPlayer = player.GetModPlayer<LihzahrdPlayer>();
			if (!lihzahrdPlayer.ExposedToSun()) {
				player.AddBuff(BuffType<Sluggish>(), 2);
			}
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
			for (int i = 0; i < Main.maxProjectiles; i++) {
				Projectile projectile = Main.projectile[i];
				if (projectile.active && projectile.type == ProjectileType<GolemSelect>() && projectile.owner == player.whoAmI)
					projectile.Kill();
			}
		}

		public override void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var lihzahrdPlayer = player.GetModPlayer<LihzahrdPlayer>();
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
					if (player.velocity.Y < 0 && (player.controlLeft || player.controlRight) && player.velocity.X == 0) {
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
					else if (player.velocity.Y < 0 && !((player.controlLeft || player.controlRight) && player.velocity.X == 0)) {
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

	public class LihzahrdPlayer : ModPlayer
	{
		public float fullRotation;
		public float targetFullRotation;
		public float headRotation;
		public float targetHeadRotation;
		public int crawlFrame;
		public int crawlFrameCounter;
		public int legFrame;
		public int legFrameCounter;
		public int selectedGolem;
		public int direction;
		public bool closeMenu;

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.LihzahrdSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(fullRotation);
			packet.Write(targetFullRotation);
			packet.Write(headRotation);
			packet.Write(targetHeadRotation);
			packet.Write(crawlFrame);
			packet.Write(crawlFrameCounter);
			packet.Write(legFrame);
			packet.Write(legFrameCounter);
			packet.Write(selectedGolem);
			packet.Write(direction);
			packet.Write(closeMenu);
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
				.AddCondition(NetworkText.FromKey("RecipeConditions.LowHealth"), r => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd)
				.Register();
			Recipe.Create(ItemID.SuperDartTrap, 1)
				.AddIngredient(ItemID.DartTrap, 1)
				.AddIngredient(ItemID.LihzahrdBrick, 5)
				.AddIngredient(ItemID.JungleSpores, 3)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(NetworkText.FromKey("RecipeConditions.LowHealth"), r => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd)
				.Register();
			Recipe.Create(ItemID.SpikyBallTrap, 1)
				.AddIngredient(ItemID.GeyserTrap, 1)
				.AddIngredient(ItemID.LihzahrdBrick, 5)
				.AddIngredient(ItemID.JungleSpores, 3)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(NetworkText.FromKey("RecipeConditions.LowHealth"), r => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd)
				.Register();
			Recipe.Create(ItemID.FlameTrap, 1)
				.AddIngredient(ItemID.DartTrap, 1)
				.AddIngredient(ItemID.LihzahrdBrick, 5)
				.AddIngredient(ItemID.Moonglow, 3)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(NetworkText.FromKey("RecipeConditions.LowHealth"), r => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd)
				.Register();
			Recipe.Create(ItemID.SpearTrap, 1)
				.AddIngredient(ItemID.GeyserTrap, 1)
				.AddIngredient(ItemID.LihzahrdBrick, 5)
				.AddIngredient(ItemID.Moonglow, 3)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(NetworkText.FromKey("RecipeConditions.LowHealth"), r => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd)
				.Register();
			Recipe.Create(ItemID.DartTrap, 1)
				.AddIngredient(ItemID.StoneBlock, 15)
				.AddIngredient(ItemID.JungleSpores, 3)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(NetworkText.FromKey("RecipeConditions.LowHealth"), r => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd)
				.Register();
			Recipe.Create(ItemID.GeyserTrap, 1)
				.AddIngredient(ItemID.StoneBlock, 15)
				.AddIngredient(ItemID.Moonglow, 3)
				.AddTile(TileID.LihzahrdFurnace)
				.AddCondition(NetworkText.FromKey("RecipeConditions.LowHealth"), r => Main.LocalPlayer.GetModPlayer<MrPlagueRacesPlayer>().race is Lihzahrd)
				.Register();
		}
	}
}
