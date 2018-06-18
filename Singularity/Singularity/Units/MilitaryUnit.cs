using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Map;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Units
{
    internal sealed class MilitaryUnit : ICollider, IRevealing, IMouseClickListener, IMousePositionListener
    {
        private const int DefaultWidth = 150;
        private const int DefaultHeight = 75;

        private static readonly Color SSelectedColor = Color.Gainsboro;
        private static readonly Color SNotSelectedColor = Color.White;

        private const double Speed = 4;

        private Color _mColor;

        private int _mColumn;
        private int _mRow;

        private bool _mIsMoving;
        private Rectangle _mBoundsSnapshot;
        private Vector2 _mToAdd;
        private double _mZoomSnapshot;

        private Vector2 _mMovementVector;

        private readonly Camera _mCamera;

        private Vector2 _mTargetPosition;
        private int _mRotation;
        private readonly Texture2D _mMilSheet;

        private bool _mSelected;

        private float _mMouseX;

        private float _mMouseY;


        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        public Vector2 Center { get; private set; }

        public bool Moved { get; private set; }

        public int RevelationRadius { get; private set; }

        public Rectangle Bounds { get; private set; }

        public Rectangle AbsBounds { get; private set; }

        public MilitaryUnit(Vector2 position, Texture2D spriteSheet, Camera camera, InputManager manager)
        {
            Id = IdGenerator.NextiD(); // TODO this will later use a random number generator to create a unique
                    // id for the specific unit.
            Health = 10; //TODO
            
            AbsolutePosition = position;
            AbsoluteSize = new Vector2(DefaultWidth, DefaultHeight);

            RevelationRadius = (int) AbsoluteSize.X;
            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X  / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);

            Moved = false;
            _mIsMoving = false;
            _mCamera = camera;

            manager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
            manager.AddMousePositionListener(this);

            _mMilSheet = spriteSheet;
        }


        /// <summary>
        /// Rotates unit in order when selected in order to face
        /// user mouse and eventually target destination
        /// </summary>
        /// <param name="target"></param>
        private void Rotate(Vector2 target)
        {
            // form a triangle from unit location to mouse location
            // adjust to be at center of sprite 150x75
            var x = (target.X - (RelativePosition.X + RelativeSize.X / 2));
            var y = (target.Y - (RelativePosition.Y + RelativeSize.Y / 2));
            var hypot = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            // calculate degree between formed triangle
            double degree;
            if (Math.Abs(hypot) < 0.01)
            {
                degree = 0;
            }
            else
            {
                degree = Math.Asin(y / hypot) * (180.0 / Math.PI);
            }

            // calculate rotation with increased degrees going counterclockwise
            if (x >= 0)
            {
                _mRotation = (int) (Math.Round(270 - degree, MidpointRounding.AwayFromZero));
            }
            else
            {
                _mRotation = (int) (Math.Round(90 + degree, MidpointRounding.AwayFromZero));
            }

            // add 42 degrees since sprite sheet starts at sprite -42d not 0
            _mRotation = (_mRotation + 42) % 360;

        }

        /// <summary>
        /// The property which defines the health of the unit
        /// </summary>
        private int Health { get; set; }

        /// <summary>
        /// The unique ID of the unit.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Used to set and get assignment. Assignment will potentially be implemented as a new type which
        /// can be used to determine what assignment a unit has.
        /// </summary>
        public string Assignment
        {
            get; set; //TODO
        }


        /// <summary>
        /// Damages the unit by a certain amount.
        /// </summary>
        /// <param name="damage"></param>
        public void MakeDamage(int damage)
        {
             Health -= damage; // TODO
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.Draw(
                _mMilSheet, 
                AbsolutePosition, 
                new Rectangle((150 * _mColumn), (75 * _mRow), (int) AbsoluteSize.X, (int) AbsoluteSize.Y), 
                _mColor, 
                0f, 
                Vector2.Zero, 
                Vector2.One, 
                SpriteEffects.None, 
                LayerConstants.MilitaryUnitLayer
                );

        }


        public void Update(GameTime gameTime)
        {

            //make sure to update the relative bounds rectangle enclosing this unit.
            Bounds = new Rectangle(
                (int)RelativePosition.X, (int)RelativePosition.Y, (int)RelativeSize.X, (int)RelativeSize.Y);

            // this makes the unit rotate according to the mouse position when its selected and not moving.
            if (_mSelected && !_mIsMoving)
            {
                Rotate(new Vector2(_mMouseX, _mMouseY));
            }

            if (HasReachedTarget())
            {
                _mIsMoving = false;
            }

            // calculate path to target position
            if (_mIsMoving && !HasReachedTarget())
            {
                MoveToTarget(_mTargetPosition);
            }

            // these are values needed to properly get the current sprite out of the spritesheet.
            _mRow = (_mRotation / 18);
            _mColumn = ((_mRotation - (_mRow * 18)) / 3);

            //finally select the appropriate color for selected/deselected units.
            _mColor = _mSelected ? SSelectedColor : SNotSelectedColor;

            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);
            AbsBounds = new Rectangle((int)AbsolutePosition.X, (int) AbsolutePosition.Y, (int)AbsoluteSize.X, (int) AbsoluteSize.Y);
            Moved = _mIsMoving;
        }

        /// <summary>
        /// Calculates the direction the unit should be moving and moves it into that direction. 
        /// </summary>
        /// <param name="target">The target to which to move</param>
        private void MoveToTarget(Vector2 target)
        {

            _mMovementVector = new Vector2(target.X - _mBoundsSnapshot.Center.X, target.Y - _mBoundsSnapshot.Center.Y);
            _mMovementVector.Normalize();
            _mToAdd += _mMovementVector * (float) (_mZoomSnapshot *  Speed);

            AbsolutePosition = new Vector2((float) (AbsolutePosition.X + _mMovementVector.X * Speed), (float) (AbsolutePosition.Y + _mMovementVector.Y * Speed));
        }

        /// <summary>
        /// Checks whether the target position is reached or not.
        /// </summary>
        private bool HasReachedTarget()
        {

            if (!(Math.Abs(_mBoundsSnapshot.Center.X + _mToAdd.X -
                           _mTargetPosition.X) < 8 &&
                  Math.Abs(_mBoundsSnapshot.Center.Y + _mToAdd.Y -
                           _mTargetPosition.Y) < 8))
            {
                return false;
            }
            _mToAdd = Vector2.Zero;
            return true;

        }

        public void MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            switch (mouseAction)
            {
                case EMouseAction.LeftClick:
                    if (_mSelected && !_mIsMoving && !withinBounds && Map.Map.IsOnTop(new Rectangle((int)(_mMouseX - RelativeSize.X / 2f), (int)(_mMouseY - RelativeSize.Y / 2f), (int)RelativeSize.X, (int)RelativeSize.Y), _mCamera))
                    {
                        _mIsMoving = true;
                        _mTargetPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                        _mBoundsSnapshot = Bounds;
                        _mZoomSnapshot = _mCamera.GetZoom();
                    }

                    if (withinBounds) { 
                        _mSelected = true;
                    }
                    return;

                case EMouseAction.RightClick:
                    _mSelected = false;
                    return;
            }
        }

        public void MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            
        }

        public void MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
           
        }

        public void MousePositionChanged(float newX, float newY)
        {
            _mMouseX = newX;
            _mMouseY = newY;
        }
    }
}
