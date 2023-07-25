using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using MrPlagueRaces.Common.UI.States;

namespace MrPlagueRaces.Common.Systems
{
	public class StatCalculationSystem : ModSystem
	{
		internal static string GetIntDifference(int defaultValue, int playerValue, int playerValueComparison, int playerValuePercentage, bool percentage = false, bool negative = false)
		{
			int valueDifference = playerValue - defaultValue;
			int valueDifferenceComparison = playerValueComparison - defaultValue - 100;
			string valueDifferenceString = (valueDifference.ToString());
			if (valueDifference != valueDifferenceComparison || percentage && valueDifference != 0)
			{
				int valuePercentage = playerValuePercentage - 100;
				valueDifferenceString = valuePercentage + "%";
			}
			if (valueDifference > 0)
			{
				valueDifferenceString = ("+" + valueDifferenceString);
			}
			string coloredString = $"{valueDifferenceString}";
			if (coloredString != "0")
			{	
				coloredString = $"[c/4DBF60:{valueDifferenceString}]";
			}
			if (negative && valueDifference > 0 || !negative && valueDifference < 0)
			{
				coloredString = $"[c/FF3640:{valueDifferenceString}]";
			}
			return coloredString;
		}

		internal static string GetFloatDifference(float defaultValue, float playerValue, bool percentage = true, bool negative = false, bool divide = false)
		{
			float defaultValueMult = (divide ? defaultValue : defaultValue * 100f);
			float playerValueMult = (divide ? playerValue : playerValue * 100f);
			int defaultValueInt = ((int)defaultValueMult);
			int playerValueInt = ((int)playerValueMult);
			int valueDifference = playerValueInt - defaultValueInt;
			string valueDifferenceString = (valueDifference.ToString());
			if (percentage && valueDifference != 0)
			{
				valueDifferenceString = (valueDifferenceString + "%");
			}
			if (valueDifference > 0)
			{
				valueDifferenceString = ("+" + valueDifferenceString);
			}
			string coloredString = $"{valueDifferenceString}";
			if (coloredString != "0")
			{	
				coloredString = $"[c/4DBF60:{valueDifferenceString}]";
			}
			if (negative && valueDifference > 0 || !negative && valueDifference < 0)
			{
				coloredString = $"[c/FF3640:{valueDifferenceString}]";
			}
			return coloredString;
		}
	}
}