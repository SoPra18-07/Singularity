using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Resources;

namespace Singularity.AI.Structures
{
    /// <summary>
    /// The enemys defenseplatform.
    /// </summary>
    [DataContract]
    public sealed class Sentinel: DefenseLaser
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
            mBlueprint = false;
            mType = EStructureType.Sentinel;
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();

        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            if (mDirector.GetClock.GetShootingLaserTime().TotalMilliseconds > 1000)
            {
                if (mShootingTarget != null)
                {
                    mDefenseAction.Execute();
                }
            }
        }
    }
}
