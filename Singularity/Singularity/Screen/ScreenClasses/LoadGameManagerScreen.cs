using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Exceptions;
using Singularity.Levels;
using Singularity.Manager;
using Singularity.Serialization;

namespace Singularity.Screen.ScreenClasses
{
    internal sealed class LoadGameManagerScreen : IScreen
    {
        public EScreen Screen { get; private set; } = EScreen.LoadGameManagerScreen;

        public bool Loaded { get; set; }

        /// <inheritdoc cref="IScreen"/>
        /// <summary>
        /// Manages the Loading of the Level/GameScreen. This is the screen that is first loaded into the stack screen manager
        /// and basically only handles the transition from LoadSelectScreen to the GameScreen (which has to be loaded) itself.
        /// </summary>
        private readonly IScreenManager mScreenManager;

        //The name of the game to load
        private string mName;

        private bool mGameLoaded;

        private ILevel mLevel;
        private bool mNewGame;

        // All connecting screens
        private ITransitionableMenu mLoadingScreen;
        private GameScreen mGameScreen;

        private UserInterfaceScreen mUi;

        // Screen transition variables
        private static string sPressed;
        private int mTransitionState;

        // The game itself (to allow for quitting)
        private readonly Game1 mGame;

        // viewport resolution changes
        private static Vector2 sViewportResolution;
        private static bool sResolutionChanged;

        private GraphicsDeviceManager mGraphics;
        private Director mDirector;
        private ContentManager mContent;

        /// <summary>
        /// Creates an instance of the LoadGameManagerScreen class
        /// </summary>
        /// <param name="graphics">The Graphicsdevicemanager of the game</param>
        /// <param name="director">The Director of the game</param>
        /// <param name="content">The Contentmanager of the game</param>
        /// <param name="screenResolution">Screen resolution of the game.</param>
        /// <param name="screenManager">Stack screen manager of the game.</param>
        /// <param name="game">Used to pass on to the options screen to change game settings</param>
        public LoadGameManagerScreen(GraphicsDeviceManager graphics, ref Director director, ContentManager content,Vector2 screenResolution, IScreenManager screenManager, Game1 game)
        {
            mScreenManager = screenManager;
            mGame = game;
            mGraphics = graphics;
            mDirector = director;
            mContent = content;

            Initialize(screenResolution);

            sPressed = "None";
            sResolutionChanged = false;
            mGameLoaded = false;
            mNewGame = false;
            mName = "";
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            //This Screen has no content to load.
        }

        /// <summary>
        /// Updates the state of the LoadGameManager and changes to the game if the conditions are met.
        /// by the stack screen manager
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (mGameLoaded)
            {
                
                mGameLoaded = false;
            }

            if (sResolutionChanged)
            {
                Initialize(sViewportResolution);
                sResolutionChanged = false;
            }

            switch (sPressed)
            {
                case "None":
                    return;
                case "Skirmish":
                    mLevel = new Skirmish(mGraphics, ref mDirector, mContent, mScreenManager, LevelType.Skirmish);
                    mGameScreen = mLevel.GameScreen;
                    mUi = mLevel.Ui;
                    mNewGame = true;
                    break;
                case "TechDemo":
                    mLevel = new TechDemo(mGraphics, ref mDirector, mContent, mScreenManager, LevelType.Techdemo);
                    mGameScreen = mLevel.GameScreen;
                    mUi = mLevel.Ui;
                    mNewGame = true;
                    break;
                case "Save1":
                    mName = XSerializer.GetSaveNames()[0];
                    break;
                case "Save2":
                    mName = XSerializer.GetSaveNames()[1];
                    break;
                case "Save3":
                    mName = XSerializer.GetSaveNames()[2];
                    break;
                case "Save4":
                    mName = XSerializer.GetSaveNames()[3];
                    break;
                case "Save5":
                    mName = XSerializer.GetSaveNames()[4];
                    break;
                case "ReturnToMainMenu":
                    mScreenManager.AddScreen(new MainMenuManagerScreen(sViewportResolution, mScreenManager, true, mGame));
                    break;
                default:
                    throw new InvalidGenericArgumentException(
                        "Somehow the LoadGameManagerScreen was assigned to a button that he should not have been assigned to. Cannot handle" +
                        "this State");
            }

