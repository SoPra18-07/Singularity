using System;
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
            AbsoluteSize = new Vector2(10, 10);
        }

        public void Follow(GeneralUnit unit)
        {
            // now, using an actual velocity and without abruptly stopping, this should look way better.

            // the actual targetPosition is a certain distance (usually 50) from the unit, in the direction of the unit.
            var diff = unit.AbsolutePosition - AbsolutePosition;
            if (Geometry.Length(diff) > 50)
            {
                var targetPosition = diff - 35 * Geometry.NormalizeVector(diff) + AbsolutePosition;

                var movementVector = Vector2.Multiply(Geometry.NormalizeVector(mVelocity), 0.4f) + Vector2.Multiply(Geometry.NormalizeVector(new Vector2(targetPosition.X - AbsolutePosition.X, targetPosition.Y - AbsolutePosition.Y)), 0.6f);

                mVelocity = movementVector * Speed;
            } else {
                mVelocity = Vector2.Multiply(mVelocity, 0.6f);
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
            mFollowing = Optional<GeneralUnit>.Of(null);
        }

        /// <summary>
        /// Required if the Resource is on a platform, the Platform is taking care of the Resources not colliding.
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Vector2 direction)
        {
            AbsolutePosition += direction;
        }

        public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawCircle(AbsolutePosition, 10, 20, ResourceHelper.GetColor(Type), 10, LayerConstants.ResourceLayer);
        }

        public void Update(GameTime gametime)
        {
            // Resoucres only update their location (if on a platform).
            AbsolutePosition += mVelocity;
            mVelocity = Vector2.Multiply(mVelocity, 0.8f);
        }
    }
}
