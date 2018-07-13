using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Utils;

namespace Singularity.Units
{
    class FlockingGroup
    {


        /// <summary>
        /// Checks whether the target position is reached or not.
        /// </summary>
        protected bool HasReachedTarget()
        {
            return Geometry.Length(mTargetPosition - Center) < mSpeed;
        }


        /// <summary>
        /// Checks whether the next waypoint has been reached.
        /// </summary>
        /// <returns></returns>
        protected bool HasReachedWaypoint()
        {
            // Debug.WriteLine("Waypoint reached.");
            // Debug.WriteLine("Next waypoint: " + mPath.Peek());

            return Geometry.Length(mPath.Peek() - Center) < mSpeed;

            // If the position is within 8 pixels of the waypoint, (i.e. it will overshoot the waypoint if it moves
            // for one more update, do the following
        }
    }
}
