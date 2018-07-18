using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.AI.Properties;
using Singularity.AI.Structures;
using Singularity.Graph;
using Singularity.Manager;
using Singularity.Map.Properties;
using Singularity.Platforms;
using Singularity.Property;
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
        private const int MediumStructureCount = 2;
        private const int HardStructureCount = 2;

        private static Dictionary<EaiDifficulty, Triple<CommandCenter, List<PlatformBlank>, List<Road>>[]> sAllStructures;

        public static Pair<Triple<CommandCenter, List<PlatformBlank>, List<Road>>, Rectangle> GetRandomStructureAtCenter(float x, float y, EaiDifficulty difficulty, ref Director director)
        {
            var rnd = new Random();

            // everything thats happening below here is to adjust the position of the taken structure to its new center.

            var index = rnd.Next(sAllStructures[difficulty].Length);

            var structure = sAllStructures[difficulty][index]; 

            // make sure to not take the same reference as in this sAllStructures dict. Otherwise the AI can't take the same structure more than once.
            // -> recreate every object and give that to the caller

            var tempOldPlatNewPlatMapping = new Dictionary<INode, PlatformBlank>();

            var commandCenter = (CommandCenter) PlatformFactory.Get(EStructureType.Command,
                ref director,
                structure.GetFirst().AbsolutePosition.X + x,
                structure.GetFirst().AbsolutePosition.Y + y, null, false, false);

            tempOldPlatNewPlatMapping[structure.GetFirst()] = commandCenter;

            var boundingRectangle = commandCenter.AbsBounds;

            var platformList = new List<PlatformBlank>();


            foreach (var platform in structure.GetSecond())
            {
                var platformToAdd = PlatformFactory.Get(platform.GetMyType(),
                    ref director,
                    platform.AbsolutePosition.X + x,
                    platform.AbsolutePosition.Y + y, null, false);
                platformToAdd.Built();

                boundingRectangle = UpdateRectangle(boundingRectangle, platformToAdd);

                tempOldPlatNewPlatMapping[platform] = platformToAdd;

                platformList.Add(platformToAdd);
            }

            var roadList = new List<Road>();

            foreach (var road in structure.GetThird())
            {
                var parent = road.GetParent();
                var child = road.GetChild();

                var roadToAdd = new Road(tempOldPlatNewPlatMapping[parent], tempOldPlatNewPlatMapping[child], ref director);
                roadList.Add(roadToAdd);
            }

            return new Pair<Triple<CommandCenter, List<PlatformBlank>, List<Road>>, Rectangle>(new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(commandCenter, platformList, roadList), boundingRectangle);
        }

        private static Rectangle UpdateRectangle(Rectangle oldRect, ICollider platform)
        {
            var tempRect = new Rectangle(oldRect.X, oldRect.Y, oldRect.Width, oldRect.Height);

            if (platform.AbsBounds.X + platform.AbsBounds.Width > oldRect.X + oldRect.Width)
            {
                tempRect.Width = (platform.AbsBounds.X - oldRect.X) + platform.AbsBounds.Width;
            }

            if (platform.AbsBounds.X  < oldRect.X)
            {
                tempRect.X = platform.AbsBounds.X;
                tempRect.Width = oldRect.Width + oldRect.X - platform.AbsBounds.X;
            }

            if (platform.AbsBounds.Y + platform.AbsBounds.Height > oldRect.Y + oldRect.Height)
            {
                tempRect.Height = (platform.AbsBounds.Y - oldRect.Y) + platform.AbsBounds.Height;
            }

            if (platform.AbsBounds.Y < oldRect.Y)
            {
                tempRect.Y = platform.AbsBounds.Y;
                tempRect.Height = oldRect.Height + oldRect.Y - platform.AbsBounds.Y;
            }

            return tempRect;
        }
        public static Pair<Triple<CommandCenter, List<PlatformBlank>, List<Road>>, Rectangle> GetStructureOnMap(EaiDifficulty difficulty, ref Director director)
        {
            while (true)
            {
                var pos = Map.Map.GetRandomPositionOnMap();
                var possibleStructure = GetRandomStructureAtCenter(pos.X, pos.Y, difficulty, ref director);

                if (Map.Map.IsOnTop(possibleStructure.GetSecond()) &&
                    !director.GetStoryManager.Level.Map.IsInVision(possibleStructure.GetSecond()))
                {
                    return possibleStructure;
                }
            }
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
            // 1 CC, 1 spawner, 1 sentinal : traingle formation

            var struct1CommandCenter = (CommandCenter) PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false, false);

            var struct1Platforms = new List<PlatformBlank>();
            var struct1Plat1 = (Spawner) PlatformFactory.Get(EStructureType.Spawner, ref director, -200, -110);
            var struct1Plat2 = (Sentinel) PlatformFactory.Get(EStructureType.Sentinel, ref director, 70, -180);

            struct1Platforms.Add(struct1Plat1);
            struct1Platforms.Add(struct1Plat2);


            var struct1Roads = new List<Road>();
            struct1Roads.Add(new Road(struct1CommandCenter, struct1Plat2, ref director));
            struct1Roads.Add(new Road(struct1CommandCenter, struct1Plat1, ref director));
            struct1Roads.Add(new Road(struct1Plat1, struct1Plat2, ref director));

            var struct1 = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct1CommandCenter, struct1Platforms, struct1Roads);

            sAllStructures[EaiDifficulty.Easy][0] = struct1;

            #endregion

            #region 2. Easy Structure
            // 1 CC, 1 spawner, 1 sentinal, 3 blank : a samll tree like strucutre 

            var struct2CommandCenter = (CommandCenter)PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false, false);

            var struct2Platforms = new List<PlatformBlank>();
            var struct2Plat1 = PlatformFactory.Get(EStructureType.Blank, ref director, -200, -110, friendly: false);
            var struct2Plat2 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200, -30);
            var struct2Plat3 = PlatformFactory.Get(EStructureType.Blank, ref director, -150, -250, friendly: false);
            var struct2Plat4 = PlatformFactory.Get(EStructureType.Spawner, ref director, 70, -240);
            var struct2Plat5 = PlatformFactory.Get(EStructureType.Blank, ref director, -150, 200, friendly: false);


            struct2Platforms.Add(struct2Plat1);
            struct2Platforms.Add(struct2Plat2);
            struct2Platforms.Add(struct2Plat3);
            struct2Platforms.Add(struct2Plat4);
            struct2Platforms.Add(struct2Plat5);


            var struct2Roads = new List<Road>();
            struct2Roads.Add(new Road(struct2CommandCenter, struct2Plat5, ref director));
            struct2Roads.Add(new Road(struct2CommandCenter, struct2Plat1, ref director));
            struct2Roads.Add(new Road(struct2CommandCenter, struct2Plat2, ref director));
            struct2Roads.Add(new Road(struct2CommandCenter, struct2Plat3, ref director));
            struct2Roads.Add(new Road(struct2CommandCenter, struct2Plat5, ref director));
            struct2Roads.Add(new Road(struct2Plat4, struct2Plat3, ref director));

            var struct2 = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct2CommandCenter, struct2Platforms, struct2Roads);

            sAllStructures[EaiDifficulty.Easy][1] = struct2;

            #endregion

            #region 3. Easy Structure
            // 1 CC, 2 sentinal, 1 spawner, 2 blank : random constellation

            var struct3ECommandCenter = (CommandCenter)PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false, false);

            var struct3EPlatforms = new List<PlatformBlank>();
            var struct3EPlat1 = PlatformFactory.Get(EStructureType.Blank, ref director, -300, 150, friendly: false);
            var struct3EPlat2 = PlatformFactory.Get(EStructureType.Blank, ref director, 200, 200, friendly: false);
            var struct3EPlat3 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 400, 300);
            var struct3EPlat4 = PlatformFactory.Get(EStructureType.Spawner, ref director, -450, 350);
            var struct3EPlat5 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -200, -200);


            struct3EPlatforms.Add(struct3EPlat1);
            struct3EPlatforms.Add(struct3EPlat2);
            struct3EPlatforms.Add(struct3EPlat3);
            struct3EPlatforms.Add(struct3EPlat4);
            struct3EPlatforms.Add(struct3EPlat5);


            var struct3ERoads = new List<Road>();
            struct3ERoads.Add(new Road(struct3ECommandCenter, struct3EPlat1, ref director));
            struct3ERoads.Add(new Road(struct3ECommandCenter, struct3EPlat2, ref director));
            struct3ERoads.Add(new Road(struct3ECommandCenter, struct3EPlat5, ref director));
            struct3ERoads.Add(new Road(struct3EPlat1, struct3EPlat4, ref director));
            struct3ERoads.Add(new Road(struct3EPlat2, struct3EPlat3, ref director));
            struct3ERoads.Add(new Road(struct3EPlat4, struct3EPlat2, ref director));
            struct3ERoads.Add(new Road(struct3EPlat1, struct3EPlat2, ref director));

            var struct3E = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct3ECommandCenter, struct3EPlatforms, struct3ERoads);

            sAllStructures[EaiDifficulty.Easy][2] = struct3E;

            #endregion

            #region 1. Medium Structure
            // 1 CC, 1 spawner, 3 sentinal, 3 blank : cc center point with 3 arms

            var struct1MCommandCenter = (CommandCenter)PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false, false);

            var struct1MPlatforms = new List<PlatformBlank>();
            var struct1MPlat1 = PlatformFactory.Get(EStructureType.Spawner, ref director, -200, -110);
            var struct1MPlat2 = PlatformFactory.Get(EStructureType.Blank, ref director, -100, -200, friendly: false);
            var struct1MPlat3 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -170, 200);
            var struct1MPlat4 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -600, -440);
            var struct1MPlat5 = PlatformFactory.Get(EStructureType.Blank, ref director, -400, 100, friendly: false);
            var struct1MPlat6 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 400, -300);
            var struct1MPlat7 = PlatformFactory.Get(EStructureType.Blank, ref director, 200, 150, friendly: false);

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
            // 1 CC, 2 spawner, 3 sentinal, 2 blank : random constellation

            var struct2MCommandCenter = (CommandCenter)PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false, false);

            var struct2MPlatforms = new List<PlatformBlank>();
            var struct2MPlat1 = PlatformFactory.Get(EStructureType.Spawner, ref director, -200, 110);
            var struct2MPlat2 = PlatformFactory.Get(EStructureType.Spawner, ref director, 60, -260);
            var struct2MPlat3 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200, -400);
            var struct2MPlat4 = PlatformFactory.Get(EStructureType.Sentinel, ref director, y: 300);
            var struct2MPlat5 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -230, -70);
            var struct2MPlat6 = PlatformFactory.Get(EStructureType.Blank, ref director, 220, 80, friendly: false);
            var struct2MPlat7 = PlatformFactory.Get(EStructureType.Blank, ref director, -300, -120, friendly: false);

            struct2MPlatforms.Add(struct2MPlat1);
            struct2MPlatforms.Add(struct2MPlat2);
            struct2MPlatforms.Add(struct2MPlat3);
            struct2MPlatforms.Add(struct2MPlat4);
            struct2MPlatforms.Add(struct2MPlat5);
            struct2MPlatforms.Add(struct2MPlat6);
            struct2MPlatforms.Add(struct2MPlat7);

            
            var struct2MRoads = new List<Road>();
            struct2MRoads.Add(new Road(struct2MCommandCenter, struct2MPlat1, ref director));
            struct2MRoads.Add(new Road(struct2MCommandCenter, struct2MPlat2, ref director));
            struct2MRoads.Add(new Road(struct2MCommandCenter, struct2MPlat5, ref director));
            struct2MRoads.Add(new Road(struct2MCommandCenter, struct2MPlat6, ref director));           
            struct2MRoads.Add(new Road(struct2MPlat1, struct2MPlat4, ref director));
            struct2MRoads.Add(new Road(struct2MPlat5, struct2MPlat7, ref director));
            struct2MRoads.Add(new Road(struct2MPlat2, struct2MPlat3, ref director));
            struct2MRoads.Add(new Road(struct2MPlat4, struct2MPlat6, ref director));
            struct2MRoads.Add(new Road(struct2MPlat6, struct2MPlat2, ref director));
            struct2MRoads.Add(new Road(struct2MPlat7, struct2MPlat2, ref director));

            var struct2M = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct2MCommandCenter, struct2MPlatforms, struct2MRoads);

            sAllStructures[EaiDifficulty.Medium][1] = struct2M;


            #endregion

            #region 3. Medium Structure



            #endregion

            #region 1. Hard Structure
            // 1 CC, 4 sentinal, 1 spawner : circular constellation around spawner and CC

            var struct1HCommandCenter = (CommandCenter)PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false, false);

            var struct1HPlatforms = new List<PlatformBlank>();
            var struct1HPlat1 = PlatformFactory.Get(EStructureType.Spawner, ref director, -200);
            var struct1HPlat2 = PlatformFactory.Get(EStructureType.Blank, ref director, -100, -200, friendly: false);
            var struct1HPlat3 = PlatformFactory.Get(EStructureType.Blank, ref director, -100, 200, friendly: false);
            var struct1HPlat4 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -400, -100);
            var struct1HPlat5 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -400, 100);
            var struct1HPlat6 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200, -100);
            var struct1HPlat7 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200, 100);

            struct1HPlatforms.Add(struct1HPlat1);
            struct1HPlatforms.Add(struct1HPlat2);
            struct1HPlatforms.Add(struct1HPlat3);
            struct1HPlatforms.Add(struct1HPlat4);
            struct1HPlatforms.Add(struct1HPlat5);
            struct1HPlatforms.Add(struct1HPlat6);
            struct1HPlatforms.Add(struct1HPlat7);

            var struct1HRoads = new List<Road>();
            struct1HRoads.Add(new Road(struct1HCommandCenter, struct1HPlat1, ref director));
            struct1HRoads.Add(new Road(struct1HCommandCenter, struct1HPlat2, ref director));
            struct1HRoads.Add(new Road(struct1HCommandCenter, struct1HPlat3, ref director));
            struct1HRoads.Add(new Road(struct1HCommandCenter, struct1HPlat6, ref director));
            struct1HRoads.Add(new Road(struct1HCommandCenter, struct1HPlat7, ref director));

            struct1HRoads.Add(new Road(struct1HPlat1, struct1HPlat2, ref director));
            struct1HRoads.Add(new Road(struct1HPlat1, struct1HPlat3, ref director));
            struct1HRoads.Add(new Road(struct1HPlat1, struct1HPlat4, ref director));
            struct1HRoads.Add(new Road(struct1HPlat1, struct1HPlat5, ref director));

            var struct1H = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct1HCommandCenter, struct1HPlatforms, struct1HRoads);

            sAllStructures[EaiDifficulty.Hard][0] = struct1H;
            #endregion

            #region 2. Hard Structure
            // 1 CC, 2 spawner, 3 sentinal : star of death

            var struct2HCommandCenter = (CommandCenter)PlatformFactory.Get(EStructureType.Command, ref director, 0f, 0f, null, false, false);

            var struct2HPlatforms = new List<PlatformBlank>();
            var struct2HPlat1 = PlatformFactory.Get(EStructureType.Spawner, ref director, -340, -120);
            var struct2HPlat2 = PlatformFactory.Get(EStructureType.Spawner, ref director, 300, 110);
            var struct2HPlat3 = PlatformFactory.Get(EStructureType.Sentinel, ref director, 200, -200);
            var struct2HPlat4 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -400, 180);
            var struct2HPlat5 = PlatformFactory.Get(EStructureType.Sentinel, ref director, -100, 300);
            var struct2HPlat6 = PlatformFactory.Get(EStructureType.Blank, ref director, -200, 100, friendly: false);
            var struct2HPlat7 = PlatformFactory.Get(EStructureType.Blank, ref director, -120, 190, friendly: false);
            var struct2HPlat8 = PlatformFactory.Get(EStructureType.Blank, ref director, 60, 210, friendly: false);


            var struct2HPlat9 = PlatformFactory.Get(EStructureType.Blank, ref director, 170, 50, friendly: false);
            var struct2HPlat10 = PlatformFactory.Get(EStructureType.Blank, ref director, -60, -20, friendly: false);

            struct2HPlatforms.Add(struct2HPlat1);
            struct2HPlatforms.Add(struct2HPlat2);
            struct2HPlatforms.Add(struct2HPlat3);
            struct2HPlatforms.Add(struct2HPlat4);
            struct2HPlatforms.Add(struct2HPlat5);
            struct2HPlatforms.Add(struct2HPlat6);
            struct2HPlatforms.Add(struct2HPlat7);
            struct2HPlatforms.Add(struct2HPlat8);
            struct2HPlatforms.Add(struct2HPlat9);
            struct2HPlatforms.Add(struct2HPlat10);


            var struct2HRoads = new List<Road>();
            struct2HRoads.Add(new Road(struct2HCommandCenter, struct2HPlat1, ref director));
            struct2HRoads.Add(new Road(struct2HCommandCenter, struct2HPlat2, ref director));
            struct2HRoads.Add(new Road(struct2HCommandCenter, struct2HPlat3, ref director));
            struct2HRoads.Add(new Road(struct2HCommandCenter, struct2HPlat4, ref director));
            struct2HRoads.Add(new Road(struct2HCommandCenter, struct2HPlat5, ref director));
            struct2HRoads.Add(new Road(struct2HPlat1, struct2HPlat6, ref director));
            struct2HRoads.Add(new Road(struct2HPlat4, struct2HPlat6, ref director));
            struct2HRoads.Add(new Road(struct2HPlat4, struct2HPlat7, ref director));
            struct2HRoads.Add(new Road(struct2HPlat5, struct2HPlat7, ref director));
            struct2HRoads.Add(new Road(struct2HPlat5, struct2HPlat8, ref director));
            struct2HRoads.Add(new Road(struct2HPlat2, struct2HPlat8, ref director));
            struct2HRoads.Add(new Road(struct2HPlat2, struct2HPlat9, ref director));
            struct2HRoads.Add(new Road(struct2HPlat3, struct2HPlat9, ref director));
            struct2HRoads.Add(new Road(struct2HPlat3, struct2HPlat10, ref director));
            struct2HRoads.Add(new Road(struct2HPlat1, struct2HPlat10, ref director));



            var struct2H = new Triple<CommandCenter, List<PlatformBlank>, List<Road>>(struct2HCommandCenter, struct2HPlatforms, struct2HRoads);

            sAllStructures[EaiDifficulty.Hard][1] = struct2H;

            #endregion

            #region 3. Hard Structure
            #endregion
        }
    }
}
