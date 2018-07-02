﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.PlatformActions;
using Singularity.Manager;
using Singularity.Resources;

namespace Singularity.Platforms
{
    [DataContract()]
    class Factory: PlatformBlank
    {
        [DataMember()]
        private const int PlatformWidth = 144;
        [DataMember()]
        private const int PlatformHeight = 127;

        public Factory(Vector2 position, Texture2D spritesheet, Texture2D basesprite, ref Director dir): base(position, spritesheet, basesprite, ref dir, EPlatformType.Factory, -12)
        {
            //mIPlatformActions[0] = new ProduceFactoryResource(this);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Factory;
            mSpritename = "Dome";
            SetPlatfromParameters();
        }

        public override void Produce()
        {
            throw new NotImplementedException();
            //mIPlatformActions[0].Execute();
        }
    }
}