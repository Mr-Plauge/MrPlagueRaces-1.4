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

namespace MrPlagueRaces.Common.Races.Fluftrodon
{
	public class Fluftrodon : Race
	{
		public override void Load()
        {
			Description = "Capable of generating paint via an arcane process, Fluftrodons highly value the arts.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z to paint. Left click and right click to paint tiles and walls, scroll to cycle color.\n[c/4DBF60:{"+"}] Hold X to charge up a powerful leap. Scales with max health.\n[c/4DBF60:{"+"}] Hold A or D against a wall and press space to walljump. Scales with max health.";
			CensorClothing = false;
			HairColor = new Color(91, 115, 177);
			SkinColor = new Color(190, 233, 255);
			DetailColor = new Color(91, 115, 177);
			EyeColor = new Color(81, 135, 255);
		}

		public override void ResetEffects(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				player.tileSpeed += 0.5f;
				player.blockRange += 10;
				player.pickSpeed -= 0.25f;
				player.GetDamage(DamageClass.Generic) -= 0.15f;
			}
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var fluftrodonPlayer = player.GetModPlayer<FluftrodonPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				if ((PlayerInput.MouseInfo.ScrollWheelValue - PlayerInput.MouseInfoOld.ScrollWheelValue) > 0 && player.ownedProjectileCounts[ProjectileType<PaintMenu>()] != 0) {
					fluftrodonPlayer.selectedPaint -= 1;
					if (fluftrodonPlayer.selectedPaint < 0) {
						fluftrodonPlayer.selectedPaint = 30;
					}
				}
				if ((PlayerInput.MouseInfo.ScrollWheelValue - PlayerInput.MouseInfoOld.ScrollWheelValue) < 0 && player.ownedProjectileCounts[ProjectileType<PaintMenu>()] != 0) {
					fluftrodonPlayer.selectedPaint += 1;
					if (fluftrodonPlayer.selectedPaint > 30) {
						fluftrodonPlayer.selectedPaint = 0;
					}
						
				}
				if (player.ownedProjectileCounts[ProjectileType<PaintMenu>()] != 0) {
					if (player.controlUseItem) {
						player.controlUseItem = false;
					}
					if (Main.mouseLeft) {
						WorldGen.paintTile(Player.tileTargetX, Player.tileTargetY, (byte)fluftrodonPlayer.selectedPaint, true);
					}
					if (Main.mouseRight) {
						WorldGen.paintWall(Player.tileTargetX, Player.tileTargetY, (byte)fluftrodonPlayer.selectedPaint, true);
					}
				}
				if (!player.dead)
				{
					if (MrPlagueRaces.RaceAbilityKeybind1.JustPressed)
					{
						if (player.ownedProjectileCounts[ProjectileType<PaintMenu>()] == 0) {
							Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, 0, 0, ProjectileType<PaintMenu>(), 0, 0, player.whoAmI);
						}
						else {
							fluftrodonPlayer.closeMenu = true;
						}
					}
					if (MrPlagueRaces.RaceAbilityKeybind2.Current && !player.HasBuff(BuffType<Airborne>()))
					{
						if (fluftrodonPlayer.jumpCharge == 0) {
							SoundEngine.PlaySound(SoundID.Item105, player.Center);
							for (int i = 0; i < 10; i++) {
								int dust = Dust.NewDust(player.position, player.width, player.height, 264);
								Main.dust[dust].color = fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint];
								Main.dust[dust].noGravity = true;
								Main.dust[dust].velocity *= 2f;
								dust = Dust.NewDust(player.position, player.width, player.height, 264);
								Main.dust[dust].color = fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint];
								Main.dust[dust].noGravity = true;
								Main.dust[dust].velocity *= 1f;
							}
						}
						if (fluftrodonPlayer.jumpCharge < 40) {
							fluftrodonPlayer.jumpCharge++;
						}
					}
					if (MrPlagueRaces.RaceAbilityKeybind2.JustReleased)
					{
						if (fluftrodonPlayer.jumpCharge > 0) {
							SoundEngine.PlaySound(SoundID.Item152, player.Center);
							SoundEngine.PlaySound(SoundID.Item39, player.Center);
							for (int i = 0; i < 20; i++) {
								int dust = Dust.NewDust(player.position, player.width, player.height, 264);
								Main.dust[dust].color = fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint];
								Main.dust[dust].noGravity = true;
								Main.dust[dust].velocity *= 5f;
								dust = Dust.NewDust(player.position, player.width, player.height, 264);
								Main.dust[dust].color = fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint];
								Main.dust[dust].noGravity = true;
								Main.dust[dust].velocity *= 4f;
							}
							Vector2 velocity = (Vector2.Normalize(Main.MouseWorld - player.Center) * (fluftrodonPlayer.jumpCharge / 2)) * (0.5f + ((float)player.statLifeMax2 * 0.001f));
							player.velocity = velocity;
							player.fallStart = (int)(player.position.Y / 16f);
							player.AddBuff(BuffType<Airborne>(), 6000);
							fluftrodonPlayer.jumpCharge = 0;
						}
					}
					if (player.controlJump && (player.controlLeft || player.controlRight) && player.velocity.X == 0 && player.velocity.Y != 0 && fluftrodonPlayer.canWallJump) {
						SoundEngine.PlaySound(SoundID.Item152, player.Center);
						SoundEngine.PlaySound(SoundID.Item39, player.Center);
						for (int i = 0; i < 5; i++) {
							int dust = Dust.NewDust(player.position, player.width, player.height, 264);
							Main.dust[dust].color = fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint];
							Main.dust[dust].noGravity = true;
							Main.dust[dust].velocity *= 5f;
							dust = Dust.NewDust(player.position, player.width, player.height, 264);
							Main.dust[dust].color = fluftrodonPlayer.paintColor[fluftrodonPlayer.selectedPaint];
							Main.dust[dust].noGravity = true;
							Main.dust[dust].velocity *= 4f;
						}
						player.velocity.Y = -10f + player.statLifeMax2 / -100;
						fluftrodonPlayer.canWallJump = false;
					}
					if (!player.controlJump) {
						fluftrodonPlayer.canWallJump = true;
					}
				}
			}
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
			for (int i = 0; i < Main.maxProjectiles; i++) {
				Projectile projectile = Main.projectile[i];
				if (projectile.active && projectile.type == ProjectileType<PaintMenu>() && projectile.owner == player.whoAmI)
					projectile.Kill();
			}
		}

		public override void PreUpdate(Player player) {
			if (player.velocity.Y == 0) {
				player.ClearBuff(BuffType<Airborne>());
			}
		}
	}

	public class FluftrodonPlayer : ModPlayer
	{
		public Color[] paintColor = { new Color(255, 255, 255) * 0, new Color(244, 0, 0), new Color(244, 109, 0), new Color(244, 241, 0), new Color(181, 244, 0), new Color(0, 244, 10), new Color(0, 244, 164), new Color(0, 226, 244), new Color(0, 162, 244), new Color(41, 0, 244), new Color(150, 0, 244), new Color(244, 0, 243), new Color(249, 100, 158), new Color(144, 0, 0), new Color(144, 64, 0), new Color(144, 136, 0), new Color(106, 144, 0), new Color(0, 144, 5), new Color(0, 144, 96), new Color(0, 133, 144), new Color(0, 95, 144), new Color(24, 0, 144), new Color(88, 0, 144), new Color(144, 0, 143), new Color(147, 59, 93), new Color(64, 63, 73), new Color(235, 232, 245), new Color(141, 138, 154), new Color(150, 121, 94), new Color(5, 3, 10), new Color(11, 255, 255), new Color(208, 239, 246)};
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
		}
	}
}
