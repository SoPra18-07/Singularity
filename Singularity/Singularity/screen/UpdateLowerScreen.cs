using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.screen
{
    internal sealed class UpdateLowerScreen : IScreen
    {
        public void Draw(SpriteBatch spriteBatch)
        {
            System.Diagnostics.Debug.WriteLine("Update Lower Screen: Draw called");
        }

        public bool DrawLower()
        {
            return false;
        }

        public void Update(GameTime gametime)
        {
            System.Diagnostics.Debug.Write("Update Lower Screen: Update called");
        }

        public bool UpdateLower()
        {
            return true;
        }
    }
}
