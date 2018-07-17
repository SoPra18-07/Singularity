using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Resources;

namespace Singularity.Platforms
{
    [DataContract]
    class Junkyard : PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;

        public Junkyard(Vector2 position,
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
                EStructureType.Junkyard,
                -50,
                friendly: friendly)
        {
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EStructureType.Junkyard;
            mSpritename = "Dome";
            SetPlatfromParameters();
        }

        private void BurnTrash()
        {
            var resourcecopy = new List<Resource>();
            resourcecopy.AddRange(mResources);
            foreach (var resource in resourcecopy)
            {
                if (resource.Type == EResourceType.Trash)
                {
                    mResources.Remove(resource);
                    mDirector.GetStoryManager.Trash();
                }
            }
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
            if (mIsBlueprint)
            {
                return;
            }
            //Destroy the trash in the same rythm as other platforms would produce
            if (!(mDirector.GetClock.GetProduceTicker().Seconds > 4))
            {
                return;
            }
            BurnTrash();
            //Ask for Trash two times;
            mDirector.GetDistributionDirector.GetManager(GetGraphIndex()).RequestResource(this, EResourceType.Trash, null, false);
        }
    }
}
