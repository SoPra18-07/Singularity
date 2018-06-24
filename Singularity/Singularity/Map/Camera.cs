﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map.Properties;
using Singularity.Property;

namespace Singularity.Map
{
    //TODO: update in such a way that zoom is centered on the current mouse position
    /// <inheritdoc/>
    /// <remarks>
    /// The camera object is used to move and zoom the map and all its components.
    /// </remarks>
    internal sealed class Camera : IUpdate, IKeyListener, IMouseWheelListener
    {

        /// <summary>
        /// The speed at which the camera moves in pixels per update.
        /// </summary>
        private const int CameraMovementSpeed = 10;

        /// <summary>
        /// The viewport of the window, e.g. the current size of it.
        /// </summary>
        private readonly Viewport mViewport;

        /// <summary>
        /// The x location of the camera unzoomed. Could also be called the "true" or "absolute" x location.
        /// </summary>
        private float mX;

        /// <summary>
        /// The y location of the camera unzoomed. Could also be called the "true" or "absolute" y location.
        /// </summary>
        private float mY;

        /// <summary>
        /// The current zoom value of the camera.
        /// </summary>
        private float mZoom;

        /// <summary>
        /// The matrix used to transform every position to the actual camera view.
        /// </summary>
        private Matrix mTransform;

        /// <summary>
        /// The bounding box of the map used, so the camera cannot move out of bounds.
        /// </summary>
        private readonly Rectangle mBounds;


        /// <summary>
        /// Creates a new Camera object which provides a transform matrix to adjust
        /// objects to the camera view.
        /// </summary>
        /// <param name="viewport">The viewport of the window</param>
        /// <param name="director">The director</param>
        /// <param name="x">The initial x position of the camera</param>
        /// <param name="y">the initial y position of the camera</param>
        public Camera(Viewport viewport, ref Director director, int x = 0, int y = 0)
        {
            if (x < 0)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = 0;
            }

            mX = x;
            mY = y;
            mViewport = viewport;
            mZoom = 1.0f;
            mBounds = new Rectangle(0, 0, MapConstants.MapWidth, MapConstants.MapHeight);

            director.GetInputManager.AddKeyListener(this);
            director.GetInputManager.AddMouseWheelListener(this);

            mTransform = Matrix.CreateScale(new Vector3(mZoom, mZoom, 1)) * Matrix.CreateTranslation(-mX, -mY, 0);

        }

        /// <summary>
        /// Gets the current transform matrix of the camera. Used to
        /// adjust objects to the camera view.
        /// </summary>
        /// <returns>The mentioned matrix</returns>
        public Matrix GetTransform()
        {
            return mTransform;
        }

        public void Update(GameTime gametime)
        {

            //finally update the matrix to all the fitting values.
            UpdateTransformMatrix();
        }

        /// <summary>
        /// Gets the current zoom level of this camera.
        /// </summary>
        /// <returns>The zoom mentioned</returns>
        public float GetZoom()
        {
            return mZoom;
        }

        // TODO: update this method such that rounding will not cause slight out of map clipping when zoomed in
        /// <summary>
        /// Checks whether the camera would move out of bounds and corrects the camera to
        /// clip to the edge if its the case.
        /// </summary>
        private void ValidatePosition()
        {
            // first of all we need to update our matrix with the new values, since they got changed by moving.
            UpdateTransformMatrix();

            /*
             * The current top left point of the camera in world space. This isn't too complicated either.
             * We know that all our objects get multiplied by our transform matrix. Thus if we want
             * to know the "true" top left point of the camera in world space, we need to revert the
             * multiplication for the point we want to know, thus multiplying by the inverse matrix.
             * vector zero is simply the origin point of the camera view. (top-left).
             */
            var cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(mTransform));

            //The current scope of the camera which gets changed by the zoom
            var cameraSize = new Vector2(mViewport.Width, mViewport.Height) / mZoom;

            //The vectors which represents the (top left)/(right bottom) corner of the bounding box, used to not move over
            var limitWorldMin = new Vector2(mBounds.Left, mBounds.Top);
            var limitWorldMax = new Vector2(mBounds.Right, mBounds.Bottom);

