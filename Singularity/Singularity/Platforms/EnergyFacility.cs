﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;
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

        public EnergyFacility(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ref Director director) 
            : base(position: position, platformSpriteSheet: platformSpriteSheet, baseSprite: baseSprite , director: ref director, center: new Vector2(x: position.X + PlatformWidth / 2f, y: position.Y + PlatformHeight - 36))
        {
            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[2];
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Something like "Hello InstanceThatManagesEnergyLevels I exist now(Myself)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Energy;
            mSpritename = "Dome";
            AbsoluteSize = SetPlatfromDrawParameters();
        }

        public void TurnOn(StoryManager story)
        {
            story.AddEnergy(energy: 20);
        }

        public void TurnOff(StoryManager story)
        {
            story.AddEnergy(energy: -20);
        }
    }
}
