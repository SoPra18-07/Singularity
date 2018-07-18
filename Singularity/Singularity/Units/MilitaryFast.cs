
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Map;

namespace Singularity.Units
{
    internal class MilitaryFast : MilitaryUnit
    {
        public MilitaryFast(Vector2 position,
            Camera camera,
            ref Director director,
            bool friendly = true)
            : base(position, camera, ref director, friendly)
        {
            Speed = MilitaryUnitStats.FastSpeed;
            Health = MilitaryUnitStats.FastHealth;
            Range = MilitaryUnitStats.FastRange;

            mColor = new Color(new Vector3(0f, 0.34375f, 0.1484375f)); // Green
            mSelectedColor = new Color(new Vector3(0, 0.4453125f, 0.2109375f)); // Lighter Green
        }
    }
}
