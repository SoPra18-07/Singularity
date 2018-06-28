using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.Platform
{
    [DataContract]
    class Mine : PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 187;

        [DataMember]
        private Director mDirector;

        public Mine(Vector2 position, Texture2D spritesheet, Texture2D basesprite, ResourceMap resource, ref Director dir, bool autoRegister = true) : base(position, spritesheet, basesprite, ref dir, EPlatformType.Mine, -50)
        {
            mDirector = dir;
            if (autoRegister)
            {
                dir.GetDistributionManager.Register(this, false);
            }

            mIPlatformActions = new IPlatformAction[2];
            mIPlatformActions[0] = new ProduceMineResource(this, resource);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Mine;
            mSpritename = "Dome";
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
