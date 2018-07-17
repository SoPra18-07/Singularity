using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using C5;
using Microsoft.Xna.Framework;
using Singularity.AI.Helper;
using Singularity.AI.Properties;
using Singularity.AI.Structures;
using Singularity.Manager;
using Singularity.Units;

namespace Singularity.AI.Behavior
{
    ///  <inheritdoc/>
    ///  <remarks>
    ///  Note: the stuff I'm about to write was written BEFORE actually implementing, somewhat as what
    ///  I would like for this behavior to do.
    ///
    ///  Move: the AI should be able to divide its units into different "fractions", namely Scouting Units,
    ///  Defending Units and Attack Units. This would definitiely make the AI seems more "human" as in not
    ///  just moving straight to the players base with all its units. Before it would first scout with say
    ///  20% of its available units and on players base sight it then should retreat and focus on unit
    ///  production to ready for battle. Where it should maybe always leave behind a set amount of units
    ///  to defend itself for incoming attacks. One could also make it such that with along time passed
    ///  and not having spotted the players base the AI goes more on the offensive/defensive with more
    ///  units defending/scouting as time passes. My initial idea for the scouting part was to create
    ///  sort of a "snapshot" of where the AI has already looked, and not moving there to scout again.
    ///  This might be hard to implement and can probably be simplified in many ways. The moving itself
    ///  will probably work very simple. My idea on the moving was to give every enemy unit a "likelihood"
    ///  of moving which increases if the unit didn't move and time passes, such that every unit basically
    ///  walks at some point and more or less evenly distributed. One could also form "squads" of 5 units or so
    ///  to scout in the same direction to be able to defend itself if a fight breaks out. Those things are currently
    ///  on my mind for moving/unit assignment
    ///
    ///  Spawn: First of all the spawn rate should increase linear to the time passed. There should also be a
    ///  enemy unit limit (maybe by difficulty?) such that the AI doesn't get completely OP in endgame. When the game
    ///  starts the priority of spawning new units should be really low, since otherwise the player wouldn't stand a chance,
    ///  I've thought of maybe having only 1 unit to scout for the first X minutes of the game (balancing, how fast can a
    ///  player actually build units himself), or maybe increasing its unit count ONLY if the player already has built a military
    ///  unit / defense laser (this could also be a feature of only easy AI difficulty, to onyl spawn units when the player
    ///  can defend himself). Other than that there's nothing really noteworthy that I'm thinking about.
    ///
    ///  New Base: So this is probably the most tricky part. I've thought of maybe giving every base a new AI assigned to it
    ///  and having a "SuperAI" which handles communication between those AIs but that probably is a bad idea, since we
    ///  want to use the same behavior for every base. First of all the AI should only spawn new bases if the player
    ///  is "flagged a threat" by the UI, e.g. by having over X platforms or over X military units etc. Ive also had the
    ///  idea of having every base correspond to scouting units or attacking units but thats probably also a bad idea since
    ///  then one could easily rush "scouting bases" etc. The new base gets chosen randomly from all the possible bases
    ///  for the given difficulty. One might also give the AI a X% chance to build a base thats one difficulty above its
    ///  current difficulty just to make things more exciting. The base gets literally just spawned, but it should at all times
    ///  be made sure that it doesn't "collide" with any of the existing bases and should NEVER be in the view of the player
    ///  when spawned, since that would look extremely weird.
    ///  
    /// </remarks>
    public sealed class AdvancedAIBehavior : IAiBehavior
    {
        private const float PriorityAddition = 0.1f;

        private readonly int[] mUnitsMovementCooldown = new int[3] {0, 0, 0};

        private readonly int[] mUnitsMovementSnapshot = new int[3] {0, 0, 0};

        /// <summary>
        /// The time in milliseconds the AI waits to do anything. (when being attacked this is ignored and the AI starts doing stuff nontheless),
        /// where it waits 5 minutes for easy, 4 minutes for medium and 2 minutes for hard.
        /// </summary>
        private readonly int[] mIdleTime = new int[3]
        {
            //300000 this is currently commented since we don't want to wait 5 minutes for debugging, remove this for actual gameplay
            0,
            240000,
            120000
        };

