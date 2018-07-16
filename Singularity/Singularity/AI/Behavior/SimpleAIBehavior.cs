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
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.AI.Behavior
{
    /// <summary>
    /// A simple implementation of an AI behavior. Simply spawns units at set intervals and
    /// moves them all at once to random locations at set intervals.
    /// </summary>
    [DataContract]
    public sealed class SimpleAIBehavior : IAiBehavior
    {
        [DataMember]
        private int MoveIntervalMillis = 0;
        [DataMember]
        private int SpawnIntervalMillis = 0;

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

        public SimpleAIBehavior(IArtificalIntelligence ai, ref Director director)
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
            if (MoveIntervalMillis <= 0)
            {
                MoveIntervalMillis = mRandom.Next(500, 1000);
            }

            if ((int) gametime.TotalGameTime.TotalMilliseconds % MoveIntervalMillis != 0)
            {
                return;
            }

            if (mEnemyUnits.Count <= 0)
            {
                return;
            }

            mEnemyUnits[mRandom.Next(mEnemyUnits.Count)].SetMovementTarget(Map.Map.GetRandomPositionOnMap());

            MoveIntervalMillis = 0;
        }

        public void Spawn(GameTime gametime)
        {

            if (SpawnIntervalMillis <= 0)
            {
                SpawnIntervalMillis = mRandom.Next(1000, 10000);
            }

            if ((int) gametime.TotalGameTime.TotalMilliseconds % SpawnIntervalMillis != 0)
            {
                return;
            }

            var index = mRandom.Next(mAi.GetSpawners().Count);

            foreach (var spawner in mAi.GetSpawners()[index])
            {
                var enemyUnit = spawner.SpawnEnemy(mDirector.GetStoryManager.Level.Camera,
                    mDirector.GetStoryManager.Level.Map,
                    mDirector.GetStoryManager.Level.GameScreen);

                mEnemyUnits.Add(enemyUnit);
            }

            SpawnIntervalMillis = 0;

        }

        public void CreateNewBase(GameTime gametime)
        {
            if ((int)gametime.TotalGameTime.TotalMilliseconds % 10000 != 0)
            {
                return;
            }

            var structure = StructureLayoutHolder.GetStructureOnMap(mAi.Difficulty, ref mDirector);
            mAi.AddStructureToGame(structure.GetFirst(), structure.GetSecond());
        }
    }
}
