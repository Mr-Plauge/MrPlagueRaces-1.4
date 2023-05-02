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
using MrPlagueRaces.Common.Races;
using System.Linq;

namespace MrPlagueRaces.Common.Players
{
	public class ComparisonPlayer : ModPlayer
	{
		public bool isComparison = false;
		public bool isPercentage = false;
		public override void ResetEffects()
		{
			if (isComparison)
			{
				Player.statLifeMax2 += 100; 
				Player.lifeRegen += 100; 
				Player.statManaMax2 += 100; 
				Player.manaRegenBonus += 100;
				Player.manaCost += 100;
				Player.statDefense += 100;
				Player.endurance += 100;
				Player.thorns += 100;
				Player.lavaMax += 100;
				Player.GetDamage(DamageClass.Melee) += 100;
				Player.GetAttackSpeed(DamageClass.Melee) += 100;
				Player.GetDamage(DamageClass.Ranged) += 100;
				Player.GetDamage(DamageClass.Magic) += 100;
				Player.GetDamage(DamageClass.Summon) += 100;
				Player.GetKnockback(DamageClass.Summon).Base += 100;
				Player.maxMinions += 100;
				Player.maxTurrets += 100;
				Player.GetDamage(DamageClass.Generic) += 100;
				Player.GetCritChance(DamageClass.Melee) += 100;
				Player.GetCritChance(DamageClass.Ranged) += 100;
				Player.GetCritChance(DamageClass.Magic) += 100;
				Player.GetCritChance(DamageClass.Generic) += 100;
				Player.GetArmorPenetration(DamageClass.Melee) += 100;
				Player.pickSpeed += 100;
				Player.tileSpeed += 100;
				Player.wallSpeed += 100;
				Player.blockRange += 100;
				Player.moveSpeed += 100;
				Player.jumpSpeedBoost += 100;
				Player.extraFall += 100;
				Player.fishingSkill += 100;
				Player.aggro += 100;
			}

			if (isPercentage)
			{
				Player.statLifeMax2 = 100; 
				Player.lifeRegen = 100; 
				Player.statManaMax2 = 100; 
				Player.manaRegenBonus = 100;
				Player.manaCost = 100;
				Player.statDefense = 100;
				Player.endurance = 100;
				Player.thorns = 100;
				Player.lavaMax = 100;
				Player.GetAttackSpeed(DamageClass.Melee) = 100;
				Player.GetKnockback(DamageClass.Summon).Base = 100;
				Player.maxMinions = 100;
				Player.maxTurrets = 100;
				Player.GetCritChance(DamageClass.Melee) = 100;
				Player.GetCritChance(DamageClass.Ranged) = 100;
				Player.GetCritChance(DamageClass.Magic) = 100;
				Player.GetCritChance(DamageClass.Generic) = 100;
				Player.GetArmorPenetration(DamageClass.Melee) = 100;
				Player.pickSpeed = 100;
				Player.tileSpeed = 100;
				Player.wallSpeed = 100;
				Player.blockRange = 100;
				Player.moveSpeed = 100;
				Player.jumpSpeedBoost = 100;
				Player.extraFall = 100;
				Player.fishingSkill = 100;
				Player.aggro = 100;
			}
		}
	}
}