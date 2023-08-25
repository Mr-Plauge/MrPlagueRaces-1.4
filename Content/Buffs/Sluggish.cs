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
	public class Sluggish: ModBuff
	{
		Player ownerPlayer;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Sluggish");
			// Description.SetDefault("Staying out of the sunlight lowers your regeneration and defense");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			ownerPlayer = player;
			player.GetModPlayer<SluggishPlayer>().sluggish = true;
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<SluggishNPC>().sluggish = true;
		}
		
		public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams) {
			Texture2D textureSkin = (Texture2D)ModContent.Request<Texture2D>("MrPlagueRaces/Content/Buffs/Sluggish_Skin");
			Rectangle sourceRectangle = new Rectangle(0, 0, textureSkin.Width, textureSkin.Height);

			if (ownerPlayer != null) {
				var mrPlagueRacesPlayer = ownerPlayer.GetModPlayer<MrPlagueRacesPlayer>();
				spriteBatch.Draw(textureSkin, drawParams.Position, sourceRectangle, ownerPlayer.skinColor * Main.buffAlpha[buffIndex], 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}

		public class SluggishPlayer : ModPlayer
		{
			public bool sluggish;

			public override void ResetEffects() {
				sluggish = false;
			}

			public override void UpdateBadLifeRegen() {
				if (sluggish) {
					if (Player.lifeRegen > 0)
						Player.lifeRegen = 0;
				}
			}
		}

		public class SluggishNPC : GlobalNPC
		{
			public override bool InstancePerEntity => true;

			public bool sluggish;

			public override void ResetEffects(NPC npc) {
				sluggish = false;
			}

			public override void UpdateLifeRegen(NPC npc, ref int damage) {
				if (sluggish) {
					if (npc.lifeRegen > 0) {
						npc.lifeRegen = 0;
					}
				}
			}
		}
	}
}
