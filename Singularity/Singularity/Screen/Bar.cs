using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;

namespace Singularity.Screen
{
    /// <summary>
    /// A Bar. Can be used to divide sections in a WindowObject
    /// </summary>
    internal sealed class Bar : IWindowItem
    {
        // color of bar
        private readonly Color mColor;

        /// <summary>
        /// Creates a simple IWindowItem that is just a bar
        /// </summary>
        /// <param name="width">the bar's width</param>
        /// <param name="color">the bar's color</param>
        public Bar(float width, Color color)
        {
            Position = Vector2.Zero;
            Size = new Vector2(width, 1);
            mColor = color;
            ActiveInWindow = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            // no update needed
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle)
            {
                // draw the bar
                spriteBatch.DrawRectangle(Position, Size, mColor);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public Vector2 Position { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public Vector2 Size { get; }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public bool ActiveInWindow { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public bool InactiveInSelectedPlatformWindow { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public bool OutOfScissorRectangle { get; set; }
    }
}
