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
		MrPlagueRacesPlayerSyncPlayer
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
				case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncPlayer:
					byte playernumber = reader.ReadByte();
					MrPlagueRacesPlayer MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();
					int PlayerRace = reader.ReadInt32();
					if (RaceLoader.TryGetRace(PlayerRace, out var race))
					{
						MrPlagueRacesPlayer.race = race;
					}
					int detailColorR = reader.ReadInt32();
					int detailColorG = reader.ReadInt32();
					int detailColorB = reader.ReadInt32();
					int colorDetailR = reader.ReadInt32();
					int colorDetailG = reader.ReadInt32();
					int colorDetailB = reader.ReadInt32();
					int colorEyesR = reader.ReadInt32();
					int colorEyesG = reader.ReadInt32();
					int colorEyesB = reader.ReadInt32();
					int colorSkinR = reader.ReadInt32();
					int colorSkinG = reader.ReadInt32();
					int colorSkinB = reader.ReadInt32();
					int colorHairR = reader.ReadInt32();
					int colorHairG = reader.ReadInt32();
					int colorHairB = reader.ReadInt32();

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