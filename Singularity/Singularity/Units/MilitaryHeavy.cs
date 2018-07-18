using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Map;

namespace Singularity.Units
{
    internal class MilitaryHeavy : MilitaryUnit
    {
        public MilitaryHeavy(Vector2 position,
            Camera camera,
            ref Director director,
            bool friendly = true)
            : base(position, camera, ref director, friendly)
        {
            Speed = MilitaryUnitStats.HeavySpeed;
            Health = MilitaryUnitStats.HeavyHealth;
            Range = MilitaryUnitStats.HeavyRange;

            mColor = new Color(0.45703125f, 0.296875f, 0.140625f); // Brown
			mSelectedColor = new Color(0.546875f, 0.3828125f, 0.22265625f); // Lighter brown
        }
    }
}
