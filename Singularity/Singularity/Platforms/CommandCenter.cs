using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.Platforms
{
    [DataContract]
    class CommandCenter: PlatformBlank
    {
        private const int ProvidingEnergy = 20;

        [DataMember]
        private const int PlatformWidth = 200;

        [DataMember]
        private const int PlatformHeight = 233;

        [DataMember]
        private List<GeneralUnit> mControlledUnits;

        public CommandCenter(Vector2 position, Texture2D spritesheet, Texture2D baseSprite, SpriteFont libSans12, ref Director director, bool blueprintState=true): base(position, spritesheet, baseSprite, libSans12, ref director, EPlatformType.Command, -50)
        {
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.

            mProvidingEnergy = ProvidingEnergy;

            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Command;
            mSpritename = "Cylinders";
            SetPlatfromParameters();
            mControlledUnits = new List<GeneralUnit>();
            director.GetStoryManager.AddEnergy(5);
            mIsBlueprint = blueprintState;
        }

        public override void Produce()
        {
            throw new NotImplementedException();
            //mIPlatformActions[0].Execute();
        }
    }
}
