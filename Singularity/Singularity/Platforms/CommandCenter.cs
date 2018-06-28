﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.Platforms
{
    [DataContract()]
    class CommandCenter: PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 200;

        [DataMember]
        private const int PlatformHeight = 233;

        [DataMember]
        private List<GeneralUnit> mControlledUnits;

        public CommandCenter(Vector2 position, Texture2D spritesheet, Texture2D baseSprite, ref Director dir): base(position, spritesheet, baseSprite, ref director, EPlatformType.Command, -50)
        {
            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[2];
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Command;
            mSpritename = "Cylinders";
            SetPlatfromParameters();
            mControlledUnits = new List<GeneralUnit>();
            mDirector.GetStoryManager.AddEnergy(energy: 5);
        }

        public override void Produce()
        {
            throw new NotImplementedException();
            //mIPlatformActions[0].Execute();
        }
    }
}
