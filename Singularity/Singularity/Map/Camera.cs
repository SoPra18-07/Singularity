using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map.Properties;
using Singularity.Property;
using Singularity.Screen;

namespace Singularity.Map
{
    /// <inheritdoc/>
    /// <remarks>
    /// The camera object is used to move and zoom the map and all its components.
    /// </remarks>
    public sealed class Camera : IKeyListener, IMouseWheelListener, IMousePositionListener
    {
        public EScreen Screen { get; private set; } = EScreen.GameScreen;

        private const float MaxZoom = 1.5f;

        /// <summary>
        /// The speed at which the camera moves in pixels per update.
        /// </summary>
        private const int CameraMovementSpeed = 10;

        /// <summary>
        /// The viewport of the window, e.g. the current size of it.
        /// </summary>
        private readonly GraphicsDevice mGraphics;

        private Vector2 mPosition;

        private Vector2 Position
        {
            get { return mPosition; }

            set
            {
                mPosition = value;
                ValidatePosition();
            }
        }

        private float mMouseX;

        private float mMouseY;

        /// <summary>
        /// The current zoom value of the camera.
        /// </summary>
        private float mZoom;

        private float Zoom
        {
            get { return mZoom; }
            set
            {
                mZoom = value;
                ValidateZoom();
                ValidatePosition();

            }
        }

        /// <summary>
        /// The matrix used to transform every position to the actual camera view.
        /// </summary>
        private Matrix mTransform;

        /// <summary>
        /// The bounding box of the map used, so the camera cannot move out of bounds.
        /// </summary>
        private readonly Rectangle mBounds;

        private readonly bool mNeo;

        private readonly InputManager mInputManager;

        private readonly Vector2 mOrigin;


        /// <summary>
        /// Creates a new Camera object which provides a transform matrix to adjust
        /// objects to the camera view.
        /// </summary>
        /// <param name="graphics">The viewport of the window</param>
        /// <param name="director">The director</param>
        /// <param name="x">The initial x position of the camera</param>
        /// <param name="y">the initial y position of the camera</param>
        /// <param name="neo">If the neo Layout should be used for navigating instead of qwertz</param>
        public Camera(GraphicsDevice graphics, ref Director director, int x = 0, int y = 0, bool neo = true)
        {

            if (x < 0)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = 0;
            }

            mOrigin = new Vector2(graphics.Viewport.Width / 2f, graphics.Viewport.Height / 2f);

            mZoom = 1.0f;
            mPosition = new Vector2(x, y);

            mNeo = neo;
            mGraphics = graphics;
            mBounds = new Rectangle(0, 0, MapConstants.MapWidth, MapConstants.MapHeight);
            mInputManager = director.GetInputManager;

            director.GetInputManager.AddKeyListener(this);
            director.GetInputManager.AddMouseWheelListener(this);
            director.GetInputManager.AddMousePositionListener(this);

            UpdateTransformMatrix();

        }

        /// <summary>
        /// Gets the current transform matrix of the camera. Used to
        /// adjust objects to the camera view.
        /// </summary>
        /// <returns>The mentioned matrix</returns>
        public Matrix GetTransform()
        {
            UpdateTransformMatrix();
            return mTransform;
        }

        /// <summary>
        /// Gets the current zoom level of this camera.
        /// </summary>
        /// <returns>The zoom mentioned</returns>
        public float GetZoom()
        {
            return mZoom;
        }

        /// <summary>
        /// Checks whether the camera would move out of bounds and corrects the camera to
        /// clip to the edge if its the case.
        /// </summary>
        private void ValidatePosition()
        {

            UpdateTransformMatrix();

            /*
             * The current top left point of the camera in world space. This isn't too complicated either.
             * We know that all our objects get multiplied by our transform matrix. Thus if we want
             * to know the "true" top left point of the camera in world space, we need to revert the
             * multiplication for the point we want to know, thus multiplying by the inverse matrix.
             * vector zero is simply the origin point of the camera view. (top-left).
             *
             */
            var cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(mTransform));

            //The current scope of the camera which gets changed by the zoom
            var cameraSize = new Vector2(mGraphics.Viewport.Width, mGraphics.Viewport.Height) / mZoom;

            //The vectors which represents the (top left)/(right bottom) corner of the bounding box, used to not move over
            var limitWorldMin = new Vector2(mBounds.Left, mBounds.Top);
            var limitWorldMax = new Vector2(mBounds.Right, mBounds.Bottom);

            //The offset created by zooming.
            var positionOffset = mPosition - cameraWorldMin;

            mPosition = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + positionOffset;

