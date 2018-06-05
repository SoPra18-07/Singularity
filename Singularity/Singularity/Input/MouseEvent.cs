using Microsoft.Xna.Framework;

namespace Singularity.Input
{
    public sealed class MouseEvent
    {
        public MouseEvent(EMouseAction mouseAction, Vector2 position)
        {
            Position = position;
            MouseAction = mouseAction;
        }
        public Vector2 Position { get; }

        public EMouseAction MouseAction { get; }
    }
}