        private readonly int[] mUnitCreationSnapshot = new int[3] {0, 0, 0};

        // the reason for this not being dependant on the difficulty is because the harder difficulity AI has WAY more spawners
        // than the easier ones, which would make it completely broken (probably already is)
        private const int ScoutCreationCooldown = 60000;

        private readonly int[] mAttackCreationCooldown = new int[3] {180000, 120000, 120000};

        private const int ScoutingSquadSize = 3;

        private const int MaxDefendingSquadSize = 3;

        private int mOldPlayerMilitaryUnitCount;



        private bool mShouldAttack;

        private Vector2 mAttackPosition;

        /// <summary>
        /// The ai this behavior is used on.
        /// </summary>
        private readonly IArtificalIntelligence mAi;

        /// <summary>
        /// The difficulty of the ai that uses this behavior. This should be used to set certain
        /// parameters to make the behavior more difficult/easy.
        /// </summary>
        private readonly EaiDifficulty mDifficulty;

        private readonly Random mRandom;

        private readonly Director mDirector;

        private readonly IPriorityQueue<PrioritizableObject<EnemyUnit>> mScoutingUnits;

        private readonly IPriorityQueue<PrioritizableObject<EnemyUnit>> mAttackingUnits;

        private readonly IPriorityQueue<PrioritizableObject<EnemyUnit>> mDefendingUnits;

        private readonly Dictionary<EnemyUnit, bool> mIsCurrentlyMoving;

        public AdvancedAIBehavior(IArtificalIntelligence ai, ref Director director)
        {
            mAi = ai;
            mDirector = director;
            mDifficulty = ai.Difficulty;
            mRandom = new Random();

            mIsCurrentlyMoving = new Dictionary<EnemyUnit, bool>();

            mScoutingUnits = new IntervalHeap<PrioritizableObject<EnemyUnit>>(new PrioritizableObjectAscendingComparer<EnemyUnit>());
            mAttackingUnits = new IntervalHeap<PrioritizableObject<EnemyUnit>>(new PrioritizableObjectAscendingComparer<EnemyUnit>());
            mDefendingUnits = new IntervalHeap<PrioritizableObject<EnemyUnit>>(new PrioritizableObjectAscendingComparer<EnemyUnit>());

            //this behavior starts off with one unit
            var spawners = ai.GetSpawners();

            if (spawners.Count <= 0)
            {
                // we don't have any spawners available in the given structure thus not able to spawn any enemy units.
                return;
            }

            if (spawners[0].Count <= 0)
            {
                return;
            }

            // we definitely have one spawner on the initial position, generate one scouting unit.
            SpawnOneUnit(EEnemyType.Scout, ai.GetSpawners()[0][0]);

            // also generate some defending units, note that these don't move away from their base, but are more or less stationary defenders.
            SpawnOneUnit(EEnemyType.Defend, ai.GetSpawners()[0][0]);
            SpawnOneUnit(EEnemyType.Defend, ai.GetSpawners()[0][0]);
            SpawnOneUnit(EEnemyType.Defend, ai.GetSpawners()[0][0]);
            SpawnOneUnit(EEnemyType.Defend, ai.GetSpawners()[0][0]);
            SpawnOneUnit(EEnemyType.Defend, ai.GetSpawners()[0][0]);
        }

        public void CreateNewBase(GameTime gametime)
        {

        }

