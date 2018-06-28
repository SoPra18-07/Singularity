using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;
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

        public Junkyard(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ref Director director)
            : base(position: position, platformSpriteSheet: platformSpriteSheet, baseSprite: baseSprite, director: ref director, center: new Vector2(x: position.X + PlatformWidth / 2f, y: position.Y + PlatformHeight - 36))
        {
            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[1];
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Junkyard;
            mSpritename = "Dome";
            AbsoluteSize = SetPlatfromDrawParameters();
        }

        public void BurnTrash()
        {
            foreach (var resource in mResources)
            {
                if (resource.Type == EResourceType.Trash)
                {
                    mResources.Remove(item: resource);
                    mDirector.GetStoryManager.Trash();
                }
            }
        }
    }
}
