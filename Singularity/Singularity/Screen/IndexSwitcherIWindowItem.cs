using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;

namespace Singularity.Screen
{
    /// <summary>
    /// An IndexSwitcher consists of two buttons and text between them. The buttons can be used to change the index of a given list
    /// </summary>
    internal sealed class IndexSwitcherIWindowItem : IWindowItem
    {
        /// <summary>
        /// the list's current index that is shown
        /// </summary>
        private int mCurrentIndex;

        /// <summary>
        /// buttons to switch index in ListOfElements
        /// </summary>
        private readonly Button mPreviousIndexButton;
        private readonly Button mNextIndexButton;

        /// <summary>
        /// the description the is given when creating the object. It can show what element's are represented by the integers in the ListOfElements
        /// </summary>
        private readonly string mDescriptionText;

        /// <summary>
        /// the complete text is composed of the description text and the integer of the current index
        /// </summary>
        private string mTextComplete;

        /// <summary>
        /// textfont
        /// </summary>
        private readonly SpriteFont mSpriteFont;

        /// <summary>
        /// the string size is the width of the TextComplete - used to measure the position of the previousIndexButton
        /// </summary>
        private float mStringSize;

        /// <summary>
        /// the list to go through using the indexSwitcher
        /// </summary>
        public List<int> ListOfElements { get; set; }

        /// <summary>
        /// Creates an IndexSwitcher consisting of two buttons and text between them.
        /// The buttons can be used to switch the index and get the values at the index of the set List.
        /// </summary>
        /// <param name="listOfElements">the list of integers to go through</param>
        /// <param name="descriptionText">text between buttons (i.e. "graph No: ")</param>
        /// <param name="width">width of the window, where this is placed in - eventual padding</param>
        /// <param name="spriteFont">textFont</param>
        /// <param name="director">director</param>
        public IndexSwitcherIWindowItem(
            List<int> listOfElements,
            string descriptionText,
            float width,
            SpriteFont spriteFont,
            Director director)
        {
            // catch bad input
            if (listOfElements.Count <= 0)
            {
                throw new IndexOutOfRangeException("the given list is too short to be used by indexSwitcher");
            }

            // set members
            mDescriptionText = descriptionText;
            mSpriteFont = spriteFont;
            mCurrentIndex = 0;

            ListOfElements = listOfElements;

            // create the previousIndexButton
            mPreviousIndexButton = new Button(" < ", spriteFont, Vector2.Zero) {Opacity = 1f};
            mPreviousIndexButton.ButtonReleased += PreviousIndex;

            mTextComplete = descriptionText + ListOfElements[mCurrentIndex];
            mStringSize = mSpriteFont.MeasureString(mTextComplete).X;

            // create the nextIndexButton
            mNextIndexButton = new Button(" > ", spriteFont, Vector2.Zero) { Opacity = 1f };
            mNextIndexButton.ButtonReleased += NextIndex;

            // set the Size, where the height is simply the height of the measured string of anything using the given SpriteFont
            Size = new Vector2(width, mSpriteFont.MeasureString("A").Y);

            ActiveInWindow = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {

            if (ActiveInWindow && !OutOfScissorRectangle && !InactiveInSelectedPlatformWindow)
            {
                // catch list getting smaller while index unchanged
                while (mCurrentIndex > ListOfElements.Count - 1)
                {
                    mCurrentIndex -= 1;
                }

                // nextIndexButton is almost aligned to the right of the window
                mNextIndexButton.Position = new Vector2(Position.X + Size.X - mNextIndexButton.Size.X, Position.Y);

                // previousIndexButton is positioned left of the infoText
                mPreviousIndexButton.Position = new Vector2(mNextIndexButton.Position.X - mStringSize - mPreviousIndexButton.Size.X, Position.Y);

                // update both buttons
                mPreviousIndexButton.Update(gametime);
                mNextIndexButton.Update(gametime);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !OutOfScissorRectangle && !InactiveInSelectedPlatformWindow)
            {
                // draw the buttons
                mPreviousIndexButton.Draw(spriteBatch);
                mNextIndexButton.Draw(spriteBatch);

                // draw the description + current integer from list between the two buttons
                spriteBatch.DrawString(mSpriteFont, mTextComplete, new Vector2(mNextIndexButton.Position.X - mStringSize, Position.Y), Color.White);
            }
        }

        /// <summary>
        /// Set the index to previous index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void PreviousIndex(object sender, EventArgs eventArgs)
        {
            mCurrentIndex -= 1;

            if (mCurrentIndex < 0)
            {
                mCurrentIndex = ListOfElements.Count - 1;
            }

            // update text + position
            mTextComplete = mDescriptionText + ListOfElements[mCurrentIndex];
            mStringSize = mSpriteFont.MeasureString(mTextComplete).X;
        }

        /// <summary>
        /// Set the index to next index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void NextIndex(object sender, EventArgs eventArgs)
        {
            mCurrentIndex += 1;

            if (mCurrentIndex > ListOfElements.Count - 1)
            {
                mCurrentIndex = 0;
            }

            // update text + position
            mTextComplete = mDescriptionText + ListOfElements[mCurrentIndex];
            mStringSize = mSpriteFont.MeasureString(mTextComplete).X;
        }

        /// <summary>
        /// returns the integer value of the lists current index
        /// </summary>
        /// <returns></returns>
        public int GetCurrentId()
        {
            return ListOfElements[mCurrentIndex];
        }

        public void JumpToSpecificId()
        {

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
