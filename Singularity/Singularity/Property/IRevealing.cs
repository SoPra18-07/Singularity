using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Property
{
    internal interface IRevealing
    {
        int RevelationRadius { get; }

        Vector2 Center { get; }
    }
}
