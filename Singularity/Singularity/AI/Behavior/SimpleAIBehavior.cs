using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
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
    public sealed class SimpleAIBehavior : IAIBehavior
    {
        private readonly IArtificalIntelligence mAi;

        private readonly Director mDirector;

        private readonly List<EnemyUnit> mEnemyUnits;

        private readonly Random mRandom;

        private int mMoveCounter;

        public SimpleAIBehavior(IArtificalIntelligence ai, ref Director director)
        {
            mDirector = director;
            mAi = ai;

            mRandom = new Random();
            mEnemyUnits = new List<EnemyUnit>();

        }

        public void Move(GameTime gametime)
        {
            if ((int) gametime.TotalGameTime.TotalMilliseconds % 10000 != 0)
            {
                return;
            }

            foreach(var enemyUnit in mEnemyUnits)
            {
                enemyUnit.Move(GetRandomPositionOnMap());
            }
        }

        public void Spawn(GameTime gametime)
        {

            if ((int) gametime.TotalGameTime.TotalMilliseconds % 10000 != 0)
            {
                return;
            }

            // spawn a new enemy unit every 30 seconds on every spawner
            foreach (var spawner in mAi.GetSpawners())
            {
                var enemyUnit = spawner.SpawnEnemy(mDirector.GetStoryManager.Level.Camera,
                    mDirector.GetStoryManager.Level.Map,
                    mDirector.GetStoryManager.Level.GameScreen);

                mEnemyUnits.Add(enemyUnit);
            }
        }

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
