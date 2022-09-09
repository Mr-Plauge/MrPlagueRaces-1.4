using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using MrPlagueRaces.Common.UI;
using MrPlagueRaces.Common.UI.States;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using MrPlagueRaces.Common.Races;

namespace MrPlagueRaces
{
	internal enum MrPlagueRacesMessageType : byte
	{
		ExecuteRaceSound,
		MrPlagueRacesPlayerSyncPlayer,
		DerpkinSyncPlayer,
		DragonkinSyncPlayer,
		FluftrodonSyncPlayer,
		GoblinSyncPlayer,
		KenkuSyncPlayer,
		KoboldSyncPlayer,
		LihzahrdSyncPlayer,
		MerfolkSyncPlayer,
		MushfolkSyncPlayer,
		SkeletonSyncPlayer,
		TabaxiSyncPlayer,
		VampireSyncPlayer,
		WendigoSyncPlayer
	}
	public class MrPlagueRaces : Mod
	{
		public static ModKeybind RaceAbilityKeybind1;
		public static ModKeybind RaceAbilityKeybind2;
		public static ModKeybind RaceAbilityKeybind3;
		public static ModKeybind RaceAbilityKeybind4;

		public override void Load() 
		{
			RaceAbilityKeybind1 = KeybindLoader.RegisterKeybind(this, "Primary Racial Ability", "Z");
			RaceAbilityKeybind2 = KeybindLoader.RegisterKeybind(this, "Secondary Racial Ability", "X");
			RaceAbilityKeybind3 = KeybindLoader.RegisterKeybind(this, "Tertiary Racial Ability", "C");
			RaceAbilityKeybind4 = KeybindLoader.RegisterKeybind(this, "Quaternary Racial Ability", "V");
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI) 
		{
			MrPlagueRacesMessageType msgType = (MrPlagueRacesMessageType)reader.ReadByte();

			switch (msgType) 
			{
				case MrPlagueRacesMessageType.ExecuteRaceSound:
					byte playernumber = reader.ReadByte();
					MrPlagueRacesPlayer MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();
					MrPlagueRacesPlayer.ExecuteRaceSound(Main.player[playernumber], reader.ReadString());
					break;
				case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncPlayer:
					playernumber = reader.ReadByte();
					MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();
					int PlayerRace = reader.ReadInt32();
					if (RaceLoader.TryGetRace(PlayerRace, out var race))
					{
						MrPlagueRacesPlayer.race = race;
					}
					MrPlagueRacesPlayer.detailColor.R = reader.ReadByte();
					MrPlagueRacesPlayer.detailColor.G = reader.ReadByte();
					MrPlagueRacesPlayer.detailColor.B = reader.ReadByte();
					MrPlagueRacesPlayer.colorDetail.R = reader.ReadByte();
					MrPlagueRacesPlayer.colorDetail.G = reader.ReadByte();
					MrPlagueRacesPlayer.colorDetail.B = reader.ReadByte();
					MrPlagueRacesPlayer.colorEyes.R = reader.ReadByte();
					MrPlagueRacesPlayer.colorEyes.G = reader.ReadByte();
					MrPlagueRacesPlayer.colorEyes.B = reader.ReadByte();
					MrPlagueRacesPlayer.colorSkin.R = reader.ReadByte();
					MrPlagueRacesPlayer.colorSkin.G = reader.ReadByte();
					MrPlagueRacesPlayer.colorSkin.B = reader.ReadByte();
					MrPlagueRacesPlayer.colorHair.R = reader.ReadByte();
					MrPlagueRacesPlayer.colorHair.G = reader.ReadByte();
					MrPlagueRacesPlayer.colorHair.B = reader.ReadByte();
					MrPlagueRacesPlayer.statsEnabled = reader.ReadBoolean();
					MrPlagueRacesPlayer.gotStatToggler = reader.ReadBoolean();
					break;
				case MrPlagueRacesMessageType.DerpkinSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Derpkin.DerpkinPlayer DerpkinPlayer = Main.player[playernumber].GetModPlayer<Common.Races.Derpkin.DerpkinPlayer>();
					DerpkinPlayer.headRotation = reader.ReadByte();
					DerpkinPlayer.targetHeadRotation = reader.ReadByte();
					DerpkinPlayer.counterSpin = reader.ReadInt32();
					break;
				case MrPlagueRacesMessageType.DragonkinSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Dragonkin.DragonkinPlayer DragonkinPlayer = Main.player[playernumber].GetModPlayer<Common.Races.Dragonkin.DragonkinPlayer>();
					DragonkinPlayer.headRotation = reader.ReadByte();
					DragonkinPlayer.targetHeadRotation = reader.ReadByte();
					DragonkinPlayer.breathingSmoke = reader.ReadBoolean();
					DragonkinPlayer.firingSmoke = reader.ReadInt32();
					DragonkinPlayer.burningOut = reader.ReadInt32();
					DragonkinPlayer.soundInterval = reader.ReadInt32();
					break;
				case MrPlagueRacesMessageType.FluftrodonSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Fluftrodon.FluftrodonPlayer FluftrodonPlayer = Main.player[playernumber].GetModPlayer<Common.Races.Fluftrodon.FluftrodonPlayer>();
					FluftrodonPlayer.selectedPaint = reader.ReadInt32();
					FluftrodonPlayer.jumpCharge = reader.ReadByte();
					FluftrodonPlayer.canWallJump = reader.ReadBoolean();
					FluftrodonPlayer.closeMenu = reader.ReadBoolean();
					break;
				case MrPlagueRacesMessageType.GoblinSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Goblin.GoblinPlayer GoblinPlayer = Main.player[playernumber].GetModPlayer<Common.Races.Goblin.GoblinPlayer>();
					GoblinPlayer.harvesterCounter = reader.ReadInt32();
					break;
				case MrPlagueRacesMessageType.KenkuSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Kenku.KenkuPlayer KenkuPlayer = Main.player[playernumber].GetModPlayer<Common.Races.Kenku.KenkuPlayer>();
					KenkuPlayer.wingTime = reader.ReadByte();
					KenkuPlayer.flying = reader.ReadBoolean();
					KenkuPlayer.wingFrame = reader.ReadInt32();
					KenkuPlayer.wingFrameCounter = reader.ReadInt32();
					KenkuPlayer.dashTime = reader.ReadInt32();
					break;
				case MrPlagueRacesMessageType.KoboldSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Kobold.KoboldPlayer KoboldPlayer = Main.player[playernumber].GetModPlayer<Common.Races.Kobold.KoboldPlayer>();
					KoboldPlayer.headRotation = reader.ReadByte();
					KoboldPlayer.targetHeadRotation = reader.ReadByte();
					KoboldPlayer.triggeringMine = reader.ReadInt32();
					KoboldPlayer.firingMine = reader.ReadInt32();
					break;
				case MrPlagueRacesMessageType.LihzahrdSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Lihzahrd.LihzahrdPlayer LihzahrdPlayer = Main.player[playernumber].GetModPlayer<Common.Races.Lihzahrd.LihzahrdPlayer>();
					LihzahrdPlayer.fullRotation = reader.ReadByte();
					LihzahrdPlayer.targetFullRotation = reader.ReadByte();
					LihzahrdPlayer.headRotation = reader.ReadByte();
					LihzahrdPlayer.targetHeadRotation = reader.ReadByte();
					LihzahrdPlayer.crawlFrame = reader.ReadInt32();
					LihzahrdPlayer.crawlFrameCounter = reader.ReadInt32();
					LihzahrdPlayer.legFrame = reader.ReadInt32();
					LihzahrdPlayer.legFrameCounter = reader.ReadInt32();
					LihzahrdPlayer.selectedGolem = reader.ReadInt32();
					LihzahrdPlayer.direction = reader.ReadInt32();
					LihzahrdPlayer.closeMenu = reader.ReadBoolean();
					break;
				case MrPlagueRacesMessageType.MerfolkSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Merfolk.MerfolkPlayer MerfolkPlayer = Main.player[playernumber].GetModPlayer<Common.Races.Merfolk.MerfolkPlayer>();
					MerfolkPlayer.fullRotation = reader.ReadByte();
					MerfolkPlayer.targetFullRotation = reader.ReadByte();
					MerfolkPlayer.headRotation = reader.ReadByte();
					MerfolkPlayer.targetHeadRotation = reader.ReadByte();
					MerfolkPlayer.swimming = reader.ReadBoolean();
					MerfolkPlayer.diveCount = reader.ReadInt32();
					MerfolkPlayer.breathHurt = reader.ReadInt32();
					MerfolkPlayer.breathInterval = reader.ReadInt32();
					MerfolkPlayer.breathMeter = reader.ReadInt32();
					break;
				case MrPlagueRacesMessageType.MushfolkSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Mushfolk.MushfolkPlayer MushfolkPlayer = Main.player[playernumber].GetModPlayer<Common.Races.Mushfolk.MushfolkPlayer>();
					MushfolkPlayer.sporeless = reader.ReadInt32();
					break;
				case MrPlagueRacesMessageType.SkeletonSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Skeleton.SkeletonPlayer SkeletonPlayer = Main.player[playernumber].GetModPlayer<Common.Races.Skeleton.SkeletonPlayer>();
					SkeletonPlayer.teleportOne = reader.ReadBoolean();
					SkeletonPlayer.teleportTwo = reader.ReadBoolean();
					SkeletonPlayer.teleportThree = reader.ReadBoolean();
					SkeletonPlayer.spirit = reader.ReadInt32();
					SkeletonPlayer.currentBody = reader.ReadInt32();
					break;
				case MrPlagueRacesMessageType.TabaxiSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Tabaxi.TabaxiPlayer TabaxiPlayer = Main.player[playernumber].GetModPlayer<Common.Races.Tabaxi.TabaxiPlayer>();
					TabaxiPlayer.TabaxiSpawn.X = reader.ReadByte();
					TabaxiPlayer.TabaxiSpawn.Y = reader.ReadByte();
					TabaxiPlayer.phased = reader.ReadBoolean();
					TabaxiPlayer.phaseChargeCounter = reader.ReadInt32();
					TabaxiPlayer.phaseActiveCounter = reader.ReadInt32();
					break;
				case MrPlagueRacesMessageType.VampireSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Vampire.VampirePlayer VampirePlayer = Main.player[playernumber].GetModPlayer<Common.Races.Vampire.VampirePlayer>();
					VampirePlayer.stealthTimer = reader.ReadInt32();
					VampirePlayer.LeechTongue = reader.ReadInt32();
					VampirePlayer.Leeching = reader.ReadBoolean();
					break;
				case MrPlagueRacesMessageType.WendigoSyncPlayer:
					playernumber = reader.ReadByte();
					Common.Races.Wendigo.WendigoPlayer WendigoPlayer = Main.player[playernumber].GetModPlayer<Common.Races.Wendigo.WendigoPlayer>();
					WendigoPlayer.rending = reader.ReadBoolean();
					WendigoPlayer.rendDelay = reader.ReadInt32();
					WendigoPlayer.rendTimer = reader.ReadInt32();
					break;
				default:
					Logger.WarnFormat("MrPlagueRaces: Unknown Message type: {0}", msgType);
					break;
			}
		}
		public override void Unload()
		{
			RaceAbilityKeybind1 = null;
			RaceAbilityKeybind2 = null;
			RaceAbilityKeybind3 = null;
			RaceAbilityKeybind4 = null;
			int[] male = { 0, 1, 2, 3, 8 };
			int[] female = { 4, 5, 6, 7, 9 };
			for (int i = 0; i < 165; i++)
			{
				TextureAssets.PlayerHair[i] = (ModContent.HasAsset($"Terraria/Images/Player_Hair_{i + 1}") ? Main.Assets.Request<Texture2D>($"Images/Player_Hair_{i + 1}", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_Hair_16", (AssetRequestMode)1));
				TextureAssets.PlayerHairAlt[i] = (ModContent.HasAsset($"Terraria/Images/Player_HairAlt_{i + 1}") ? Main.Assets.Request<Texture2D>($"Images/Player_HairAlt_{i + 1}", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_HairAlt_16", (AssetRequestMode)1));
			}
			foreach (int i in male)
			{
				TextureAssets.Players[i, 0] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_0") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_0", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_0", (AssetRequestMode)1));
				TextureAssets.Players[i, 1] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_1") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_1", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_1", (AssetRequestMode)1));
				TextureAssets.Players[i, 2] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_2") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_2", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_2", (AssetRequestMode)1));
				TextureAssets.Players[i, 3] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_3") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_3", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_3", (AssetRequestMode)1));
				TextureAssets.Players[i, 4] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_4") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_4", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_4", (AssetRequestMode)1));
				TextureAssets.Players[i, 5] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_5") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_5", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_5", (AssetRequestMode)1));
				TextureAssets.Players[i, 6] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_6") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_6", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_6", (AssetRequestMode)1));
				TextureAssets.Players[i, 7] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_7") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_7", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_7", (AssetRequestMode)1));
				TextureAssets.Players[i, 8] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_8") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_8", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_8", (AssetRequestMode)1));
				TextureAssets.Players[i, 9] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_9") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_9", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_9", (AssetRequestMode)1));
				TextureAssets.Players[i, 10] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_10") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_10", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_10", (AssetRequestMode)1));
				TextureAssets.Players[i, 11] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_11") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_11", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_11", (AssetRequestMode)1));
				TextureAssets.Players[i, 12] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_12") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_12", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_12", (AssetRequestMode)1));
				TextureAssets.Players[i, 13] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_13") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_13", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_13", (AssetRequestMode)1));
				TextureAssets.Players[i, 14] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_14") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_14", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_13", (AssetRequestMode)1));
				TextureAssets.Players[i, 15] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_15") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_15", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_15", (AssetRequestMode)1));
			}
			foreach (int i in female)
			{
				TextureAssets.Players[i, 0] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_0") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_0", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_0", (AssetRequestMode)1));
				TextureAssets.Players[i, 1] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_1") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_1", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_1", (AssetRequestMode)1));
				TextureAssets.Players[i, 2] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_2") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_2", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_2", (AssetRequestMode)1));
				TextureAssets.Players[i, 3] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_3") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_3", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_4_3", (AssetRequestMode)1));
				TextureAssets.Players[i, 4] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_4") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_4", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_4_4", (AssetRequestMode)1));
				TextureAssets.Players[i, 5] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_5") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_5", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_4_5", (AssetRequestMode)1));
				TextureAssets.Players[i, 6] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_6") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_6", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_4_6", (AssetRequestMode)1));
				TextureAssets.Players[i, 7] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_7") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_7", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_4_7", (AssetRequestMode)1));
				TextureAssets.Players[i, 8] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_8") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_8", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_4_8", (AssetRequestMode)1));
				TextureAssets.Players[i, 9] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_9") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_9", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_4_9", (AssetRequestMode)1));
				TextureAssets.Players[i, 10] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_10") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_10", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_4_10", (AssetRequestMode)1));
				TextureAssets.Players[i, 11] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_11") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_11", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_4_11", (AssetRequestMode)1));
				TextureAssets.Players[i, 12] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_12") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_12", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_4_12", (AssetRequestMode)1));
				TextureAssets.Players[i, 13] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_13") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_13", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_4_13", (AssetRequestMode)1));
				TextureAssets.Players[i, 14] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_14") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_14", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_4_13", (AssetRequestMode)1));
				TextureAssets.Players[i, 15] = (ModContent.HasAsset($"Terraria/Images/Player_{i}_15") ? Main.Assets.Request<Texture2D>($"Images/Player_{i}_15", (AssetRequestMode)1) : Main.Assets.Request<Texture2D>("Images/Player_0_15", (AssetRequestMode)1));
			}
			TextureAssets.Ghost = Main.Assets.Request<Texture2D>("Images/Ghost", (AssetRequestMode)1);
		}
	}
}