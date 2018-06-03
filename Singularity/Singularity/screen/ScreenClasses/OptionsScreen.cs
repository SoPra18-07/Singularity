using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Handles everything thats going on explicitly in the game.
    /// E.g. game objects, the map, camera. etc.
    /// </summary>
    class OptionsScreen : IScreen
    {
        private IScreenManager mScreenManager;
        private IScreen mGameplayOptionsScreen;
        private IScreen mGraphicsOptionScreen;
        private IScreen mAudioOptionsScreen;
        private Vector2 mScreenCenter;

        /// <summary>
        /// Shown after Options on the main menu or pause menu has been clicked.
        /// Allows different settings and options to be set. Buttons include
        /// for the different settings and a back button.
        /// </summary>
        /// <param name="screenResolution"></param>
        /// <param name="screenManager"></param>
        /// <param name="gameplayOptionsScreen"></param>
        /// <param name="graphicsOptionsScreen"></param>
        /// <param name="audioOptionScreen"></param>
        public OptionsScreen(Vector2 screenResolution, IScreenManager screenManager,
            IScreen gameplayOptionsScreen, IScreen graphicsOptionsScreen, IScreen audioOptionScreen)
        {
            mScreenCenter = screenResolution;
            mScreenManager = screenManager;
            mGameplayOptionsScreen = gameplayOptionsScreen;
            mGraphicsOptionScreen = gameplayOptionsScreen;
            mAudioOptionsScreen = audioOptionScreen;
        }

        public void Update(GameTime gametime)
        {
            // TODO make it work
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.StrokedRectangle(mScreenCenter,
                new Vector2(600, 400),
                Color.White,
                Color.White,
                0.5f,
                0.21f);
        }

        public bool UpdateLower()
        {
            throw new NotImplementedException();
        }

        public bool DrawLower()
        {
            throw new NotImplementedException();
        }
    }
}
