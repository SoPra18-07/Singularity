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
    internal sealed class Mine : PlatformBlank
    {
        [DataMember]
        private new const int PlatformWidth = 144;
        [DataMember]
        private new const int PlatformHeight = 187;


        public Mine(Vector2 position,
            Texture2D spritesheet,
            Texture2D basesprite,
            SpriteFont libSans12,
            ResourceMap resource,
            ref Director director,
            bool friendly = true)
            : base(position,
                spritesheet,
                basesprite,
                libSans12,
                ref director,
                EPlatformType.Mine,
                -50,
                friendly)
        {
            mIPlatformActions.Add(new ProduceMineResource(platform: this, resourceMap: resource, director: ref mDirector));
            // Todo: Add Costs of the platform here if you got them.
            // mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Mine;
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
                Produce();
            }
        }
    }
}
