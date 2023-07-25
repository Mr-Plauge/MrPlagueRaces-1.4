using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace MrPlagueRaces 
{
    public class MrPlagueRacesConfig : ModConfig
    {
        public static MrPlagueRacesConfig Instance;
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Toggle Race Stats")]
        [Tooltip("Toggle race stats")]
        [DefaultValue(true)]
        public bool raceStats
        {
            get;
            set;
        }

        [Label("Toggle Experimental Content")]
        [Tooltip("Toggle experimental content")]
        [DefaultValue(true)]
        public bool experimentalContent
        {
            get;
            set;
        }
    }
}