            UpdateTransformMatrix();
        }

        /// <summary>
        /// Zooms to the given zoomTarget by the amount specified
        /// </summary>
        /// <param name="zoomTarget">The target to zoom to</param>
        /// <param name="amount">The amount to zoom</param>
        private void ZoomToTarget(Vector2 zoomTarget, float amount)
        {
            var oldZoom = Zoom;

            Zoom += amount;

            //we don't want to move to move to the target if the zoom hasn't changed
            if (Math.Abs(Zoom - oldZoom) < float.Epsilon)
            {
                return;
            }

            var diff = Math.Sign(amount) * (mOrigin - zoomTarget) / Zoom;

            SetPosition(Position - diff / 10);

        }

        private void ValidateZoom()
        {

            if (mZoom > MaxZoom)
            {
                mZoom = MaxZoom;
            }

            var minZoomX = (float) mGraphics.Viewport.Width / mBounds.Width;
            var minZoomY = (float) mGraphics.Viewport.Height / mBounds.Height;

            mZoom = MathHelper.Max(mZoom, MathHelper.Max(minZoomX, minZoomY));
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
            mTransform = Matrix.CreateTranslation(new Vector3(-mPosition, 0f))
                         * Matrix.CreateTranslation(new Vector3(-mOrigin, 0f))
                         * Matrix.CreateScale(mZoom, mZoom, 1f)
                         * Matrix.CreateTranslation(new Vector3(mOrigin, 0f));
            mInputManager.CameraMoved(mTransform);
        }

        public bool KeyTyped(KeyEvent keyEvent)
        {
            return true;
        }

        public bool KeyPressed(KeyEvent keyEvent)
        {
            var movementVector = new Vector2();

            var giveThrough = true;

            if (mNeo)
            {
                foreach (var key in keyEvent.CurrentKeys)
                {
                    switch (key)
                    {
                        case Keys.V:
                            movementVector.Y = -CameraMovementSpeed;
                            giveThrough = false;
                            break;

                        case Keys.I:
                            movementVector.Y = CameraMovementSpeed;
                            giveThrough = false;
                            break;

                        case Keys.U:
                            movementVector.X = -CameraMovementSpeed;
                            giveThrough = false;
                            break;

                        case Keys.A:
                            movementVector.X = CameraMovementSpeed;
                            giveThrough = false;
                            break;
                    }
                }
            }
            else
            {
                foreach (var key in keyEvent.CurrentKeys)
                {
                    switch (key)
                    {
                        case Keys.W:
                            movementVector.Y = -CameraMovementSpeed;
                            giveThrough = false;
                            break;

                        case Keys.S:
                            movementVector.Y = CameraMovementSpeed;
                            giveThrough = false;
                            break;

                        case Keys.A:
                            movementVector.X = -CameraMovementSpeed;
                            giveThrough = false;
                            break;

                        case Keys.D:
                            movementVector.X = CameraMovementSpeed;
                            giveThrough = false;
                            break;
                    }
                }
            }

            // make sure to scale the movement vector with the zoom level, since we don't want super slow movement when zoomed out
            // and super fast movement when zoomed in
            Position = Position + movementVector * (1 / Zoom);

            return giveThrough;
        }

        public bool KeyReleased(KeyEvent keyEvent)
        {
            return true;
        }

        public bool MouseWheelValueChanged(EMouseAction mouseAction)
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
            ZoomToTarget(new Vector2(mMouseX, mMouseY), scrollChange * Zoom);

            return false;
        }

        public Matrix GetStencilProjection()
        {

            var cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(mTransform));

            return Matrix.CreateOrthographicOffCenter(cameraWorldMin.X,
                cameraWorldMin.X + GetSize().X,
                cameraWorldMin.Y + GetSize().Y,
                cameraWorldMin.Y,
                0,
                1);
        }

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            mMouseX = screenX;
            mMouseY = screenY;
        }

        public Vector2 GetRelativePosition()
        {
            return Vector2.Transform(Vector2.Zero, Matrix.Invert(mTransform));
        }

        private void SetPosition(Vector2 position)
        {
            Position = new Vector2(position.X, position.Y);
        }

        public void CenterOn(Vector2 position)
        {
            var size = GetSize();
            var rel = GetRelativePosition();
            var offset = Position - rel;

            SetPosition(new Vector2(position.X - (size.X / 2) + offset.X, position.Y - (size.Y / 2) + offset.Y));
        }

        public Vector2 GetSize()
        {
            return new Vector2(mGraphics.Viewport.Width, mGraphics.Viewport.Height) / Zoom;
        }
    }
}

