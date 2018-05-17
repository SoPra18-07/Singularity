using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Map.Properties;
using Singularity.Property;

namespace Singularity.Map
{
    internal sealed class Camera : IUpdate
    {

        private const int CameraMovementSpeed = 10;

        private readonly Viewport mViewport;

        private int mX;

        private int mY;

        private float mZoom;

        private Matrix mTransform;

        private readonly Rectangle mBounds;

        private int mOldScrollWheelValue;

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

        public Matrix GetTransform()
        {
            return mTransform;
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

            mTransform = Matrix.CreateScale(new Vector3(mZoom, mZoom, 1)) * Matrix.CreateTranslation(-mX, -mY, 0);
            ValidatePosition();
            mTransform = Matrix.CreateScale(new Vector3(mZoom, mZoom, 1)) * Matrix.CreateTranslation(-mX, -mY, 0);
        }

        private void ValidatePosition()
        {
            //The current top left point of the camera in world space.
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
    }
}
