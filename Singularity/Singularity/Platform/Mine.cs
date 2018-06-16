using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Map;
using Singularity.Property;
using Singularity.Resources;

namespace Singularity.Platform
{
    [DataContract()]
    class Mine : PlatformBlank, IRevealing
    {
        [DataMember()]
        private const int PlatformWidth = 144;
        [DataMember()]
        private const int PlatformHeight = 127;

        public Mine(Vector2 position, Texture2D spritesheet, ResourceMap resource): base(position, spritesheet, new Vector2(position.X + PlatformWidth / 2f, position.Y + PlatformHeight - 36))
        {
            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[2];
            mIPlatformActions[0] = new ProduceMineResource(this, resource);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Mine;
            mSpritename = "Dome";
            AbsoluteSize = new Vector2(PlatformWidth, PlatformHeight);
        }

        public override void Produce()
        {
            mIPlatformActions[1].Execute();
        }
    }
}
