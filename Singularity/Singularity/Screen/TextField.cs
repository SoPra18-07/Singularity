using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen
{
    /// <summary>
    /// A TextField constists of a string which is cut to fit a specified width
    /// </summary>
    internal sealed class TextField : IWindowItem
    {
        #region member variables

        private string mSplittedText;

        // textfont
        private readonly SpriteFont mSpriteFont;

        // textcolor
        private readonly Color mColor;

        #endregion

        /// <summary>
        /// Creates a TextField which is automatically multilined to fit the size
        /// </summary>
        /// <param name="text">text for the field</param>
        /// <param name="position">top left corner of the field</param>
        /// <param name="size">size of the text field</param>
        /// <param name="spriteFont">spritefont for the text</param>
        /// <param name="color">text color</param>
        public TextField(string text, Vector2 position, Vector2 size, SpriteFont spriteFont, Color color)
        {
            // use parameter-variables
            Position = position;
            mSpriteFont = spriteFont;
            mColor = color;

            // split text to fit size-width
            mSplittedText = SplitLineToMultiline(text, size, spriteFont);

            // update to fot the newly splitted text-height
            Size = new Vector2(size.X, spriteFont.MeasureString(mSplittedText).Y);

            ActiveInWindow = true;
        }

        public void UpdateText(string newText)
        {
            Size = mSpriteFont.MeasureString(newText);
            mSplittedText = SplitLineToMultiline(newText, Size, mSpriteFont);
        }

        public void Update(GameTime gametime)
        {
            // no update needed
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle && !WindowIsInactive)
            {
                // draw the text
                spriteBatch.DrawString(mSpriteFont, mSplittedText, Position, mColor);
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool isHighest)
        {
            spriteBatch.DrawString(mSpriteFont, mSplittedText, Position, mColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        /// <summary>
        /// split a text to fit size-width
        /// </summary>
        /// <param name="text">text to split</param>
        /// <param name="size">size to fit</param>
        /// <param name="spriteFont">text font</param>
        /// <returns>the string splitted to fit the size-width</returns>
        private static string SplitLineToMultiline(string text, Vector2 size, SpriteFont spriteFont)
        {
            var splittedLines = new StringBuilder();
            var workingLine = new StringBuilder();

            var wordList = new LinkedList<string>(text.Split(' '));

            while (wordList.Count > 0)
                // words unprocessed
            {
                var word = wordList.First();
                wordList.RemoveFirst();

                if (spriteFont.MeasureString(word).X > size.X)
                    // split single words too long for the given width
                {
                    var workingWord = word;
                    var newLength = 0;

                    // calc number of letters to fit size
                    for (var numberOfLetters = 0; numberOfLetters < word.Length; numberOfLetters++)
                    {
                        if (0.5 * spriteFont.MeasureString(workingWord).X > size.X)
                            // reduce size by half to increase calculation speed
                        {
                            workingWord = workingWord.Substring(0, workingWord.Length / 2);
                        }
                        else if (spriteFont.MeasureString(workingWord).X > size.X)
                            // reduce size one letter at a time
                        {
                            workingWord = workingWord.Substring(0, workingWord.Length - 1);
                        }

                        newLength = workingWord.Length;
                    }

                    // calculated division
                    var head = word.Substring(0, newLength);
                    var tail = word.Substring(newLength);

                    word = head;
                    wordList.AddFirst(tail);
                }


                if (spriteFont.MeasureString(workingLine).X + spriteFont.MeasureString(word).X > size.X)
                    // current word too big to be added to the current line -> create new line
                {
                    splittedLines.AppendLine(workingLine.ToString());
                    workingLine.Clear();
                }

                // add the current word to the line
                workingLine.Append(word + " ");
            }

            // add the final line
            splittedLines.Append(workingLine);

            return splittedLines.ToString();
        }

        /// <inheritdoc />
        public Vector2 Position { get; set; }

        /// <inheritdoc />
        public Vector2 Size { get; set; }

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
