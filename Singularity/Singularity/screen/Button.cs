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
using Singularity.Property;


namespace Singularity.Screen
{
    class Button: IWindowItem
    {
        public event EventHandler ButtonClicked;
        public event EventHandler ButtonHovering;
        private float mScale;
        private Texture2D mButtonTexture;
        private bool mCurrentMouseState;
        private bool mPreviousMouseState;
        private Vector2 mPosition;
        private bool misText;
        private string mbuttonText;
        private SpriteFont mFont;



        public Button(float scale, Texture2D buttonTexture, Vector2 position)
        {
            misText = false;
            mScale = scale;
            mButtonTexture = buttonTexture;
            mPreviousMouseState = true;
            mCurrentMouseState = true;
            mPosition = position;
        }

        public Button(float scale, string buttonText, SpriteFont font, Vector2 position)
        {
            misText = true;
            mbuttonText = buttonText;
            mScale = scale;
            mFont = font;
            mPosition = position;
        }

        protected virtual void OnButtonClicked(EventArgs e)
        {
            ButtonClicked?.Invoke(this, e);
        }

        protected virtual void OnButtonHovering(EventArgs e)
        {
            ButtonHovering?.Invoke(this, e);
        }

        /// <summary>
        /// sends out button events (button hover/ button click)
        /// </summary>
        /// <returns></returns>
        public void ButtonEvents()
        {
            if (mCurrentMouseState == false && mPreviousMouseState == true)
            {
                OnButtonClicked(EventArgs.Empty);
            }
            if (Mouse.GetState().X < mPosition.X && Mouse.GetState().X >= mPosition.X + mButtonTexture.Width &&
                Mouse.GetState().Y < mPosition.Y && Mouse.GetState().Y >= mPosition.Y + mButtonTexture.Height)
            {
                OnButtonHovering(EventArgs.Empty);
            }
        }

        /// <summary>
        /// draws the button to specified scale
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {

            if (misText == false)
            {
                spriteBatch.Draw(mButtonTexture,
                    mPosition,
                    null,
                    Color.AntiqueWhite,
                    0f,
                    new Vector2(0, 0),
                    mScale,
                    SpriteEffects.None,
                    0f);

            }

            else
            {
                spriteBatch.DrawString(mFont,
                    origin: Vector2.Zero, 
                    position: mPosition, 
                    color: Color.White,
                    text: mbuttonText,
                    rotation: 0f,
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: 0.2f);
            }
        }

        /// <summary>
        /// updates the state of the button (left clicked or not)
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            // if left click within button area change current mouse state
            if (Mouse.GetState().LeftButton == ButtonState.Pressed &&
                Mouse.GetState().X < mPosition.X && Mouse.GetState().X >= mPosition.X + mButtonTexture.Width &&
                Mouse.GetState().Y < mPosition.Y && Mouse.GetState().Y >= mPosition.Y + mButtonTexture.Height)
            {
                mPreviousMouseState = mCurrentMouseState;
                mCurrentMouseState = false;
            }

            // if left click released then change current mouse state
            if (Mouse.GetState().LeftButton == ButtonState.Released && mCurrentMouseState == false &&
                Mouse.GetState().X < mPosition.X && Mouse.GetState().X >= mPosition.X + mButtonTexture.Width &&
                Mouse.GetState().Y < mPosition.Y && Mouse.GetState().Y >= mPosition.Y + mButtonTexture.Height)
            {
                mPreviousMouseState = mCurrentMouseState;
                mCurrentMouseState = true;
            }

            ButtonEvents();
        }
    }
}
