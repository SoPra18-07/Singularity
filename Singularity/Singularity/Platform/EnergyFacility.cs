using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public EnergyFacility(Vector2 position, Texture2D spritesheet): base(position, spritesheet, new Vector2(position.X + PlatformWidth / 2f, position.Y + PlatformHeight - 36))
        {
            MiPlatformActions = new IPlatformAction[2];
            //mActions[0] = BuildPlatformBlueprint(this);
            //mActions[1] = ;
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Something like "Hello InstanceThatManagesEnergyLevels I exist now(Myself)"
            //Add Costs of the platform here if you got them.
            MCost = new Dictionary<EResourceType, int>();
            MType = EPlatformType.Energy;
            MSpritename = "Dome";
            AbsoluteSize = new Vector2(PlatformWidth, PlatformHeight);
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
