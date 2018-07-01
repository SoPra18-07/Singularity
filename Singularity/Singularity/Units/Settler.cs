using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platform;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Utils;

namespace Singularity.Units
{
    [DataContract]
    class Settler: ICollider, IRevealing, IMouseClickListener, IMousePositionListener, IKeyListener
    {


        #region Declarations
        [DataMember]
        private readonly Camera mCamera;
        [DataMember]
        private Director mDirector;
        [DataMember]
        private Map.Map mMap;
        [DataMember]
        private MilitaryPathfinder mPathfinder;
        [DataMember]
        private static readonly Color sSelectedColor = Color.Bisque;
        [DataMember]
        private static readonly Color sNotSelectedColor = Color.Beige;
        [DataMember]
        private bool mSelected;
        [DataMember]
        private const double Speed = 2;
        [DataMember]
        private bool mIsMoving;
        [DataMember]
        private Rectangle mBoundsSnapshot;
        [DataMember]
        private Vector2 mToAdd;
        [DataMember]
        private double mZoomSnapshot;
        [DataMember]
        private Vector2 mMovementVector;
        [DataMember]
        private Vector2 mTargetPosition;
        [DataMember]
        private float mMouseX;
        [DataMember]
        private float mMouseY;
        [DataMember]
        private Stack<Vector2> mPath;
        private Vector2[] mDebugPath; //TODO this is for debugging
        [DataMember]
        private GameScreen mGameScreen;
        [DataMember]
        private UserInterfaceScreen mUi;
        #endregion

        #region Properties
        // TODO i use this bool for now to make the settler inactive
        // TODO im not sure exactly how to remove it from the 
        [DataMember]
        public bool Dead { get; private set; }
        [DataMember]
        private int Health { get; set; }
        [DataMember]
        public Vector2 RelativePosition { get; set; }
        [DataMember]
        public Vector2 RelativeSize { get; set; }
        [DataMember]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember]
        public Vector2 AbsoluteSize { get; set; }
        [DataMember]
        public bool[,] ColliderGrid { get; }
        [DataMember]
        public Rectangle AbsBounds { get; private set; }
        [DataMember]
        public bool Moved { get; private set; }
        [DataMember]
        public int Id { get; }
        [DataMember]
        public int RevelationRadius { get; }
        [DataMember]
        public Vector2 Center { get; private set; }
        [DataMember]
        public EScreen Screen { get; } = EScreen.GameScreen;
        [DataMember]
        public Rectangle Bounds { get; private set; }

        #endregion

        // constructor for settler (position)
        public Settler(Vector2 position, Camera camera, ref Director director, ref Map.Map map, GameScreen gameScreen, UserInterfaceScreen ui)
        {
            Id = IdGenerator.NextiD(); // id for the specific unit.
            Health = 10;

            AbsolutePosition = position;
            AbsoluteSize = new Vector2(20, 20);

            RevelationRadius = (int)AbsoluteSize.X * 3;
            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);

            Moved = false;
            mCamera = camera;

            mDirector = director;

            mDirector.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
            Debug.WriteLine("Added to mouse click listener");
            mDirector.GetInputManager.AddKeyListener(this);
            mDirector.GetInputManager.AddMousePositionListener(this);

            mMap = map;