            //This means a save has to be loaded
            if (mName != "")
            {
                var levelToBe = XSerializer.Load(mName, false);
                if (levelToBe.IsPresent())
                {
                    mScreenManager.AddScreen(mLoadingScreen);
                    mLevel = (ILevel)levelToBe.Get();
                    mLevel.ReloadContent(mContent, mGraphics, ref mDirector, mScreenManager);
                    mGameScreen = mLevel.GameScreen;
                    mUi = mLevel.Ui;
                    //Remove all screens above this screen, of course this only works if this screen is really on the bottom of the stack
                    for (var i = mScreenManager.GetScreenCount() - 1; i > 0; i--)
                    {
                        mScreenManager.RemoveScreen();
                    }
                    mScreenManager.AddScreen(mGameScreen);
                    mScreenManager.AddScreen(mUi);
                    mGameLoaded = true;
                    mName = "";
                }
            }
            
            else if (mNewGame)
            {
                //Remove all screens above this screen, of course this only works if this screen is really on the bottom of the stack
                for (var i = mScreenManager.GetScreenCount() - 1; i > 0; i--)
                {
                    mScreenManager.RemoveScreen();
                }
                mScreenManager.AddScreen(mGameScreen);
                mScreenManager.AddScreen(mUi);
                
                mGameLoaded = true;
                mNewGame = false;
            }
            
            sPressed = "None";
        }


        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // This screen draws nothing
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should update.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be updated.</returns>
        public bool UpdateLower()
        {
            //Below this screen is nothing so dont update anything.
            return false;
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should be drawn.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be drawn.</returns>
        public bool DrawLower()
        {
            //Below this screen is nothing so dont draw anything.
            return false;
        }

        public static void SetResolution(Vector2 viewportResolution)
        {
            sResolutionChanged = true;
            sViewportResolution = viewportResolution;
        }

        /// <summary>
        /// Initialize the Loading Screen, by creating it with the desired resolution.
        /// </summary>
        /// <param name="screenResolution"></param>
        private void Initialize(Vector2 screenResolution)
        {
            mLoadingScreen = new LoadingScreen(screenResolution);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArg"></param>
        public static void OnReturnToMainMenuClicked(Object sender, EventArgs eventArg)
        {
            sPressed = "ReturnToMainMenu";
        }

        #region MainMenuScreen Button Handlers

        /// <summary>
        /// Receives Save1 button released event and changes sPressed
        /// to result in Loading the game within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArg"></param>
        public static void OnSave1Released(Object sender, EventArgs eventArg)
        {
            sPressed = "Save1";
        }

        /// <summary>
        /// Receives Save2 button released event and changes sPressed
        /// to result in Loading the game within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArg"></param>
        public static void OnSave2Released(Object sender, EventArgs eventArg)
        {
            sPressed = "Save2";
        }

        /// <summary>
        /// Receives Save3 button released event and changes sPressed
        /// to result in Loading the game within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArg"></param>
        public static void OnSave3Released(Object sender, EventArgs eventArg)
        {
            sPressed = "Save3";
        }

        /// <summary>
        /// Receives Save4 button released event and changes sPressed
        /// to result in Loading the game within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArg"></param>
        public static void OnSave4Released(Object sender, EventArgs eventArg)
        {
            sPressed = "Save4";
        }

        /// <summary>
        /// Receives Save5 button released event and changes sPressed
        /// to result in Loading the game within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArg"></param>
        public static void OnSave5Released(Object sender, EventArgs eventArg)
        {
            sPressed = "Save5";
        }

        /// <summary>
        /// Receives Save5 button released event and changes sPressed
        /// to result in Loading the game within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArg"></param>
        public static void OnSkirmishReleased(Object sender, EventArgs eventArg)
        {
            sPressed = "Skirmish";
        }

        /// <summary>
        /// Used to create a new story mode game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnStoryButtonReleased(Object sender, EventArgs eventArgs)
        {
            // TODO: implement start game with story
            throw new NotImplementedException("No story yet unfortunately");

        }

        public static void OnTechDemoButtonReleased(Object sender, EventArgs eventArgs)
        {
            sPressed = "TechDemo";
        }
        #endregion

    }
}

