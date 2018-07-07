using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
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
    public sealed class GameScreen : IScreen
    {
        public EScreen Screen { get; private set; } = EScreen.GameScreen;
        public bool Loaded { get; set; }

        // map and fog of war
        private readonly Map.Map mMap;
        private readonly FogOfWar mFow;

        // director for Managing all the Managers
        private Director mDirector;
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
        private readonly Camera mCamera;

        private SelectionBox mSelBox;
        private Texture2D mBlankPlat;
        private Texture2D mCylPlat;



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

            mSelBox = new SelectionBox(Color.White, mCamera, ref mDirector);
            AddObject(mSelBox);

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

            mMap.GetStructureMap().Draw(spriteBatch);

            foreach (var spatial in mSpatialObjects)
            {
                spatial.Draw(spriteBatch);
            }

            spriteBatch.End();

            mFow.FillInvertedMask(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, mTransformMatrix);

            mMap.GetStructureMap().DrawAboveFow(spriteBatch);

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
                updateable.Update(gametime);
            }

            foreach (var spatial in mSpatialObjects.Concat(mMap.GetStructureMap().GetPlatformList()))
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
            mMap.GetStructureMap().Update(gametime);

            mFow.Update(gametime);

            mTransformMatrix = mCamera.GetTransform();
        }

        public void LoadContent(ContentManager content)
        {
            AddObject(mMap);

            AddObjects(ResourceHelper.GetRandomlyDistributedResources(50));

            mDirector.GetSoundManager.SetLevelThemeMusic("Tutorial");
            mDirector.GetSoundManager.SetSoundPhase(SoundPhase.Build);

            // This is for the creation of the Command Centers from the settlers
            mBlankPlat = content.Load<Texture2D>("PlatformBasic");
            mCylPlat = content.Load<Texture2D>("Cylinders");
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
            var settler = toAdd as Settler;
            var conUnit = toAdd as ControllableUnit;

            if (!typeof(IDraw).IsAssignableFrom(typeof(T)) && !typeof(IUpdate).IsAssignableFrom(typeof(T)) && road == null && platform == null)
            {
                return false;
            }

            if (road != null)
            {
                mMap.AddRoad(road);
                return true;
            }

            if (platform != null)
            {
                mMap.AddPlatform(platform);
                return true;
            }

            // subscribes the game screen the the settler event (to build a command center)
            // TODO unsubscribe / delete settler when event is fired
            if (settler != null)
            {
                settler.BuildCommandCenter += SettlerBuild;
            }

            // subscribe every military unit to the selection box
            if (conUnit != null)
            {
                mSelBox.SelectingBox += conUnit.BoxSelected;
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
            var isSuccessful = true;

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
            var settler = toRemove as Settler;
            var controllableUnit = toRemove as ControllableUnit;

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

            // TODO don't know if this is necessary, but unsubscribe GameScreen from this instance event
            if (settler != null)
            {
                settler.BuildCommandCenter -= SettlerBuild;
            }

            // unsubscribe from this military unit when deleted
            if (controllableUnit != null)
            {
                //mSelBox.SelectingBox -= milUnit.BoxSelected;
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

        public Map.Map GetMap()
        {
            return mMap;
        }

        public Camera GetCamera()
        {
            return mCamera;
        }


        /// <summary>
        /// This get executed when a settler is transformed into a command center
        /// Essentially this builds a command center
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <param name="v"> the position at which the settler is currently at</param>
        /// <param name="s"> settler passes itself along so that it can be deleted </param>
        private void SettlerBuild(object sender, EventArgs eventArgs, Vector2 v, Settler s)
        {
            // TODO eventually the EPlacementType should be instance but currently that
            // TODO requires a road to be place and therefore throws an exception !!!!!

            CommandCenter cCenter = new CommandCenter(new Vector2(v.X-55, v.Y-100), mCylPlat, mBlankPlat, ref mDirector, false);
            var genUnit = new GeneralUnit(cCenter, ref mDirector);
            var genUnit2 = new GeneralUnit(cCenter, ref mDirector);

            // adds the command center to the GameScreen, as well as two general units
            AddObject(cCenter);
            AddObject(genUnit);
            AddObject(genUnit2);

            // removes the settler from the GameScreen
            RemoveObject(s);
        }


    }
}
