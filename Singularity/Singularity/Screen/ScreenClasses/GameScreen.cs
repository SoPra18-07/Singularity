using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
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

        /// <summary>
        /// This list contains all the drawable objects currently in the game.
        /// </summary>
        private readonly LinkedList<IDraw> mDrawables;

        /// <summary>
        /// This list contains all the updateable objects currently in the game.
        /// </summary>
        private readonly LinkedList<IUpdate> mUpdateables;

        /// <summary>
        /// The camera object which holds transformation values.
        /// </summary>
        private Camera mCamera;

        public GameScreen(Viewport viewport, InputManager inputManager)
        {
            mDrawables = new LinkedList<IDraw>();
            mUpdateables = new LinkedList<IUpdate>();

            mInputManager = inputManager;
            mViewport = viewport;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTransform());

            foreach (var drawable in mDrawables)
            {
                drawable.Draw(spriteBatch);
            }

            spriteBatch.End();

        }

        public bool DrawLower()
        {
            return false;
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
                updateable.Update(gametime);

            }
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

            mFow = new FogOfWar(mapBackground);
            mMap = new Map.Map(mapBackground, mViewport, mFow, mInputManager);
            mCamera = mMap.GetCamera();
            AddObject(mMap);

            mMUnit1 = new MilitaryUnit(new Vector2(600, 600), mMUnitSheet, mMap.GetCamera(), mInputManager);
            mMUnit2 = new MilitaryUnit(new Vector2(100, 600), mMUnitSheet, mMap.GetCamera(), mInputManager);

            mFow.AddRevealingObject(mMUnit1);
            mFow.AddRevealingObject(mMUnit2);

            mMap.AddPlatform(mPlatform);
            mMap.AddPlatform(mPlatform2);
            mMap.AddPlatform(mPlatform3);

            // load roads
            mRoad1 = new Road(mPlatform, mPlatform2, false);
            var road2 = new Road(mPlatform, mPlatform3, false);
            var road3 = new Road(mPlatform2, mPlatform3, false);

            AddObject(mMUnit1);
            AddObject(mMUnit2);
            AddObject(mPlatform);
            AddObject(mPlatform2);
            AddObject(mPlatform3);
            AddObject(mRoad1);
            AddObject(road2);
            AddObject(road3);
            AddObject(mFow);
            AddObject(ResourceHelper.GetRandomlyDistributedResources(5));

            // artificially adding wait to test loading screen
            System.Threading.Thread.Sleep(500);
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