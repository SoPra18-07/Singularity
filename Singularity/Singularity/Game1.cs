﻿using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Singularity.Platform;
using Singularity.Input;

using Singularity.Map;
using Singularity.Resources;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal sealed class Game1 : Game
    {
        private GraphicsDeviceManager mGraphics;

        private Texture2D mPlatformBlankTexture;
        private Texture2D mPlatformDomeTexture;
        private PlatformBlank mPlatform;
        private PlatformBlank mPlatform2;

        private MilitaryUnit mMUnit1;
        private MilitaryUnit mMUnit2;
        private EnergyFacility mPlatform3;
        private Map.Map mMap;

        private static Song sSoundtrack;

        // Screens
        private GameScreen mGameScreen;
        private MenuBackgroundScreen mMenuBackgroundScreen;
        private SplashScreen mSplashScreen;
        private MainMenuScreen mMainMenuScreen;
        private AchievementsScreen mAchievementsScreen;
        private GameModeSelectScreen mGameModeSelectScreen;
        private LoadSelectScreen mLoadSelectScreen;
        private OptionsScreen mOptionsScreen;
        private MainMenuManagerScreen mMainMenuManager;
        private InputManager mInputManager;

        // roads
        private Road mRoad1;

        // Sprites!
        private SpriteBatch mSpriteBatch;
        private Texture2D mPlatformSheet;
        private Texture2D mMUnitSheet;

        // Screen Manager
        private readonly IScreenManager mScreenManager;

        internal Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            mInputManager = new InputManager();
            mScreenManager = new StackScreenManager();

            mInputManager = new InputManager();

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // can be used to debug the screen manager
            /*
               mScreenManager.AddScreen(new RenderLowerScreen());
               mScreenManager.AddScreen(new UpdateLowerScreen());
            */

            // XSerializer.TestSerialization();
            IsMouseVisible = true;
            mGraphics.PreferredBackBufferWidth = 1080;
            mGraphics.PreferredBackBufferHeight = 720;
            mGraphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            var viewportResolution = new Vector2(GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            mMUnitSheet = Content.Load<Texture2D>("UnitSpriteSheet");

            mPlatformSheet = Content.Load<Texture2D>("PlatformSpriteSheet");
            mPlatform = new PlatformBlank(new Vector2(300, 400), mPlatformSheet);
            mPlatform2 = new PlatformBlank(new Vector2(800, 600), mPlatformSheet);
            // TODO: use this.Content to load your game content here
            mPlatformBlankTexture = Content.Load<Texture2D>("PlatformBasic");
            mPlatformDomeTexture = Content.Load<Texture2D>("Dome");
            mPlatform = new PlatformBlank(new Vector2(300, 400), mPlatformBlankTexture);
            mPlatform2 = new Junkyard(new Vector2(800, 600), mPlatformDomeTexture);
            mPlatform3 = new EnergyFacility(new Vector2(600, 200), mPlatformDomeTexture);

            mMap = new Map.Map(Content.Load<Texture2D>("MockUpBackground"), mGraphics.GraphicsDevice.Viewport, true);

            mMUnit1 = new MilitaryUnit(new Vector2(600, 600), mMUnitSheet, mMap.GetCamera());
            mMUnit2 = new MilitaryUnit(new Vector2(100, 600), mMUnitSheet, mMap.GetCamera());

            mMap.AddPlatform(mPlatform);
            mMap.AddPlatform(mPlatform2);

            mGameScreen = new GameScreen(mMap);

            mMainMenuManager = new MainMenuManagerScreen(viewportResolution, mScreenManager, true);

            // Add the screens to the screen manager
            // The idea is that the game screen is always at the bottom and stuff is added simply
            // on top of it.
            mScreenManager.AddScreen(mGameScreen);
            mScreenManager.AddScreen(mMainMenuManager);
            // TODO load game screen contents only after game new game or load game has been started

            mMainMenuManager.LoadContent(Content);

            // load roads
            mRoad1 = new Road(mPlatform, mPlatform2, false);
            var road2 = new Road(mPlatform, mPlatform3, false);
            var road3 = new Road(mPlatform2, mPlatform3, false);

            mGameScreen.AddObject(mMUnit1);
            mGameScreen.AddObject(mMUnit2);
            mGameScreen.AddObject(mPlatform);
            mGameScreen.AddObject(mPlatform2);
            mGameScreen.AddObject(mPlatform3);
            mGameScreen.AddObject(mRoad1);
            mGameScreen.AddObject(road2);
            mGameScreen.AddObject(road3);
            mGameScreen.AddObject(ResourceHelper.GetRandomlyDistributedResources(5));

            // load and play Soundtrack as background music
            sSoundtrack = Content.Load<Song>("BGMusic");
            MediaPlayer.Play(sSoundtrack);
            MediaPlayer.Volume = 0.1F;
            MediaPlayer.IsRepeating = true;

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // a new static input manager. It requires updating every tick to figure out where
            // the mouse is.
            InputManager2.Update(gameTime);

            mInputManager.Update(gameTime);
            mScreenManager.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            mScreenManager.Draw(mSpriteBatch);
            base.Draw(gameTime);
        }
    }
}
