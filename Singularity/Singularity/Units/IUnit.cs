using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Units
{
    interface IUnit
    {

        /// <summary>
        /// get method for unit ID
        /// </summary>
        int Id{ get; }

        /// <summary>
        /// get method for unit Position
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Type will eventually be Assignment type
        /// </summary>
        String Assignment { get; set; }

        /// <summary>
        /// Draw method for unit
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="texture"></param>
        void Draw(SpriteBatch spriteBatch, Texture2D texture);

        /// <summary>
        /// Update method for unit
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime);
    }
}
