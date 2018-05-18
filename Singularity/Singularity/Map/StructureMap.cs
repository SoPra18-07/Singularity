using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Map
{
    internal sealed class StructureMap : IDraw, IUpdate
    {

        //private readonly LinkedList<PlatformBlank> mPlatforms;
        //private readonly LinkedList<Pair<PlatformBlank, PlatformBlank>> mRoads;

        public StructureMap()
        {
            //mPlatforms = new LinkedList<PlatformBlank>();
            //mRoads = new LinkedList<Pair<PlatformBlank, PlatformBlank>>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the platforms / roads
        }

        public void Update(GameTime gameTime)
        {
            //Update the platforms / roads
        }

        /*
        public void AddPlatform(PlatformBlank platform)
        {
            mPlatforms.AddLast(platform);
        }

        public void RemovePlatform(PlatformBlank platform)
        {
            mPlatforms.AddLast(platform);
        }
        */
    }
}
