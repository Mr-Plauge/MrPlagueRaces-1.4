using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace MrPlagueRaces.Content.Items
{
	[AutoloadEquip(EquipType.Body)]
	public class InvisibleShirt : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Invisible Shirt");
			// Tooltip.SetDefault("Hides your torsowear");
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
			player.body = -1;
		}

		public override void AddRecipes() {
			CreateRecipe(1).AddIngredient(ItemID.Cobweb, 20).AddTile(TileID.WorkBenches).Register();
		}
	}
}
