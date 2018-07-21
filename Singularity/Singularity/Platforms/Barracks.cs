using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;

namespace Singularity.Platforms
{
    /// <inheritdoc cref="PlatformBlank"/>
    [DataContract]
    internal sealed class Barracks : PlatformBlank
    {
        public Barracks(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            SpriteFont libSans12,
            ref Director director,
            bool friendly = true)
            : base(position: position,
                platformSpriteSheet: platformSpriteSheet,
                baseSprite: baseSprite,
                libsans12: libSans12,
                director: ref director,
                type: EStructureType.Barracks,
                friendly: friendly)
        {
            mIPlatformActions.Add(new MakeFastMilitaryUnit(this, ref director));
            mIPlatformActions.Add(new MakeHeavyMilitaryUnit(this, ref director));
            mIPlatformActions.Add(new MakeStandardMilitaryUnit(this, ref director));

            mCost = GetResourceCosts(EStructureType.Barracks);
            
        }
    }
}
