using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Sound;
using Singularity.Utils;

namespace Singularity.Units
{
    internal sealed class MilitaryUnit : ICollider, IRevealing, IMouseClickListener, IMousePositionListener
    {
        public EScreen Screen { get; private set; } = EScreen.GameScreen;

        private const int DefaultWidth = 150;
        private const int DefaultHeight = 75;

        private static readonly Color sSelectedColor = Color.Gray;
        private static readonly Color sNotSelectedColor = Color.DarkGray;

        private const double Speed = 4;

        private Color mColor;

        private int mColumn;
        private int mRow;

        private bool mIsMoving;
        private Rectangle mBoundsSnapshot;
        private Vector2 mToAdd;
        private double mZoomSnapshot;

        private Vector2 mMovementVector;

        private readonly Camera mCamera;

        private Vector2 mTargetPosition;
        private int mRotation;
        private readonly Texture2D mMilSheet;

        private bool mSelected;

        private float mMouseX;

        private float mMouseY;

        private readonly float mScale;

        private Map.Map mMap;

        private Stack<Vector2> mPath;

        private Director mDirector;

        private MilitaryPathfinder mPathfinder;

        private Vector2[] mDebugPath; //TODO this is for debugging

        private Vector2 mEnemyPosition;

        private bool mShoot;

        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        public Vector2 Center { get; private set; }

        public bool Moved { get; private set; }

        public int RevelationRadius { get; private set; }

        public Rectangle Bounds { get; private set; }

        public Rectangle AbsBounds { get; private set; }

        
        public MilitaryUnit(Vector2 position, Texture2D spriteSheet, Camera camera, ref Director director, ref Map.Map map)
        {
            Id = IdGenerator.NextiD(); // id for the specific unit.
            Health = 10;

            mScale = 0.4f;

            AbsolutePosition = position;
            AbsoluteSize = new Vector2(x: DefaultWidth * mScale, y: DefaultHeight * mScale);

            RevelationRadius = 500;
            Center = new Vector2(x: AbsolutePosition.X + AbsoluteSize.X * mScale / 2, y: AbsolutePosition.Y + AbsoluteSize.Y * mScale / 2);

            Moved = false;
            mIsMoving = false;
            mCamera = camera;

            mDirector = director;

            mDirector.GetInputManager.AddMouseClickListener(iMouseClickListener: this, leftClickType: EClickType.Both, rightClickType: EClickType.Both);
            mDirector.GetInputManager.AddMousePositionListener(iMouseListener: this);

            mMilSheet = spriteSheet;

            mMap = map;

            mPathfinder = new MilitaryPathfinder();
        }


        /// <summary>
        /// Rotates unit in order when selected in order to face
        /// user mouse and eventually target destination.
        /// </summary>
        /// <param name="target"></param>
        private void Rotate(Vector2 target)
        {
            // form a triangle from unit location to mouse location
            // adjust to be at center of sprite
            var x = target.X - (RelativePosition.X + RelativeSize.X / 2);
            var y = target.Y - (RelativePosition.Y + RelativeSize.Y / 2);
            var hypot = Math.Sqrt(d: Math.Pow(x: x, y: 2) + Math.Pow(x: y, y: 2));

            // calculate degree between formed triangle
            double degree;
            if (Math.Abs(value: hypot) < 0.01)
            {
                degree = 0;
            }
            else
            {
                degree = Math.Asin(d: y / hypot) * (180.0 / Math.PI);
            }

            // calculate rotation with increased degrees going counterclockwise
            if (x >= 0)
            {
                mRotation = (int) Math.Round(value: 270 - degree, mode: MidpointRounding.AwayFromZero);
            }
            else
            {
                mRotation = (int) Math.Round(value: 90 + degree, mode: MidpointRounding.AwayFromZero);
            }

            // add 42 degrees since sprite sheet starts at sprite -42deg not 0
            mRotation = (mRotation + 42) % 360;

        }

        /// <summary>
        /// Defines the health of the unit, defaults to 10.
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
                texture: mMilSheet,
                position: AbsolutePosition,
                sourceRectangle: new Rectangle(x: 150 * mColumn, y: 75 * mRow, width: (int) (AbsoluteSize.X / mScale), height: (int) (AbsoluteSize.Y / mScale)),
                color: mColor,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: new Vector2(value: mScale),
                effects: SpriteEffects.None,
                layerDepth: LayerConstants.MilitaryUnitLayer
                );
            
            // TODO DEBUG REGION
            if (mDebugPath != null)
            {
                for (var i = 0; i < mDebugPath.Length - 1; i++)
                {
                    spriteBatch.DrawLine(point1: mDebugPath[i], point2: mDebugPath[i + 1], color: Color.Orange);
                }
            }

