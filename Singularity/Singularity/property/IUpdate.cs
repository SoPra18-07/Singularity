using Microsoft.Xna.Framework;

namespace Singularity.property
{   
    /// <summary>
    /// Provides an Interface for everything that should be able to have a update method.
    /// </summary>
    internal interface IUpdate
    {   
        /// <summary>
        /// Used to update logic related code. 
        /// </summary>
        /// <param name="gametime">Provides a snapshot of current game time values</param>
        void Update(GameTime gametime);
    }
}