        public void Move(GameTime gametime)
        {
            if (gametime.TotalGameTime.TotalMilliseconds < mIdleTime[(int) mAi.Difficulty])
            {
                return;
            }

            UpdateMovingValues();

            //first of update the priority of the unit that has the lowest of them all.
            UpdateLowestPriorityQueueElements();

            #region Scouting

            if (mUnitsMovementCooldown[(int)EEnemyType.Scout] <= 0)
            {
                // this is the time in millis that the unit stands still after having reached its position. This allows the player
                // to properly counter the scouts if lucky, these are most likely the only movement values to be tinkered with with
                // increasing difficulty.
                mUnitsMovementCooldown[(int)EEnemyType.Scout] = mRandom.Next(1000, 10000);
            }

            // first handle scouting units
            if (gametime.TotalGameTime.TotalMilliseconds - mUnitsMovementSnapshot[(int)EEnemyType.Scout] > mUnitsMovementCooldown[(int) EEnemyType.Scout])
            {
                // so the idea is to move ONE squad of units specified by ScoutingSquadSize. The squad gets "randomly rolled" everytime
                // this code gets called, this makes the AI difficult to predict.

                var queue = GetPrioritiyQueueByEnemyType(EEnemyType.Scout);

                var squadMembers = new List<EnemyUnit>();

                while (!queue.IsEmpty && squadMembers.Count < ScoutingSquadSize)
                {
                    // we don't wanna do anything if the one with highest priority is currently already moving.
                    // this allows for smaller squad building if not enough units are currently available.
                    if (mIsCurrentlyMoving[queue.Max().GetObject()])
                    {
                        break;
                    }

                    squadMembers.Add(queue.DeleteMax().GetObject());
                }

                // TODO: create some sort of map snapshot to not scout into the same position more than once.
                var position = Map.Map.GetRandomPositionOnMap();

                // the only reason for the offset is to actually see that multiple units are moving and them not overlapping.
                // this will not be needed anymore with flocking.
                var offset = 0;

                foreach (var squadMember in squadMembers)
                {
                    // TODO: if a scout unit attacks a player platform, retreat and add these to defend/attack, since
                    // we no longer need scouts, so we can focus on attacking/defending.
                    squadMember.SetMovementTarget(position + new Vector2(offset, offset));
                    mIsCurrentlyMoving[squadMember] = true;
                    AddToQueue(EEnemyType.Scout, squadMember);

                    offset += 20;
                }

                mUnitsMovementSnapshot[(int) EEnemyType.Scout] = (int) gametime.TotalGameTime.TotalMilliseconds;

            }
            #endregion

            #region Attacking

            // first of all update all the cooldowns if thy were reset.
            if (mUnitsMovementCooldown[(int)EEnemyType.Attack] <= 0)
            {
                //attacking units shouldn't stand still for long since they are designed for combat and should be versatile in reacting.
                mUnitsMovementCooldown[(int)EEnemyType.Attack] = mRandom.Next(500, 1000);
            }


            // only move the attacking units if they should actually attack, this allows for units to wait after a battle and await their backup
            if (gametime.TotalGameTime.TotalMilliseconds - mUnitsMovementSnapshot[(int) EEnemyType.Attack] > mUnitsMovementCooldown[(int) EEnemyType.Attack] && mShouldAttack)
            {
                // the idea for attacking units is to only be produced when the AI knows where the player is. These will form waves and
                // go straight to the enemy base.

                var queue = GetPrioritiyQueueByEnemyType(EEnemyType.Attack);

                var squadMembers = new List<EnemyUnit>();

                while (!queue.IsEmpty)
                {
                    // basically we want to get all the units we have at our disposal to attack.

                    squadMembers.Add(queue.DeleteMax().GetObject());
                }

                // TODO: create some sort of map snapshot to not scout into the same position more than once.
                var position = mAttackPosition;

                // the only reason for the offset is to actually see that multiple units are moving and them not overlapping.
                // this will not be needed anymore with flocking.
                var offset = 0;

                foreach (var squadMember in squadMembers)
                {
                    squadMember.SetMovementTarget(position + new Vector2(offset, offset));
                    mIsCurrentlyMoving[squadMember] = true;
                    AddToQueue(EEnemyType.Attack, squadMember);

                    offset += 10;
                }

                mUnitsMovementSnapshot[(int)EEnemyType.Attack] = (int)gametime.TotalGameTime.TotalMilliseconds;
            }

            #endregion

            #region Defending

            if (mUnitsMovementCooldown[(int)EEnemyType.Defend] <= 0)
            {
                // this is the time in millis that the unit stands still after having moved and reached its position. Since defending units don't
                // need to move that much this is definitely OK. Note this is only for the "idle" movement. If attacked behavior might change
                mUnitsMovementCooldown[(int)EEnemyType.Defend] = mRandom.Next(1000, 10000);
            }

            if (gametime.TotalGameTime.TotalMilliseconds - mUnitsMovementSnapshot[(int)EEnemyType.Defend] > mUnitsMovementCooldown[(int)EEnemyType.Defend])
            {
                // the idea is for defending units to only move around bases of the ai, this might leave some bases undefended, which is fine,
                // since otherwise the player might be too much handicapped

                var queue = GetPrioritiyQueueByEnemyType(EEnemyType.Defend);

                var squadMembers = new List<EnemyUnit>();

                while (!queue.IsEmpty && squadMembers.Count < MaxDefendingSquadSize)
                {
                    // we want to maximally reposition the amount specified in MaxDefendingSquadSize

                    squadMembers.Add(queue.DeleteMax().GetObject());
                }

                // so we basically want to walk on the edges of the rectangle.
                var randomBounds = mAi.GetBoundsOfStructure(mRandom.Next(mAi.GetStructureCount()));

                foreach (var squadMember in squadMembers)
                {
                    squadMember.SetMovementTarget(GetRandomPositionOnRectangle(randomBounds));
                    mIsCurrentlyMoving[squadMember] = true;
                    AddToQueue(EEnemyType.Defend, squadMember);
                }

                mUnitsMovementSnapshot[(int)EEnemyType.Defend] = (int)gametime.TotalGameTime.TotalMilliseconds;
            }

            #endregion
        }

