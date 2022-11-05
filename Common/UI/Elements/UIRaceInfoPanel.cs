using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using MrPlagueRaces.Common.Players;
using MrPlagueRaces.Common.Systems;

namespace MrPlagueRaces.Common.UI.States
{
	public class UIRaceInfoPanel : UIList
	{
		private readonly Player _player;

		public int totalHeight;

		private int statLifeMax2Comparison = 100;
		private int lifeRegenComparison = 0;
		private int statManaMax2Comparison = 20;
		private int manaRegenBonusComparison = 0;
		private int statDefenseComparison = 0;
		private float enduranceComparison = 0f;
		private float thornsComparison = 0f;
		private int lavaMaxComparison = 0;
		private float meleeDamageComparison = 1f;
		private float meleeSpeedComparison = 1f;
		private float rangedDamageComparison = 1f;
		private float magicDamageComparison = 1f;
		private float manaCostComparison = 1f;
		private float minionDamageComparison = 1f;
		private int maxMinionsComparison = 1;
		private float minionKBComparison = 0f;
		private int maxTurretsComparison = 1;
		private float allDamageComparison = 1f;
		private float meleeCritComparison = 0f;
		private float rangedCritComparison = 0f;
		private float magicCritComparison = 0f;
		private float allCritComparison = 4f;
		private float armorPenetrationComparison = 0f;
		private float pickSpeedComparison = 1f;
		private float tileSpeedComparison = 1f;
		private float wallSpeedComparison = 1f;
		private int blockRangeComparison = 0;
		private float moveSpeedComparison = 1f;
		private float jumpSpeedBoostComparison = 0f;
		private int extraFallComparison = 0;
		private int fishingSkillComparison = 0;
		private int aggroComparison = 0;

		private int statLifeMax2Percentage = 100;
		private int lifeRegenPercentage = 0;
		private int statManaMax2Percentage = 20;
		private int manaRegenBonusPercentage = 0;
		private int statDefensePercentage = 0;
		private float endurancePercentage = 0f;
		private float thornsPercentage = 0f;
		private int lavaMaxPercentage = 0;
		private float meleeDamagePercentage = 1f;
		private float meleeSpeedPercentage = 1f;
		private float rangedDamagePercentage = 1f;
		private float magicDamagePercentage = 1f;
		private float manaCostPercentage = 1f;
		private float minionDamagePercentage = 1f;
		private int maxMinionsPercentage = 1;
		private float minionKBPercentage = 0f;
		private int maxTurretsPercentage = 1;
		private float allDamagePercentage = 1f;
		private float meleeCritPercentage = 0f;
		private float rangedCritPercentage = 0f;
		private float magicCritPercentage = 0f;
		private float allCritPercentage = 4f;
		private float armorPenetrationPercentage = 0f;
		private float pickSpeedPercentage = 1f;
		private float tileSpeedPercentage = 1f;
		private float wallSpeedPercentage = 1f;
		private int blockRangePercentage = 0;
		private float moveSpeedPercentage = 1f;
		private float jumpSpeedBoostPercentage = 0f;
		private int extraFallPercentage = 0;
		private int fishingSkillPercentage = 0;
		private int aggroPercentage = 0;

		private void ComparisonPlayerSetup()
		{
			var comparisonPlayer = _player.GetModPlayer<ComparisonPlayer>();
			comparisonPlayer.isComparison = true;
			_player.ResetEffects();
			statLifeMax2Comparison = _player.statLifeMax2;
			lifeRegenComparison = _player.lifeRegen;
			statManaMax2Comparison = _player.statManaMax2;
			manaRegenBonusComparison = _player.manaRegenBonus;
			statDefenseComparison = _player.statDefense;
			enduranceComparison = _player.endurance;
			thornsComparison = _player.thorns;
			lavaMaxComparison = _player.lavaMax;
			meleeDamageComparison = ((_player.GetDamage(DamageClass.Melee).Additive - 1) * 100);
			meleeSpeedComparison = _player.GetAttackSpeed(DamageClass.Melee);
			rangedDamageComparison = ((_player.GetDamage(DamageClass.Ranged).Additive - 1) * 100);
			magicDamageComparison = ((_player.GetDamage(DamageClass.Magic).Additive - 1) * 100);
			manaCostComparison = _player.manaCost;
			minionDamageComparison = ((_player.GetDamage(DamageClass.Summon).Additive - 1) * 100);
			maxMinionsComparison = _player.maxMinions;
			minionKBComparison = _player.GetKnockback(DamageClass.Summon).Base;
			maxTurretsComparison = _player.maxTurrets;
			allDamageComparison = ((_player.GetDamage(DamageClass.Generic).Additive - 1) * 100);
			meleeCritComparison = _player.GetCritChance(DamageClass.Melee);
			rangedCritComparison = _player.GetCritChance(DamageClass.Ranged);
			magicCritComparison = _player.GetCritChance(DamageClass.Magic);
			allCritComparison = _player.GetCritChance(DamageClass.Generic);
			armorPenetrationComparison = _player.GetArmorPenetration(DamageClass.Melee);
			pickSpeedComparison = _player.pickSpeed;
			tileSpeedComparison = _player.tileSpeed;
			wallSpeedComparison = _player.wallSpeed;
			blockRangeComparison = _player.blockRange;
			moveSpeedComparison = _player.moveSpeed;
			jumpSpeedBoostComparison = _player.jumpSpeedBoost;
			extraFallComparison = _player.extraFall;
			fishingSkillComparison = _player.fishingSkill;
			aggroComparison = _player.aggro;
			comparisonPlayer.isComparison = false;
			_player.ResetEffects();
		}

