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
    class LoadingScreen : IScreen
    {
        /// <summary>
        /// Shown when resources are being loaded into the game.
        /// As long as not all resources have been loaded (e.g. map generation
        /// or on a save file load), this screen will be shown.
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