            if (mShoot)
            {
                // draws a laser line a a slight glow around the line, then sets the shoot future off 
                spriteBatch.DrawLine(point1: Center, point2: MapCoordinates(v: mEnemyPosition), color: Color.White, thickness: 2);
                spriteBatch.DrawLine(point1: new Vector2(x: Center.X - 2, y: Center.Y), point2: MapCoordinates(v: mEnemyPosition), color: Color.White * .2f, thickness: 6);
                mShoot = false;
            }
        }


        public void Update(GameTime gameTime)
        {

            //make sure to update the relative bounds rectangle enclosing this unit.
            Bounds = new Rectangle(
                x: (int)RelativePosition.X, y: (int)RelativePosition.Y, width: (int)RelativeSize.X, height: (int)RelativeSize.Y);

            // this makes the unit rotate according to the mouse position when its selected and not moving.
            if (mSelected && !mIsMoving && !mShoot)
            {
                Rotate(target: new Vector2(x: mMouseX, y: mMouseY));
            }

            else if (mShoot)
            {
                Rotate(target: mEnemyPosition);
            }

            if (HasReachedTarget())
            {
                mIsMoving = false;
            }

            // calculate path to target position
            else if (mIsMoving)
            {
                if (!HasReachedWaypoint())
                {
                    MoveToTarget(target: mPath.Peek());
                    

                }
                else
                {
                    mPath.Pop();
                    MoveToTarget(target: mPath.Peek());
                }
            }

            // these are values needed to properly get the current sprite out of the spritesheet.
            mRow = mRotation / 18;
            mColumn = (mRotation - mRow * 18) / 3;

            //finally select the appropriate color for selected/deselected units.
            mColor = mSelected ? sSelectedColor : sNotSelectedColor;

            Center = new Vector2(x: AbsolutePosition.X + AbsoluteSize.X * mScale / 2, y: AbsolutePosition.Y + AbsoluteSize.Y * mScale / 2);
            AbsBounds = new Rectangle(x: (int)AbsolutePosition.X + 16, y: (int) AbsolutePosition.Y + 11, width: (int)(AbsoluteSize.X * mScale), height: (int) (AbsoluteSize.Y * mScale));
            Moved = mIsMoving;

            //TODO this needs to be taken out once the military manager takes control of shooting
            if (Keyboard.GetState().IsKeyDown(key: Keys.Space))
            {
                // shoots at mouse and plays laser sound at full volume
                Shoot(target: new Vector2(x: Mouse.GetState().X, y: Mouse.GetState().Y));
                mDirector.GetSoundManager.PlaySound(name: "LaserSound", x: Center.X, y: Center.Y, volume: 1f, pitch: 1f, isGlobal: true, loop: false, soundClass: SoundClass.Effect);

            }
        }

        public void Shoot(Vector2 target)
        {
            mShoot = true;
            mEnemyPosition = target;
            Rotate(target: target);

        }

        /// <summary>
        /// Calculates the direction the unit should be moving and moves it into that direction.
        /// </summary>
        /// <param name="target">The target to which to move</param>
        private void MoveToTarget(Vector2 target)
        {

            var movementVector = new Vector2(x: target.X - Center.X, y: target.Y - Center.Y);
            movementVector.Normalize();
            mToAdd += mMovementVector * (float) (mZoomSnapshot *  Speed);

            AbsolutePosition = new Vector2(x: (float) (AbsolutePosition.X + movementVector.X * Speed), y: (float) (AbsolutePosition.Y + movementVector.Y * Speed));
        }

        /// <summary>
        /// Checks whether the target position is reached or not.
        /// </summary>
        private bool HasReachedTarget()
        {

            if (!(Math.Abs(value: Center.X + mToAdd.X -
                           mTargetPosition.X) < 8 &&
                  Math.Abs(value: Center.Y + mToAdd.Y -
                           mTargetPosition.Y) < 8))
            {
                return false;
            }
            mToAdd = Vector2.Zero;
            return true;
        }

        private bool HasReachedWaypoint()
        {
            if (Math.Abs(value: Center.X + mToAdd.X - mPath.Peek().X) < 8
                && Math.Abs(value: Center.Y + mToAdd.Y - mPath.Peek().Y) < 8)
            {
                Debug.WriteLine(message: "Waypoint reached.");
                Debug.WriteLine(message: "Next waypoint: " +  mPath.Peek());
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
                    if (mSelected && !mIsMoving && !withinBounds && Map.Map.IsOnTop(rect: new Rectangle(x: (int)(mMouseX - RelativeSize.X / 2f), y: (int)(mMouseY - RelativeSize.Y / 2f), width: (int)RelativeSize.X, height: (int)RelativeSize.Y), camera: mCamera))
                    {
                        mIsMoving = true;
                        mTargetPosition = Vector2.Transform(position: new Vector2(x: Mouse.GetState().X, y: Mouse.GetState().Y),
                            matrix: Matrix.Invert(matrix: mCamera.GetTransform()));
                        var currentPosition = Center;
                        Debug.WriteLine(message: "Starting path finding at: " + currentPosition.X +", " + currentPosition.Y);
                        Debug.WriteLine(message: "Target: " + mTargetPosition.X + ", " + mTargetPosition.Y);

                        mPath = new Stack<Vector2>();
                        mPath = mPathfinder.FindPath(startPosition: currentPosition,
                            endPosition: mTargetPosition,
                            map: ref mMap);

                        // TODO: DEBUG REGION
                        mDebugPath = mPath.ToArray();

                        // TODO: END DEBUG REGION

                        mBoundsSnapshot = Bounds;
                        mZoomSnapshot = mCamera.GetZoom();
                        giveThrough = true;
                    }

                    if (withinBounds) {
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

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }

        public void MousePositionChanged(float newX, float newY)
        {
            mMouseX = newX;
            mMouseY = newY;
        }

        /// <summary>
        /// Used to find the coordinates of the given Vector2 based on the overall map
        /// instead of just the camera shot, returns Vector2 of absolute position
        /// </summary>
        /// <returns></returns>
        private Vector2 MapCoordinates(Vector2 v)
        {
            return new Vector2(x: Vector2.Transform(position: new Vector2(x: v.X, y: v.Y),
                matrix: Matrix.Invert(matrix: mCamera.GetTransform())).X, y: Vector2.Transform(position: new Vector2(x: v.X, y: v.Y),
                matrix: Matrix.Invert(matrix: mCamera.GetTransform())).Y);
        }
    }
}
