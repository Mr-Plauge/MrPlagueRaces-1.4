using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Buffs
{
	public class Unseen: ModBuff
	{
		Player ownerPlayer;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Unseen");
			// Description.SetDefault("Out of sight, out of mind\nIgnored by most enemies\nDealt damage is greatly increased");
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			ownerPlayer = player;
			player.GetModPlayer<UnseenPlayer>().unseen = true;
		}

		public class UnseenPlayer : ModPlayer
		{
			public bool unseen;

			public override void ResetEffects() {
				if (unseen) {
					for (int i = 0; i < 670; i++) {
						Player.npcTypeNoAggro[i] = true;
					}
					Player.aggro -= 5000;
					Player.GetDamage(DamageClass.Generic) += 10f;
				}
				unseen = false;
			}
		}
		
		public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams) {
			Texture2D textureEyes = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/Unseen_Eyes");
			Texture2D textureDetail = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/Unseen_Detail");
			Texture2D textureSkin = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/Unseen_Skin");
			Rectangle sourceRectangle = new Rectangle(0, 0, textureSkin.Width, textureSkin.Height);

			if (ownerPlayer != null) {
				var mrPlagueRacesPlayer = ownerPlayer.GetModPlayer<MrPlagueRacesPlayer>();
				spriteBatch.Draw(textureSkin, drawParams.Position, sourceRectangle, ownerPlayer.skinColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(textureDetail, drawParams.Position, sourceRectangle, mrPlagueRacesPlayer.detailColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(textureEyes, drawParams.Position, sourceRectangle, ownerPlayer.eyeColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}
	}
}
