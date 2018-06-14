using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Property;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Resources
{
    /// <inheritdoc cref="IResource"/>
    /// <summary>
    /// Represents a resource in the game. Written in such a fashion that it can represent any resource there is and will be.
    /// </summary>
    public sealed class Resource : ISpatial
    {

        /// <summary>
        /// The color of this resource.
        /// </summary>
        private readonly Color mColor;

        public EResourceType Type { get; }

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }
        

        public Vector2 AbsolutePosition { get; set; }
        

        public Vector2 AbsoluteSize { get; set; }

        /// <summary>
        /// Creates a new resource object for the given type, position and spriteSheet.
        /// </summary>
        /// <param name="type">The resource type of this resource. Specifies what kind of resource this will represent in the game</param>
        /// <param name="position">The inital position for this resource</param>
        /// <param name="width">The width of the resource on screen</param>
        public Resource(EResourceType type, Vector2 position, int width)
        {
            Type = type;

            AbsolutePosition = position;
            AbsoluteSize = new Vector2(width, (int)(width * 0.6f));

            mColor = ResourceHelper.GetColor(type);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawEllipse(new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int)AbsoluteSize.X, (int)AbsoluteSize.Y), mColor, 4f, LayerConstants.ResourceLayer);
        }

        public void Update(GameTime gametime)
        {
            //TODO: implement update code
        }

    }
}
