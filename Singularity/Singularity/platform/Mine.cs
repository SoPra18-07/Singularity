using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Resources;

namespace Singularity.platform
{
    [DataContract()]
    class Mine : PlatformBlank
    {
        public Mine(Vector2 position, Texture2D spritesheet): base(position, spritesheet)
        {
            //mActions = IPlatformAction[];
            //mActions[0] = ProduceQuarryResource(this);
            //mActions[1] = ;
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<IResources, int>();
        }

        public override void Produce()
        {
            throw new NotImplementedException();
            //foreach (var pair in mAssignedUnits)
            //{
            //if (pair.Value.GetJobType() == JobType.Production)
            //{
            //pair.Key.Produce();
            //}

            //}
        }
    }
}
