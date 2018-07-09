using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;

namespace Singularity.Screen
{
    class ManualThroughListIWindowItem : IWindowItem
    {
        // the list's current index which is shown
        private int mCurrentIndex;

        private Button mPreviousIndexButton;
        private Button mNextIndexButton;

        private string mDescriptionText;

        private string mTextComplete;

        public List<int> GraphList { get; set; }

        public ManualThroughListIWindowItem(
            string descriptionText,
            float width,
            SpriteFont spriteFont,
            Director director)
        {
            mCurrentIndex = 0;
            mDescriptionText = descriptionText;

            mPreviousIndexButton = new Button(" < ", spriteFont, Vector2.Zero);
            mPreviousIndexButton.ButtonClicked += PreviousIndex;

            mNextIndexButton = new Button(" > ", spriteFont, Vector2.Zero);
            mNextIndexButton.ButtonClicked += NextIndex;

            Size = new Vector2(); // TODO
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            //
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO
        }

        private void PreviousIndex(object sender, EventArgs eventArgs)
        {
            mCurrentIndex -= 1;

            if (mCurrentIndex < 0)
            {
                mCurrentIndex = GraphList.Count - 1;
            }
        }

        private void NextIndex(object sender, EventArgs eventArgs)
        {
            mCurrentIndex += 1;

            if (mCurrentIndex > GraphList.Count - 1)
            {
                mCurrentIndex = 0;
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
