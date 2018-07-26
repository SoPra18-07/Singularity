using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Nature;
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
    [DataContract]
    public sealed class GameScreen : IScreen
    {
        [DataMember]
        public EScreen Screen { get; private set; } = EScreen.GameScreen;
        [DataMember]
        public bool Loaded { get; set; }

        // map and fog of war
        private Map.Map mMap;
        private FogOfWar mFow;

        // director for Managing all the Managers
        private Director mDirector;

        /// <summary>
        /// This list contains all the drawable objects currently in the game.
        /// </summary>
        [DataMember]
        private readonly LinkedList<IDraw> mDrawables;

        /// <summary>
        /// This list contains all the updateable objects currently in the game.
        /// </summary>
        [DataMember]
        private readonly LinkedList<IUpdate> mUpdateables;

        /// <summary>
        /// The idea is that all spatial objects are affected by the fog of war, so we save them seperately to have a seperation
        /// in our game screen. This way we can apply masks and all that stuff more easily.
        /// </summary>
        [DataMember]
        private readonly LinkedList<ISpatial> mSpatialObjects;
        [DataMember]
        private Matrix mTransformMatrix;

        [DataMember]
        private bool mUistarted;

        /// <summary>
        /// The camera object which holds transformation values.
        /// </summary>
        private Camera mCamera;

        private SelectionBox mSelBox;



        public GameScreen(GraphicsDevice graphicsDevice, ref Director director, Map.Map map, Camera camera, FogOfWar fow)
        {

            mDrawables = new LinkedList<IDraw>();
            mUpdateables = new LinkedList<IUpdate>();
            mSpatialObjects = new LinkedList<ISpatial>();

            mMap = map;
            mCamera = camera;
            mFow = fow;

            mUistarted = false;
            mDirector = director;

            mSelBox = new SelectionBox(Color.White, mCamera, ref mDirector);
            AddObject(mSelBox);
        }

        public void ReloadContent(ContentManager content, GraphicsDeviceManager graphics, Map.Map map, FogOfWar fow , Camera camera, ref Director director, UserInterfaceScreen ui)
        {
            mMap = map;
            mFow = fow;
            mCamera = camera;
            mDirector = director;
            mDirector.GetSoundManager.SetLevelThemeMusic("Tutorial");
            mDirector.GetSoundManager.SetSoundPhase(SoundPhase.Build);
            mSelBox = new SelectionBox(Color.White, mCamera, ref mDirector);
            AddObject(mSelBox);
            //All collider items have to be readded to the ColliderMap
            var colliderlist = new List<ICollider>();
            foreach (var spatial in mSpatialObjects)
            {
                var collider = spatial as ICollider;
                if (collider != null && !colliderlist.Contains(collider))
                {
                    mMap.UpdateCollider(collider);
                    colliderlist.Add(collider);
                }
            }

            //Reload the content for all ingame objects like Platforms etc.
            foreach (var drawable in mDrawables)
            {
                //TODO: Add terrain when its in master
                var possibleMilitaryUnit = drawable as MilitaryUnit;
                var possibleEnemy = drawable as EnemyUnit;
                var possibleSettler = drawable as Settler;
                var possiblegenunit = drawable as GeneralUnit;
                var possiblerock = drawable as Rock;
                var possiblepuddle = drawable as Puddle;
                var conUnit = drawable as FreeMovingUnit;
                if (conUnit != null)
                {
                    mSelBox.SelectingBox += conUnit.BoxSelected;
                }
                possibleEnemy?.ReloadContent(content, ref mDirector, camera, ref mMap);
                possiblepuddle?.ReloadContent(ref mDirector);
                possiblerock?.ReloadContent(ref mDirector);
                //This should also affect enemy units, since they are military units
                possibleMilitaryUnit?.ReloadContent(content, ref mDirector, camera, ref map);
                possibleSettler?.ReloadContent(ref mDirector, mCamera, ref map, this, ui);
                if (possibleSettler != null)
                {
                    possibleSettler.BuildCommandCenter += SettlerBuild;
                }
                possiblegenunit?.ReloadContent(ref mDirector, content);
            }

            //Reload the content for all ingame objects like Platforms etc.
            foreach (var updateable in mUpdateables)
            {
                //TODO: Add terrain when its in master
                var possibleMilitaryUnit = updateable as MilitaryUnit;
                var possibleEnemy = updateable as EnemyUnit;
                var possibleSettler = updateable as Settler;
                var possiblegenunit = updateable as GeneralUnit;
                var possiblerock = updateable as Rock;
                var possiblepuddle = updateable as Puddle;

                var freeMovingUnit = updateable as FreeMovingUnit;
                if (freeMovingUnit != null && freeMovingUnit.Friendly)
                {
                    mSelBox.SelectingBox += freeMovingUnit.BoxSelected;
                }
                possibleEnemy?.ReloadContent(content, ref mDirector, camera, ref mMap);
                possiblepuddle?.ReloadContent(ref mDirector);
                possiblerock?.ReloadContent(ref mDirector);
                //This should also affect enemy units, since they are military units
                possibleMilitaryUnit?.ReloadContent(content, ref mDirector, camera, ref map);
                possibleSettler?.ReloadContent(ref mDirector, mCamera, ref map, this, ui);
                if (possibleSettler != null)
                {
                    possibleSettler.BuildCommandCenter += SettlerBuild;
                }
                possiblegenunit?.ReloadContent(ref mDirector, content);
            }

            //Reload the content for all ingame objects like Platforms etc.
            foreach (var spatial in mSpatialObjects)
            {
                //TODO: Add terrain when its in master
                var possibleMilitaryUnit = spatial as MilitaryUnit;
                var possibleEnemy = spatial as EnemyUnit;
                var possibleSettler = spatial as Settler;
                var possiblegenunit = spatial as GeneralUnit;
                var possiblerock = spatial as Rock;
                var possiblepuddle = spatial as Puddle;
                var freeMovingUnit = spatial as FreeMovingUnit;
                var platform = spatial as PlatformBlank;
                if (freeMovingUnit != null && freeMovingUnit.Friendly)
                {
                    mSelBox.SelectingBox += freeMovingUnit.BoxSelected;
                }
                possibleEnemy?.ReloadContent(content, ref mDirector, camera, ref mMap);
                platform?.ReloadContent(content, ref mDirector);
                possiblepuddle?.ReloadContent(ref mDirector);
                possiblerock?.ReloadContent(ref mDirector);
                //This should also affect enemy units, since they are military units
                possibleMilitaryUnit?.ReloadContent(content, ref mDirector, camera, ref map);
                possibleSettler?.ReloadContent(ref mDirector, mCamera, ref map, this, ui);

                if (possibleSettler != null)
                {
                    possibleSettler.BuildCommandCenter += SettlerBuild;
                }
                possiblegenunit?.ReloadContent(ref mDirector, content);
            }

            if (mUistarted)
            {
                ui.Activate();
            }
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

            if (GlobalVariables.FowEnabled)
            {

                mFow.DrawMasks(spriteBatch);

                spriteBatch.Begin(SpriteSortMode.FrontToBack,
                    BlendState.AlphaBlend,
                    null,
                    mFow.GetApplyMaskStencilState(),
                    null,
                    null,
                    mTransformMatrix);

                mMap.GetStructureMap().Draw(spriteBatch);
                mMap.GetResourceMap().Draw(spriteBatch);

                foreach (var spatial in mSpatialObjects)
                {
                    spatial.Draw(spriteBatch);
                }

                spriteBatch.End();

                mFow.FillInvertedMask(spriteBatch);
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.FrontToBack,
                    BlendState.AlphaBlend,
                    null,
                    null,
                    null,
                    null,
                    mTransformMatrix);

                mMap.GetStructureMap().Draw(spriteBatch);
                mMap.GetResourceMap().Draw(spriteBatch);

                foreach (var spatial in mSpatialObjects)
                {
                    spatial.Draw(spriteBatch);
                }

                spriteBatch.End();
            }

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, mTransformMatrix);

            mMap.GetStructureMap().DrawAboveFow(spriteBatch);

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
                updateable.Update(gametime);
            }

            List<ISpatial> copyList = (mSpatialObjects.Concat(mMap.GetStructureMap().GetPlatformList()).ToList());

            foreach (var spatial in copyList)
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
            
            var settler = toAdd as Settler;
            var freeMovingUnit = toAdd as FreeMovingUnit;

            if (!typeof(IDraw).IsAssignableFrom(typeof(T)) && !typeof(IUpdate).IsAssignableFrom(typeof(T)))
            {
                return false;
            }
            
            /*
            if (platform != null)
            {
                // mMap.AddPlatform(platform);
                mDirector.GetMilitaryManager.AddPlatform(platform);
                // mUpdateables.AddLast(platform); // otherwise Platforms won't produce as of now.
                return true;
            } // */

            // subscribes the game screen the the settler event (to build a command center)
            // TODO unsubscribe / delete settler when event is fired
            if (settler != null)
            {
                settler.BuildCommandCenter += SettlerBuild;
            }

            // subscribe every military unit to the selection box
            if (freeMovingUnit != null)
            {
                if (freeMovingUnit.Friendly)
                {
                    mSelBox.SelectingBox += freeMovingUnit.BoxSelected;
                }

                mDirector.GetMilitaryManager.AddUnit(freeMovingUnit);
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
            var freeMovingUnit = toRemove as FreeMovingUnit;

            if (!typeof(IDraw).IsAssignableFrom(typeof(T)) && !typeof(IUpdate).IsAssignableFrom(typeof(T)) && road == null && platform == null)
            {
                return false;
            }

            if (typeof(ICollider).IsAssignableFrom(typeof(T)))
            {
                mMap.GetCollisionMap().RemoveCollider((ICollider)toRemove);
            }

            if (road != null && !road.Blueprint)
            {
                mMap.RemoveRoad(road);
            }

            if (platform != null && !platform.mBlueprint)
            {
                mMap.RemovePlatform(platform);
                mDirector.GetMilitaryManager.RemovePlatform(platform);
            }

            if (settler != null)
            {
                settler.BuildCommandCenter -= SettlerBuild;
            }

            // unsubscribe from this military unit when deleted
            if (freeMovingUnit != null && freeMovingUnit.Friendly)
            {
                mSelBox.SelectingBox -= freeMovingUnit.BoxSelected;
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

        public void Unload()
        {
            var keyListenersList = new List<IKeyListener>();
            var mousePosListenersList = new List<IMousePositionListener>();
            var mouseClickListenersList = new List<IMouseClickListener>();
            var mouseScrollListenersList = new List<IMouseWheelListener>();

            foreach (var updateable in mUpdateables)
            {
                var key = updateable as IKeyListener;
                var mousePos = updateable as IMousePositionListener;
                var mouseClick = updateable as IMouseClickListener;
                var mouseScroll = updateable as IMouseWheelListener;

                if (key != null)
                {
                    mDirector.GetInputManager.FlagForRemoval(key);
                }

                if (mousePos != null)
                {
                    mDirector.GetInputManager.RemoveMousePositionListener(mousePos);
                }

                if (mouseClick != null)
                {
                    mDirector.GetInputManager.FlagForRemoval(mouseClick);
                }

                if (mouseScroll != null)
                {
                    mDirector.GetInputManager.FlagForRemoval(mouseScroll);
                }
            }
            mDirector.GetInputManager.RemoveMousePositionListener(mSelBox);
            mDirector.GetInputManager.FlagForRemoval(mSelBox);
            mDirector.GetInputManager.FlagForRemoval(mCamera as IKeyListener);
            mDirector.GetInputManager.FlagForRemoval(mCamera as IMouseWheelListener);
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

            // adds the command center to the GameScreen, as well as two general units

            var cCenter = PlatformFactory.Get(EStructureType.Command, ref mDirector, v.X - 55, v.Y - 100, commandBlueprint: false);
            mDirector.GetMilitaryManager.AddPlatform(cCenter);
            mDirector.GetStoryManager.Level.Map.AddPlatform(cCenter);

            var genUnit = new GeneralUnit(cCenter, ref mDirector);
            AddObject(genUnit);

            var genUnit2 = new GeneralUnit(cCenter, ref mDirector);
            AddObject(genUnit2);

            var genUnit3 = new GeneralUnit(cCenter, ref mDirector);
            AddObject(genUnit3);

            var dbg = false;

            // /*
            dbg = true;
            var beginRes = new Dictionary<EResourceType, int>
            {
                {EResourceType.Metal, 100},
                {EResourceType.Stone, 100},
                {EResourceType.Water, 100},
                {EResourceType.Oil, 100},
                {EResourceType.Copper, 100},
                {EResourceType.Fuel, 100},
                {EResourceType.Chip, 100},
                {EResourceType.Concrete, 100},
                {EResourceType.Plastic, 100},
                {EResourceType.Steel, 100},
                {EResourceType.Sand, 100},
                {EResourceType.Silicon, 100},
                {EResourceType.Trash, 2}
            }; // */

            // var beginRes = new Dictionary<EResourceType, int> {{EResourceType.Metal, 12}, {EResourceType.Stone, 8}};

            foreach (var pair in beginRes)
            {
                if (!dbg)
                {
                    for (int i = 0; i < pair.Value; i++)
                    {
                        cCenter.StoreResource(new Resource(pair.Key, cCenter.Center, mDirector));
                    }
                }
                else
                {
                    for (int i = 0; i < pair.Value; i++)
                    {
                        cCenter.StoreResource(new Resource(pair.Key, cCenter.Center, mDirector));
/*                        cCenter.StoreResource(new Resource(pair.Key, cCenter.Center, mDirector));
                        cCenter.StoreResource(new Resource(pair.Key, cCenter.Center, mDirector));
                        cCenter.StoreResource(new Resource(pair.Key, cCenter.Center, mDirector));
                        cCenter.StoreResource(new Resource(pair.Key, cCenter.Center, mDirector));*/
                    }
                }
            }
            

            // removes the settler from the GameScreen
            RemoveObject(s);
            mUistarted = true;
        }


    }
}
