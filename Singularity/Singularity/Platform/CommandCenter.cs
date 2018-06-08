using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.Platform
{
    [DataContract()]
    class CommandCenter: PlatformBlank
    {
        [DataMember()]
        private const int PlatformWidth = 200;

        [DataMember()]
        private const int PlatformHeight = 233;

        [DataMember()]
        private List<GeneralUnit> mControlledUnits;

        public CommandCenter(Vector2 position, Texture2D spritesheet, StoryManager.StoryManager story): base(position, spritesheet)
        {
            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[2];
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Command;
            mSpritename = "Cylinders";
            AbsoluteSize = new Vector2(PlatformWidth, PlatformHeight);
            mControlledUnits = new List<GeneralUnit>();
            story.AddEnergy(5);
        }

        public override void Produce()
        {
            throw new NotImplementedException();
            //mIPlatformActions[0].Execute();
        }
    }
}
