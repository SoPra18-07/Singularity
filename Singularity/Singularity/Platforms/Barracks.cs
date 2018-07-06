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
            : base(position, platformSpriteSheet, baseSprite, libSans12, ref director, EPlatformType.Barracks)
        {

            mIPlatformActions.Add(new MakeFastMilitaryUnit(this, ref director));
            mIPlatformActions.Add(new MakeStrongMilitrayUnit(this, ref director));

            Debug.WriteLine("Barracks created.");
        }
    }
}
