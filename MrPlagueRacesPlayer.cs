using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using MrPlagueRaces.Common.UI;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.Races.Human;
using MrPlagueRaces.Common.UI.States;
using MrPlagueRaces.Content.Items;

namespace MrPlagueRaces
{
	public class MrPlagueRacesPlayer : ModPlayer
	{
		public Race race;
		public Color detailColor = new Color(255, 255, 255);
		public Color colorDetail = new Color(255, 255, 255);
		public Color colorEyes = new Color(255, 255, 255);
		public Color colorSkin = new Color(255, 255, 255);
		public Color colorHair = new Color(255, 255, 255);
		public bool statsEnabled = true;
		public bool gotStatToggler = false;

		public override void SaveData(TagCompound tag)
        {
			if (race != null)
			{
				tag["Race"] = race.FullName;
				tag["detailColorR"] = detailColor.R;
				tag["detailColorG"] = detailColor.G;
				tag["detailColorB"] = detailColor.B;
				tag["statsEnabled"] = statsEnabled;
				tag["gotStatToggler"] = gotStatToggler;
			}
        }

		public override void LoadData(TagCompound tag)
        {
			if ((tag.ContainsKey("Race") && RaceLoader.TryGetRace(tag.GetString("Race"), out var loadedRace)))
			{
				race = loadedRace;
			}
			else
			{
				race = ModContent.GetInstance<Human>();
			}
			if (tag.ContainsKey("detailColorR"))
			{
				detailColor.R = (byte)tag["detailColorR"];
			}
			if (tag.ContainsKey("detailColorG"))
			{
				detailColor.G = (byte)tag["detailColorG"];
			}
			if (tag.ContainsKey("detailColorB"))
			{
				detailColor.B = (byte)tag["detailColorB"];
			}
			if (tag.ContainsKey("statsEnabled"))
			{
				statsEnabled = tag.GetBool("statsEnabled");
			}
			if (tag.ContainsKey("gotStatToggler"))
			{
				gotStatToggler = tag.GetBool("gotStatToggler");
			}

			if ((Player.hair + 1) > GetRaceHairCount(Player) && GetRaceHairCount(Player) != 0)
			{
				Player.hair = (GetRaceHairCount(Player) - 1);
			}
        }

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) 
		{
			if (race != null)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)MrPlagueRacesMessageType.MrPlagueRacesPlayerSyncPlayer);
				packet.Write((byte)Player.whoAmI);
				packet.Write(race.Id);
				packet.Write(detailColor.R);
				packet.Write(detailColor.G);
				packet.Write(detailColor.B);
				packet.Write(colorDetail.R);
				packet.Write(colorDetail.G);
				packet.Write(colorDetail.B);
				packet.Write(colorDetail.A);
				packet.Write(colorEyes.R);
				packet.Write(colorEyes.G);
				packet.Write(colorEyes.B);
				packet.Write(colorSkin.R);
				packet.Write(colorSkin.G);
				packet.Write(colorSkin.B);
				packet.Write(colorHair.R);
				packet.Write(colorHair.G);
				packet.Write(colorHair.B);
				packet.Write((bool)statsEnabled);
				packet.Write((bool)gotStatToggler);

				packet.Send(toWho, fromWho);
			}
		}

		public override void PostItemCheck()
		{
			if (!gotStatToggler)
            {
                gotStatToggler = true;
                Player.QuickSpawnItem(Wiring.GetProjectileSource(0, 0), ModContent.ItemType<Stat_Toggler>());
            }
		}

		public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
		{
			colorDetail = Player.GetImmuneAlpha(Lighting.GetColorClamped((int)((double)Player.position.X + (double)Player.width * 0.5) / 16, (int)(((double)Player.position.Y + (double)Player.height * 0.5) / 16.0), detailColor), drawInfo.shadow);
			colorEyes = Player.GetImmuneAlpha(Lighting.GetColorClamped((int)((double)Player.position.X + (double)Player.width * 0.5) / 16, (int)(((double)Player.position.Y + (double)Player.height * 0.5) / 16.0), Player.eyeColor), drawInfo.shadow);
			colorSkin = Player.GetImmuneAlpha(Lighting.GetColorClamped((int)((double)Player.position.X + (double)Player.width * 0.5) / 16, (int)(((double)Player.position.Y + (double)Player.height * 0.5) / 16.0), Player.skinColor), drawInfo.shadow);
			colorHair = Player.GetImmuneAlpha(Lighting.GetColorClamped((int)((double)Player.position.X + (double)Player.width * 0.5) / 16, (int)(((double)Player.position.Y + (double)Player.height * 0.5) / 16.0), Player.hairColor), drawInfo.shadow);
			if (Player.dead) {
				TextureAssets.Ghost = GetRaceTexture(Player, "Ghost");
			}
			else {
				TextureAssets.Ghost = ModContent.Request<Texture2D>("MrPlagueRaces/Assets/Textures/Blank");
			}
			if (race == null) {
				for (int i = 0; i < RaceLoader.Races.Count; i++) {
					if (RaceLoader.Races[i].Name == "Human") {
						race = RaceLoader.Races[i];
					}
				}
			}
		}

		public override void PreUpdate()
		{
			if (Player.dead && Player.ghost && Player.difficulty != 2) {
				Player.ghost = false;
			}
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
		{
			if (race != null)
			{
				playSound = false;
			}
			return true;
		}

		public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
		{
			if (race != null)
			{
				PlayRaceSound(Player, "Hurt");
			}
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (race != null)
			{
				playSound = false;
			}
			return true;
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			if (race != null)
			{
				PlayRaceSound(Player, "Killed");
			}
		}

		public Asset<Texture2D> GetRaceTexture(Player player, string texturePath, string defaultTexturePath = "MrPlagueRaces/Assets/Textures/Blank")
		{
			Asset<Texture2D> Race_Texture = null;
			if (race != null)
			{
				if (player.Male)
				{
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/{texturePath}"))
					{
						Race_Texture = ModContent.Request<Texture2D>($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/{texturePath}");
					}
					else
					{
						Race_Texture = ModContent.Request<Texture2D>($"{defaultTexturePath}");
					}
				}
				else
				{
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Female/{texturePath}"))
					{
						Race_Texture = ModContent.Request<Texture2D>($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Female/{texturePath}");
					}
					else
					{
						if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/{texturePath}"))
						{
							Race_Texture = ModContent.Request<Texture2D>($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/{texturePath}");
						}
						else
						{
							Race_Texture = ModContent.Request<Texture2D>($"{defaultTexturePath}");
						}
					}
				}
			}
			else {
				Race_Texture = ModContent.Request<Texture2D>($"{defaultTexturePath}");
			}
			return Race_Texture;
		}

		public void PlayRaceSound(Player player, string soundPath)
		{
			if (race != null)
			{
				if (Main.netMode == NetmodeID.Server) {
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)MrPlagueRacesMessageType.ExecuteRaceSound);
					packet.Write((byte)player.whoAmI);
					packet.Write(soundPath);
					packet.Send();
				}
				else {
					ExecuteRaceSound(player, soundPath);
				}
			}
		}

		public void ExecuteRaceSound(Player player, string soundPath)
		{
			if (race != null)
			{
				if (player.Male)
				{
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Male/{soundPath}"))
					{
						SoundEngine.PlaySound(new SoundStyle($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Male/{soundPath}"), player.Center);
					}
					else
					{
						SoundEngine.PlaySound(new SoundStyle($"{Mod.Name}/Assets/Sounds/Players/Races/Human/Male/{soundPath}"), player.Center);
					}
				}
				else
				{
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Female/{soundPath}"))
					{
						SoundEngine.PlaySound(new SoundStyle($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Female/{soundPath}"), player.Center);
					}
					else if (ModContent.HasAsset($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Male/{soundPath}"))
					{
						SoundEngine.PlaySound(new SoundStyle($"{race.Mod.Name}/{race.SoundLocation}/{race.Name}/Male/{soundPath}"), player.Center);
					}
					else if (ModContent.HasAsset($"{race.Mod.Name}/Assets/Sounds/Players/Races/Human/Female/{soundPath}"))
					{
						SoundEngine.PlaySound(new SoundStyle($"{Mod.Name}/Assets/Sounds/Players/Races/Human/Female/{soundPath}"), player.Center);
					}
					else
					{
						SoundEngine.PlaySound(new SoundStyle($"{Mod.Name}/Assets/Sounds/Players/Races/Human/Male/{soundPath}"), player.Center);
					}
				}
			}
		}

		public int GetRaceHairCount(Player player)
		{
			int TotalCount = 0;
			int HairCount = 0;
			int EyesCount = 0;
			int ColorlessCount = 0;
			int DetailCount = 0;
			int SkinCount = 0;
			int HairCountGlowmask = 0;
			int EyesCountGlowmask = 0;
			int ColorlessCountGlowmask = 0;
			int DetailCountGlowmask = 0;
			int SkinCountGlowmask = 0;
			if (race != null)
			{
				for (int i = 0; i < 165; i++)
				{
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/ColorHair/Hairstyles/Hair_{i + 1}"))
					{
						HairCount += 1;
					}
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/ColorEyes/Hairstyles/Hair_{i + 1}"))
					{
						EyesCount += 1;
					}
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/Colorless/Hairstyles/Hair_{i + 1}"))
					{
						ColorlessCount += 1;
					}
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/ColorDetail/Hairstyles/Hair_{i + 1}"))
					{
						DetailCount += 1;
					}
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/ColorSkin/Hairstyles/Hair_{i + 1}"))
					{
						SkinCount += 1;
					}
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/ColorHair/Glowmask/Hairstyles/Hair_{i + 1}"))
					{
						HairCountGlowmask += 1;
					}
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/ColorEyes/Glowmask/Hairstyles/Hair_{i + 1}"))
					{
						EyesCountGlowmask += 1;
					}
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/Colorless/Glowmask/Hairstyles/Hair_{i + 1}"))
					{
						ColorlessCountGlowmask += 1;
					}
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/ColorDetail/Glowmask/Hairstyles/Hair_{i + 1}"))
					{
						DetailCountGlowmask += 1;
					}
					if (ModContent.HasAsset($"{race.Mod.Name}/{race.TextureLocation}/{race.Name}/Male/ColorSkin/Glowmask/Hairstyles/Hair_{i + 1}"))
					{
						SkinCountGlowmask += 1;
					}
				}
				TotalCount = HairCount;
				if (EyesCount > TotalCount) {
					TotalCount = EyesCount;
				}
				if (ColorlessCount > TotalCount) {
					TotalCount = ColorlessCount;
				}
				if (DetailCount > TotalCount) {
					TotalCount = DetailCount;
				}
				if (SkinCount > TotalCount) {
					TotalCount = SkinCount;
				}
				if (HairCountGlowmask > TotalCount) {
					TotalCount = HairCountGlowmask;
				}
				if (EyesCountGlowmask > TotalCount) {
					TotalCount = EyesCountGlowmask;
				}
				if (ColorlessCountGlowmask > TotalCount) {
					TotalCount = ColorlessCountGlowmask;
				}
				if (DetailCountGlowmask > TotalCount) {
					TotalCount = DetailCountGlowmask;
				}
				if (SkinCountGlowmask > TotalCount) {
					TotalCount = SkinCountGlowmask;
				}
			}
			return TotalCount;
		}
	}
}