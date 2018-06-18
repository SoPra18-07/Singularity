using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph.Paths;
using Singularity.Input;
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
        // sprite textures
        private Texture2D _mPlatformSheet;
        private Texture2D _mMUnitSheet;
        private Texture2D _mPlatformBlankTexture;
        private Texture2D _mPlatformDomeTexture;

        // platforms
        private PlatformBlank _mPlatform;
        private PlatformBlank _mPlatform2;
        private EnergyFacility _mPlatform3;

        // units
        private MilitaryUnit _mMUnit1;
        private MilitaryUnit _mMUnit2;

        // map and fog of war
        private Map.Map _mMap;
        private FogOfWar _mFow;

        // director for Managing all the Managers
        private readonly Director director;
        private readonly InputManager _mInputManager;
        private readonly GraphicsDevice _mGraphicsDevice;

        // roads
        private Road _mRoad1;

        /// <summary>
        /// This list contains all the drawable objects currently in the game.
        /// </summary>
        private readonly LinkedList<IDraw> _mDrawables;

        /// <summary>
        /// This list contains all the updateable objects currently in the game.
        /// </summary>
        private readonly LinkedList<IUpdate> _mUpdateables;

        /// <summary>
        /// The idea is that all spatial objects are affected by the fog of war, so we save them seperately to have a seperation
        /// in our game screen. This way we can apply masks and all that stuff more easily.
        /// </summary>
        private readonly LinkedList<ISpatial> _mSpatialObjects;

        /// <summary>
        /// The camera object which holds transformation values.
        /// </summary>
        private Camera _mCamera;


        public GameScreen(GraphicsDevice graphicsDevice, InputManager inputManager)
        {
            _mGraphicsDevice = graphicsDevice;

            _mDrawables = new LinkedList<IDraw>();
            _mUpdateables = new LinkedList<IUpdate>();
            _mSpatialObjects = new LinkedList<ISpatial>();

            _mInputManager = inputManager;

            // TODO: This is actually the wrong place to initialize the Director. Move it.
            director = new Director(inputManager, new StoryManager(), new PathManager(), new SoundManager(), new MilitaryManager());
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            // if you're interested in whats going on here, refer to the documentation of the FogOfWar class. 

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, _mCamera.GetTransform());

            foreach (var drawable in _mDrawables)
            {
                drawable.Draw(spriteBatch);
            }

            spriteBatch.End();

            _mFow.DrawMasks(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, _mFow.GetApplyMaskStencilState(), null, null, _mCamera.GetTransform());

            foreach (var spatial in _mSpatialObjects)
            {
                spatial.Draw(spriteBatch);
            }
            spriteBatch.End();

            _mFow.FillInvertedMask(spriteBatch);
           

        }

        public bool DrawLower()
        {
            return false;
        }

        public void Update(GameTime gametime)
        {

            foreach (var updateable in _mUpdateables)
            {
                updateable.Update(gametime);

            }

            foreach (var spatial in _mSpatialObjects)
            {
                var collidingObject = spatial as ICollider;

                if (collidingObject != null)
                {
                    _mMap.UpdateCollider(collidingObject);
                }

                spatial.RelativePosition = Vector2.Transform(spatial.AbsolutePosition, _mCamera.GetTransform());
                spatial.RelativeSize = spatial.AbsoluteSize * _mCamera.GetZoom();

                spatial.Update(gametime);
            }
            _mFow.Update(gametime);


        }

        public void LoadContent(ContentManager content)
        {
            var mapBackground = content.Load<Texture2D>("MockUpBackground");
            _mMap = new Map.Map(mapBackground, _mGraphicsDevice.Viewport, director, true);
            _mCamera = _mMap.GetCamera();
            //Give the Distributionmanager the Graph he is operating on. 
            //TODO: Talk about whether the DistributionManager should operate on all Graphs or if we want to make additional DMs.
            var dist = new Manager.DistributionManager();

            _mMUnitSheet = content.Load<Texture2D>("UnitSpriteSheet");

            _mPlatformSheet = content.Load<Texture2D>("PlatformSpriteSheet");
            _mPlatform = new PlatformBlank(new Vector2(300, 400), _mPlatformSheet);
            _mPlatform2 = new PlatformBlank(new Vector2(800, 600), _mPlatformSheet);
            // TODO: use this.Content to load your game content here
            _mPlatformBlankTexture = content.Load<Texture2D>("PlatformBasic");
            _mPlatformDomeTexture = content.Load<Texture2D>("Dome");
            _mPlatform = new PlatformBlank(new Vector2(300, 400), _mPlatformBlankTexture);
            _mPlatform2 = new Junkyard(new Vector2(800, 600), _mPlatformDomeTexture);
            _mPlatform3 = new EnergyFacility(new Vector2(600, 200), _mPlatformDomeTexture);
            _mPlatform3 = new EnergyFacility(new Vector2(600, 200), _mPlatformDomeTexture);

            var genUnit2 = new GeneralUnit(_mPlatform2, director.GetPathManager, dist);
            var genUnit3 = new GeneralUnit(_mPlatform3, director.GetPathManager, dist);

            var platform4 = new Well(new Vector2(1000, 200), _mPlatformDomeTexture, _mMap.GetResourceMap());
            var platform5 = new Quarry(new Vector2(1300, 400), _mPlatformDomeTexture, _mMap.GetResourceMap());

            var genUnit = new GeneralUnit(_mPlatform, director.GetPathManager, dist);
            var genUnit4 = new GeneralUnit(platform4, director.GetPathManager, dist);
            var genUnit5 = new GeneralUnit(platform5, director.GetPathManager, dist);

            _mFow = new FogOfWar(_mCamera, _mGraphicsDevice);

            _mMUnit1 = new MilitaryUnit(new Vector2(600, 600), _mMUnitSheet, _mMap.GetCamera(), _mInputManager);
            _mMUnit2 = new MilitaryUnit(new Vector2(100, 600), _mMUnitSheet, _mMap.GetCamera(), _mInputManager);

            // load roads
            _mRoad1 = new Road(_mPlatform, _mPlatform2, false);
            var road2 = new Road(_mPlatform3, platform4, false);
            var road3 = new Road(_mPlatform2, _mPlatform3, false);
            var road4 = new Road(platform4, platform5, false);
            var road5 = new Road(platform5, _mPlatform, false);
            var road6 = new Road(_mPlatform, platform4, false);

            AddObject(_mMap);

            _mFow.AddRevealingObject(_mMUnit1);
            _mFow.AddRevealingObject(_mMUnit2);
            _mFow.AddRevealingObject(_mPlatform);
            _mFow.AddRevealingObject(_mPlatform2);
            _mFow.AddRevealingObject(_mPlatform3);
            _mFow.AddRevealingObject(platform4);
            _mFow.AddRevealingObject(platform5);

            _mMap.AddPlatform(_mPlatform);
            _mMap.AddPlatform(_mPlatform2);
            _mMap.AddPlatform(_mPlatform3);
            _mMap.AddPlatform(platform4);
            _mMap.AddPlatform(platform5);
            _mMap.AddRoad(_mRoad1);
            _mMap.AddRoad(road2);
            _mMap.AddRoad(road3);
            _mMap.AddRoad(road4);
            _mMap.AddRoad(road5);
            _mMap.AddRoad(road6);

            AddObject(_mMUnit1);
            AddObject(_mMUnit2);
            AddObject(_mPlatform);
            AddObject(_mPlatform2);
            AddObject(_mPlatform3);
            AddObject(platform4);
            AddObject(platform5);
            AddObject(_mRoad1);
            AddObject(road2);
            AddObject(road3);
            AddObject(road4);
            AddObject(road5);
            AddObject(road6);

            AddObject(genUnit);
            AddObject(genUnit2);
            AddObject(genUnit3);
            AddObject(genUnit4);
            AddObject(genUnit5);
  
            AddObjects(ResourceHelper.GetRandomlyDistributedResources(5));

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

            if (typeof(ISpatial).IsAssignableFrom(typeof(T)))
            {
                _mSpatialObjects.AddLast((ISpatial) toAdd);
                return true;
            }

            if (typeof(IDraw).IsAssignableFrom(typeof(T)))
            {
                _mDrawables.AddLast((IDraw)toAdd);
            }
            if (typeof(IUpdate).IsAssignableFrom(typeof(T)))
            {
                _mUpdateables.AddLast((IUpdate)toAdd);
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
                _mDrawables.Remove((IDraw)toRemove);
            }
            if (typeof(IUpdate).IsAssignableFrom(typeof(T)))
            {
                _mUpdateables.Remove((IUpdate)toRemove);
            }
            return true;
        }
    }
}