using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.AI.Behavior;
using Singularity.AI.Properties;
using Singularity.AI.Structures;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.AI
{
    public sealed class BasicAi : IArtificalIntelligence
    {
        public EAIDifficulty Difficulty { get; private set; }

        private readonly Director mDirector;

        private readonly IAIBehavior mBehavior;

        private readonly Triple<CommandCenter, List<PlatformBlank>, List<Road>> mStructure;

        public BasicAi(EAIDifficulty difficulty, ref Director director)
        {
            Difficulty = difficulty;
            mDirector = director;

            //TODO: change the behavior with the difficulty
            mBehavior = new SimpleAIBehavior(this, ref director);

            mStructure = StructureLayoutHolder.GetRandomStructureAtCenter(9000, 3000, difficulty);

            director.GetStoryManager.Level.GameScreen.AddObject(mStructure.GetFirst());
            director.GetStoryManager.Level.GameScreen.AddObjects(mStructure.GetSecond());
            director.GetStoryManager.Level.GameScreen.AddObjects(mStructure.GetThird());
        }

        public void Update(GameTime gametime)
        {

            mBehavior.Spawn(gametime);

            mBehavior.Move(gametime);
        }

        public IEnumerable<Spawner> GetSpawners()
        {
            var tempList = new List<Spawner>();

            foreach (var platform in mStructure.GetSecond())
            {
                var spawner = platform as Spawner;

                if (spawner == null)
                {
                    continue;
                }
                tempList.Add(spawner);
            }

            return tempList;
        }

    }
}
