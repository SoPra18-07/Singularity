using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Map.Properties;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Map
{
    internal sealed class FogOfWar : IDraw, IUpdate
    {

        private static readonly Vector2 sDefaultSize = new Vector2(500, 500);

        private readonly bool[,] mToDraw;

        private readonly CollisionMap mCollMap;
        private readonly StructureMap mStructMap;

        public FogOfWar(Map map)
        { 
            mCollMap = map.GetCollisionMap();
            mStructMap = map.GetStructureMap();

            // make sure the resolution of the fog of war is as dense as the collision map
            mToDraw = new bool[mCollMap.GetCollisionMap().GetLength(0), mCollMap.GetCollisionMap().GetLength(1)];
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < mToDraw.GetLength(0); i++)
            {
                for (var j = 0; j < mToDraw.GetLength(1); j++)
                {
                    if (mToDraw[i, j] == true)
                    {
                        continue;
                    }

                    spriteBatch.FillRectangle(
                        new Rectangle(
                            i * MapConstants.GridWidth, 
                            j * MapConstants.GridHeight,
                            MapConstants.GridWidth, 
                            MapConstants.GridHeight), 
                        new Color(Color.Black, 0.5f),
                        0,
                        LayerConstants.FogOfWarLayer);
                }
            }
        }

        public void Update(GameTime gametime)
        {
            //Used to update the ToDraw array with accurate values.
            foreach (var platform in mStructMap.GetPlatforms())
            {
                var center = Geometry.GetCenter(platform);

                for (var y = center.Y - (sDefaultSize.Y / 2);
                    y <= center.Y + (sDefaultSize.Y / 2);
                    y += MapConstants.GridHeight)
                {
                    for (var x = center.X - (sDefaultSize.X / 2);
                        x <= center.X + (sDefaultSize.X / 2);
                        x += MapConstants.GridWidth)
                    {
                        mToDraw[(int) x / MapConstants.GridWidth, (int) y / MapConstants.GridHeight] = true;
                    }
                }
            }
        }

        public bool IsConcealed(ISpatial spatial)
        {
            return !mToDraw[(int) spatial.AbsolutePosition.X / MapConstants.GridWidth, (int) spatial.AbsolutePosition.Y / MapConstants.GridHeight] &&
                   !mToDraw[(int) (spatial.AbsolutePosition.X + spatial.AbsoluteSize.X) / MapConstants.GridWidth, (int) spatial.AbsolutePosition.Y / MapConstants.GridHeight] &&
                   !mToDraw[(int) spatial.AbsolutePosition.X / MapConstants.GridWidth, (int) (spatial.AbsolutePosition.Y + spatial.AbsoluteSize.Y) / MapConstants.GridHeight] &&
                   !mToDraw[(int) (spatial.AbsolutePosition.X + spatial.AbsoluteSize.X) / MapConstants.GridWidth, (int) (spatial.AbsolutePosition.Y + spatial.AbsoluteSize.Y) / MapConstants.GridHeight];

        }
    }
}
