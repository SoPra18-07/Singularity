using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Map.Properties;
using Singularity.Platform;
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

            mCamera = new Camera(viewport, 0, 300);

            mCollisionMap = new CollisionMap();
            mStructureMap = new StructureMap();
            mResourceMap = new ResourceMap(initialResources);
        }

        /// <see cref="CollisionMap.UpdateCollider(Vector2, int)"/>
        public void UpdateCollider(Vector2 coordinates, int id)
        {
            mCollisionMap.UpdateCollider(coordinates, id);
        }

        public void Draw(SpriteBatch spriteBatch)
        {   

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTransform());

            //draw the background texture
            spriteBatch.Draw(mBackgroundTexture,
                new Rectangle(0, 0, MapConstants.MapWidth, MapConstants.MapHeight),
                Color.White);

            mStructureMap.Draw(spriteBatch);


            //make sure to only draw the grid if a texture is given.
            if (mDebugLine == null)
            {
                spriteBatch.End();
                return;
            }
            //draw the collision map grid.
            var collisonMap = mCollisionMap.GetCollisionMap();

            for (var columnCount = 0; columnCount <= collisonMap.GetLength(0); columnCount++)
            {
                spriteBatch.Draw(mDebugLine,
                    new Rectangle(columnCount * MapConstants.GridWidth, 0, 1, MapConstants.MapHeight),
                    Color.Yellow);
            }

            for (var rowCount = 0; rowCount <= collisonMap.GetLength(0); rowCount++)
            {
                spriteBatch.Draw(mDebugLine,
                    new Rectangle(0, rowCount * MapConstants.GridHeight, MapConstants.MapWidth, 1),
                    Color.Cyan);
            }



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
            mStructureMap.Update(gametime);
        }

        /// <see cref="StructureMap.AddPlatform(PlatformBlank)"/>
        public void AddPlatform(PlatformBlank platform)
        {
            mStructureMap.AddPlatform(platform);
        }

        /// <see cref="StructureMap.RemovePlatform(PlatformBlank)"/>
        public void RemovePlatform(PlatformBlank platform)
        {
            mStructureMap.RemovePlatform(platform);
        }
    }
}
