using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Buffs
{
	public class Troglodyte: ModBuff
	{
		Player ownerPlayer;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Troglodyte");
			Description.SetDefault("The sun lowers your endurance and makes you slower");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			ownerPlayer = player;
			player.endurance -= 0.75f;
			player.moveSpeed -= 0.75f;
		}
		
		public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams) {
			Texture2D textureSkin = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/Troglodyte_Skin");
			Texture2D textureHair = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/Troglodyte_Hair");
			Rectangle sourceRectangle = new Rectangle(0, 0, textureSkin.Width, textureSkin.Height);

			if (ownerPlayer != null) {
				var mrPlagueRacesPlayer = ownerPlayer.GetModPlayer<MrPlagueRacesPlayer>();
				spriteBatch.Draw(textureSkin, drawParams.Position, sourceRectangle, ownerPlayer.skinColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(textureHair, drawParams.Position, sourceRectangle, ownerPlayer.hairColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}
	}
}
