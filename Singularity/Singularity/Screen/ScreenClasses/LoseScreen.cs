﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph.Paths;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Utils;

namespace Singularity.Screen.ScreenClasses
{
    public sealed class LoseScreen : IScreen
    {
        private SpriteFont mLibSans72;
        private SpriteFont mLibSans14;
        private SpriteFont mLibSans20;

        private readonly Vector2 mScreenSize;

        private readonly Director mDirector;

        private WindowObject mStatisticsWindow;

        private Texture2D mSingularityText;

        private Texture2D mSingularityLogo;

        private float mFadingScreenColorValue;

        private Button mMainMenuButton;

        private int mCounter;

        private readonly IScreenManager mScreenManager;

        public LoseScreen(ref Director director, IScreenManager screenManager)
        {
            mDirector = director;
            mScreenManager = screenManager;

            mScreenSize = new Vector2(mDirector.GetGraphicsDeviceManager.PreferredBackBufferWidth, mDirector.GetGraphicsDeviceManager.PreferredBackBufferHeight);

            mFadingScreenColorValue = 0.5f;
            mCounter = 0;
        }

        public bool Loaded { get; set; }

        public EScreen Screen { get; } = EScreen.LoseScreen;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.FillRectangle(location: Vector2.Zero,
                size: new Vector2(x: mScreenSize.X,
                    y: mScreenSize.Y),
                color: new Color(r: mFadingScreenColorValue,
                                 g: 0,
                                 b: 0) * 0.65f);

            if (mCounter >= 100 && mCounter < 250)
            {
                spriteBatch.DrawString(spriteFont: mLibSans72,
                    text: "Defeat",
                    position: new Vector2(x: (mScreenSize.X - mLibSans72.MeasureString(text: "Victory").X) / 2,
                                          y: 100),
                    color: Color.Maroon);
                spriteBatch.Draw(texture: mSingularityLogo,
                    position: mScreenSize / 2,
                    sourceRectangle: null,
                    color: Color.White,
                    rotation: 0f,
                    origin: new Vector2(mSingularityLogo.Width, mSingularityLogo.Height) / 2,
                    scale: 0.5f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);

            }
            else if (mCounter >= 250)
            {
                spriteBatch.Draw(texture: mSingularityLogo,
                    position: mScreenSize / 2,
                    sourceRectangle: null,
                    color: new Color(r: 0.2f,
                        g: 0.2f,
                        b: 0.2f,
                        alpha: 0.1f),
                    rotation: 0f,
                    origin: new Vector2(mSingularityLogo.Width, mSingularityLogo.Height) / 2,
                    scale: 0.5f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);

                spriteBatch.DrawString(spriteFont: mLibSans72,
                    text: "Defeat",
                    position: new Vector2(x: (mScreenSize.X -
                                              mLibSans72.MeasureString(text: "Victory")
                                                  .X) /
                                             2,
                        y: 100),
                    color: Color.Maroon);

                spriteBatch.Draw(texture: mSingularityText,
                    position: new Vector2(x: mScreenSize.X / 2,
                                          y: mScreenSize.Y / 2.5f),
                    sourceRectangle: null,
                    color: new Color(r: 1f,
                        g: 1f,
                        b: 1f,
                        alpha: 1f),
                    rotation: 0f,
                    origin: new Vector2(mSingularityText.Width / 2f, mSingularityText.Height / 2f),
                    scale: 0.8f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }

            if (mCounter >= 300)
            {
                mStatisticsWindow.Draw(spriteBatch: spriteBatch);
                var measuredButtonStringSize = mLibSans20.MeasureString("Main Menu");
                var buttonPositionX = mScreenSize.X - measuredButtonStringSize.X - 20;
                var buttonPositionY = 20;

                spriteBatch.FillRectangle(new Vector2(buttonPositionX, buttonPositionY), measuredButtonStringSize, Color.Black);

                mMainMenuButton.Draw(spriteBatch: spriteBatch);
            }

            spriteBatch.End();
        }

        public bool DrawLower()
        {
            return true;
        }

        public void LoadContent(ContentManager content)
        {
            mLibSans72 = content.Load<SpriteFont>("LibSans72");
            mLibSans14 = content.Load<SpriteFont>("LibSans14");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mSingularityText = content.Load<Texture2D>("SingularityText");
            mSingularityLogo = content.Load<Texture2D>("Logo");

            mStatisticsWindow = new WindowObject("// Statistics", new Vector2((mScreenSize.X - mScreenSize.X / 2) / 2, mScreenSize.Y - (mScreenSize.Y / 2.8f) - 20), new Vector2(mScreenSize.X / 2, mScreenSize.Y / 2.8f), Color.White, new Color(0.467f, 0.534f, 0.6f, 0.8f), 10, 20, false, mLibSans20, mDirector, EScreen.LoseScreen) { Active = false };

            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Units created: ", mDirector.GetStoryManager.UnitsCount["created"], Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));
            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Units lost: ", mDirector.GetStoryManager.UnitsCount["lost"], Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));
            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Units killed: ", mDirector.GetStoryManager.UnitsCount["killed"], Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));
            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Resources created: ", mDirector.GetStoryManager.ResourcesCount.Sum(x => x.Value), Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));
            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Platforms created: ", mDirector.GetStoryManager.PlatformsCount["created"], Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));
            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Platforms lost: ", mDirector.GetStoryManager.PlatformsCount["lost"], Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));
            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Platforms destroyed: ", mDirector.GetStoryManager.PlatformsCount["destroyed"], Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));

            var measuredButtonStringSize = mLibSans20.MeasureString("Main Menu");

            var buttonPositionX = mScreenSize.X - measuredButtonStringSize.X - 20;
            var buttonPositionY = 20;

            mMainMenuButton = new Button("Main Menu", mLibSans20, new Vector2(buttonPositionX, buttonPositionY), Color.White, true) { Opacity = 1f };

            mMainMenuButton.ButtonReleased += ReturnToMainMenu;
        }

        public void Update(GameTime gametime)
        {
            if (mFadingScreenColorValue > 0)
            {
                mFadingScreenColorValue -= 0.005f;
            }

            if (mCounter < 250)
            {
                mCounter += 1;
            }
            else if (mCounter < 300)
            {
                mCounter += 1;
                mStatisticsWindow.Active = true;
            }
            else
            {
                mStatisticsWindow.Update(gametime);
                mMainMenuButton.Update(gametime);
            }
        }

        public bool UpdateLower()
        {
            return false;
        }

        private void ReturnToMainMenu(object sender, EventArgs eventArgs)
        {
            mDirector.GetStoryManager.Level.GameScreen.Unload();
            mDirector.GetClock = new Clock();
            mDirector.GetIdGenerator = new IdGenerator();
            mDirector.GetInputManager.RemoveEverythingFromInputManager();
            mDirector.GetStoryManager = new StoryManager(mDirector);
            mDirector.GetStoryManager.SetScreenManager(mScreenManager);
            mDirector.GetPathManager = new PathManager();
            mDirector.GetDistributionDirector = new DistributionDirector(mDirector);
            mDirector.GetMilitaryManager = new MilitaryManager(mDirector);
            mDirector.GetDeathManager = new DeathManager();
            mDirector.GetActionManager = new ActionManager();

            for (var i = 0; i < mScreenManager.GetScreenCount() - 1; i++)
            {
                mScreenManager.RemoveScreen();
            }
            LoadGameManagerScreen.OnReturnToMainMenuClicked(sender, eventArgs);
        }
    }
}
