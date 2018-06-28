using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Utils;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Singularity.Platform
{
    public class PlatformPlacement : IDraw, IUpdate, IMousePositionListener, IMouseClickListener
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

        private readonly bool mMouseFollowOnly;

        private bool mIsFinished;

        private readonly PlatformBlank mPlatform;

        private PlatformBlank mHoveringPlatform;

        private Road mConnectionRoad;

        private float mMouseX;

        private float mMouseY;

        private readonly Camera mCamera;

        public PlatformPlacement(EPlatformType platformType, EPlacementType placementType, EScreen screen, Camera camera, ref Director director, float x = 0, float y = 0, ResourceMap resourceMap = null)
        {
            mCamera = camera;
            Screen = screen;

            director.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
            director.GetInputManager.AddMousePositionListener(this);

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
            //TODO: change to not hardcoded value.
            mPlatform.SetLayer(0.99f);

            UpdateBounds();

        }

        private void UpdateBounds()
        {
            mPlatform.RelativePosition = Vector2.Transform(mPlatform.AbsolutePosition, mCamera.GetTransform());
            mPlatform.RelativeSize = mPlatform.AbsoluteSize * mCamera.GetZoom();
            Bounds = new Rectangle((int) mPlatform.RelativePosition.X, (int) mPlatform.RelativePosition.Y, (int) mPlatform.RelativeSize.X, (int) mPlatform.RelativeSize.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            mPlatform.Draw(spriteBatch);
            mConnectionRoad?.Draw(spriteBatch);
        }

        public void Update(GameTime gametime)
        {
            switch (mCurrentState.GetState())
            {
                case 1:
                    mPlatform.AbsolutePosition = new Vector2(mMouseX - mPlatform.AbsoluteSize.X / 2f, mMouseY - mPlatform.AbsoluteSize.Y / 2f);
                    break;

                case 2:
                    mConnectionRoad.Destination = new Vector2(mMouseX, mMouseY);
                    if (mHoveringPlatform != null)
                    {
                        mConnectionRoad.Destination = mHoveringPlatform.Center;
                    }
                    break;

                case 3:
                    mPlatform.SetLayer(LayerConstants.PlatformLayer);
                    mConnectionRoad.Blueprint = false;
                    mConnectionRoad.Place(mPlatform, mHoveringPlatform);
                    mIsFinished = true;
                    break;

                default:
                    break;
            }
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
                        // the second boolean expression limits two platforms to only be connectable by a road if the road isn't in the fog of war.
                        // this was requested by felix
                        if (mHoveringPlatform != null 
                            && Vector2.Distance(mHoveringPlatform.Center, mPlatform.Center) <= (mPlatform.RevelationRadius + mHoveringPlatform.RevelationRadius))
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
                if (mCurrentState.GetState() != 2)
                {
                    return giveThrough;
                }

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

        public bool IsFinished()
        {
            return mIsFinished;
        }

        public void SetHovering(PlatformBlank hovering)
        {
            mHoveringPlatform = hovering;
        }

        public PlatformBlank GetPlatform()
        {
            return mPlatform;
        }

        public Road GetRoad()
        {
            return mConnectionRoad;
        }
    }
}
