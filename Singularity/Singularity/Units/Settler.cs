using System;
using System.Runtime.Serialization;
using System.Security.Policy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Screen.ScreenClasses;

namespace Singularity.Units
{
    /// <inheritdoc cref="FreeMovingUnit"/>
    [DataContract]
    internal sealed class Settler: FreeMovingUnit, IKeyListener
    {
        #region Declarations
        private GameScreen mGameScreen;

        private UserInterfaceScreen mUi;

        [DataMember]
        private bool mNeverMoved;
        #endregion

        /// <summary>
        /// Constructs a new settler at the specified position.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        /// <param name="gameScreen">Gamescreen of the game so that the settler can build platforms.</param>
        /// <param name="ui">UI of the game so the settler can edit the UI.</param>
        public Settler(Vector2 position, Camera camera, ref Director director, GameScreen gameScreen, UserInterfaceScreen ui)
            : base(position, camera, ref director)
        {
            Speed = 4;
            Health = 10;

            AbsoluteSize = new Vector2(20, 20);

            RevelationRadius = (int)AbsoluteSize.X * 6;

            mDirector.GetInputManager.FlagForAddition(this);

            mNeverMoved = true;

            mGameScreen = gameScreen;
            mUi = ui;

            /* too inefficient
            ColliderGrid = new[,]
            {
                {true, true},
                {true, true}
            };
            */

            SetAbsBounds();
        }

        public static Settler Create(Vector2 position, ref Director director)
        {
            var map = director.GetStoryManager.Level.Map;
            return new Settler(position, director.GetStoryManager.Level.Camera, ref director, director.GetStoryManager.Level.GameScreen, director.GetUserInterfaceController.ControlledUserInterface);
        }

        #region BuildCommanCenterEvent
        public delegate void SettlerEventHandler(object source, EventArgs args, Vector2 v, Settler s);
        public event SettlerEventHandler BuildCommandCenter;

        /// <summary>
        /// The subscriber and subscription occurs in the GameScreen class
        /// everytime a settler is added the Gamescreen starts to listen for
        /// a build command center event from it (if it recieves it it unsubscribes from
        /// this settle instance and also removes the settler from GameScreen)
        /// </summary>
        private void OnBuildCommandCenter()
        {
            if (BuildCommandCenter != null)
            {
                //I commented it out, so we get no Errors due to no Multiple Graph Compatibility.
                BuildCommandCenter(this, EventArgs.Empty, AbsolutePosition, this);
                //Note: It doesnt matter if this is called multiple times from settlers other than the first settler. It will only set variables
                //to true, that has been true already.
                mUi.Activate();
            }
            // debug.
            mUi.Activate();
        }
        #endregion

        public void ReloadContent(ref Director director, Camera camera, ref Map.Map map, GameScreen gamescreen, UserInterfaceScreen ui)
        {
            ReloadContent(ref director, camera);
            mGameScreen = gamescreen;
            mUi = ui;
            mDirector.GetInputManager.FlagForAddition(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draws settler as stroked rectangle
            spriteBatch.StrokedRectangle(AbsolutePosition,
                AbsoluteSize,
                Color.Gray,
                Selected ? Color.Wheat : Color.Beige,
                .8f,
                1f,
                LayerConstants.MilitaryUnitLayer);
            
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (Moved)
            {
                mNeverMoved = false;
            }
        }

        public override void SetAbsBounds()
        {
            AbsBounds = new Rectangle((int)AbsolutePosition.X,
                (int)AbsolutePosition.Y,
                (int)AbsoluteSize.X,
                (int)AbsoluteSize.Y);
        }

        public override bool Die()
        {
            base.Die();
            mDirector.GetInputManager.FlagForRemoval((IKeyListener) this);
            return true;
        }

        public bool KeyTyped(KeyEvent keyEvent)
        {
            // b key is used to convert the settler unit into a command center
            var keyArray = keyEvent.CurrentKeys;
            foreach (var key in keyArray)
            {
                // if key b has been pressed and the settler unit is selected and its not moving
                // --> send out event that deletes settler and adds a command center
                if (key != Keys.B || !Selected || (!mGroup.Get().NearTarget() && !mNeverMoved)) continue;
                OnBuildCommandCenter();
                return false;
            }

            return true;
        }


        #region Unused EventListeners
        public bool KeyPressed(KeyEvent keyEvent)
        {
            return true;
        }

        public bool KeyReleased(KeyEvent keyEvent)
        {
            return true;
        }

        #endregion
    }
}
