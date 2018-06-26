using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.Resources
{
    /// <inheritdoc cref="ISpatial"/>
    /// <inheritdoc cref="IDraw"/>
    /// <inheritdoc cref="IUpdate"/>
    /// <summary>
    /// Represents a resource in the game. Written in such a fashion that it can represent any resource there is and will be.
    /// </summary>
    public sealed class MapResource : ISpatial
    {

        /// <summary>
        /// The color of this resource.
        /// </summary>
        private readonly Color mColor;

        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        private EResourceType Type { get; }

        private int Amount { get; set; }

        /// <summary>
        /// Creates a new resource object for the given type, position and spriteSheet.
        /// </summary>
        /// <param name="type">The resource type of this resource. Specifies what kind of resource this will represent in the game</param>
        /// <param name="position">The inital position for this resource</param>
        /// <param name="width">The width of the resource on screen</param>
        public MapResource(EResourceType type, Vector2 position, int width)
        {
            Type = type;
            AbsolutePosition = position;
            AbsoluteSize = new Vector2(width, width * 0.6f);

            // maybe needs some tweaks, it was mentioned that more resources is in a relation with bigger resource representation
            // this needs adjustment as soon as we actually do something with resources.
            Amount = width / 10;

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
