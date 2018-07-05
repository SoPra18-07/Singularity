using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Resources
{
    /// <inheritdoc cref="ISpatial"/>
    /// <inheritdoc cref="IDraw"/>
    /// <inheritdoc cref="IUpdate"/>
    /// <summary>
    /// Represents a resource-field on the Map in the game. Resources can be extraced from it using the Well, the Mine or the Quarry.
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

        public EResourceType Type { get; }

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
            AbsoluteSize = new Vector2(x: width, y: width * 0.6f);

            // maybe needs some tweaks, it was mentioned that more resources is in a relation with bigger resource representation
            // this needs adjustment as soon as we actually do something with resources.
            Amount = width;

            mColor = new Color(color: Vector3.Multiply(value1: ResourceHelper.GetColor(type: type).ToVector3(), scaleFactor: 0.75f));
            // mColor = ResourceHelper.GetColor(type);

        }

        public Optional<Resource> Get(Vector2 location)
        {
            if (Amount > 0)
            {
                Amount -= 1;
                return Optional<Resource>.Of(value: new Resource(type: Type, position: location));
            }
            return Optional<Resource>.Of(value: null);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawEllipse(rect: new Rectangle(x: (int)AbsolutePosition.X, y: (int)AbsolutePosition.Y, width: (int)AbsoluteSize.X, height: (int)AbsoluteSize.Y), color: mColor, thickness: Amount / 2f, layer: LayerConstants.MapResourceLayer);
        }

        public void Update(GameTime gametime)
        {
            // There is no update code.
        }

        public bool Die()
        {
            return true;
        }
    }
}
