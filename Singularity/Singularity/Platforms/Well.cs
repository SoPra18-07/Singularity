using System;
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
    internal sealed class Well: PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;

        public Well(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ResourceMap resource, ref Director director)
            : base(position: position, platformSpriteSheet: platformSpriteSheet, baseSprite: baseSprite, director: ref director, center: new Vector2(x: position.X + PlatformWidth / 2f, y: position.Y + PlatformHeight - 36))
        {
            director.GetDistributionManager.Register(platform: this, isDef: false);
            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[2];
            mIPlatformActions[1] = new ProduceWellResource(platform: this, resourceMap: resource, director: ref mDirector);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Well;
            mSpritename = "Dome";
            AbsoluteSize = SetPlatfromDrawParameters();
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
                Console.Out.WriteLine(value: "PRODUCE!!!!");
                Produce();
            }
        }
    }
}
