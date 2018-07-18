using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using C5;
using Microsoft.Xna.Framework;
using Singularity.AI.Helper;
using Singularity.AI.Properties;
using Singularity.AI.Structures;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Units;
using Singularity.Utils;

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
    ///
    ///  This is what the behavior already does:
    ///
    ///  Move: The AI has 3 different unit "typed", attacking, scouting and defending, each of them being a different
    ///  enemy unit, where scouting is a fast unit, attacking a normal one and defending a heavy one. The scouting units
    ///  always try to avoid battle and instantly flee after attacking something, whereas the defending units and the attacking
    ///  ones always focus down certain targets that are dangerous. The snapshot mentioned above is not yet implemented, since
    ///  I think the remaining time should be invested into bugfixes. All of the units move in squads, where the scouting units
    ///  always try to form squads of three, and enemy units try to move with all their forces at once. All the stuff like when
    ///  to move etc. is handled in the respective method and can be read there. Scouting units serve as "markers" if you will,
    ///  as soon as a scouting unit attacks something that "something" is marked for the AI as hostile and it will move with
    ///  its attacking units there if it decides that it is ready to attack. This actually looks pretty cool in action
    ///
    ///  Spawn: So I made it in such a way that the AI doesn't do anything the first X minutes (defined by difficulty) and only
    ///  "activates" itself after said minutes or if it gets attacked directly (one of its units). The spawning itself is pretty simple
    ///  every X seconds a new scouting unit spawns, and every "base" comes with 5 defending units. The spawning of the attacking units
    ///  is as described above, the AI creates a snapshot at one time and in X minutes creates as much attacking units as the player
    ///  had then, and repeats itself. The AI can never have more units than the player once had at his peak. (this also serves as
    ///  a unit limitation)
    ///
    ///  New Base: I've kept the conditions for a new base pretty simple. Right now if the player has more than 20 platforms OR more than
    ///  4 defensive platforms a new base gets created by the AI. This works in such a way that for the 3rd base the condition is 40 platforms OR
    ///  8 defensive platforms etc. Every base spawns with its own set of defensive units, and defending units roam freely between all bases.
    ///  Thats probably about it with this one. The functionality of not spawning in the players viewable area should be fulfilled, but its
    ///  hard to test, since it is completely random where they get placed. (I've never had them be in my viewable area and I've tested hours and hours).
    ///  Bases should also not be able to overlap with eachother
    /// </remarks>
    [DataContract]
    public sealed class AdvancedAiBehavior : IAiBehavior
    {
        private const int PlatformCountNewBaseTrigger = 20;

        private const int DefensePlatformCountNewBaseTrigger = 4;

        private const float PriorityAddition = 0.1f;

        [DataMember]
        private readonly int[] mUnitsMovementCooldown = new int[3] {0, 0, 0};

        [DataMember]
        private readonly int[] mUnitsMovementSnapshot = new int[3] {0, 0, 0};

        /// <summary>
        /// The time in milliseconds the AI waits to do anything. (when being attacked this is ignored and the AI starts doing stuff nontheless),
        /// where it waits 5 minutes for easy, 4 minutes for medium and 2 minutes for hard.
        /// </summary>
        [DataMember]
        private readonly int[] mIdleTime = new int[3]
        {
            300000,
            240000,
            120000
        };

        [DataMember]
        private readonly int[] mUnitCreationSnapshot = new int[3] {0, 0, 0};

        // the reason for this not being dependant on the difficulty is because the harder difficulity AI has WAY more spawners
        // than the easier ones, which would make it completely broken (probably already is)
        [DataMember]
        private const int ScoutCreationCooldown = 60000;

        [DataMember]
        private readonly int[] mAttackCreationCooldown = new int[3]
        {
            180000,
            120000,
            60000
        };
        
        private const int ScoutingSquadSize = 3;

        private const int MaxDefendingSquadSize = 3;

        [DataMember]
        private int mOldPlayerMilitaryUnitCount;

        [DataMember]
        private bool mActive;

        [DataMember]
        private int mBaseCount;

        /// <summary>
        /// as long as this is true the AI will keep attacking the target specified in mAttackPosition.
        /// </summary>
        [DataMember]
        private bool mShouldAttack;

        [DataMember]
        private ICollider mAttackPosition;

        /// <summary>
        /// The ai this behavior is used on.
        /// </summary>
        [DataMember]
        private readonly IArtificalIntelligence mAi;

        /// <summary>
        /// The difficulty of the ai that uses this behavior. This should be used to set certain
        /// parameters to make the behavior more difficult/easy.
        /// </summary>
        [DataMember]
        private readonly EaiDifficulty mDifficulty;

        [DataMember]
        private readonly Random mRandom;

        [DataMember]
        private Director mDirector;

        [DataMember]
        private List<Rectangle> mCollidingRects;

        //note, the heap implementation cannot be serialized.

        private IPriorityQueue<PrioritizableObject<EnemyUnit>> mScoutingUnits;

        private IPriorityQueue<PrioritizableObject<EnemyUnit>> mAttackingUnits;

        private IPriorityQueue<PrioritizableObject<EnemyUnit>> mDefendingUnits;

        [DataMember]
        private readonly List<PrioritizableObject<EnemyUnit>> mAllUnits;

        [DataMember]
        private readonly Dictionary<EnemyUnit, bool> mIsCurrentlyMoving;

        public AdvancedAiBehavior(IArtificalIntelligence ai, ref Director director)
        {
            mAi = ai;
            mDirector = director;
            mDifficulty = ai.Difficulty;
            mRandom = new Random();

            mIsCurrentlyMoving = new Dictionary<EnemyUnit, bool>();

            mCollidingRects = new List<Rectangle>();
            mScoutingUnits = new IntervalHeap<PrioritizableObject<EnemyUnit>>(new PrioritizableObjectAscendingComparer<EnemyUnit>());
            mAttackingUnits = new IntervalHeap<PrioritizableObject<EnemyUnit>>(new PrioritizableObjectAscendingComparer<EnemyUnit>());
            mDefendingUnits = new IntervalHeap<PrioritizableObject<EnemyUnit>>(new PrioritizableObjectAscendingComparer<EnemyUnit>());
            mAllUnits = new List<PrioritizableObject<EnemyUnit>>();

            CreateNewBase(null);
        }

        public void CreateNewBase(GameTime gametime)
        {
            if (mBaseCount > 0 && !mActive)
            {
                // don't build any further bases when the AI isn't active.
                return;
            }

            if (!(mDirector.GetMilitaryManager.PlayerPlatformCount > PlatformCountNewBaseTrigger * mBaseCount ||
                  mDirector.GetMilitaryManager.PlayerDefensePlatformCount > DefensePlatformCountNewBaseTrigger * mBaseCount))
            {
                return;
            }

            var requestNewPlatform = false;

            Pair<Triple<CommandCenter, List<PlatformBlank>, List<Road>>, Rectangle> baseToAdd;

            do
            {
                baseToAdd = StructureLayoutHolder.GetStructureOnMap(mAi.Difficulty, ref mDirector);

                // make sure the new structure doesn't overlap with an existing enemy structure
                if (mBaseCount <= 0)
                {
                    break;
                }

                foreach (var rect in mCollidingRects)
                {
                    if (!rect.Intersects(baseToAdd.GetSecond()))
                    {
                        continue;
                    }

                    requestNewPlatform = true;
                    break;
                }

            } while (requestNewPlatform);

            mAi.AddStructureToGame(baseToAdd.GetFirst(), baseToAdd.GetSecond());

            mCollidingRects.Add(baseToAdd.GetSecond());

            var spawners = mAi.GetSpawners();

            if (spawners.Count <= mBaseCount || spawners[mBaseCount].Count <= 0)
            {
                // we don't have any spawners available in the given structure thus not able to spawn any enemy units.
                mBaseCount++;
                return;
            }

            Debug.WriteLine("spawn now");

            SpawnOneUnit(EEnemyType.Scout, mAi.GetSpawners()[mBaseCount][0]);

            // also generate some defending units, note that these don't move away from their base, but are more or less stationary defenders.
            SpawnOneUnit(EEnemyType.Defend, mAi.GetSpawners()[mBaseCount][0]);
            SpawnOneUnit(EEnemyType.Defend, mAi.GetSpawners()[mBaseCount][0]);
            SpawnOneUnit(EEnemyType.Defend, mAi.GetSpawners()[mBaseCount][0]);
            SpawnOneUnit(EEnemyType.Defend, mAi.GetSpawners()[mBaseCount][0]);
            SpawnOneUnit(EEnemyType.Defend, mAi.GetSpawners()[mBaseCount][0]);

            mBaseCount++;
        }

        public void Move(GameTime gametime)
        {
            if (mBaseCount <= 0)
            {
                return;
            }

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

            if (gametime.TotalGameTime.TotalMilliseconds > mIdleTime[(int) mAi.Difficulty] && !mActive)
            {
                mActive = true;
            }

            if (!mActive)
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
                // var map = mDirector.GetStoryManager.Level.Map;
                // var squad = new FlockingGroup(ref mDirector, ref map);

                // while (!queue.IsEmpty && squad.Count < ScoutingSquadSize)
                while (!queue.IsEmpty && squadMembers.Count < ScoutingSquadSize)
                {
                    // we don't wanna do anything if the one with highest priority is currently already moving.
                    // this allows for smaller squad building if not enough units are currently available.
                    if (mIsCurrentlyMoving[queue.Max().GetObject()])
                    {
                        break;
                    }
                    // squad.AssignUnit(queue.DeleteMax().GetObject());
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
                    squadMember.SetMovementTarget(position.Center + new Vector2(position.AbsBounds.Width / 2f, position.AbsBounds.Height / 2f) + new Vector2(offset, offset));
                    mIsCurrentlyMoving[squadMember] = true;
                    AddToQueue(EEnemyType.Attack, squadMember);

                    offset += 10;
                }

                mUnitsMovementSnapshot[(int)EEnemyType.Attack] = (int)gametime.TotalGameTime.TotalMilliseconds;
            }

            #endregion
        }

        public void Spawn(GameTime gametime)
        {
            if (gametime.TotalGameTime.TotalMilliseconds > mIdleTime[(int)mAi.Difficulty] && !mActive)
            {
                mActive = true;
            }

            if (!mActive || mBaseCount <= 0)
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
                var spawners = mAi.GetSpawners();

                var myUnitCount = GetPrioritiyQueueByEnemyType(EEnemyType.Attack).Count;

                for (var i = 0; i < mOldPlayerMilitaryUnitCount - myUnitCount; i++)
                {
                    // this represents all the spawners at a random structure of the AI
                    var structureToSpawnAt = spawners[mRandom.Next(spawners.Count)];

                    // this represents one random spawner at the current structure, this might possibly not work, if
                    // there's a structure with no spawner, but if we try to "catch" that bug we might run into an
                    // infinite loop, so I've just left it since this will be read if it happens at some point.
                    // EDIT: this does fix it but not in a good way, since now the AI won't have enough units
                    if (structureToSpawnAt.Count <= 0)
                    {
                        continue;
                    }

                    var randomSpawner = structureToSpawnAt[mRandom.Next(structureToSpawnAt.Count)];
                    
                    SpawnOneUnit(EEnemyType.Attack, randomSpawner);
                } 

                mOldPlayerMilitaryUnitCount = mDirector.GetMilitaryManager.PlayerUnitCount;
                mUnitCreationSnapshot[(int) EEnemyType.Attack] = (int) gametime.TotalGameTime.TotalMilliseconds;
            }

            //TODO: spawn defending units someday

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

        private void SetAttackTarget(ICollider attackPosition)
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
            var toAdd = new PrioritizableObject<EnemyUnit>(unit, prio);

            GetPrioritiyQueueByEnemyType(type).Add(toAdd);
            mAllUnits.Add(toAdd);
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

            GetPrioritiyQueueByEnemyType(type).Add(minPrioUnit);
        }

        public void Kill(EnemyUnit unit)
        {
            var type = EEnemyType.Attack;

            var unitAsHeavy = unit as EnemyHeavy;
            var unitAsFast = unit as EnemyFast;

            if (unitAsHeavy != null)
            {
                type = EEnemyType.Defend;

            }else if (unitAsFast != null)
            {
                type = EEnemyType.Scout;
            }

            // note, a heap structure is definitely no good solution for a kill method (obviously), but it should be ok, since the enemy won't even have over 100 units
            // in its own "assignment" and iterating over max 100 units on a kill call is still ok, therefore the logic for moving is super simple.
            var reAdd = new List<PrioritizableObject<EnemyUnit>>();
            var queue = GetPrioritiyQueueByEnemyType(type);

            while (!queue.IsEmpty)
            {
                var current = queue.DeleteMin();

                if (current.GetObject().Equals(unit))
                {
                    mAllUnits.Remove(current);
                    break;
                }
                reAdd.Add(current);
            }

            foreach (var toAdd in reAdd)
            {
                queue.Add(toAdd);
            }

        }

        public void ReloadContent(ref Director dir)
        {
            mDirector = dir;

            mAttackingUnits = new IntervalHeap<PrioritizableObject<EnemyUnit>>(new PrioritizableObjectAscendingComparer<EnemyUnit>());
            mDefendingUnits = new IntervalHeap<PrioritizableObject<EnemyUnit>>(new PrioritizableObjectAscendingComparer<EnemyUnit>());
            mScoutingUnits = new IntervalHeap<PrioritizableObject<EnemyUnit>>(new PrioritizableObjectAscendingComparer<EnemyUnit>());

            foreach (var unit in mAllUnits)
            {
                var asFast = unit.GetObject() as EnemyFast;
                var asHeavy = unit.GetObject() as EnemyHeavy;

                if (asFast != null)
                {
                    mScoutingUnits.Add(unit);
                    continue;
                }

                if (asHeavy != null)
                {
                    mDefendingUnits.Add(unit);
                    continue;
                }
                mAttackingUnits.Add(unit);
            }

        }

        public void Shooting(MilitaryUnit sender, ICollider shootingAt, GameTime gametime)
        {
            // make sure to active the AI if it wasn't already because something is shooting
            // (either getting shot at or shooting at smth)
            if (!mActive)
            {
                mActive = true;
            }

            // now check whether your units fired or you get attacked by the enemy, we don't really need to do anything if
            // we're not the ones shooting

            var asEnemy = sender as EnemyUnit;

            if (asEnemy == null)
            {
                return;
            }

            var senderAsFast = sender as EnemyFast;
            var senderAsHeavy = sender as EnemyHeavy;

            var targetAsPlatform = shootingAt as PlatformBlank;

            if (senderAsFast != null)
            {
                // the one who shot was a scouting unit, make sure to retreat ASAP
                Retreat(senderAsFast, EEnemyType.Scout, gametime);

                // at this point we know that our scout attacked an enemy platform
                if (targetAsPlatform != null)
                {
                    // make sure to not attack the center, since then the unit wouldn't even move there since there's collision.
                    // this basically sets the position to attack for the AI if it decides to attack.
                    SetAttackTarget(targetAsPlatform);
                }

                return;
            }

            if (senderAsHeavy != null)
            {
                // a defending unit shot, thus move (currently all) defending units to where this happened
                var allDefendings = GetPrioritiyQueueByEnemyType(EEnemyType.Defend).ToList();

                foreach (var defending in allDefendings)
                {
                    defending.GetObject().SetMovementTarget(shootingAt.Center);
                }

                return;
            }

            // now the unit is defintely an attacking one, make sure to retreat once the target is destroyed.

            if (shootingAt.Health > 0)
            {
                SetAttackTarget(shootingAt);

                return;
            }

            mShouldAttack = false;
            Retreat(asEnemy, EEnemyType.Attack, gametime);
        }

        private void Retreat(EnemyUnit unit, EEnemyType type, GameTime gametime)
        {
            // make the scout that attacked retreat to a random structure of the ai
            unit.SetMovementTarget(GetRandomPositionOnRectangle(mAi.GetBoundsOfStructure(mRandom.Next(mAi.GetStructureCount()))));

            mUnitsMovementSnapshot[(int) type] = (int) gametime.TotalGameTime.TotalMilliseconds;
        }
    }
}



