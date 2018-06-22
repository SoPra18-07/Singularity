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

namespace Singularity.Platform
{
    [DataContract()]
    class Quarry : PlatformBlank, IRevealing
    {
        [DataMember()]
        private const int PlatformWidth = 144;
        [DataMember()]
        private const int PlatformHeight = 127;

        public Quarry(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ResourceMap resource): base(position, platformSpriteSheet, baseSprite, new Vector2(position.X + PlatformWidth / 2f, position.Y + PlatformHeight - 36))
        {
            mIPlatformActions = new IPlatformAction[2];
            //mActions[0] = BuildQuarryBlueprint(this);
            mIPlatformActions[1] = new ProduceQuarryResource(this, resource);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Quarry;
            mSpritename = "Dome";
            AbsoluteSize = SetPlatfromDrawParameters();
        }

        public override void Produce()
        {
            mIPlatformActions[1].Execute();
        }
    }
}
