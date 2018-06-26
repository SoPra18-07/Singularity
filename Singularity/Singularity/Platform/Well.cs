using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Map;
using Singularity.Resources;

namespace Singularity.Platform
{
    [DataContract]
    internal sealed class Well: PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;

        public Well(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ResourceMap resource): base(position, platformSpriteSheet, baseSprite, new Vector2(position.X + PlatformWidth / 2f, position.Y + PlatformHeight - 36))
        {
            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[2];
            mIPlatformActions[1] = new ProduceWellResource(this, resource);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Well;
            mSpritename = "Dome";
            AbsoluteSize = SetPlatfromDrawParameters();
        }

        public override void Produce()
        {
            mIPlatformActions[1].Execute();
        }
    }
}
