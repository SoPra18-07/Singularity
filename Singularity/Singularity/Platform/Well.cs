﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Map;
using Singularity.Resources;

namespace Singularity.Platform
{
    [DataContract()]
    class Well: PlatformBlank
    {
        [DataMember()]
        private const int PlatformWidth = 144;
        [DataMember()]
        private const int PlatformHeight = 127;

        public Well(Vector2 position, Texture2D spritesheet, ResourceMap resource): base(position, spritesheet, new Vector2(position.X + PlatformWidth / 2f, position.Y + PlatformHeight - 36))
        {
            MiPlatformActions = new IPlatformAction[2];
            //mActions[0] = BuildWellBlueprint(this);
            MiPlatformActions[1] = new ProduceWellResource(this, resource);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            MCost = new Dictionary<EResourceType, int>();
            MType = EPlatformType.Well;
            MSpritename = "Dome";
            AbsoluteSize = new Vector2(PlatformWidth, PlatformHeight);
        }

        public override void Produce()
        {
            MiPlatformActions[1].Execute();
        }
    }
}
