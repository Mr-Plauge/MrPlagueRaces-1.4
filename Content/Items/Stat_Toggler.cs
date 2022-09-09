using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace MrPlagueRaces.Content.Items
{

	public class Stat_Toggler : ModItem
	{
		public override void SetStaticDefaults() 
		{
		    DisplayName.SetDefault("Racial Stat Nullifier");
			Tooltip.SetDefault("Toggles racial stats and abilities on and off for the current character when used");
		}

		public override void SetDefaults() 
		{
            Item.width = 52;
            Item.height = 52;
            Item.useStyle = ItemUseStyleID.MowTheLawn;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.rare = ItemRarityID.Quest;
		}

		public override bool? UseItem(Player player)
		{
		    MrPlagueRacesPlayer mrPlagueRacesPlayer = player.GetModPlayer<MrPlagueRacesPlayer>();
			mrPlagueRacesPlayer.PlayRaceSound(player, "Hurt");
			mrPlagueRacesPlayer.statsEnabled = !mrPlagueRacesPlayer.statsEnabled;
			if (mrPlagueRacesPlayer.statsEnabled) {
				
				Main.NewText(player.name + "'s racial stats and abilities have been enabled!", 77, 191, 96);
			}
			else {
				Main.NewText(player.name + "'s racial stats and abilities have been disabled!", 221, 65, 65);
			}
			return true;
		}
	}
}
