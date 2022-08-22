using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using MrPlagueRaces.Common.Races;
using System.Linq;

namespace MrPlagueRaces.Common.Players
{
	public class RaceHookPlayer : ModPlayer
	{
		public override void ResetEffects()
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ResetEffects(Player);
			}
		}

		public override void UpdateDead()
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.UpdateDead(Player);
			}
		}

		public override void UpdateBadLifeRegen()
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.UpdateBadLifeRegen(Player);
			}
		}

		public override void UpdateLifeRegen()
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.UpdateLifeRegen(Player);
			}
		}

		public override void NaturalLifeRegen(ref float regen)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.NaturalLifeRegen(Player, ref regen);
			}
		}

		public override void PreUpdate()
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.PreUpdate(Player);
			}
		}

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ProcessTriggers(Player, triggersSet);
			}
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				return mrPlagueRacesPlayer.race.PreHurt(Player, pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource, ref cooldownCounter);
			}
			else
			{
				return true;
			}
		}


		public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.Hurt(Player, pvp, quiet, damage, hitDirection, crit, cooldownCounter);
			}
		}

		public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.PostHurt(Player, pvp, quiet, damage, hitDirection, crit, cooldownCounter);
			}
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				return mrPlagueRacesPlayer.race.PreKill(Player, damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
			}
			else
			{
				return true;
			}
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.Kill(Player, damage, hitDirection, pvp, damageSource);
			}
		}

		public override void OnHitAnything(float x, float y, Entity victim)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnHitAnything(Player, x, y, victim);
			}
		}

		public override bool? CanHitNPC(Item item, NPC target)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				return mrPlagueRacesPlayer.race.CanHitNPC(Player, item, target);
			}
			else
			{
				return null;
			}
		}

		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyHitNPC(Player,item, target, ref damage, ref knockback, ref crit);
			}
		}

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnHitNPC(Player, item, target, damage, knockback, crit);
			}
		}

		public override bool? CanHitNPCWithProj(Projectile proj, NPC target)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				return mrPlagueRacesPlayer.race.CanHitNPCWithProj(Player, proj, target);
			}
			else
			{
				return null;
			}
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyHitNPCWithProj(Player, proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
			}
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnHitNPCWithProj(Player, proj, target, damage, knockback, crit);
			}
		}

		public override bool CanHitPvp(Item item, Player target)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				return mrPlagueRacesPlayer.race.CanHitPvp(Player, item, target);
			}
			else
			{
				return true;
			}
		}

		public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyHitPvp(Player, item, target, ref damage, ref crit);
			}
		}

		public override void OnHitPvp(Item item, Player target, int damage, bool crit)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnHitPvp(Player, item, target, damage, crit);
			}
		}

		public override bool CanHitPvpWithProj(Projectile proj, Player target)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				return mrPlagueRacesPlayer.race.CanHitPvpWithProj(Player, proj, target);
			}
			else
			{
				return true;
			}
		}

		public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyHitPvpWithProj(Player, proj, target, ref damage, ref crit);
			}
		}

		public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnHitPvpWithProj(Player, proj, target, damage, crit);
			}
		}

		public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				return mrPlagueRacesPlayer.race.CanBeHitByNPC(Player, npc, ref cooldownSlot);
			}
			else
			{
				return true;
			}
		}

		public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyHitByNPC(Player, npc, ref damage, ref crit);
			}
		}

		public override void OnHitByNPC(NPC npc, int damage, bool crit)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnHitByNPC(Player, npc, damage, crit);
			}
		}

		public override bool CanBeHitByProjectile(Projectile proj)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				return mrPlagueRacesPlayer.race.CanBeHitByProjectile(Player, proj);
			}
			else
			{
				return true;
			}
		}

		public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyHitByProjectile(Player, proj, ref damage, ref crit);
			}
		}

		public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnHitByProjectile(Player, proj, damage, crit);
			}
		}

		public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyDrawInfo(Player, ref drawInfo);
			}
		}

		public override void HideDrawLayers(PlayerDrawSet drawInfo)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.HideDrawLayers(Player, drawInfo);
			}
		}

		public override void ModifyScreenPosition()
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyScreenPosition(Player);
			}
		}

		public override void ModifyZoom(ref float zoom)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyZoom(Player, ref zoom);
			}
		}

		public override void OnEnterWorld(Player player)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnEnterWorld(Player);
			}
		}

		public override void OnRespawn(Player player)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnRespawn(Player);
			}
		}

		public override bool CanUseItem(Item item)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				return mrPlagueRacesPlayer.race.CanUseItem(Player, item);
			}
			else
			{
				return true;
			}
		}

		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				return mrPlagueRacesPlayer.race.AddStartingItems(Player, mediumCoreDeath);
			}
			else
			{
				return Enumerable.Empty<Item>();
			}
		}

		public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyStartingInventory(Player, itemsByMod, mediumCoreDeath);
			}
		}
	}
}