        public void Spawn(GameTime gametime)
        {
            if (gametime.TotalGameTime.TotalMilliseconds < mIdleTime[(int)mAi.Difficulty])
            {
                return;
            }

            // ok so after some long thinking, ive decided that the AI always "tries" to have as many attacking units as the player has military units
            // with a small margin. Maybe for easy the AI strives to have Player.MilitaryCount - 5 and on Hard even Player.Military + 1 or smth. The way
            // i though of this to make it somewhat "balanced" is that the AI always "lags behind" the player in creation. For example: the AI
            // creates a snapshot of player military units and creates that much in 3 minutes. Then it creates a new snapshot of player units and
            // again creates this much in 3 minutes. This way the player has the upper hand for 3 minutes. This time can be changed with difficulty

            // I'd say the AI spawns more scouting units as time passes. things need to be carefully thought through though, since it is super likely
            // for the AI to find the players base rather easily since the units move SUPER fast. I'd say we delay AI actions for 5 minutes (scouting, attacking)
            // and after that set time it starts scouting and maybe creates a new scouting unit every 1 minute. If the player is detected those are evenly distributed
            // to defending/attacking units and the AI focuses on that (if the AI was attacked by the player those become defending units, and otherwise attacking)

            // If the players base has been detected the AI stocks up on military units as mentioned above.

            #region Scouting

            //TODO: check whether the enemy has already attacked you or you detected him
            if (gametime.TotalGameTime.TotalMilliseconds - mUnitCreationSnapshot[(int) EEnemyType.Scout] > ScoutCreationCooldown)
            {
                var spawners = mAi.GetSpawners();

                foreach (var indexToSpawners in spawners)
                {
                    foreach (var spawner in indexToSpawners.Value)
                    {
                        // spawn one scout unit for every spawner the enemy has
                        SpawnOneUnit(EEnemyType.Scout, spawner);
                    }
                }

                mUnitCreationSnapshot[(int) EEnemyType.Scout] = (int) gametime.TotalGameTime.TotalMilliseconds;
            }

            #endregion

            //TODO: only run this code when the enemy has been detected or your base has been detected
            if (gametime.TotalGameTime.TotalMilliseconds - mUnitCreationSnapshot[(int) EEnemyType.Attack] >
                mAttackCreationCooldown[(int) mAi.Difficulty])
            {
                // the timer is over, create as much units as the player had at that moment

            }

        }

