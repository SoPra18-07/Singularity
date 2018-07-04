using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Map;

namespace Singularity.Units
{
    class ControllableUnit : FreeMovingUnit
    {
        public ControllableUnit(Vector2 position, Camera camera, ref Director director, ref Map.Map map) : base(position, camera, ref director, ref map)
        {

        }
    }
}
