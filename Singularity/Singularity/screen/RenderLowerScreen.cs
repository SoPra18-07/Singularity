using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen
{
    /// <inheritdoc/>
    /// <remarks>
    /// A basic implementation for a Screen, used for debugging purposes.
    /// </remarks>
    internal sealed class RenderLowerScreen : IScreen
    {
        public void Draw(SpriteBatch spriteBatch)
        {
            System.Diagnostics.Debug.WriteLine("RenderLowerScreen: Draw called");
        }

        public bool DrawLower()
        {
            return true;
        }

        public void Update(GameTime gametime)
        {
            System.Diagnostics.Debug.WriteLine("RenderLowerScreen: Update called");
        }

        public bool UpdateLower()
        {
            return false;
        }
    }
}
