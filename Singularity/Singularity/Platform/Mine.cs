﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Resources;

namespace Singularity.Platform
{
    [DataContract]
    class Mine : PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;

        [DataMember]
        private Director mDirector;

        public Mine(Vector2 position, Texture2D spritesheet, Texture2D basesprite, ResourceMap resource, ref Director dir) : base(position, spritesheet, basesprite, new Vector2(position.X + PlatformWidth / 2f, position.Y + PlatformHeight - 36))
        {
            mDirector = dir;
            dir.GetDistributionManager.Register(this, false);

            mIPlatformActions = new IPlatformAction[2];
            mIPlatformActions[0] = new ProduceMineResource(this, resource);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Mine;
            mSpritename = "Dome";
            AbsoluteSize = new Vector2(PlatformWidth, PlatformHeight);
        }

        public void Produce()
        {
            for (var i = 0; i < mAssignedUnits.Count; i++)
            {
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
