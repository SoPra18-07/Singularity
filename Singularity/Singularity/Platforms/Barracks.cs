using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;

namespace Singularity.Platforms
{
    internal class Barracks : PlatformBlank
    {
        public Barracks(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, SpriteFont libSans12, ref Director director)
            : base(position: position, platformSpriteSheet: platformSpriteSheet, baseSprite: baseSprite, libSans12Font: libSans12, director: ref director, type: EPlatformType.Barracks)
        {

            mIPlatformActions.Add(item: new MakeFastMilitaryUnit(platform: this, director: ref director));
            mIPlatformActions.Add(item: new MakeStrongMilitrayUnit(platform: this, director: ref director));

            Debug.WriteLine(message: "Barracks created.");
        }
    }
}
