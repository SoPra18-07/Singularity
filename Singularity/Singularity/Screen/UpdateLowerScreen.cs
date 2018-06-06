using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen
{
    /// <inheritdoc/>
    /// <remarks>
    /// A basic implementation for a Screen, used for debugging purposes.
    /// </remarks>
    internal sealed class UpdateLowerScreen : IScreen
    {
        public void Draw(SpriteBatch spriteBatch)
        {
            Debug.WriteLine("Update Lower Screen: Draw called");
        }

        public bool DrawLower()
        {
            return false;
        }

        public void Update(GameTime gametime)
        {
            Debug.Write("Update Lower Screen: Update called");
        }

        public bool UpdateLower()
        {
            return true;
        }
    }
}
