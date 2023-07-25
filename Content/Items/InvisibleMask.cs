using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace MrPlagueRaces.Content.Items
{

	[AutoloadEquip(EquipType.Head)]
	public class InvisibleMask : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Invisible Mask");
			// Tooltip.SetDefault("Hides your headwear");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
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
			player.head = -1;
		}

		public override void AddRecipes() {
			CreateRecipe(1).AddIngredient(ItemID.Cobweb, 15).AddTile(TileID.WorkBenches).Register();
		}
	}
}
