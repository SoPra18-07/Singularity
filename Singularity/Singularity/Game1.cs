using System.Diagnostics;
using System.Runtime.InteropServices;
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
        // the time in seconds it took to complete drawing the last frame
        public static float mDeltaTime;

        internal readonly GraphicsDeviceManager mGraphics;
        internal readonly GraphicsAdapter mGraphicsAdapter;

        private float mLastFrameTime;

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

            mDirector = new Director(Content, mGraphics);


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
            mGraphics.PreferredBackBufferWidth = 1920; // 1080
            mGraphics.PreferredBackBufferHeight = 1080; // 720
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

            //This needs to be done because in the Constructor of Tutorial all Ingame-things (including screens) etc. are initialized and added
            var level = new Skirmish(mGraphics, ref mDirector, Content, mScreenManager);

            var mainMenuManager = new MainMenuManagerScreen(viewportResolution, mScreenManager, true, this);
            //ATTENTION: THE INGAME SCREENS ARE HANDLED IN THE LEVELS NOW!
            //mScreenManager.AddScreen(mainMenuManager); // TODO: This makes it so that the main menu is bypassed

            // TODO: load and play Soundtrack as background music
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
            // the reason we calculate the delta time here is since this is the only place where all the drawing "flows together"
            // here we can say that all the drawing is definitely finished after base.Draw is called, so here is the only place we truly
            // can calculate the passed time every frame.
            if (gameTime.TotalGameTime.Milliseconds - mLastFrameTime >= 0)
            {
                mDeltaTime = (gameTime.TotalGameTime.Milliseconds - mLastFrameTime) / 1000;
            }

            GraphicsDevice.Clear(Color.Black);

            mScreenManager.Draw(mSpriteBatch);
            base.Draw(gameTime);

            mLastFrameTime = gameTime.TotalGameTime.Milliseconds;
        }
    }
}
