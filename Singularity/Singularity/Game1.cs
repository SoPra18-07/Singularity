using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Singularity.platform;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public sealed class Game1 : Game
    {
        private GraphicsDeviceManager mGraphics;
        
        private PlatformBlank mPlatform;
        private PlatformBlank mPlatform2;

        private MilitaryUnit mMUnit1;
        private MilitaryUnit mMUnit2;
        
        private Map.Map mMap;

        private static Song sSoundtrack;

        // Screens
        private GameScreen mGameScreen;
        private MainMenuManagerScreen mMainMenuManager;

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
            mMUnit1 = new MilitaryUnit(new Vector2(600, 600), mMUnitSheet);
            mMUnit2 = new MilitaryUnit(new Vector2(100, 600), mMUnitSheet);

            mPlatformSheet = Content.Load<Texture2D>("PlatformSpriteSheet");
            mPlatform = new PlatformBlank(new Vector2(300, 400), mPlatformSheet);
            mPlatform2 = new PlatformBlank(new Vector2(800, 600), mPlatformSheet);

            mMap = new Map.Map(Content.Load<Texture2D>("MockUpBackground"), mGraphics.GraphicsDevice.Viewport, true);

            mMap.AddPlatform(mPlatform);
            mMap.AddPlatform(mPlatform2);

            mGameScreen = new GameScreen(mMap);

            // This loads the contents of the mainmenuscreen
            mMainMenuManager = new MainMenuManagerScreen(viewportResolution, mScreenManager, true);

            // Add the screens to the screen manager
            // The idea is that the game screen is always at the bottom and stuff is added simply
            // on top of it.
            mScreenManager.AddScreen(mGameScreen);
            mScreenManager.AddScreen(mMainMenuManager);
            // TODO load game screen contents only after game new game or load game has been started

            mMainMenuManager.LoadContent(Content);

            // load roads
            mRoad1 = new Road(new Vector2(300, 400), new Vector2(800, 600), false);

            mGameScreen.AddObject<MilitaryUnit>(mMUnit1);
            mGameScreen.AddObject<MilitaryUnit>(mMUnit2);
            mGameScreen.AddObject<PlatformBlank>(mPlatform);
            mGameScreen.AddObject<PlatformBlank>(mPlatform2);
            mGameScreen.AddObject<Road>(mRoad1);

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