using System.Collections.Generic;
using System.Diagnostics;
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
    sealed class Quarry : PlatformBlank
    {
        public Quarry(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            SpriteFont libSans12,
            ResourceMap resource,
            ref Director director,
            bool friendly = true)
            : base(position,
                platformSpriteSheet,
                baseSprite,
                libSans12,
                ref director,
                EStructureType.Quarry,
                -50,
                friendly)
        {

            // Add possible Actions in this List
            mIPlatformActions.Add(new ProduceQuarryResource(platform: this, resourceMap: resource, director: ref mDirector));
            // Todo: Add Costs of the platform here if you got them.
            // mCost = new Dictionary<EResourceType, int>();
            mType = EStructureType.Quarry;
            mSpritename = "Dome";
            Property = JobType.Production;
            SetPlatfromParameters();
            mPlatformWidth = 144;
            mPlatformHeight = 127;
        }

        public override void Produce()
        {
            mIPlatformActions[0].Execute();
        }
    }
}
