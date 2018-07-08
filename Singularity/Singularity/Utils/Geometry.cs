using System;
using Microsoft.Xna.Framework;

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
            var length = Math.Sqrt(d: Math.Pow(x: vector.X, y: 2) + Math.Pow(x: vector.Y, y: 2) + Math.Pow(x: vector.Z, y: 2));

            //make sure to not divide by zero
            return length <= 1e-13 ? Vector3.Zero : new Vector3(x: (float)(vector.X / length), y: (float)(vector.Y / length), z: (float)(vector.Z / length));
        }

        public static double Length(Vector2 vec)
        {
            return Math.Sqrt(d: Math.Pow(x: vec.X, y: 2) + Math.Pow(x: vec.Y, y: 2));
        }


        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="vector">The vector to normalize</param>
        /// <returns>A new normalized vector of the given vector</returns>
        public static Vector2 NormalizeVector(Vector2 vector)
        {
            var tempVector = NormalizeVector(vector: new Vector3(x: vector.X, y: vector.Y, z: 0));

            return new Vector2(x: tempVector.X, y: tempVector.Y);
        }

        public static Vector2 GetCenter(float x, float y, float width, float height)
        {
            return new Vector2(x: x + width/2f, y: y + height/2f);
        }
    }
}
