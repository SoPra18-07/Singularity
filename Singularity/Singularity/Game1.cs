﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Singularity.Input;
using Singularity.Map;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Sound;
using Singularity.Units;

namespace Singularity
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal sealed class Game1 : Game
    {
        internal GraphicsDeviceManager mGraphics;
        internal GraphicsAdapter mGraphicsAdapter;

        private SoundManager mSoundManager;

        // Screens
        internal GameScreen mGameScreen;
        private MainMenuManagerScreen mMainMenuManager;
        private InputManager mInputManager;


        // Sprites!
        private SpriteBatch mSpriteBatch;


        // Screen Manager
        private readonly IScreenManager mScreenManager;

        internal Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            mGraphicsAdapter = GraphicsAdapter.DefaultAdapter;

            mScreenManager = new StackScreenManager();

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
            mGraphics.PreferredBackBufferWidth = 1280;
            mGraphics.PreferredBackBufferHeight = 1024;
            mGraphics.IsFullScreen = false;
            mGraphics.ApplyChanges();
            mSoundManager = new SoundManager();

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

            var camera = new Camera(GraphicsDevice.Viewport);

            mInputManager = new InputManager(camera);

            camera.SetInputManager(mInputManager);

            mGameScreen = new GameScreen(mGraphics.GraphicsDevice.Viewport, mInputManager, camera, mScreenManager);

            mMainMenuManager = new MainMenuManagerScreen(viewportResolution, mScreenManager, true, this);

            // Add the screens to the screen manager
            // The idea is that the game screen is always at the bottom and stuff is added simply
            // on top of it.

            mScreenManager.AddScreen(mGameScreen);
            mScreenManager.AddScreen(mMainMenuManager);

            mMainMenuManager.LoadContent(Content);
            // load and play Soundtrack as background music
            mSoundManager.LoadContent(Content);
            mSoundManager.PlaySoundTrack();

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