            //The offset created by zooming.
            var positionOffsetX = mX - cameraWorldMin.X;
            var positionOffsetY = mY - cameraWorldMin.Y;

            //finally adjust the values by the given bounds.
            mX = (int) (MathHelper.Clamp(cameraWorldMin.X, limitWorldMin.X, limitWorldMax.X - cameraSize.X) +
                        positionOffsetX);
            mY = (int) (MathHelper.Clamp(cameraWorldMin.Y, limitWorldMin.Y, limitWorldMax.Y - cameraSize.Y) +
                        positionOffsetY);

        }

        ///<summary>
        /// This isn't as complicated as one might think. the first matrix we create looks as follows:
        ///
        ///     [    mZoom   0       0   ]
        /// A = [    0       mZoom   0   ]
        ///     [    0       0       1   ]
        ///
        /// The translation matrix looks as follows:
        ///
        ///     [    1   0   -mX ]
        /// B = [    0   1   -mY ]
        ///     [    0   0   1   ]
        ///
        /// The matrix product would look as follows:
        ///
        ///          [   mZoom   0       -mZoom * mX ]   (x)     (mZoom * x - z * mZoom * mX )       (mZoom * x - mZoom * mX )
        /// A * B =  [   0       mZoom   -mZoom * mY ] * (y) =   (mZoom * y - z * mZoom * mY )   =   (mZoom * y - mZoom * mY )
        ///          [   0       0       1           ]   (z)     (z                          )       (1                      )
        ///
        /// which does make sense,  since we're operating in a 2D room with 2 coordinates. First we adjust our original x coordinate
        /// by the zoom factor. The higher our camera positional values are the more we have to move the object away. Imagine moving
        /// the camera to the right, all the objects have to be moved to the left then to adjust to the camera view. The amount we subtract
        /// also has to get adjusted by the zoom. And thats all we need to do. Wrapped in a convinient matrix.
        ///
        /// For reference also check:
        /// https://en.wikipedia.org/wiki/Translation_(geometry)
        /// https://en.wikipedia.org/wiki/Scaling_(geometry)
        ///</summary>
        private void UpdateTransformMatrix()
        {
            mTransform = Matrix.CreateScale(new Vector3(mZoom, mZoom, 1)) * Matrix.CreateTranslation(-mX, -mY, 0);
        }

        public void KeyTyped(KeyEvent keyEvent)
        {

        }

        public void KeyPressed(KeyEvent keyEvent)
        {
            var moved = false;

            foreach (var key in keyEvent.CurrentKeys)
            {

                switch (key)
                {
                    case Keys.W:
                        mY -= CameraMovementSpeed;
                        moved = true;
                        break;

                    case Keys.S:
                        mY += CameraMovementSpeed;
                        moved = true;
                        break;

                    case Keys.A:
                        mX -= CameraMovementSpeed;
                        moved = true;
                        break;

                    case Keys.D:
                        mX += CameraMovementSpeed;
                        moved = true;
                        break;
                }
            }

            if (moved)
            {
                ValidatePosition();
            }
        }

        public void KeyReleased(KeyEvent keyEvent)
        {

        }

        public void MouseWheelValueChanged(EMouseAction mouseAction)
        {
            var scrollChange = 0f;

            switch (mouseAction)
            {
                case EMouseAction.ScrollUp:
                    scrollChange = 0.1f;
                    break;

                case EMouseAction.ScrollDown:
                    scrollChange = -0.1f;
                    break;
            }

            if (!((mZoom + scrollChange) * MapConstants.MapWidth < mViewport.Width ||
                  (mZoom + scrollChange) * MapConstants.MapHeight < mViewport.Height))
            {
                mZoom += scrollChange;
            }
        }

        public Matrix GetStencilProjection()
        {

            var cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(mTransform));

            return Matrix.CreateOrthographicOffCenter(cameraWorldMin.X,
                cameraWorldMin.X + (mViewport.Width / mZoom),
                cameraWorldMin.Y + (mViewport.Height / mZoom),
                cameraWorldMin.Y,
                0,
                1);
        }
    }
}

