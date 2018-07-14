using System;
using System.Runtime.Serialization;
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
    [DataContract]
    public sealed class SelectionBox : IDraw, IUpdate, IMouseClickListener, IMousePositionListener
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
            director.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
            director.GetInputManager.AddMousePositionListener(this);
            mBoxExists = false;
        }


        // event includes location an size of created selection box
        private void OnSelectingBox()
        {
            SelectingBox?.Invoke(this, EventArgs.Empty, new Vector2(mXStart, mYStart), mSizeBox);
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            if (mBoxExists)
            {
                // if selection box has been created by user then draw
                spriteBatch.StrokedRectangle(new Vector2(mXStart, mYStart), mSizeBox, Color.White, Color.White, .8f, .5f, LayerConstants.FogOfWarLayer);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (mBoxExists)
            {
                mXStart = mStartBox.X;
                mYStart = mStartBox.Y;

                // calculate the left hand corner of the selection box based on
                // the starting position of where selection box was initiated
                if (MouseCoordinates().X < mStartBox.X)
                {
                    mXStart = MouseCoordinates().X;
                }

                if (MouseCoordinates().Y < mStartBox.Y)
                {
                    mYStart = MouseCoordinates().Y;
                }

                // caculate current size of selection box based on mouse position and point of start
                mSizeBox = new Vector2(Math.Abs(mStartBox.X - MouseCoordinates().X),
                    Math.Abs(mStartBox.Y - MouseCoordinates().Y));
            }
        }

        public EScreen Screen { get; } = EScreen.GameScreen;

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
                        mSizeBox = new Vector2(0, 0);
                        // can also be a simple click without a selection box, therefore pass on input
                        return true;
                    }

                    else
                    {
                        return false;
                    }
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

                        // if not an "accidental selection box" send out bounds
                        // mil unit checks if intersected and sets its selected bool as true if yet
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
                        return false;
                    }
                    return true;
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
        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {

        }


        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }
        #endregion


    }


}
