using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.AI.Behavior;
using Singularity.AI.Properties;
using Singularity.AI.Structures;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Utils;

namespace Singularity.AI
{
    /// <summary>
    /// A basic AI implementation which simply chooses a random structure to play the game and
    /// calls Move() and Spawn() of its behavior object
    /// </summary>
    [DataContract]
    public sealed class BasicAi : IArtificalIntelligence
    {
        [DataMember]
        public EaiDifficulty Difficulty { get; private set; }

        private Director mDirector;
        [DataMember]
        private readonly IAiBehavior mBehavior;

        // this is a representation of the structure this AI operates on.
        [DataMember]
        private readonly Triple<CommandCenter, List<PlatformBlank>, List<Road>> mStructure;

        public BasicAi(EaiDifficulty difficulty, ref Director director)
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

        public void ReloadContent(ref Director dir)
        {
            mDirector = dir;
            mBehavior.ReloadContent(ref mDirector);
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

        public void Kill(PlatformBlank platform)
        {
            mStructure.GetSecond().Remove(platform);
        }
    }
}
