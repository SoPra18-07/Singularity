using System;
using Microsoft.Xna.Framework;
using Singularity.Platform;

namespace Singularity.Utils
{

    /// <summary>
    /// Provides helper functions for geometry.
    /// </summary>
    internal static class Geometry
    {

        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="vector">The vector to normalize</param>
        /// <returns>A new normalized vector of the given vector</returns>
        public static Vector3 NormalizeVector(Vector3 vector)
        {
            var length = Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2) + Math.Pow(vector.Z, 2));

            //make sure to not divide by zero
            return length == 0 ? Vector3.Zero : new Vector3((float)(vector.X / length), (float)(vector.Y / length), (float)(vector.Z / length));
        }


        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="vector">The vector to normalize</param>
        /// <returns>A new normalized vector of the given vector</returns>
        public static Vector2 NormalizeVector(Vector2 vector)
        {
            var tempVector = Geometry.NormalizeVector(new Vector3(vector.X, vector.Y, 0));

            return new Vector2(tempVector.X, tempVector.Y);
        }

        public static Vector2 GetCenter(float x, float y, float width, float height)
        {
            return new Vector2(x + (width/2f), y + (height/2f));
        }

        public static Vector2 GetCenter(PlatformBlank platform)
        {
            //TODO: change to ISpatial
            return GetCenter(platform.AbsolutePosition.X,
                platform.AbsolutePosition.Y,
                platform.AbsoluteSize.X,
                platform.AbsoluteSize.Y);

        }

    }
}
