using Microsoft.Xna.Framework;

namespace Singularity.Input
{
    internal sealed class MouseEvent
    {
        public Vector2 Position { get; } = Vector2.One;

        public EMouseAction MouseAction { get; } = EMouseAction.LeftClick; // solve warnings for now


        public enum EMouseAction
        {
            LeftClick,
            RightClick
        };
    }
}
