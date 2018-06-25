using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Map;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.Platform
{
    [DataContract()]
    class Well: PlatformBlank, IRevealing
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;
        [DataMember]
        private DistributionManager.DistributionManager mDist;

        public Well(Vector2 position, Texture2D spritesheet, ResourceMap resource, DistributionManager.DistributionManager dist) : base(position, spritesheet, new Vector2(position.X + PlatformWidth / 2f, position.Y + PlatformHeight - 36))
        {
            mDist = dist;
            dist.Register(this, false);
            mIPlatformActions = new IPlatformAction[2];
            //mActions[0] = BuildWellBlueprint(this);
            mIPlatformActions[1] = new ProduceWellResource(this, resource);
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Well;
            mSpritename = "Dome";
            AbsoluteSize = new Vector2(PlatformWidth, PlatformHeight);
        }

        public void Produce()
        {
            for (var i = 0; i < mAssignedUnits.Count; i++)
            {
                mIPlatformActions[1].Execute();
            }
        }

        public new void Update(GameTime time)
        {
            base.Update(time);
            if (time.TotalGameTime.TotalSeconds % 5 <= 0.5)
            {
                Console.Out.WriteLine("PRODUCE!!!!");
                Produce();
            }
        }
    }
}
