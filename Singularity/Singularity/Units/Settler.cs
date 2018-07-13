using System;
using System.Runtime.Serialization;
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
    /// <inheritdoc cref="ControllableUnit"/>
    [DataContract]
    internal sealed class Settler: ControllableUnit, IKeyListener
    {
        #region Declarations
        [DataMember]
        private GameScreen mGameScreen;
        [DataMember]
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
        /// <param name="map">Reference to the game map.</param>
        /// <param name="gameScreen">Gamescreen of the game so that the settler can build platforms.</param>
        /// <param name="ui">UI of the game so the settler can edit the UI.</param>
        public Settler(Vector2 position, Camera camera, ref Director director, ref Map.Map map, GameScreen gameScreen, UserInterfaceScreen ui)
            : base(position, camera, ref director, ref map)
        {
            mSpeed = 4;
            Health = 10;

            AbsoluteSize = new Vector2(20, 20);

            RevelationRadius = (int)AbsoluteSize.X * 6;

            mDirector.GetInputManager.AddKeyListener(this);

            mNeverMoved = true;

            mGameScreen = gameScreen;
            mUi = ui;
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
        }
        #endregion


        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draws settler as stroked rectangle
            spriteBatch.StrokedRectangle(AbsolutePosition,
                AbsoluteSize,
                Color.Gray,
                mSelected ? Color.Wheat : Color.Beige,
                .8f,
                1f,
                LayerConstants.MilitaryUnitLayer);

            #region Debug

            if (mDebugPath == null)
            {
                return;
            }

            for (var i = 0; i < mDebugPath.Length - 1; i++)
            {
                spriteBatch.DrawLine(mDebugPath[i], mDebugPath[i + 1], Color.Orange);
            }
            #endregion
        }


        public override void Update(GameTime gameTime)
        {
            //make sure to update the relative bounds rectangle enclosing this unit.
            Bounds = new Rectangle((int) RelativePosition.X,
                (int) RelativePosition.Y,
                (int) RelativeSize.X,
                (int) RelativeSize.Y);

            if (HasReachedTarget())
            {
                mIsMoving = false;
            }

            // calculate path to target position
            else if (mIsMoving)
            {
                if (!HasReachedWaypoint())
                {
                    MoveToTarget(mPath.Peek(), mSpeed);
                }

                else
                {
                    mPath.Pop();
                    MoveToTarget(mPath.Peek(), mSpeed);
                }
            }

            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);

            AbsBounds = new Rectangle((int) AbsolutePosition.X + 16,
                (int) AbsolutePosition.Y + 11,
                (int) AbsoluteSize.X,
                (int) AbsoluteSize.Y);
            Moved = mIsMoving;

            if (Moved)
            {
                mNeverMoved = false;
            }

        }

        public bool KeyTyped(KeyEvent keyEvent)
        {
            // b key is used to convert the settler unit into a command center
            var keyArray = keyEvent.CurrentKeys;
            foreach (var key in keyArray)
            {
                // if key b has been pressed and the settler unit is selected and its not moving
                // --> send out event that deletes settler and adds a command center
                if (key == Keys.B && mSelected && (HasReachedTarget() || mNeverMoved))
                {
                    OnBuildCommandCenter();
                    return false;
                }
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
