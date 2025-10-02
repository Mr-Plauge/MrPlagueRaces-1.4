using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Common.Systems;

namespace MrPlagueRaces.Content.Items
{
    public class EntangledStrands : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Entangled Strands");
            //Tooltip.SetDefault("To begin anew, one must leave an old self behind");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.maxStack = Item.CommonMaxStack;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.UseSound = SoundID.DD2_EtherianPortalSpawnEnemy;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Purple.ToVector3() * 2f * Main.essScale);
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                ModContent.GetInstance<RaceChangeUISystem>().ShowMyUI();
            }
            return true;
        }

        public override void UseItemFrame(Player player)
        {
            int itemTime = player.itemTime;
            int itemTimeMax = player.itemTimeMax;
            float num21 = itemTimeMax;
            num21 = (num21 - itemTime) / num21;
            float num19 = 44f;
            float num18 = 3.14156f * 3f;
            Vector2 vector4 = Utils.RotatedBy(new Vector2(15f, 0f), (double)(num18 * num21), default(Vector2));
            vector4.X *= player.direction;
            Vector2 vector5 = default(Vector2);
            for (int num17 = 0; num17 < 2; num17++)
            {
                int type3 = 295;
                if (num17 == 1)
                {
                    vector4.X *= -1f;
                    type3 = 296;
                }
                vector5 = new Vector2(vector4.X, num19 * (1f - num21) - num19 + (float)(player.height / 2));
                vector5 += player.Center;
                int num16 = Dust.NewDust(vector5, 0, 0, type3, 0f, 0f, 100);
                Main.dust[num16].position = vector5;
                Main.dust[num16].noGravity = true;
                Main.dust[num16].velocity = Vector2.Zero;
                Main.dust[num16].scale = 1.3f;
                Main.dust[num16].customData = this;
            }
        }
    }
}