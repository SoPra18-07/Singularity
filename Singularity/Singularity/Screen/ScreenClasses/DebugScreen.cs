using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Map.Properties;
using Singularity.Property;

namespace Singularity.Screen.ScreenClasses
{
    /// <summary>
    /// Used to show debug information. This might not be the prettiest, but it definitely does its work...
    /// </summary>
    public sealed class DebugScreen : IScreen, IKeyListener
    {
        private const string DisableText = "Disable Fow";
        private const string EnableText = "Enable Fow";

        public bool Loaded { get; set; }

        public EScreen Screen { get; private set; } = EScreen.GameScreen;

        private SpriteFont mFont;

        private StackScreenManager mScreenManager;

        private Camera mCamera;

        private Map.Map mMap;

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

        private int mGenUnitCount;

        public DebugScreen(StackScreenManager screenManager, Camera camera, Map.Map map, ref Director director)
        {
            mUpdateRate = 2.0f;

            mScreenManager = screenManager;
            mCamera = camera;
            mMap = map;
            mDirector = director;

            director.GetInputManager.FlagForAddition(this);

        }

        public void ReloadContent(ContentManager content, Camera camera, Map.Map map, StackScreenManager screenManager, ref Director director)
        {
            director.GetInputManager.FlagForAddition(this);
            mFont = content.Load<SpriteFont>("LibSans14");
            mCamera = camera;
            mMap = map;
            mScreenManager = screenManager;
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
            spriteBatch.DrawString(mFont, "GeneralUnitCount: " + mGenUnitCount, new Vector2(30, 295), Color.White);

            spriteBatch.DrawLine(150, 209, 300, 209, Color.White);
            spriteBatch.DrawLine(20, 235, 20, 335, Color.White);
            spriteBatch.DrawLine(20, 335, 300, 335, Color.White);
            spriteBatch.DrawLine(300, 209, 300, 335, Color.White);


            spriteBatch.DrawString(mFont, "EnemyDifficulty: " + mDirector.GetStoryManager.Level.Ai.Difficulty, new Vector2(15, 355), Color.White);
            spriteBatch.DrawString(mFont, "FPS: " + mFps, new Vector2(15, 395), Color.White);
            spriteBatch.DrawString(mFont, "UPS: " + mUps, new Vector2(15, 415), Color.White);

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

            var genUnitsCount = 0;

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

                foreach (var genUnit in platform.GetGeneralUnitsOnPlatform())
                {
                    genUnitsCount++;
                }
            }

            mActivePlatforms = activeCounter;
            mDeactivePlatforms = deactiveCounter;
            mGenUnitCount = genUnitsCount;

            mFowButton.Update(gametime);
        }

        public bool UpdateLower()
        {
            return true;
        }

        public bool KeyTyped(KeyEvent keyEvent)
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

                return false;
            }

            return true;
        }

        public bool KeyPressed(KeyEvent keyEvent)
        {
            return true;
        }

        public bool KeyReleased(KeyEvent keyEvent)
        {
            return true;
        }

        private void FowButtonClicked(object sender, EventArgs args)
        {
            if (mClicked)
            {
                return;
            }

            GlobalVariables.mFowEnabled = !GlobalVariables.mFowEnabled;

            mFowButton.ChangeText(GlobalVariables.mFowEnabled ? DisableText : EnableText);

            mClicked = true;
        }

        private void FowButtonReleased(object sender, EventArgs args)
        {
            mClicked = false;
        }
    }
}
