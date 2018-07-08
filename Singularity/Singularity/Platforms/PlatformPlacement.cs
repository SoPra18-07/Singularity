using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Utils;

namespace Singularity.Platforms
{
    /// <summary>
    /// This handles platforms which can get placed on the game screen as objects.
    /// </summary>
    public sealed class PlatformPlacement : IDraw, IUpdate, IMousePositionListener, IMouseClickListener
    {
        public EScreen Screen { get; private set; }
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// A 3 state machine if you will.
        /// State 1 (inital): Platform follows the mouse and left click triggers next state
        /// State 2 (road)  : The platform is locked in place and a road is needs to be connected to another platform.
        /// last state with right click, and next state when a road is connected.
        /// State 3 (add)   : A new platform object gets added to the structure map.
        /// </summary>
        private readonly State3 mCurrentState;

        /// <summary>
        /// Whether to only follow the mouse or not
        /// </summary>
        private readonly bool mMouseFollowOnly;

        /// <summary>
        /// Whether the placement is finished or not
        /// </summary>
        private bool mIsFinished;

        /// <summary>
        /// The platform to place
        /// </summary>
        private readonly PlatformBlank mPlatform;

        /// <summary>
        /// The platform which is currently hovered.
        /// </summary>
        private PlatformBlank mHoveringPlatform;

        /// <summary>
        /// The current road that needs to get connected to another platform
        /// </summary>
        private Road mConnectionRoad;

        /// <summary>
        /// The world space X coordinate of the mouse
        /// </summary>
        private float mMouseX;

        /// <summary>
        /// The world space Y coordinate of the mouse
        /// </summary>
        private float mMouseY;

        private bool mCanceled;

        private readonly Camera mCamera;

        private readonly Director mDirector;

        private bool mUnregister;

        public PlatformPlacement(EPlatformType platformType, EPlacementType placementType, EScreen screen, Camera camera, ref Director director, float x = 0, float y = 0, ResourceMap resourceMap = null, Vector2 position = default(Vector2))
        {
            mUnregister = false;

            mCamera = camera;
            Screen = screen;
            mDirector = director;

            mDirector.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
            mDirector.GetInputManager.AddMousePositionListener(this);

            // for further information as to why which states refer to the documentation for mCurrentState
            switch (placementType)
            {
                case EPlacementType.Instant:
                    mCurrentState = new State3(3);
                    break;

                case EPlacementType.MouseFollowAndRoad:
                    mCurrentState = new State3(1);
                    break;

                case EPlacementType.OnlyMouseFollow:
                    mCurrentState = new State3(1);
                    mMouseFollowOnly = true;
                    break;

                case EPlacementType.OnlyRoad:
                    mCurrentState = new State3(2);
                    break;

                default:
                    break;
            }

            mPlatform = PlatformFactory.Get(platformType, ref director, x, y, resourceMap, false);
            mPlatform.SetLayer(LayerConstants.PlatformAboveFowLayer);

            UpdateBounds();

        }

