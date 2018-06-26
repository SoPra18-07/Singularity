using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph.Paths;
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
        public EScreen Screen { get; private set; } = EScreen.GameScreen;
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

        private EnemyUnit mEnemy1;

        // map and fog of war
        private Map.Map mMap;
        private FogOfWar mFow;

        // director for Managing all the Managers
        private Director mDirector;
        private readonly GraphicsDevice mGraphicsDevice;

        //Other managers
        private PathManager mPathManager;

        private Manager.StoryManager mStoryManager;

        private DistributionManager mDistributionManager;
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



        public GameScreen(GraphicsDevice graphicsDevice, ref Director director, Map.Map map, Camera camera, FogOfWar fow)
        {
            mGraphicsDevice = graphicsDevice;

            mDrawables = new LinkedList<IDraw>();
            mUpdateables = new LinkedList<IUpdate>();
            mSpatialObjects = new LinkedList<ISpatial>();

            mMap = map;
            mCamera = camera;
            mFow = fow;

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
            return true;
        }

        public void Update(GameTime gametime)
        {
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

            AddObject(mMap);
  
            AddObjects(ResourceHelper.GetRandomlyDistributedResources(5));

            mDirector.GetSoundManager.SetLevelThemeMusic("Tutorial");
            mDirector.GetSoundManager.SetSoundPhase(SoundPhase.Build);
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

            var road = toAdd as Road;
            var platform = toAdd as PlatformBlank;

            if (!typeof(IDraw).IsAssignableFrom(typeof(T)) && !typeof(IUpdate).IsAssignableFrom(typeof(T)) && road == null && platform == null)
            {
                return false;
            }

            if (road != null)
            {
                mMap.AddRoad(road);
            }

            if (platform != null)
            {
                mMap.AddPlatform(platform);
            }

            if (typeof(IRevealing).IsAssignableFrom(typeof(T)))
            {
                mFow.AddRevealingObject((IRevealing)toAdd);
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
            var road = toRemove as Road;
            var platform = toRemove as PlatformBlank;

            if (!typeof(IDraw).IsAssignableFrom(typeof(T)) && !typeof(IUpdate).IsAssignableFrom(typeof(T)) && road == null && platform == null)
            {
                return false;
            }

            if (road != null)
            {
                mMap.RemoveRoad(road);
            }

            if (platform != null)
            {
                mMap.RemovePlatform(platform);
            }

            if (typeof(IRevealing).IsAssignableFrom(typeof(T)))
            {
                mFow.RemoveRevealingObject((IRevealing)toRemove);
            }

            if (typeof(ISpatial).IsAssignableFrom(typeof(T)))
            {
                mSpatialObjects.Remove((ISpatial)toRemove);
                return true;
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
