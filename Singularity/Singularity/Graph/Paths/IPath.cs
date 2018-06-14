using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Graph.Paths
{
    public interface IPath
    {
        Queue<Vector2> GetPath();
    }
}
