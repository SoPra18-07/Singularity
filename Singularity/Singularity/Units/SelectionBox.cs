using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <param name="manager"></param>
        public SelectionBox(Color color, Camera camera, ref Director director)
        {
            mColor = color;
            mCamera = camera;

            mDirector = director;

            director.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
            director.GetInputManager.AddMousePositionListener(this);
        }


        // event includes location an size of created selection box
        private void OnSelectingBox()
        {
            SelectingBox?.Invoke(this, EventArgs.Empty, new Vector2(mXStart, mYStart), mSizeBox);
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

                spriteBatch.StrokedRectangle(new Vector2(mXStart, mYStart), mSizeBox, Color.White, Color.White, .8f, .5f);

            }
        }

        public void Update(GameTime gameTime)
        {
            mSizeBox = new Vector2(Math.Abs(mStartBox.X - MouseCoordinates().X), Math.Abs(mStartBox.Y - MouseCoordinates().Y));
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
            return new Vector2(Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                Matrix.Invert(mCamera.GetTransform())).X, Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                Matrix.Invert(mCamera.GetTransform())).Y);
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
