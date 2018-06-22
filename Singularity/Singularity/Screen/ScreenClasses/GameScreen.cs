﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platform;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Sound;
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

        public bool Loaded { get; set; }

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

        // director for Managing all the Managers
        private Director mDirector;
        private readonly GraphicsDevice mGraphicsDevice;

        // roads
        private Road mRoad1;

        /// <summary>
        /// This list contains all the drawable objects currently in the game.
        /// </summary>
        private readonly LinkedList<IDraw> mDrawables;

        /// <summary>
        /// This list contains all the updateable objects currently in the game.
        /// </summary>
        private readonly LinkedList<IUpdate> mUpdateables;

        /// <summary>
        /// The idea is that all spatial objects are affected by the fog of war, so we save them seperately to have a seperation
        /// in our game screen. This way we can apply masks and all that stuff more easily.
        /// </summary>
        private readonly LinkedList<ISpatial> mSpatialObjects;

        private Matrix mTransformMatrix;

        /// <summary>
        /// The camera object which holds transformation values.
        /// </summary>
        private Camera mCamera;


        public GameScreen(GraphicsDevice graphicsDevice, Director director)
        {
            mGraphicsDevice = graphicsDevice;

            mDrawables = new LinkedList<IDraw>();
            mUpdateables = new LinkedList<IUpdate>();
            mSpatialObjects = new LinkedList<ISpatial>();

            mDirector = director;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            // if you're interested in whats going on here, refer to the documentation of the FogOfWar class.

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, mTransformMatrix);

            foreach (var drawable in mDrawables)
            {
                drawable.Draw(spriteBatch);
            }

            spriteBatch.End();

            mFow.DrawMasks(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, mFow.GetApplyMaskStencilState(), null, null, mTransformMatrix);

            foreach (var spatial in mSpatialObjects)
            {
                spatial.Draw(spriteBatch);
            }
            spriteBatch.End();

            mFow.FillInvertedMask(spriteBatch);


        }

        public bool DrawLower()
        {
            return false;
        }

        public void Update(GameTime gametime)
        {

            foreach (var updateable in mUpdateables)
            {
                updateable.Update(gametime);

            }

            foreach (var spatial in mSpatialObjects)
            {
                var collidingObject = spatial as ICollider;

                if (collidingObject != null)
                {
                    mMap.UpdateCollider(collidingObject);
                }

                spatial.RelativePosition = Vector2.Transform(spatial.AbsolutePosition, mCamera.GetTransform());
                spatial.RelativeSize = spatial.AbsoluteSize * mCamera.GetZoom();

                spatial.Update(gametime);
            }
            mFow.Update(gametime);

            mTransformMatrix = mCamera.GetTransform();


        }

        public void LoadContent(ContentManager content)
        {

            var mapBackground = content.Load<Texture2D>("MockUpBackground");
            mMap = new Map.Map(mapBackground, mGraphicsDevice.Viewport, mDirector);
            mCamera = mMap.GetCamera();

            //Give the Distributionmanager the Graph he is operating on.
            //TODO: Talk about whether the DistributionManager should operate on all Graphs or if we want to make additional DMs.

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
            mPlatform3 = new EnergyFacility(new Vector2(600, 200), mPlatformDomeTexture);

            var genUnit2 = new GeneralUnit(mPlatform2, ref mDirector);
            var genUnit3 = new GeneralUnit(mPlatform3, ref mDirector);

            var platform4 = new Well(new Vector2(1000, 200), mPlatformDomeTexture, mMap.GetResourceMap());
            var platform5 = new Quarry(new Vector2(1300, 400), mPlatformDomeTexture, mMap.GetResourceMap());

            var genUnit = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit4 = new GeneralUnit(platform4, ref mDirector);
            var genUnit5 = new GeneralUnit(platform5, ref mDirector);

            mFow = new FogOfWar(mCamera, mGraphicsDevice);

            mMUnit1 = new MilitaryUnit(new Vector2(600, 600), mMUnitSheet, mMap.GetCamera(), mDirector);
            mMUnit2 = new MilitaryUnit(new Vector2(100, 600), mMUnitSheet, mMap.GetCamera(), mDirector);

            // load roads
            mRoad1 = new Road(mPlatform, mPlatform2, false);
            var road2 = new Road(mPlatform3, platform4, false);
            var road3 = new Road(mPlatform2, mPlatform3, false);
            var road4 = new Road(platform4, platform5, false);
            var road5 = new Road(platform5, mPlatform, false);
            var road6 = new Road(mPlatform, platform4, false);

            AddObject(mMap);

            mFow.AddRevealingObject(mMUnit1);
            mFow.AddRevealingObject(mMUnit2);
            mFow.AddRevealingObject(mPlatform);
            mFow.AddRevealingObject(mPlatform2);
            mFow.AddRevealingObject(mPlatform3);
            mFow.AddRevealingObject(platform4);
            mFow.AddRevealingObject(platform5);

            mMap.AddPlatform(mPlatform);
            mMap.AddPlatform(mPlatform2);
            mMap.AddPlatform(mPlatform3);
            mMap.AddPlatform(platform4);
            mMap.AddPlatform(platform5);
            mMap.AddRoad(mRoad1);
            mMap.AddRoad(road2);
            mMap.AddRoad(road3);
            mMap.AddRoad(road4);
            mMap.AddRoad(road5);
            mMap.AddRoad(road6);

            AddObject(mMUnit1);
            AddObject(mMUnit2);
            AddObject(mPlatform);
            AddObject(mPlatform2);
            AddObject(mPlatform3);
            AddObject(platform4);
            AddObject(platform5);
            AddObject(mRoad1);
            AddObject(road2);
            AddObject(road3);
            AddObject(road4);
            AddObject(road5);
            AddObject(road6);

            AddObject(genUnit);
            // AddObject(genUnit2);
            // AddObject(genUnit3);
            // AddObject(genUnit4);
            // AddObject(genUnit5);

            AddObjects(ResourceHelper.GetRandomlyDistributedResources(5));

            mDirector.GetSoundManager.SetLevelThemeMusic("Tutorial");
            mDirector.GetSoundManager.SetSoundPhase(SoundPhase.Build);
        }

        public bool UpdateLower()
        {
            return false;
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

            if (typeof(ISpatial).IsAssignableFrom(typeof(T)))
            {
                mSpatialObjects.AddLast((ISpatial) toAdd);
                return true;
            }

            if (typeof(IDraw).IsAssignableFrom(typeof(T)))
            {
                mDrawables.AddLast((IDraw)toAdd);
            }
            if (typeof(IUpdate).IsAssignableFrom(typeof(T)))
            {
                mUpdateables.AddLast((IUpdate)toAdd);
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
            bool isSuccessful = true;

            foreach (var t in toAdd)
            {
                isSuccessful = isSuccessful && AddObject(t);
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
                mDrawables.Remove((IDraw)toRemove);
            }
            if (typeof(IUpdate).IsAssignableFrom(typeof(T)))
            {
                mUpdateables.Remove((IUpdate)toRemove);
            }
            return true;
        }
    }
}