        private Vector2 GetRandomPositionOnRectangle(Rectangle randomBounds)
        {
            var movingOnXorY = mRandom.Next(2);
            var movingLeftOrRight = mRandom.Next(2);

            var position = Vector2.Zero;

            if (movingOnXorY == 0 && movingLeftOrRight == 0)
            {
                // move on the left side of the rectangle
                position = new Vector2(randomBounds.X, mRandom.Next(randomBounds.Y, randomBounds.Y + randomBounds.Height));

            }
            else if (movingOnXorY == 0 && movingLeftOrRight == 1)
            {
                // move on the right side of the rectangle
                position = new Vector2(randomBounds.X + randomBounds.Width, mRandom.Next(randomBounds.Y, randomBounds.Y + randomBounds.Height));

            }
            else if (movingOnXorY == 1 && movingLeftOrRight == 0)
            {
                // move on the top side of the rectangle
                position = new Vector2(mRandom.Next(randomBounds.X, randomBounds.X + randomBounds.Width), randomBounds.Y);

            }
            else if (movingOnXorY == 1 && movingLeftOrRight == 1)
            {
                // move on the bottom side of the rectangle
                position = new Vector2(mRandom.Next(randomBounds.X, randomBounds.X + randomBounds.Width), randomBounds.Y + randomBounds.Height);
            }

            return position;
        }

        private void SetAttackTarget(Vector2 attackPosition)
        {
            mShouldAttack = true;
            mAttackPosition = attackPosition;
        }

        private void SpawnOneUnit(EEnemyType type, Spawner spawner)
        {
            var unit = spawner.SpawnEnemy(type, mDirector.GetStoryManager.Level.Camera,
                mDirector.GetStoryManager.Level.Map,
                mDirector.GetStoryManager.Level.GameScreen);

            //newly spawned units have no extra priority over others already existing.
            AddToQueue(type, unit);

            mIsCurrentlyMoving[unit] = false;
        }

        private void AddToQueue(EEnemyType type, EnemyUnit unit, float prio = 0f)
        {
            GetPrioritiyQueueByEnemyType(type).Add(new PrioritizableObject<EnemyUnit>(unit, 0f));
        }

        private IPriorityQueue<PrioritizableObject<EnemyUnit>> GetPrioritiyQueueByEnemyType(EEnemyType type)
        {
            switch (type)
            {
                case EEnemyType.Attack:
                    return mAttackingUnits;

                case EEnemyType.Defend:
                    return mDefendingUnits;

                case EEnemyType.Scout:
                    return mScoutingUnits;

                default:
                    throw new Exception("Unexpected enemy type");
            }
        }

        private void UpdateMovingValues()
        {
            var toChange = new List<EnemyUnit>();

            foreach (var keyValue in mIsCurrentlyMoving)
            {
                if (!keyValue.Value)
                {
                    continue;
                }

                if (!keyValue.Key.Moved)
                {
                    toChange.Add(keyValue.Key);
                }
            }

            foreach (var enemy in toChange)
            {
                mIsCurrentlyMoving[enemy] = false;
            }
        }

        private void UpdateLowestPriorityQueueElements()
        {
            UpdateLowestPriorityQueueElement(EEnemyType.Attack);
            UpdateLowestPriorityQueueElement(EEnemyType.Defend);
            UpdateLowestPriorityQueueElement(EEnemyType.Scout);
        }

        private void UpdateLowestPriorityQueueElement(EEnemyType type)
        {
            if (GetPrioritiyQueueByEnemyType(type).Count <= 0)
            {
                return;
            }

            var minPrioUnit = GetPrioritiyQueueByEnemyType(type).DeleteMin();

            // we don't want to increase the priority if the unit is moving, this allows for us to not give units paths that
            // are already moving to one.
            if (mIsCurrentlyMoving[minPrioUnit.GetObject()])
            {
                GetPrioritiyQueueByEnemyType(type).Add(minPrioUnit);
                return;
            }

            minPrioUnit.SetPrioritization(minPrioUnit.GetPrioritization() + PriorityAddition);
            Debug.WriteLine(minPrioUnit.GetPrioritization() + ", " + mIsCurrentlyMoving[minPrioUnit.GetObject()]);

            GetPrioritiyQueueByEnemyType(type).Add(minPrioUnit);
        }

        private float GetRandomPriority()
        {
            return (float) mRandom.NextDouble();
        }

        public void ReloadContent(ref Director dir)
        {
            throw new NotImplementedException();
        }
    }
}



