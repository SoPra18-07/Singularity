using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;

namespace Singularity.Screen.ScreenClasses
{
    class UserInterfaceScreen : IScreen
    {
        private List<WindowObject> mWindowList;
        private SpriteFont mLibSans20;
        private InputManager mInputManager;

        public UserInterfaceScreen(InputManager inputManager)
        {
            mInputManager = inputManager;
        }

        public void Update(GameTime gametime)
        {
            foreach (var window in mWindowList)
            {
                window.Update(gametime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var window in mWindowList)
            {
                window.Draw(spriteBatch);
            }
        }

        public void LoadContent(ContentManager content)
        {
            mLibSans20 = content.Load<SpriteFont>("LibSans20");

            // test windows object
            mWindowList = new List<WindowObject>();

            var currentScreenWidth = 1080;
            var currentScreenHeight = 720;

            var windowColor = new Color(255, 0, 0, 1f);
            var borderColor = new Color(50, 50, 50, 1f);

            // set position- and size-values for all windows of the userinterface
            var topBarHeight = currentScreenHeight / 30;
            var topBarWidth = currentScreenWidth;

            var civilUnitsX = topBarHeight / 2;
            var civilUnitsY = topBarHeight / 2 + topBarHeight;
            var civilUnitsWidth = (int)(currentScreenWidth / 4.5);
            var civilUnitsHeight = (int)(currentScreenHeight / 1.8);

            var resourceX = topBarHeight / 2;
            var resourceY = 2 * (topBarHeight / 2) + topBarHeight + civilUnitsHeight;
            var resourceWidth = civilUnitsWidth;
            var resourceHeight = (int)(currentScreenHeight / 2.75);

            var eventLogX = civilUnitsX + civilUnitsWidth + 50; // TODO
            var eventLogY = civilUnitsY;
            var eventLogWidth = civilUnitsWidth;
            var eventLogHeight = (int)(currentScreenHeight / 2.5);

            // create windowObjects for all windows of the userinterface
            // INFO: parameters are: NAME, POSITION-vector, SIZE-vector, COLOR of border, COLOR of filling, opacity of everything that gets drawn, borderPadding, objectPadding, minimizable, fontForText, inputmanager)
            var civilUnitsWindow = new WindowObject("// CIVIL UNITS", new Vector2(civilUnitsX, civilUnitsY), new Vector2(civilUnitsWidth, civilUnitsHeight), borderColor, windowColor, 0.5f, 1f, 1f, true, mLibSans20, mInputManager);
            var topBarWindow = new WindowObject("", new Vector2(0, 0), new Vector2(topBarWidth, topBarHeight), borderColor, windowColor, 0.5f, 1f, 1f, false, mLibSans20, mInputManager);
            var resourceWindow = new WindowObject("// RESOURCES", new Vector2(resourceX, resourceY), new Vector2(resourceWidth, resourceHeight), borderColor, windowColor, 0.5f, 1f, 1f, true, mLibSans20, mInputManager);
            var eventLogWindow = new WindowObject("// EVENT LOG", new Vector2(eventLogX, eventLogY), new Vector2(eventLogWidth, eventLogHeight), borderColor, windowColor, 0.5f, 1f, 1f, true, mLibSans20, mInputManager);

            // add all windowObjects of the userinterface
            mWindowList.Add(civilUnitsWindow);
            mWindowList.Add(topBarWindow);
            mWindowList.Add(resourceWindow);
            mWindowList.Add(eventLogWindow);
        }

        public bool UpdateLower()
        {
            return true;
        }

        public bool DrawLower()
        {
            return true;
        }
    }
}