		private void PercentagePlayerSetup()
		{
			var comparisonPlayer = _player.GetModPlayer<ComparisonPlayer>();
			comparisonPlayer.isPercentage = true;
			_player.ResetEffects();
			statLifeMax2Percentage = _player.statLifeMax2;
			lifeRegenPercentage = _player.lifeRegen;
			statManaMax2Percentage = _player.statManaMax2;
			manaRegenBonusPercentage = _player.manaRegenBonus;
			statDefensePercentage = _player.statDefense;
			endurancePercentage = _player.endurance;
			thornsPercentage = _player.thorns;
			lavaMaxPercentage = _player.lavaMax;
			meleeDamagePercentage = ((_player.GetDamage(DamageClass.Melee).Additive - 1) * 100);
			meleeSpeedPercentage = _player.GetAttackSpeed(DamageClass.Melee);
			rangedDamagePercentage = ((_player.GetDamage(DamageClass.Ranged).Additive - 1) * 100);
			magicDamagePercentage = ((_player.GetDamage(DamageClass.Magic).Additive - 1) * 100);
			manaCostPercentage = _player.manaCost;
			minionDamagePercentage = ((_player.GetDamage(DamageClass.Summon).Additive - 1) * 100);
			maxMinionsPercentage = _player.maxMinions;
			minionKBPercentage = _player.GetKnockback(DamageClass.Summon).Base;
			maxTurretsPercentage = _player.maxTurrets;
			allDamagePercentage = ((_player.GetDamage(DamageClass.Generic).Additive - 1) * 100);
			meleeCritPercentage = _player.GetCritChance(DamageClass.Melee);
			rangedCritPercentage = _player.GetCritChance(DamageClass.Ranged);
			magicCritPercentage = _player.GetCritChance(DamageClass.Magic);
			allCritPercentage = _player.GetCritChance(DamageClass.Generic);
			armorPenetrationPercentage = _player.GetArmorPenetration(DamageClass.Melee);
			pickSpeedPercentage = _player.pickSpeed;
			tileSpeedPercentage = _player.tileSpeed;
			wallSpeedPercentage = _player.wallSpeed;
			blockRangePercentage = _player.blockRange;
			moveSpeedPercentage = _player.moveSpeed;
			jumpSpeedBoostPercentage = _player.jumpSpeedBoost;
			extraFallPercentage = _player.extraFall;
			fishingSkillPercentage = _player.fishingSkill;
			aggroPercentage = _player.aggro;
			comparisonPlayer.isPercentage = false;
			_player.ResetEffects();
		}
		
		public UISlicedImage descriptionBackground;
		public UIText descriptionText;
		public UISlicedImage abilitiesBackground;
		public UIText abilitiesText;

		public UIRaceInfoPanel(Player player, float textScale = 1f, bool large = false)
		{
			_player = player;
		}

