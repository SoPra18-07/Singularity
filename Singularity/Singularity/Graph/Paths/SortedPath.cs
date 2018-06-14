using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Graph.Paths
{
    public class SortedPath : IPath
    {

        private readonly Queue<Vector2> mPath;


        public SortedPath(IEnumerable<Vector2> path = null)
        {
            if (path == null)
            {
                mPath = new Queue<Vector2>();
                return;
            }
            mPath = new Queue<Vector2>(path);

        }

        public Queue<Vector2> GetPath()
        {
            return mPath;
        }

        public void AddNewPosition(Vector2 position)
        {
            mPath.Enqueue(position);
        }
    }
}
