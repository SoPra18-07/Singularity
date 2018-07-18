using System;
using System.Runtime.InteropServices;
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
            var length = Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2) + Math.Pow(vector.Z, 2));

            //make sure to not divide by zero
            return length <= 1e-13 ? Vector3.Zero : new Vector3((float)(vector.X / length), (float)(vector.Y / length), (float)(vector.Z / length));
        }

        public static double Length(Vector2 vec)
        {
            return FastSqrt(vec.X * vec.X + vec.Y * vec.Y);
        }


        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="vector">The vector to normalize</param>
        /// <returns>A new normalized vector of the given vector</returns>
        public static Vector2 NormalizeVector(Vector2 vector)
        {
            var tempVector = NormalizeVector(new Vector3(vector.X, vector.Y, 0));

            return new Vector2(tempVector.X, tempVector.Y);
        }

        public static Vector2 GetCenter(float x, float y, float width, float height)
        {
            return new Vector2(x + width/2f, y + height/2f);
        }

        /// <summary>
        /// Returns whether the given Vector2 is contained in the circle specified by radius
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        public static bool Contains(Vector2 circleCenter, float radius, Vector2 toCheck, bool alsoOn = false)
        {
            var distanceSquared = Math.Pow(toCheck.X - circleCenter.X, 2) + Math.Pow(toCheck.Y - circleCenter.Y, 2);

            if (alsoOn)
            {
                return distanceSquared <= Math.Pow(radius, 2);
            }

            return distanceSquared < Math.Pow(radius, 2);

        }

        /// <summary>
        /// Gets the approximate distance between 2 points quickly.
        /// </summary>
        /// <param name="pointA">The first point.</param>
        /// <param name="pointB">The second point.</param>
        /// <returns></returns>
        public static float GetQuickDistance(Vector2 pointA, Vector2 pointB)
        {
            var dx = pointB.X - pointA.X;
            var dy = pointB.Y - pointA.Y;

            return FastSqrt(dx * dx + dy * dy);

        }

        /// <summary>
        /// A method that compares if 2 floats are almost equal to each other within a specified margin.
        /// </summary>
        /// <param name="x">The first float to be compared.</param>
        /// <param name="y">The second float to be compared.</param>
        /// <param name="epsilon">The acceptable margin of error.</param>
        /// <returns></returns>
        public static bool FloatAlmostEqual(float x, float y, float epsilon=0.00000000001f)
        {
            double dx = x;
            double dy = y;
            if (dx - dy < epsilon)
            {
                return true;
            }

            return dy - dx < epsilon;
        }


        /// <summary>
        /// Fast squareroot algorithm with 98% accuracy.
        ///  </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        /// <see cref="http://blog.wouldbetheologian.com/2011/11/fast-approximate-sqrt-method-in-c.html"/>
        public static float FastSqrt(float z)
        {
            if (z < 0.15e-10 && z > -0.15e-10)
            {
                return 0;
            }

            FloatIntUnion u;
            u.tmp = 0;
            u.f = z;
            u.tmp -= 1 << 23; /* Subtract 2^m. */
            u.tmp >>= 1; /* Divide by 2. */
            u.tmp += 1 << 29; /* Add ((b + 1) / 2) * 2^m. */
            return u.f;
        }

        /// <summary>
        /// Divides quickly by 2^divideBy.
        /// </summary>
        /// <param name="z">The numerator.</param>
        /// <param name="divideBy">The exponent of the denominator 2^divideBy.</param>
        /// <returns>The float divided by 2^divideBy.</returns>
        public static float BitShiftFloat(float z, int divideBy)
        {
            FloatIntUnion u;
            u.tmp = 0;
            u.f = z;
            u.tmp >>= divideBy;
            return u.f;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatIntUnion
        {
            [FieldOffset(0)]
            public float f;

            [FieldOffset(0)]
            public int tmp;
        }
    }


}
