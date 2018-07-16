using Microsoft.Xna.Framework;

namespace Singularity.Property
{
    public interface IRevealing
    {
        bool Friendly { get; }

        int RevelationRadius { get; }

        Vector2 Center { get; }
    }
}
