using Microsoft.Xna.Framework;

namespace Singularity.Property
{
    internal interface ICollider : ISpatial
    {
        Rectangle AbsBounds { get; }

        bool Moved { get; }

        int Id { get; }
    }
}
