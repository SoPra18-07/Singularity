using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.AI.Properties;
using Singularity.Manager;
using Singularity.Map.Properties;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.AI.Behavior
{
    /// <summary>
    /// A simple implementation of an AI behavior. Simply spawns units at set intervals and
    /// moves them all at once to random locations at set intervals. This is outdated and probably won't work as expected
    /// </summary>
    [DataContract]
    public sealed class SimpleAiBehavior : IAiBehavior
    {
        [DataMember]
        private int mMoveIntervalMillis = 0;
        [DataMember]
        private int mSpawnIntervalMillis = 0;

        /// <summary>
        /// The AI this behavior operates on
        /// </summary>
        [DataMember]
        private readonly IArtificalIntelligence mAi;

        private Director mDirector;

        /// <summary>
        /// A list of all the enemy units currently spawned by the AI
        /// </summary>
        [DataMember]
        private readonly List<EnemyUnit> mEnemyUnits;

        [DataMember]
        private readonly Random mRandom;

        public SimpleAiBehavior(IArtificalIntelligence ai, ref Director director)
        {
            mDirector = director;
            mAi = ai;

            mRandom = new Random();
            mEnemyUnits = new List<EnemyUnit>();

        }

        public void ReloadContent(ref Director dir)
        {
            mDirector = dir;

        }

        public void Move(GameTime gametime)
        {
            if (mMoveIntervalMillis <= 0)
            {
                mMoveIntervalMillis = mRandom.Next(500, 1000);
            }

            if ((int) gametime.TotalGameTime.TotalMilliseconds % mMoveIntervalMillis != 0)
            {
                return;
            }

            if (mEnemyUnits.Count <= 0)
            {
                return;
            }

            mEnemyUnits[mRandom.Next(mEnemyUnits.Count)].SetMovementTarget(Map.Map.GetRandomPositionOnMap());

            mMoveIntervalMillis = 0;
        }

        public void Spawn(GameTime gametime)
        {

            if (mSpawnIntervalMillis <= 0)
            {
                mSpawnIntervalMillis = mRandom.Next(1000, 10000);
            }

            if ((int) gametime.TotalGameTime.TotalMilliseconds % mSpawnIntervalMillis != 0)
            {
                return;
            }

            var index = mRandom.Next(mAi.GetSpawners().Count);

            foreach (var spawner in mAi.GetSpawners()[index])
            { 
                var enemyUnit = spawner.SpawnEnemy(EEnemyType.Attack, 
                    mDirector.GetStoryManager.Level.Camera,
                    mDirector.GetStoryManager.Level.Map,
                    mDirector.GetStoryManager.Level.GameScreen);

                mEnemyUnits.Add(enemyUnit);
            }

            mSpawnIntervalMillis = 0;

        }

        public void CreateNewBase(GameTime gametime)
        {
            // don't do this right now since its super annoying for people actually trying to do other stuff
            return;

            if ((int)gametime.TotalGameTime.TotalMilliseconds % 10000 != 0)
            {
                return;
            }

            var structure = StructureLayoutHolder.GetStructureOnMap(mAi.Difficulty, ref mDirector);
            mAi.AddStructureToGame(structure.GetFirst(), structure.GetSecond());
        }

        public void Kill(EnemyUnit unit)
        {
            throw new NotImplementedException();
        }

        public void Shooting(MilitaryUnit sender, ICollider shootingAt)
        {
            throw new NotImplementedException();
        }

        public void Shooting(MilitaryUnit sender, ICollider shootingAt, GameTime gametime)
        {
            throw new NotImplementedException();
        }
    }
}
