using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Resources;

namespace Singularity.platform
{
    [DataContract()]
    class Junkyard : PlatformBlank
    {
        public Junkyard(Vector2 position, Texture2D spritesheet): base(position, spritesheet)
        {
            //mActions = IPlatformAction[1];
            //mActions[0] = BuildBlueprintJunkyard(this);
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<IResources, int>();
        }

        public void BurnTrash()
        {
            //foreach (var resource in mResources)
            //{
            //if (resource.Type == EResourceType.Trash)
            //{
            //    mResources.Remove(resource);
            //}
            //}
        }
    }
}
