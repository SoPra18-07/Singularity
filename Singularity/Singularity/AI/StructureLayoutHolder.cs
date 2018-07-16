using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.AI.Properties;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Utils;

namespace Singularity.AI
{
    /// <summary>
    /// This class provides set structures for the AI to get, such that the AI always has a number of different
    /// structures at its disposal for different difficulties
    /// </summary>
    public static class StructureLayoutHolder
    {
        // adjust these accordingly when adding new structures to the AI
        private const int EasyStructureCount = 1;
        private const int MediumStructureCount = 0;
        private const int HardStructureCount = 0;

        private static Dictionary<EaiDifficulty, Triple<CommandCenter, List<PlatformBlank>, List<Road>>[]> sAllStructures;

        public static Triple<CommandCenter, List<PlatformBlank>, List<Road>> GetRandomStructureAtCenter(float x, float y, EaiDifficulty difficulty)
        {
            var rnd = new Random();

            var structure = sAllStructures[difficulty][rnd.Next(sAllStructures[difficulty].Length - 1)];

            // everything thats happening below here is to adjust the position of the taken structure to its new center.
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

            return structure;
        }

        public static Triple<CommandCenter, List<PlatformBlank>, List<Road>> GetRandomStructureAtCenter(Vector2 center, EaiDifficulty difficulty)
        {
            return GetRandomStructureAtCenter(center.X, center.Y, difficulty);
        }

        /// <summary>
        /// Initializes this StructureLayoutHolder object, which sets up all the structures the AI has at its disposal.
        /// </summary>
        /// <param name="director"></param>
        public static void Initialize(ref Director director)
        {
            sAllStructures = new Dictionary<EaiDifficulty, Triple<CommandCenter, List<PlatformBlank>, List<Road>>[]>();

            sAllStructures[EaiDifficulty.Easy] = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>[EasyStructureCount];
            sAllStructures[EaiDifficulty.Medium] = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>[MediumStructureCount];
            sAllStructures[EaiDifficulty.Hard] = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>[HardStructureCount];

            #region 1. Easy Structure

            var struct1CommandCenter = (CommandCenter) PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false);

            var struct1Platforms = new List<PlatformBlank>();
            var struct1Plat1 = PlatformFactory.Get(EStructureType.Spawner, ref director, -200, commandBlueprint:false);
            var struct1Plat2 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200, commandBlueprint:false);

            struct1Platforms.Add(struct1Plat1);
            struct1Platforms.Add(struct1Plat2);


            var struct1Roads = new List<Road>();
            struct1Roads.Add(new Road(struct1CommandCenter, struct1Plat2, ref director));
            struct1Roads.Add(new Road(struct1CommandCenter, struct1Plat1, ref director));

            var struct1 = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct1CommandCenter, struct1Platforms, struct1Roads);

            sAllStructures[EaiDifficulty.Easy][0] = struct1;

            #endregion

            #region 2. Easy Structure

            var struct2CommandCenter = (CommandCenter)PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false);

            #endregion
        }
    }
}
