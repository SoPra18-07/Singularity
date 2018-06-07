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
    public sealed class Resource : IResource
    {

        public const int DefaultWidth = 200;

        /// <summary>
        /// The current position of this resource on the map.
        /// </summary>
        private Vector2 mPosition;

        /// <summary>
        /// The color of this resource.
        /// </summary>
        private readonly Color mColor;


        //TODO: remove when ISpatial is there. 

        /// <summary>
        /// The width of this resource. The height gets set by this value.
        /// </summary>
        private int mWidth;

        /// <summary>
        /// The height of this resource. Gets automatically set by the width.
        /// </summary>
        private int mHeight;

        /*
           TODO: im not quite sure whether the ID is "unique" such that every instance of this class
           TODO: should hold a different id, or if every different resouce should hold a different id,
           TODO: thus i've not written any id assignment yet.
        */
        public int Id { get; }

        public EResourceType Type { get; }

        /// <summary>
        /// Creates a new resource object for the given type, position and spriteSheet.
        /// </summary>
        /// <param name="type">The resource type of this resource. Specifies what kind of resource this will represent in the game</param>
        /// <param name="position">The inital position for this resource</param>
        /// <param name="width">The width of the resource on screen</param>
        public Resource(EResourceType type, Vector2 position, int width)
        {
            Type = type;
            mPosition = position;

            mColor = ResourceHelper.GetColor(type);

            // could be handled more dynamically, for now this is o.k.
            mHeight = (int) (width * 0.6f);

            mWidth = width;

            if (width <= 0)
            {
                mWidth = DefaultWidth;
            }

        }

        public void Follow(GeneralUnit unitToFollow)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawEllipse(new Rectangle((int) mPosition.X, (int) mPosition.Y, mWidth, mHeight), mColor, 4f, LayerConstants.ResourceLayer);
        }

        public Vector2 GetPosition()
        {
            return mPosition;
        }

        public void Update(GameTime gametime)
        {
            //TODO: implement update code
        }

        public void Use()
        {
            throw new NotImplementedException();
        }
    }
}
