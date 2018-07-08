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
    class Mine : PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 187;


        public Mine(Vector2 position, Texture2D spritesheet, Texture2D basesprite, ResourceMap resource, SpriteFont libSans12, ref Director director, bool autoRegister = true) : base(position: position, platformSpriteSheet: spritesheet, baseSprite: basesprite, libSans12Font: libSans12, director: ref director, type: EPlatformType.Mine, centerOffsetY: -50)
        {
            if (autoRegister)
            {
                director.GetDistributionManager.Register(platform: this, isDef: false);
            }

            mIPlatformActions.Add(item: new ProduceMineResource(platform: this, resourceMap: resource, director: ref mDirector));
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Mine;
            mSpritename = "Dome";
            Property = JobType.Production;
            SetPlatfromParameters();
        }

        public override void Produce()
        {
            foreach (var pair in mAssignedUnits[key: JobType.Production])
            {
                //That means the unit is not at work yet.
                if (!pair.GetSecond())
                {
                    continue;
                }
                mIPlatformActions[index: 1].Execute();
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
