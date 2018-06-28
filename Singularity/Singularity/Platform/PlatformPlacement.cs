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

        private readonly PlatformBlank mPlatform;

        private bool mHoveringPlatform;

        private float mMouseX;

        private float mMouseY;

        public PlatformPlacement(EPlatformType platformType, EPlacementType placementType, EScreen screen, ref Director director, float x = 0, float y = 0, ResourceMap resourceMap = null)
        {
            director.GetInputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.Both);
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

            mHoveringPlatform = false;

            Screen = screen;
            mPlatform = PlatformFactory.Get(platformType, ref director, x, y, resourceMap);

            UpdateBounds();

        }

        private void UpdateBounds()
        {
            Bounds = new Rectangle((int) mPlatform.RelativePosition.X, (int) mPlatform.RelativePosition.Y, (int) mPlatform.RelativeSize.X, (int) mPlatform.RelativeSize.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gametime)
        {
            switch (mCurrentState.GetState())
            {
                case 1:
                    break;

                case 2:
                    break;

                case 3:
                    break;

                default:
                    break;
            }
        }


        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            var giveThrough = true;

            if (mouseAction == EMouseAction.LeftClick && withinBounds)
            {
                switch (mCurrentState.GetState())
                {
                    case 1:
                        mCurrentState.NextState();

                        if (mMouseFollowOnly)
                        {
                            mCurrentState.NextState();
                        }

                        giveThrough = false;
                        break;

                    case 2:
                        if (mHoveringPlatform)
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

        public void MousePositionChanged(float newX, float newY)
        {
            mMouseX = newX;
            mMouseY = newY;
        }
    }
}
