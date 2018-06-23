using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;
using Singularity.Resources;

namespace Singularity.Platform
{
    [DataContract()]
    class EnergyFacility : PlatformBlank
    {
        [DataMember()]
        private const int PlatformWidth = 144;
        [DataMember()]
        private const int PlatformHeight = 127;

        public EnergyFacility(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite) : base(position, platformSpriteSheet, baseSprite , new Vector2(position.X + PlatformWidth / 2f, position.Y + PlatformHeight - 36))
        {
            mIPlatformActions = new IPlatformAction[2];
            //mActions[0] = BuildPlatformBlueprint(this);
            //mActions[1] = ;
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Something like "Hello InstanceThatManagesEnergyLevels I exist now(Myself)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Energy;
            mSpritename = "Dome";
            AbsoluteSize = SetPlatfromDrawParameters();
        }

        public void TurnOn(StoryManager.StoryManager story)
        {
            story.AddEnergy(20);
        }

        public void TurnOff(StoryManager.StoryManager story)
        {
            story.AddEnergy(-20);
        }
    }
}
