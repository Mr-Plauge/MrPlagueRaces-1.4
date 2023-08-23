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

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyHurt(Player, ref modifiers);
			}
		}

		public override void PostHurt(Player.HurtInfo info)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.PostHurt(Player, info);
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

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyHitNPC(Player, target, ref modifiers);
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnHitNPC(Player, target, hit, damageDone);
			}
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyHitNPCWithProj(Player, proj, target, ref modifiers);
			}
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnHitNPCWithProj(Player, proj, target, hit, damageDone);
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

		public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyHitByNPC(Player, npc, ref modifiers);
			}
		}

		public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnHitByNPC(Player, npc, hurtInfo);
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

		public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.ModifyHitByProjectile(Player, proj, ref modifiers);
			}
		}

		public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnHitByProjectile(Player, proj, hurtInfo);
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

		public override void OnEnterWorld()
		{
			var mrPlagueRacesPlayer = Player.GetModPlayer<MrPlagueRacesPlayer>();
			if (mrPlagueRacesPlayer.race != null)
			{
				mrPlagueRacesPlayer.race.OnEnterWorld(Player);
			}
		}

		public override void OnRespawn()
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