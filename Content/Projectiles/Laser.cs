using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MrPlagueRaces.Content.Buffs;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces.Common.Races.Lihzahrd;
using static Terraria.ModLoader.ModContent;

namespace MrPlagueRaces.Content.Projectiles
{
	public class Laser : ModProjectile
	{
		public int travelTime;
		public bool playedSound;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Laser");
		}

		public override void SetDefaults() {
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.hide = true;
			Projectile.extraUpdates = 10; // {T} Added this so the laser projectile is more reliable. It effectively increases how fast this projectile ticks, without missing collisions.
		}

		public override void AI() {
			Projectile.timeLeft++;
			travelTime++;
			if (travelTime > 1000) {
				Projectile.ai[1] = Projectile.Center.X;
				Projectile.position.X = Projectile.ai[0];
				if (!playedSound) {
					SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
					playedSound = true;
				}
				travelTime = 0;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X == 0) {
				Projectile.ai[1] = Projectile.Center.X;
				Projectile.position.X = Projectile.ai[0];
				if (!playedSound) {
					SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
					playedSound = true;
				}
				travelTime = 0;
			}
			return false;
		}
	}
}
