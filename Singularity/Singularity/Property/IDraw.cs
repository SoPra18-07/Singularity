﻿using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Property
{
    /// <summary>
    /// Provides an Interface for everything that should be able to have a draw method.
    /// </summary>
    public interface IDraw
    {
        /// <summary>
        /// Used to draw content onto the screen with the given SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used to draw onto the screen</param>
        void Draw(SpriteBatch spriteBatch);
    }
}
