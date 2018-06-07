using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Map;
using Singularity.Property;
using Singularity.Screen;

namespace Singularity.screen
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// The game screen handles everything thats going on explicitly in the game. E.g. game objects, the map, camera. etc.
    /// </summary>
    internal sealed class GameScreen : IScreen
    {
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
        private readonly Camera mCamera;

        public GameScreen(Map.Map map)
        {
            mDrawables = new LinkedList<IDraw>();
            mUpdateables = new LinkedList<IUpdate>();

            mCamera = map.GetCamera();

            AddObject<Map.Map>(map);
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
                mDrawables.AddLast((IDraw) toAdd);
            }
            if (typeof(IUpdate).IsAssignableFrom(typeof(T)))
            { 
                mUpdateables.AddLast((IUpdate) toAdd);
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
                mDrawables.Remove((IDraw) toRemove);
            }
            if (typeof(IUpdate).IsAssignableFrom(typeof(T)))
            {
                mUpdateables.Remove((IUpdate) toRemove);
            }
            return true;
        }
    }
}
