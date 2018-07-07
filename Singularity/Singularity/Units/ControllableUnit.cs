using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Map.Properties;

namespace Singularity.Units
{
    /// <inheritdoc cref="IMouseClickListener"/>
    /// <inheritdoc cref="IMousePositionListener"/>
    /// <inheritdoc cref="FreeMovingUnit"/>
    internal abstract class ControllableUnit : FreeMovingUnit, IMouseClickListener, IMousePositionListener
    {
        #region Fields

        /// <summary>
        /// Indicates if the unit is currently selected.
        /// </summary>
        internal bool mSelected;

        /// <summary>
        /// Stores the current x position of the mouse
        /// </summary>
        internal float mMouseX;

        /// <summary>
        /// Stores the current y position of the mouse
        /// </summary>
        internal float mMouseY;

        #endregion

        /// <summary>
        /// Provides an abstract superclass for all controllable units.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        /// <param name="map">Reference to the game map.</param>
        /// This abstract class is a subclass of FreeMovingUnit. It provides any of its subclasses the
        /// ability to be selected and moved by the player. As such, most of this class is simply
        /// mouse event handlers. Any object that is a subclass of this class is immediately
        /// subscribed to mouse events by the gamescreen.
        protected ControllableUnit(Vector2 position, Camera camera, ref Director director, ref Map.Map map)
            : base(position: position, camera: camera, director: ref director, map: ref map)
        {
            mDirector.GetInputManager.AddMouseClickListener(iMouseClickListener: this, leftClickType: EClickType.Both, rightClickType: EClickType.Both);
            mDirector.GetInputManager.AddMousePositionListener(iMouseListener: this);
        }

        #region Mouse Handlers
        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            // todo: someone look at the ReSharper warning following here:
            var giveThrough = true;

            switch (mouseAction)
            {
                case EMouseAction.LeftClick:
                    // check for if the unit is selected, not moving, the click is not within the bounds of the unit, and the click was on the map.
                    if (mSelected
                        && !mIsMoving
                        && !withinBounds
                        && Map.Map.IsOnTop(rect: new Rectangle(x: (int)(mMouseX - RelativeSize.X / 2f),
                                y: (int)(mMouseY - RelativeSize.Y / 2f),
                                width: (int)RelativeSize.X,
                                height: (int)RelativeSize.Y),
                            camera: mCamera))
                    {
                        mTargetPosition = Vector2.Transform(position: new Vector2(x: Mouse.GetState().X, y: Mouse.GetState().Y),
                            matrix: Matrix.Invert(matrix: mCamera.GetTransform()));

                        if (mMap.GetCollisionMap().GetWalkabilityGrid().IsWalkableAt(
                            iX: (int)mTargetPosition.X / MapConstants.GridWidth,
                            iY: (int)mTargetPosition.Y / MapConstants.GridWidth))
                        {
                            FindPath(currentPosition: Center, targetPosition: mTargetPosition);
                        }
                    }


                    if (withinBounds)
                    {
                        mSelected = true;
                        giveThrough = false;
                    }

                    break;

                case EMouseAction.RightClick:
                    mSelected = false;
                    break;
            }

            return giveThrough;
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            mMouseX = screenX;
            mMouseY = screenY;
        }

        /// <summary>
        /// This is called up every time a selection box is created
        /// if MUnit bounds intersects with the selection box then it become selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="position"> top left corner of the selection box</param>
        /// <param name="size"> size of selection box</param>
        public void BoxSelected(object sender, EventArgs e, Vector2 position, Vector2 size)
        {
            // create a rectangle from given parameters
            Rectangle selBox = new Rectangle(x: (int)position.X, y: (int)position.Y, width: (int)size.X, height: (int)size.Y);

            // check if selection box intersects with MUnit bounds
            if (selBox.Intersects(value: AbsBounds))
            {
                mSelected = true;
            }
        }

        #endregion
    }
}
