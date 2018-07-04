using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Map.Properties;
using Singularity.Screen;

namespace Singularity.Units
{
    /// <inheritdoc cref="IMouseClickListener"/>
    /// <inheritdoc cref="IMousePositionListener"/>
    abstract class ControllableUnit : FreeMovingUnit, IMouseClickListener, IMousePositionListener
    {
        /// <summary>
        /// Indicates if the unit is currently selected.
        /// </summary>
        internal bool mSelected;

        internal float mMouseX;

        internal float mMouseY;

        public EScreen Screen { get; } = EScreen.GameScreen;

        /// <summary>
        /// Provides an abstract superclass for all controllable units
        /// </summary>
        /// <param name="position"></param>
        /// <param name="camera"></param>
        /// <param name="director"></param>
        /// <param name="map"></param>
        protected ControllableUnit(Vector2 position, Camera camera, ref Director director, ref Map.Map map)
            : base(position, camera, ref director, ref map)
        {
            

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
                        && Map.Map.IsOnTop(new Rectangle((int)(mMouseX - RelativeSize.X / 2f),
                                (int)(mMouseY - RelativeSize.Y / 2f),
                                (int)RelativeSize.X,
                                (int)RelativeSize.Y),
                            mCamera))
                    {
                        if (mMap.GetCollisionMap().GetWalkabilityGrid().IsWalkableAt(
                            (int)mTargetPosition.X / MapConstants.GridWidth,
                            (int)mTargetPosition.Y / MapConstants.GridWidth))
                        {
                            mTargetPosition = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                                Matrix.Invert(mCamera.GetTransform()));

                            FindPath(mTargetPosition, Center);
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
            Rectangle selBox = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

            // check if selection box intersects with MUnit bounds
            if (selBox.Intersects(AbsBounds))
            {
                mSelected = true;
            }
        }

        #endregion
    }
}
