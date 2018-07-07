using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool Loaded { get; set; }

        public EScreen Screen { get; private set; } = EScreen.DebugScreen;

        private SpriteFont mFont;

        private readonly StackScreenManager mScreenManager;

        private readonly Camera mCamera;

        private int mCurrentFps;

        private Map.Map mMap;

        public DebugScreen(StackScreenManager screenManager, Camera camera, Map.Map map, ref Director director)
        {
            mScreenManager = screenManager;
            mCamera = camera;
            mMap = map;

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
            spriteBatch.DrawString(mFont, "PlatformCount: " + mMap.GetStructureMap().GetPlatformList().Count, new Vector2(30, 235), Color.White);
            spriteBatch.DrawString(mFont, "GraphCount: " + mMap.GetStructureMap().GetGraphCount(), new Vector2(30, 255), Color.White);
            spriteBatch.DrawString(mFont, "MilitaryUnitCount: " + "TODO", new Vector2(30, 275), Color.White);
            spriteBatch.DrawString(mFont, "GeneralUnitCount: " + "TODO", new Vector2(30, 295), Color.White);

            spriteBatch.DrawLine(150, 209, 300, 209, Color.White);
            spriteBatch.DrawLine(20, 235, 20, 335, Color.White);
            spriteBatch.DrawLine(20, 335, 300, 335, Color.White);
            spriteBatch.DrawLine(300, 209, 300, 335, Color.White);



            spriteBatch.DrawString(mFont, "FPS: " + mCurrentFps, new Vector2(15, 365), Color.White);


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
        }

        public void Update(GameTime gametime)
        {
            mCurrentFps = (int) Math.Round(1 / gametime.ElapsedGameTime.TotalSeconds, MidpointRounding.ToEven);
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
    }
}
