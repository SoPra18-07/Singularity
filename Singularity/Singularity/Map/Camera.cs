using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Map.Properties;
using Singularity.Property;

namespace Singularity.Map
{   
    /// <inheritdoc/>
    /// <remarks>
    /// The camera object is used to move and zoom the map and all its components.
    /// </remarks>
    internal sealed class Camera : IUpdate
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
        private int mX;

        /// <summary>
        /// The y location of the camera unzoomed. Could also be called the "true" or "absolute" y location.
        /// </summary>
        private int mY;

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
        /// The scroll wheel value which is always 1 update behind the actual update.
        /// </summary>
        private int mOldScrollWheelValue;

        /// <summary>
        /// Creates a new Camera object which provides a transform matrix to adjust
        /// objects to the camera view.
        /// </summary>
        /// <param name="viewport">The viewport of the window</param>
        /// <param name="x">The initial x position of the camera</param>
        /// <param name="y">the initial y position of the camera</param>
        public Camera(Viewport viewport, int x = 0, int y = 0)
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
            mOldScrollWheelValue = 0;
            mBounds = new Rectangle(0, 0, MapConstants.MapWidth, MapConstants.MapHeight);

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

        public float GetZoom()
        {
            return mZoom;
        }

        //TODO: remove this when input manager is there, since we don't need to fetch it anymore
        public void Update(GameTime gametime)
        {

            // camera movement related stuff
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                mY -= CameraMovementSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                mY += CameraMovementSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                mX -= CameraMovementSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                mX += CameraMovementSpeed;
            }
            ValidatePosition();


            // camera scroll related stuff
            var scaleChange = 0f;

            if (Mouse.GetState().ScrollWheelValue - mOldScrollWheelValue < 0)
            {
                scaleChange = -0.1f;
            }
            else if (Mouse.GetState().ScrollWheelValue - mOldScrollWheelValue > 0)
            {
                scaleChange = 0.1f;
            }

            if (!((mZoom + scaleChange) * MapConstants.MapWidth < mViewport.Width ||
                  (mZoom + scaleChange) * MapConstants.MapHeight < mViewport.Height))
            {
                mZoom += scaleChange;
            }

            mOldScrollWheelValue = Mouse.GetState().ScrollWheelValue;

            /*
             * This isn't as complicated as one might think. the first matrix we create looks as follows:
             *
             *     [    mZoom   0       0   ]
             * A = [    0       mZoom   0   ]
             *     [    0       0       1   ]
             *
             * The translation matrix looks as follows:
             *
             *     [    1   0   -mX ]
             * B = [    0   1   -mY ]
             *     [    0   0   1   ]
             *
             * The matrix product would look as follows:
             *
             *          [   mZoom   0       -mZoom * mX ]   (x)     (mZoom * x - z * mZoom * mX )       (mZoom * x - mZoom * mX )
             * A * B =  [   0       mZoom   -mZoom * mY ] * (y) =   (mZoom * y - z * mZoom * mY )   =   (mZoom * y - mZoom * mY )
             *          [   0       0       1           ]   (z)     (z                          )       (1                      )
             *
             * which does make sense,  since we're operating in a 2D room with 2 coordinates. First we adjust our original x coordinate
             * by the zoom factor. The higher our camera positional values are the more we have to move the object away. Imagine moving
             * the camera to the right, all the objects have to be moved to the left then to adjust to the camera view. The amount we subtract
             * also has to get adjusted by the zoom. And thats all we need to do. Wrapped in a convinient matrix.
             *
             */
            mTransform = Matrix.CreateScale(new Vector3(mZoom, mZoom, 1)) * Matrix.CreateTranslation(-mX, -mY, 0);
        }

        /// <summary>
        /// Checks whether the camera would move out of bounds and corrects the camera to
        /// clip to the edge if its the case.
        /// </summary>
        private void ValidatePosition()
        {
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

        public Rectangle GetAsView(int x, int y, int width, int height)
        {
            var newPos = Vector2.Transform(new Vector2(x, y), mTransform);
            var newSize = new Vector2(width * mZoom, height * mZoom);

            return new Rectangle((int) newPos.X, (int) newPos.Y, (int) newSize.X, (int) newSize.Y);
        }
        public Rectangle GetAsView(Rectangle rectangle)
        {
            return GetAsView(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}
