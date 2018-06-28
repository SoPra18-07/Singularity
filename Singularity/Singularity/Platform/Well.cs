using System;
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
    internal sealed class Well: PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;
        [DataMember]
        private Director mDirector;

        public Well(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ResourceMap resource, ref Director dir, bool autoRegister = true) : base(position, platformSpriteSheet, baseSprite, ref dir, EPlatformType.Well, -50)
        {
            mDirector = dir;
            if (autoRegister)
            {
                dir.GetDistributionManager.Register(this, false);
            }

            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[2];
            mIPlatformActions[1] = new ProduceWellResource(this, resource);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Well;
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
                Console.Out.WriteLine("PRODUCE!!!!");
                Produce();
            }
        }
    }
}
