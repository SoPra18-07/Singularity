using System;
using Microsoft.Xna.Framework;
using Singularity.Property;

namespace Singularity.Events
{
    public sealed class SpatialPositionEventArgs : EventArgs
    {
        private readonly ISpatial mSpatial;

        public SpatialPositionEventArgs(ISpatial spatial)
        {
            mSpatial = spatial;

        }
        /* ReSharper
        public Vector2 GetAbsolutePosition()
        {
            return mSpatial.AbsolutePosition;
        }

        public Vector2 GetRelativePosition()
        {
            return mSpatial.RelativePosition;
        }
        */

        public Vector2 GetAbsoluteCenter()
        {
            return mSpatial.AbsolutePosition + mSpatial.AbsoluteSize / 2;
        }

        /*
        public Vector2 GetRelativeCenter()
        {
            return mSpatial.RelativePosition + mSpatial.RelativeSize / 2;
        }
        */
    }
}
