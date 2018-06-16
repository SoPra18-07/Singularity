using Microsoft.Xna.Framework;

namespace Singularity.Property
{

    /// <summary>
    /// This interface provides necessary properties for, well, spatial objects. (german: räumlich)
    /// The idea is that every object that should get affected by the camera should also be
    /// a spatial object, since it moves according to the cameras values. The relative coordinates
    /// thus store the "actual position on the current screen in relation to the camera". Those
    /// should be used when checking for the current position of this object.
    /// </summary>
    internal interface ISpatial : IDraw, IUpdate
    {
        /// <summary>
        /// The relative position of this object. Relative to the camera that is. These
        /// are used for position queries, for example to check for collision or mouseclicks on
        /// this object. Don't use this in this objects draw method. This position gets
        /// set by the GameScreen every update.
        /// </summary>
        Vector2 RelativePosition { set; get; }

        /// <summary>
        /// The relative size of this object. Relative to the camera that is. These
        /// are used for position queries, for example to check for collision or mouseclicks on
        /// this object. Don't use this in this objects draw method. This size gets
        /// set by the GameScreen every update.
        /// </summary>
        Vector2 RelativeSize { set; get; }

        /// <summary>
        /// The absolute position of this object. Unaffected by zoom/movement. These
        /// are used for the internal drawing method. These get set once for static objects.
        /// </summary>
        Vector2 AbsolutePosition { set; get; }

        /// <summary>
        /// The absolute size of this object. Unaffected by zoom. These
        /// are used for the internal drawing method. These get normally set once in the game.
        /// </summary>
        Vector2 AbsoluteSize { set; get; }

    }
}
