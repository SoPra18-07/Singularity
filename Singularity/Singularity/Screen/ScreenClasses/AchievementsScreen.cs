using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="ITransitionableMenu"/>
    /// <summary>
    /// Used to show achievements that the player has earned.
    /// It shows achievements already earned in a list format, not
    /// unsimilar to how steam shows achievements. It has no buttons
    /// other than back and simply shows textures of the achievements
    /// and their description. If an achievement has not been earned yet,
    /// the achievement texture is blacked out.
    /// </summary>
    internal sealed class AchievementsScreen : ITransitionableMenu
    {
        /// <summary>
        /// Updates the contents of the screen.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time </param>
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
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            // TODO
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

        public bool TransitionRunning { get; }
        public void TransitionTo(EScreen originScreen, EScreen targetScreen, GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
