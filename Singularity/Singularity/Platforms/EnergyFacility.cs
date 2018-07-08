using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Resources;

namespace Singularity.Platforms
{
    [DataContract]
    internal sealed class EnergyFacility : PlatformBlank
    {
        private const int ProvidingEnergy = 20;

        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;

        public EnergyFacility(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, SpriteFont libSans12, ref Director director) : base(position, platformSpriteSheet, baseSprite, libSans12, ref director, EPlatformType.Energy, -50)
        {
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Something like "Hello InstanceThatManagesEnergyLevels I exist now(Myself)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Energy;
            mSpritename = "Dome";
            mProvidingEnergy = ProvidingEnergy;

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
