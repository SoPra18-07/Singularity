using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Property;
using Microsoft.Xna.Framework;

namespace Singularity.AI.Behavior
{
    interface IAIBehavior
    {
        void Move(GameTime gametime);

        void Spawn(GameTime gametime);
    }
}
