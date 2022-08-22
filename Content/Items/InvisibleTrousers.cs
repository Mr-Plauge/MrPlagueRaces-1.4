using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace MrPlagueRaces.Content.Items
{

	[AutoloadEquip(EquipType.Legs)]
	public class InvisibleTrousers : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Invisible Trousers");
			Tooltip.SetDefault("Hides your legwear");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 18;
			Item.vanity = true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * 0.5f;
		}

		public override void UpdateEquip(Player player) 
		{
			player.legs = -1;
		}

		public override void AddRecipes() {
			CreateRecipe(1).AddIngredient(ItemID.Cobweb, 10).AddTile(TileID.WorkBenches).Register();
		}
	}
}
