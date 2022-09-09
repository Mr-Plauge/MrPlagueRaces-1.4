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

namespace MrPlagueRaces.Common.Races.Kobold
{
	public class Kobold : Race
	{
		public override void Load()
        {
			Description = "Adapted for living in subterranean environments, Kobolds are talented miners.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z to breathe out a sparkmine.\n[c/4DBF60:{"+"}] Press X to fire a cluster of mines.\n[c/4DBF60:{"+"}] Press C to detonate any mines within range. Clustermines have an unlimited activation radius.\n[c/FF3640:{"-"}] Sunlight slows and weakens you.";
			CensorClothing = false;
			StarterShirt = true;
			ClothStyle = 2;
			HairColor = new Color(217, 196, 196);
			SkinColor = new Color(171, 87, 80);
			DetailColor = new Color(255, 125, 90);
			EyeColor = new Color(255, 180, 92);
			ShirtColor = new Color(216, 156, 95);
			UnderShirtColor = new Color(119, 115, 157);
		}

		public override void ResetEffects(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				player.pickSpeed -= 0.75f;
				player.moveSpeed += 0.05f;
				player.statLifeMax2 -= (player.statLifeMax2 / 10);
				player.endurance -= 0.05f;
			}
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var koboldPlayer = player.GetModPlayer<KoboldPlayer>();
			koboldPlayer.headRotation = 0;
			koboldPlayer.targetHeadRotation = 0;
			koboldPlayer.firingMine = 0;
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var koboldPlayer = player.GetModPlayer<KoboldPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				if (!player.dead)
				{
					if (!player.HasBuff(BuffType<Refueling>())) {
						Tile targetedTile = Main.tile[(int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16];
						if (MrPlagueRaces.RaceAbilityKeybind1.JustPressed)
						{
							Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * 10f;
							for (int i = 0; i < 3; i++) {
								Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, velocity.X + Main.rand.Next(3) - Main.rand.Next(3), velocity.Y + Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<Breathmine>(), 0, 0, player.whoAmI);
							}
							for (int i = 0; i < 6; i++) {
								Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, Main.rand.Next(3) - Main.rand.Next(3), Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<Breathmine>(), 0, 0, player.whoAmI);
							}
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), Main.MouseWorld.X, Main.MouseWorld.Y, 0f, 0f, ProjectileType<Sparkmine>(), (6 * (player.statLifeMax2 / 40)), 0, player.whoAmI);
							SoundEngine.PlaySound(SoundID.DD2_DrakinShot, player.Center);
							player.direction = Main.MouseWorld.X >= player.Center.X ? 1 : -1;
							koboldPlayer.firingMine = 20;
							player.AddBuff(BuffType<Refueling>(), 120 - (player.statLifeMax2 / 20));
						}
						if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed)
						{
							player.direction = Main.MouseWorld.X >= player.Center.X ? 1 : -1;
							SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, player.Center);
							Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * 10f;
							for (int i = 0; i < 3; i++) {
								Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X + (float)(player.width / 2) / 16, player.Center.Y, velocity.X + Main.rand.Next(3) - Main.rand.Next(3), velocity.Y + Main.rand.Next(3) - Main.rand.Next(3), ProjectileType<Clustermine>(), (6 * (player.statLifeMax2 / 40)), 0, player.whoAmI);
							}
							koboldPlayer.firingMine = 20;
							player.AddBuff(BuffType<Refueling>(), 120 - (player.statLifeMax2 / 20));
						}
					}
					if (MrPlagueRaces.RaceAbilityKeybind3.JustPressed)
					{
						koboldPlayer.triggeringMine = 1;
					}
				}
			}
		}

		public override void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var koboldPlayer = player.GetModPlayer<KoboldPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				if (!player.dead) {
					if (koboldPlayer.firingMine > 0) {
						player.direction = Main.MouseWorld.X >= player.Center.X ? 1 : -1;
						Vector2 offset = Main.MouseWorld - player.Center;
						koboldPlayer.targetHeadRotation = (offset * player.direction).ToRotation() * 0.55f;
					} 
					else 
					{
						koboldPlayer.targetHeadRotation = 0;
					}
					koboldPlayer.headRotation = MathHelper.Lerp(koboldPlayer.headRotation, koboldPlayer.targetHeadRotation, 16f / 60);
					player.headRotation = koboldPlayer.headRotation;
				}
			}
		}

		public override void PreUpdate(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var koboldPlayer = player.GetModPlayer<KoboldPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				if (!player.dead) {
					if (koboldPlayer.firingMine > 0) {
						koboldPlayer.firingMine--;
					}
					if (koboldPlayer.firingMine < 0) {
						koboldPlayer.firingMine = 0;
					}
					Lighting.AddLight(player.Center, player.eyeColor.ToVector3());
					if (koboldPlayer.ExposedToSun()) {
						player.AddBuff(BuffType<Troglodyte>(), 2);
					}
				}
			}
		}
	}

	public class KoboldPlayer : ModPlayer
	{
		public float headRotation;
		public float targetHeadRotation;
		public int triggeringMine;
		public int firingMine = 0;

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.KoboldSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(headRotation);
			packet.Write(targetHeadRotation);
			packet.Write(triggeringMine);
			packet.Write(firingMine);
		}

		public override void PreUpdate()
		{
			if (!Player.dead) {
				if (triggeringMine > 0) {
					triggeringMine--;
				}
				if (triggeringMine < 0) {
					triggeringMine = 0;
				}
			}
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
}
