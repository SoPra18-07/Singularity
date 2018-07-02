﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.PlatformActions;
using Singularity.Manager;
using Singularity.Resources;

namespace Singularity.Platforms
{
    [DataContract]
    internal sealed class EnergyFacility : PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;

        public EnergyFacility(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ref Director dir) : base(position, platformSpriteSheet, baseSprite , ref dir, EPlatformType.Energy, -50)
        {
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Something like "Hello InstanceThatManagesEnergyLevels I exist now(Myself)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Energy;
            mSpritename = "Dome";
            SetPlatfromParameters();
        }

        public void TurnOn(StoryManager story)
        {
            story.AddEnergy(20);
        }

        public void TurnOff(StoryManager story)
        {
            story.AddEnergy(-20);
        }
    }
}