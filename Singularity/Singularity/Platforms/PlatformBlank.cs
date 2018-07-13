﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Exceptions;
using Singularity.Graph;
using Singularity.Input;
using Singularity.Manager;
using Singularity.PlatformActions;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Screen;
using Singularity.Sound;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Platforms
{
    /// <inheritdoc cref="IRevealing"/>
    /// <inheritdoc cref="INode"/>
    /// <inheritdoc cref="ICollider"/>
    /// <inheritdoc cref="IDamageable"/>
    [DataContract]
    public class PlatformBlank : IRevealing, INode, ICollider, IMouseClickListener
    {

        [DataMember]
        private List<GeneralUnit> mAllGenUnits;

        [DataMember]
        private int mGraphIndex;
        [DataMember]
        private float mLayer;

        // true, if this platform has already sent data since activation
        [DataMember]
        private bool mDataSent;

        // determines whether the platform has already been added to the inputManager
        [DataMember]
        private bool mAddedToInputManager;

        // previous values sent to the UIController - used to only send data if the values have been updated
        [DataMember]
        private List<Resource> mPrevResources;
        [DataMember]
        private Dictionary<JobType, List<Pair<GeneralUnit, bool>>> mPrevUnitAssignments;
        [DataMember]
        private List<IPlatformAction> mPrevPlatformActions;
        [DataMember]
        private bool mPreviousIsActiveState;
        [DataMember]
        private bool mPreviousIsManuallyDeactivatedState;

        /// <summary>
        /// true, if the platform is sleected in the UI
        /// </summary>
        [DataMember]
        public bool IsSelected { get; set; }
        [DataMember]
        public EScreen Screen { get; private set; } = EScreen.GameScreen;
        [DataMember]
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// List of inwards facing edges/roads towards the platform.
        /// </summary>
        [DataMember]
        private List<IEdge> mInwardsEdges;

        /// <summary>
        /// List of outwards facing edges/roads.
        /// </summary>
        [DataMember]
        private List<IEdge> mOutwardsEdges;
        [DataMember]
        public bool Friendly { get; set; }

        /// <summary>
        /// Indicates the type of platform this is, defaults to blank.
        /// </summary>
        [DataMember]
        internal EStructureType mType;

        /// <summary>
        /// Indicates the platform width
        /// </summary>
        [DataMember]
        protected const int PlatformWidth = 148;

        /// <summary>
        /// Indicates the platform height.
        /// </summary>
        [DataMember]
        protected const int PlatformHeight = 172;

        /// <summary>
        /// How much health the platform has
        /// </summary>
        [DataMember]
        public int Health { get; private set; }

        /// <summary>
        /// Indicates if the platform is a "real" platform or a blueprint.
        /// </summary>
        [DataMember]
        protected bool mIsBlueprint;

        [DataMember]
        protected int mProvidingEnergy;

        [DataMember]
        protected int mDrainingEnergy;


        [DataMember]
        protected Dictionary<EResourceType, int> mCost;
        [DataMember]
        protected List<IPlatformAction> mIPlatformActions;

        protected Texture2D mPlatformSpriteSheet;
        protected Texture2D mPlatformBaseTexture;

        //This means the platformspritesheetname not the name of the base texture
        [DataMember]
        protected string mSpritename;

        [DataMember]
        protected Dictionary<JobType, List<Pair<GeneralUnit, bool>>> mAssignedUnits;

        [DataMember]
        protected List<Resource> mResources;
        [DataMember]
        protected Dictionary<EResourceType, int> mRequested;

        [DataMember]
        private bool mIsActive;

        [DataMember]
        private bool mIsManuallyDeactivated;

        [DataMember]
        public Vector2 Center { get; set; }

        [DataMember]
        public int RevelationRadius { get; protected set; } = 200;
        [DataMember]
        public Rectangle AbsBounds { get; internal set; }
        [DataMember]
        public bool Moved { get; private set; }

        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        protected Director mDirector;

        ///<summary>
        /// The sprite sheet that should be used. 0 for basic, 1 for cone, 2 for cylinder, 3 for dome.
        /// </summary>
        [DataMember]
        protected int mSheet;

        /// <summary>
        /// Where on the spritesheet the platform is located
        /// </summary>
        [DataMember]
        protected int mSheetPosition;


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
        [DataMember]
        protected Color mColor = Color.White;
        [DataMember]
        protected Color mColorBase;

        protected PlatformInfoBox mInfoBox;

        public static SpriteFont mLibSans12;

        public bool[,] ColliderGrid { get; internal set; }

        //This is for registering the platform at the DistrManager.
        [DataMember]
        public JobType Property { get; set; }

        public PlatformBlank(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, SpriteFont libsans12, ref Director director, EStructureType type = EStructureType.Blank, float centerOffsetY = -36, bool friendly = true)
        {

            mPrevPlatformActions = new List<IPlatformAction>();

            Id = director.GetIdGenerator.NextiD();

            mDirector = director;

            mCenterOffsetY = centerOffsetY;

            mLayer = LayerConstants.PlatformLayer;

            mType = type;

            mColorBase = friendly ? Color.White : Color.Red;

            mInwardsEdges = new List<IEdge>();
            mOutwardsEdges = new List<IEdge>();

            AbsolutePosition = position;

            SetPlatfromParameters(); // this changes the draw parameters based on the platform type but
            // also sets the AbsoluteSize and collider grids

            //default?
            Health = 100;

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
            mCost = new Dictionary<EResourceType, int>();

            mProvidingEnergy = 0;
            mDrainingEnergy = 0;
            mIsActive = true;

            mAllGenUnits = new List<GeneralUnit>();
            mResources = new List<Resource>();

            mPlatformSpriteSheet = platformSpriteSheet;
            mPlatformBaseTexture = baseSprite;
            mSpritename = "PlatformBasic";
            mLibSans12 = libsans12;

            mIsBlueprint = true;
            mRequested = new Dictionary<EResourceType, int>();

            Moved = false;
            UpdateValues();

            Debug.WriteLine("PlatformBlank created");
            
            Friendly = friendly;
            var str = GetResourceString();
            mInfoBox = new PlatformInfoBox(
                itemList: new List<IWindowItem>
                {
                    new TextField(text: str, position: AbsolutePosition + new Vector2(x: 0, y: AbsoluteSize.Y + 10), size: mLibSans12.MeasureString(text: str), spriteFont: mLibSans12, color: Color.White)
                },
                size: mLibSans12.MeasureString(str),
                platform: this, director: mDirector);
                // mInfoBox = new PlatformInfoBox(new List<IWindowItem> { new TextField("PlattformInfo", AbsolutePosition, AbsoluteSize, mLibSans12, Color.White) }, AbsoluteSize, new Color(0.86f, 0.86f, 0.86f), new Color(1f, 1, 1), true, this, mDirector);

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

        internal void ReloadContent(ContentManager content, ref Director dir)
        {
            mPlatformSpriteSheet = content.Load<Texture2D>(mSpritename);
            mPlatformBaseTexture = content.Load<Texture2D>("PlatformBasic");
            mDirector = dir;
            mDirector.GetInputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);
            mAddedToInputManager = true;
            foreach (var action in mIPlatformActions)
            {
                action.ReloadContent(ref dir);
            }

            foreach (var action in mPrevPlatformActions)
            {
                action.ReloadContent(ref dir);
            }
            var str = GetResourceString();
            mInfoBox = new PlatformInfoBox(
                itemList: new List<IWindowItem>
                {
                    new TextField(text: str, position: AbsolutePosition + new Vector2(x: 0, y: AbsoluteSize.Y + 10), size: mLibSans12.MeasureString(text: str), spriteFont: mLibSans12, color: Color.White)
                },
                size: mLibSans12.MeasureString(str),
                platform: this, director: mDirector);
            SetPlatfromParameters();
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
            AbsBounds = new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);
            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y + mCenterOffsetY);
        }

        public void Register()
        {
            if (IsProduction())
            {
                mDirector.GetDistributionDirector.GetManager(GetGraphIndex()).Register(this, false);
            } else if (IsDefense())
            {
                mDirector.GetDistributionDirector.GetManager(GetGraphIndex()).Register(this, true);
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
        internal List<Resource> GetPlatformResources()
        {
            return mResources;
        }

        public void MakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                if (mType == EStructureType.Blank)
                {
                    Die();
                }
                else
                {
                    // makes destruction sound
                    mDirector.GetSoundManager.PlaySound("DestroyPlat", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
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
            mResources.Add(resource);
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
            var index = mResources.FindIndex(x => x.Type == resourcetype); // (FindIndex returns -1 if not found)
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
            return mRequested; // todo: change to sum of Requested Resources from PlatformActions. (there's no other required resources after all)
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
        public virtual void Draw(SpriteBatch spritebatch)
        {
            var transparency = mIsBlueprint ? 0.35f : 1f;

            switch (mSheet)
            {
                case 0:
                    // Basic platform
                    spritebatch.Draw(mPlatformBaseTexture,
                        AbsolutePosition,
                        null,
                        mColor * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        LayerConstants.BasePlatformLayer);
                    break;
                case 1:
                    break;
                case 2:
                    // Cylinder (Unit Platforms
                    // Draw the basic platform first
                    spritebatch.Draw(mPlatformBaseTexture,
                        Vector2.Add(AbsolutePosition, new Vector2(0, 81)),
                        null,
                        mColorBase * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        LayerConstants.BasePlatformLayer);
                    // then draw what's on top of that
                    spritebatch.Draw(mPlatformSpriteSheet,
                        AbsolutePosition,
                        new Rectangle(PlatformWidth * mSheetPosition, 0, 148, 153),
                        mColor * transparency,
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
                        mColorBase * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        LayerConstants.BasePlatformLayer);
                    // Dome
                    spritebatch.Draw(mPlatformSpriteSheet,
                        AbsolutePosition,
                        new Rectangle(148 * (mSheetPosition % 4), 109 * (mSheetPosition / 4), 148, 109),
                        mColor * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        mLayer);
                    break;
            }

            mInfoBox.UpdateString(GetResourceString());
            mInfoBox.Draw(spritebatch);

            // also draw the resources on top
            /*
            foreach (var res in mResources)
            {
                res.Draw(spritebatch);
            }
            // */
        }

        /// <inheritdoc cref="Singularity.Property.IUpdate"/>
        public virtual void Update(GameTime t)
        {
            Uncollide();

            Bounds = new Rectangle((int)RelativePosition.X, (int)RelativePosition.Y, (int)RelativeSize.X, (int)RelativeSize.Y);

            if (!mAddedToInputManager)
            {
                // add this platform to inputManager once
                mDirector.GetInputManager
                    .AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);
                mAddedToInputManager = true;

                mDirector.GetEventLog.AddEvent(ELogEventType.PlatformBuilt, mType + " has been built", this);
            }

            // set the mDataSent bool to false if there was a change in platform infos since the data was sent last time
            // or if the platform is not selected, so that if it gets selected it will send the current data to the UIController
            if (mPrevResources != GetPlatformResources() ||
                mPrevUnitAssignments != GetAssignedUnits() ||
                mPrevPlatformActions != GetIPlatformActions() ||
                mPreviousIsActiveState != IsActive() ||
                mPreviousIsManuallyDeactivatedState != IsManuallyDeactivated() ||
                !IsSelected)
            {
                mDataSent = false;
            }

            // manage updating of values in the UI
            if (IsSelected && !mDataSent)
                // the platform is selected + the current data has yet to be sent then
            {
                // update previous values
                mPrevResources = GetPlatformResources();
                mPrevUnitAssignments = GetAssignedUnits();
                mPrevPlatformActions = GetIPlatformActions();
                mPreviousIsManuallyDeactivatedState = IsManuallyDeactivated();
                mPreviousIsActiveState = IsActive();

                // send data to UIController
                mDirector.GetUserInterfaceController.SetDataOfSelectedPlatform(Id, mIsActive, mIsManuallyDeactivated, mType, GetPlatformResources(), GetAssignedUnits(), GetIPlatformActions());

                // set the bool for sent-data to true, since the data has just been sent
                mDataSent = true;
            }
        }

        private void Uncollide()
        {
            // take care of the Resources on top not colliding. todo: fixme. @fkarg
        }

        public EStructureType GetMyType()
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

        public void RemoveEdge(IEdge edge)
        {
            if (mInwardsEdges.Contains(edge))
            {
                mInwardsEdges.Remove(edge);
            }

            if (mOutwardsEdges.Contains(edge))
            {
                mOutwardsEdges.Remove(edge);
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
            return mType == b.GetMyType();
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return AbsoluteSize.GetHashCode() * 17 + AbsolutePosition.GetHashCode() + mType.GetHashCode();
        }


        /// <summary>
        /// Sets all the parameters to draw a platform properly and calculates the absolute size of a platform.
        /// </summary>
        /// <returns>Absolute Size of a platform</returns>
        protected void SetPlatfromParameters()
        {
            mSheetPosition = 0;
            switch (mType)
            {
                case EStructureType.Blank:
                    mSheet = 0;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        88);
                    break;
                case EStructureType.Energy:
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EStructureType.Factory:
                    mSheetPosition = 1;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EStructureType.Junkyard:
                    mSheetPosition = 2;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EStructureType.Mine:
                    mSheetPosition = 3;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EStructureType.Packaging:
                    mSheetPosition = 4;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EStructureType.Quarry:
                    mSheetPosition = 5;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EStructureType.Storage:
                    mSheetPosition = 6;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EStructureType.Well:
                    mSheetPosition = 7;
                    mSheet = 3;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        127);
                    break;
                case EStructureType.Kinetic:
                    mSheet = 1;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        165);
                    break;
                case EStructureType.Laser:
                    mSheet = 1;
                    mSheetPosition = 1;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        165);
                    break;
                case EStructureType.Barracks:
                    mSheet = 2;
                    mSheetPosition = 1;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        170);
                    break;
                case EStructureType.Command:
                    mSheet = 2;
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
                case 1:
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
                case 2:
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
                case 3:
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

        public void SetLayer(float layer)
        {
            mLayer = layer;
        }

        /// <summary>
        /// This will kill only the specialised part of the platform.
        /// </summary>
        public void DieBlank()
        {

            mDirector.GetDistributionDirector.GetManager(GetGraphIndex()).Kill(this);
            mType = EPlatformType.Blank;

            // create the event in eventLog that the specialised part has been destroyed
            mDirector.GetEventLog.AddEvent(ELogEventType.PlatformDestroyed, mType + " has been destroyed", this);
            // if platform was an enemy keep red base
            mColor = Friendly ? Color.White : Color.Red;

            mColor = Color.White;
            mType = EStructureType.Blank;
            mSpritename = "PlatformBasic";
            SetPlatfromParameters();

            // position of blank needs to be adjusted to fit previous location of specialized plat base
            AbsolutePosition = new Vector2(AbsolutePosition.X, AbsolutePosition.Y + 81);

            //default?
            Health = 100;

            mIPlatformActions.RemoveAll(a => a.Die());

            mAssignedUnits[JobType.Idle].RemoveAll(p => p.GetSecond() && p.GetFirst().Die());
            mAssignedUnits[JobType.Defense].RemoveAll(p => p.GetSecond() && p.GetFirst().Die());
            mAssignedUnits[JobType.Construction].RemoveAll(p => p.GetSecond() && p.GetFirst().Die());
            mAssignedUnits[JobType.Logistics].RemoveAll(p => p.GetSecond() && p.GetFirst().Die());
            mAssignedUnits[JobType.Production].RemoveAll(p => p.GetSecond() && p.GetFirst().Die());


            mResources.RemoveAll(r => r.Die());
            mResources = new List<Resource> {new Resource(EResourceType.Trash, Center), new Resource(EResourceType.Trash, Center),
                new Resource(EResourceType.Trash, Center), new Resource(EResourceType.Trash, Center), new Resource(EResourceType.Trash, Center)};

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

            // create event in eventLog that the platform has been destroyed
            mDirector.GetEventLog.AddEvent(ELogEventType.PlatformDestroyed, mType + " has been destroyed", this);

            // TODO: REMOVE from everywhere.
            // see https://github.com/SoPra18-07/Singularity/issues/215

            // removing the PlatformActions first

            var toKill = new List<IEdge>();

            foreach (var road in mInwardsEdges)
            {
                toKill.Add(road);
            }

            foreach (var road in mOutwardsEdges)
            {
                toKill.Add(road);
            }

            foreach (var road in toKill)
            {
                ((Road)road).Die();
            }

            toKill.Clear();
            mInwardsEdges.Clear();
            mOutwardsEdges.Clear();
            ;



            mResources.RemoveAll(r => r.Die());

            mIPlatformActions.ForEach(a => a.Platform = null);
            mIPlatformActions.RemoveAll(a => a.Die());
            mDirector.GetStoryManager.Level.GameScreen.RemoveObject(this);
            return true;
        }

        public IEnumerable<INode> GetChilds()
        {
            var childs = new List<INode>();

            foreach (var outgoing in GetOutwardsEdges())
            {
                childs.Add(outgoing.GetChild());
            }

            foreach (var ingoing in GetInwardsEdges())
            {
                childs.Add(ingoing.GetParent());
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

        public void Activate(bool manually)
        {
            //TODO: Tell the PlatformAction to request everything it needs again.
            if (manually)
            {
                // TODO find a power on sound
                mDirector.GetSoundManager.PlaySound("PowerOff",
                    Center.X,
                    Center.Y,
                    .1f,
                    .01f,
                    true,
                    false,
                    SoundClass.Effect);
                mIsManuallyDeactivated = false;
            }
            mIsActive = true;
            ResetColor();
            //Only reregister the platforms if they are defense or production platforms
            if (IsDefense())
            {
                mDirector.GetDistributionDirector.GetManager(GetGraphIndex()).Register(this, true);
            }else if (IsProduction())
            {
                mDirector.GetDistributionDirector.GetManager(GetGraphIndex()).Register(this, false);
            }
        }

        /// <summary>
        /// A function made to determine whether this platform is a defending platform.
        /// </summary>
        /// <returns>True if thats the case, false otherwise</returns>
        public bool IsDefense()
        {
            if (mType == EStructureType.Kinetic
                || mType == EStructureType.Laser)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// A function made to determine whether this paltform is a producing platform.
        /// </summary>
        /// <returns>True if thats the casem, false otherwise</returns>
        public bool IsProduction()
        {
            if (mType == EStructureType.Well
                || mType == EStructureType.Quarry
                || mType == EStructureType.Mine
                || mType == EStructureType.Factory)
            {
                return true;
            }

            return false;
        }

        public void Deactivate(bool manually)
        {
            if (manually)
            {
                // TODO maybe need to regulate sound a little when put to action
                mDirector.GetSoundManager.PlaySound("PowerDown",
                    Center.X,
                    Center.Y,
                    .1f,
                    .01f,
                    true,
                    false,
                    SoundClass.Effect);

                mIsManuallyDeactivated = true;
            }

            mIsActive = false;
            // TODO: remove this or change it to something more appropriately, this is used by @Ativelox for
            // TODO: debugging purposes to easily see which platforms are currently deactivated
            mColor = Color.Green;
            //Only unregister if this platform is a defense or production platform
            if (IsDefense())
            {
                var selflist = new List<PlatformBlank>();
                selflist.Add(this);
                mDirector.GetDistributionDirector.GetManager(GetGraphIndex()).Unregister(selflist, true, true);
            }
            else if (IsProduction())
            {
                var selflist = new List<PlatformBlank>();
                selflist.Add(this);
                mDirector.GetDistributionDirector.GetManager(GetGraphIndex()).Unregister(selflist, false, true);
            }
        }

        public List<GeneralUnit> GetGeneralUnitsOnPlatform()
        {
            return mAllGenUnits;
        }

        public void AddGeneralUnit(GeneralUnit unit)
        {
            mAllGenUnits.Add(unit);
        }

        public void RemoveGeneralUnit(GeneralUnit unit)
        {
            mAllGenUnits.Remove(unit);
        }

        public bool IsManuallyDeactivated()
        {
            return mIsManuallyDeactivated;
        }

        public int GetProvidingEnergy()
        {
            return mProvidingEnergy;
        }

        public int GetDrainingEnergy()
        {
            return mDrainingEnergy;
        }

        public bool IsActive()
        {
            return mIsActive;
        }

        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            if (!withinBounds)
            {
                return true;
            }

            if (mouseAction != EMouseAction.LeftClick)
            {
                MakeDamage(Health);
                return false;
            }
            mDirector.GetUserInterfaceController.ActivateMe(this);
            mDirector.GetUserInterfaceController.SelectedPlatformSetsGraphId(mGraphIndex);
            return false;
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            return !withinBounds;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }

        private string GetResourceString()
        {
            if (mResources.Count == 0)
            {
                return "None";
            }
            var resString = "";
            var cType = (EResourceType)0;
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
    }
}
