using System;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen
{
    /// <summary>
    /// IWindowItem which will place a text on the left side and an amount on the right side
    /// </summary>
    internal sealed class TextAndAmountIWindowItem : IWindowItem
    {
        // the text - will be placed on the left side of the item
        private readonly string mText;

        // the amount - will be placed on the right side on the item
        public int Amount { get; set; }

        // the spritefont used for the text
        private readonly SpriteFont mSpriteFont;

        // the color used to draw the text
        private readonly Color mColor;

        /// <summary>
        /// IWindowItem which will place a text on the left side and an amount on the right side
        /// </summary>
        /// <param name="text">the text for the left side</param>
        /// <param name="amount">the amount for the right side</param>
        /// <param name="position">the object's position - can be zero if added to a window</param>
        /// <param name="size">the object's size - the width is needed to place text/amount</param>
        /// <param name="spriteFont">spritefont used for drawstring</param>
        /// <param name="color">the text's color</param>
        public TextAndAmountIWindowItem(string text,
            int amount,
            Vector2 position,
            Vector2 size,
            SpriteFont spriteFont,
            Color color)
        {
            // use parameters
            mText = text;
            Amount = amount;
            Position = position;
            mSpriteFont = spriteFont;
            Size = new Vector2(size.X, mSpriteFont.MeasureString("A").Y);
            mColor = color;

            ActiveWindow = true;
        }
        public void Update(GameTime gametime)
        {
            // update not needed
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // draw the text to the item's left side
            spriteBatch.DrawString(mSpriteFont, mText, Position, mColor);

            // draw the amount to the item's right side
            spriteBatch.DrawString(mSpriteFont, Amount.ToString(), new Vector2(Position.X + Size.X - mSpriteFont.MeasureString(Amount.ToString()).X - 40, Position.Y), mColor);
        }

        public Vector2 Position { get; set; }
        public Vector2 Size { get; }
        public bool ActiveWindow { get; set; }
    }
}
