using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Resources;

namespace Singularity.KI
{
    /// <summary>
    /// The enemys defenseplatform.
    /// </summary>
    [DataContract]
    class Sentinel: DefenseLaser
    {
        [DataMember]
        private const int DrainingEnergy = 0;

        internal Sentinel(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            SpriteFont libSans12,
            ref Director director,
            bool friendly = false) : base(position,
            platformSpriteSheet,
            baseSprite,
            libSans12,
            ref director,
            friendly: friendly)
        {
            mDrainingEnergy = DrainingEnergy;

            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
        }
    }
}
