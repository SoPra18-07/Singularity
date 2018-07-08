using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;

namespace Singularity.Screen
{
    internal sealed class BarIWindowItem : IWindowItem
    {
        // color of bar
        private readonly Color mColor;

        public BarIWindowItem(Vector2 size, Color color)
        {
            Position = new Vector2(0, 0);
            Size = size;
            mColor = color;
            ActiveInWindow = true;
        }

        public void Update(GameTime gametime)
        {
            // no update needed
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle)
            {
                spriteBatch.DrawRectangle(Position, new Vector2(Size.X - 40, 1), mColor);
            }
        }

        public Vector2 Position { get; set; }
        public Vector2 Size { get; }
        public bool ActiveInWindow { get; set; }
        public bool InactiveInSelectedPlatformWindow { get; set; }
        public bool OutOfScissorRectangle { get; set; }
    }
}
