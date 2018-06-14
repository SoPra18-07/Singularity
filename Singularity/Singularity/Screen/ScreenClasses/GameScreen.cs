using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Map;
using Singularity.Platform;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Handles everything thats going on explicitly in the game.
    /// E.g. game objects, the map, camera. etc.
    /// </summary>
    internal sealed class GameScreen : IScreen
    {
        // sprite textures
        private Texture2D mPlatformSheet;
        private Texture2D mMUnitSheet;
        private Texture2D mPlatformBlankTexture;
        private Texture2D mPlatformDomeTexture;

        // platforms
        private PlatformBlank mPlatform;
        private PlatformBlank mPlatform2;
        private EnergyFacility mPlatform3;

        // units
        private MilitaryUnit mMUnit1;
        private MilitaryUnit mMUnit2;

        // map and fog of war
        private Map.Map mMap;
        private FogOfWar mFow;

        // input manager and viewport
        private readonly InputManager mInputManager;
        private readonly Viewport mViewport;

        // roads
        private Road mRoad1;

        private PlatformBuildingRoadConnector mPlatformRoadConnector;

        /// <summary>
        /// This list contains all the drawable objects currently in the game.
        /// </summary>
        private readonly LinkedList<IDraw> mDrawables;

        /// <summary>
        /// This list contains all the updateable objects currently in the game.
        /// </summary>
        private readonly LinkedList<IUpdate> mUpdateables;

        private readonly LinkedList<IDraw> mDrawablesToAdd;

        private readonly LinkedList<IUpdate> mUpdateablesToAdd;

        private readonly LinkedList<IDraw> mDrawablesToRemove;

        private readonly LinkedList<IUpdate> mUpdateablesToRemove;

        /// <summary>
        /// The camera object which holds transformation values.
        /// </summary>
        private Camera mCamera;

        public GameScreen(Viewport viewport, InputManager inputManager, Camera camera)
        {
            mDrawables = new LinkedList<IDraw>();
            mUpdateables = new LinkedList<IUpdate>();
            mDrawablesToAdd = new LinkedList<IDraw>();
            mUpdateablesToAdd = new LinkedList<IUpdate>();
            mDrawablesToRemove = new LinkedList<IDraw>();
            mUpdateablesToRemove = new LinkedList<IUpdate>();

            mCamera = camera;

            mInputManager = inputManager;
            mViewport = viewport;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTransform());

            foreach (var drawable in mDrawables)
            {
                drawable.Draw(spriteBatch);
            }

            spriteBatch.End();

        }

        public bool DrawLower()
        {
            return true;
        }

        public void Update(GameTime gametime)
        {
            foreach (var updateable in mUpdateables)
            {
                var spatial = updateable as ISpatial;

                if (spatial != null)
                {
                    spatial.RelativePosition = Vector2.Transform(spatial.AbsolutePosition, mCamera.GetTransform());
                    spatial.RelativeSize = spatial.AbsoluteSize * mCamera.GetZoom();


                }

                var platform = updateable as PlatformBlank;

                if (platform != null)
                {
                    if (platform.IsPlaced && !platform.IsAdded)
                    {
                        mMap.AddPlatform(platform);
                        platform.IsAdded = true;
                    }

                    if (platform.IsSemiPlaced && !platform.IsPlaced)
                    {
                        mPlatformRoadConnector.SetPlatformToConnect(platform);
                    }
                }

                var road = updateable as Road;

                if (road != null)
                {
                    if (road.IsPlaced && !road.IsAdded)
                    {
                        mMap.AddRoad(road);
                        road.IsAdded = true;
                    }
                }

                updateable.Update(gametime);

                var collider = updateable as ICollider;

                if (collider != null)
                {
                    mMap.UpdateCollider(collider);
                }

            }

            foreach (var updateToAdd in mUpdateablesToAdd)
            {
                mUpdateables.AddLast(updateToAdd);
            }

            foreach (var updateToRemove in mUpdateablesToRemove)
            {
                mUpdateables.Remove(updateToRemove);
            }

            foreach (var drawToAdd in mDrawablesToAdd)
            {
                mDrawables.AddLast(drawToAdd);
            }

            foreach (var drawToRemove in mDrawablesToRemove)
            {
                mDrawables.Remove(drawToRemove);
            }

            mUpdateablesToAdd.Clear();
            mUpdateablesToRemove.Clear();
            mDrawablesToAdd.Clear();
            mDrawablesToRemove.Clear();
        }

        public void LoadContent(ContentManager content)
        {
            var mapBackground = content.Load<Texture2D>("MockUpBackground");


            mMUnitSheet = content.Load<Texture2D>("UnitSpriteSheet");

            mPlatformSheet = content.Load<Texture2D>("PlatformSpriteSheet");
            mPlatform = new PlatformBlank(new Vector2(300, 400), mPlatformSheet);
            mPlatform2 = new PlatformBlank(new Vector2(800, 600), mPlatformSheet);
            // TODO: use this.Content to load your game content here
            mPlatformBlankTexture = content.Load<Texture2D>("PlatformBasic");
            mPlatformDomeTexture = content.Load<Texture2D>("Dome");
            mPlatform = new PlatformBlank(new Vector2(300, 400), mPlatformBlankTexture);
            mPlatform2 = new Junkyard(new Vector2(800, 600), mPlatformDomeTexture);
            mPlatform3 = new EnergyFacility(new Vector2(600, 200), mPlatformDomeTexture);


            var resources = ResourceHelper.GetRandomlyDistributedResources(5);

            mFow = new FogOfWar(mapBackground);

            var structMap = new StructureMap(mFow);

            mMap = new Map.Map(mapBackground, mFow, mCamera, structMap, false, resources);
            mCamera = mMap.GetCamera();

            mPlatformRoadConnector = new PlatformBuildingRoadConnector(structMap, mInputManager, this);

            mMUnit1 = new MilitaryUnit(new Vector2(600, 600), mMUnitSheet, mMap.GetCamera(), mInputManager);
            mMUnit2 = new MilitaryUnit(new Vector2(100, 600), mMUnitSheet, mMap.GetCamera(), mInputManager);

            mFow.AddRevealingObject(mMUnit1);
            mFow.AddRevealingObject(mMUnit2);

            // load roads
            mRoad1 = new Road(mPlatform, mPlatform2, false);
            var road2 = new Road(mPlatform, mPlatform3, false);
            var road3 = new Road(mPlatform2, mPlatform3, false);

            AddObject(mMap);
            AddObject(mMUnit1);
            AddObject(mMUnit2);
            AddObject(mPlatform);
            AddObject(mPlatform2);
            AddObject(mPlatform3);
            AddObject(mRoad1);
            AddObject(road2);
            AddObject(road3);
            AddObject(mFow);
            AddObject(mPlatformRoadConnector);
            AddObjects(resources);
        }

        public bool UpdateLower()
        {
            return true;
        }

        /// <summary>
        /// Adds the given object to the game screens list of objects to handle.
        /// </summary>
        /// <typeparam name="T">The type of the object to be added. Needs to inherit from IDraw or IUpdate</typeparam>
        /// <param name="toAdd">The object to be added to the game screen</param>
        /// <returns>True if the given object could be added, false otherwise</returns>
        public bool AddObject<T>(T toAdd)
        {
            if (!typeof(IDraw).IsAssignableFrom(typeof(T)) && !typeof(IUpdate).IsAssignableFrom(typeof(T)))
            {
                return false;
            }

            if (typeof(IDraw).IsAssignableFrom(typeof(T)))
            {
                mDrawablesToAdd.AddLast((IDraw)toAdd);
            }
            if (typeof(IUpdate).IsAssignableFrom(typeof(T)))
            {
                mUpdateablesToAdd.AddLast((IUpdate)toAdd);
            }

            return true;

        }

        /// <summary>
        /// Adds the given objects to the game screens list of objects to handle.
        /// </summary>
        /// <typeparam name="T">The type of the objects to be added. Needs to inherit from IDraw or IUpdate</typeparam>
        /// <param name="toAdd">The objects to be added to the game screen</param>
        /// <returns>True if all given objects could be added to the screen, false otherwise</returns>
        public bool AddObjects<T>(IEnumerable<T> toAdd)
        {
            var isSuccessful = true;

            foreach (var t in toAdd)
            {
                isSuccessful = isSuccessful && AddObject<T>(t);
            }

            return isSuccessful;

        }

        /// <summary>
        /// Removes the given object from the game screens list of objects to handle.
        /// </summary>
        /// <typeparam name="T">The type of the object to be removed. Needs to inherit from IDraw or IUpdate</typeparam>
        /// <param name="toRemove">The object to be removed to the game screen</param>
        /// <returns>True if the given object could be removed, false otherwise</returns>
        public bool RemoveObject<T>(T toRemove)
        {
            if (!typeof(IDraw).IsAssignableFrom(typeof(T)) && !typeof(IUpdate).IsAssignableFrom(typeof(T)))
            {
                return false;
            }

            if (typeof(IDraw).IsAssignableFrom(typeof(T)))
            {
                mDrawablesToRemove.Remove((IDraw)toRemove);
            }
            if (typeof(IUpdate).IsAssignableFrom(typeof(T)))
            {
                mUpdateablesToRemove.Remove((IUpdate)toRemove);
            }
            return true;
        }
    }
}