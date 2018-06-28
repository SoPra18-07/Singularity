using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;
using Singularity.Libraries;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Resources
{
    public class Resource : ISpatial
    {
        // TODO: fkarg implement

        private const float Speed = 4f;
        private Vector2 mVelocity;

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        public EResourceType Type { get; internal set; }

        private Optional<GeneralUnit> mFollowing;

        public Resource(EResourceType type, Vector2 position)
        {
            Type = type;
            AbsolutePosition = position;
            AbsoluteSize = new Vector2(x: 10, y: 10);
        }

        public void Follow(GeneralUnit unit)
        {
            // now, using an actual velocity and without abruptly stopping, this should look way better.

            // the actual targetPosition is a certain distance (usually 50) from the unit, in the direction of the unit.
            var diff = unit.AbsolutePosition - AbsolutePosition;
            if (Geometry.Length(vec: diff) > 40)
            {
                var targetPosition = diff - 40 * Geometry.NormalizeVector(vector: diff) + AbsolutePosition;

                var factor = 0.4f; // experimental. seems to be a good value though.

                var movementVector = Vector2.Multiply(value1: Geometry.NormalizeVector(vector: mVelocity), scaleFactor: factor) + Vector2.Multiply(value1: Geometry.NormalizeVector(vector: new Vector2(x: targetPosition.X - AbsolutePosition.X, y: targetPosition.Y - AbsolutePosition.Y)), scaleFactor: 1 - factor);

                mVelocity = movementVector * Speed;
            } else {
                mVelocity = Vector2.Multiply(value1: mVelocity, scaleFactor: 0.93f);
            }
            AbsolutePosition = AbsolutePosition + mVelocity;

        }

        public Vector2 GetVelocity()
        {
            return mVelocity;
        }

        /// <summary>
        /// UnFollow a unit. If a Unit successfully delivered a Resource to the target-platform.
        /// </summary>
        public void UnFollow()
        {
            mFollowing = Optional<GeneralUnit>.Of(value: null);
        }

        /// <summary>
        /// Required if the Resource is on a platform, the Platform is taking care of the Resources not colliding.
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Vector2 direction)
        {
            mVelocity += Geometry.NormalizeVector(vector: direction) * Speed;
        }

        public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawCircle(center: AbsolutePosition, radius: 8, sides: 20, color: ResourceHelper.GetColor(type: Type), thickness: 10, layerDepth: LayerConstants.ResourceLayer);
            spriteBatch.DrawCircle(center: AbsolutePosition, radius: 10, sides: 20, color: Color.Black, thickness: 2, layerDepth: LayerConstants.ResourceLayer);
        }

        public void Update(GameTime gametime)
        {
            // Resoucres only update their location (if on a platform).
            AbsolutePosition += mVelocity;
            mVelocity = Vector2.Multiply(value1: mVelocity, scaleFactor: 0.8f);
        }
    }
}
