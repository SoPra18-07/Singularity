using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Platform
{
    public class PlatformPlacement : IDraw, IUpdate
    {
        /// <summary>
        /// A 3 state machine if you will.
        /// State 1 (inital): Platform follows the mouse and left click triggers next state
        /// State 2 (road)  : The platform is locked in place and a road is needs to be connected to another platform.
        /// last state with right click, and next state when a road is connected.
        /// State 3 (add)   : A new platform object gets added to the structure map.
        /// </summary>
        private readonly State3 mCurrentState;

        private readonly bool mMouseFollowOnly;

        private readonly Vector2 mPlatformLocation;

        private readonly PlatformBlank mPlatform;

        public PlatformPlacement(EPlatformType platformType, EPlacementType placementType, float x = 0, float y = 0)
        {
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
            mPlatformLocation = new Vector2(x, y);
            //mPlatformType = platformType;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gametime)
        {
            throw new NotImplementedException();
        }
    }
}
