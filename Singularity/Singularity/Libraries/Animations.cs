using System;
using Microsoft.Xna.Framework;

namespace Singularity.Libraries
{
    /// <summary>
    /// Gives a bunch of helpers to help with animations
    /// </summary>
    internal static class Animations
    {
        /// <summary>
        /// Returns an double of what value an animation should be at that uses
        /// easing. Can be used with translate, transform, and opacity
        /// animations. For example, an opacity animation to ease an object
        /// in between 0% to 100% would use this method for the current opacity
        /// value with each update.
        /// </summary>
        /// <param name="startValue">The start value of the animation</param>
        /// <param name="endValue">The end value of the animation</param>
        /// <param name="startTime">The GameTime when the animation started</param>
        /// <param name="duration">The duration of the animationin miliseconds</param>
        /// <param name="gameTime">The GameTime object</param>
        /// <returns>The current value of the easing animation</returns>
        public static double Easing(float startValue,
            float endValue,
            double startTime,
            double duration,
            GameTime gameTime)
        {
            // range is a separate var to make it easier to read.
            var range = endValue - startValue;

            // x is a separate var to make it easier to read as well.
            var x = gameTime.TotalGameTime.TotalMilliseconds - startTime;
            return startValue + range * (-Math.Cos(x * Math.PI / duration) / 2 + 0.5d);
        }
    }
}
