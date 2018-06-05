using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Property;


namespace Singularity.Screen
{
    class Button : IWindowItem
    {
        private bool misText;
        private float mScale;
        private Texture2D mbuttonTexture;
        private string mbuttonText;
        private Vector2 mPosition;
        private float mWidth;
        private float mHeight;
        private SpriteFont mFont;
        private Color mColor;
        private InputManager mInputMangager;

        public event EventHandler ButtonReleased;
        public event EventHandler ButtonHovering;

        /// <summary>
        /// Creates a button using a Texture2D
        /// </summary>
        /// <param name="scale"> scale of the texture</param>
        /// <param name="buttonTexture"></param>
        /// <param name="position"></param>
        public Button(float scale, Texture2D buttonTexture, Vector2 position)
        {
            misText = false;
            mScale = scale;
            mbuttonTexture = buttonTexture;
            mPosition = position;
            mWidth = mbuttonTexture.Width;
            mHeight = mbuttonTexture.Height;
            mColor = Color.White;
        }

        /// <summary>
        /// Creates a Button made of text
        /// </summary>
        /// <param name="buttonText">text that button will appear as</param>
        /// <param name="font"></param>
        /// <param name="position"></param>
        public Button(string buttonText, SpriteFont font, Vector2 position)
        {
            misText = true;
            mbuttonText = buttonText;
            mFont = font;
            mPosition = position;
            mWidth = mFont.MeasureString(mbuttonText).X;
            mHeight = mFont.MeasureString(mbuttonText).Y;
            mColor = Color.White;
        }

        /// <summary>
        /// Sends out event that button has been left click released
        /// </summary>
        protected virtual void OnButtonReleased()
        {
            if (ButtonReleased != null)
            {
                ButtonReleased(this, EventArgs.Empty);
            }

        }


        /// <summary>
        /// Sends out event that mouse is hovering over the button
        /// </summary>
        protected virtual void OnButtonHovering()
        {
            if (ButtonHovering != null)
            {
                ButtonHovering(this, EventArgs.Empty);
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // draw for button that uses a Texture2D
            if (misText == false)
            {
                spriteBatch.Draw(mbuttonTexture,
                    mPosition,
                    null,
                    mColor,
                    0f,
                    new Vector2(0, 0),
                    mScale,
                    SpriteEffects.None,
                    0f);

            }

            // draw for button that uses text
            else
            {
                spriteBatch.DrawString(mFont,
                    origin: Vector2.Zero,
                    position: mPosition,
                    color: mColor,
                    text: mbuttonText,
                    rotation: 0f,
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: 0.2f);
            }
        }
        public void Update(GameTime gametime)
        {
            if (Mouse.GetState().X >= mPosition.X &&
                Mouse.GetState().X <= mPosition.X + mWidth &&
                Mouse.GetState().Y > mPosition.Y &&
                Mouse.GetState().Y <= mPosition.Y + mHeight)
            {
                OnButtonHovering();
                mColor = Color.Gray;
            }

            else
            {
                mColor = Color.White;
            }

        }

        public void MousePressed(MouseEvent mouseEvent)
        {
        }

    }


}
