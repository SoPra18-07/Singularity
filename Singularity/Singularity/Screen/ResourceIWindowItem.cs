using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Resources;

namespace Singularity.Screen
{
    /// <summary>
    /// A ResourceItem consists of a small colored rectangle at the beginning, followed by the resource's name and
    /// an integer representing the resources' amount
    /// </summary>
    internal sealed class ResourceIWindowItem : IWindowItem
    {
        #region member variables

        // resourceColor
        private readonly Color mTypeColor;

        // resource name
        private readonly string mResourceText;

        // textFont
        private readonly SpriteFont mSpriteFont;

        // position of resourceColor
        private Vector2 mColorPosition;

        // position of textfield
        private Vector2 mTextPosition;

        // position of amount text
        private Vector2 mAmountPosition;

        #endregion

        // resource amount
        public int Amount { get; set; }

        /// <summary>
        /// Creates a ResourceIWindowItem, which consists of the resources color, it's name and it's amount
        /// </summary>
        /// <param name="resourceType">resource type</param>
        /// <param name="amount">resource amount</param>
        /// <param name="size">size of the item to create (width important)</param>
        /// <param name="spriteFont">text's spritefont</param>
        public ResourceIWindowItem(EResourceType resourceType, int amount, Vector2 size, SpriteFont spriteFont)
        {
            mSpriteFont = spriteFont;

            mTypeColor = ResourceHelper.GetColor(resourceType);
            mResourceText = resourceType.ToString();

            Amount = amount;

            // set to Vector.Zero, since the window will manage this item's position
            Position = Vector2.Zero;

            // set size to automatically fit the text height
            Size = new Vector2(size.X, mSpriteFont.MeasureString(mResourceText).Y);

            ActiveInWindow = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle)
            {
                // update positions
                mColorPosition = new Vector2(Position.X, Position.Y + Size.Y / 4);
                mTextPosition = new Vector2(Position.X + Size.Y, Position.Y);
                mAmountPosition = new Vector2(Position.X + Size.X - mSpriteFont.MeasureString(Amount.ToString()).X, Position.Y);

                // TODO : ? UPDATE FOR THE UI RESOURCE WINDOW ?
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle)
            {
                // draw the color rectangle at the beginning
                spriteBatch.StrokedRectangle(
                    location: mColorPosition,
                    size: new Vector2(Size.Y - Size.Y / 2, Size.Y - Size.Y / 2),
                    colorBorder: Color.White,
                    colorCenter: mTypeColor,
                    opacityBorder: 1f,
                    opacityCenter: 1f);
                // draw the resource text
                spriteBatch.DrawString(
                    spriteFont: mSpriteFont,
                    text: mResourceText,
                    position: mTextPosition,
                    color: Color.White);
                // draw the amount aligned to the right side 
                spriteBatch.DrawString(
                    spriteFont: mSpriteFont,
                    text: Amount.ToString(),
                    position: mAmountPosition,
                    color: Color.White);
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
