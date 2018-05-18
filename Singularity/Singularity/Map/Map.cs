using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Map.Properties;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Utils;

namespace Singularity.Map
{   
    internal sealed class Map : IDraw, IUpdate
    {   
        private readonly CollisionMap mCollisionMap;
        private readonly StructureMap mStructureMap;
        private readonly ResourceMap mResourceMap;

        private readonly Camera mCamera;

        private readonly Texture2D mBackgroundTexture;
        private readonly Texture2D mDebugLine;

        /// <summary>
        /// Creates a new Map object, which solely draws its background
        /// texture and if debugLine is given the grid. If initialResources isn't
        /// further specified there will be no resources on the map.
        /// </summary>
        /// <param name="backgroundTexture">The background texture of the map</param>
        /// <param name="viewport">The viewport of the window</param>
        /// <param name="debugLine">The texture used to draw the lines for the grid, if not specified the grid won't be drawn</param>
        /// <param name="initialResources">The initial resources of this map, if not specified there will not be any on the map</param>
        public Map(Texture2D backgroundTexture, Viewport viewport, Texture2D debugLine = null, IDictionary<Vector2, Pair<EResourceType, int>> initialResources = null)
        {
            mBackgroundTexture = backgroundTexture;
            mDebugLine = debugLine;

            mCollisionMap = new CollisionMap();
            mStructureMap = new StructureMap();
            mResourceMap = new ResourceMap(initialResources);

            mCamera = new Camera(viewport, 0, 300);
        }

        /// <see cref="CollisionMap.UpdateCollider(Vector2, int)"/>
        public void UpdateCollider(Vector2 coordinates, int id)
        {
            mCollisionMap.UpdateCollider(coordinates, id);
        }

        public void Draw(SpriteBatch spriteBatch)
        {   
            spriteBatch.Begin(
                //SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTransform()
                );

            var position = Vector2.Transform(new Vector2(0, 0), mCamera.GetTransform());

            //draw the background texture
            spriteBatch.Draw(mBackgroundTexture,
                new Rectangle((int) position.X, (int) position.Y, (int) (MapConstants.MapWidth * mCamera.GetZoom()),  (int) (MapConstants.MapHeight * mCamera.GetZoom())),
                Color.White);

            //pass the draw method to the structure map so it can draw all the structures in the game
            mStructureMap.Draw(spriteBatch);

            //make sure to only draw the grid if a texture is given.
            if (mDebugLine == null)
            {
                return;
            }
            //TODO: actually draw the grid.
            //draw the collision map grid.
            var collisonMap = mCollisionMap.GetCollisionMap();
            

            for (var i = 0; i < collisonMap.GetLength(0); i++)
            {
                for (var j = 0; j < collisonMap.GetLength(1); j++)
                {
                    if (!collisonMap[i, j].IsPresent())
                    {
                        continue;
                    }

                    var coords = collisonMap[i, j].Get().GetFirst();
                    spriteBatch.Draw(mDebugLine, new Rectangle((int)coords.X, (int)coords.Y, 2, 2), Color.Yellow);

                }
            }
            spriteBatch.End();
        }
        //TODO: remove if input manager is available since we only use this to pass an update to the camera.
        public void Update(GameTime gametime)
        {
            mCamera.Update(gametime);
        }
    }
}
