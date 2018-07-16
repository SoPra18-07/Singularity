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

        // this is a representation of the structure this AI operates on, this is a list since the AI might possibly have multiple bases
        [DataMember]
        private readonly List<Triple<CommandCenter, List<PlatformBlank>, List<Road>>> mStructure;

        public BasicAi(EaiDifficulty difficulty, ref Director director)
        {
            Difficulty = difficulty;
            mDirector = director;

            //TODO: change the behavior with the difficulty
            mBehavior = new SimpleAIBehavior(this, ref director);

            mStructure = new List<Triple<CommandCenter, List<PlatformBlank>, List<Road>>>();
            AddStructureToGame(StructureLayoutHolder.GetRandomStructureAtCenter(9000, 3000, difficulty, ref director).GetFirst());
        }

        public void ReloadContent(ref Director dir)
        {
            mDirector = dir;
            mBehavior.ReloadContent(ref mDirector);
        }

        public void Update(GameTime gametime)
        {
            mBehavior.CreateNewBase(gametime);

            mBehavior.Spawn(gametime);

            mBehavior.Move(gametime);
        }

        public Dictionary<int, List<Spawner>> GetSpawners()
        {
            var tempList = new Dictionary<int, List<Spawner>>();

            var index = 0;

            foreach (var structure in mStructure)
            {
                tempList[index] = new List<Spawner>();

                foreach (var platform in structure.GetSecond())
                {
                    var spawner = platform as Spawner;

                    if (spawner == null)
                    {
                        continue;
                    }
                    tempList[0].Add(spawner);
                }

                index++;
            }

            return tempList;
        }

        public void Kill(PlatformBlank platform)
        {
            foreach (var structure in mStructure)
            {
                if (!structure.GetSecond().Contains(platform))
                {
                    continue;
                }
                structure.GetSecond().Remove(platform);
                return;
            }
        }

        public void AddStructureToGame(Triple<CommandCenter, List<PlatformBlank>, List<Road>> structure)
        {
            mStructure.Add(structure);

            mDirector.GetStoryManager.Level.GameScreen.AddObject(structure.GetFirst());
            mDirector.GetStoryManager.Level.GameScreen.AddObjects(structure.GetSecond());
            mDirector.GetStoryManager.Level.GameScreen.AddObjects(structure.GetThird());
        }
    }
}
