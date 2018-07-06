using Microsoft.Xna.Framework;
using Singularity.Property;

namespace Singularity.Screen
{
    public interface IWindowItem : IUpdate, IDraw
    {
        Vector2 Position { get; set; }

        Vector2 Size { get; }

        bool ActiveWindow { get; set; }
    }
}