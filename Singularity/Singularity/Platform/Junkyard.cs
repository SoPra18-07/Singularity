using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Resources;

namespace Singularity.Platform
{
    [DataContract]
    class Junkyard : PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;
        [DataMember]
        private Director mDirector;

        public Junkyard(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ref Director dir) : base(position, platformSpriteSheet, baseSprite, ref dir, -12)
        {
            mDirector = dir;
            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[1];
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Junkyard;
            mSpritename = "Dome";
            SetPlatfromParameters();
        }

        public void BurnTrash()
        {
            foreach (var resource in mResources)
            {
                if (resource.Type == EResourceType.Trash)
                {
                    mResources.Remove(resource);
                    mDirector.GetStoryManager.Trash();
                }
            }
        }
    }
}
