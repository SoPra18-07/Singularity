﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Exceptions;
using Singularity.Graph;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Platform
{
    /// <inheritdoc cref="IRevealing"/>
    /// <inheritdoc cref="INode"/>
    /// <inheritdoc cref="ICollider"/>
    [DataContract]
    public class PlatformBlank : IRevealing, INode, ICollider, IMousePositionListener, IMouseClickListener

    {

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
        /// Current mouse position on map
        /// </summary>
        private Vector2 mMouse;

        /// <summary>
        /// true, if left click on platform
        /// </summary>
        private bool mClickOnPlatform;

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

        private Color mColor;

        public bool[,] ColliderGrid { get; internal set; }


        public PlatformBlank(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ref Director director, EPlatformType type = EPlatformType.Blank, float centerOffsetY = -36)
        {

            Id = IdGenerator.NextiD();

            mColor = Color.White;

            mDirector = director;

            mCenterOffsetY = centerOffsetY;

            mLayer = LayerConstants.PlatformLayer;

            mType = EPlatformType.Blank;

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

            Moved = false;
            UpdateValues();

            // add platform to input manager to enable platforms being selected
            director.GetInputManager.AddMousePositionListener(this);
            director.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.InBoundsOnly);
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
            Bounds = AbsBounds;
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
                        mColor * transparency,
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
                        mColor * transparency,
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
                        mColor * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        mLayer - 0.01f);
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
                        mColor * transparency,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        mLayer - 0.01f);
                    // Dome
                    spritebatch.Draw(mPlatformSpriteSheet,
                        AbsolutePosition,
                        new Rectangle(148 * (mSheetPosition % 4), 109 * (int) Math.Floor(mSheetPosition / 4d), 148, 109),
                        mColor * transparency,
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

            if (Selected)
            {
                mDirector.GetUserInterfaceController.SetDataOfSelectedPlatform(mType, GetPlatformResources(), GetAssignedUnits(), GetIPlatformActions());
            }
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
                    mSheetPosition = 1;
                    AbsBounds = new Rectangle((int)AbsolutePosition.X,
                        (int)AbsolutePosition.Y,
                        PlatformWidth,
                        170);
                    break;
                case EPlatformType.Command:
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

        public void SetLayer(float layer)
        {
            mLayer = layer;
        }

        /// <summary>
        /// true, if the platform is selected
        /// </summary>
        public bool Selected { get; set; }

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            // update mouse position to current position on map
            mMouse = new Vector2(worldX, worldY);
        }

        public EScreen Screen { get; } = EScreen.GameScreen;
        public Rectangle Bounds { get; private set; }
        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            // prevent mouse input going through the platform
            if (mMouse.X >= AbsBounds.X &&
                mMouse.X <= AbsBounds.X + AbsoluteSize.X &&
                mMouse.Y >= AbsBounds.Y &&
                mMouse.Y <= AbsBounds.Y + AbsoluteSize.Y)
                // mouse on top of platform
            {
                // this bool catches release mouse when not clicked on platform
                mClickOnPlatform = true;
                return false;
            }

            return true;
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            // prevent mouse input going through the platform
            if (mMouse.X >= AbsBounds.X &&
                mMouse.X <= AbsBounds.X + AbsoluteSize.X &&
                mMouse.Y >= AbsBounds.Y &&
                mMouse.Y <= AbsBounds.Y + AbsoluteSize.Y)
                // mouse on top of platform
            {
                return false;
            }

            return true;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            if (mMouse.X >= AbsBounds.X &&
                mMouse.X <= AbsBounds.X + AbsoluteSize.X &&
                mMouse.Y >= AbsBounds.Y &&
                mMouse.Y <= AbsBounds.Y + AbsoluteSize.Y &&
                mClickOnPlatform)
                // mouse on top of platform
            {
                mClickOnPlatform = false;
                Selected = true;
                return false;
            }

            return true;
        }
    }
}
