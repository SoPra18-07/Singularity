using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Levels;
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
        internal readonly GraphicsDeviceManager mGraphics;
        internal readonly GraphicsAdapter mGraphicsAdapter;

        // Screens
        private GameScreen mGameScreen;
        private MainMenuManagerScreen mMainMenuManager;
        private UserInterfaceScreen mUserInterfaceScreen;

        // Sprites!
        private SpriteBatch mSpriteBatch;


        // Screen Manager
        private readonly IScreenManager mScreenManager;


        //
        private Director mDirector;


        internal Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            mGraphicsAdapter = GraphicsAdapter.DefaultAdapter;

            mDirector = new Director(Content);


            mScreenManager = new StackScreenManager(Content, mDirector.GetInputManager);

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
            mGraphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            mGraphics.PreferredBackBufferWidth = 1080;
            mGraphics.PreferredBackBufferHeight = 720;
            mGraphics.IsFullScreen = false;
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


            mGameScreen = new Skirmish(mGraphics.GraphicsDevice, ref mDirector, Content).GetGameScreen();

            //mGameScreen = new GameScreen(mGraphics.GraphicsDevice, ref mDirector);

            mMainMenuManager = new MainMenuManagerScreen(viewportResolution, mScreenManager, true, this);

            mUserInterfaceScreen = new UserInterfaceScreen(ref mDirector, mGraphics, mGameScreen);

            // Add the screens to the screen manager
            // The idea is that the game screen is always at the bottom and stuff is added simply
            // on top of it.
            mScreenManager.AddScreen(mGameScreen);
            mScreenManager.AddScreen(mUserInterfaceScreen);
            //mScreenManager.AddScreen(mMainMenuManager); // TODO: This makes it so that the main menu is bypassed

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
            mDirector.Update(gameTime, IsActive);
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

            mScreenManager.Draw(mSpriteBatch);
            base.Draw(gameTime);
        }
    }
}
