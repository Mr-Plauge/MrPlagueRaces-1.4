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
	public class SoulfireImbuement: ModBuff
	{
		Player ownerPlayer;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Soulfire Imbuement");
			Description.SetDefault("Mana regeneration is greatly increased");
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<SoulfireImbuementPlayer>().soulfireimbuement = true;
			ownerPlayer = player;
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<SoulfireImbuementNPC>().soulfireimbuement = true;
		}
		
		public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams) {
			Texture2D textureEyes = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/SoulfireImbuement_Eyes");
			Texture2D textureSkin = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/SoulfireImbuement_Skin");
			Rectangle sourceRectangle = new Rectangle(0, 0, textureEyes.Width, textureEyes.Height);

			if (ownerPlayer != null) {
				var mrPlagueRacesPlayer = ownerPlayer.GetModPlayer<MrPlagueRacesPlayer>();
				spriteBatch.Draw(textureSkin, drawParams.Position, sourceRectangle, ownerPlayer.skinColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(textureEyes, drawParams.Position, sourceRectangle, ownerPlayer.eyeColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}
	}
	
	public class SoulfireImbuementPlayer : ModPlayer
	{
		public bool soulfireimbuement;

		public override void ResetEffects() {
			soulfireimbuement = false;
		}

		public override void PreUpdate() {
			if (soulfireimbuement)
			{
				Player.statMana += 20;
			}
		}
	}

	public class SoulfireImbuementNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public bool soulfireimbuement;

		public override void ResetEffects(NPC npc) {
			soulfireimbuement = false;
		}
	}
}
