using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;
using Singularity.Resources;

namespace Singularity.Platform
{
    [DataContract()]
    class Junkyard : PlatformBlank, IRevealing
    {
        [DataMember()]
        private const int PlatformWidth = 144;
        [DataMember()]
        private const int PlatformHeight = 127;
        public new Vector2 Center => new Vector2(AbsolutePosition.X + PlatformWidth / 2, AbsolutePosition.Y + PlatformHeight - 36);
        public Junkyard(Vector2 position, Texture2D spritesheet): base(position, spritesheet)
        {
            mIPlatformActions = new IPlatformAction[1];
            //mActions[0] = BuildBlueprintJunkyard(this);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Junkyard;
            mSpritename = "Dome";
            AbsoluteSize = new Vector2(PlatformWidth, PlatformHeight);
        }

        public void BurnTrash()
        {
            foreach (var resource in mResources)
            {
                if (resource.Type == EResourceType.Trash)
                {
                    mResources.Remove(resource);
                }
            }
        }
    }
}
