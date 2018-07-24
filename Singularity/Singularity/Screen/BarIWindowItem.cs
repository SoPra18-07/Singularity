using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;

namespace Singularity.Screen
{
    /// <summary>
    /// A Bar. Can be used to divide sections in a WindowObject
    /// </summary>
    internal sealed class BarIWindowItem : IWindowItem
    {
        // color of bar
        private readonly Color mColor;

        /// <summary>
        /// Creates a simple IWindowItem that is just a bar
        /// </summary>
        /// <param name="width">the bar's width</param>
        /// <param name="color">the bar's color</param>
        public BarIWindowItem(float width, Color color)
        {
            Position = Vector2.Zero;
            Size = new Vector2(width, 1);
            mColor = color;
            ActiveInWindow = true;
        }

        /// <inheritdoc />
        public void Update(GameTime gametime)
        {
            // no update needed
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle && !WindowIsInactive)
            {
                // draw the bar
                spriteBatch.DrawRectangle(Position, Size, mColor);
            }
        }

        /// <inheritdoc />
        public Vector2 Position { get; set; }
        /// <inheritdoc />
        public Vector2 Size { get; }
        /// <inheritdoc />
        public bool ActiveInWindow { get; set; }
        /// <inheritdoc />
        public bool InactiveInSelectedPlatformWindow { get; set; }
        /// <inheritdoc />
        public bool OutOfScissorRectangle { get; set; }
        /// <inheritdoc />
        public bool WindowIsInactive { get; set; }
    }
}
