using Microsoft.Xna.Framework;
using Singularity.Property;

namespace Singularity.Screen
{
    internal interface IWindowItem : IUpdate, IDraw
    {
        /// <summary>
        /// The IWindowItem's position
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// The IWindowItem's size
        /// </summary>
        Vector2 Size { get; }

        /// <summary>
        /// Bool to determine whether this item is active in the window or not
        /// </summary>
        bool ActiveInWindow { get; set; }

        /// <summary>
        /// This value is only used by the selectedPlatformWindow if this IWindowItem should be deactivated due to folding up of a section.
        /// If false, the item is active!
        /// </summary>
        bool InactiveInSelectedPlatformWindow { get; set; }

        /// <summary>
        /// This value is used by the windowObject to deactivate the IWindowItem if it completely goes out of the scissor rectangle.
        /// This prevents buttons/.. from being active while outside the window
        /// </summary>
        bool OutOfScissorRectangle { get; set; }
    }
}