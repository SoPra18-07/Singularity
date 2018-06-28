using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Sound;

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
        
        // map and fog of war
        private readonly Map.Map mMap;
        private readonly FogOfWar mFow;

        // director for Managing all the Managers
        private readonly Director mDirector;
        private readonly GraphicsDevice mGraphicsDevice;

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

            spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.AlphaBlend, samplerState: null, depthStencilState: null, rasterizerState: null, effect: null, transformMatrix: mTransformMatrix);

            foreach (var drawable in mDrawables)
            {
                drawable.Draw(spriteBatch: spriteBatch);
            }

            spriteBatch.End();

            mFow.DrawMasks(spriteBatch: spriteBatch);

            spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.AlphaBlend, samplerState: null, depthStencilState: mFow.GetApplyMaskStencilState(), rasterizerState: null, effect: null, transformMatrix: mTransformMatrix);

            foreach (var spatial in mSpatialObjects)
            {
                spatial.Draw(spriteBatch: spriteBatch);
            }

            spriteBatch.End();

            mFow.FillInvertedMask(spriteBatch: spriteBatch);
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
                    mMap.UpdateCollider(collider: collidingObject);
                }

                spatial.RelativePosition = Vector2.Transform(position: spatial.AbsolutePosition, matrix: mCamera.GetTransform());
                spatial.RelativeSize = spatial.AbsoluteSize * mCamera.GetZoom();

                spatial.Update(gametime: gametime);
            }
            mFow.Update(gametime: gametime);

            mTransformMatrix = mCamera.GetTransform();
        }

        public void LoadContent(ContentManager content)
        {

            AddObject(toAdd: mMap);

            AddObjects(toAdd: ResourceHelper.GetRandomlyDistributedResources(amount: 5));

            mDirector.GetSoundManager.SetLevelThemeMusic(name: "Tutorial");
            mDirector.GetSoundManager.SetSoundPhase(soundPhase: SoundPhase.Build);
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

            if (!typeof(IDraw).IsAssignableFrom(c: typeof(T)) && !typeof(IUpdate).IsAssignableFrom(c: typeof(T)) && road == null && platform == null)
            {
                return false;
            }

            if (road != null)
            {
                mMap.AddRoad(road: road);
            }

            if (platform != null)
            {
                mMap.AddPlatform(platform: platform);
            }

            if (typeof(IRevealing).IsAssignableFrom(c: typeof(T)))
            {
                mFow.AddRevealingObject(revealingObject: (IRevealing)toAdd);
            }

            if (typeof(ISpatial).IsAssignableFrom(c: typeof(T)))
            {
                mSpatialObjects.AddLast(value: (ISpatial) toAdd);
                return true;
            }

            if (typeof(IDraw).IsAssignableFrom(c: typeof(T)))
            {
                mDrawables.AddLast(value: (IDraw)toAdd);
            }
            if (typeof(IUpdate).IsAssignableFrom(c: typeof(T)))
            {
                mUpdateables.AddLast(value: (IUpdate)toAdd);
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
                isSuccessful = isSuccessful && AddObject(toAdd: t);
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

            if (!typeof(IDraw).IsAssignableFrom(c: typeof(T)) && !typeof(IUpdate).IsAssignableFrom(c: typeof(T)) && road == null && platform == null)
            {
                return false;
            }

            if (road != null)
            {
                mMap.RemoveRoad(road: road);
            }

            if (platform != null)
            {
                mMap.RemovePlatform(platform: platform);
            }

            if (typeof(IRevealing).IsAssignableFrom(c: typeof(T)))
            {
                mFow.RemoveRevealingObject(revealingObject: (IRevealing)toRemove);
            }

            if (typeof(ISpatial).IsAssignableFrom(c: typeof(T)))
            {
                mSpatialObjects.Remove(value: (ISpatial)toRemove);
                return true;
            }

            if (typeof(IDraw).IsAssignableFrom(c: typeof(T)))
            {
                mDrawables.Remove(value: (IDraw)toRemove);
            }
            if (typeof(IUpdate).IsAssignableFrom(c: typeof(T)))
            {
                mUpdateables.Remove(value: (IUpdate)toRemove);
            }
            return true;
        }
    }
}