        /// <summary>
        /// Updates the relative size of this platform and its bounds. Since this is at this time not an object of the game screen those
        /// values don't get updated automatically.
        /// </summary>
        private void UpdateBounds()
        {
            mPlatform.RelativePosition = Vector2.Transform(mPlatform.AbsolutePosition, mCamera.GetTransform());
            mPlatform.RelativeSize = mPlatform.AbsoluteSize * mCamera.GetZoom();
            Bounds = new Rectangle((int) mPlatform.RelativePosition.X, (int) mPlatform.RelativePosition.Y, (int) mPlatform.RelativeSize.X, (int) mPlatform.RelativeSize.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //make sure to draw the platform and the road if available.
            mPlatform.Draw(spriteBatch);
            mConnectionRoad?.Draw(spriteBatch);
        }

        public void Update(GameTime gametime)
        {
            switch (mCurrentState.GetState())
            {
                case 1:
                    mPlatform.ResetColor();
                    // for this, we want the platform to follow the mouse, and also be centered on the sprite.
                    mPlatform.AbsolutePosition = new Vector2(mMouseX - mPlatform.AbsoluteSize.X / 2f,
                        mMouseY - mPlatform.AbsoluteSize.Y / 2f);

                    if (mHoveringPlatform == null)
                    {
                        break;
                    }
                    mPlatform.SetColor(Color.Red);

                    break;

                case 2:
                    // now we want a road to follow our mouse
                    mConnectionRoad.Destination = new Vector2(mMouseX, mMouseY);

                    // we prematurely reset the color of the platform, so we don't have to worry about it being red
                    mPlatform.ResetColor();
                    if (mHoveringPlatform == null)
                    {
                        break;

                    }
                    // at this point we have a hovering platform, so we clip the road destination to its center
                    mConnectionRoad.Destination = mHoveringPlatform.Center;

                    // we only color the platform red if the distance to the platform hovered is too great
                    if (Vector2.Distance(mHoveringPlatform.Center, mPlatform.Center) >
                        mPlatform.RevelationRadius + mHoveringPlatform.RevelationRadius)
                    {
                        mPlatform.SetColor(Color.Red);
                    }

                    break;

                case 3:
                    // this case is the 'finish' state, we set everything up, so the platform can get added to the game
                    mPlatform.SetLayer(LayerConstants.PlatformLayer);
                    mConnectionRoad.Blueprint = false;
                    mIsFinished = true;
                    mUnregister = true;
                    break;

                default:
                    break;
            }

            if (mUnregister)
            {
                UnregisterFromInputManager();
            }

            // don't forget to always update the relative position since the camera might have moved.
            UpdateBounds();
        }


        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            var giveThrough = true;

            if (mouseAction == EMouseAction.LeftClick)
            {
                switch (mCurrentState.GetState())
                {
                    case 1:
                        mPlatform.UpdateValues();

                        //first check if the platform is even on the map, if not we don't want to progress, since it isn't a valid position
                        if (!Map.Map.IsOnTop(mPlatform.AbsBounds) || mHoveringPlatform != null)
                        {
                            break;
                        }

                        // the platform was on the map -> advance to next state and create the road to connect to another platform
                        mCurrentState.NextState();
                        mConnectionRoad = new Road(mPlatform, null, true);

                        if (mMouseFollowOnly)
                        {
                            mCurrentState.NextState();
                            mConnectionRoad = null;
                        }

                        giveThrough = false;
                        break;

                    case 2:
                        if (mHoveringPlatform == null)
                        {
                            break;

                        }

                        // this limits two platforms to only be connectable by a road if the road isn't in the fog of war this was requested by felix
                        if (Vector2.Distance(mHoveringPlatform.Center, mPlatform.Center) <=
                                mPlatform.RevelationRadius + mHoveringPlatform.RevelationRadius)
                        {
                            mCurrentState.NextState();
                        }

                        giveThrough = false;

                        break;

                    default:
                        break;

                }

            }

            if (mouseAction == EMouseAction.RightClick)
            {
                if (mCurrentState.GetState() == 1)
                {
                    mCanceled = true;
                    mIsFinished = true;
                    giveThrough = false;
                    mUnregister = true;

                    return giveThrough;
                }

                // we only need to do something with rightclick if were in the 2nd state, since then we revert.
                if (mCurrentState.GetState() != 2)
                {
                    return giveThrough;
                }

                // make sure to reset colors when reverting to the last state. The rest is just some cleanup to properly
                // get to the previous state
                mPlatform.ResetColor();
                mConnectionRoad = null;
                mCurrentState.PreviousState();
                giveThrough = false;
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

            mMouseX = worldX;
            mMouseY = worldY;
        }

        /// <summary>
        /// Whether the platform is finished, and ready for game addition
        /// </summary>
        /// <returns>True, if the platform is finished, and ready for game addition, false otherwise</returns>
        public bool IsFinished()
        {
            return mIsFinished;
        }

        /// <summary>
        /// Sets the hovering platform.
        /// </summary>
        /// <param name="hovering">The platform which currently gets hovered by the user</param>
        public void SetHovering(PlatformBlank hovering)
        {
            mHoveringPlatform = hovering;
        }

        /// <summary>
        /// Gets the current platform created by this
        /// </summary>
        /// <returns>The platform mentioned</returns>
        public PlatformBlank GetPlatform()
        {
            return mPlatform;
        }

        /// <summary>
        /// Gets the road created by this
        /// </summary>
        /// <returns>The road mentioned</returns>
        public Road GetRoad()
        {
            return mConnectionRoad;
        }

        public bool IsCanceled()
        {
            return mCanceled;
        }

        private void UnregisterFromInputManager()
        {
            mDirector.GetInputManager.RemoveMouseClickListener(this);
            mDirector.GetInputManager.RemoveMousePositionListener(this);
        }
    }
}
