using System;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Resources
{

    /// <summary>
    /// Provides a helper class to ease access to the corresponding texture for a given resource type and the sprite sheet holding all the images for resources.
    /// </summary>
    internal static class ResourceHelper
    {

        /// <summary>
        /// Gets the fitting texture for the given resource type and the spritesheet holding all the resource images.
        /// </summary>
        /// <param name="type">The resource type of the resource to get the texture for</param>
        /// <param name="spriteSheet">The spritesheet holding all the textures for resources</param>
        /// <returns>The texture for the given type</returns>
        public static Texture2D GetTexture(EResourceType type, Texture2D spriteSheet)
        {
            // TODO: actually implement code, since the spritesheet is as of now not created, thus no code can be provided.
            switch (type)
            {
                case EResourceType.Chip:
                    break;

                case EResourceType.Concrete:
                    break;

                case EResourceType.Copper:
                    break;

                case EResourceType.Fuel:
                    break;

                case EResourceType.Metal:
                    break;

                case EResourceType.Oil:
                    break;

                case EResourceType.Plastic:
                    break;

                case EResourceType.Sand:
                    break;

                case EResourceType.Silicon:
                    break;

                case EResourceType.Steel:
                    break;

                case EResourceType.Stone:
                    break;

                case EResourceType.Water:
                    break;

                default:
                    throw new NotSupportedException();
            }

            return null;
        }
    }
}
