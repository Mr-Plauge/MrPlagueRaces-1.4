using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Config;

namespace MrPlagueRaces.Common.Races.Human
{
    public class Human : Race
    {
        public override void Load()
        {
            Description = "Surprisingly durable and resilient, Humans can adapt to any situation.";
            DisplayName = "[c/FF9D00:Human]";
            StarterShirt = true;
            StarterPants = true;
            HairColor = new Color(215, 90, 55);
            SkinColor = new Color(255, 125, 90);
            DetailColor = new Color(255, 125, 90);
            EyeColor = new Color(105, 90, 75);
            ShirtColor = new Color(175, 165, 140);
            UnderShirtColor = new Color(160, 180, 215);
            PantsColor = new Color(255, 230, 175);
            ShoeColor = new Color(160, 105, 60);
        }

        public override void ResetEffects(Player player)
        {
            RaceStatDictionary = ModContent.GetInstance<HumanConfig>().humanStats;
        }
    }

    public class HumanConfig : ModConfig
    {
        public static HumanConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("Human")]
        [BackgroundColor(110, 141, 255)]
        public Dictionary<RacialStatType, RacialStatPercentageModifier> humanStats = new Dictionary<RacialStatType, RacialStatPercentageModifier>()
        {
            //n describes negative, p describes positive. IE, n20 is equivalent to -20%
            [RacialStatType.none] = RacialStatPercentageModifier.zero
        };
    }
}
