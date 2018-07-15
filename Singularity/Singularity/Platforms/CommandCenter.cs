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
    public sealed class CommandCenter: PlatformBlank
    {
        [DataMember]
        private const int ProvidingEnergy = 20;

        [DataMember]
        private const int PlatformWidth = 200;

        [DataMember]
        private const int PlatformHeight = 233;

        [DataMember]
        private List<GeneralUnit> mControlledUnits;

        public CommandCenter(Vector2 position,
            Texture2D spritesheet,
            Texture2D baseSprite,
            SpriteFont libSans12,
            ref Director director,
            bool blueprintState = true,
            bool friendly = true)
             : base(position,
                spritesheet,
                baseSprite,
                libSans12,
                ref director,
                EStructureType.Command,
                -50, friendly)
        {
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.

            mProvidingEnergy = ProvidingEnergy;

            mCost = new Dictionary<EResourceType, int>();
            mType = EStructureType.Command;
            mSpritename = "Cylinders";
            SetPlatfromParameters();
            mControlledUnits = new List<GeneralUnit>();
            mIsBlueprint = blueprintState;
            mPlatformWidth = 200;
            mPlatformHeight = 233;
        }

        public override void Produce()
        {
            throw new NotImplementedException();
            //mIPlatformActions[0].Execute();
        }
    }
}
