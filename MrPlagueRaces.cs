using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using MrPlagueRaces.Common.UI;
using MrPlagueRaces.Common.UI.States;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using MrPlagueRaces.Content.Mounts;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.Races.Derpkin;
using MrPlagueRaces.Common.Races.Dragonkin;
using MrPlagueRaces.Common.Races.Fluftrodon;
using MrPlagueRaces.Common.Races.Goblin;
using MrPlagueRaces.Common.Races.Human;
using MrPlagueRaces.Common.Races.Kenku;
using MrPlagueRaces.Common.Races.Kobold;
using MrPlagueRaces.Common.Races.Lihzahrd;
using MrPlagueRaces.Common.Races.Lycan;
using MrPlagueRaces.Common.Races.Merfolk;
using MrPlagueRaces.Common.Races.Mushfolk;
using MrPlagueRaces.Common.Races.Skeleton;
using MrPlagueRaces.Common.Races.Soulbeast;
using MrPlagueRaces.Common.Races.Tabaxi;
using MrPlagueRaces.Common.Races.Vampire;
using static Terraria.ModLoader.ModContent;
using System;
using MrPlagueRaces.Content.Buffs;

namespace MrPlagueRaces
{
	internal enum MrPlagueRacesMessageType : byte
	{
		ExecuteRaceSound,
        MrPlagueRacesPlayerSyncRace,
        MrPlagueRacesPlayerSyncSkinColor,
        MrPlagueRacesPlayerSyncHairColor,
        MrPlagueRacesPlayerSyncEyeColor,
        MrPlagueRacesPlayerSyncDetailColor,
        MrPlagueRacesPlayerSyncAuxDetail1Color,
        MrPlagueRacesPlayerSyncAuxDetail2Color,
        MrPlagueRacesPlayerSyncAuxDetail3Color,
        MrPlagueRacesPlayerSyncShirtColor,
        MrPlagueRacesPlayerSyncUndershirtColor,
        MrPlagueRacesPlayerSyncPantsColor,
        MrPlagueRacesPlayerSyncShoeColor,
        MrPlagueRacesPlayerSyncHairstyles,
        MrPlagueRacesPlayerSyncClothStyle,
        MrPlagueRacesPlayerSyncPosition,
        MrPlagueRacesPlayerSyncVelocity,
        MrPlagueRacesPlayerSyncRotationsAndOffsets,
        MrPlagueRacesPlayerSyncDismount,
        MrPlagueRacesPlayerSyncCrawl,
        MrPlagueRacesPlayerSyncBat,
        MrPlagueRacesPlayerSyncMouseWorld,
        MrPlagueRacesPlayerSyncPlayerFrames,
        MrPlagueRacesPlayerSyncBlink,

        DerpkinSyncPlayer,
		DragonkinSyncPlayer,
		FluftrodonSyncPlayer,
		GoblinSyncPlayer,
		KenkuSyncPlayer,
		KoboldSyncPlayer,
		LihzahrdSyncPlayer,
        LycanSyncPlayer, // {T} Added this.
		MerfolkSyncPlayer,
		MushfolkSyncPlayer,
		SkeletonSyncPlayer,
		TabaxiSyncPlayer,
		VampireSyncPlayer,
		SoulbeastSyncPlayer,

        DerpkinLeapSound,
        DerpkinSpinSound,
        DragonkinFireballSound,
        FluftrodonChargeSound,
        FluftrodonJumpSound,
        GoblinChargeSound,
        GoblinShootSound,
        KenkuDashSound,
        KenkuSummonFeathersSound,
        KenkuFlapSound,
        KoboldMineSummonSound,
        KoboldClusterMineSummonSound,
        KoboldExplosionSound,
        LihzahrdSummonGolemSound,
        LycanChargeSound,
        LycanChargeTickSound,
        LycanRewindSound,
        LycanTeleportSound,
        MushfolkTeleportSound,
        TabaxiDashChargeSound,
        TabaxiDashExecuteSound,
        TabaxiSetTeleportSound,
        TabaxiUseTeleportSound,
        VampireTransformSound,
        VampireExitTransformationSound,
        VampireShootTongueSound,
        SkeletonSummonSound,
        SkeletonTeleportSound,
        SoulbeastRendSound,
        SoulbeastRendExitSound,
        EntangledStrandsConfirmSound,

