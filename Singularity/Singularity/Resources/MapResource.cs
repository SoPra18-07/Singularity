using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;
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
    [DataContract]
    public sealed class MapResource : ADie, ISpatial
    {
        /// <summary>
        /// The color of this resource.
        /// </summary>
        [DataMember]
        private readonly Color mColor;

        [DataMember]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember]
        public Vector2 AbsoluteSize { get; set; }
        [DataMember]
        public Vector2 RelativePosition { get; set; }
        [DataMember]
        public Vector2 RelativeSize { get; set; }
        [DataMember]
        public EResourceType Type { get; private set; }
        [DataMember]
        private int Amount { get; set; }

        /// <summary>
        /// Creates a new resource object for the given type, position and spriteSheet.
        /// </summary>
        /// <param name="type">The resource type of this resource. Specifies what kind of resource this will represent in the game</param>
        /// <param name="position">The inital position for this resource</param>
        /// <param name="width">The width of the resource on screen</param>
        /// /// <param name="director">Director of the game.</param>
        public MapResource(EResourceType type, Vector2 position, int width, ref Director director) : base(ref director)
        {
            Type = type;
            AbsolutePosition = position;
            AbsoluteSize = new Vector2(width, width * 0.6f);

            // maybe needs some tweaks, it was mentioned that more resources is in a relation with bigger resource representation
            // this needs adjustment as soon as we actually do something with resources.
            Amount = width;

            mColor = new Color(Vector3.Multiply(ResourceHelper.GetColor(type).ToVector3(), 0.75f));
            // mColor = ResourceHelper.GetColor(type);
        }

        public new void ReloadContent(ref Director dir)
        {
            base.ReloadContent(ref dir);
            mDirector = dir;
        }

        public Optional<Resource> Get(Vector2 location)
        {
            if (Amount <= 0)
            {
                return Optional<Resource>.Of(null);
            }
            Amount -= 1;

            // Track the creation of a resource in the statistics.
            mDirector.GetStoryManager.UpdateResources(Type);
            return Optional<Resource>.Of(new Resource(Type, location, mDirector));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawEllipse(new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int)AbsoluteSize.X, (int)AbsoluteSize.Y), mColor, Amount / 2f, LayerConstants.MapResourceLayer);
            // for some reason this is not below the FOW.
        }

        public void Update(GameTime gametime)
        {
            // There is no update code.
        }

        public override bool Die()
        {
            return true;
        }
    }
}
