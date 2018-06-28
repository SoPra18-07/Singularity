using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Exceptions;
using Singularity.Graph;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Platform
{
    [DataContract]
    public class PlatformBlank : IRevealing, INode, ICollider

    {

        private float mLayer;

        private List<IEdge> mInwardsEdges;

        private List<IEdge> mOutwardsEdges;

        [DataMember]
        internal EPlatformType mType;

        [DataMember]
        private const int PlatformWidth = 148;
        [DataMember]
        private const int PlatformHeight = 172;
        [DataMember]
        private int mHealth;
        [DataMember]
        private int mId;
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
        protected Dictionary<JobType, List<Pair<GeneralUnit, bool>>> mAssignedUnits;

        [DataMember]
        protected List<Resource> mResources;
        [DataMember]
        protected Dictionary<EResourceType, int> mRequested;

        public Vector2 Center { get; set; }

        public int RevelationRadius { get; } = 200;

        public Rectangle AbsBounds { get; internal set; }

        public bool Moved { get; private set; }

        public int Id { get; }

        private readonly Director mDirector;

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

        private readonly float mCenterOffsetY;

        public PlatformBlank(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ref Director director, float centerOffsetY = -36)
        {

            Id = IdGenerator.NextiD();

            mDirector = director;

            mCenterOffsetY = centerOffsetY;

            mLayer = LayerConstants.PlatformLayer;

            mType = EPlatformType.Blank;

            AbsoluteSize = SetPlatfromDrawParameters();

            mInwardsEdges = new List<IEdge>();
            mOutwardsEdges = new List<IEdge>();

            AbsolutePosition = position;

            //default?
            mHealth = 100;

            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[1];

            mAssignedUnits = new Dictionary<JobType, List<Pair<GeneralUnit, bool>>>();
            mAssignedUnits.Add(JobType.Idle, new List<Pair<GeneralUnit, bool>>());
            mAssignedUnits.Add(JobType.Defense, new List<Pair<GeneralUnit, bool>>());
            mAssignedUnits.Add(JobType.Production, new List<Pair<GeneralUnit, bool>>());
            mAssignedUnits.Add(JobType.Logistics, new List<Pair<GeneralUnit, bool>>());
            mAssignedUnits.Add(JobType.Construction, new List<Pair<GeneralUnit, bool>>());

            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();

            mResources = new List<Resource>();

            mPlatformSpriteSheet = platformSpriteSheet;
            mPlatformBaseTexture = baseSprite;
            mSpritename = "PlatformBasic";

            mIsBlueprint = true;
            mRequested = new Dictionary<EResourceType, int>();

            AbsBounds = new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int) AbsoluteSize.X, (int) AbsoluteSize.Y);
            Moved = false;

            UpdateValues();

        }

        public void UpdateValues()
        {
            AbsBounds = new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);
            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y + mCenterOffsetY);
        }

        public void Register()
        {
            //TODO: make this so we can also register defense platforms
            mDirector.GetDistributionManager.Register(this, false);
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
            mAssignedUnits[job].Add(new Pair<GeneralUnit, bool>(unit, false));
        }

        /// <summary>
        /// The units will call this methods when they reached the platform they have to work on.
        /// </summary>
        /// <param name="unit"></param>
        public void ShowedUp(GeneralUnit unit, JobType job)
        {
            var pair = mAssignedUnits[job].Find(x => x.GetFirst().Equals(unit));
            if (pair == null)
            {
                throw new InvalidGenericArgumentException("There is no such unit! => Something went wrong...");
            }
            mAssignedUnits[job].Remove(pair);
            mAssignedUnits[job].Add(new Pair<GeneralUnit, bool>(unit, true));
        }

        /// <summary>
        /// Remove an Assigned Unit from the Assigned List.
        /// </summary>
        /// <param name="unit">The unit to unassign.</param>
        /// <param name="job">The Job of the unit</param>
        public void UnAssignUnits(GeneralUnit unit, JobType job)
        {
            var pair = mAssignedUnits[job].Find(x => x.GetFirst().Equals(unit));
            mAssignedUnits[job].Remove(pair);
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
                        mLayer - 0.01f);
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
                        mLayer - 0.01f);
                    // then draw what's on top of that
                    spritebatch.Draw(mPlatformSpriteSheet,
                        AbsolutePosition,
                        new Rectangle(PlatformWidth * mSheetPosition, 0, 148, 148),
                        Color.White * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        mLayer);
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
                        mLayer - 0.01f);
                    // then draw what's on top of that
                    spritebatch.Draw(mPlatformSpriteSheet,
                        AbsolutePosition,
                        new Rectangle(PlatformWidth * mSheetPosition, 0, 148, 153),
                        Color.White * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        mLayer);
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
                        mLayer - 0.01f);
                    // Dome
                    spritebatch.Draw(mPlatformSpriteSheet,
                        AbsolutePosition,
                        new Rectangle(148 * (mSheetPosition % 4), 109 * (int) Math.Floor(mSheetPosition / 4d), 148, 109),
                        Color.White * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        mLayer);
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
        protected Vector2 SetPlatfromDrawParameters()
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
                    return new Vector2(148, 85);
                case 1:
                    // cones
                    return new Vector2(148, 165);
                case 2:
                    // cylinders
                    return new Vector2(148, 170);
                case 3:
                    // domes
                    return new Vector2(148, 126);
                default:
                    return Vector2.Zero;
            }
        }

        public void SetLayer(float layer)
        {
            mLayer = layer;
        }
    }
}
