using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Map;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.Platform
{
    [DataContract()]
    class Well: PlatformBlank, IRevealing
    {
        [DataMember()]
        private const int PlatformWidth = 144;
        [DataMember()]
        private const int PlatformHeight = 127;

        public Vector2 Center => new Vector2(AbsolutePosition.X + PlatformWidth / 2, AbsolutePosition.Y + PlatformHeight - 36);

        public Well(Vector2 position, Texture2D spritesheet, ResourceMap resource): base(position, spritesheet)
        {
            mIPlatformActions = new IPlatformAction[2];
            //mActions[0] = BuildWellBlueprint(this);
            mIPlatformActions[1] = new ProduceWellResource(this, resource);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Well;
            mSpritename = "Dome";
            AbsoluteSize = new Vector2(PlatformWidth, PlatformHeight);
        }

        public override void Produce()
        {
            mIPlatformActions[1].Execute();
        }
    }
}
