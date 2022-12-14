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
	public class Mycobuffer: ModBuff
	{
		Player ownerPlayer;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Mycobuffer");
			Description.SetDefault("Mycelium is regenerating your bodily tissue");
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			ownerPlayer = player;
			player.GetModPlayer<MycobufferPlayer>().mycobuffed = true;
		}
		
		public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams) {
			Texture2D textureHair = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/Mycobuffer_Hair");
			Texture2D textureEyes = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/Mycobuffer_Eyes");
			Texture2D textureSkin = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/Mycobuffer_Skin");
			Rectangle sourceRectangle = new Rectangle(0, 0, textureHair.Width, textureHair.Height);
			if (ownerPlayer != null) {
				spriteBatch.Draw(textureSkin, drawParams.Position, sourceRectangle, ownerPlayer.skinColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(textureEyes, drawParams.Position, sourceRectangle, ownerPlayer.eyeColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(textureHair, drawParams.Position, sourceRectangle, ownerPlayer.hairColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}

		public class MycobufferPlayer : ModPlayer
		{
			public bool mycobuffed;

			public override void ResetEffects() {
				mycobuffed = false;
			}
			public override void UpdateBadLifeRegen() {
				if (mycobuffed) {
					if (Player.lifeRegen < 0)
					{
						Player.lifeRegen = 0;
					}
					Player.lifeRegenTime = 0;
					Player.lifeRegen += Player.statLifeMax2 / 5;
				}
			}
		}
	}
}
