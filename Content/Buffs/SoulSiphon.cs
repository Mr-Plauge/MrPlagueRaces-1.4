using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.Races.Soulbeast;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Buffs
{
	public class SoulSiphon: ModBuff
	{
		public override void SetStaticDefaults() {
			//DisplayName.SetDefault("Soul Siphon");
			//Description.SetDefault("Devouring the souls of your enemies\nLife regeneration is greatly increased");
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<SoulbeastPlayer>().SoulSiphon = true;
		}
	}
}
