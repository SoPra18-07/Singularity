using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Exceptions;
using Singularity.Graph;
using Singularity.Manager;
using Singularity.PlatformActions;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Screen;
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

        private int mGraphIndex;

        private float mLayer;

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
        protected List<IPlatformAction> mIPlatformActions;
        private readonly Texture2D mPlatformSpriteSheet;
        private readonly Texture2D mPlatformBaseTexture;
        [DataMember]
        protected string mSpritename;
        [DataMember]
        protected Dictionary<JobType, List<Pair<GeneralUnit, bool>>> mAssignedUnits;

        [DataMember]
        protected List<Resource> mResources;
        [DataMember]
        protected Dictionary<EResourceType, int> mRequested;

        public Vector2 Center { get; private set; }

        public int RevelationRadius { get; } = 200;

        public Rectangle AbsBounds { get; private set; }

        public bool Moved { get; private set; }

        public int Id { get; }
        
        [DataMember]
        protected Director mDirector;

        // the sprite sheet that should be used. 0 for basic, 1 for cone, 2 for cylinder, 3 for dome
        private int mSheet;
        private int mSheetPosition;


        [DataMember]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember]
        public Vector2 AbsoluteSize { get; set; }

        [DataMember]
        public Vector2 RelativePosition { get; set; }
        [DataMember]
        public Vector2 RelativeSize { get; set; }

        [DataMember]
        private readonly float mCenterOffsetY;

        private Color mColor;

        private PlatformInfoBox mInfoBox;

        public SpriteFont mLibSans12;

        public bool[,] ColliderGrid { get; private set; }

        //This is for registering the platform at the DistrManager.
        [DataMember]
        public JobType Property { get; set; }

        public PlatformBlank(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, SpriteFont libSans12Font, ref Director director, EPlatformType type = EPlatformType.Blank, float centerOffsetY = -36)
        {

            Id = IdGenerator.NextiD();

            mColor = Color.White;

            mDirector = director;

            mCenterOffsetY = centerOffsetY;

            mLayer = LayerConstants.PlatformLayer;

            mType = type;

            mInwardsEdges = new List<IEdge>();
            mOutwardsEdges = new List<IEdge>();

            AbsolutePosition = position;
            
            mLibSans12 = libSans12Font;

            SetPlatfromParameters(); // this changes the draw parameters based on the platform type but
            // also sets the AbsoluteSize and collider grids

            //default?
            mHealth = 100;

            //I dont think this class has to register in the DistributionManager
            //Add possible Actions in this array
            mIPlatformActions = new List<IPlatformAction>();

            mAssignedUnits = new Dictionary<JobType, List<Pair<GeneralUnit, bool>>>
            {
                {JobType.Idle, new List<Pair<GeneralUnit, bool>>()},
                {JobType.Defense, new List<Pair<GeneralUnit, bool>>()},
                {JobType.Production, new List<Pair<GeneralUnit, bool>>()},
                {JobType.Logistics, new List<Pair<GeneralUnit, bool>>()},
                {JobType.Construction, new List<Pair<GeneralUnit, bool>>()}
            };

            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int> { {EResourceType.Metal, 2} };

            mResources = new List<Resource>();

            mPlatformSpriteSheet = platformSpriteSheet;
            mPlatformBaseTexture = baseSprite;
            mSpritename = "PlatformBasic";

            mIsBlueprint = true;
            mRequested = new Dictionary<EResourceType, int>();

            Moved = false;
            UpdateValues();

            Debug.WriteLine(message: "PlatformBlank created");

            var str = GetResourceString();
            mInfoBox = new PlatformInfoBox(
                itemList: new List<IWindowItem>
                {
                    new TextField(text: str, position: AbsolutePosition + new Vector2(x: 0, y: AbsoluteSize.Y + 10), size: mLibSans12.MeasureString(text: str), spriteFont: mLibSans12)
                }, 
                size: mLibSans12.MeasureString(str),
                platform: this, director: mDirector);

            /*
            var infoBuildBlank = new TextField("Blank Platform",
                Vector2.Zero,
                mLibSans12.MeasureString("Blank Platform"),
                mLibSans12);

            mInfoBuildBlank = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoBuildBlank },
                size: mLibSans12.MeasureString("Blank Platform"),
                borderColor: new Color(0.86f, 0.85f, 0.86f),
                centerColor: new Color(1f, 1f, 1f),//(0.75f, 0.75f, 0.75f),
                boundsRectangle: new Rectangle(
                    (int)mBlankPlatformButton.Position.X,
                    (int)mBlankPlatformButton.Position.Y,
                    (int)mBlankPlatformButton.Size.X,
                    (int)mBlankPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);
            // */

        }

        internal void AddBlueprint(BuildBluePrint buildBluePrint)
        {
            mIPlatformActions.Add(buildBluePrint);
        }

        public void SetColor(Color color)
        {
            mColor = color;
        }

        public void ResetColor()
        {
            mColor = Color.White;
        }

        public void UpdateValues()
        {
            AbsBounds = new Rectangle(x: (int)AbsolutePosition.X, y: (int)AbsolutePosition.Y, width: (int)AbsoluteSize.X, height: (int)AbsoluteSize.Y);
            Center = new Vector2(x: AbsolutePosition.X + AbsoluteSize.X / 2, y: AbsolutePosition.Y + AbsoluteSize.Y + mCenterOffsetY);
        }

        public void Register()
        {
            //TODO: make this so we can also register defense platforms
            if (Property == JobType.Production)
            {
                mDirector.GetDistributionManager.Register(platform: this, isDef: false);
            } else if (Property == JobType.Defense)
            {
                mDirector.GetDistributionManager.Register(platform: this, isDef: true);
            }
        }

        /// <summary>
        /// Get the assigned Units of this platform.
        /// </summary>
        /// <returns> a Dictionary, under each JobType there is an entry with a list containing all assigned units plus a bool to show wether they are present on the platform</returns>
        public Dictionary<JobType, List<Pair<GeneralUnit, bool>>> GetAssignedUnits()
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
            mAssignedUnits[key: job].Add(item: new Pair<GeneralUnit, bool>(firstValue: unit, secondValue: false));
        }

        /// <summary>
        /// The units will call this methods when they reached the platform they have to work on.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="job"></param>
        public void ShowedUp(GeneralUnit unit, JobType job)
        {
            var pair = mAssignedUnits[key: job].Find(match: x => x.GetFirst().Equals(obj: unit));
            if (pair == null)
            {
                throw new InvalidGenericArgumentException(message: "There is no such unit! => Something went wrong...");
            }
            mAssignedUnits[key: job].Remove(item: pair);
            mAssignedUnits[key: job].Add(item: new Pair<GeneralUnit, bool>(firstValue: unit, secondValue: true));
        }

        /// <summary>
        /// Remove an Assigned Unit from the Assigned List.
        /// </summary>
        /// <param name="unit">The unit to unassign.</param>
        /// <param name="job">The Job of the unit</param>
        public void UnAssignUnits(GeneralUnit unit, JobType job)
        {
            var pair = mAssignedUnits[key: job].Find(match: x => x.GetFirst().Equals(obj: unit));
            mAssignedUnits[key: job].Remove(item: pair);
        }

        public virtual void Produce()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get the special IPlatformActions you can perform on this platform.
        /// </summary>
        /// <returns> an array with the available IPlatformActions.</returns>
        public List<IPlatformAction> GetIPlatformActions()
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
                if (mType == EPlatformType.Blank)
                {
                    Die();
                }
                else
                {
                    DieBlank();
                }
            }
        }

        /// <summary>
        /// Add a new resource to the platform.
        /// </summary>
        /// <param name="resource"> the resource to be added to the platform </param>
        public void StoreResource(Resource resource)
        {
            mResources.Add(item: resource);
            mResources = mResources.OrderBy(r => r.Type).ToList();
            Uncollide();
        }

        /// <summary>
        /// Use this method to get the resource you asked for. Removes the resource from the platform.
        /// </summary>
        /// <param name="resourcetype">The resource you ask for</param>
        /// <returns>the resource you asked for, null otherwise.</returns>
        public Optional<Resource> GetResource(EResourceType resourcetype)
        {
            // TODO: reservation of Resources (and stuff)? Nah lets not do this
            var index = mResources.FindIndex(match: x => x.Type == resourcetype); // (FindIndex returns -1 if not found)
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
            return mRequested; // todo: change to sum of Requested Resources from PlatformActions. (there's no other required resources after all)
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
                        color: mColor * transparency,
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
                        color: mColor * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: LayerConstants.BasePlatformLayer);
                    // then draw what's on top of that
                    spritebatch.Draw(texture: mPlatformSpriteSheet,
                        position: AbsolutePosition,
                        sourceRectangle: new Rectangle(x: PlatformWidth * mSheetPosition, y: 0, width: 148, height: 148),
                        color: mColor * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: mLayer);
                    break;
                case 2:
                    // Cylinder
                    // Draw the basic platform first
                    spritebatch.Draw(texture: mPlatformBaseTexture,
                        position: Vector2.Add(value1: AbsolutePosition, value2: new Vector2(x: 0, y: 81)),
                        sourceRectangle: null,
                        color: mColor * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: LayerConstants.BasePlatformLayer);
                    // then draw what's on top of that
                    spritebatch.Draw(texture: mPlatformSpriteSheet,
                        position: AbsolutePosition,
                        sourceRectangle: new Rectangle(x: PlatformWidth * mSheetPosition, y: 0, width: 148, height: 153),
                        color: mColor * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: mLayer);
                    break;
                case 3:
                    // Draw the basic platform first
                    spritebatch.Draw(texture: mPlatformBaseTexture,
                        position: Vector2.Add(value1: AbsolutePosition, value2: new Vector2(x: -3, y: 38)),
                        sourceRectangle: null,
                        color: mColor * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: LayerConstants.BasePlatformLayer);
                    // Dome
                    spritebatch.Draw(texture: mPlatformSpriteSheet,
                        position: AbsolutePosition,
                        sourceRectangle: new Rectangle(x: 148 * (mSheetPosition % 4), y: 109 * (mSheetPosition / 4), width: 148, height: 109),
                        color: mColor * transparency,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: mLayer);
                    break;
            }

            mInfoBox.UpdateString(GetResourceString());
            mInfoBox.Draw(spriteBatch: spritebatch);

            // also draw the resources on top
            /*
            foreach (var res in mResources)
            {
                res.Draw(spriteBatch: spritebatch);
            }
            // */
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

        private new EPlatformType GetType()
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

        public void RemoveEdge(IEdge edge)
        {
            if (mInwardsEdges.Contains(item: edge))
            {
                mInwardsEdges.Remove(item: edge);
            }

            if (mOutwardsEdges.Contains(item: edge))
            {
                mOutwardsEdges.Remove(item: edge);
            }

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
            return mType == b.GetType();
        }

        [SuppressMessage(category: "ReSharper", checkId: "NonReadonlyMemberInGetHashCode")]
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
                    mSheetPosition = 1;
                    AbsBounds = new Rectangle(x: (int)AbsolutePosition.X,
                        y: (int)AbsolutePosition.Y,
                        width: PlatformWidth,
                        height: 170);
                    break;
                case EPlatformType.Command:
                    mSheet = 2;
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
                    AbsoluteSize = new Vector2(x: 148, y: 85);
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
                case 1:
                    // cones
                    AbsoluteSize = new Vector2(x: 148, y: 165);
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
                case 2:
                    // cylinders
                    AbsoluteSize = new Vector2(x: 148, y: 170);
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
                case 3:
                    // domes
                    AbsoluteSize = new Vector2(x: 148, y: 126);
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
                    throw new ArgumentOutOfRangeException(paramName: "Attempted to use a spritesheet "
                        + "for platforms that doesn't exist.");
            }
        }

        public void SetLayer(float layer)
        {
            mLayer = layer;
        }

        /// <summary>
        /// This will kill only the specialised part of the platform.
        /// </summary>
        public void DieBlank()
        {

            mDirector.GetDistributionManager.Kill(platform: this);


            mColor = Color.White;
            mType = EPlatformType.Blank;
            mSpritename = "PlatformBasic";
            SetPlatfromParameters();

            //default?
            mHealth = 100;

            mIPlatformActions.RemoveAll(match: a => a.Die());

            mAssignedUnits[key: JobType.Idle].RemoveAll(match: p => p.GetSecond() && p.GetFirst().Die());
            mAssignedUnits[key: JobType.Defense].RemoveAll(match: p => p.GetSecond() && p.GetFirst().Die());
            mAssignedUnits[key: JobType.Construction].RemoveAll(match: p => p.GetSecond() && p.GetFirst().Die());
            mAssignedUnits[key: JobType.Logistics].RemoveAll(match: p => p.GetSecond() && p.GetFirst().Die());
            mAssignedUnits[key: JobType.Production].RemoveAll(match: p => p.GetSecond() && p.GetFirst().Die());


            mResources.RemoveAll(match: r => r.Die());
            mResources = new List<Resource> {new Resource(type: EResourceType.Trash, position: Center), new Resource(type: EResourceType.Trash, position: Center),
                new Resource(type: EResourceType.Trash, position: Center), new Resource(type: EResourceType.Trash, position: Center), new Resource(type: EResourceType.Trash, position: Center)};

            mRequested = new Dictionary<EResourceType, int>();

            Moved = false;
            UpdateValues();
        }

        /// <summary>
        /// This will kill the platform for good.
        /// </summary>
        public bool Die()
        {

            DieBlank();

            // TODO: REMOVE from everywhere.
            // see https://github.com/SoPra18-07/Singularity/issues/215

            // removing the PlatformActions first

            mInwardsEdges.RemoveAll(match: e => ((Road) e).Die());
            mOutwardsEdges.RemoveAll(match: e => ((Road) e).Die()); // this is indirectly calling the Kill(road) function below


            mResources.RemoveAll(match: r => r.Die());

            mIPlatformActions.ForEach(action: a => a.Platform = null);
            mIPlatformActions.RemoveAll(match: a => a.Die());
            mDirector.GetDistributionManager.Kill(platform: this);
            mDirector.GetStoryManager.StructureMap.RemovePlatform(platform: this);
            mDirector.GetStoryManager.Level.GameScreen.RemoveObject(toRemove: this);
            return true;
        }

        public void Kill(IEdge road)
        {
            mInwardsEdges.Remove(item: road);
            mOutwardsEdges.Remove(item: road);
            mDirector.GetStoryManager.StructureMap.RemoveRoad(road: (Road) road);
            mDirector.GetStoryManager.Level.GameScreen.RemoveObject(toRemove: road);
        }

        public void Kill(IPlatformAction action)
        {
            mIPlatformActions.Remove(action);
        }

        public IEnumerable<INode> GetChilds()
        {
            var childs = new List<INode>();

            foreach (var outgoing in GetOutwardsEdges())
            {
                childs.Add(item: outgoing.GetChild());
            }

            foreach (var ingoing in GetInwardsEdges())
            {
                childs.Add(item: ingoing.GetParent());
            }
            return childs;
        }

        public void SetGraphIndex(int graphIndex)
        {
            mGraphIndex = graphIndex;
        }

        public int GetGraphIndex()
        {
            return mGraphIndex;
        }

        public string GetResourceString()
        {
            if (mResources.Count == 0)
            {
                return "";
            }
            var resString = "";
            var cType = (EResourceType) 0;
            var counter = 0;
            foreach (var res in mResources)
            {
                if (counter > 0 && res.Type != cType)
                {
                    resString += cType + ": " + counter + ", ";
                    counter = 0;
                }
                cType = res.Type;
                counter++;
            }
            return resString + cType + ": " + counter;
        }

        public void Built()
        {
            mIsBlueprint = false;
        }
    }
}
