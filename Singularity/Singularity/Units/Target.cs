using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Map;

namespace Singularity.Units
{
    [DataContract]
    internal sealed class Target : EnemyUnit
    {
        public Target(Vector2 position, Camera camera, ref Director director, ref Map.Map map) : base(position, camera, ref director)
        {
            Speed = 0;
            Health = 100;
        }


        public override void Update(GameTime gameTime)
        {
            // do nothing
        }
    }
}
