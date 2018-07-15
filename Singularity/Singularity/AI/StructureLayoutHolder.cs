using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.AI.Properties;
using Singularity.AI.Structures;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Utils;

namespace Singularity.AI
{
    public static class StructureLayoutHolder
    {
        // adjust these accordingly when adding new structures to the AI
        private const int EasyStructureCount = 1;
        private const int MediumStructureCount = 0;
        private const int HardStructureCount = 0;

        private static Dictionary<EAIDifficulty, Triple<CommandCenter, List<PlatformBlank>, List<Road>>[]> sAllStructures;

        public static Triple<CommandCenter, List<PlatformBlank>, List<Road>> GetRandomStructureAtCenter(float x, float y, EAIDifficulty difficulty)
        {
            var rnd = new Random();

            var structure = sAllStructures[difficulty][rnd.Next(sAllStructures[difficulty].Length - 1)];

            structure.GetFirst().AbsolutePosition += new Vector2(x, y);
            structure.GetFirst().UpdateValues();

            foreach (var platform in structure.GetSecond())
            {
                platform.AbsolutePosition += new Vector2(x, y);
                platform.UpdateValues();
            }

            foreach (var road in structure.GetThird())
            {
                road.ResetCenterValues();
            }

            // return a random structure for the given difficulty
            return structure;
        }

        public static Triple<CommandCenter, List<PlatformBlank>, List<Road>> GetRandomStructureAtCenter(Vector2 center, EAIDifficulty difficulty)
        {
            return GetRandomStructureAtCenter(center.X, center.Y, difficulty);
        }

        public static void Initialize(ref Director director)
        {
            sAllStructures = new Dictionary<EAIDifficulty, Triple<CommandCenter, List<PlatformBlank>, List<Road>>[]>();

            sAllStructures[EAIDifficulty.Easy] = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>[EasyStructureCount];
            sAllStructures[EAIDifficulty.Medium] = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>[MediumStructureCount];
            sAllStructures[EAIDifficulty.Hard] = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>[HardStructureCount];

            #region 1. Easy Structure

            var struct1CommandCenter = (CommandCenter) PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false);

            var struct1Platforms = new List<PlatformBlank>();
            var struct1Plat1 = PlatformFactory.Get(EStructureType.Spawner, ref director, -200);
            var struct1Plat2 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200);

            struct1Platforms.Add(struct1Plat1);
            struct1Platforms.Add(struct1Plat2);


            var struct1Roads = new List<Road>();
            struct1Roads.Add(new Road(struct1CommandCenter, struct1Plat2, ref director));
            struct1Roads.Add(new Road(struct1CommandCenter, struct1Plat1, ref director));

            var struct1 = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct1CommandCenter, struct1Platforms, struct1Roads);

            sAllStructures[EAIDifficulty.Easy][0] = struct1;

            #endregion
        }
    }
}
