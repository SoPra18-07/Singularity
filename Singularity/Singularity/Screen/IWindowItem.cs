using Microsoft.Xna.Framework;
using Singularity.Property;

namespace Singularity.Screen
{
    interface IWindowItem : IUpdate, IDraw
    {
        //TODO implement the methods specific to IWindowItem
        Vector2 Position { get; set; }

        Vector2 Size { get; }

        bool Active { get; set; }
    }
}