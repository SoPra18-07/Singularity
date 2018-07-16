using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;

namespace Singularity.Screen
{
    internal class Checkbox : Button
    {
        /// <summary>
        /// The size of the checkbox, determined by the height of the font used.
        /// </summary>
        private readonly Vector2 mCheckboxSize;

        /// <summary>
        /// The position of the checkbox.
        /// </summary>
        private readonly Vector2 mCheckboxPosition;

        /// <summary>
        /// The state of the checkbox
        /// </summary>
        public bool CheckboxState { get; set; }

        internal Checkbox(string buttonText, SpriteFont font, Vector2 textPosition, Vector2 checkboxPosition, Color color)
            : base(buttonText, font, textPosition, color)
        {
            mCheckboxSize = new Vector2(font.MeasureString("Gg").Y);
            mCheckboxPosition = checkboxPosition;

            // calculate the offset from the end of the string the start of the checkbox
            var checkboxLength = checkboxPosition.X - textPosition.X + mCheckboxSize.X;

            Size = new Vector2(checkboxLength, mCheckboxSize.Y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawRectangle(mCheckboxPosition,
                mCheckboxSize,
                mColor);
            if (CheckboxState)
            {
                spriteBatch.DrawLine(Vector2.Add(mCheckboxPosition, new Vector2(1, 0)),
                    Vector2.Add(Vector2.Add(mCheckboxPosition, mCheckboxSize), new Vector2(1, 0)),
                    mColor);
                spriteBatch.DrawLine(mCheckboxPosition.X + mCheckboxSize.X + 2,
                    mCheckboxPosition.Y,
                    mCheckboxPosition.X + 2,
                    mCheckboxPosition.Y + mCheckboxSize.Y,
                    mColor);
            }
        }

        protected override void OnButtonReleased()
        {
            base.OnButtonReleased();
            CheckboxState = !CheckboxState;
        }
    }
}
