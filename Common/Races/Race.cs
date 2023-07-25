using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MrPlagueRaces.Common.Races
{
	public abstract class Race : ModType
	{
		public int Id { get; internal set; }
		public Mod mod { get; internal set; }
        public string DisplayName = null;
		public string Description = null;
		public string AbilitiesDescription = null;

		public string TextureLocation = "Assets/Textures/Players/Races";
		public string SoundLocation = "Assets/Sounds/Players/Races";
		public int ClothStyle = 1;
		public int HairStyle = 0;
		public bool CensorClothing = true;
		public bool StarterShirt = false;
		public bool StarterPants = false;
		public bool AlwaysDrawHair = false;
		public Color HairColor = new Color(215, 90, 55);
		public Color SkinColor = new Color(255, 125, 90);
		public Color DetailColor = new Color(255, 125, 90);
		public Color EyeColor = new Color(105, 90, 75);
		public Color ShirtColor = new Color(175, 165, 140);
		public Color UnderShirtColor = new Color(160, 180, 215);
		public Color PantsColor = new Color(255, 230, 175);
		public Color ShoeColor = new Color(160, 105, 60);

		protected sealed override void Register()
		{
			RaceLoader.AddRace(this);
			ContentInstance.Register(this);
			SetStaticDefaults();
		}
	}
}