using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Map;
using Singularity.Platform;

namespace Singularity.Screen.ScreenClasses
{
    class PresentationUIScreen : IScreen, IMousePositionListener, IMouseClickListener
    {
        private Button mButton;
        private GameScreen mGameScreen;

        private PlatformBlank mCurrentPlatform;

        private Texture2D mTexture;

        private float mouseX;
        private float mouseY;

        public Rectangle Bounds { get; set; }

        public PresentationUIScreen(GameScreen gameScreen, InputManager inputManager)
        {
            mGameScreen = gameScreen;

            inputManager.AddMousePositionListener(this);
            inputManager.AddMouseClickListener(this, EClickType.OutOfBoundsOnly, EClickType.InBoundsOnly);

            Bounds = new Rectangle(0, 0, 0, 0);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            mButton.Draw(spriteBatch);
            spriteBatch.End();
        }

        public void Update(GameTime gametime)
        {
            mButton.Update(gametime);

            if (mCurrentPlatform == null)
            {
                return;
            }

            if (mCurrentPlatform.IsPlaced)
            {
                mCurrentPlatform = null;
                return;
            }

            mCurrentPlatform.AbsolutePosition = new Vector2(mouseX - mCurrentPlatform.AbsoluteSize.X / 2f, mouseY - mCurrentPlatform.AbsoluteSize.Y / 2f);
        }

        public void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("PlatformBasic");

            mButton = new Button(.5f, mTexture, new Vector2(0, 0), false);
            mButton.ButtonReleased += OnButtonRelease;
        }

        public bool DrawLower()
        {
            return false;
        }

        public bool UpdateLower()
        {
            return false;
        }

        /// <summary>
        /// Handler for the Gameplay button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnButtonRelease(Object sender, EventArgs eventArgs)
        {
            if (mCurrentPlatform != null)
            {
                return;
            }

            mCurrentPlatform = new PlatformBlank(new Vector2(mouseX, mouseY), mTexture, false);
            mGameScreen.AddObject(mCurrentPlatform);
        }

        public void MousePositionChanged(float relX, float relY, float absX, float absY)
        {
            mouseX = absX;
            mouseY = absY;
        }

        public void MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            if(mCurrentPlatform != null && !mCurrentPlatform.IsSemiPlaced && mouseAction == EMouseAction.LeftClick)
            {
                mCurrentPlatform.IsSemiPlaced = true;
                if (mCurrentPlatform.IsSemiPlaced) { 
                    mCurrentPlatform = null;
                }
            }
        }

        public void MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
        }

        public void MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
        }
    }
}