            mPathfinder = new MilitaryPathfinder();
            mGameScreen = gameScreen;
            mUi = ui;
            mTargetPosition = Center;
            Dead = false;

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
                BuildCommandCenter(this, EventArgs.Empty, AbsolutePosition, this);
                //Note: It doesnt matter if this is called multiple times from settlers other than the first settler. It will only set variables
                //to true, that has been true already.
                mUi.Activate();
            }
        }
        #endregion

        /// <summary>
        /// Inflicts damage on the Settler Unit
        /// </summary>
        /// <param name="damage"> amount of damage to be inflicted on Unit</param>
        public void MakeDamage(int damage)
        {
            Health -= damage; // TODO
            if (Health <= 0)
            {
                Dead = true;
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // Draws settler as stroked rectangle
            spriteBatch.StrokedRectangle(AbsolutePosition,
                AbsoluteSize,
                Color.Gray,
                (mSelected) ? sSelectedColor : sNotSelectedColor,
                .8f,
                1f,
                LayerConstants.MilitaryUnitLayer);

            // TODO DEBUG REGION
            if (mDebugPath != null)
            {
                for (var i = 0; i < mDebugPath.Length - 1; i++)
                {
                    spriteBatch.DrawLine(mDebugPath[i], mDebugPath[i + 1], Color.Orange);
                }
            }
        }


        public void Update(GameTime gameTime)
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
                    MoveToTarget(mPath.Peek());
                }

                else
                {
                    mPath.Pop();
                    MoveToTarget(mPath.Peek());
                }
            }

            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);

            // TODO modify aboslute bounds (these are taken from military unit)
            AbsBounds = new Rectangle((int) AbsolutePosition.X + 16,
                (int) AbsolutePosition.Y + 11,
                (int) (AbsoluteSize.X),
                (int) (AbsoluteSize.Y));
            Moved = mIsMoving;
            
        }

        private void MoveToTarget(Vector2 target)
        {

            var movementVector = new Vector2(target.X - Center.X, target.Y - Center.Y);
            movementVector.Normalize();
            mToAdd += mMovementVector * (float)(mZoomSnapshot * Speed);

            AbsolutePosition = new Vector2((float)(AbsolutePosition.X + movementVector.X * Speed), (float)(AbsolutePosition.Y + movementVector.Y * Speed));
        }


        private bool HasReachedTarget()
        {

            if (!(Math.Abs(Center.X + mToAdd.X -
                           mTargetPosition.X) < 8 &&
                  Math.Abs(Center.Y + mToAdd.Y -
                           mTargetPosition.Y) < 8))
            {
                return false;
            }
            mToAdd = Vector2.Zero;
            return true;
        }

        private bool HasReachedWaypoint()
        {
            if (Math.Abs(Center.X + mToAdd.X - mPath.Peek().X) < 8
                && Math.Abs(Center.Y + mToAdd.Y - mPath.Peek().Y) < 8)
            {
                Debug.WriteLine("Waypoint reached.");
                Debug.WriteLine("Next waypoint: " + mPath.Peek());
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            var giveThrough = true;

            switch (mouseAction)
            {
                case EMouseAction.LeftClick:
                    if (mSelected && !mIsMoving && !withinBounds && Map.Map.IsOnTop(
                            new Rectangle((int) (mMouseX - RelativeSize.X / 2f),
                                (int) (mMouseY - RelativeSize.Y / 2f),
                                (int) RelativeSize.X,
                                (int) RelativeSize.Y),
                            mCamera))
                    {
                        mIsMoving = true;
                        mTargetPosition = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                            Matrix.Invert(mCamera.GetTransform()));
                        var currentPosition = Center;
                        Debug.WriteLine("Starting path finding at: " + currentPosition.X + ", " + currentPosition.Y);
                        Debug.WriteLine("Target: " + mTargetPosition.X + ", " + mTargetPosition.Y);

                        mPath = new Stack<Vector2>();
                        mPath = mPathfinder.FindPath(currentPosition,
                            mTargetPosition,
                            ref mMap);

                        // TODO: DEBUG REGION
                        mDebugPath = mPath.ToArray();

                        // TODO: END DEBUG REGION

                        mBoundsSnapshot = Bounds;
                        mZoomSnapshot = mCamera.GetZoom();
                        giveThrough = true;
                    }

                    if (withinBounds)
                    {
                        mSelected = true;
                        giveThrough = false;
                    }

                    break;

                case EMouseAction.RightClick:
                    mSelected = false;
                    giveThrough = true;
                    break;
            }

            return giveThrough;
        }

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            mMouseX = screenX;
            mMouseY = screenY;

        }

        public void KeyTyped(KeyEvent keyEvent)
        {
            // b key is used to convert the settler unit into a command center
            Keys[] keyArray = keyEvent.CurrentKeys;
            foreach (Keys key in keyArray)
            {
                // if key b has been pressed and the settler unit is selected and its not moving
                // --> send out event that deletes settler and adds a command center
                if (key == Keys.B && mSelected && HasReachedTarget())
                {
                    OnBuildCommandCenter();
                }  
            }
        }


        #region NotUsed
        public void KeyPressed(KeyEvent keyEvent)
        {

        }

        public void KeyReleased(KeyEvent keyEvent)
        {
            
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }
        #endregion


    }
}
