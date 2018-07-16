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
        private const int EasyStructureCount = 3;
        private const int MediumStructureCount = 1;
        private const int HardStructureCount = 0;

        private static Dictionary<EaiDifficulty, Triple<CommandCenter, List<PlatformBlank>, List<Road>>[]> sAllStructures;

        public static Triple<CommandCenter, List<PlatformBlank>, List<Road>> GetRandomStructureAtCenter(float x, float y, EaiDifficulty difficulty)
        {
            var rnd = new Random();
            //var structure = sAllStructures[difficulty][sAllStructures[difficulty].Length - 1];
            var structure = sAllStructures[EaiDifficulty.Easy][2];
            // 
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
            //One Spawner then one Command Center then one Sentinal laser tower in a line

            var struct1CommandCenter = (CommandCenter) PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false, false);

            var struct1Platforms = new List<PlatformBlank>();
            var struct1Plat1 = PlatformFactory.Get(EStructureType.Spawner, ref director, -200);
            var struct1Plat2 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200);

            struct1Platforms.Add(struct1Plat1);
            struct1Platforms.Add(struct1Plat2);


            var struct1Roads = new List<Road>();
            struct1Roads.Add(new Road(struct1CommandCenter, struct1Plat2, ref director));
            struct1Roads.Add(new Road(struct1CommandCenter, struct1Plat1, ref director));

            var struct1 = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct1CommandCenter, struct1Platforms, struct1Roads);

            sAllStructures[EaiDifficulty.Easy][0] = struct1;

            #endregion

            #region 2. Easy Structure
            // a Command Center surrounded by four Sentinal defense towers

            var struct2CommandCenter = (CommandCenter)PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false, false);

            var struct2Platforms = new List<PlatformBlank>();
            var struct2Plat1 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -200);
            var struct2Plat2 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200);
            var struct2Plat3 = PlatformFactory.Get(EStructureType.Sentinel, ref director, y: 200);
            var struct2Plat4 = PlatformFactory.Get(EStructureType.Sentinel, ref director, y:-200);


            struct2Platforms.Add(struct2Plat1);
            struct2Platforms.Add(struct2Plat2);
            struct2Platforms.Add(struct2Plat3);
            struct2Platforms.Add(struct2Plat4);


            var struct2Roads = new List<Road>();
            struct2Roads.Add(new Road(struct2CommandCenter, struct2Plat1, ref director));
            struct2Roads.Add(new Road(struct2CommandCenter, struct2Plat2, ref director));
            struct2Roads.Add(new Road(struct2CommandCenter, struct2Plat3, ref director));
            struct2Roads.Add(new Road(struct2CommandCenter, struct2Plat4, ref director));

            var struct2 = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct2CommandCenter, struct2Platforms, struct2Roads);

            sAllStructures[EaiDifficulty.Easy][1] = struct2;

            #endregion

            #region 3. Easy Structure
            // One Command Center, one Spawner, and one Sentinal in a triangle layout

            var struct3ECommandCenter = (CommandCenter)PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false, false);

            var struct3EPlatforms = new List<PlatformBlank>();
            var struct3EPlat1 = PlatformFactory.Get(EStructureType.Spawner, ref director, -400);
            var struct3EPlat2 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -200, -200);

            struct3EPlatforms.Add(struct3EPlat1);
            struct3EPlatforms.Add(struct3EPlat2);


            var struct3ERoads = new List<Road>();
            struct3ERoads.Add(new Road(struct3ECommandCenter, struct3EPlat1, ref director));
            struct3ERoads.Add(new Road(struct3ECommandCenter, struct3EPlat2, ref director));
            struct3ERoads.Add(new Road(struct3EPlat1, struct3EPlat2, ref director));

            var struct3E = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct3ECommandCenter, struct3EPlatforms, struct3ERoads);

            sAllStructures[EaiDifficulty.Easy][2] = struct3E;

            #endregion

            #region 1. Medium Structure
            // a Spawner and Command Center in the Middle with 6 Sentinal laser towers surrounding it

            var struct1MCommandCenter = (CommandCenter)PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false, false);

            var struct1MPlatforms = new List<PlatformBlank>();
            var struct1MPlat1 = PlatformFactory.Get(EStructureType.Spawner, ref director, -200);
            var struct1MPlat2 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -100, -200);
            var struct1MPlat3 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -100, 200);
            var struct1MPlat4 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -400, -100);
            var struct1MPlat5 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -400, 100);
            var struct1MPlat6 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200, -100);
            var struct1MPlat7 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200, 100);

            struct1MPlatforms.Add(struct1MPlat1);
            struct1MPlatforms.Add(struct1MPlat2);
            struct1MPlatforms.Add(struct1MPlat3);
            struct1MPlatforms.Add(struct1MPlat4);
            struct1MPlatforms.Add(struct1MPlat5);
            struct1MPlatforms.Add(struct1MPlat6);
            struct1MPlatforms.Add(struct1MPlat7);

            var struct1MRoads = new List<Road>();
            struct1MRoads.Add(new Road(struct1MCommandCenter, struct1MPlat1, ref director));
            struct1MRoads.Add(new Road(struct1MCommandCenter, struct1MPlat2, ref director));
            struct1MRoads.Add(new Road(struct1MCommandCenter, struct1MPlat3, ref director));
            struct1MRoads.Add(new Road(struct1MCommandCenter, struct1MPlat6, ref director));
            struct1MRoads.Add(new Road(struct1MCommandCenter, struct1MPlat7, ref director));

            struct1MRoads.Add(new Road(struct1MPlat1, struct1MPlat2, ref director));
            struct1MRoads.Add(new Road(struct1MPlat1, struct1MPlat3, ref director));
            struct1MRoads.Add(new Road(struct1MPlat1, struct1MPlat4, ref director));
            struct1MRoads.Add(new Road(struct1MPlat1, struct1MPlat5, ref director));

            var struct1M = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct1MCommandCenter, struct1MPlatforms, struct1MRoads);

            sAllStructures[EaiDifficulty.Medium][0] = struct1M;


            #endregion

            #region 2. Medium Structure
            var struct2MCommandCenter = (CommandCenter)PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false, false);

            var struct2MPlatforms = new List<PlatformBlank>();
            var struct2MPlat1 = PlatformFactory.Get(EStructureType.Blank, ref director, -500, -100, friendly:false, commandBlueprint:false);
            var struct2MPlat2 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -300, -150);
            var struct2MPlat3 = PlatformFactory.Get(EStructureType.Spawner, ref director, -150, -200);
            var struct2MPlat4 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200, -150);
            var struct2MPlat5 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 150, -100);




            var struct2MPlat6 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200, -100);
            var struct2MPlat7 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200, 100);
            var struct2MPlat8 = PlatformFactory.Get(EStructureType.Blank, ref director, 200, 200);

            struct1MPlatforms.Add(struct1MPlat1);
            struct1MPlatforms.Add(struct1MPlat2);
            struct1MPlatforms.Add(struct1MPlat3);
            struct1MPlatforms.Add(struct1MPlat4);
            struct1MPlatforms.Add(struct1MPlat5);
            struct1MPlatforms.Add(struct1MPlat6);
            struct1MPlatforms.Add(struct1MPlat7);
            /*
            var struct1MRoads = new List<Road>();
            struct1MRoads.Add(new Road(struct1MCommandCenter, struct1MPlat1, ref director));
            struct1MRoads.Add(new Road(struct1MCommandCenter, struct1MPlat2, ref director));
            struct1MRoads.Add(new Road(struct1MCommandCenter, struct1MPlat3, ref director));
            struct1MRoads.Add(new Road(struct1MCommandCenter, struct1MPlat6, ref director));
            struct1MRoads.Add(new Road(struct1MCommandCenter, struct1MPlat7, ref director));

            struct1MRoads.Add(new Road(struct1MPlat1, struct1MPlat2, ref director));
            struct1MRoads.Add(new Road(struct1MPlat1, struct1MPlat3, ref director));
            struct1MRoads.Add(new Road(struct1MPlat1, struct1MPlat4, ref director));
            struct1MRoads.Add(new Road(struct1MPlat1, struct1MPlat5, ref director));

            var struct1M = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct1MCommandCenter, struct1MPlatforms, struct1MRoads);

            sAllStructures[EaiDifficulty.Medium][0] = struct1M;
            */

            #endregion
        }
    }
}
