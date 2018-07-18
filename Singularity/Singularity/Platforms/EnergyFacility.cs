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
        [DataMember]
        private const int ProvidingEnergy = 20;

        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;

        public EnergyFacility(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            SpriteFont libSans12,
            ref Director director,
            bool friendly = true)
            : base(position,
                platformSpriteSheet,
                baseSprite,
                libSans12,
                ref director,
                EStructureType.Energy,
                -50,
                friendly)
        {
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Something like "Hello InstanceThatManagesEnergyLevels I exist now(Myself)"
            mCost = GetResourceCosts(EStructureType.Energy);
            mType = EStructureType.Energy;
            mSpritename = "Dome";
            mProvidingEnergy = ProvidingEnergy;

            SetPlatfromParameters();
        }
    }
}
