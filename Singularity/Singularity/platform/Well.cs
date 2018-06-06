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

namespace Singularity.platform
{
    [DataContract()]
    class Well: PlatformBlank
    {
        [DataMember()]
        private const int PlatformWidth = 144;
        [DataMember()]
        private const int PlatformHeight = 127;

        public Well(Vector2 position, Texture2D spritesheet): base(position, spritesheet)
        {
            //mActions = IPlatformAction[2];
            //mActions[0] = BuildWellBlueprint(this);
            //mActions[1] = ProduceWellResource(this);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<IResource, int>();
        }

        public override void Produce()
        {
            throw new NotImplementedException();
            //mActions[1].Execute();
        }
    }
}
