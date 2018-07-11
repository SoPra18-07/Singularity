using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;

namespace Singularity.Screen
{
    /// <summary>
    /// A SwitcherAndRightAlignedItem can either consist of one IWindwoItem, which is aligned to the right according to width or it can
    /// consist of two IWindowItems which are drawn above one another and are aligned to the right according to width.
    /// This item can be used by buttons to disable one another to implement an switch between two states or to simply
    /// align an IWindowItem to the right side of something.
    /// </summary>
    internal sealed class SwitchAndRightAlignedIWindowItem : IWindowItem
    {
        private readonly IWindowItem mFirstItem;
        private readonly IWindowItem mOptionalSecondItem;

        /// <summary>
        /// Creates a SwitchAndRightAlignedItem
        /// </summary>
        /// <param name="firstItem">the first item</param>
        /// <param name="optionalSecondItem">the second, optional item</param>
        /// <param name="width">the width to get the item to align right</param>
        /// <param name="director">director</param>
        public SwitchAndRightAlignedIWindowItem(IWindowItem firstItem, IWindowItem optionalSecondItem, float width, ref Director director)
        {
            mFirstItem = firstItem;

            if (optionalSecondItem != null)
            {
                mOptionalSecondItem = optionalSecondItem;
            }

            Size = new Vector2(width, firstItem.Size.Y);

            Position = Vector2.Zero;

            ActiveInWindow = true;

        }

        /// <inheritdoc />
        public void Update(GameTime gametime)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle)
            {
                mFirstItem.Position = new Vector2(Position.X + Size.X - mFirstItem.Size.X - 50, Position.Y);
                mFirstItem.Update(gametime);

                if (mOptionalSecondItem != null)
                {
                    mOptionalSecondItem.Position = new Vector2(Position.X + Size.X - mOptionalSecondItem.Size.X - 50, Position.Y);
                    mOptionalSecondItem.Update(gametime);
                }
            }
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle)
            {
                mFirstItem.Draw(spriteBatch);

                mOptionalSecondItem?.Draw(spriteBatch);
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
    }
}
