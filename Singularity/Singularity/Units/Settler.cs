using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Utils;

namespace Singularity.Units
{
    class Settler: ICollider, IRevealing, IMouseClickListener, IMousePositionListener
    {
        public Vector2 RelativePosition { get; set; }
        public Vector2 RelativeSize { get; set; }
        public Vector2 AbsolutePosition { get; set; }
        public Vector2 AbsoluteSize { get; set; }
        public Rectangle AbsBounds { get; private set; }
        public bool Moved { get; private set; }
        public int Id { get; }
        public int RevelationRadius { get; }
        public Vector2 Center { get; private set; }
        public EScreen Screen { get; }
        public Rectangle Bounds { get; private set; }


        private readonly Camera mCamera;
        private Director mDirector;
        private Map.Map mMap;
        private MilitaryPathfinder mPathfinder;
        private static readonly Color sSelectedColor = Color.Bisque;
        private static readonly Color sNotSelectedColor = Color.Beige;
        private bool mSelected;
        private const double Speed = 4;
        private bool mIsMoving;
        private Rectangle mBoundsSnapshot;
        private Vector2 mToAdd;
        private double mZoomSnapshot;
        private Vector2 mMovementVector;
        private Vector2 mTargetPosition;
        private float mMouseX;
        private float mMouseY;
        private Stack<Vector2> mPath;
        private Vector2[] mDebugPath; //TODO this is for debugging



        // constructor for settler (position)
        public Settler(Vector2 position, Camera camera, ref Director director, ref Map.Map map)
        {
            Id = IdGenerator.NextiD(); // id for the specific unit.

            AbsolutePosition = position;
            AbsoluteSize = new Vector2(20, 20);

            RevelationRadius = (int)AbsoluteSize.X * 3;
            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);

            Moved = false;
            mCamera = camera;

            mDirector = director;

            mDirector.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
            Debug.WriteLine("Added to mouse click listener");
            mDirector.GetInputManager.AddMousePositionListener(this);

            mMap = map;

            mPathfinder = new MilitaryPathfinder();
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.StrokedRectangle(AbsolutePosition, AbsoluteSize, Color.Gray, (mSelected)? sSelectedColor : sNotSelectedColor, .8f, 1f);
            
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
            Bounds = new Rectangle((int)RelativePosition.X, (int)RelativePosition.Y, (int)RelativeSize.X, (int)RelativeSize.Y);

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
            AbsBounds = new Rectangle((int)AbsolutePosition.X + 16, (int)AbsolutePosition.Y + 11, (int)(AbsoluteSize.X), (int)(AbsoluteSize.Y));
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
            Debug.WriteLine("Clicked");
            var giveThrough = true;

            switch (mouseAction)
            {
                case EMouseAction.LeftClick:
                    if (mSelected && !mIsMoving && !withinBounds && Map.Map.IsOnTop(new Rectangle((int)(mMouseX - RelativeSize.X / 2f), (int)(mMouseY - RelativeSize.Y / 2f), (int)RelativeSize.X, (int)RelativeSize.Y), mCamera))
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
    }
}
