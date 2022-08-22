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

namespace MrPlagueRaces.Common.Races.Wendigo
{
	public class Wendigo : Race
	{
		public override void Load()
        {
			Description = "Made from an amalgamation of souls, Wendigoes can shred through the fabric of space.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Hold Z to tear through reality, turning you into an intangible missile. Maximum duration scales with max health.\n[c/4DBF60:{"+"}] Hold X to attack with your claws, shredding enemy defense. Damage scales with max health.";
			CensorClothing = false;
			HairColor = new Color(255, 255, 255);
			SkinColor = new Color(57, 59, 70);
			DetailColor = new Color(57, 59, 70);
			EyeColor = new Color(118, 194, 255);
		}
		
		public override void ResetEffects(Player player)
		{
			player.moveSpeed += 0.25f;
			player.GetDamage(DamageClass.Generic).Base += 10f;
			player.endurance -= 0.1f;
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var wendigoPlayer = player.GetModPlayer<WendigoPlayer>();
			wendigoPlayer.rending = false;
			wendigoPlayer.rendDelay = 0;
			wendigoPlayer.rendTimer = 120 + (player.statLifeMax2 / 10);
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var wendigoPlayer = player.GetModPlayer<WendigoPlayer>();
			if (!player.dead)
			{
				if (MrPlagueRaces.RaceAbilityKeybind1.Current && wendigoPlayer.rendTimer > 0 && !player.HasBuff(BuffType<MolecularRecoil>()))
				{
					wendigoPlayer.rendDelay++;
					if (wendigoPlayer.rendDelay == 1) {
						SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost, player.Center);
						SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, player.Center);
						Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, 0, 0, ProjectileType<RendClaw>(), player.statLifeMax2 / 5, 1, player.whoAmI);
					}
					if (wendigoPlayer.rendDelay == 9) {
						SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, player.Center);
						SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, player.Center);
						SoundEngine.PlaySound(SoundID.Zombie104, player.Center);
						for (int i = 0; i < 20; i++) {
							int dust = Dust.NewDust(player.position, player.width, player.height, 264);
							Main.dust[dust].color = player.eyeColor;
							Main.dust[dust].noGravity = true;
							Main.dust[dust].velocity *= 5f;
							dust = Dust.NewDust(player.position, player.width, player.height, 264);
							Main.dust[dust].color = player.eyeColor;
							Main.dust[dust].noGravity = true;
							Main.dust[dust].velocity *= 4f;
						}
					}
					if (wendigoPlayer.rendDelay >= 10) {
						player.ghost = true;
						Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * 15;
						player.velocity = velocity;
						player.controlUp = false;
						player.controlLeft = false;
						player.controlDown = false;
						player.controlRight = false;
						player.controlJump = false;
						wendigoPlayer.rending = true;
						player.fallStart = (int)(player.position.Y / 16f);
					}
				}
				else
				{
					if (wendigoPlayer.rendDelay >= 10) {
						for (int i = 0; i < 10; i++) {
							int dust = Dust.NewDust(player.position, player.width, player.height, 264);
							Main.dust[dust].color = player.eyeColor;
							Main.dust[dust].noGravity = true;
							Main.dust[dust].velocity *= 5f;
							dust = Dust.NewDust(player.position, player.width, player.height, 264);
							Main.dust[dust].color = player.eyeColor;
							Main.dust[dust].noGravity = true;
							Main.dust[dust].velocity *= 4f;
						}
						SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, player.Center);
						SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, player.Center);
						SoundEngine.PlaySound(SoundID.Zombie103, player.Center);
						player.AddBuff(BuffType<MolecularRecoil>(), 360 - wendigoPlayer.rendTimer);
					}
					player.ghost = false;
					wendigoPlayer.rending = false;
					wendigoPlayer.rendDelay = 0;
					wendigoPlayer.rendTimer = 120 + (player.statLifeMax2 / 10);
				}
				if (MrPlagueRaces.RaceAbilityKeybind2.Current && !MrPlagueRaces.RaceAbilityKeybind1.Current) {
					if (player.ownedProjectileCounts[ProjectileType<SoulClaw>()] == 0) {
						Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X, player.Center.Y, 0, 0, ProjectileType<SoulClaw>(), player.statLifeMax2 / 4, 1, player.whoAmI);
					}
				}
			}
		}

		public override void PreUpdate(Player player)
		{
			var wendigoPlayer = player.GetModPlayer<WendigoPlayer>();
			if (!player.dead)
			{
				if (wendigoPlayer.rending) {
					for (int i = 0; i < 25; i++)
					{
						int dust = Dust.NewDust(player.Center, 0, 0, 264);
						Main.dust[dust].color = player.eyeColor;
						Main.dust[dust].noGravity = true;
					}
					if (wendigoPlayer.rendTimer > 0) {
						wendigoPlayer.rendTimer--;
					}
				}
			}
		}
	}

	public class WendigoPlayer : ModPlayer
	{
		public bool rending = false;
		public int rendDelay;
		public int rendTimer = 120;
	}
}
