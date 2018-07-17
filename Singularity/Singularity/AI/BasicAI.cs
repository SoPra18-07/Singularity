using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.AI.Behavior;
using Singularity.AI.Properties;
using Singularity.AI.Structures;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Property;
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
        private readonly List<Pair<Triple<CommandCenter, List<PlatformBlank>, List<Road>>, Rectangle>> mStructure;

        private readonly List<Rectangle> mBoundsToDraw;

        public BasicAi(EaiDifficulty difficulty, ref Director director)
        {
            Difficulty = difficulty;
            mDirector = director;

            mBoundsToDraw = new List<Rectangle>();

            mStructure = new List<Pair<Triple<CommandCenter, List<PlatformBlank>, List<Road>>, Rectangle>>();
            var structure = StructureLayoutHolder.GetRandomStructureAtCenter(9000, 3000, difficulty, ref director);

            AddStructureToGame(structure.GetFirst(), structure.GetSecond());


            //TODO: change the behavior with the difficulty
            mBehavior = new AdvancedAIBehavior(this, ref director);
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

                foreach (var platform in structure.GetFirst().GetSecond())
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
                if (!structure.GetFirst().GetSecond().Contains(platform))
                {
                    continue;
                }
                structure.GetFirst().GetSecond().Remove(platform);
                return;
            }
        }

        public void AddStructureToGame(Triple<CommandCenter, List<PlatformBlank>, List<Road>> structure, Rectangle bounds)
        {
            mStructure.Add(new Pair<Triple<CommandCenter, List<PlatformBlank>, List<Road>>, Rectangle>(structure, bounds));

            mBoundsToDraw.Add(bounds);

            mDirector.GetStoryManager.Level.GameScreen.AddObject(structure.GetFirst());
            mDirector.GetStoryManager.Level.GameScreen.AddObjects(structure.GetSecond());
            mDirector.GetStoryManager.Level.GameScreen.AddObjects(structure.GetThird());
        }

        public Rectangle GetBoundsOfStructure(int index)
        {
            return mStructure[index].GetSecond();
        }

        public int GetStructureCount()
        {
            return mStructure.Count;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!GlobalVariables.DebugState)
            {
                return;
            }

            foreach (var rectangle in mBoundsToDraw)
            {
                spriteBatch.DrawRectangle(rectangle, Color.Red, 4f, LayerConstants.PlatformLayer);
            }
        }
    }
}
