
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Singularity.map
{
    internal sealed class Map
    {

        private const int StandardWidth = 2000;
        private const int StandardHeight = 1000;

        private Texture2D Background { get; }
        public int Width { get; }
        public int Height { get; }
        public Vector2 Scope { get; set; }

        private Vector2 mAnchor;

        private const int MapMovementSpeed = 20;

        private readonly Random mRandom;

        private readonly SortedSet<SpatialGraphic> mEntities;

        public Map(Texture2D background, int scopeWidth, int scopeHeight, int width = StandardWidth, int height = StandardHeight)
        {
            mEntities = new SortedSet<SpatialGraphic>(new LayerComparator<SpatialGraphic>());
            Background = background;
            Width = width;
            Height = height;
            Scope = new Vector2(scopeWidth, scopeHeight);
            mAnchor = new Vector2(0, 0);
            mRandom = new Random();

        }

        /// <summary>
        /// Adds the graphic specified by the given parameters to this map.
        /// </summary>
        /// <param name="graphic">The graphic of the Object to be added</param>
        /// <param name="position">The position of the Object to be added</param>
        /// <param name="layer">The layer of the Object to be added</param>
        public void AddGraphic(Texture2D graphic, Vector2 position, int layer)
        {
            mEntities.Add(new SpatialGraphic(graphic, position, layer));
        }

        /// <summary>
        /// Removes the graphic specified by the given parameters from this map.
        /// </summary>
        /// <param name="graphic">The graphic of the Object to be removed</param>
        /// <param name="position">The position of the Object to be removed</param>
        /// <param name="layer">The layer of the Object to be removed</param>
        public void RemoveGraphic(Texture2D graphic, Vector2 position, int layer)
        {
            mEntities.Remove(new SpatialGraphic(graphic, position, layer));
        }

        /// <summary>
        /// Checks for input and appropriately sets the new anchor point of the camera.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                mAnchor.Y += MapMovementSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                mAnchor.X += MapMovementSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                mAnchor.Y -= MapMovementSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                mAnchor.X -= MapMovementSpeed;
            }

            if (mAnchor.Y > 0)
            {
                mAnchor.Y = 0;
            }

            if (mAnchor.X > 0)
            {
                mAnchor.X = 0;
            }

            if (mAnchor.X - Scope.X < -Width)
            {
                mAnchor.X = Scope.X - Width;
            }

            if (mAnchor.Y - Scope.Y < -Height)
            {
                mAnchor.Y = Scope.Y - Height;
            }
        }

        /// <summary>
        /// Draws the background and the units which got added to this map.
        /// </summary>
        /// <param name="graphics">Provides various graphics related variables</param>
        /// <param name="spriteBatch">Used to draw onto the canvas</param>
        internal void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            //TODO: project the image onto the given scope so it works if the scope is smaller than the window
            spriteBatch.Draw(Background, new Rectangle((int) mAnchor.X, (int) mAnchor.Y, Width, Height), Color.White);

            var entitiesIterator = mEntities.GetEnumerator();
            do
            {
                var current = entitiesIterator.Current;
                if (current == null)
                {
                    continue;
                }
                spriteBatch.Draw(current.Graphic, new Vector2(current.Position.X + mAnchor.X, current.Position.Y + mAnchor.Y), Color.AliceBlue);

            } while (entitiesIterator.MoveNext());

            entitiesIterator.Dispose();

        }
        /// <summary>
        /// Generates the given graphic at a random location on the background.
        /// </summary>
        /// <param name="graphic">The graphic of the unit rendered on the background</param>
        public void GenerateGraphicAtRandomLocation(Texture2D graphic)
        {
            var x = mRandom.Next(0, Width);
            var y = mRandom.Next(0, Height);
            AddGraphic(graphic, new Vector2(x, y), mRandom.Next(0, 10));
        }
    }
}
