using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Screen;

namespace Singularity.Units
{
    class SelectionBox : IDraw, IUpdate, IMouseClickListener, IMousePositionListener
    {
        private Color mColor;
        private Vector2 mStartBox;
        private Vector2 mSizeBox;
        private bool mBoxExists;

        private float mXStart;
        private float mYStart;

        public delegate void SelectionBoxEventHandler(object source, EventArgs args, Vector2 leftCorner, Vector2 size);
        public event SelectionBoxEventHandler SelectingBox;

        private readonly Camera mCamera;
        private Director mDirector;

        /// <summary>
        /// Creates a selection box
        /// </summary>
        /// <param name="color"></param>
        /// <param name="camera"></param>
        /// <param name="director"></param>
        public SelectionBox(Color color, Camera camera, ref Director director)
        {
            mColor = color;
            mCamera = camera;

            mDirector = director;

            director.GetInputManager.AddMouseClickListener(iMouseClickListener: this, leftClickType: EClickType.Both, rightClickType: EClickType.Both);
            director.GetInputManager.AddMousePositionListener(iMouseListener: this);
        }


        // event includes location an size of created selection box
        private void OnSelectingBox()
        {
            SelectingBox?.Invoke(source: this, args: EventArgs.Empty, leftCorner: new Vector2(x: mXStart, y: mYStart), size: mSizeBox);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // if selection box has been created by user then draw
            if (mBoxExists)
            {

                mXStart = mStartBox.X;
                mYStart = mStartBox.Y;

                if (MouseCoordinates().X < mStartBox.X)
                {
                    mXStart = MouseCoordinates().X;
                }

                if (MouseCoordinates().Y < mStartBox.Y)
                {
                    mYStart = MouseCoordinates().Y;
                }

                spriteBatch.StrokedRectangle(location: new Vector2(x: mXStart, y: mYStart), size: mSizeBox, colorBorder: Color.White, colorCenter: Color.White, opacityBorder: .8f, opacityCenter: .5f);

            }
        }

        public void Update(GameTime gameTime)
        {
            mSizeBox = new Vector2(x: Math.Abs(value: mStartBox.X - MouseCoordinates().X), y: Math.Abs(value: mStartBox.Y - MouseCoordinates().Y));
        }

        public EScreen Screen { get; }

        public Rectangle Bounds { get; }

        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            switch (mouseAction)
            {
                // if left click and selection box doesnt exist then create one
                case EMouseAction.LeftClick:
                    if (!mBoxExists)
                    {
                        mBoxExists = true;
                        mStartBox = MouseCoordinates();
                        mSizeBox = MouseCoordinates();
                    }
                    return false;
            }

            return true;
        }




        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            switch (mouseAction)
            {
                // once box has been created and released, send out event with box dimensions
                case EMouseAction.LeftClick:
                    if (mBoxExists)
                    {

                        mBoxExists = false;
                        if (mSizeBox.X > 2 && mSizeBox.Y > 2)
                        {
                            mXStart = mStartBox.X;
                            mYStart = mStartBox.Y;
                            if (MouseCoordinates().X < mStartBox.X)
                            {
                                mXStart = MouseCoordinates().X;
                            }

                            if (MouseCoordinates().Y < mStartBox.Y)
                            {
                                mYStart = MouseCoordinates().Y;
                            }

                            OnSelectingBox();
                        }
                    }
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Used to find the coordinates of the mouse based on the overall all map
        /// instead of just the camera shot, returns Vector2 of absolute position
        /// </summary>
        /// <returns></returns>
        private Vector2 MouseCoordinates()
        {
            return new Vector2(x: Vector2.Transform(position: new Vector2(x: Mouse.GetState().X, y: Mouse.GetState().Y),
                matrix: Matrix.Invert(matrix: mCamera.GetTransform())).X, y: Vector2.Transform(position: new Vector2(x: Mouse.GetState().X, y: Mouse.GetState().Y),
                matrix: Matrix.Invert(matrix: mCamera.GetTransform())).Y);
        }

        #region NotUsedInputMouseActions
        public void MousePositionChanged(float newX, float newY)
        {

        }


        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }
        #endregion


    }


}
