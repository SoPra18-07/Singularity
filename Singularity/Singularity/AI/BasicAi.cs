﻿using System.Collections.Generic;
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
using Singularity.Units;
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

        [DataMember]
        private readonly List<Rectangle> mBoundsToDraw;

        [DataMember]
        private int mCommandCenterKillCount;

        [DataMember]
        public bool IsTutorial { set; get; }

        public BasicAi(EaiDifficulty difficulty, ref Director director, bool isTutorial = false)
        {
            Difficulty = difficulty;
            mDirector = director;
            IsTutorial = isTutorial;

            mBoundsToDraw = new List<Rectangle>();

            mStructure = new List<Pair<Triple<CommandCenter, List<PlatformBlank>, List<Road>>, Rectangle>>();

            mBehavior = new AdvancedAiBehavior(this, ref director);
        }

        public void ReloadContent(ref Director dir)
        {
            mDirector = dir;
            mBehavior.ReloadContent(ref mDirector);
        }

        public void Update(GameTime gametime)
        {
            if (IsTutorial)
            {
                return;
            }

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
                    tempList[index].Add(spawner);
                }
                index++;

            }

            return tempList;
        }

        public void Kill(PlatformBlank platform)
        {
            foreach (var structure in mStructure)
            {
                if (platform.Equals(structure.GetFirst().GetFirst()))
                {
                    mCommandCenterKillCount++;
                    structure.GetFirst().GetSecond().Remove(platform);
                    break;
                }

                if (!structure.GetFirst().GetSecond().Contains(platform))
                {
                    continue;
                }
                structure.GetFirst().GetSecond().Remove(platform);
                return;
            }

            if (mCommandCenterKillCount < mStructure.Count)
            {
                return;
            }
            mDirector.GetStoryManager.Win();

        }

        public void Kill(EnemyUnit unit)
        {
            mBehavior.Kill(unit);
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

        public void Shooting(MilitaryUnit sender, ICollider shootingAt, GameTime gametime)
        {
            mBehavior.Shooting(sender, shootingAt, gametime);
        }

    }
}
