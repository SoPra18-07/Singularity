using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Map.Properties;
using Singularity.Utils;

namespace Singularity.Units
{
    /// <inheritdoc cref="IMouseClickListener"/>
    /// <inheritdoc cref="IMousePositionListener"/>
    /// <inheritdoc cref="FreeMovingUnit"/>
    [DataContract]
    internal abstract class ControllableUnit : FreeMovingUnit, IMouseClickListener, IMousePositionListener
    {
        #region Fields

        /// <summary>
        /// Indicates if the unit is currently selected.
        /// </summary>
        [DataMember]
        internal bool mSelected;
        
        #endregion

        /// <summary>
        /// Provides an abstract superclass for all controllable units.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        /// This abstract class is a subclass of FreeMovingUnit. It provides any of its subclasses the
        /// ability to be selected and moved by the player. As such, most of this class is simply
        /// mouse event handlers. Any object that is a subclass of this class is immediately
        /// subscribed to mouse events by the gamescreen.
        protected ControllableUnit(Vector2 position, Camera camera, ref Director director, bool friendly = true)
            : base(position, camera, ref director, friendly)
        {
            mDirector.GetInputManager.FlagForAddition(this, EClickType.Both, EClickType.Both);
            mDirector.GetInputManager.AddMousePositionListener(this);
            mGroup = Optional<FlockingGroup>.Of(null);
        }

        protected new void ReloadContent(ref Director director, Camera camera, ref Map.Map map)
        {
            base.ReloadContent(ref director, camera);
            mDirector.GetInputManager.FlagForAddition(this, EClickType.Both, EClickType.Both);
            mDirector.GetInputManager.AddMousePositionListener(this);
        }

        #region Mouse Handlers
        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            var giveThrough = true;

            switch (mouseAction)
            {
                case EMouseAction.LeftClick:
                    // check for if the unit is selected, not moving, the click is not within the bounds of the unit, and the click was on the map.
                    if (mSelected
                        // && !Moved // now this should do pathfinding even while moving
                        && !withinBounds
                        && Map.Map.IsOnTop(new Rectangle((int)(mMouseX - RelativeSize.X / 2f),
                                (int)(mMouseY - RelativeSize.Y / 2f),
                                (int)RelativeSize.X,
                                (int)RelativeSize.Y),
                            mCamera))
                    {
                        if (!mGroup.IsPresent())
                        {
                            var group = Optional<FlockingGroup>.Of(mDirector.GetMilitaryManager.GetNewFlock());
                            group.Get().AssignUnit(this);
                            mGroup = group;
                            // do the fuck not change these lines here. Costs you at least 3h for debugging.
                        }

                        var target = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                            Matrix.Invert(mCamera.GetTransform()));

                        if (mGroup.Get().Map.GetCollisionMap().GetWalkabilityGrid().IsWalkableAt(
                            (int)target.X / MapConstants.GridWidth,
                            (int)target.Y / MapConstants.GridHeight))
                        {
                            mGroup.Get().FindPath(target);
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
                mDirector.GetMilitaryManager.AddSelected(this); // send to FlockingManager
            }
        }

        #endregion
    }
}
