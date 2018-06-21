﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;

namespace Singularity
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal sealed class Game1 : Game
    {
        internal readonly GraphicsDeviceManager MGraphics;
        internal readonly GraphicsAdapter MGraphicsAdapter;


        // Screens
        internal GameScreen MGameScreen;
        private MainMenuManagerScreen _mMainMenuManager;


        // Sprites!
        private SpriteBatch _mSpriteBatch;


        // Screen Manager
        private readonly IScreenManager _mScreenManager;


        //
        private readonly Director _director;


        internal Game1()
        {
            MGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            MGraphicsAdapter = GraphicsAdapter.DefaultAdapter;

            _mScreenManager = new StackScreenManager(Content);
            _director = new Director(Content);

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            MGraphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            MGraphics.PreferredBackBufferWidth = 1080;
            MGraphics.PreferredBackBufferHeight = 720;
            MGraphics.IsFullScreen = false;
            MGraphics.ApplyChanges();

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
            _mSpriteBatch = new SpriteBatch(GraphicsDevice);

            MGameScreen = new GameScreen(MGraphics.GraphicsDevice, _director);

            _mMainMenuManager = new MainMenuManagerScreen(viewportResolution, _mScreenManager, true, this);

            // Add the screens to the screen manager
            // The idea is that the game screen is always at the bottom and stuff is added simply
            // on top of it.
            _mScreenManager.AddScreen(MGameScreen);
            _mScreenManager.AddScreen(_mMainMenuManager);

            _mMainMenuManager.LoadContent(Content);

            // load and play Soundtrack as background music
            // todo this
            // director.GetSoundManager.LoadContent(Content);
            //_mSoundManager.PlaySoundTrack();

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
            _director.Update(gameTime);
            _mScreenManager.Update(gameTime);
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

            _mScreenManager.Draw(_mSpriteBatch);
            base.Draw(gameTime);
        }
    }
}
