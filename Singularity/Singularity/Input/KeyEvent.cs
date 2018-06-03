
using Microsoft.Xna.Framework.Input;

namespace Singularity.Input
{
    internal sealed class KeyEvent
    {
        public KeyEvent(Keys[] currentKeys)
        {
            CurrentKeys = currentKeys;
        }

        private Keys[] CurrentKeys { get; }
    }
}
