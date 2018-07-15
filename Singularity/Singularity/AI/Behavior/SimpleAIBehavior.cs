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
        private const int MoveIntervalMillis = 10000;
        [DataMember]
        private const int SpawnIntervalMillis = 1000;

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
            if ((int) gametime.TotalGameTime.TotalMilliseconds % MoveIntervalMillis != 0)
            {
                return;
            }

            foreach(var enemyUnit in mEnemyUnits)
            {
                enemyUnit.SetMovementTarget(GetRandomPositionOnMap());
            }
        }

        public void Spawn(GameTime gametime)
        {
            return;
            if ((int) gametime.TotalGameTime.TotalMilliseconds % SpawnIntervalMillis != 0)
            {
                return;
            }

            foreach (var spawner in mAi.GetSpawners())
            {
                var enemyUnit = spawner.SpawnEnemy(mDirector.GetStoryManager.Level.Camera,
                    mDirector.GetStoryManager.Level.Map,
                    mDirector.GetStoryManager.Level.GameScreen);

                mEnemyUnits.Add(enemyUnit);
            }
        }

        /// <summary>
        /// Gets a valid random position on the current map
        /// </summary>
        /// <returns></returns>
        private Vector2 GetRandomPositionOnMap()
        {
            var isOnMap = false;
            var pos = Vector2.Zero;

            while (!isOnMap)
            {
                pos = new Vector2(mRandom.Next(MapConstants.MapWidth), mRandom.Next(MapConstants.MapHeight));
                if (Map.Map.IsOnTop(pos))
                {
                    isOnMap = true;
                }

            }
            return pos;

        }
    }
}
