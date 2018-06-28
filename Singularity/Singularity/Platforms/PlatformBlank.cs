using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph;
using Singularity.Manager;
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

        [DataMember]
        protected Director mDirector;

        public bool[,] ColliderGrid { get; internal set; }

        private readonly float mCenterOffsetY;


        public PlatformBlank(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ref Director director, EPlatformType type = EPlatformType.Blank, float centerOffsetY = -36)
        {

            mDirector = director;

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
            mAssignedUnits.Add(key: JobType.Idle, value: new List<GeneralUnit>());
            mAssignedUnits.Add(key: JobType.Defense, value: new List<GeneralUnit>());
            mAssignedUnits.Add(key: JobType.Production, value: new List<GeneralUnit>());
            mAssignedUnits.Add(key: JobType.Logistics, value: new List<GeneralUnit>());
            mAssignedUnits.Add(key: JobType.Construction, value: new List<GeneralUnit>());

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
            var list = mAssignedUnits[key: job];
            list.Add(item: unit);
        }

        /// <summary>
        /// Remove an Assigned Unit from the Assigned List.
        /// </summary>
        /// <param name="unit">The unit to unassign.</param>
        /// <param name="job">The Job of the unit</param>
        public void UnAssignUnits(GeneralUnit unit, JobType job)
        {
            var list = mAssignedUnits[key: job];
            list.Remove(item: unit);
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
        /// <param name="damage">Negative values for healing, positive for damage</param>
        public void TakeHealDamage(int damage)
        {
            mHealth += damage;
            if (mHealth <= 0)
            {
                //destroyplatform
            }
        }

        /// <summary>
        /// Add a new resource to the platform.
        /// </summary>
        /// <param name="resource"> the resource to be added to the platform </param>
        public void StoreResource(Resource resource)
        {
            mResources.Add(item: resource);
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
            var index = mResources.FindIndex(match: x => x.Type == resourcetype);
            if (index < 0)
            {
                return Optional<Resource>.Of(value: null);
            }

            var foundresource = mResources[index: index];
            mResources.RemoveAt(index: index);
            return Optional<Resource>.Of(value: foundresource);
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
            mRequested.Add(key: resource, value: number);
        }

        /// <inheritdoc cref="Singularity.Property.IDraw"/>
        public void Draw(SpriteBatch spritebatch)
        {
            var transparency = mIsBlueprint ? 0.35f : 1f;

            switch (mSheet)
            {
                case 0:
                    // Basic platform
                    spritebatch.Draw(texture: mPlatformBaseTexture,
                        position: AbsolutePosition,
                        sourceRectangle: null,
                        color: Color.White * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: LayerConstants.BasePlatformLayer);
                    break;
                case 1:
                    // Cone
                    // Draw the basic platform first
                    spritebatch.Draw(texture: mPlatformBaseTexture,
                        position: Vector2.Add(value1: AbsolutePosition, value2: new Vector2(x: -3, y: 73)),
                        sourceRectangle: null,
                        color: Color.White * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: LayerConstants.BasePlatformLayer);
                    // then draw what's on top of that
                    spritebatch.Draw(texture: mPlatformSpriteSheet,
                        position: AbsolutePosition,
                        sourceRectangle: new Rectangle(x: PlatformWidth * mSheetPosition, y: 0, width: 148, height: 148),
                        color: Color.White * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: LayerConstants.PlatformLayer);
                    break;
                case 2:
                    // Cylinder
                    // Draw the basic platform first
                    spritebatch.Draw(texture: mPlatformBaseTexture,
                        position: Vector2.Add(value1: AbsolutePosition, value2: new Vector2(x: -3, y: 82)),
                        sourceRectangle: null,
                        color: Color.White * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: LayerConstants.BasePlatformLayer);
                    // then draw what's on top of that
                    spritebatch.Draw(texture: mPlatformSpriteSheet,
                        position: AbsolutePosition,
                        sourceRectangle: new Rectangle(x: PlatformWidth * mSheetPosition, y: 0, width: 148, height: 153),
                        color: Color.White * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: LayerConstants.PlatformLayer);
                    break;
                case 3:
                    // Draw the basic platform first
                    spritebatch.Draw(texture: mPlatformBaseTexture,
                        position: Vector2.Add(value1: AbsolutePosition, value2: new Vector2(x: -3, y: 38)),
                        sourceRectangle: null,
                        color: Color.White * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: LayerConstants.BasePlatformLayer);
                    // Dome
                    spritebatch.Draw(texture: mPlatformSpriteSheet,
                        position: AbsolutePosition,
                        sourceRectangle: new Rectangle(x: 148 * (mSheetPosition % 4), y: 109 * (int) Math.Floor(d: mSheetPosition / 4d), width: 148, height: 109),
                        color: Color.White * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: LayerConstants.PlatformLayer);
                    break;
            }

            // also draw the resources on top

            foreach (var res in mResources)
            {
                res.Draw(spriteBatch: spritebatch);
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
                mInwardsEdges.Add(item: edge);
                return;
            }
            mOutwardsEdges.Add(item: edge);

        }

        public void RemoveEdge(IEdge edge, EEdgeFacing facing)
        {
            if (facing == EEdgeFacing.Inwards)
            {
                mInwardsEdges.Remove(item: edge);
                return;
            }
            mOutwardsEdges.Remove(item: edge);

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
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 88);
                    break;
                case EPlatformType.Energy:
                    mSheet = 3;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 127);
                    break;
                case EPlatformType.Factory:
                    mSheetPosition = 1;
                    mSheet = 3;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 127);
                    break;
                case EPlatformType.Junkyard:
                    mSheetPosition = 2;
                    mSheet = 3;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 127);
                    break;
                case EPlatformType.Mine:
                    mSheetPosition = 3;
                    mSheet = 3;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 127);
                    break;
                case EPlatformType.Packaging:
                    mSheetPosition = 4;
                    mSheet = 3;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 127);
                    break;
                case EPlatformType.Quarry:
                    mSheetPosition = 5;
                    mSheet = 3;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 127);
                    break;
                case EPlatformType.Storage:
                    mSheetPosition = 6;
                    mSheet = 3;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 127);
                    break;
                case EPlatformType.Well:
                    mSheetPosition = 7;
                    mSheet = 3;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 127);
                    break;
                case EPlatformType.Kinetic:
                    mSheet = 1;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 165);
                    break;
                case EPlatformType.Laser:
                    mSheet = 1;
                    mSheetPosition = 1;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 165);
                    break;
                case EPlatformType.Barracks:
                    mSheet = 2;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 170);
                    break;
                case EPlatformType.Command:
                    mSheet = 2;
                    mSheetPosition = 1;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 170);
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
