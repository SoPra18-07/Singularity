using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;

namespace Singularity.Map
{
    internal sealed class FogOfWar : IDraw
    {

        private readonly CollisionMap mCollMap;
        private readonly StructureMap mStructMap;

        public FogOfWar(Map map)
        {
            mCollMap = map.GetCollisionMap();
            mStructMap = map.GetStructureMap();
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}