        FluftrodonChargeDust,
        FluftrodonJumpDust,
        GoblinHarvesterFireballDust,
        GoblinHarvesterShootDust,
        KoboldExplosionSparkDust,
        LycanRedDust,
        LycanBlueDust,
        LycanTeleportBurstDust,
        TabaxiTeleportDust,
        VampireTransformDust,
        SoulbeastRendDust,
        EntangledStrandsDust

    }
    public class MrPlagueRaces : Mod
	{
		public static ModKeybind RaceAbilityKeybind1;
		public static ModKeybind RaceAbilityKeybind2;
		public static ModKeybind RaceAbilityKeybind3;
		public static ModKeybind RaceAbilityKeybind4;

		public override void Load() 
		{
			// Load keybinds
			RaceAbilityKeybind1 = KeybindLoader.RegisterKeybind(this, "Primary Racial Ability", "Z");
			RaceAbilityKeybind2 = KeybindLoader.RegisterKeybind(this, "Secondary Racial Ability", "X");
			RaceAbilityKeybind3 = KeybindLoader.RegisterKeybind(this, "Tertiary Racial Ability", "C");
			RaceAbilityKeybind4 = KeybindLoader.RegisterKeybind(this, "Quaternary Racial Ability", "V");
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI) 
		{
			// Send custom player data to clients
			MrPlagueRacesMessageType msgType = (MrPlagueRacesMessageType)reader.ReadByte();

			switch (msgType) 
			{
				case MrPlagueRacesMessageType.ExecuteRaceSound:
					byte playernumber = reader.ReadByte();
					MrPlagueRacesPlayer MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();
					MrPlagueRacesPlayer.ExecuteRaceSound(Main.player[playernumber], reader.ReadString());
                    //Main.NewText("Receiving ExecuteRaceSound from... " + Main.player[playernumber].name, 255, 255, 255);
                    break;
				case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncRace:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();
                    int PlayerRace = reader.ReadInt32();
					if (RaceLoader.TryGetRace(PlayerRace, out var race))
					{
						MrPlagueRacesPlayer.race = race;
                    }
                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncRace(-1, whoAmI);
                    }
                    //Main.NewText("Receiving SyncRace from... " + Main.player[playernumber].name, 255, 255, 255);
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncSkinColor:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].skinColor.R = reader.ReadByte();
                    Main.player[playernumber].skinColor.G = reader.ReadByte();
                    Main.player[playernumber].skinColor.B = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncSkinColor(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncHairColor:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].hairColor.R = reader.ReadByte();
                    Main.player[playernumber].hairColor.G = reader.ReadByte();
                    Main.player[playernumber].hairColor.B = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncHairColor(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncEyeColor:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].eyeColor.R = reader.ReadByte();
                    Main.player[playernumber].eyeColor.G = reader.ReadByte();
                    Main.player[playernumber].eyeColor.B = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncEyeColor(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncDetailColor:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    MrPlagueRacesPlayer.detailColor.R = reader.ReadByte();
                    MrPlagueRacesPlayer.detailColor.G = reader.ReadByte();
                    MrPlagueRacesPlayer.detailColor.B = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncDetailColor(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncAuxDetail1Color:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    MrPlagueRacesPlayer.auxilaryDetailColor1.R = reader.ReadByte();
                    MrPlagueRacesPlayer.auxilaryDetailColor1.G = reader.ReadByte();
                    MrPlagueRacesPlayer.auxilaryDetailColor1.B = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncAuxDetail1Color(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncAuxDetail2Color:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    MrPlagueRacesPlayer.auxilaryDetailColor2.R = reader.ReadByte();
                    MrPlagueRacesPlayer.auxilaryDetailColor2.G = reader.ReadByte();
                    MrPlagueRacesPlayer.auxilaryDetailColor2.B = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncAuxDetail2Color(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncAuxDetail3Color:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    MrPlagueRacesPlayer.auxilaryDetailColor3.R = reader.ReadByte();
                    MrPlagueRacesPlayer.auxilaryDetailColor3.G = reader.ReadByte();
                    MrPlagueRacesPlayer.auxilaryDetailColor3.B = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncAuxDetail3Color(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncShirtColor:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].shirtColor.R = reader.ReadByte();
                    Main.player[playernumber].shirtColor.G = reader.ReadByte();
                    Main.player[playernumber].shirtColor.B = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncShirtColor(-1, whoAmI);
                    }
                    break;
				case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncUndershirtColor:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].underShirtColor.R = reader.ReadByte();
                    Main.player[playernumber].underShirtColor.G = reader.ReadByte();
                    Main.player[playernumber].underShirtColor.B = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncUndershirtColor(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncPantsColor:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].pantsColor.R = reader.ReadByte();
                    Main.player[playernumber].pantsColor.G = reader.ReadByte();
                    Main.player[playernumber].pantsColor.B = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncPantsColor(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncShoeColor:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].shoeColor.R = reader.ReadByte();
                    Main.player[playernumber].shoeColor.G = reader.ReadByte();
                    Main.player[playernumber].shoeColor.B = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncShoeColor(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncHairstyles:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].hair = reader.ReadInt32();
					MrPlagueRacesPlayer.auxilaryHairstyle1 = reader.ReadInt32();
                    MrPlagueRacesPlayer.auxilaryHairstyle2 = reader.ReadInt32();
                    MrPlagueRacesPlayer.auxilaryHairstyle3 = reader.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncHairstyles(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncClothStyle:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].skinVariant = reader.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncClothStyle(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncPosition:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].position.X = reader.ReadSingle();
                    Main.player[playernumber].position.Y = reader.ReadSingle();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncPosition(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncVelocity:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].velocity.X = reader.ReadSingle();
                    Main.player[playernumber].velocity.Y = reader.ReadSingle();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncVelocity(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncRotationsAndOffsets:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].fullRotationOrigin.X = reader.ReadSingle();
                    Main.player[playernumber].fullRotationOrigin.Y = reader.ReadSingle();

                    Main.player[playernumber].fullRotation = reader.ReadSingle();

                    Main.player[playernumber].headRotation = reader.ReadSingle();
                    Main.player[playernumber].bodyRotation = reader.ReadSingle();
                    Main.player[playernumber].legRotation = reader.ReadSingle();

                    Main.player[playernumber].headPosition.X = reader.ReadSingle();
                    Main.player[playernumber].headPosition.Y = reader.ReadSingle();

                    Main.player[playernumber].bodyPosition.X = reader.ReadSingle();
                    Main.player[playernumber].bodyPosition.Y = reader.ReadSingle();

                    Main.player[playernumber].legPosition.X = reader.ReadSingle();
                    Main.player[playernumber].legPosition.Y = reader.ReadSingle();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncRotationsAndOffsets(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.DerpkinSyncPlayer:
                    playernumber = reader.ReadByte();
                    DerpkinPlayer DerpkinPlayer = Main.player[playernumber].GetModPlayer<DerpkinPlayer>();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        DerpkinPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.DragonkinSyncPlayer:
                    playernumber = reader.ReadByte();
                    DragonkinPlayer DragonkinPlayer = Main.player[playernumber].GetModPlayer<DragonkinPlayer>();

                    DragonkinPlayer.headRotation = reader.ReadSingle();
                    DragonkinPlayer.targetHeadRotation = reader.ReadSingle();
                    DragonkinPlayer.breathingSmoke = reader.ReadBoolean();
                    DragonkinPlayer.firingSmoke = reader.ReadInt32();
                    DragonkinPlayer.burningOut = reader.ReadInt32();
                    DragonkinPlayer.soundInterval = reader.ReadInt32();
                    DragonkinPlayer.direction = reader.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        DragonkinPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.FluftrodonSyncPlayer:
                    playernumber = reader.ReadByte();
                    FluftrodonPlayer FluftrodonPlayer = Main.player[playernumber].GetModPlayer<FluftrodonPlayer>();

                    FluftrodonPlayer.selectedPaint = reader.ReadInt32();
                    FluftrodonPlayer.jumpCharge = reader.ReadSingle();
                    FluftrodonPlayer.canWallJump = reader.ReadBoolean();
                    FluftrodonPlayer.closeMenu = reader.ReadBoolean();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        FluftrodonPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.GoblinSyncPlayer:
                    playernumber = reader.ReadByte();
                    GoblinPlayer GoblinPlayer = Main.player[playernumber].GetModPlayer<GoblinPlayer>();

                    GoblinPlayer.harvesterCounter = reader.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        GoblinPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.KenkuSyncPlayer:
                    playernumber = reader.ReadByte();
                    KenkuPlayer KenkuPlayer = Main.player[playernumber].GetModPlayer<KenkuPlayer>();

                    KenkuPlayer.wingTime = reader.ReadSingle();
                    KenkuPlayer.flying = reader.ReadBoolean();
                    KenkuPlayer.wingFrame = reader.ReadInt32();
                    KenkuPlayer.wingFrameCounter = reader.ReadInt32();
                    KenkuPlayer.dashTime = reader.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        KenkuPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.KoboldSyncPlayer:
                    playernumber = reader.ReadByte();
                    KoboldPlayer KoboldPlayer = Main.player[playernumber].GetModPlayer<KoboldPlayer>();

                    KoboldPlayer.headRotation = reader.ReadSingle();
                    KoboldPlayer.targetHeadRotation = reader.ReadSingle();
                    KoboldPlayer.triggeringMine = reader.ReadInt32();
                    KoboldPlayer.firingMine = reader.ReadInt32();
                    KoboldPlayer.direction = reader.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        KoboldPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.LihzahrdSyncPlayer: // {T} Updated to match LihzahrdPlayer.SyncPlayer's new code.
                    playernumber = reader.ReadByte();
                    LihzahrdPlayer LihzahrdPlayer = Main.player[playernumber].GetModPlayer<LihzahrdPlayer>();

                    LihzahrdPlayer.direction = reader.ReadInt32();

                    /*LihzahrdPlayer.boulderGolemID = reader.ReadInt32();
                    LihzahrdPlayer.tetherGolemID = reader.ReadInt32();
                    LihzahrdPlayer.laserGolemID = reader.ReadInt32();
                    LihzahrdPlayer.barrierGolemID = reader.ReadInt32();
                    LihzahrdPlayer.spiderGolemID = reader.ReadInt32();
                    LihzahrdPlayer.lifeGolemID = reader.ReadInt32();

                    LihzahrdPlayer.boulderGolemLeftBoulderID = reader.ReadInt32();
                    LihzahrdPlayer.boulderGolemRightBoulderID = reader.ReadInt32();

                    LihzahrdPlayer.tetherGolemLeftTetherID = reader.ReadInt32();
                    LihzahrdPlayer.tetherGolemRightTetherID = reader.ReadInt32();
                    LihzahrdPlayer.tetherGolemMiddleTetherID = reader.ReadInt32();

                    LihzahrdPlayer.laserGolemLeftGripperID = reader.ReadInt32();
                    LihzahrdPlayer.laserGolemRightGripperID = reader.ReadInt32();
                    LihzahrdPlayer.laserGolemLaserID = reader.ReadInt32();

                    LihzahrdPlayer.barrierGolemLeftBarrierID = reader.ReadInt32();
                    LihzahrdPlayer.barrierGolemRightBarrierID = reader.ReadInt32();

                    LihzahrdPlayer.spiderGolemLeftGripperID = reader.ReadInt32();
                    LihzahrdPlayer.spiderGolemRightGripperID = reader.ReadInt32();*/

                    if (Main.netMode == NetmodeID.Server)
                    {
                        LihzahrdPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.LycanSyncPlayer: // {T} Added this to sync lycan ability state.
                    playernumber = reader.ReadByte();
                    RewindPlayer lycanPlayer = Main.player[playernumber].GetModPlayer<RewindPlayer>();

                    lycanPlayer.chargingRewind = reader.ReadBoolean();
                    lycanPlayer.chargingTimeline = reader.ReadBoolean();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        lycanPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.MerfolkSyncPlayer:
                    playernumber = reader.ReadByte();
                    MerfolkPlayer MerfolkPlayer = Main.player[playernumber].GetModPlayer<MerfolkPlayer>();

                    MerfolkPlayer.fullRotation = reader.ReadSingle();
                    MerfolkPlayer.targetFullRotation = reader.ReadSingle();
                    MerfolkPlayer.headRotation = reader.ReadSingle();
                    MerfolkPlayer.targetHeadRotation = reader.ReadSingle();
                    MerfolkPlayer.swimming = reader.ReadBoolean();
                    MerfolkPlayer.diveCount = reader.ReadInt32();
                    MerfolkPlayer.breathHurt = reader.ReadInt32();
                    MerfolkPlayer.breathInterval = reader.ReadInt32();
                    MerfolkPlayer.breathMeter = reader.ReadInt32();
                    MerfolkPlayer.direction = reader.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MerfolkPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.MushfolkSyncPlayer:
                    playernumber = reader.ReadByte();
                    MushfolkPlayer MushfolkPlayer = Main.player[playernumber].GetModPlayer<MushfolkPlayer>();

                    // {T} Fixed a read underflow error caused by not reading all of the data that was being sent in this packet from MushfolkPlayer.
                    MushfolkPlayer.growingMushrooms = reader.ReadBoolean();
                    MushfolkPlayer.sporeless = reader.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MushfolkPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.SkeletonSyncPlayer:
                    playernumber = reader.ReadByte();
                    SkeletonPlayer SkeletonPlayer = Main.player[playernumber].GetModPlayer<SkeletonPlayer>();

                    SkeletonPlayer.teleportOne = reader.ReadBoolean();
                    SkeletonPlayer.teleportTwo = reader.ReadBoolean();
                    SkeletonPlayer.teleportThree = reader.ReadBoolean();
                    SkeletonPlayer.spirit = reader.ReadInt32();
                    SkeletonPlayer.currentBody = reader.ReadInt32();
                    SkeletonPlayer.velocityX = reader.ReadSingle();
                    SkeletonPlayer.velocityY = reader.ReadSingle();
                    SkeletonPlayer.targetVelocityX = reader.ReadSingle();
                    SkeletonPlayer.targetVelocityY = reader.ReadSingle();
                    SkeletonPlayer.lastUnobstructedPosition.X = reader.ReadSingle();
                    SkeletonPlayer.lastUnobstructedPosition.Y = reader.ReadSingle();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        SkeletonPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.TabaxiSyncPlayer:
                    playernumber = reader.ReadByte();
                    TabaxiPlayer TabaxiPlayer = Main.player[playernumber].GetModPlayer<TabaxiPlayer>();

                    TabaxiPlayer.TabaxiSpawn.X = reader.ReadSingle();
                    TabaxiPlayer.TabaxiSpawn.Y = reader.ReadSingle();
                    TabaxiPlayer.phased = reader.ReadBoolean();
                    TabaxiPlayer.phaseChargeCounter = reader.ReadInt32();
                    TabaxiPlayer.phaseActiveCounter = reader.ReadInt32();
                    TabaxiPlayer.lastUnobstructedPosition.X = reader.ReadSingle();
                    TabaxiPlayer.lastUnobstructedPosition.Y = reader.ReadSingle();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        TabaxiPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.VampireSyncPlayer:
                    playernumber = reader.ReadByte();
                    VampirePlayer VampirePlayer = Main.player[playernumber].GetModPlayer<VampirePlayer>();

                    VampirePlayer.stealthTimer = reader.ReadInt32();
                    VampirePlayer.LeechTongue = reader.ReadInt32();
                    VampirePlayer.Leeching = reader.ReadBoolean();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        VampirePlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.SoulbeastSyncPlayer:
                    playernumber = reader.ReadByte();
                    SoulbeastPlayer SoulbeastPlayer = Main.player[playernumber].GetModPlayer<SoulbeastPlayer>();

                    SoulbeastPlayer.rending = reader.ReadBoolean();
                    SoulbeastPlayer.rendDelay = reader.ReadInt32();
                    SoulbeastPlayer.rendTimer = reader.ReadInt32();
                    SoulbeastPlayer.lastUnobstructedPosition.X = reader.ReadSingle();
                    SoulbeastPlayer.lastUnobstructedPosition.Y = reader.ReadSingle();
                    SoulbeastPlayer.direction = reader.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        SoulbeastPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncDismount:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].mount.Dismount(Main.player[playernumber]);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncDismount(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncCrawl:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].mount.SetMount(MountType<Crawl>(), Main.player[playernumber], false);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncCrawl(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncBat:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].mount.SetMount(MountType<StealthBat>(), Main.player[playernumber], false);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncBat(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncMouseWorld:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    MrPlagueRacesPlayer.mouseWorld.X = reader.ReadSingle();
                    MrPlagueRacesPlayer.mouseWorld.Y = reader.ReadSingle();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncMouseWorld(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncPlayerFrames:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    MrPlagueRacesPlayer.headFrame.X = reader.ReadInt32();
                    MrPlagueRacesPlayer.headFrame.Y = reader.ReadInt32();

                    MrPlagueRacesPlayer.bodyFrame.X = reader.ReadInt32();
                    MrPlagueRacesPlayer.bodyFrame.Y = reader.ReadInt32();

                    MrPlagueRacesPlayer.legFrame.X = reader.ReadInt32();
                    MrPlagueRacesPlayer.legFrame.Y = reader.ReadInt32();

                    MrPlagueRacesPlayer.hairFrame.X = reader.ReadInt32();
                    MrPlagueRacesPlayer.hairFrame.Y = reader.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncPlayerFrames(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncBlink:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    Main.player[playernumber].eyeHelper.BlinkBecausePlayerGotHurt();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SyncBlink(-1, whoAmI);
                    }
                    //Main.NewText("Receiving SyncCrawl from... " + Main.player[playernumber].name, 255, 255, 255);
                    break;
                case MrPlagueRacesMessageType.DerpkinLeapSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item39, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.DerpkinLeapSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.DerpkinSpinSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item1, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.DerpkinSpinSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.DragonkinFireballSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.DragonkinFireballSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.FluftrodonChargeSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item105, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.FluftrodonChargeSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.FluftrodonJumpSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item152, Main.player[playernumber].Center);
                    SoundEngine.PlaySound(SoundID.Item39, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.FluftrodonJumpSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.GoblinChargeSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryCircle, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.GoblinChargeSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.GoblinShootSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.GoblinShootSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.KenkuDashSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.KenkuDashSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.KenkuSummonFeathersSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.KenkuSummonFeathersSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.KenkuFlapSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item32, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.KenkuFlapSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.KoboldMineSummonSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.DD2_DrakinShot, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.KoboldMineSummonSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.KoboldClusterMineSummonSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.KoboldClusterMineSummonSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.KoboldExplosionSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.KoboldExplosionSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.LihzahrdSummonGolemSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    SoundEngine.PlaySound(SoundID.DD2_DefenseTowerSpawn, new Vector2(x,y));

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.LihzahrdSummonGolemSound(-1, whoAmI, x, y);
                    }
                    break;
                case MrPlagueRacesMessageType.LycanChargeSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item159, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.LycanChargeSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.LycanChargeTickSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.MenuTick, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.LycanChargeTickSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.LycanRewindSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item164, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.LycanRewindSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.LycanTeleportSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item117, Main.player[playernumber].Center);
                    SoundEngine.PlaySound(SoundID.Item130, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.LycanTeleportSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.MushfolkTeleportSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.NPCDeath58, Main.player[playernumber].Center);
                    SoundEngine.PlaySound(SoundID.NPCDeath55, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.MushfolkTeleportSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.TabaxiDashChargeSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item162, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.TabaxiDashChargeSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.TabaxiDashExecuteSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item163, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.TabaxiDashExecuteSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.TabaxiSetTeleportSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item105, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.TabaxiSetTeleportSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.TabaxiUseTeleportSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item165, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.TabaxiUseTeleportSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.VampireTransformSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.AbigailUpgrade, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.VampireTransformSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.VampireExitTransformationSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.AbigailAttack, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.VampireExitTransformationSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.VampireShootTongueSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item111, Main.player[playernumber].Center);
                    SoundEngine.PlaySound(SoundID.Item171, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.VampireShootTongueSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.SkeletonSummonSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SkeletonSummonSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.SkeletonTeleportSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SkeletonTeleportSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.SoulbeastRendSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Main.player[playernumber].Center);
                    SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, Main.player[playernumber].Center);
                    SoundEngine.PlaySound(SoundID.Zombie104, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SoulbeastRendSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.SoulbeastRendExitSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Main.player[playernumber].Center);
                    SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, Main.player[playernumber].Center);
                    SoundEngine.PlaySound(SoundID.Zombie103, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SoulbeastRendExitSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.EntangledStrandsConfirmSound:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    SoundEngine.PlaySound(SoundID.Item176, Main.player[playernumber].Center);
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, Main.player[playernumber].Center);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.EntangledStrandsConfirmSound(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.FluftrodonChargeDust:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();
                    FluftrodonPlayer = Main.player[playernumber].GetModPlayer<FluftrodonPlayer>();

                    for (int i = 0; i < 10; i++)
                    {
                        int dust = Dust.NewDust(Main.player[playernumber].position, Main.player[playernumber].width, Main.player[playernumber].height, 264);
                        Main.dust[dust].color = FluftrodonPlayer.paintColor[FluftrodonPlayer.selectedPaint];
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 2f;
                        dust = Dust.NewDust(Main.player[playernumber].position, Main.player[playernumber].width, Main.player[playernumber].height, 264);
                        Main.dust[dust].color = FluftrodonPlayer.paintColor[FluftrodonPlayer.selectedPaint];
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 1f;
                    }

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.FluftrodonChargeDust(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.FluftrodonJumpDust:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();
                    FluftrodonPlayer = Main.player[playernumber].GetModPlayer<FluftrodonPlayer>();

                    for (int i = 0; i < 5; i++)
                    {
                        int dust = Dust.NewDust(Main.player[playernumber].position, Main.player[playernumber].width, Main.player[playernumber].height, 264);
                        Main.dust[dust].color = FluftrodonPlayer.paintColor[FluftrodonPlayer.selectedPaint];
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 5f;
                        dust = Dust.NewDust(Main.player[playernumber].position, Main.player[playernumber].width, Main.player[playernumber].height, 264);
                        Main.dust[dust].color = FluftrodonPlayer.paintColor[FluftrodonPlayer.selectedPaint];
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 4f;
                    }

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.FluftrodonJumpDust(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.GoblinHarvesterFireballDust:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    for (int i = 0; i < 30; i++)
                    {
                        Dust dust19 = Dust.NewDustDirect(new Vector2(Main.player[playernumber].Center.X + (Main.player[playernumber].direction == 1 ? 0 : -10), Main.player[playernumber].Center.Y - 30), 0, 0, 27, 0f, Main.player[playernumber].velocity.Y * 0.4f, 180, default(Color), 1.95f);
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

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.GoblinHarvesterFireballDust(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.GoblinHarvesterShootDust:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    for (int i = 0; i < 30; i++)
                    {
                        int dust = Dust.NewDust(new Vector2(Main.player[playernumber].Center.X + (Main.player[playernumber].direction == 1 ? 0 : -10), Main.player[playernumber].Center.Y - 30), 0, 0, 27);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 3f;
                        dust = Dust.NewDust(new Vector2(Main.player[playernumber].Center.X + (Main.player[playernumber].direction == 1 ? 0 : -10), Main.player[playernumber].Center.Y - 30), 0, 0, 27);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 2f;
                    }

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.GoblinHarvesterShootDust(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.KoboldExplosionSparkDust:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    x = reader.ReadSingle();
                    y = reader.ReadSingle();

                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();

                    int goreIndex = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(x + (float)(width / 2) - 24f, y + (float)(height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                    Main.gore[goreIndex].scale = 0.6f;
                    Main.gore[goreIndex].alpha = 100;
                    Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                    Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                    goreIndex = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(x + (float)(width / 2) - 24f, y + (float)(height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                    Main.gore[goreIndex].scale = 0.6f;
                    Main.gore[goreIndex].alpha = 100;
                    Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                    Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                    goreIndex = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(x + (float)(width / 2) - 24f, y + (float)(height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                    Main.gore[goreIndex].scale = 0.6f;
                    Main.gore[goreIndex].alpha = 100;
                    Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                    Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
                    goreIndex = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(x + (float)(width / 2) - 24f, y + (float)(height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                    Main.gore[goreIndex].scale = 0.6f;
                    Main.gore[goreIndex].alpha = 100;
                    Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                    Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;

                    for (int i = 0; i < 20; i++)
                    {
                        int dust = Dust.NewDust(new Vector2(x, y), width, height, 278);
                        Main.dust[dust].color = Main.player[playernumber].eyeColor;
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 3f;
                        dust = Dust.NewDust(new Vector2(x, y), width, height, 278);
                        Main.dust[dust].color = Main.player[playernumber].eyeColor;
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 2f;
                    }

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.KoboldExplosionSparkDust(-1, whoAmI, x, y, width, height);
                    }
                    break;
                case MrPlagueRacesMessageType.LycanRedDust:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    x = reader.ReadSingle();
                    y = reader.ReadSingle();
                    float scale = reader.ReadSingle();

                    int dustMark = Dust.NewDust(new Vector2(x + (Main.player[playernumber].direction == 1 ? 8 : 4), y + 8), 0, 0, 66);
                    Main.dust[dustMark].color = new Color(255, 0, 0);
                    Main.dust[dustMark].velocity = Vector2.Zero;
                    Main.dust[dustMark].scale = scale;
                    Main.dust[dustMark].noGravity = true;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.LycanRedDust(-1, whoAmI, x, y, scale);
                    }
                    break;
                case MrPlagueRacesMessageType.LycanBlueDust:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    x = reader.ReadSingle();
                    y = reader.ReadSingle();
                    scale = reader.ReadSingle();

                    dustMark = Dust.NewDust(new Vector2(x + (Main.player[playernumber].direction == 1 ? 8 : 4), y + 8), 0, 0, 66);
                    Main.dust[dustMark].color = new Color(0, 0, 255);
                    Main.dust[dustMark].velocity = Vector2.Zero;
                    Main.dust[dustMark].scale = scale;
                    Main.dust[dustMark].noGravity = true;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.LycanBlueDust(-1, whoAmI, x, y, scale);
                    }
                    break;
                case MrPlagueRacesMessageType.LycanTeleportBurstDust:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    x = reader.ReadSingle();
                    y = reader.ReadSingle();

                    for (int i = 0; i < 40; i++)
                    {
                        int dustRewind = Dust.NewDust(new Vector2(x + (Main.player[playernumber].direction == 1 ? 8 : 4), y + 8), 0, 0, 66);
                        Main.dust[dustRewind].color = new Color(0, 0, 255);
                        Main.dust[dustRewind].velocity *= 5f;
                        Main.dust[dustRewind].scale = 0.9f;
                        Main.dust[dustRewind].noGravity = true;
                    }

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.LycanTeleportBurstDust(-1, whoAmI, x, y);
                    }
                    break;
                case MrPlagueRacesMessageType.TabaxiTeleportDust:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    for (int i = 0; i < 25; i++)
                    {
                        int dust = Dust.NewDust(Main.player[playernumber].position, Main.player[playernumber].width, Main.player[playernumber].height, 263);
                        Main.dust[dust].color = Main.player[playernumber].eyeColor;
                        Main.dust[dust].noGravity = true;
                        Dust obj6 = Main.dust[dust];
                        obj6.velocity *= new Vector2(0.2f, 0.2f) + (Main.player[playernumber].velocity * 0.1f);
                    }

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.TabaxiTeleportDust(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.VampireTransformDust:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    int num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(Main.player[playernumber].position.X, Main.player[playernumber].position.Y - 10f), Main.player[playernumber].velocity, 99);
                    Main.gore[num].velocity *= 0.3f;
                    num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(Main.player[playernumber].position.X, Main.player[playernumber].position.Y + (float)(Main.player[playernumber].height / 2) - 10f), Main.player[playernumber].velocity, 99);
                    Main.gore[num].velocity *= 0.3f;
                    num = Gore.NewGore(Wiring.GetProjectileSource(0, 0), new Vector2(Main.player[playernumber].position.X, Main.player[playernumber].position.Y + (float)Main.player[playernumber].height - 10f), Main.player[playernumber].velocity, 99);
                    Main.gore[num].velocity *= 0.3f;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.VampireTransformDust(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.SoulbeastRendDust:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    for (int i = 0; i < 10; i++)
                    {
                        int dust = Dust.NewDust(Main.player[playernumber].position, Main.player[playernumber].width, Main.player[playernumber].height, 264);
                        Main.dust[dust].color = Main.player[playernumber].eyeColor;
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 5f;
                        dust = Dust.NewDust(Main.player[playernumber].position, Main.player[playernumber].width, Main.player[playernumber].height, 264);
                        Main.dust[dust].color = Main.player[playernumber].eyeColor;
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 4f;
                    }

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.SoulbeastRendDust(-1, whoAmI);
                    }
                    break;
                case MrPlagueRacesMessageType.EntangledStrandsDust:
                    playernumber = reader.ReadByte();
                    MrPlagueRacesPlayer = Main.player[playernumber].GetModPlayer<MrPlagueRacesPlayer>();

                    int animTime = reader.ReadInt32();
                    MrPlagueRacesPlayer.StrandAnimation(animTime, 295, 296);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        MrPlagueRacesPlayer.EntangledStrandsDust(-1, whoAmI, animTime);
                    }
                    break;
            }
        }
        public override void Unload()
		{
			// Unload keybinds
			RaceAbilityKeybind1 = null;
			RaceAbilityKeybind2 = null;
			RaceAbilityKeybind3 = null;
			RaceAbilityKeybind4 = null;

            // Return all player sheets to normal when the mod is disabled
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