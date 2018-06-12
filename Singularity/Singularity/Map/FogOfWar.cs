using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Map.Properties;
using Singularity.Property;

namespace Singularity.Map
{
    /// <inheritdoc cref="IUpdate"/>
    /// <inheritdoc cref="IDraw"/>
    /// <remarks>
    /// The FogOfWar grays out areas which are not "visible" from the current state of the game. Platforms and Units can
    /// discover new areas which is why they possess a light circle around them.
    /// </remarks>
    internal sealed class FogOfWar : IDraw, IUpdate
    {
        
        /// <summary>
        /// This array holds bit values of whether the position (tile) was visited or not. Where 1 = visited and 0 = unvisited.
        /// </summary>
        private bool[,] mToDraw;

        /// <summary>
        /// A list of all the objects which are able to reveal the fog of war.
        /// </summary>
        private readonly LinkedList<IRevealing> mRevealingObjects;

        /// <summary>
        /// The background texture of the map.
        /// </summary>
        private readonly Texture2D mMapTexture;

        /// <summary>
        /// Creates a new FogOfWar object for the given mapTexture. This texture should be backed by the actual map background texture
        /// AND its dimensions should be backed by the dimensions specified in MapConstants.MapWidth and MapConstants.MapHeight.
        /// </summary>
        /// <param name="mapTexture">The texture of the map mentioned</param>
        public FogOfWar(Texture2D mapTexture)
        {
            mRevealingObjects = new LinkedList<IRevealing>();

            mMapTexture = mapTexture;

            // make sure the resolution of the fog of war is as dense as the collision map
            mToDraw = new bool[
                (MapConstants.MapWidth / MapConstants.FogOfWarGridWidth),
                (MapConstants.MapHeight / MapConstants.FogOfWarGridHeight)
            ];
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // basically we need to iterate through the whole array and draw a gray layer above the map if the position is unvisited
            for (var i = 0; i < mToDraw.GetLength(0); i++)
            {
                for (var j = 0; j < mToDraw.GetLength(1); j++)
                {
                    if (mToDraw[i, j])
                    {
                        continue;
                    }
                    
                    /*
                     * This is definitely sub-optimal. But we need to recall that spriteBatch can only operate on rectangles.
                     * Thus we somehow make the illusion that objects are cut off behind the fog of war and only partially
                     * visible if their appearance is only partially visible. This can be achieved by the following method
                     * of redrawing the map in its pieces over the density of the array used here and leaving the spaces blank
                     * which are visited.
                     */
                    spriteBatch.Draw(
                        mMapTexture,
                        new Rectangle((int)(i * (MapConstants.FogOfWarGridWidth)), (int) j * MapConstants.FogOfWarGridHeight, MapConstants.FogOfWarGridWidth, MapConstants.FogOfWarGridHeight),
                        new Rectangle((int) (i * (MapConstants.FogOfWarGridWidth)), (int) j * MapConstants.FogOfWarGridHeight, MapConstants.FogOfWarGridWidth, MapConstants.FogOfWarGridHeight),
                        Color.White,
                        0f, 
                        Vector2.Zero,
                        SpriteEffects.None,
                        LayerConstants.FogOfWarMapLayer
                        
                    );
                    

                    spriteBatch.FillRectangle(
                        new Rectangle(
                            i * MapConstants.FogOfWarGridWidth, 
                            j * MapConstants.FogOfWarGridHeight,
                            MapConstants.FogOfWarGridWidth,
                            MapConstants.FogOfWarGridHeight),
                        new Color(Color.Black, .5f),
                        0,
                        LayerConstants.FogOfWarLayer);
                        
                }

            }
        }

        /// <summary>
        /// Adds a revealing object to the fog of war.
        /// </summary>
        /// <param name="revealingObject">The object which can reveal the fog of war.</param>
        public void AddRevealingObject(IRevealing revealingObject)
        {
            mRevealingObjects.AddLast(revealingObject);
        }

        /// <summary>
        /// Removes a revealing object from the fog of war.
        /// </summary>
        /// <param name="revealingObject">The object which can reveal the fog of war.</param>
        public void RemoveRevealingObject(IRevealing revealingObject)
        {
            mRevealingObjects.Remove(revealingObject);
        }

        public void Update(GameTime gametime)
        {
            // this is definitely not the best solution, the reason for this is that we dont want
            // revealed portions to be revealed forever. So we need to set everything to false again before we reupdate the array.
            mToDraw = new bool[
                (MapConstants.MapWidth / MapConstants.FogOfWarGridWidth),
                (MapConstants.MapHeight / MapConstants.FogOfWarGridHeight)
            ];

            foreach (var revealingObject in mRevealingObjects)
            {
                // first, we check every tile candidate which could be in the inner of the circle presented by the revealing object,
                // where (x, y) is the center point of each tile.
                for (var x = revealingObject.Center.X - revealingObject.RevelationRadius;
                    x <= revealingObject.Center.X + revealingObject.RevelationRadius;
                    x += MapConstants.FogOfWarGridWidth)
                {
                    for (var y = revealingObject.Center.Y - revealingObject.RevelationRadius;
                        y <= revealingObject.Center.Y + revealingObject.RevelationRadius;
                        y += MapConstants.FogOfWarGridHeight)
                    {
                        // Now we check whether the distance of the current point to the center of the circle is smaller than the radius of the circle
                        // if yes -> point is in the circle, if no -> point is out of the circle
                        if (Math.Sqrt(Math.Pow(x - revealingObject.Center.X, 2) +
                                      Math.Pow(y - revealingObject.Center.Y, 2)) > revealingObject.RevelationRadius ||
                            x > MapConstants.MapWidth || x < 0 || y > MapConstants.MapHeight || y < 0)
                        {
                            continue;
                        }

                        mToDraw[(int) (x / MapConstants.FogOfWarGridWidth), (int) (y / MapConstants.FogOfWarGridHeight)] = true;

                    }
                }
            }
        }
    }
}
