using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Map;
using Singularity.Property;
using Singularity.Screen.ScreenClasses;

namespace Singularity.Platform
{
    public sealed class PlatformBuildingRoadConnector : IUpdate, IMousePositionListener, IMouseClickListener
    {
        private readonly LinkedList<PlatformBlank> mPlatforms;

        private Road mRoad;

        private PlatformBlank mPlatformToConnect;
        private readonly GameScreen mGameScreen;

        private float mMouseX;
        private float mMouseY;

        public Rectangle Bounds { get; private set; }

        internal PlatformBuildingRoadConnector(StructureMap structMap, InputManager inputManager, GameScreen gameScreen)
        {
            mGameScreen = gameScreen;
            mPlatforms = structMap.GetPlatforms();
            inputManager.AddMousePositionListener(this);
            inputManager.AddMouseClickListener(this, EClickType.OutOfBoundsOnly, EClickType.InBoundsOnly);

            Bounds = new Rectangle(0, 0, 0, 0);
        }

        public void Update(GameTime gametime)
        {
            if (mPlatformToConnect == null)
            {
                return;
            }

            if (mRoad == null)
            {
                mPlatformToConnect.IsPlaced = true;
                mPlatformToConnect = null;
                return;
            }
            
            if (mPlatformToConnect.IsPlaced)
            {
                mPlatformToConnect = null;
                return;
            }

            mRoad.AbsolutePosition = new Vector2(mMouseX, mMouseY);

            var hoveringPlatform = Hovering();

            if (hoveringPlatform == null)
            {
                return;
            }

            mRoad.AbsolutePosition = hoveringPlatform.Center;
        }

        public void MousePositionChanged(float relX, float relY, float absX, float absY)
        {
            mMouseX = absX;
            mMouseY = absY;
        }

        private PlatformBlank Hovering()
        {
            foreach (var platform in mPlatforms)
            {
                if (platform.AbsBounds.Intersects(new Rectangle((int) mMouseX, (int) mMouseY, 1, 1)))
                {
                    return platform;
                }
            }

            return null;
        }

        public void SetPlatformToConnect(PlatformBlank platform)
        {
            if (mPlatformToConnect != null)
            {
                return;
            }
            mPlatformToConnect = platform;
            mRoad = new Road(mPlatformToConnect, mPlatformToConnect, false, false);
            mGameScreen.AddObject(mRoad);

        }

        public void MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            if (!(mouseAction == EMouseAction.LeftClick && Hovering() != null) || mRoad == null)
            {
                return;
            }
            mRoad.IsSemiPlaced = true;
            mRoad.IsPlaced = true;
            mRoad = null;
        }

        public void MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            
        }

        public void MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            
        }
    }
}
