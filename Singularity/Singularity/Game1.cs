using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Singularity.platform;
using Singularity.Screen;
using Singularity.Units;

namespace Singularity
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public sealed class Game1 : Game
    {
        private GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private Texture2D mPlatformSheet;
        private PlatformBlank mPlatform;
        private Texture2D mMUnitSheet;
        private MilitaryUnit mMUnit1;
        private MilitaryUnit mMUnit2;
        private PlatformBlank mPlatform2;
        private Map.Map mMap;
        private static Song sSoundtrack;

        // roads
        private Road mRoad1;

        // Sprites!

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
            // TODO: Add your initialization logic here
            // can be used to debug the screen manager
            /*
               mScreenManager.AddScreen(new RenderLowerScreen());
               mScreenManager.AddScreen(new UpdateLowerScreen());
            */
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
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            mMUnitSheet = Content.Load<Texture2D>("UnitSpriteSheet");
            mMUnit1 = new MilitaryUnit(new Vector2(600, 600), mMUnitSheet);
            mMUnit2 = new MilitaryUnit(new Vector2(100, 600), mMUnitSheet);

            // TODO: use this.Content to load your game content here
            mPlatformSheet = Content.Load<Texture2D>("PlatformSpriteSheet");
            mPlatform = new PlatformBlank(new Vector2(300, 400), mPlatformSheet);
            mPlatform2 = new PlatformBlank(new Vector2(800, 600), mPlatformSheet);

            var lineTexture = new Texture2D(mGraphics.GraphicsDevice, 1, 1);
            lineTexture.SetData<Color>(new Color[] { Color.White });
            mMap = new Map.Map(Content.Load<Texture2D>("MockUpBackground"), mGraphics.GraphicsDevice.Viewport, lineTexture);

            mMap.AddPlatform(mPlatform);
            mMap.AddPlatform(mPlatform2);

            // load roads
            mRoad1 = new Road(new Vector2(300, 400), new Vector2(800, 600), false);

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

            // TODO: Add your update logic here
            mScreenManager.Update(gameTime);
            mMap.Update(gameTime);
            mMUnit1.Update(gameTime);
            mMUnit2.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            mScreenManager.Draw(mSpriteBatch);
            mMap.Draw(mSpriteBatch);
            mMUnit1.Draw(mSpriteBatch);
            mMUnit2.Draw(mSpriteBatch);
            mRoad1.Draw(mSpriteBatch);
            base.Draw(gameTime);
        }
    }
}