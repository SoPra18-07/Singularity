using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;

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

        public LoseScreen(Director director, IScreenManager screenManager)
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

            spriteBatch.StrokedRectangle(Vector2.Zero, new Vector2(mScreenSize.X, mScreenSize.Y), new Color(mFadingScreenColorValue, 0, 0), new Color(mFadingScreenColorValue, 0, 0), 0.65f, 0.65f);

            if (mCounter >= 100 && mCounter < 250)
            {
                spriteBatch.DrawString(mLibSans72, "Defeat", new Vector2((mScreenSize.X - mLibSans72.MeasureString("Defeat").X) / 2, (mScreenSize.Y - mSingularityLogo.Height) / 4), Color.Red);

                spriteBatch.Draw(mSingularityLogo, new Vector2((mScreenSize.X - mSingularityLogo.Width) / 2, (mScreenSize.Y - mSingularityLogo.Height) / 2), Color.White);
            }
            else if (mCounter >= 250)
            {
                spriteBatch.Draw(mSingularityLogo, new Vector2((mScreenSize.X - mSingularityLogo.Width) / 2, (mScreenSize.Y - mSingularityLogo.Height) / 2), new Color(0.2f, 0.2f, 0.2f, 0.1f));

                spriteBatch.DrawString(mLibSans72, "Defeat", new Vector2((mScreenSize.X - mLibSans72.MeasureString("Defeat").X) / 2, (mScreenSize.Y - mSingularityLogo.Height) / 4), Color.Red);

                spriteBatch.Draw(mSingularityText, new Vector2((mScreenSize.X - mSingularityText.Width) / 2, (mScreenSize.Y - mSingularityText.Height) / 2.5f), Color.White);
            }

            if (mCounter >= 300)
            {
                mStatisticsWindow.Draw(spriteBatch);

                mMainMenuButton.Draw(spriteBatch);
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

            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Units created: ", mDirector.GetStoryManager.Units["created"], Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));
            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Units lost: ", mDirector.GetStoryManager.Units["lost"], Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));
            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Units killed: ", mDirector.GetStoryManager.Units["killed"], Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));
            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Resources created: ", mDirector.GetStoryManager.Resources.Sum(x => x.Value), Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));
            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Platforms created: ", mDirector.GetStoryManager.Platforms["created"], Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));
            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Platforms lost: ", mDirector.GetStoryManager.Platforms["lost"], Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));
            mStatisticsWindow.AddItem(new TextAndAmountIWindowItem("Platforms destroyed: ", mDirector.GetStoryManager.Platforms["destroyed"], Vector2.Zero, new Vector2(mStatisticsWindow.Size.X, mLibSans14.MeasureString("A").Y), mLibSans14, Color.White));

            var measuredButtonStringSize = mLibSans20.MeasureString("Main Menu");

            var buttonPositionX = mScreenSize.X - measuredButtonStringSize.X - 20;//mScreenSize.X - ((mScreenSize.X - mStatisticsWindow.Size.X) / 2 - measuredButtonStringSize.X) / 2 - measuredButtonStringSize.X;
            var buttonPositionY = 20;//mScreenSize.Y - measuredButtonStringSize.Y - 20;//mScreenSize.Y - ((mScreenSize.X - mStatisticsWindow.Size.X) / 2 - measuredButtonStringSize.X) / 2 - measuredButtonStringSize.Y;

            mMainMenuButton = new Button("Main Menu", mLibSans20, new Vector2(buttonPositionX, buttonPositionY), Color.Red) { Opacity = 1f };

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
            for (int i = 0; i < mScreenManager.GetScreenCount() - 1; i++)
            {
                mScreenManager.RemoveScreen();
            }
            LoadGameManagerScreen.OnReturnToMainMenuClicked(sender, eventArgs);
        }
    }
}
