using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Serialization;

namespace Singularity
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal sealed class Game1 : Game
    {
        /// <summary>
        /// An instance of the GlobalVariables class to allow for serialization
        /// </summary>
        private GlobalVariablesInstance mInstance;

        // the time in seconds it took to complete drawing the last frame
        public static float mDeltaTime;
        private float mLastFrameTime;

        internal readonly GraphicsDeviceManager mGraphics;
        internal readonly GraphicsAdapter mGraphicsAdapter;

        // Screens
        private LoadGameManagerScreen mLoadGameManager;
        private MainMenuManagerScreen mMainMenuManager;

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

            LoadConfig();

            mGraphicsAdapter = GraphicsAdapter.DefaultAdapter;

            mDirector = new Director(Content, mGraphics, mInstance);

            mScreenManager = new StackScreenManager(Content, mDirector.GetInputManager);

            mDirector.GetStoryManager.SetScreenManager(mScreenManager);
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

            if (GlobalVariables.IsFullScreen)
            {
                mGraphics.PreferredBackBufferWidth = mGraphicsAdapter.CurrentDisplayMode.Width;
                mGraphics.PreferredBackBufferHeight = mGraphicsAdapter.CurrentDisplayMode.Height;
            }
            else
            {

                mGraphics.PreferredBackBufferWidth = GlobalVariables.ResolutionList[GlobalVariables.ChosenResolution].Item1;
                mGraphics.PreferredBackBufferHeight = GlobalVariables.ResolutionList[GlobalVariables.ChosenResolution].Item2;
            }

            mGraphics.IsFullScreen = GlobalVariables.IsFullScreen;

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

            mLoadGameManager = new LoadGameManagerScreen(mGraphics, ref mDirector, Content, viewportResolution, mScreenManager, this);
            mMainMenuManager = new MainMenuManagerScreen(viewportResolution, mScreenManager, ref mDirector, true, this);

            //ATTENTION: THE INGAME SCREENS ARE HANDLED IN THE LEVELS NOW!
            mScreenManager.AddScreen(mLoadGameManager);
            mScreenManager.AddScreen(mMainMenuManager);

            // TODO: load and play Soundtrack as background music
            // director.GetSoundManager.LoadContent(Content);
            //_mSoundManager.PlaySoundTrack();
            //XSerializer.Save(new Skirmish(mGraphics, ref mDirector, Content, mScreenManager), "SwagSireDrizzle's_Game.xml", false);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Not used/
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

            mDirector.GetActionManager.ActualExec();
            // make sure this is ALWAYS the last call in our update cycle otherwise things might get nasty.
            mDirector.GetDeathManager.KillAddedObjects();

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

        private void LoadConfig()
        {
            var configuration = XSerializer.Load(@"Config.xml", true);

            if (configuration.IsPresent())
            {
                mInstance = (GlobalVariablesInstance)configuration.Get();
            }
            else
            {
                mInstance = new GlobalVariablesInstance();
            }

            mInstance.LoadToStatic();
        }
    }
}
