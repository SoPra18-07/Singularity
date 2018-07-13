using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Resources;

namespace Singularity.Platforms
{
    [DataContract]
    class Storage: PlatformBlank
    {
        [DataMember]
        private const int PlatformWidth = 144;
        [DataMember]
        private const int PlatformHeight = 127;

        public Storage(Vector2 position,
            Texture2D spritesheet,
            Texture2D basesprite,
            SpriteFont libSans12,
            ref Director director,
            bool friendly = true)
            : base(position,
                spritesheet,
                basesprite,
                libSans12,
                ref director,
                EStructureType.Storage,
                -50,
                friendly: friendly)
        {
            // Todo: Add Costs of the platform here if you got them.
            // mCost = new Dictionary<EResourceType, int>();
            mType = EStructureType.Storage;
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
