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
	public class Photosensitive: ModBuff
	{
		Player ownerPlayer;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Photosensitive");
			Description.SetDefault("You are set on fire when attacked");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			ownerPlayer = player;
			player.GetModPlayer<PhotosensitivePlayer>().photosensitive = true;
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<PhotosensitiveNPC>().photosensitive = true;
		}
		
		public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams) {
			Texture2D textureEyes = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/Photosensitive_Eyes");
			Texture2D textureDetail = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/Photosensitive_Detail");
			Texture2D textureSkin = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/Photosensitive_Skin");
			Rectangle sourceRectangle = new Rectangle(0, 0, textureSkin.Width, textureSkin.Height);

			if (ownerPlayer != null) {
				var mrPlagueRacesPlayer = ownerPlayer.GetModPlayer<MrPlagueRacesPlayer>();
				spriteBatch.Draw(textureSkin, drawParams.Position, sourceRectangle, ownerPlayer.skinColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(textureDetail, drawParams.Position, sourceRectangle, mrPlagueRacesPlayer.detailColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(textureEyes, drawParams.Position, sourceRectangle, ownerPlayer.eyeColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}

		public class PhotosensitivePlayer : ModPlayer
		{
			public bool photosensitive;
			public int counter;

			public override void ResetEffects() {
				photosensitive = false;
			}

			public override void UpdateBadLifeRegen() {
				if (photosensitive && counter > 0) {
					if (Player.lifeRegen > 0)
						Player.lifeRegen = 0;
					Player.lifeRegenTime = 0;
					Player.lifeRegen -= Player.statLifeMax2 / 10;
				}
			}

			public override void PreUpdate() {
				if (counter > 0) {
					counter--;
				}
			}

			public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
			{
				if (photosensitive) {
					counter = 120;
					SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, Player.Center);
				}
			}

			public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright) {
				if (photosensitive && Main.rand.Next(5) < 4 && counter > 0) {
					Dust dust19 = Dust.NewDustDirect(new Vector2(Player.position.X - 2f, Player.position.Y - 2f), Player.width + 4, Player.height + 4, 174, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 1, default(Color), 1f);
					dust19.noGravity = true;
					dust19.velocity *= 0.75f;
					dust19.velocity.X *= 0.75f;
					dust19.velocity.Y -= 1f;
					if (Main.rand.Next(4) == 0)
					{
						dust19.noGravity = false;
						dust19.scale *= 0.5f;
					}
				}
			}
		}

		public class PhotosensitiveNPC : GlobalNPC
		{
			public override bool InstancePerEntity => true;

			public bool photosensitive;
			public int counter;

			public override void ResetEffects(NPC npc) {
				photosensitive = false;
			}

			public override void UpdateLifeRegen(NPC npc, ref int damage) {
				if (photosensitive && counter > 0) {
					if (npc.lifeRegen > 0) {
						npc.lifeRegen = 0;
					}
					npc.lifeRegen -= npc.lifeMax / 10;
				}
			}

			public override void PostAI(NPC npc) {
				if (counter > 0) {
					counter--;
				}
			}

			public override void HitEffect(NPC npc, int hitDirection, double damage)
			{
				if (photosensitive) {
					counter = 120;
					SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, npc.Center);
				}
			}

			public override void DrawEffects(NPC npc, ref Color drawColor) {
				if (photosensitive && Main.rand.Next(5) < 4 && counter > 0) {
					Dust dust19 = Dust.NewDustDirect(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, 174, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 1, default(Color), 1f);
					dust19.noGravity = true;
					dust19.velocity *= 0.75f;
					dust19.velocity.X *= 0.75f;
					dust19.velocity.Y -= 1f;
					if (Main.rand.Next(4) == 0)
					{
						dust19.noGravity = false;
						dust19.scale *= 0.5f;
					}
				}
			}
		}
	}
}
