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
	public class TemporalRecoil: ModBuff
	{
		Player ownerPlayer;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Temporal Recoil");
			Description.SetDefault("Unable to rewind time");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			ownerPlayer = player;
		}
		
		public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams) {
			Texture2D textureHair = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/TemporalRecoil_Hair");
			Texture2D textureEyes = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/TemporalRecoil_Eyes");
			Texture2D textureDetail = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/TemporalRecoil_Detail");
			Texture2D textureSkin = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/TemporalRecoil_Skin");
			Rectangle sourceRectangle = new Rectangle(0, 0, textureHair.Width, textureHair.Height);

			if (ownerPlayer != null) {
				var mrPlagueRacesPlayer = ownerPlayer.GetModPlayer<MrPlagueRacesPlayer>();
				spriteBatch.Draw(textureSkin, drawParams.Position, sourceRectangle, ownerPlayer.skinColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(textureDetail, drawParams.Position, sourceRectangle, mrPlagueRacesPlayer.detailColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(textureEyes, drawParams.Position, sourceRectangle, ownerPlayer.eyeColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(textureHair, drawParams.Position, sourceRectangle, ownerPlayer.hairColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}
	}
}
