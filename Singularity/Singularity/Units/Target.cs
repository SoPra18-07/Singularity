using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;

namespace Singularity.Units
{
    internal sealed class Target : EnemyUnit
    {
        public Target(Vector2 position, Camera camera, ref Director director, ref Map.Map map) : base(position, camera, ref director, ref map)
        {
            mSpeed = 0;
        }

        public override void Update(GameTime gameTime)
        {
            // do nothing
            base.Update(gameTime);
        }
    }
}
