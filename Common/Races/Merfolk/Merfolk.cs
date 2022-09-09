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

namespace MrPlagueRaces.Common.Races.Merfolk
{
	public class Merfolk : Race
	{
		public override void Load()
        {
			Description = "Excellent at swimming and fishing, Merfolk breathe water instead of air.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] While underwater, hold Z to streamline-swim. Improves control and speed.\n[c/4DBF60:{"+"}] While underwater, press X to kick forward for a boost of momentum.";
			CensorClothing = false;
			HairColor = new Color(108, 255, 61);
			SkinColor = new Color(58, 188, 116);
			DetailColor = new Color(108, 255, 61);
			EyeColor = new Color(255, 81, 81);
		}

		public override void ResetEffects(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				player.pickSpeed -= 0.1f;
				player.tileSpeed += 0.1f;
				player.ignoreWater = true;
				player.merman = false;
				player.gills = false;
				player.accFlipper = true;
			}
		}

		public override void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var merfolkPlayer = player.GetModPlayer<MerfolkPlayer>();
			merfolkPlayer.fullRotation = 0;
			merfolkPlayer.targetFullRotation = 0;
			merfolkPlayer.headRotation = 0;
			merfolkPlayer.targetHeadRotation = 0;
			merfolkPlayer.swimming = false;
			merfolkPlayer.diveCount = 0;
			merfolkPlayer.breathHurt = 0;
			merfolkPlayer.breathInterval = 7;
			merfolkPlayer.breathMeter = 200;
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var merfolkPlayer = player.GetModPlayer<MerfolkPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				if (!player.dead && player.active)
				{
					if (MrPlagueRaces.RaceAbilityKeybind1.Current && player.wet && merfolkPlayer.diveCount == 0)
					{
						Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * (5 + (player.statLifeMax2 / 100));
						player.velocity = velocity;
						merfolkPlayer.swimming = true;
						if (player.swimTime <= 10) {
							player.swimTime = 30;
						}
						player.controlUp = false;
						player.controlLeft = false;
						player.controlDown = false;
						player.controlRight = false;
						player.controlJump = false;
						player.fallStart = (int)(player.position.Y / 16f);
						player.AddBuff(BuffType<FluidGrace>(), 340);
					}
					else if (merfolkPlayer.diveCount == 0)
					{
						merfolkPlayer.swimming = false;
					}
					if (MrPlagueRaces.RaceAbilityKeybind2.JustPressed && player.wet && merfolkPlayer.diveCount == 0)
					{
						merfolkPlayer.diveCount = 60;
					}
				}
			}
		}

		public override void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var merfolkPlayer = player.GetModPlayer<MerfolkPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				if (!player.dead && !player.sleeping.isSleeping) {
					if (merfolkPlayer.swimming) {
						Vector2 offset = Main.MouseWorld - player.Center;
						merfolkPlayer.targetFullRotation = ((offset * player.direction).ToRotation() * 0.55f) + (player.direction == 1 ? 1.575f : -1.575f);
						merfolkPlayer.targetHeadRotation = ((offset * player.direction).ToRotation() * 0.55f) + (player.direction == 1 ? -1.575f : 1.575f);
						player.direction = Main.MouseWorld.X >= player.Center.X ? 1 : -1;
					} 
					else 
					{
						merfolkPlayer.targetFullRotation = 0;
						merfolkPlayer.targetHeadRotation = 0;
					}
					merfolkPlayer.fullRotation = MathHelper.Lerp(merfolkPlayer.fullRotation, merfolkPlayer.targetFullRotation, 16f / 60);
					merfolkPlayer.headRotation = MathHelper.Lerp(merfolkPlayer.headRotation, merfolkPlayer.targetHeadRotation, 16f / 60);
					player.fullRotationOrigin = new Vector2((player.width / 2), (player.height / 2));
					player.fullRotation = (merfolkPlayer.swimming ? merfolkPlayer.targetFullRotation : merfolkPlayer.fullRotation);
					player.headRotation = (merfolkPlayer.swimming ? merfolkPlayer.targetHeadRotation : merfolkPlayer.headRotation);
					if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) && !merfolkPlayer.swimming && merfolkPlayer.diveCount == 0)
					{
						player.headRotation = player.velocity.Y * (float)player.direction * 0.1f;
						if ((double)player.headRotation < -0.3)
						{
							player.headRotation = -0.3f;
						}
						if ((double)player.headRotation > 0.3)
						{
							player.headRotation = 0.3f;
						}
					}
				}
			}
		}

		public override void PreUpdate(Player player)
		{
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			var merfolkPlayer = player.GetModPlayer<MerfolkPlayer>();
			if (mrPlagueRacesPlayer.statsEnabled) {
				if (merfolkPlayer.diveCount > 0) {
					merfolkPlayer.swimming = true;
					if (merfolkPlayer.diveCount == 60) {
						Vector2 velocity = -Vector2.Normalize(Main.MouseWorld - player.Center) * 5;
						player.velocity = velocity;
						player.swimTime = 0;
					}
					if (merfolkPlayer.diveCount == 45) {
						player.swimTime = 30;
					}
					if (merfolkPlayer.diveCount == 40) {
						Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * (15 + (player.statLifeMax2 / 100));
						player.velocity = velocity;
						player.AddBuff(BuffType<FluidGrace>(), 120);
					}
					merfolkPlayer.diveCount--;
				}
				if (player.dead)
				{
					merfolkPlayer.breathInterval = 0;
					merfolkPlayer.breathMeter = 200;
				}
				if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
				{
					merfolkPlayer.breathInterval = 0;
					if (merfolkPlayer.breathMeter < 200)
					{
						merfolkPlayer.breathMeter += 3;
					}
					if (merfolkPlayer.breathMeter > 200)
					{
						merfolkPlayer.breathMeter = 200;
					}
					player.breath = (merfolkPlayer.breathMeter + 2);
					merfolkPlayer.breathHurt = 0;
				}
				else
				{
					merfolkPlayer.breathInterval += 1;
					if (merfolkPlayer.breathInterval >= 7)
					{
						merfolkPlayer.breathMeter -= 1;
						merfolkPlayer.breathInterval = 0;
					}
					player.breath = (merfolkPlayer.breathMeter - 2);
				}
				if (player.breath == 0)
				{
					SoundEngine.PlaySound(SoundID.Drown, player.Center);
				}
				if (player.breath <= 0)
				{
					player.lifeRegenTime = 0;
					player.breath = 0;
					merfolkPlayer.breathHurt += 1;
					if (merfolkPlayer.breathHurt >= 7)
					{
						player.statLife -= 2;
						merfolkPlayer.breathHurt = 0;
					}
					if (player.statLife <= 0)
					{
						player.statLife = 0;
						switch (Main.rand.Next(8))
						{
							case 0:
								player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " is sleeping with the airbreathers."), 10.0, 0, false);
								break;
							case 1:
								player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " didn't make it to the water."), 10.0, 0, false);
								break;
							case 2:
								player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " was out of their element."), 10.0, 0, false);
								break;
							case 3:
								player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " suffocated."), 10.0, 0, false);
								break;
							case 4:
								player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " couldn't breathe."), 10.0, 0, false);
								break;
							case 5:
								player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " is food for the land dwellers."), 10.0, 0, false);
								break;
							case 6:
								player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " tried breathing air."), 10.0, 0, false);
								break;
							default:
								player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " had gills instead of lungs."), 10.0, 0, false);
								break;
						}
					}
				}
			}
		}
	}

	public class MerfolkPlayer : ModPlayer
	{
		public float fullRotation;
		public float targetFullRotation;
		public float headRotation;
		public float targetHeadRotation;
		public bool swimming = false;
		public int diveCount;
		public int breathHurt;
		public int breathInterval = 7;
		public int breathMeter = 200;

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.MerfolkSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(fullRotation);
			packet.Write(targetFullRotation);
			packet.Write(headRotation);
			packet.Write(targetHeadRotation);
			packet.Write(swimming);
			packet.Write(diveCount);
			packet.Write(breathHurt);
			packet.Write(breathInterval);
			packet.Write(breathMeter);
		}
	}
}
