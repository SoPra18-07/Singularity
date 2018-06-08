using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen.ScreenClasses.OptionScreens
{
    class GameplayOptionsScreen : IScreen
    {
        /// <inheritdoc cref="IScreen"/>
        /// <summary>
        /// Handles everything thats going on explicitly in the game.
        /// E.g. game objects, the map, camera. etc.
        /// </summary>
        /// 
        private Vector2 mMenuCenter;
        
        /// <summary>
        /// Constructor for the gamplay options screen which allows
        /// players to change gameplay options.
        /// </summary>
        /// <param name="menuCenter">Center of the options menu</param>
        public GameplayOptionsScreen(Vector2 menuOrigin)
        {
            mMenuCenter = menuOrigin;
        }

        public void Update(GameTime gametime)
        {
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public bool UpdateLower()
        {
            return true;
        }

        public bool DrawLower()
        {
            return true;
        }
    }
}
