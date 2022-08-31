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
using MrPlagueRaces.Content.Mounts;
using MrPlagueRaces.Content.Projectiles;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.Races.Vampire
{
	public class Vampire : Race
	{
		public override void Load()
        {
			Description = "Naturally gifted with a form of soul magic, Vampires can morph into a bat-like state.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z to transform into a bat. You enter stealth after a few seconds, making enemies ignore you.\n[c/4DBF60:{"+"}] Press X while in batform to release your abnormally long tongue, which latches onto enemies and drains their health.\n[c/FF3640:{"-"}] You cannot use healing potions.\n[c/FF3640:{"-"}] Getting attacked in sunlight burns you.";
			CensorClothing = false;
			HairColor = new Color(91, 86, 94);
			SkinColor = new Color(91, 86, 94);
			DetailColor = new Color(175, 165, 140);
			EyeColor = new Color(255, 81, 81);
		}

		public override void ResetEffects(Player player)
		{
			player.moveSpeed += 0.1f;
			if (player.mount.Type == MountType<StealthBat>()) {
				player.endurance -= 0.5f;
			}
			else {
				player.endurance -= 0.15f;
			}
			player.statLifeMax2 -= (player.statLifeMax2 / 4);
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var vampirePlayer = player.GetModPlayer<VampirePlayer>();
			if (!player.dead)
			{
				if (MrPlagueRaces.RaceAbilityKeybind1.JustPressed)
				{
					if (player.mount.Type != MountType<StealthBat>()) {
						player.mount.SetMount(MountType<StealthBat>(), player, false);
						SoundEngine.PlaySound(SoundID.AbigailUpgrade, player.Center);
						int num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(player.position.X, player.position.Y - 10f), player.velocity, 99);
						Main.gore[num].velocity *= 0.3f;
						num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(player.position.X, player.position.Y + (float)(player.height / 2) - 10f), player.velocity, 99);
						Main.gore[num].velocity *= 0.3f;
						num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(player.position.X, player.position.Y + (float)player.height - 10f), player.velocity, 99);
						Main.gore[num].velocity *= 0.3f;
					}
					else {
						player.mount.Dismount(player);
						SoundEngine.PlaySound(SoundID.AbigailAttack, player.Center);
						int num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(player.position.X, player.position.Y - 10f), player.velocity, 99);
						Main.gore[num].velocity *= 0.3f;
						num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(player.position.X, player.position.Y + (float)(player.height / 2) - 10f), player.velocity, 99);
						Main.gore[num].velocity *= 0.3f;
						num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(player.position.X, player.position.Y + (float)player.height - 10f), player.velocity, 99);
						Main.gore[num].velocity *= 0.3f;
					}
				}
				if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed && player.mount.Type == MountType<StealthBat>())
				{
					Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * 10f;
					if (player.ownedProjectileCounts[ProjectileType<LeechTongue>()] == 0) {
						SoundEngine.PlaySound(SoundID.Item111, player.Center);
						SoundEngine.PlaySound(SoundID.Item171, player.Center);
						vampirePlayer.LeechTongue = Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, velocity.X, velocity.Y, ProjectileType<LeechTongue>(), 1, 0, player.whoAmI);
					}
					else {
						Main.projectile[vampirePlayer.LeechTongue].ai[0] = 60;
					}
				}
				if (player.mount.Type == MountType<StealthBat>()) {
					if (player.controlUseItem) {
						player.controlUseItem = false;
					}
				}
			}
		}

		public override void UpdateBadLifeRegen(Player player) {
			if (player.lifeRegen > 0)
				player.lifeRegen = 0;
			player.lifeRegenTime = 0;
		}

		public override bool CanUseItem(Player player, Item item)
		{
			if (item.healLife > 0) {
				return false;
			}
			else {
				return true;
			}
		}

		public override void HideDrawLayers(Player player, PlayerDrawSet drawInfo) {
			if (player.mount.Type == MountType<StealthBat>())
			{
				foreach (var layer in PlayerDrawLayerLoader.Layers)
				{
					if (layer != PlayerDrawLayers.MountBack && layer != PlayerDrawLayers.MountFront)
					{
						layer.Hide();
					}
				}
			}
		}

		public override void PreUpdate(Player player) {
			var vampirePlayer = player.GetModPlayer<VampirePlayer>();
			if (!player.dead)
			{
				if (player.mount.Type == MountType<StealthBat>() && !vampirePlayer.Leeching) {
					if (vampirePlayer.stealthTimer < 420) {
						vampirePlayer.stealthTimer++;
					}
					if (vampirePlayer.stealthTimer == 420) {
						if (!player.HasBuff(BuffType<Unseen>())) {
							SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse, player.Center);
						}
						player.AddBuff(BuffType<Unseen>(), 120);
					}
				}
				else {
					vampirePlayer.stealthTimer = 0;
				}
				if (vampirePlayer.ExposedToSun()) {
					player.AddBuff(BuffType<Photosensitive>(), 2);
				}
			}
		}
	}

	public class VampirePlayer : ModPlayer
	{
		public int stealthTimer;
		public int LeechTongue;
		public bool Leeching;

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.VampireSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(stealthTimer);
			packet.Write(LeechTongue);
			packet.Write(Leeching);
		}

		public bool ExposedToSun()
		{
			Tile[] smallWallTiles = new Tile[2];
			Point playerTilePointSmall = (Main.LocalPlayer.position / 16).ToPoint();
			smallWallTiles[0] = Framing.GetTileSafely(playerTilePointSmall.X, playerTilePointSmall.Y);
			smallWallTiles[1] = Framing.GetTileSafely(playerTilePointSmall.X + 1, playerTilePointSmall.Y);
			bool behindSmallWall = false;
			foreach (var tile in smallWallTiles)
			{
				if (tile.WallType > 0)
				{
					behindSmallWall = true;
				}
				else
				{
					behindSmallWall = false;
					break;
				}
			}
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
			if (Player.mount.Type != MountType<StealthBat>())
			{
				return (!hasCeilingAbove || !behindWall) && !((double)Player.Center.Y > Main.worldSurface * 16.0) && Main.dayTime && !(Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir));
			}
			else
			{
				return (!hasCeilingAbove || !behindSmallWall) && !((double)Player.Center.Y > Main.worldSurface * 16.0) && Main.dayTime && !(Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir));
			}
		}
	}
}
