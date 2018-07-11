using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;

namespace Singularity.Platforms
{
    /// <inheritdoc cref="PlatformBlank"/>
    [DataContract]
    internal class Barracks : PlatformBlank
    {
        public Barracks(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            ref Director director,
            bool friendly = true)
            : base(position: position,
                platformSpriteSheet: platformSpriteSheet,
                baseSprite: baseSprite,
                director: ref director,
                type: EPlatformType.Barracks,
                friendly: friendly)
        {

            mIPlatformActions.Add(new MakeFastMilitaryUnit(this, ref director));
            mIPlatformActions.Add(new MakeStrongMilitaryUnit(this, ref director));
            mIPlatformActions.Add(new MakeStandardMilitaryUnit(this, ref director));

            Debug.WriteLine("Barracks created.");
        }
    }
}
