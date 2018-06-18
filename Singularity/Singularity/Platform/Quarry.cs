﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Map;
using Singularity.Resources;

namespace Singularity.Platform
{
    [DataContract()]
    class Quarry : PlatformBlank
    {
        [DataMember()]
        private const int PlatformWidth = 144;
        [DataMember()]
        private const int PlatformHeight = 127;

        public Quarry(Vector2 position, Texture2D spritesheet, ResourceMap resource): base(position, spritesheet, new Vector2(position.X + PlatformWidth / 2f, position.Y + PlatformHeight - 36))
        {
            MiPlatformActions = new IPlatformAction[2];
            //mActions[0] = BuildQuarryBlueprint(this);
            MiPlatformActions[1] = new ProduceQuarryResource(this, resource);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            MCost = new Dictionary<EResourceType, int>();
            MType = EPlatformType.Quarry;
            MSpritename = "Dome";
            AbsoluteSize = new Vector2(PlatformWidth, PlatformHeight);
        }

        public override void Produce()
        {
            MiPlatformActions[1].Execute();
        }
    }
}
