using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Utilities.FileBrowser;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MrPlagueRaces.Common.Races
{
	public abstract class Race : ModType
	{
		public int Id { get; internal set; }
		public Mod mod { get; internal set; }
        public string DisplayName = null;
		public string Description = null;
		public string AbilitiesDescription = null;

		public string TextureLocation = "Assets/Textures/Players/Races";
		public string SoundLocation = "Assets/Sounds/Players/Races";
		public int ClothStyle = 1;
		public int HairStyle = 0;
		public bool CensorClothing = true;
		public bool StarterShirt = false;
		public bool StarterPants = false;
		public bool AlwaysDrawHair = false;
		public Color HairColor = new Color(215, 90, 55);
		public Color SkinColor = new Color(255, 125, 90);
		public Color DetailColor = new Color(255, 125, 90);
        public Color AuxilaryDetailColor1 = new Color(0, 0, 0) * 0;
        public Color AuxilaryDetailColor2 = new Color(0, 0, 0) * 0;
        public Color AuxilaryDetailColor3 = new Color(0, 0, 0) * 0;
        public Color EyeColor = new Color(105, 90, 75);
		public Color ShirtColor = new Color(175, 165, 140);
		public Color UnderShirtColor = new Color(160, 180, 215);
		public Color PantsColor = new Color(255, 230, 175);
		public Color ShoeColor = new Color(160, 105, 60);
        public int AuxilaryHairstyle1 = 0;
        public int AuxilaryHairstyle2 = 0;
        public int AuxilaryHairstyle3 = 0;

        // The race's sprite sheets
        public RaceSheet[] Arm_Sheet = new RaceSheet[16];
        public RaceSheet[,] Arm_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Arm_Auxilary1_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Arm_Auxilary2_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Arm_Auxilary3_StyleSheet = new RaceSheet[16, 165];

        public RaceSheet[] Hand_Sheet = new RaceSheet[16];
        public RaceSheet[,] Hand_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Hand_Auxilary1_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Hand_Auxilary2_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Hand_Auxilary3_StyleSheet = new RaceSheet[16, 165];

        public RaceSheet[] Body_Sheet = new RaceSheet[16];
        public RaceSheet[,] Body_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Body_Auxilary1_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Body_Auxilary2_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Body_Auxilary3_StyleSheet = new RaceSheet[16, 165];

        public RaceSheet[] Legs_Sheet = new RaceSheet[16];
        public RaceSheet[,] Legs_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Legs_Auxilary1_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Legs_Auxilary2_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Legs_Auxilary3_StyleSheet = new RaceSheet[16, 165];

        public RaceSheet[,] Hair_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Hair_Auxilary1_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Hair_Auxilary2_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Hair_Auxilary3_StyleSheet = new RaceSheet[16, 165];

        public RaceSheet[,] HairAlt_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] HairAlt_Auxilary1_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] HairAlt_Auxilary2_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] HairAlt_Auxilary3_StyleSheet = new RaceSheet[16, 165];

        public RaceSheet[] Head_Sheet = new RaceSheet[16];
        public RaceSheet[,] Head_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Head_Auxilary1_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Head_Auxilary2_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Head_Auxilary3_StyleSheet = new RaceSheet[16, 165];

        public RaceSheet[] EyeLids_Sheet = new RaceSheet[16];
        public RaceSheet[,] EyeLids_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] EyeLids_Auxilary1_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] EyeLids_Auxilary2_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] EyeLids_Auxilary3_StyleSheet = new RaceSheet[16, 165];

        public RaceSheet[] Eyes_Sheet = new RaceSheet[16];
        public RaceSheet[,] Eyes_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Eyes_Auxilary1_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Eyes_Auxilary2_StyleSheet = new RaceSheet[16, 165];
        public RaceSheet[,] Eyes_Auxilary3_StyleSheet = new RaceSheet[16, 165];

        public RaceSheet[] Shirt_Sheet = new RaceSheet[13];
        public RaceSheet[] Undershirt_Sheet = new RaceSheet[13];
        public RaceSheet[] Pants_Sheet = new RaceSheet[13];
        public RaceSheet[] Shoes_Sheet = new RaceSheet[13];
        public RaceSheet[] ShirtAddition_Sheet = new RaceSheet[13];
        public RaceSheet[] PantsAddition_Sheet = new RaceSheet[13];
        public RaceSheet CensorShirt_Sheet;
        public RaceSheet CensorPants_Sheet;

        public bool hasRegisteredAbilityDescriptions = false;

        public Dictionary<RacialStatType, RacialStatPercentageModifier> RaceStatDictionary = null;

        // Function for inserting a new Racial Ability Description in the player creation UI. Takes a boolean to determine whether it should display
        public void RegisterAbilityDescription(bool isAbilityEnabled, string abilityDescription)
		{
			if (isAbilityEnabled && ModContent.GetInstance<MrPlagueRacesConfig>().raceStats)
			{
                AbilitiesDescription += ((AbilitiesDescription == "" ? "" : "\n") + abilityDescription);
				hasRegisteredAbilityDescriptions = true;
            }
        }

        protected sealed override void Register()
		{
            RaceLoader.AddRace(this);
            ContentInstance.Register(this);
            SetStaticDefaults();
            ReloadSheetsFromDefaultFilepath();
        }

        public void ReloadSheetsFromDefaultFilepath()
        {
            // Load this race's spritesheets from the default filepath

            for (int i = 0; i < 5; i++) // loop through all 5 player clothing styles
            {
                this.Shirt_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = GetRaceSheet($"Clothes/Style_{i + 1}/Shirt", SheetCategory.Body, 0, -1, 0, $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Shirt");
                this.Undershirt_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = GetRaceSheet($"Clothes/Style_{i + 1}/Undershirt", SheetCategory.Body, 0, -1, 0, $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Undershirt");
                this.Pants_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = GetRaceSheet($"Clothes/Style_{i + 1}/Pants", SheetCategory.Legs, 0, -1, 0, $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Pants");
                this.Shoes_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = GetRaceSheet($"Clothes/Style_{i + 1}/Shoes", SheetCategory.Legs, 0, -1, 0, $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/Shoes");
                this.ShirtAddition_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = GetRaceSheet($"Clothes/Style_{i + 1}/ShirtAddition", SheetCategory.Body, 0, -1, 0, $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/ShirtAddition");
                this.PantsAddition_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = GetRaceSheet($"Clothes/Style_{i + 1}/PantsAddition", SheetCategory.Legs, 0, -1, 0, $"MrPlagueRaces/Assets/Textures/Players/Clothes/Male/Clothes/Style_{i + 1}/PantsAddition");
            }

            for (int i = 0; i < 5; i++) // loop through all 5 player clothing styles
            {
                this.Shirt_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = GetRaceSheet($"Clothes/Style_{i + 1}/Shirt", SheetCategory.Body, 0, -1, 0, $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/Shirt");
                this.Undershirt_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = GetRaceSheet($"Clothes/Style_{i + 1}/Undershirt", SheetCategory.Body, 0, -1, 0, $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/Undershirt");
                this.Pants_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = GetRaceSheet($"Clothes/Style_{i + 1}/Pants", SheetCategory.Legs, 0, -1, 0, $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/Pants");
                this.Shoes_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = GetRaceSheet($"Clothes/Style_{i + 1}/Shoes", SheetCategory.Legs, 0, -1, 0, $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/Shoes");
                this.ShirtAddition_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = GetRaceSheet($"Clothes/Style_{i + 1}/ShirtAddition", SheetCategory.Body, 0, -1, 0, $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/ShirtAddition");
                this.PantsAddition_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = GetRaceSheet($"Clothes/Style_{i + 1}/PantsAddition", SheetCategory.Legs, 0, -1, 0, $"MrPlagueRaces/Assets/Textures/Players/Clothes/Female/Clothes/Style_{i + 1}/PantsAddition");
            }

            this.CensorShirt_Sheet = GetRaceSheet("Clothes/CensorShirt", SheetCategory.Body, 0, -1, 0, "MrPlagueRaces/Assets/Textures/Players/Clothes/CensorShirt");
            this.CensorPants_Sheet = GetRaceSheet("Clothes/CensorPants", SheetCategory.Legs, 0, -1, 0, "MrPlagueRaces/Assets/Textures/Players/Clothes/CensorPants");

            for (int i = 0; i < 16; i++) // loop through all 16 player colors
            {
                this.Arm_Sheet[i] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Arms", SheetCategory.Arms, i, -1, 0);
                this.Hand_Sheet[i] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hands", SheetCategory.Hands, i, -1, 0);
                this.Body_Sheet[i] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Body", SheetCategory.Body, i, - 1, 0);
                this.Legs_Sheet[i] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Legs", SheetCategory.Legs, i, -1, 0);
                this.Head_Sheet[i] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Head", SheetCategory.Head, i, - 1, 0);
                this.EyeLids_Sheet[i] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/EyeLids", SheetCategory.EyeLids, i, -1, 0);
                this.Eyes_Sheet[i] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Eyes", SheetCategory.Eyes, i, -1, 0);

                for (int j = 0; j < 165; j++) // loop through all 165 possible player hairs
                {
                    this.Arm_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles/Arms_{j + 1}", SheetCategory.Arms, i, j, 0);
                    this.Arm_Auxilary1_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_1/Arms_{j + 1}", SheetCategory.Arms, i, j, 1);
                    this.Arm_Auxilary2_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_2/Arms_{j + 1}", SheetCategory.Arms, i, j, 2);
                    this.Arm_Auxilary3_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_3/Arms_{j + 1}", SheetCategory.Arms, i, j, 3);

                    this.Hand_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles/Hands_{j + 1}", SheetCategory.Hands, i, j ,0);
                    this.Hand_Auxilary1_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_1/Hands_{j + 1}", SheetCategory.Hands, i, j, 1);
                    this.Hand_Auxilary2_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_2/Hands_{j + 1}", SheetCategory.Hands, i, j, 2);
                    this.Hand_Auxilary3_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_3/Hands_{j + 1}", SheetCategory.Hands, i, j, 3);


                    this.Body_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles/Body_{j + 1}", SheetCategory.Body, i, j, 0);
                    this.Body_Auxilary1_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_1/Body_{j + 1}", SheetCategory.Body, i, j, 1);
                    this.Body_Auxilary2_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_2/Body_{j + 1}", SheetCategory.Body, i, j, 2);
                    this.Body_Auxilary3_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_3/Body_{j + 1}", SheetCategory.Body, i, j, 3);

                    this.Legs_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles/Legs_{j + 1}", SheetCategory.Legs, i, j, 0);
                    this.Legs_Auxilary1_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_1/Legs_{j + 1}", SheetCategory.Legs, i, j, 1);
                    this.Legs_Auxilary2_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_2/Legs_{j + 1}", SheetCategory.Legs, i, j, 2);
                    this.Legs_Auxilary3_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_3/Legs_{j + 1}", SheetCategory.Legs, i, j, 3);

                    this.Hair_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles/Hair_{j + 1}", SheetCategory.Hair, i, j, 0);
                    this.Hair_Auxilary1_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_1/Hair_{j + 1}", SheetCategory.Hair, i, j, 1);
                    this.Hair_Auxilary2_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_2/Hair_{j + 1}", SheetCategory.Hair, i, j, 2);
                    this.Hair_Auxilary3_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_3/Hair_{j + 1}", SheetCategory.Hair, i, j, 3);

                    this.HairAlt_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles/HairAlt_{j + 1}", SheetCategory.HairAlt, i, j, 0);
                    this.HairAlt_Auxilary1_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_1/HairAlt_{j + 1}", SheetCategory.HairAlt, i, j, 1);
                    this.HairAlt_Auxilary2_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_2/HairAlt_{j + 1}", SheetCategory.HairAlt, i, j, 2);
                    this.HairAlt_Auxilary3_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_3/HairAlt_{j + 1}", SheetCategory.HairAlt, i, j, 3);

                    this.Head_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles/Head_{j + 1}", SheetCategory.Head, i, j, 0);
                    this.Head_Auxilary1_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_1/Head_{j + 1}", SheetCategory.Head, i, j, 1);
                    this.Head_Auxilary2_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_2/Head_{j + 1}", SheetCategory.Head, i, j, 2);
                    this.Head_Auxilary3_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_3/Head_{j + 1}", SheetCategory.Head, i, j, 3);

                    this.EyeLids_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles/EyeLids_{j + 1}", SheetCategory.EyeLids, i, j, 0);
                    this.EyeLids_Auxilary1_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_1/EyeLids_{j + 1}", SheetCategory.EyeLids, i, j, 1);
                    this.EyeLids_Auxilary2_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_2/EyeLids_{j + 1}", SheetCategory.EyeLids, i, j, 2);
                    this.EyeLids_Auxilary3_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_3/EyeLids_{j + 1}", SheetCategory.EyeLids, i, j, 3);

                    this.Eyes_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles/Eyes_{j + 1}", SheetCategory.Eyes, i, j, 0);
                    this.Eyes_Auxilary1_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_1/Eyes_{j + 1}", SheetCategory.Eyes, i, j, 1);
                    this.Eyes_Auxilary2_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_2/Eyes_{j + 1}", SheetCategory.Eyes, i, j, 2);
                    this.Eyes_Auxilary3_StyleSheet[i, j] = GetRaceSheet($"{PlayerLayerHelpers.PlayerColors[i]}/Hairstyles_3/Eyes_{j + 1}", SheetCategory.Eyes, i, j, 3);
                }
            }
        }

        public void CopySheetsFromAnotherRace(Race race, bool copyFamiliarClothing = true, bool copyCensorClothing = true)
        {
            // Set all of this race's spritesheets to another race's spritesheets (clothing optional)
            if (copyFamiliarClothing)
            {
                for (int i = 0; i < 5; i++) // loop through all 5 player clothing styles
                {
                    this.Shirt_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = race.Shirt_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]];
                    this.Undershirt_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = race.Undershirt_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]];
                    this.Pants_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = race.Pants_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]];
                    this.Shoes_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = race.Shoes_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]];
                    this.ShirtAddition_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = race.ShirtAddition_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]];
                    this.PantsAddition_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = race.PantsAddition_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]];
                }

                for (int i = 0; i < 5; i++) // loop through all 5 player clothing styles
                {
                    this.Shirt_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = race.Shirt_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]];
                    this.Undershirt_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = race.Undershirt_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]];
                    this.Pants_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = race.Pants_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]];
                    this.Shoes_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = race.Shoes_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]];
                    this.ShirtAddition_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = race.ShirtAddition_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]];
                    this.PantsAddition_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = race.PantsAddition_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]];
                }
            }
            if (copyCensorClothing)
            {
                this.CensorShirt_Sheet = race.CensorShirt_Sheet;
                this.CensorPants_Sheet = race.CensorPants_Sheet;
            }

            for (int i = 0; i < 16; i++)  // loop through all 16 player colors
            {
                this.Arm_Sheet[i] = race.Arm_Sheet[i];
                this.Hand_Sheet[i] = race.Hand_Sheet[i];
                this.Body_Sheet[i] = race.Body_Sheet[i];
                this.Legs_Sheet[i] = race.Legs_Sheet[i];
                this.Head_Sheet[i] = race.Head_Sheet[i];
                this.EyeLids_Sheet[i] = race.EyeLids_Sheet[i];
                this.Eyes_Sheet[i] = race.Eyes_Sheet[i];

                for (int j = 0; j < 165; j++) // loop through all 165 possible player hairs
                {
                    this.Arm_StyleSheet[i, j] = race.Arm_StyleSheet[i, j];
                    this.Arm_Auxilary1_StyleSheet[i, j] = race.Arm_Auxilary1_StyleSheet[i, j];
                    this.Arm_Auxilary2_StyleSheet[i, j] = race.Arm_Auxilary2_StyleSheet[i, j];
                    this.Arm_Auxilary3_StyleSheet[i, j] = race.Arm_Auxilary3_StyleSheet[i, j];

                    this.Hand_StyleSheet[i, j] = race.Hand_StyleSheet[i, j];
                    this.Hand_Auxilary1_StyleSheet[i, j] = race.Hand_Auxilary1_StyleSheet[i, j];
                    this.Hand_Auxilary2_StyleSheet[i, j] = race.Hand_Auxilary2_StyleSheet[i, j];
                    this.Hand_Auxilary3_StyleSheet[i, j] = race.Hand_Auxilary3_StyleSheet[i, j];


                    this.Body_StyleSheet[i, j] = race.Body_StyleSheet[i, j];
                    this.Body_Auxilary1_StyleSheet[i, j] = race.Body_Auxilary1_StyleSheet[i, j];
                    this.Body_Auxilary2_StyleSheet[i, j] = race.Body_Auxilary2_StyleSheet[i, j];
                    this.Body_Auxilary3_StyleSheet[i, j] = race.Body_Auxilary3_StyleSheet[i, j];

                    this.Legs_StyleSheet[i, j] = race.Legs_StyleSheet[i, j];
                    this.Legs_Auxilary1_StyleSheet[i, j] = race.Legs_Auxilary1_StyleSheet[i, j];
                    this.Legs_Auxilary2_StyleSheet[i, j] = race.Legs_Auxilary2_StyleSheet[i, j];
                    this.Legs_Auxilary3_StyleSheet[i, j] = race.Legs_Auxilary3_StyleSheet[i, j];

                    this.Hair_StyleSheet[i, j] = race.Hair_StyleSheet[i, j];
                    this.Hair_Auxilary1_StyleSheet[i, j] = race.Hair_Auxilary1_StyleSheet[i, j];
                    this.Hair_Auxilary2_StyleSheet[i, j] = race.Hair_Auxilary2_StyleSheet[i, j];
                    this.Hair_Auxilary3_StyleSheet[i, j] = race.Hair_Auxilary3_StyleSheet[i, j];

                    this.HairAlt_StyleSheet[i, j] = race.HairAlt_StyleSheet[i, j];
                    this.HairAlt_Auxilary1_StyleSheet[i, j] = race.HairAlt_Auxilary1_StyleSheet[i, j];
                    this.HairAlt_Auxilary2_StyleSheet[i, j] = race.HairAlt_Auxilary2_StyleSheet[i, j];
                    this.HairAlt_Auxilary3_StyleSheet[i, j] = race.HairAlt_Auxilary3_StyleSheet[i, j];

                    this.Head_StyleSheet[i, j] = race.Head_StyleSheet[i, j];
                    this.Head_Auxilary1_StyleSheet[i, j] = race.Head_Auxilary1_StyleSheet[i, j];
                    this.Head_Auxilary2_StyleSheet[i, j] = race.Head_Auxilary2_StyleSheet[i, j];
                    this.Head_Auxilary3_StyleSheet[i, j] = race.Head_Auxilary3_StyleSheet[i, j];

                    this.EyeLids_StyleSheet[i, j] = race.EyeLids_StyleSheet[i, j];
                    this.EyeLids_Auxilary1_StyleSheet[i, j] = race.EyeLids_Auxilary1_StyleSheet[i, j];
                    this.EyeLids_Auxilary2_StyleSheet[i, j] = race.EyeLids_Auxilary2_StyleSheet[i, j];
                    this.EyeLids_Auxilary3_StyleSheet[i, j] = race.EyeLids_Auxilary3_StyleSheet[i, j];

                    this.Eyes_StyleSheet[i, j] = race.Eyes_StyleSheet[i, j];
                    this.Eyes_Auxilary1_StyleSheet[i, j] = race.Eyes_Auxilary1_StyleSheet[i, j];
                    this.Eyes_Auxilary2_StyleSheet[i, j] = race.Eyes_Auxilary2_StyleSheet[i, j];
                    this.Eyes_Auxilary3_StyleSheet[i, j] = race.Eyes_Auxilary3_StyleSheet[i, j];
                }
            }
        }

        public void ClearSheets(bool clearFamiliarClothing = true, bool clearCensorClothing = true)
        {
            // Clear all of this race's spritesheets (clothing optional)
            if (clearFamiliarClothing)
            {
                for (int i = 0; i < 5; i++) // loop through all 5 player clothing styles
                {
                    this.Shirt_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = GetBlankRaceSheet();
                    this.Undershirt_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = GetBlankRaceSheet();
                    this.Pants_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = GetBlankRaceSheet();
                    this.Shoes_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = GetBlankRaceSheet();
                    this.ShirtAddition_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = GetBlankRaceSheet();
                    this.PantsAddition_Sheet[PlayerLayerHelpers.MaleClothingIDs[i]] = GetBlankRaceSheet();
                }

                for (int i = 0; i < 5; i++) // loop through all 5 player clothing styles
                {
                    this.Shirt_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = GetBlankRaceSheet();
                    this.Undershirt_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = GetBlankRaceSheet();
                    this.Pants_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = GetBlankRaceSheet();
                    this.Shoes_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = GetBlankRaceSheet();
                    this.ShirtAddition_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = GetBlankRaceSheet();
                    this.PantsAddition_Sheet[PlayerLayerHelpers.FemaleClothingIDs[i]] = GetBlankRaceSheet();
                }
            }
            if (clearCensorClothing)
            {
                this.CensorShirt_Sheet = GetBlankRaceSheet();
                this.CensorPants_Sheet = GetBlankRaceSheet();
            }

            for (int i = 0; i < 16; i++)  // loop through all 16 player colors
            {
                this.Arm_Sheet[i] = GetBlankRaceSheet();
                this.Hand_Sheet[i] = GetBlankRaceSheet();
                this.Body_Sheet[i] = GetBlankRaceSheet();
                this.Legs_Sheet[i] = GetBlankRaceSheet();
                this.Head_Sheet[i] = GetBlankRaceSheet();
                this.EyeLids_Sheet[i] = GetBlankRaceSheet();
                this.Eyes_Sheet[i] = GetBlankRaceSheet();

                for (int j = 0; j < 165; j++) // loop through all 165 possible player hairs
                {
                    this.Arm_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Arm_Auxilary1_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Arm_Auxilary2_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Arm_Auxilary3_StyleSheet[i, j] = GetBlankRaceSheet();

                    this.Hand_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Hand_Auxilary1_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Hand_Auxilary2_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Hand_Auxilary3_StyleSheet[i, j] = GetBlankRaceSheet();


                    this.Body_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Body_Auxilary1_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Body_Auxilary2_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Body_Auxilary3_StyleSheet[i, j] = GetBlankRaceSheet();

                    this.Legs_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Legs_Auxilary1_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Legs_Auxilary2_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Legs_Auxilary3_StyleSheet[i, j] = GetBlankRaceSheet();

                    this.Hair_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Hair_Auxilary1_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Hair_Auxilary2_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Hair_Auxilary3_StyleSheet[i, j] = GetBlankRaceSheet();

                    this.HairAlt_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.HairAlt_Auxilary1_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.HairAlt_Auxilary2_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.HairAlt_Auxilary3_StyleSheet[i, j] = GetBlankRaceSheet();

                    this.Head_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Head_Auxilary1_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Head_Auxilary2_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Head_Auxilary3_StyleSheet[i, j] = GetBlankRaceSheet();

                    this.EyeLids_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.EyeLids_Auxilary1_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.EyeLids_Auxilary2_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.EyeLids_Auxilary3_StyleSheet[i, j] = GetBlankRaceSheet();

                    this.Eyes_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Eyes_Auxilary1_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Eyes_Auxilary2_StyleSheet[i, j] = GetBlankRaceSheet();
                    this.Eyes_Auxilary3_StyleSheet[i, j] = GetBlankRaceSheet();
                }
            }
        }

        public virtual void PostSetRaceValues(ref Player player, ref Player clonePlayer, ref bool shouldReplaceHair, ref bool shouldreplaceClothingStyle, ref bool shouldReplaceClothingColors, ref bool shouldReplaceSkinValues) { } // use this to apply custom data after your race is selected
        public virtual void PreRaceChange(Player player) { } // Use this to reset variables when your race is about to be switched to
        public virtual void PostRaceChange(Player player) { } // Use this for custom behavior when your race has been switched to
        public virtual void ResetEffects(Player player) { }
        public virtual void UpdateDead(Player player) { }
        public virtual void UpdateBadLifeRegen(Player player) { }
        public virtual void UpdateLifeRegen(Player player) { }
        public virtual void NaturalLifeRegen(Player player, ref float regen) { }
        public virtual void PreUpdate(Player player) { }
        public virtual void PostUpdate(Player player) { }
        public virtual void ProcessTriggers(Player player, TriggersSet triggersSet) { }
        public virtual void ModifyHurt(Player player, ref Player.HurtModifiers modifiers) { }
        public virtual void PostHurt(Player player, Player.HurtInfo info) { }
        public virtual bool PreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource) => true;
        public virtual void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) { }
        public virtual void OnHitAnything(Player player, float x, float y, Entity victim) { }
        public virtual void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers) { }
        public virtual void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers) { }
        public virtual void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual bool CanHitPvp(Player player, Item item, Player target) => true;
        public virtual void OnHitPvp(Player player, Item item, Player target, int damage, bool crit) { }
        public virtual bool CanHitPvpWithProj(Player player, Projectile proj, Player target) => true;
        public virtual bool CanBeHitByNPC(Player player, NPC npc, ref int cooldownSlot) => true;
        public virtual void ModifyHitByNPC(Player player, NPC npc, ref Player.HurtModifiers modifiers) { }
        public virtual void OnHitByNPC(Player player, NPC npc, Player.HurtInfo hurtInfo) { }
        public virtual bool CanBeHitByProjectile(Player player, Projectile proj) => true;
        public virtual bool FreeDodge(Player player, Player.HurtInfo info) => false;
        public virtual void ModifyHitByProjectile(Player player, Projectile proj, ref Player.HurtModifiers modifiers) { }
        public virtual void OnHitByProjectile(Player player, Projectile proj, Player.HurtInfo hurtInfo) { }
        public virtual void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo) { }
        public virtual void ModifyDrawLayerOrdering(Player player, IDictionary<PlayerDrawLayer, PlayerDrawLayer.Position> positions) { }
        public virtual void HideDrawLayers(Player player, PlayerDrawSet drawInfo) { }
        public virtual void ModifyScreenPosition(Player player) { }
        public virtual void ModifyZoom(Player player, ref float zoom) { }
        public virtual void OnEnterWorld(Player player) { }
        public virtual void OnRespawn(Player player) { }
        public virtual bool CanUseItem(Player player, Item item) => true;
        public virtual IEnumerable<Item> AddStartingItems(Player player, bool mediumCoreDeath) => Enumerable.Empty<Item>();
        public virtual void ModifyStartingInventory(Player player, IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath) { }

        //  Grabs a racial sheet via filepath
        public RaceSheet GetRaceSheet(string texturePath, SheetCategory sheetType = SheetCategory.Head, int colorType = 0, int hairNumber = -1, int hairTrackNumber = 0, string defaultTexturePath = "MrPlagueRaces/Assets/Textures/Blank")
        {
            Asset<Texture2D>[] raceTexture = new Asset<Texture2D>[2];
            raceTexture[0] = ModContent.Request<Texture2D>($"{defaultTexturePath}");
            raceTexture[1] = ModContent.Request<Texture2D>($"{defaultTexturePath}");
            if (ModContent.HasAsset($"{this.Mod.Name}/{this.TextureLocation}/{this.Name}/Male/{texturePath}"))
            {
                raceTexture[0] = ModContent.Request<Texture2D>($"{this.Mod.Name}/{this.TextureLocation}/{this.Name}/Male/{texturePath}");
            }
            if (ModContent.HasAsset($"{this.Mod.Name}/{this.TextureLocation}/{this.Name}/Female/{texturePath}"))
            {
                raceTexture[1] = ModContent.Request<Texture2D>($"{this.Mod.Name}/{this.TextureLocation}/{this.Name}/Female/{texturePath}");
            }
            else
            {
                raceTexture[1] = raceTexture[0];
            }
            RaceSheet raceSheet = new RaceSheet(raceTexture, sheetType, colorType, hairNumber, hairTrackNumber);
            return raceSheet;
        }

        //  Grabs a racial sheet via IDs (sheet type, color type, hairstyle, and hairstyle track)
        public ref RaceSheet GetRaceSheetFromIDs(SheetCategory sheetType, int colorType = 0, int hairNumber = -1, int hairTrackNumber = 0)
        {
            switch (sheetType) {
                case SheetCategory.Arms:
                    if (hairNumber == -1)
                    {
                        return ref this.Arm_Sheet[colorType];
                    }
                    else
                    {
                        switch (hairTrackNumber)
                        {
                            case 0:
                                return ref this.Arm_StyleSheet[colorType, hairNumber];
                                break;
                            case 1:
                                return ref this.Arm_Auxilary1_StyleSheet[colorType, hairNumber];
                                break;
                            case 2:
                                return ref this.Arm_Auxilary2_StyleSheet[colorType, hairNumber];
                                break;
                            case 3:
                                return ref this.Arm_Auxilary3_StyleSheet[colorType, hairNumber];
                                break;
                        }
                    }
                    break;
                case SheetCategory.Hands:
                    if (hairNumber == -1)
                    {
                        return ref this.Hand_Sheet[colorType];
                    }
                    else
                    {
                        switch (hairTrackNumber)
                        {
                            case 0:
                                return ref this.Hand_StyleSheet[colorType, hairNumber];
                                break;
                            case 1:
                                return ref this.Hand_Auxilary1_StyleSheet[colorType, hairNumber];
                                break;
                            case 2:
                                return ref this.Hand_Auxilary2_StyleSheet[colorType, hairNumber];
                                break;
                            case 3:
                                return ref this.Hand_Auxilary3_StyleSheet[colorType, hairNumber];
                                break;
                        }
                    }
                    break;
                case SheetCategory.Body:
                    if (hairNumber == -1)
                    {
                        return ref this.Body_Sheet[colorType];
                    }
                    else
                    {
                        switch (hairTrackNumber)
                        {
                            case 0:
                                return ref this.Body_StyleSheet[colorType, hairNumber];
                                break;
                            case 1:
                                return ref this.Body_Auxilary1_StyleSheet[colorType, hairNumber];
                                break;
                            case 2:
                                return ref this.Body_Auxilary2_StyleSheet[colorType, hairNumber];
                                break;
                            case 3:
                                return ref this.Body_Auxilary3_StyleSheet[colorType, hairNumber];
                                break;
                        }
                    }
                    break;
                case SheetCategory.Legs:
                    if (hairNumber == -1)
                    {
                        return ref this.Legs_Sheet[colorType];
                    }
                    else
                    {
                        switch (hairTrackNumber)
                        {
                            case 0:
                                return ref this.Legs_StyleSheet[colorType, hairNumber];
                                break;
                            case 1:
                                return ref this.Legs_Auxilary1_StyleSheet[colorType, hairNumber];
                                break;
                            case 2:
                                return ref this.Legs_Auxilary2_StyleSheet[colorType, hairNumber];
                                break;
                            case 3:
                                return ref this.Legs_Auxilary3_StyleSheet[colorType, hairNumber];
                                break;
                        }
                    }
                    break;
                case SheetCategory.Head:
                    if (hairNumber == -1)
                    {
                        return ref this.Head_Sheet[colorType];
                    }
                    else
                    {
                        switch (hairTrackNumber)
                        {
                            case 0:
                                return ref this.Head_StyleSheet[colorType, hairNumber];
                                break;
                            case 1:
                                return ref this.Head_Auxilary1_StyleSheet[colorType, hairNumber];
                                break;
                            case 2:
                                return ref this.Head_Auxilary2_StyleSheet[colorType, hairNumber];
                                break;
                            case 3:
                                return ref this.Head_Auxilary3_StyleSheet[colorType, hairNumber];
                                break;
                        }
                    }
                    break;
                case SheetCategory.EyeLids:
                    if (hairNumber == -1)
                    {
                        return ref this.EyeLids_Sheet[colorType];
                    }
                    else
                    {
                        switch (hairTrackNumber)
                        {
                            case 0:
                                return ref this.EyeLids_StyleSheet[colorType, hairNumber];
                                break;
                            case 1:
                                return ref this.EyeLids_Auxilary1_StyleSheet[colorType, hairNumber];
                                break;
                            case 2:
                                return ref this.EyeLids_Auxilary2_StyleSheet[colorType, hairNumber];
                                break;
                            case 3:
                                return ref this.EyeLids_Auxilary3_StyleSheet[colorType, hairNumber];
                                break;
                        }
                    }
                    break;
                case SheetCategory.Eyes:
                    if (hairNumber == -1)
                    {
                        return ref this.Eyes_Sheet[colorType];
                    }
                    else
                    {
                        switch (hairTrackNumber)
                        {
                            case 0:
                                return ref this.Eyes_StyleSheet[colorType, hairNumber];
                                break;
                            case 1:
                                return ref this.Eyes_Auxilary1_StyleSheet[colorType, hairNumber];
                                break;
                            case 2:
                                return ref this.Eyes_Auxilary2_StyleSheet[colorType, hairNumber];
                                break;
                            case 3:
                                return ref this.Eyes_Auxilary3_StyleSheet[colorType, hairNumber];
                                break;
                        }
                    }
                    break;
                case SheetCategory.Hair:
                    if (hairNumber == -1)
                    {
                        return ref this.Hair_StyleSheet[colorType, 0];
                    }
                    else
                    {
                        switch (hairTrackNumber)
                        {
                            case 0:
                                return ref this.Hair_StyleSheet[colorType, hairNumber];
                                break;
                            case 1:
                                return ref this.Hair_Auxilary1_StyleSheet[colorType, hairNumber];
                                break;
                            case 2:
                                return ref this.Hair_Auxilary2_StyleSheet[colorType, hairNumber];
                                break;
                            case 3:
                                return ref this.Hair_Auxilary3_StyleSheet[colorType, hairNumber];
                                break;
                        }
                    }
                    break;
                case SheetCategory.HairAlt:
                    if (hairNumber == -1)
                    {
                        return ref this.HairAlt_StyleSheet[colorType, 0];
                    }
                    else
                    {
                        switch (hairTrackNumber)
                        {
                            case 0:
                                return ref this.HairAlt_StyleSheet[colorType, hairNumber];
                                break;
                            case 1:
                                return ref this.HairAlt_Auxilary1_StyleSheet[colorType, hairNumber];
                                break;
                            case 2:
                                return ref this.HairAlt_Auxilary2_StyleSheet[colorType, hairNumber];
                                break;
                            case 3:
                                return ref this.HairAlt_Auxilary3_StyleSheet[colorType, hairNumber];
                                break;
                        }
                    }
                    break;
            }
            return ref this.Head_Sheet[colorType];
        }

        // Requests an image file from ModContent and applies it to a specified sheet (obtained via IDs).
        public void SetOverrideTextureFromModContent(string filePath, SheetCategory sheetType, int colorType = 0, int hairNumber = -1, int hairTrackNumber = 0, int gender = 0)
        {
            if (filePath != null)
            {

                Texture2D tempTexture = ModContent.Request<Texture2D>(filePath).Value;
                if (gender == 0 && this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[1] == this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[0])
                {
                    this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[1] = tempTexture;
                }
                this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[gender] = tempTexture;
            }
        }

        // Requests an image file from the user's computer and applies it to a specified sheet (obtained via IDs).
        public void SetOverrideTextureFromFilePath(string filePath, SheetCategory sheetType, int colorType = 0, int hairNumber = -1, int hairTrackNumber = 0, int gender = 0)
        {
            if (filePath != null)
            {

                try
                {
                    using (FileStream stream = File.OpenRead(filePath))
                    {
                        Texture2D tempTexture = Texture2D.FromStream(Main.instance.GraphicsDevice, stream);
                        if (gender == 0 && this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[1] == this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[0])
                        {
                            this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[1] = tempTexture;
                        }
                        this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[gender] = tempTexture;
                    }
                }
                catch (Exception exception)
                {
                    FancyErrorPrinter.ShowFailedToLoadAssetError(exception, filePath);
                }
            }
        }

        // Requests an image file from the user and applies it to a specified sheet (obtained via IDs).
        public void SetOverrideTextureFromUser(SheetCategory sheetType, int colorType = 0, int hairNumber = -1, int hairTrackNumber = 0, int gender = 0)
        {
            ExtensionFilter[] extensions = new ExtensionFilter[1]
            {
                new ExtensionFilter("Image files", "png")
            };
            string myImageFile = FileBrowser.OpenFilePanel("Select image file", extensions);
            if (myImageFile != null)
            {
                try
                {
                    using (FileStream stream = File.OpenRead(myImageFile))
                    {
                        Texture2D tempTexture = Texture2D.FromStream(Main.instance.GraphicsDevice, stream);
                        if (gender == 0 && this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[1] == this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[0])
                        {
                            this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[1] = tempTexture;
                        }
                        this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[gender] = tempTexture;
                    }
                }
                catch (Exception exception)
                {
                    FancyErrorPrinter.ShowFailedToLoadAssetError(exception, myImageFile);
                }
            }
        }

        // Requests and returns an image filepath from the user
        public string FilePathFromUser()
        {
            ExtensionFilter[] extensions = new ExtensionFilter[1]
            {
                new ExtensionFilter("Image files", "png")
            };
            string myImageFile = FileBrowser.OpenFilePanel("Select image file", extensions);
            if (myImageFile != null)
            {
                return myImageFile;
            }
            return null;
        }

        public void ClearOverrideTexture(SheetCategory sheetType, int colorType = 0, int hairNumber = -1, int hairTrackNumber = 0, int gender = 0)
        {
            if (gender == 0 && this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[1] == this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[0])
            {
                this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[1] = null;
            }
            if (gender == 1 && this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[0] != null)
            {
                this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[gender] = this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[0];
            }
            else
            {
                this.GetRaceSheetFromIDs(sheetType, colorType, hairNumber, hairTrackNumber).OverrideTexture[gender] = null;
            }
        }

        public RaceSheet GetBlankRaceSheet()
        {
            Asset<Texture2D>[] blankRaceTexture = { ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank"), ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank") };
            RaceSheet raceSheet = new RaceSheet(blankRaceTexture, SheetCategory.Head, 0, -1, 0);
            return raceSheet;
        }
    }

    public enum SheetCategory
    {
        Arms,
        Hands,
        Body,
        Legs,
        Head,
        EyeLids,
        Eyes,
        Hair,
        HairAlt
    }

    public struct RaceSheet // A more optimized approach to racial spritesheet handling
    {
		public Asset<Texture2D>[] Texture = new Asset<Texture2D>[2]; // Determines this sheet's texture for male [0] and female [1]].

        // None of the variables below control how the RaceSheet is rendered; they are solely for identification
		public SheetCategory SheetType { get; } // Reflects what player-part this RaceSheet is (head, body, etc)
        public int ColorType { get; } // Reflects what color is used for this RaceSheet (colorSkin, colorHair, etc)
        public int HairNumber { get; } // Reflects what hairstyle this RaceSheet is attached to (hair_1, hair_2, etc)
        public int HairTrackNumber { get; } // Reflects what hairstyle track this RaceSheet is attached to (player.hair, mrPlagueRacesPlayer.auxilaryHairstyle1, mrPlagueRacesPlayer.auxilaryHairstyle2, etc)
        public Texture2D[] OverrideTexture = { null, null };

        public RaceSheet(Asset<Texture2D>[] texture, SheetCategory sheetType, int colorType, int hairNumber, int hairTrackNumber)
        {
			Texture = texture;
            SheetType = sheetType;
			ColorType = colorType;
            HairNumber = hairNumber;
            HairTrackNumber = hairTrackNumber;
        }

        public bool IsBlank(int gender = 0)
        {
            return Texture[gender] == ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
        }

        public bool IsOverrideTextureBlank(int gender = 0)
        {
            return OverrideTexture[gender] == null;
        }
    }
}