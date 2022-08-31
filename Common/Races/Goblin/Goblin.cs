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
using MrPlagueRaces.Content.Prefixes;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Common.Races.Goblin
{
	public class Goblin : Race
	{
		public override void Load()
        {
			Description = "Known for their effective tinkering, Goblins forge their equipment with dark flames.";
			AbilitiesDescription = $"[c/4DBF60:{"+"}] Press Z to shadowforge your held item, granting it unique powers. Costs mana.\n[c/4DBF60:{"+"}] Press X to unleash a shadowflame harvester, which burns enemies and grants you improved mana regeneration.";
			ClothStyle = 2;
			HairStyle = 15;
			StarterShirt = true;
			StarterPants = true;
			HairColor = new Color(85, 96, 123);
			SkinColor = new Color(182, 215, 126);
			DetailColor = new Color(182, 215, 126);
			EyeColor = new Color(105, 90, 75);
			ShirtColor = new Color(182, 91, 91);
			UnderShirtColor = new Color(166, 113, 93);
			PantsColor = new Color(175, 227, 255);
			ShoeColor = new Color(160, 105, 60);
		}

		public override void ResetEffects(Player player)
		{
			var goblinPlayer = player.GetModPlayer<GoblinPlayer>();
			player.moveSpeed += 0.1f;
			player.maxMinions += 1;
			player.maxTurrets += 1;
			player.statLifeMax2 -= (player.statLifeMax2 / 5);
			player.statManaMax2 += 40;
		}

		public override void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			var goblinPlayer = player.GetModPlayer<GoblinPlayer>();
			var mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			if (!player.dead)
			{
				if (MrPlagueRaces.RaceAbilityKeybind1.JustPressed && goblinPlayer.IsEquipment(player.HeldItem) && player.HeldItem.stack == 1 && player.statMana >= 60)
				{
					var item = player.inventory[player.selectedItem];
					if (ItemLoader.PreReforge(item))
					{
						var favorited = item.favorited;
						var stack = item.stack;

						var obj = new Item();
						obj.netDefaults(item.netID);
						var obj2 = obj.CloneWithModdedDataFrom(item);

						if (goblinPlayer.IsAccessory(player.HeldItem)) {
							obj2.Prefix(goblinPlayer.AccessoryPrefixes[Main.rand.Next(goblinPlayer.AccessoryPrefixes.Length)]);
							while (obj2.prefix == item.prefix) {
								obj.prefix = obj2.prefix;
								obj2 = obj.CloneWithModdedDataFrom(item);
								obj2.Prefix(goblinPlayer.AccessoryPrefixes[Main.rand.Next(goblinPlayer.AccessoryPrefixes.Length)]);
							}
						}
						
						if ((goblinPlayer.IsWeapon(player.HeldItem) && !goblinPlayer.IsTool(player.HeldItem)) || ItemID.Sets.IsDrill[player.HeldItem.type] || ItemID.Sets.IsChainsaw[player.HeldItem.type]) {
							obj2.Prefix(goblinPlayer.AnyWeaponPrefixes[Main.rand.Next(goblinPlayer.AnyWeaponPrefixes.Length)]);
							while (obj2.prefix == item.prefix) {
								obj.prefix = obj2.prefix;
								obj2 = obj.CloneWithModdedDataFrom(item);
								obj2.Prefix(goblinPlayer.AnyWeaponPrefixes[Main.rand.Next(goblinPlayer.AnyWeaponPrefixes.Length)]);
							}
						}

						if (goblinPlayer.IsTool(player.HeldItem) && (!ItemID.Sets.IsDrill[player.HeldItem.type] && !ItemID.Sets.IsChainsaw[player.HeldItem.type])) {
							obj2.Prefix(goblinPlayer.ToolPrefixes[Main.rand.Next(goblinPlayer.ToolPrefixes.Length)]);
							while (obj2.prefix == item.prefix) {
								obj.prefix = obj2.prefix;
								obj2 = obj.CloneWithModdedDataFrom(item);
								obj2.Prefix(goblinPlayer.ToolPrefixes[Main.rand.Next(goblinPlayer.ToolPrefixes.Length)]);
							}
						}

						item = obj2.Clone();
						item.position.X = Main.player[Main.myPlayer].position.X + (float)(Main.player[Main.myPlayer].width / 2) - (float)(item.width / 2);
						item.position.Y = Main.player[Main.myPlayer].position.Y + (float)(Main.player[Main.myPlayer].height / 2) - (float)(item.height / 2);
						item.favorited = favorited;
						item.stack = stack;
						player.inventory[player.selectedItem] = item;

						ItemLoader.PostReforge(item);
						PopupText.NewText(PopupTextContext.ItemReforge, item, item.stack, noStack: true);
						SoundEngine.PlaySound(SoundID.Item37);
						SoundEngine.PlaySound(SoundID.DD2_BookStaffCast);
						for (int i = 0; i < 140; i++) {
							if (Main.rand.Next(50) == 1) {
								Dust dust19 = Dust.NewDustDirect(new Vector2(player.position.X - 2f, player.position.Y - 2f), player.width + 4, player.height + 4, 27, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 180, default(Color), 1.95f);
								dust19.noGravity = true;
								dust19.velocity *= 0.75f;
								dust19.velocity.X *= 0.75f;
								dust19.velocity.Y -= 1f;
								if (Main.rand.Next(4) == 0)
								{
									dust19.noGravity = false;
									dust19.scale *= 0.5f;
								}
							}
						}
						player.statMana -= 60;
					}
				}
				if (MrPlagueRaces.RaceAbilityKeybind2.Current && !player.HasBuff(BuffType<Fathoming>()) && goblinPlayer.harvesterCounter == 0)
				{
					goblinPlayer.harvesterCounter = 30;
					SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryCircle, player.Center);
				}
				if (goblinPlayer.harvesterCounter > 0) {
					player.controlUseItem = false;
				}
			}
		}

		public override void PreUpdate(Player player) {
			var goblinPlayer = player.GetModPlayer<GoblinPlayer>();
			if (goblinPlayer.harvesterCounter > 0) {
				goblinPlayer.harvesterCounter--;
				for (int i = 0; i < 30; i++) {
					Dust dust19 = Dust.NewDustDirect(new Vector2(player.Center.X + (player.direction == 1 ? 0 : -10), player.Center.Y - 30), 0, 0, 27, 0f, player.velocity.Y * 0.4f, 180, default(Color), 1.95f);
					dust19.noGravity = true;
					dust19.velocity *= 0.75f;
					dust19.velocity.X *= 0.75f;
					dust19.velocity.Y -= 1f;
					if (Main.rand.Next(4) == 0)
					{
						dust19.noGravity = false;
						dust19.scale *= 0.5f;
					}
				}
			}
			if (goblinPlayer.harvesterCounter == 1) {
				Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * 10f;
				Projectile.NewProjectile(Wiring.GetProjectileSource(0, 0), player.Center.X + (player.direction == 1 ? 0 : -10), player.Center.Y - 30, velocity.X, velocity.Y, ProjectileType<ShadowflameHarvester>(), 5, 5, player.whoAmI);
				SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, player.Center);
				player.AddBuff(BuffType<Fathoming>(), 240);
				for (int i = 0; i < 30; i++) {
					int dust = Dust.NewDust(new Vector2(player.Center.X + (player.direction == 1 ? 0 : -10), player.Center.Y - 30), 0, 0, 27);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 3f;
					dust = Dust.NewDust(new Vector2(player.Center.X + (player.direction == 1 ? 0 : -10), player.Center.Y - 30), 0, 0, 27);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 2f;
				}
			}
		}

		public override void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo) {
			var goblinPlayer = player.GetModPlayer<GoblinPlayer>();
			if (goblinPlayer.harvesterCounter > 0) {
				player.bodyFrame.Y = player.bodyFrame.Height * 5;
			}
		}
	}

	public class GoblinPlayer : ModPlayer
	{
		public int[] AccessoryPrefixes = { PrefixType<Combustible>(), PrefixType<Constructive>(), PrefixType<Flawless>(), PrefixType<Impactful>(), PrefixType<Hexed>(), PrefixType<Luminescent>(), PrefixType<Regenerative>(), PrefixType<Reinforced>(), PrefixType<Resilient>(), PrefixType<Streamlined>(), PrefixType<Undying>(), PrefixType<Volatile>() };
		public int[] AnyWeaponPrefixes = { PrefixType<Accelerative>(), PrefixType<Bewitched>(), PrefixType<Bombarding>(), PrefixType<Explosive>(), PrefixType<Immolating>(), PrefixType<Revitalizing>(), PrefixType<Targeting>(), PrefixType<Warping>() };
		public int[] ToolPrefixes = { PrefixType<Fortunate>(), PrefixType<Recreational>(), PrefixType<Trailblazing>(), PrefixType<Tranquilizing>() };

		public int harvesterCounter;

		public bool IsAccessory(Item Item)
		{
			return (Item.accessory == true);
		}
		public bool IsTool(Item Item)
		{
			return (Item.pick > 0 || Item.axe > 0 || Item.hammer > 0);
		}

		public bool IsWeapon(Item Item)
		{
			return (Item.damage > 0);
		}
		public bool IsEquipment(Item Item)
		{
			return ((IsAccessory(Item) || IsTool(Item) || IsWeapon(Item)) && Item.IsCandidateForReforge);
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MrPlagueRacesMessageType.GoblinSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(harvesterCounter);
		}
	}
}
