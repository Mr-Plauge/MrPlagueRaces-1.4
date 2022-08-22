using System.Collections.Generic;
using Terraria.ModLoader;

namespace MrPlagueRaces.Common.Races
{
	public sealed class RaceLoader : ILoadable
	{
		public static readonly List<Race> Races = new List<Race>();
		public static readonly Dictionary<string, Race> RacesByFullNames = new Dictionary<string, Race>();

        public void Load(Mod mod)
        {

        }
        public void Unload()
        {
            Races.Clear();
			RacesByFullNames.Clear();
		}

		public static void AddRace(Race race)
		{
			race.Id = Races.Count;

			Races.Add(race);
			RacesByFullNames.Add(race.FullName, race);
		}

		public static bool TryGetRace(int id, out Race result)
		{
			if (id >= 0 && id < Races.Count)
			{
				result = Races[id];

				return true;
			}

			result = null;

			return false;
		}

		public static bool TryGetRace(string fullName, out Race result) => RacesByFullNames.TryGetValue(fullName, out result);
	}
}