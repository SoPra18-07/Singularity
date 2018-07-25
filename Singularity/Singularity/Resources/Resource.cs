using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Resources
{
    [DataContract]
    public class Resource : ADie, ISpatial
    {
        // TODO: fkarg implement
        [DataMember]
        private const float Speed = 4f;
        [DataMember]
        private Vector2 mVelocity;
        [DataMember]
        public Vector2 RelativePosition { get; set; }
        [DataMember]
        public Vector2 RelativeSize { get; set; }
        [DataMember]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember]
        public Vector2 AbsoluteSize { get; set; }

        [DataMember]
        public EResourceType Type { get; internal set; }
        [DataMember]
        private Optional<GeneralUnit> mFollowing;

        public Resource(EResourceType type, Vector2 position, Director director) : base(ref director)
        {
            Type = type;
            AbsolutePosition = position;
            AbsoluteSize = new Vector2(10, 10);

        }

        public void Follow(GeneralUnit unit)
        {
            // now, using an actual velocity and without abruptly stopping, this should look way better.
            var diff = unit.AbsolutePosition - AbsolutePosition;
            // var targetPosition = diff - Geometry.NormalizeVector(diff) * 40 + AbsolutePosition;
            var dist = (float) Geometry.Length(diff);
            mVelocity = Geometry.NormalizeVector(diff) * Speed;
            if (dist < 10)
            {
                mVelocity = default(Vector2);
            } else if (dist < 30) {
                mVelocity = Vector2.Multiply(mVelocity, dist / 120f);
            } else if (dist < 70) {
                mVelocity = Vector2.Multiply(mVelocity, dist / 70f);
            }

            AbsolutePosition += mVelocity;
        }

        public new void ReloadContent(ref Director dir)
        {
            //This reload is called in the platform and the units to really reload every resource
            base.ReloadContent(ref dir);
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
            mVelocity += Geometry.NormalizeVector(direction) * Speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(AbsolutePosition, 6, 20, ResourceHelper.GetColor(Type), 6, LayerConstants.ResourceLayer);
            spriteBatch.DrawCircle(AbsolutePosition, 7, 20, Color.Black, 1, LayerConstants.ResourceLayer);
        }

        public void Update(GameTime gametime)
        {
            // Resoucres only update their location (if on a platform).
            AbsolutePosition += mVelocity;
            mVelocity = Vector2.Multiply(mVelocity, 0.8f);
        }

        public override bool Die()
        {
            UnFollow();
            return true;
        }
    }
}
