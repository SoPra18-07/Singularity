using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen
{
    internal sealed class TextField : IWindowItem
    {

        private readonly string mSplittedText;
        private readonly SpriteFont mSpriteFont;

        /// <summary>
        /// Creates a TextField which is automatically multilined to fit the size
        /// </summary>
        /// <param name="text">text for the field</param>
        /// <param name="position">top left corner of the field</param>
        /// <param name="size">size of the text field</param>
        /// <param name="spriteFont">spritefont for the text</param>
        public TextField(string text, Vector2 position, Vector2 size, SpriteFont spriteFont)
        {
            // use parameter-variables
            Position = position;
            mSpriteFont = spriteFont;

            // split text to fit size-width
            mSplittedText = SplitLineToMultiline(text: text, size: size, spriteFont: spriteFont);

            // update size
            Size = new Vector2(x: size.X, y: spriteFont.MeasureString(text: mSplittedText).Y);

            Active = true;
        }

        public void Update(GameTime gametime)
        {
            // no update needed
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(spriteFont: mSpriteFont, text: mSplittedText, position: Position, color: new Color(r: 0,g: 0,b: 0));
        }

        /// <summary>
        /// split a text to fit size-width
        /// </summary>
        /// <param name="text">text to split</param>
        /// <param name="size">size to fit</param>
        /// <param name="spriteFont">text font</param>
        /// <returns></returns>
        private static string SplitLineToMultiline(string text, Vector2 size, SpriteFont spriteFont)
        {
            var splittedLines = new StringBuilder();
            var workingLine = new StringBuilder();

            var wordList = new LinkedList<string>(collection: text.Split(' '));

            while (wordList.Count > 0)
                // words unprocessed
            {
                var word = wordList.First();
                wordList.RemoveFirst();

                if (spriteFont.MeasureString(text: word).X > size.X)
                    // split single words too long for the given width
                {
                    Console.Out.WriteLine(value: word);
                    var workingWord = word;
                    var newLength = 0;

                    // calc number of letters to fit size
                    for (var numberOfLetters = 0; numberOfLetters < word.Length; numberOfLetters++)
                    {
                        if (0.5 * spriteFont.MeasureString(text: workingWord).X > size.X)
                            // reduce size by half to increase calculation speed
                        {
                            workingWord = workingWord.Substring(startIndex: 0, length: workingWord.Length / 2);
                        }
                        else if (spriteFont.MeasureString(text: workingWord).X > size.X)
                            // reduce size one letter at a time
                        {
                            workingWord = workingWord.Substring(startIndex: 0, length: workingWord.Length - 1);
                        }

                        newLength = workingWord.Length;
                    }

                    // calculated division
                    var head = word.Substring(startIndex: 0, length: newLength);
                    var tail = word.Substring(startIndex: newLength);

                    word = head;
                    wordList.AddFirst(value: tail);
                }


                if (spriteFont.MeasureString(text: workingLine).X + spriteFont.MeasureString(text: word).X > size.X)
                    // current word too big to be added to the current line -> create new line
                {
                    splittedLines.AppendLine(value: workingLine.ToString());
                    workingLine.Clear();
                }

                // add the current word to the line
                workingLine.Append(value: word + " ");
            }

            // add the final line
            splittedLines.Append(value: workingLine);

            return splittedLines.ToString();
        }

        /// <summary>
        /// top left corner of the textfield
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// size of the textfield
        /// </summary>
        public Vector2 Size { get; }

        /// <summary>
        ///
        /// </summary>
        public bool Active { get; set; }
    }
}
