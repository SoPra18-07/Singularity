using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Resources;

namespace Singularity.platform
{
    class Well: PlatformBlank
    {
        public Well(Vector2 position, Texture2D spritesheet): base(position, spritesheet)
        {
            //mActions = Action[];
            //mActions[0] = ;
            //mActions[1] = ;
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<IResources, int>();
        }
    }
}
