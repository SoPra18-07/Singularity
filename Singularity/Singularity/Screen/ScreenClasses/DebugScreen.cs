using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;

namespace Singularity.Screen.ScreenClasses
{
    /// <summary>
    /// Used to show debug information. This might not be the prettiest, but it definitely does its work...
    /// </summary>
    public sealed class DebugScreen : IScreen, IKeyListener
    {
        private const string DisableText = "Disable FoW";
        private const string EnableText = "Enable Fow";

        public bool Loaded { get; set; }

        public EScreen Screen { get; private set; } = EScreen.DebugScreen;

        private SpriteFont mFont;

        private readonly StackScreenManager mScreenManager;

        private readonly Camera mCamera;

        private readonly Map.Map mMap;

        private readonly Director mDirector;

        private int mActivePlatforms;
        private int mDeactivePlatforms;

        private int mFrameCount;
        private double mDt;
        private int mFps;
        private readonly float mUpdateRate;

        private Button mFowButton;

        private int mUps;

        private bool mClicked;

        public DebugScreen(StackScreenManager screenManager, Camera camera, Map.Map map, ref Director director)
        {
            mUpdateRate = 2.0f;

            mScreenManager = screenManager;
            mCamera = camera;
            mMap = map;
            mDirector = director;

            director.GetInputManager.AddKeyListener(this);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.FillRectangle(new Rectangle(10, 0, 400, 500), new Color(Color.Black, 0.8f));
            spriteBatch.DrawRectangle(new Rectangle(10, 0, 400, 500), Color.Black, 10f);

            spriteBatch.DrawString(mFont, "Camera", new Vector2(15, 15), Color.White);
            spriteBatch.DrawString(mFont, "ScreenX: " + mCamera.GetRelativePosition().X, new Vector2(30, 50), Color.White);
            spriteBatch.DrawString(mFont, "ScreenY: " + mCamera.GetRelativePosition().Y, new Vector2(30, 70), Color.White);
            spriteBatch.DrawString(mFont, "Width: " + mCamera.GetSize().X, new Vector2(30, 90), Color.White);
            spriteBatch.DrawString(mFont, "Height: " + mCamera.GetSize().Y, new Vector2(30, 110), Color.White);
            spriteBatch.DrawString(mFont, "Zoom: " + mCamera.GetZoom(), new Vector2(30, 130), Color.White);

            spriteBatch.DrawLine(100, 24, 300, 24, Color.White);
            spriteBatch.DrawLine(20, 50, 20, 170, Color.White);
            spriteBatch.DrawLine(20, 170, 300, 170, Color.White);
            spriteBatch.DrawLine(300, 24, 300, 170, Color.White);




            spriteBatch.DrawString(mFont, "GameObjects", new Vector2(15, 200), Color.White);
            spriteBatch.DrawString(mFont, "PlatformCount: " + mMap.GetStructureMap().GetPlatformList().Count + ", " + mActivePlatforms + ", " + mDeactivePlatforms, new Vector2(30, 235), Color.White);
            spriteBatch.DrawString(mFont, "GraphCount: " + mMap.GetStructureMap().GetGraphCount(), new Vector2(30, 255), Color.White);
            spriteBatch.DrawString(mFont, "MilitaryUnitCount: " + mDirector.GetMilitaryManager.TotalUnitCount, new Vector2(30, 275), Color.White);
            spriteBatch.DrawString(mFont, "GeneralUnitCount: " + "TODO", new Vector2(30, 295), Color.White);

            spriteBatch.DrawLine(150, 209, 300, 209, Color.White);
            spriteBatch.DrawLine(20, 235, 20, 335, Color.White);
            spriteBatch.DrawLine(20, 335, 300, 335, Color.White);
            spriteBatch.DrawLine(300, 209, 300, 335, Color.White);



            spriteBatch.DrawString(mFont, "FPS: " + mFps, new Vector2(15, 365), Color.White);
            spriteBatch.DrawString(mFont, "UPS: " + mUps, new Vector2(15, 385), Color.White);

            mFowButton.Draw(spriteBatch);

            //spriteBatch.DrawString(mFont, "FPS: " + mCurrentFps, new Vector2(15, 200), Color.White);
            spriteBatch.End();
        }

        public bool DrawLower()
        {
            return true;
        }

        public void LoadContent(ContentManager content)
        {
            mFont = content.Load<SpriteFont>("LibSans14");


            mFowButton = new Button(DisableText, mFont, new Vector2(130, 450), Color.Red, true) {Opacity = 1f};

            mFowButton.ButtonClicked += FowButtonClicked;
            mFowButton.ButtonReleased += FowButtonReleased;
        }

        public void Update(GameTime gametime)
        {

            mFrameCount++;
            mDt += Game1.mDeltaTime;
            if (mDt > 1f / mUpdateRate)
            {
                mFps = (int) Math.Round(mFrameCount / mDt);
                mFrameCount = 0;
                mDt -= 1 / mUpdateRate;
            }

            mUps = (int) Math.Round(1 / gametime.ElapsedGameTime.TotalSeconds);

            var activeCounter = 0;
            var deactiveCounter = 0;
            foreach (var platform in mMap.GetStructureMap().GetPlatformList())
            {
                if (platform.IsActive())
                {
                    activeCounter++;
                }
                else
                {
                    deactiveCounter++;
                }
            }

            mActivePlatforms = activeCounter;
            mDeactivePlatforms = deactiveCounter;

            mFowButton.Update(gametime);
        }

        public bool UpdateLower()
        {
            return true;
        }

        public void KeyTyped(KeyEvent keyEvent)
        {
            // switch the debug state on f4 press
            if (keyEvent.CurrentKeys.Contains(Keys.F4))
            {
                if (!GlobalVariables.DebugState)
                {
                    mScreenManager.AddScreen(this);
                    GlobalVariables.DebugState = !GlobalVariables.DebugState;
                }
                else if(GlobalVariables.DebugState && mScreenManager.Peek().Equals(this))
                {
                    mScreenManager.RemoveScreen();
                    GlobalVariables.DebugState = !GlobalVariables.DebugState;
                }
            }
        }

        public void KeyPressed(KeyEvent keyEvent)
        {

        }

        public void KeyReleased(KeyEvent keyEvent)
        {

        }

        private void FowButtonClicked(object sender, EventArgs args)
        {
            if (mClicked)
            {
                return;
            }

            GlobalVariables.FowEnabled = !GlobalVariables.FowEnabled;

            if (GlobalVariables.FowEnabled)
            {
                mFowButton.ChangeText(DisableText);
            }
            else
            {
                mFowButton.ChangeText(EnableText);
            }


            mClicked = true;
        }

        private void FowButtonReleased(object sender, EventArgs args)
        {
            mClicked = false;
        }
    }
}
