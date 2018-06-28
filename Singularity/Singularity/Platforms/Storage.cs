using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;
using Singularity.Resources;

namespace Singularity.Platforms
{
    [DataContract()]
    class Storage: PlatformBlank
    {
        [DataMember()]
        private const int PlatformWidth = 144;
        [DataMember()]
        private const int PlatformHeight = 127;

        public Storage(Vector2 position, Texture2D spritesheet, Texture2D basesprite, ref Director director): base(position, spritesheet, basesprite, director, EPlatformType.Storage, -50)
        {
            //Add possible Actions in this array
            mIPlatformActions = new IPlatformAction[2];
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mType = EPlatformType.Storage;
            mSpritename = "Dome";
            SetPlatfromParameters();
        }

        /// <summary>
        /// Connects this storage to another storage to act like a portal.
        /// </summary>
        /// <param name="storage">The other storage to be connected to</param>
        public void Connect(Storage storage)
        {
            throw new NotImplementedException();
        }
    }
}
