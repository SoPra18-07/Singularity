using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.PlatformActions;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.Platforms
{
    [DataContract]
    internal sealed class Mine : PlatformBlank
    {
        public Mine(Vector2 position,
            Texture2D spritesheet,
            Texture2D basesprite,
            SpriteFont libSans12,
            ResourceMap resource,
            ref Director director,
            bool friendly = true)
            : base(position,
                spritesheet,
                basesprite,
                libSans12,
                ref director,
                EStructureType.Mine,
                -50,
                friendly)
        {
            mIPlatformActions.Add(new ProduceMineResource(platform: this, resourceMap: resource, director: ref mDirector));
            // Todo: Add Costs of the platform here if you got them.
            // mCost = new Dictionary<EResourceType, int>();
            mCost = new Dictionary<EResourceType, int>();
            mType = EStructureType.Mine;
            mSpritename = "Dome";
            Property = JobType.Production;
            SetPlatfromParameters();
            mPlatformWidth = 144;
            mPlatformHeight = 187;
        }

        public override void Produce()
        {
            mIPlatformActions[0].Execute();
        }
    }
}
