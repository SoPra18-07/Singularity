using Microsoft.Xna.Framework.Input;

namespace Singularity.Input
{
    public sealed class KeyEvent
    {
        public KeyEvent(Keys[] currentKeys)
        {
            CurrentKeys = currentKeys;
        }

        public Keys[] CurrentKeys { get; }
    }
}
