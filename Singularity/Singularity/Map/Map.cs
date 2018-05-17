using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Map.Properties;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Map
{
    internal sealed class Map : IDraw, IUpdate
    {
        private readonly CollisionMap mCollisionMap;
        private readonly Camera mCamera;

        private readonly Texture2D mBackgroundTexture;
        private readonly Texture2D mDebugLine;

        public Map(Texture2D backgroundTexture, Viewport viewport) : this(backgroundTexture, null, viewport)
        {

        }

        public Map(Texture2D backgroundTexture, Texture2D debugLine, Viewport viewport)
        {
            mBackgroundTexture = backgroundTexture;
            mDebugLine = debugLine;

            mCollisionMap = new CollisionMap();
            mCamera = new Camera(viewport, 0, 300);
        }

        public void UpdateCollider(Vector2 coordinates, int id)
        {
            mCollisionMap.UpdateCollider(coordinates, id);
        }

        public void Draw(SpriteBatch spriteBatch)
        {   
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTransform());
            spriteBatch.Draw(mBackgroundTexture,
                new Rectangle(0, 0, MapConstants.MapWidth, MapConstants.MapHeight),
                Color.White);


            if (mDebugLine == null)
            {
                return;
            }
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