		public void UpdateStats()
		{
			var mrPlagueRacesPlayer = _player.GetModPlayer<MrPlagueRacesPlayer>();
			ComparisonPlayerSetup();
			PercentagePlayerSetup();
			Asset<Texture2D>[] statImage = 
			{
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/Health"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/HealthRegen"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/Mana"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/ManaRegen"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/ManaCost"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/Defense"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/Endurance"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/Thorns"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/LavaMax"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MeleeDamage"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MeleeSpeed"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/RangedDamage"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MagicDamage"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/SummonDamage"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MaxMinions"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MinionKnockback"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MaxTurrets"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/AllDamage"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MeleeCrit"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/RangedCrit"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MagicCrit"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/AllCrit"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/ArmorPenetration"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/PickSpeed"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/TileSpeed"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/WallSpeed"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/BlockRange"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/MoveSpeed"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/JumpSpeed"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/FallResistance"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/FishingSkill"),
				ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/Stats/Aggro")
			};
			string[] statText = 
			{
				StatCalculationSystem.GetIntDifference(100, _player.statLifeMax2, statLifeMax2Comparison, statLifeMax2Percentage), 
				StatCalculationSystem.GetIntDifference(0, _player.lifeRegen, lifeRegenComparison, lifeRegenPercentage), 
				StatCalculationSystem.GetIntDifference(20, _player.statManaMax2, statManaMax2Comparison, statManaMax2Percentage), 
				StatCalculationSystem.GetIntDifference(0, _player.manaRegenBonus, manaRegenBonusComparison, manaRegenBonusPercentage),
				StatCalculationSystem.GetFloatDifference(1f, _player.manaCost),
				StatCalculationSystem.GetIntDifference(0, _player.statDefense, statDefenseComparison, statDefensePercentage),
				StatCalculationSystem.GetFloatDifference(0f, _player.endurance),
				StatCalculationSystem.GetFloatDifference(0f, _player.thorns),
				StatCalculationSystem.GetIntDifference(0, _player.lavaMax, lavaMaxComparison, lavaMaxPercentage),
				StatCalculationSystem.GetFloatDifference(0f, ((_player.GetDamage(DamageClass.Melee).Additive - 1) * 100), true, false, true),
				StatCalculationSystem.GetFloatDifference(1f, _player.GetAttackSpeed(DamageClass.Melee)),
				StatCalculationSystem.GetFloatDifference(0f, ((_player.GetDamage(DamageClass.Ranged).Additive - 1) * 100), true, false, true),
				StatCalculationSystem.GetFloatDifference(0f, ((_player.GetDamage(DamageClass.Magic).Additive - 1) * 100), true, false, true),
				StatCalculationSystem.GetFloatDifference(0f, ((_player.GetDamage(DamageClass.Summon).Additive - 1) * 100), true, false, true),
				StatCalculationSystem.GetIntDifference(1, _player.maxMinions, maxMinionsComparison, maxMinionsPercentage),
				StatCalculationSystem.GetFloatDifference(0f, _player.GetKnockback(DamageClass.Summon).Base, true, false, true),
				StatCalculationSystem.GetIntDifference(1, _player.maxTurrets, maxTurretsComparison, maxTurretsPercentage),
				StatCalculationSystem.GetFloatDifference(0f, ((_player.GetDamage(DamageClass.Generic).Additive - 1) * 100), true, false, true),
				StatCalculationSystem.GetFloatDifference(0f, _player.GetCritChance(DamageClass.Melee), true, false, true),
				StatCalculationSystem.GetFloatDifference(0f, _player.GetCritChance(DamageClass.Ranged), true, false, true),
				StatCalculationSystem.GetFloatDifference(0f, _player.GetCritChance(DamageClass.Magic), true, false, true),
				StatCalculationSystem.GetFloatDifference(4f, _player.GetCritChance(DamageClass.Generic), true, false, true),
				StatCalculationSystem.GetFloatDifference(0f, _player.GetArmorPenetration(DamageClass.Melee)),
				StatCalculationSystem.GetFloatDifference(1f, _player.pickSpeed, true, true),
				StatCalculationSystem.GetFloatDifference(1f, _player.tileSpeed),
				StatCalculationSystem.GetFloatDifference(1f, _player.wallSpeed),
				StatCalculationSystem.GetIntDifference(0, _player.blockRange, blockRangeComparison, blockRangePercentage),
				StatCalculationSystem.GetFloatDifference(1f, _player.moveSpeed),
				StatCalculationSystem.GetFloatDifference(0f, _player.jumpSpeedBoost),
				StatCalculationSystem.GetIntDifference(0, _player.extraFall, extraFallComparison, extraFallPercentage),
				StatCalculationSystem.GetIntDifference(0, _player.fishingSkill, fishingSkillComparison, fishingSkillPercentage),
				StatCalculationSystem.GetIntDifference(0, _player.aggro, aggroComparison, aggroPercentage)
			};
			string[] statHoverText = 
			{
				"Health",
				"Health Regeneration Rate",
				"Mana",
				"Mana Regeneration Rate",
				"Mana Cost",
				"Defense",
				"Endurance",
				"Thorns",
				"Lava Immunity Time",
				"Melee Damage",
				"Melee Speed",
				"Ranged Damage",
				"Magic Damage",
				"Summon Damage",
				"Minions",
				"Minion Knockback",
				"Turrets",
				"Damage",
				"Melee Critical Strike Chance",
				"Ranged Critical Strike Chance",
				"Magic Critical Strike Chance",
				"Critical Strike Chance",
				"Armor Penetration",
				"Mining Delay",
				"Tile Placement Speed",
				"Wall Placement Speed",
				"Placement Range",
				"Movement Speed",
				"Jump Speed",
				"Fall Damage Resistance",
				"Fishing Skill",
				"Aggro"
			};
			Clear();

			if (mrPlagueRacesPlayer.race.Description != null)
			{
				descriptionBackground = new UISlicedImage(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/CategoryPanelHighlight", (AssetRequestMode)1))
				{
					Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
					Height = StyleDimension.FromPixelsAndPercent(0f, 0f)
				};
				descriptionBackground.SetSliceDepths(10);
				descriptionBackground.Color = Color.LightGray * 0.7f;
				Add(descriptionBackground);
				descriptionText = new UIText(mrPlagueRacesPlayer.race.Description)
				{
					HAlign = 0f,
					VAlign = 0.5f,
					Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
					Top = StyleDimension.FromPixelsAndPercent(15f, 0f),
					IsWrapped = true
				};
				descriptionText.PaddingLeft = 4f;
				descriptionText.PaddingRight = 4f;
				descriptionBackground.Append(descriptionText);
				descriptionText.SetText(mrPlagueRacesPlayer.race.Description);
				descriptionBackground.Height = new StyleDimension(descriptionText.MinHeight.Pixels, 0f);
			}

			if (mrPlagueRacesPlayer.race.AbilitiesDescription != null)
			{
				abilitiesBackground = new UISlicedImage(ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/UI/CategoryPanelHighlight", (AssetRequestMode)1))
				{
					Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
					Height = StyleDimension.FromPixelsAndPercent(0f, 0f)
				};
				abilitiesBackground.SetSliceDepths(10);
				abilitiesBackground.Color = Color.LightGray * 0.7f;
				Add(abilitiesBackground);
				abilitiesText = new UIText(mrPlagueRacesPlayer.race.AbilitiesDescription)
				{
					HAlign = 0f,
					VAlign = 0.5f,
					Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
					Top = StyleDimension.FromPixelsAndPercent(15f, 0f),
					TextOriginX = 0f,
					IsWrapped = true
				};
				abilitiesText.PaddingLeft = 4f;
				abilitiesText.PaddingRight = 4f;
				abilitiesBackground.Append(abilitiesText);
				abilitiesText.SetText(mrPlagueRacesPlayer.race.AbilitiesDescription);
				abilitiesBackground.Height = new StyleDimension(abilitiesText.MinHeight.Pixels, 0f);
			}

			
			UIElement statContainer = new UIElement
			{
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f)
			};
			Add(statContainer);
			statContainer.SetPadding(0f);
			int buttonCount = 0;
			for (int i = 0; i < 32; i++)
			{
				if (statText[i] != "0")
				{
					UIRaceStatPanel statPanel = new UIRaceStatPanel(statImage[i], statText[i], statHoverText[i])
					{
						Width = StyleDimension.FromPixels(130f),
						Height = StyleDimension.FromPixels(30f),
						Left = StyleDimension.FromPixels((float)(buttonCount % 2) * 134f),
						Top = StyleDimension.FromPixels((float)(buttonCount / 2) * 35f)
					};
					statContainer.Append(statPanel);
					buttonCount += 1;
				}
			}
			totalHeight = ((mrPlagueRacesPlayer.race.Description != null ? (int)descriptionBackground.Height.Pixels : 0) + (mrPlagueRacesPlayer.race.AbilitiesDescription != null ? (int)abilitiesBackground.Height.Pixels : 0) + (buttonCount / 2 + ((buttonCount % 2 != 0) ? 1 : 0)) * 35);
		}
	}
}
