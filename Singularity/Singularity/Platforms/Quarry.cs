using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.PlatformActions;
using Singularity.Resources;

namespace Singularity.Platforms
{
    [DataContract]
    class Quarry : PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;

        public Quarry(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ResourceMap resource, ref Director director): base(position, platformSpriteSheet, baseSprite, ref director, EPlatformType.Quarry, -50)
        {
            director.GetDistributionManager.Register(platform: this, isDef: false);
            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[2];
            mIPlatformActions[0] = new ProduceQuarryResource(platform: this, resourceMap: resource);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Quarry;
            mSpritename = "Dome";
            SetPlatfromParameters();
        }

        public override void Produce()
        {
            for (var i = 0; i < mAssignedUnits.Count; i++)
            {
                mIPlatformActions[1].Execute();
            }
        }

        public new void Update(GameTime time)
        {
            base.Update(t: time);
            if (time.TotalGameTime.TotalSeconds % 5 <= 0.5)
            {
                Produce();
            }
        }
    }
}
