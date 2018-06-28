using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;

namespace Singularity.Platforms
{
    internal class Barracks : PlatformBlank
    {
        public Barracks(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ref Director director, Vector2 center = new Vector2())
            : base(position: position, platformSpriteSheet: platformSpriteSheet, baseSprite: baseSprite, director: ref director, center: center)
        {
        }
    }
}
