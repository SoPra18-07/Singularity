using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        }
        public void Update(GameTime gametime)
        {
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
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
