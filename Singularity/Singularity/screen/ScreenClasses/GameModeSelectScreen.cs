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
    
    class GameModeSelectScreen : IScreen
    {
        /// <summary>
        /// Shown after the "New Game" button on the main
        /// menu has been clicked. It shows the option to either
        /// play a new campaign or a new skirmish. It uses two text buttons
        /// and a back button
        /// </summary>
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
