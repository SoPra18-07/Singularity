using Microsoft.Xna.Framework;

namespace Singularity.Input
{
    internal sealed class MouseEvent
    {
        public MouseEvent(EMouseAction mouseAction, Vector2 position)
        {
            Position = position;
            MouseAction = mouseAction;
        }
        private Vector2 Position { get; }

        private EMouseAction MouseAction { get; }
    }
}
