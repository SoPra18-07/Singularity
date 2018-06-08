using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Shown after Options on the main menu or pause menu has been clicked.
    /// Allows different settings and options to be set. Buttons include
    /// for the different settings and a back button.
    /// </summary>
    class OptionsScreen : IScreen
    {
        private Game1 game;
        // layout. Made only once to reduce unnecssary calculations at draw time
        private readonly Vector2 mBoxPosition;
        private readonly Vector2 mWindowTitlePosition;
        private readonly Vector2 mLinePosition;
        private readonly float mTabPadding;
        private readonly float mContentPadding;
        private readonly float mTopContentPadding;

        // All strings are variables to allow for easy editing and localization
        private readonly string mWindowTitleString;
        private readonly string mGameplayString;
        private readonly string mGraphicsString;
        private readonly string mAudioString;
        private readonly string mSaveChangesString;
        private readonly string mBackString;

        private readonly string mFullScreenString;
        private readonly string mResolutionString;
        private readonly string mAntialiasingString;

        // fonts
        private SpriteFont mLibSans36;
        private SpriteFont mLibSans20;

        // tab buttons
        private readonly List<Button> mTabButtons;
        private Button mGameplayButton;
        private Button mGraphicsButton;
        private Button mAudioButton;
        private Button mSaveButton;
        private Button mBackButton;

        // Graphics tab
        private readonly List<Button> mGraphicsButtons;
        private Button mFullScreen; // todo replace with a toggle
        private Button mResolution1; // todo replace with a better system
        private Button mResolution2; // todo replace with a better system
        private Button mAntialiasing; // todo replace with a toggle
        
        // Audio tab
        // todo add the following:
        // Master volume and toggle
        // Background volume and toggle
        // Sound effect volume and toggle
        // 3D sound effect toggle
        
        private EOptionScreenState mScreenState;
        
        /// <summary>
        /// Creates an instance of the Options screen.
        /// </summary>
        /// <param name="screenResolution">Screen resolution used for scaling</param>
        public OptionsScreen(Vector2 screenResolution, Game1 game)
        {
            // scaling of all positions according to viewport size
            mBoxPosition = new Vector2(screenResolution.X / 2 - 306, screenResolution.Y * 0.2f);
            mTabPadding = mBoxPosition.X + 36;
            mContentPadding = mBoxPosition.X + 204;
            mTopContentPadding = mBoxPosition.Y + 84;
            mWindowTitlePosition = new Vector2(mBoxPosition.X + 20, mBoxPosition.Y + 20);
            mLinePosition = new Vector2(mBoxPosition.X + 180, mBoxPosition.Y + 85);
            
            mWindowTitleString = "Options";
            mGameplayString = "Gameplay";
            mGraphicsString = "Graphics";
            mAudioString = "Audio";
            mSaveChangesString = "Apply";
            mBackString = "Back";

            mFullScreenString = "Full Screen";
            mResolutionString = "Resolution:";
            mAntialiasingString = "Anti-Aliasing";

            mTabButtons = new List<Button>(5);
            mGraphicsButtons = new List<Button>(4);

            mScreenState = EOptionScreenState.Gameplay;
            this.game = game;
        }

        /// <summary>
        /// Loads any content that this screen might need.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLibSans36 = content.Load<SpriteFont>("LibSans36");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");

            // make the tab select buttons
            mGameplayButton = new Button(mGameplayString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding));
            mGraphicsButton = new Button(mGraphicsString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding + 40));
            mAudioButton = new Button(mAudioString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding + 80));
            mSaveButton = new Button(mSaveChangesString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding + 120));
            mBackButton = new Button(mBackString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding + 160));

            mTabButtons.Add(mGameplayButton);
            mTabButtons.Add(mGraphicsButton);
            mTabButtons.Add(mAudioButton);
            mTabButtons.Add(mSaveButton);
            mTabButtons.Add(mBackButton);

            // Gameplay settings
            // TODO figure out what settings can be implemented in here

            // Graphics settings
            mFullScreen = new Button(mFullScreenString, mLibSans20, new Vector2(mContentPadding, mTopContentPadding));
            mResolution1 = new Button("1080 x 720", mLibSans20, new Vector2(mContentPadding, mTopContentPadding + 40));
            mResolution2 = new Button("1920 x 1080", mLibSans20, new Vector2(mContentPadding, mTopContentPadding + 80));
            mAntialiasing = new Button(mAntialiasingString, mLibSans20, new Vector2(mContentPadding, mTopContentPadding + 120));

            mGraphicsButtons.Add(mFullScreen);
            mGraphicsButtons.Add(mResolution1);
            mGraphicsButtons.Add(mResolution2);
            mGraphicsButtons.Add(mAntialiasing);


            // Button handler bindings
            mGameplayButton.ButtonReleased += OnGameplayReleased;
            mGraphicsButton.ButtonReleased += OnGraphicsReleased;
            mAudioButton.ButtonReleased += OnAudioReleased;
            mBackButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;

            mFullScreen.ButtonReleased += OnFullScreenReleased;
            mResolution1.ButtonReleased += OnResoOneReleased;
            mResolution2.ButtonReleased += OnResoTwoReleased;
            mAntialiasing.ButtonReleased += OnAntialiasingReleased;
        }

        /// <summary>
        /// Updates the contents of the screen.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time </param>
        public void Update(GameTime gametime)
        {
            foreach (Button button in mTabButtons)
            {
                button.Update(gametime);
            }

            switch (mScreenState)
            {
                case EOptionScreenState.Gameplay:
                    break;
                case EOptionScreenState.Graphics:
                    foreach (Button button in mGraphicsButtons)
                    {
                        button.Update(gametime);
                    }
                    break;
                case EOptionScreenState.Audio:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // background window
            spriteBatch.StrokedRectangle(mBoxPosition,
                new Vector2(612, 406),
                Color.White,
                Color.White,
                0.5f,
                0.21f);

            // line in the middle
            spriteBatch.DrawLine(point: mLinePosition,
                angle: (float) Math.PI / 2,
                length: 301,
                color: new Color(new Vector4(255, 255, 255, 0.5f)),
                thickness: 1);

            // window title
            spriteBatch.DrawString(mLibSans36,
                text: mWindowTitleString,
                position: mWindowTitlePosition,
                color: Color.White);

            // tab buttons
            foreach (Button button in mTabButtons)
            {
                button.Draw(spriteBatch);
            }

            // actual options
            switch (mScreenState)
            {
                case EOptionScreenState.Gameplay:
                    spriteBatch.DrawString(mLibSans20, "Difficulty", new Vector2(mContentPadding, mTopContentPadding), Color.White);
                    break;
                case EOptionScreenState.Graphics:
                    foreach (Button button in mGraphicsButtons)
                    {
                        button.Draw(spriteBatch);
                    }
                    break;
                case EOptionScreenState.Audio:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            spriteBatch.End();
        }

        

        /// <summary>
        /// Determines whether or not the screen below this on the stack should update.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be updated.</returns>
        public bool UpdateLower()
        {
            return true;
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should be drawn.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be drawn.</returns>
        public bool DrawLower()
        {
            return true;
        }

        #region Button Handlers

        /// <summary>
        /// Handler for the Gameplay button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnGameplayReleased(Object sender, EventArgs eventArgs)
        {
            mScreenState = EOptionScreenState.Gameplay;
        }

        /// <summary>
        /// Handler for the Graphics button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnGraphicsReleased(Object sender, EventArgs eventArgs)
        {
            mScreenState = EOptionScreenState.Graphics;
        }

        /// <summary>
        /// Handler for the Audio button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnAudioReleased(Object sender, EventArgs eventArgs)
        {
            mScreenState = EOptionScreenState.Audio;
        }

        /// <summary>
        /// Makes the game full screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnFullScreenReleased(Object sender, EventArgs eventArgs)
        {
            game.mGraphics.IsFullScreen = true;
            game.mGraphics.ApplyChanges();
        }

        private void OnResoOneReleased(Object sender, EventArgs eventArgs)
        {
            game.mGraphics.PreferredBackBufferWidth = 1080;
            game.mGraphics.PreferredBackBufferHeight = 720;
            game.mGraphics.ApplyChanges();
            MainMenuManagerScreen.SetResolution(new Vector2(1080, 720));
        }

        private void OnResoTwoReleased(Object sender, EventArgs eventArgs)
        {
            game.mGraphics.PreferredBackBufferWidth = 1920;
            game.mGraphics.PreferredBackBufferHeight = 1080;
            game.mGraphics.ApplyChanges();
            MainMenuManagerScreen.SetResolution(new Vector2(1920, 1080));
        }

        private void OnAntialiasingReleased(Object sender, EventArgs eventArgs)
        {
            // todo figure this out later
        }

        #endregion
    }
}
