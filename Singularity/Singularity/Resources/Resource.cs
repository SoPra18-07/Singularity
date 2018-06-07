using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Utils;

namespace Singularity.Resources
{
    /// <inheritdoc cref="IResource"/>
    /// <summary>
    /// Represents a resource in the game. Written in such a fashion that it can represent any resource there is and will be.
    /// </summary>
    public class Resource : IResource
    {
        /// <summary>
        /// The velocity of this resource object.
        /// </summary>
        private const float Velocity = 2;

        /// <summary>
        /// The width for the texture to be rendered.
        /// </summary>
        private const int Width = 10;

        /// <summary>
        /// The height for the texture to be rendered.
        /// </summary>
        private const int Height = 10;

        /// <summary>
        /// The current position of this resource on the map.
        /// </summary>
        private Vector2 mPosition;

        /// <summary>
        /// The direction the resource is moving.
        /// </summary>
        private Vector2 mDirection;

        /// <summary>
        /// The texture of this resource.
        /// </summary>
        private Texture2D mTexture;

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
        /// <param name="spriteSheet">The sprite sheet for all the resources in the game</param>
        public Resource(EResourceType type, Vector2 position, Texture2D spriteSheet)
        {
            if (position.Equals(null))
            {
                mPosition = new Vector2(0, 0);
            }

            Type = type;
            mPosition = position;
            mDirection = Vector2.Zero;

            mTexture = ResourceHelper.GetTexture(type, spriteSheet);

        }

        public void Accelerate(Vector2 vector)
        {
            // TODO: im not quite sure what was the intended way for the acceleration so im for now just implementing a basic acceleration without friction and a default velocity.
            mDirection += Geometry.NormalizeVector(vector);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: remove comment when a sprite sheet is provided since mTexture will be null atm.
            /*
            spriteBatch.Draw(mTexture, 
                new Rectangle((int) mPosition.X, (int) mPosition.Y, Width, Height), 
                Color.White);
            */
        }

        public Vector2 GetPosition()
        {
            return mPosition;
        }

        public void Update(GameTime gametime)
        {
            mPosition += mDirection * Velocity;
        }

        public void Use()
        {
            throw new NotImplementedException();
        }
    }
}
