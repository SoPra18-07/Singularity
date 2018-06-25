using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Shown after clicking save game on the pause menu. Allows the user
    /// to select a new name for the save file and save the game. It has
    /// an input field, shows the current save files, and back and
    /// save buttons.
    /// </summary>
    class SaveGameScreen : IScreen
    {

        public EScreen Screen { get; private set; } = EScreen.SaveGameScreen;
        public bool Loaded { get; set; }

        /// <summary>
        /// Updates the contents of the screen.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time</param>
        public void Update(GameTime gametime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads any content that this screen might need.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should update.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be updated.</returns>
        public bool UpdateLower()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should be drawn.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be drawn.</returns>
        public bool DrawLower()
        {
            throw new NotImplementedException();
        }
    }
}
