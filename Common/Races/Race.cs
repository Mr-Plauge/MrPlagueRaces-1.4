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

		public virtual void ResetEffects(Player player) { }
		public virtual void UpdateDead(Player player) { }
		public virtual void UpdateBadLifeRegen(Player player) { }
		public virtual void UpdateLifeRegen(Player player) { }
		public virtual void NaturalLifeRegen(Player player, ref float regen) { }
		public virtual void PreUpdate(Player player) { }
		public virtual void ProcessTriggers(Player player, TriggersSet triggersSet) { }
		public virtual bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter) => true;
		public virtual void Hurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter) { }
		public virtual void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter) { }
		public virtual bool PreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource) => true;
		public virtual void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) { }
		public virtual void OnHitAnything(Player player, float x, float y, Entity victim) { }
		public virtual bool? CanHitNPC(Player player, Item item, NPC target) => null;
		public virtual void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit) { }
		public virtual void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit) { }
		public virtual bool? CanHitNPCWithProj(Player player, Projectile proj, NPC target) => null;
		public virtual void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) { }
		public virtual void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit) { }
		public virtual bool CanHitPvp(Player player, Item item, Player target) => true;
		public virtual void ModifyHitPvp(Player player, Item item, Player target, ref int damage, ref bool crit) { }
		public virtual void OnHitPvp(Player player, Item item, Player target, int damage, bool crit) { }
		public virtual bool CanHitPvpWithProj(Player player, Projectile proj, Player target) => true;
		public virtual void ModifyHitPvpWithProj(Player player, Projectile proj, Player target, ref int damage, ref bool crit) { }
		public virtual void OnHitPvpWithProj(Player player, Projectile proj, Player target, int damage, bool crit) { }
		public virtual bool CanBeHitByNPC(Player player, NPC npc, ref int cooldownSlot) => true;
		public virtual void ModifyHitByNPC(Player player, NPC npc, ref int damage, ref bool crit) { }
		public virtual void OnHitByNPC(Player player, NPC npc, int damage, bool crit) { }
		public virtual bool CanBeHitByProjectile(Player player, Projectile proj) => true;
		public virtual void ModifyHitByProjectile(Player player, Projectile proj, ref int damage, ref bool crit) { }
		public virtual void OnHitByProjectile(Player player, Projectile proj, int damage, bool crit) { }
		public virtual void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo) { }
		public virtual void ModifyDrawLayerOrdering(Player player, IDictionary<PlayerDrawLayer, PlayerDrawLayer.Position> positions) { }
		public virtual void HideDrawLayers(Player player, PlayerDrawSet drawInfo) { }
		public virtual void ModifyScreenPosition(Player player) { }
		public virtual void ModifyZoom(Player player, ref float zoom) { }
		public virtual void OnEnterWorld(Player player) { }
		public virtual void OnRespawn(Player player) { }
		public virtual bool CanUseItem(Player player, Item item) => true;
		public virtual IEnumerable<Item> AddStartingItems(Player player, bool mediumCoreDeath) => Enumerable.Empty<Item>();
		public virtual void ModifyStartingInventory(Player player, IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath) { }
	}
}