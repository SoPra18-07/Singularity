using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.PlatformActions;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.Platforms
{
    [DataContract]
    internal sealed class Well: PlatformBlank
    {
        [DataMember]
        private new const int PlatformWidth = 144;
        [DataMember]
        private new const int PlatformHeight = 127;

        public Well(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            ResourceMap resource,
            ref Director director,
            bool autoRegister = true,
            bool friendly = true)
            : base(position,
                platformSpriteSheet,
                baseSprite,
                ref director,
                EPlatformType.Well,
                -50,
                friendly: friendly)
        {
            if (autoRegister)
            {
                director.GetDistributionManager.Register(this, false);
            }

            //Add possible Actions in this array
            mIPlatformActions.Add(new ProduceWellResource(platform: this, resourceMap: resource, director: ref mDirector));
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Well;
            mSpritename = "Dome";
            Property = JobType.Production;
            SetPlatfromParameters();
        }

        public override void Produce()
        {
            foreach (var pair in mAssignedUnits[JobType.Production])
            {
                //That means the unit is not at work yet.
                if (!pair.GetSecond())
                {
                    continue;
                }
                mIPlatformActions[1].Execute();
            }
        }

        public new void Update(GameTime time)
        {
            base.Update(time);
            if (time.TotalGameTime.TotalSeconds % 5 <= 0.5)
            {
                Console.Out.WriteLine("PRODUCE!!!!");
                Produce();
            }
        }
    }
}
