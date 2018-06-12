using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Property
{
    internal interface ICollider : ISpatial
    {
        Rectangle AbsBounds { get; }

        bool Moved { get; }

        int Id { get; }
    }
}
