using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Resources;

namespace Singularity.Screen
{
    internal sealed class ResourceIWindowItem : IWindowItem
    {
        // color of the resource
        private readonly Color mTypeColor;

        // resource name
        private readonly string mResourceText;

        // sprite font for text description
        private readonly SpriteFont mSpriteFont;

        // resourceColor position
        private Vector2 mColorPosition;

        // resourceText position
        private Vector2 mTextPosition;

        // amount position
        private Vector2 mAmountPosition;

        public ResourceIWindowItem(EResourceType resourceType, int amount, Vector2 size, SpriteFont spriteFont)
        {
            mSpriteFont = spriteFont;

            mTypeColor = ResourceHelper.GetColor(resourceType);
            mResourceText = resourceType.ToString();

            Amount = amount;

            Position = new Vector2(0, 0);

            // set position to automatically fit the text height
            Size = new Vector2(size.X, mSpriteFont.MeasureString(mResourceText).Y);

            ActiveWindow = true;

        }

        public void Update(GameTime gametime)
        {
            // update the positions
            mColorPosition = new Vector2(Position.X, Position.Y + Size.Y / 4);
            mTextPosition = new Vector2(Position.X + Size.Y, Position.Y);
            mAmountPosition = new Vector2(Position.X + Size.X - mSpriteFont.MeasureString(Amount.ToString()).X, Position.Y);

            // TODO : ? UPDATE FOR THE UI RESOURCE WINDOW ?
        }

        public void Draw(SpriteBatch spriteBatch)
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

        public Vector2 Position { get; set; }
        public Vector2 Size { get; }
        public bool ActiveWindow { get; set; }

        // the amount of resources
        public int Amount { get; set; }
    }
}
