﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph;
using Singularity.PlatformActions;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Platforms
{
    /// <inheritdoc cref="IRevealing"/>
    /// <inheritdoc cref="INode"/>
    /// <inheritdoc cref="ICollider"/>
    [DataContract]
    public class PlatformBlank : IRevealing, INode, ICollider

    {
        /// <summary>
        /// List of inwards facing edges/roads towards the platform.
        /// </summary>
        private List<IEdge> mInwardsEdges;

        /// <summary>
        /// List of outwards facing edges/roads.
        /// </summary>
        private List<IEdge> mOutwardsEdges;

        /// <summary>
        /// Indicates the type of platform this is, defaults to blank.
        /// </summary>
        [DataMember]
        internal EPlatformType mType;

        /// <summary>
        /// Indicates the platform width
        /// </summary>
        [DataMember]
        private const int PlatformWidth = 148;

        /// <summary>
        /// Indicates the platform height.
        /// </summary>
        [DataMember]
        private const int PlatformHeight = 172;

        /// <summary>
        /// How much health the platform has
        /// </summary>
        [DataMember]
        private int mHealth;

        /// <summary>
        /// Indicates if the platform is a "real" platform or a blueprint.
        /// </summary>
        [DataMember]
        protected bool mIsBlueprint;


        [DataMember]
        protected Dictionary<EResourceType, int> mCost;
        [DataMember]
        protected IPlatformAction[] mIPlatformActions;
        private readonly Texture2D mPlatformSpriteSheet;
        private readonly Texture2D mPlatformBaseTexture;
        [DataMember]
        protected string mSpritename;
        [DataMember]
        protected Dictionary<JobType, List<GeneralUnit>> mAssignedUnits;

        [DataMember]
        protected List<Resource> mResources;
        [DataMember]
        protected Dictionary<EResourceType, int> mRequested;

        public Vector2 Center { get; set; }

        public int RevelationRadius { get; } = 200;

        public Rectangle AbsBounds { get; internal set; }

        public bool Moved { get; private set; }

        public int Id { get; }

        // the sprite sheet that should be used. 0 for basic, 1 for cone, 2 for cylinder, 3 for dome
        private int mSheet;
        private int mSheetPosition;

        internal Vector2 GetLocation()
        {
            throw new NotImplementedException();
        }

        [DataMember]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember]
        public Vector2 AbsoluteSize { get; set; }

        [DataMember]
        public Vector2 RelativePosition { get; set; }
        [DataMember]
        public Vector2 RelativeSize { get; set; }

        public bool[,] ColliderGrid { get; internal set; }

        private readonly float mCenterOffsetY;


        public PlatformBlank(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, EPlatformType type = EPlatformType.Blank, float centerOffsetY = -36)
        {

            Id = IdGenerator.NextiD();

            mCenterOffsetY = centerOffsetY;

            mType = type;

            mInwardsEdges = new List<IEdge>();
            mOutwardsEdges = new List<IEdge>();

            AbsolutePosition = position;

            SetPlatfromParameters(); // this changes the draw parameters based on the platform type but
            // also sets the AbsoluteSize and collider grids

            //default?
            mHealth = 100;

            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[1];

            mAssignedUnits = new Dictionary<JobType, List<GeneralUnit>>();
            mAssignedUnits.Add(JobType.Idle, new List<GeneralUnit>());
            mAssignedUnits.Add(JobType.Defense, new List<GeneralUnit>());
            mAssignedUnits.Add(JobType.Production, new List<GeneralUnit>());
            mAssignedUnits.Add(JobType.Logistics, new List<GeneralUnit>());
            mAssignedUnits.Add(JobType.Construction, new List<GeneralUnit>());

            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();

            mResources = new List<Resource>();

            mPlatformSpriteSheet = platformSpriteSheet;
            mPlatformBaseTexture = baseSprite;
            mSpritename = "PlatformBasic";

            mIsBlueprint = true;
            mRequested = new Dictionary<EResourceType, int>();

            Moved = false;

            UpdateValues();

        }

        public void UpdateValues()
        {
            AbsBounds = new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int) AbsoluteSize.X, (int) AbsoluteSize.Y);
            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y + mCenterOffsetY);
        }

        /// <summary>
        /// Get the assigned Units of this platform.
        /// </summary>
        /// <returns> a list containing references of the units</returns>
        public Dictionary<JobType, List<GeneralUnit>> GetAssignedUnits()
        {
            return mAssignedUnits;
        }

        /// <summary>
        /// Assign Units to this platform.
        /// </summary>
        /// <param name="unit">The unit to be assigned.</param>
        /// <param name="job">The Job to be done by the unit</param>
        public void AssignUnits(GeneralUnit unit, JobType job)
        {
            var list = mAssignedUnits[job];
            list.Add(unit);
        }

        /// <summary>
        /// Remove an Assigned Unit from the Assigned List.
        /// </summary>
        /// <param name="unit">The unit to unassign.</param>
        /// <param name="job">The Job of the unit</param>
        public void UnAssignUnits(GeneralUnit unit, JobType job)
        {
            var list = mAssignedUnits[job];
            list.Remove(unit);
        }

        public virtual void Produce()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get the special IPlatformActions you can perform on this platform.
        /// </summary>
        /// <returns> an array with the available IPlatformActions.</returns>
        public IPlatformAction[] GetIPlatformActions()
        {
            return mIPlatformActions;
        }

        /// <summary>
        /// Perform the given PlatformAction on the platform.
        /// </summary>
        /// <param name="platformAction"> The IPlatformAction to be performed </param>
        /// <returns> true if it was succesfull</returns>
        public bool DoIPlatformAction(IPlatformAction platformAction)
        {
            // FIXME might need to give the ID instead
            // This return is normally an if, I just had to do it this way because resharper would cry otherwise.
            // As soon as doBlueprintBuild is implemented we can change this.
            // return (IPlatformAction == IPlatformAction.BlueprintBuild);
            // {
            // doBlueprintBuild
            // return true;
            // }

            return true;
        }

        /// <summary>
        /// Get the requirements of resources to build this platform.
        /// </summary>
        /// <returns> a dictionary of the resources with a number telling how much of it is required</returns>
        public Dictionary<EResourceType, int> GetResourcesRequired()
        {
            return mCost;
        }

        /// <summary>
        /// Get the Resources on the platform.
        /// </summary>
        /// <returns> a List containing the references to the resource-objects</returns>
        public List<Resource> GetPlatformResources()
        {
            return mResources;
        }

        /// <summary>
        /// Get the health points of the platform
        /// </summary>
        /// <returns> the health points as integer</returns>
        public int GetHealth()
        {
            return mHealth;
        }

        /// <summary>
        /// Heal the platform or inflict damage on it.
        /// </summary>
        /// <param name="damage">Positive values for healing, negative for damage</param>
        public void TakeHealDamage(int damage)
        {
            mHealth += damage;
            if (mHealth <= 0)
            {
                if (mType == EPlatformType.Blank) {
                    // TODO: REMOVE from everywhere.
                    // References to Platform are in:
                    // - DistrMgr
                    // - StructureMap
                    // - IPlatformActions (delete them as well!)
                    // - Tasks of Units
                    // - PathFinding
                    // - Roads
                    // - GameScreen (currently hardcoded)
                    // - (Hardcoded in Level)
                    // References to Roads are in:
                    // - Platforms
                    // - StructureMap
                    // - Pathfinding-stuff
                    // References to Resources are in:
                    // - Units (carrying)
                    // - Platforms (on top)
                    // - DistrMgr (?) (reservations)
                    // - Tasks
                    // References to Units are in:
                    // - DistrMgr
                    // - Tasks (from Units and the DistrMgr)
                    // - Platform
                    // - StructureMap (?)
                    // - CommandCenter (-> StoryManager keeping Track of Graph-Unit-Limits)
                    // - IPlatformActions (assigned)
                    // - Resources (when following)
                    // - (Hardcoded in Level)
                }
                // destroyplatform
            }
        }

        /// <summary>
        /// Add a new resource to the platform.
        /// </summary>
        /// <param name="resource"> the resource to be added to the platform </param>
        public void StoreResource(Resource resource)
        {
            mResources.Add(resource);
            Uncollide();
        }

        /// <summary>
        /// Use this method to get the resource you asked for. Removes the resource from the platform.
        /// </summary>
        /// <param name="resourcetype">The resource you ask for</param>
        /// <returns>the resource you asked for, null otherwise.</returns>
        public Optional<Resource> GetResource(EResourceType resourcetype)
        {
            // TODO: reservation of Resources (and stuff)
            var index = mResources.FindIndex(x => x.Type == resourcetype);
            if (index < 0)
            {
                return Optional<Resource>.Of(null);
            }

            var foundresource = mResources[index];
            mResources.RemoveAt(index);
            return Optional<Resource>.Of(foundresource);
        }

        /// <summary>
        /// Get the resources that are requested and the amount of it.
        /// </summary>
        /// <returns>A dictionary containing this information.</returns>
        public Dictionary<EResourceType, int> GetmRequested()
        {
            return mRequested;
        }

        /// <summary>
        /// Change the Resources requested by this platform
        /// </summary>
        /// <param name="resource">the resource to be requested (or not)</param>
        /// <param name="number">the number of that resource</param>
        public void SetmRequested(EResourceType resource, int number)
        {
            mRequested.Add(resource, number);
        }

        /// <inheritdoc cref="Singularity.Property.IDraw"/>
        public void Draw(SpriteBatch spritebatch)
        {
            var transparency = mIsBlueprint ? 0.35f : 1f;

            switch (mSheet)
            {
                case 0:
                    // Basic platform
                    spritebatch.Draw(mPlatformBaseTexture,
                        AbsolutePosition,
                        null,
                        Color.White * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        LayerConstants.BasePlatformLayer);
                    break;
                case 1:
                    // Cone
                    // Draw the basic platform first
                    spritebatch.Draw(mPlatformBaseTexture,
                        Vector2.Add(AbsolutePosition, new Vector2(-3, 73)),
                        null,
                        Color.White * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        LayerConstants.BasePlatformLayer);
                    // then draw what's on top of that
                    spritebatch.Draw(mPlatformSpriteSheet,
                        AbsolutePosition,
                        new Rectangle(PlatformWidth * mSheetPosition, 0, 148, 148),
                        Color.White * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        LayerConstants.PlatformLayer);
                    break;
                case 2:
                    // Cylinder
                    // Draw the basic platform first
                    spritebatch.Draw(mPlatformBaseTexture,
                        Vector2.Add(AbsolutePosition, new Vector2(-3, 82)),
                        null,
                        Color.White * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        LayerConstants.BasePlatformLayer);
                    // then draw what's on top of that
                    spritebatch.Draw(mPlatformSpriteSheet,
                        AbsolutePosition,
                        new Rectangle(PlatformWidth * mSheetPosition, 0, 148, 153),
                        Color.White * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        LayerConstants.PlatformLayer);
                    break;
                case 3:
                    // Draw the basic platform first
                    spritebatch.Draw(mPlatformBaseTexture,
                        Vector2.Add(AbsolutePosition, new Vector2(-3, 38)),
                        null,
                        Color.White * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        LayerConstants.BasePlatformLayer);
                    // Dome
                    spritebatch.Draw(mPlatformSpriteSheet,
                        AbsolutePosition,
                        new Rectangle(148 * (mSheetPosition % 4), 109 * (int) Math.Floor(mSheetPosition / 4d), 148, 109),
                        Color.White * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        LayerConstants.PlatformLayer);
                    break;
            }

            // also draw the resources on top

            foreach (var res in mResources)
            {
                res.Draw(spritebatch);
            }
        }

        /// <inheritdoc cref="Singularity.Property.IUpdate"/>
        public void Update(GameTime t)
        {
            Uncollide();
        }

        private void Uncollide()
        {
            // take care of the Resources on top not colliding. todo: fixme. @fkarg
        }

        public EPlatformType GetMyType()
        {
            return mType;
        }

        public bool PlatformHasSpace()
        {
            return mResources.Count < 10;
        }

        public void AddEdge(IEdge edge, EEdgeFacing facing)
        {
            if (facing == EEdgeFacing.Inwards)
            {
                mInwardsEdges.Add(edge);
                return;
            }
            mOutwardsEdges.Add(edge);

        }

        public void RemoveEdge(IEdge edge, EEdgeFacing facing)
        {
            if (facing == EEdgeFacing.Inwards)
            {
                mInwardsEdges.Remove(edge);
                return;
            }
            mOutwardsEdges.Remove(edge);

        }

        public IEnumerable<IEdge> GetOutwardsEdges()
        {
            return mOutwardsEdges;
        }

        public IEnumerable<IEdge> GetInwardsEdges()
        {
            return mInwardsEdges;
        }

        public override bool Equals(object other)
        {
            var b = other as PlatformBlank;

            if(b == null)
            {
                return false;
            }

            if(AbsolutePosition != b.AbsolutePosition)
            {
                return false;
            }

            if(AbsoluteSize != b.AbsoluteSize)
            {
                return false;
            }
            if(mType != b.GetMyType())
            {
                return false;
            }
            return true;

        }

        public override int GetHashCode()
        {
            return AbsoluteSize.GetHashCode() * 17 + AbsolutePosition.GetHashCode() + mType.GetHashCode();
        }

        /// <summary>
        /// Sets all the parameters to draw a platfrom properly and calculates the absolute size of a platform.
        /// </summary>
        /// <returns>Absolute Size of a platform</returns>
        protected void SetPlatfromParameters()
        {
            mSheetPosition = 0;
            switch (mType)
            {
                case EPlatformType.Blank:
                    mSheet = 0;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        88);
                    break;
                case EPlatformType.Energy:
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EPlatformType.Factory:
                    mSheetPosition = 1;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EPlatformType.Junkyard:
                    mSheetPosition = 2;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EPlatformType.Mine:
                    mSheetPosition = 3;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EPlatformType.Packaging:
                    mSheetPosition = 4;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EPlatformType.Quarry:
                    mSheetPosition = 5;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EPlatformType.Storage:
                    mSheetPosition = 6;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EPlatformType.Well:
                    mSheetPosition = 7;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EPlatformType.Kinetic:
                    mSheet = 1;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        165);
                    break;
                case EPlatformType.Laser:
                    mSheet = 1;
                    mSheetPosition = 1;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        165);
                    break;
                case EPlatformType.Barracks:
                    mSheet = 2;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        170);
                    break;
                case EPlatformType.Command:
                    mSheet = 2;
                    mSheetPosition = 1;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        170);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (mSheet)
            {
                case 0:
                    // basic platforms
                    AbsoluteSize = new Vector2(148, 85);
                    ColliderGrid = new [,]
                    {
                        { false, true,  true,  true,  true,  true,  true,  false },
                        { true,  true,  true,  true,  true,  true,  true,  true  },
                        { true,  true,  true,  true,  true,  true,  true,  true  },
                        { true,  true,  true,  true,  true,  true,  true,  false },
                        { false, false, true,  true,  true,  true,  false, false },
                        { false, false, false, false, false, false, false, false }
                    };
                    break;
                case (1):
                    // cones
                    AbsoluteSize = new Vector2(148, 165);
                    ColliderGrid = new [,]
                    {
                        { false, false, false, false, false, false, false, false },
                        { false, false, false, false, false, false, false, false },
                        { false, false, false, false, false, false, false, false },
                        { false, false, false, false, false, false, false, false },
                        { false, true,  true,  true,  true,  true,  true,  false },
                        { true,  true,  true,  true,  true,  true,  true,  true  },
                        { true,  true,  true,  true,  true,  true,  true,  true  },
                        { true,  true,  true,  true,  true,  true,  true,  false },
                        { false, false, true,  true,  true,  true,  false, false },
                        { false, false, false, false, false, false, false, false }
                    };
                    break;
                case (2):
                    // cylinders
                    AbsoluteSize = new Vector2(148, 170);
                    ColliderGrid = new [,]
                    {
                        { false, false, false, false, false, false, false, false },
                        { false, false, false, false, false, false, false, false },
                        { false, false, false, false, false, false, false, false },
                        { false, false, false, false, false, false, false, false },
                        { false, true,  true,  true,  true,  true,  true,  false },
                        { true,  true,  true,  true,  true,  true,  true,  true  },
                        { true,  true,  true,  true,  true,  true,  true,  true  },
                        { true,  true,  true,  true,  true,  true,  true,  false },
                        { false, false, true,  true,  true,  true,  false, false },
                        { false, false, false, false, false, false, false, false }
                    };
                    break;
                case (3):
                    // domes
                    AbsoluteSize = new Vector2(148, 126);
                    ColliderGrid = new [,]
                    {
                        { false, false, false, false, false, false, false, false },
                        { false, false, false, false, false, false, false, false },
                        { false, true,  true,  true,  true,  true,  true,  false },
                        { true,  true,  true,  true,  true,  true,  true,  true  },
                        { true,  true,  true,  true,  true,  true,  true,  true  },
                        { true,  true,  true,  true,  true,  true,  true,  false },
                        { false, false, true,  true,  true,  true,  false, false },
                        { false, false, false, false, false, false, false, false }
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Attempted to use a spritesheet "
                        + "for platforms that doesn't exist.");
            }
        }
    }
}
