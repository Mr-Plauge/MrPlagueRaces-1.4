using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Config;
using MrPlagueRaces.Common.Races;
using System.Linq;

namespace MrPlagueRaces.Common.Players
{
	public class StatConfigPlayer : ModPlayer
	{
		public override void ResetEffects()
        {
            var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();

            if (mrPlagueRacesPlayer.race != null && mrPlagueRacesPlayer.race.RaceStatDictionary != null && ModContent.GetInstance<MrPlagueRacesConfig>().raceStats)
            {
                Dictionary<RacialStatType, RacialStatPercentageModifier> configStatDictionary = mrPlagueRacesPlayer.race.RaceStatDictionary;
                if (configStatDictionary.ContainsKey(RacialStatType.statLifeMax2))
                {
                    Player.statLifeMax2 += (int)(Player.statLifeMax2 / StatConfigHelpers.RacialStatPercentageFractionIndex[(int)configStatDictionary[RacialStatType.statLifeMax2]]);
                }
                if (configStatDictionary.ContainsKey(RacialStatType.lifeRegen))
                {
                    Player.lifeRegen += (int)StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.lifeRegen]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.statManaMax2))
                {
                    Player.statManaMax2 += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.statManaMax2]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.manaRegenBonus))
                {
                    Player.manaRegenBonus += (int)StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.manaRegenBonus]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.manaCost))
                {
                    Player.manaCost += StatConfigHelpers.RacialStatPercentageFloatIndex[200 - (int)configStatDictionary[RacialStatType.manaCost]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.statDefense))
                {
                    Player.statDefense += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.statDefense]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.endurance))
                {
                    Player.endurance += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.endurance]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.thorns))
                {
                    Player.thorns += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.thorns]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.lavaMax))
                {
                    Player.lavaMax += (int)StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.lavaMax]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.meleeDamage))
                {
                    Player.GetDamage(DamageClass.Melee) += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.meleeDamage]] + ((int)configStatDictionary[RacialStatType.meleeDamage] > 100 && (int)configStatDictionary[RacialStatType.meleeDamage] < 118 ? 0.005f : 0f);
                }
                if (configStatDictionary.ContainsKey(RacialStatType.meleeSpeed))
                {
                    Player.GetAttackSpeed(DamageClass.Melee) += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.meleeSpeed]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.rangedDamage))
                {
                    Player.GetDamage(DamageClass.Ranged) += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.rangedDamage]] + ((int)configStatDictionary[RacialStatType.rangedDamage] > 100 && (int)configStatDictionary[RacialStatType.rangedDamage] < 118 ? 0.005f : 0f);
                }
                if (configStatDictionary.ContainsKey(RacialStatType.magicDamage))
                {
                    Player.GetDamage(DamageClass.Magic) += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.magicDamage]] + ((int)configStatDictionary[RacialStatType.magicDamage] > 100 && (int)configStatDictionary[RacialStatType.magicDamage] < 118 ? 0.005f : 0f);
                }
                if (configStatDictionary.ContainsKey(RacialStatType.minionDamage))
                {
                    Player.GetDamage(DamageClass.Summon) += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.minionDamage]] + ((int)configStatDictionary[RacialStatType.minionDamage] > 100 && (int)configStatDictionary[RacialStatType.minionDamage] < 118 ? 0.005f : 0f);
                }
                if (configStatDictionary.ContainsKey(RacialStatType.allDamage))
                {
                    Player.GetDamage(DamageClass.Generic) += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.allDamage]] + ((int)configStatDictionary[RacialStatType.allDamage] > 100 && (int)configStatDictionary[RacialStatType.allDamage] < 118 ? 0.005f : 0f);
                }
                if (configStatDictionary.ContainsKey(RacialStatType.minionKB))
                {
                    Player.GetKnockback(DamageClass.Summon).Base += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.minionKB]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.maxMinions))
                {
                    Player.maxMinions += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.maxMinions]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.maxTurrets))
                {
                    Player.maxTurrets += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.maxTurrets]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.meleeCrit))
                {
                    Player.GetCritChance(DamageClass.Melee) += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.meleeCrit]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.rangedCrit))
                {
                    Player.GetCritChance(DamageClass.Ranged) += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.rangedCrit]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.magicCrit))
                {
                    Player.GetCritChance(DamageClass.Magic) += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.magicCrit]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.allCrit))
                {
                    Player.GetCritChance(DamageClass.Generic) += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.allCrit]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.armorPenetration))
                {
                    Player.GetArmorPenetration(DamageClass.Melee) += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.armorPenetration]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.pickSpeed))
                {
                    Player.pickSpeed += StatConfigHelpers.RacialStatPercentageFloatIndex[200 - (int)configStatDictionary[RacialStatType.pickSpeed]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.tileSpeed))
                {
                    Player.tileSpeed += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.tileSpeed]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.wallSpeed))
                {
                    Player.wallSpeed += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.wallSpeed]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.blockRange))
                {
                    Player.blockRange += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.blockRange]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.moveSpeed))
                {
                    Player.moveSpeed += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.moveSpeed]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.jumpSpeedBoost))
                {
                    Player.jumpSpeedBoost += StatConfigHelpers.RacialStatPercentageFloatIndex[(int)configStatDictionary[RacialStatType.jumpSpeedBoost]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.extraFall))
                {
                    Player.extraFall += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.extraFall]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.fishingSkill))
                {
                    Player.fishingSkill += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.fishingSkill]];
                }
                if (configStatDictionary.ContainsKey(RacialStatType.aggro))
                {
                    Player.aggro += StatConfigHelpers.RacialStatPercentageQuantityIndex[(int)configStatDictionary[RacialStatType.aggro]];
                }
            }
        }
	}
}