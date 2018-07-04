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
using Singularity.Map.Properties;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Sound;
using Singularity.Utils;

namespace Singularity.Units
{
    internal sealed class MilitaryUnit : ControllableUnit, IMouseClickListener, IMousePositionListener
    {
        public EScreen Screen { get; private set; } = EScreen.GameScreen;

        private const int DefaultWidth = 150;
        private const int DefaultHeight = 75;

        private const double Speed = 4;

        private int mColumn;
        private int mRow;




        private Vector2 mMovementVector;


        private int mRotation;
        private readonly Texture2D mMilSheet;
        private readonly Texture2D mGlowTexture;

        private bool mSelected;

        private float mMouseX;

        private float mMouseY;

        private readonly float mScale;

        private Vector2 mEnemyPosition;

        private bool mShoot;



        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }






        public MilitaryUnit(Vector2 position, Texture2D spriteSheet, Camera camera, ref Director director, ref Map.Map map)
            : base(position, camera, ref director, ref map)
        {
            Health = 10;

            mScale = 0.4f;

            AbsoluteSize = new Vector2(DefaultWidth * mScale, DefaultHeight * mScale);

            RevelationRadius = 400;

            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X * mScale / 2, AbsolutePosition.Y + AbsoluteSize.Y * mScale / 2);

            Moved = false;
            mIsMoving = false;
            mDirector = director;

            mDirector.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
            mDirector.GetInputManager.AddMousePositionListener(this);

            mMilSheet = spriteSheet;
            mGlowTexture = glowSpriteSheet;
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
                mRotation = (int) Math.Round(270 - degree, MidpointRounding.AwayFromZero);
            }
            else
            {
                mRotation = (int) Math.Round(90 + degree, MidpointRounding.AwayFromZero);
            }

            // add 42 degrees since sprite sheet starts at sprite -42deg not 0
            mRotation = (mRotation + 42) % 360;

        }

        /// <summary>
        /// Defines the health of the unit, defaults to 10.
        /// </summary>
        private int Health { get; set; }


        /// <summary>
        /// Damages the unit by a certain amount.
        /// </summary>
        /// <param name="damage"></param>
        public void MakeDamage(int damage)
        {
             Health -= damage;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw military unit
            spriteBatch.Draw(
                mMilSheet,
                AbsolutePosition,
                new Rectangle(150 * mColumn, 75 * mRow, (int) (AbsoluteSize.X / mScale), (int) (AbsoluteSize.Y / mScale)),
                mSelected ? Color.DarkGray : Color.Gray,
                0f,
                Vector2.Zero,
                new Vector2(mScale),
                SpriteEffects.None,
                LayerConstants.MilitaryUnitLayer
                );

            // Draw the glow under it
            if (mSelected)
            {
                spriteBatch.Draw(
                    mGlowTexture,
                    Vector2.Add(AbsolutePosition, new Vector2(-4.5f, -4.5f)),
                    new Rectangle(172 * mColumn, 100 * mRow, 172, 100),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(mScale),
                    SpriteEffects.None,
                    LayerConstants.MilitaryUnitLayer - 0.1f);
            }

            if (GlobalVariables.DebugState)
            {
                // TODO DEBUG REGION
                if (mDebugPath != null)
                {
                    for (var i = 0; i < mDebugPath.Length - 1; i++)
                    {
                        spriteBatch.DrawLine(mDebugPath[i], mDebugPath[i + 1], Color.Orange);
                    }
                }
            }

            if (mShoot)
            {
                // draws a laser line a a slight glow around the line, then sets the shoot future off
                spriteBatch.DrawLine(Center, MapCoordinates(mEnemyPosition), Color.White, 2);
                spriteBatch.DrawLine(new Vector2(Center.X - 2, Center.Y), MapCoordinates(mEnemyPosition), Color.White * .2f, 6);
                mShoot = false;
            }
        }


        public void Update(GameTime gameTime)
        {

            //make sure to update the relative bounds rectangle enclosing this unit.
            Bounds = new Rectangle(
                (int)RelativePosition.X, (int)RelativePosition.Y, (int)RelativeSize.X, (int)RelativeSize.Y);

            // this makes the unit rotate according to the mouse position when its selected and not moving.
            if (mSelected && !mIsMoving && !mShoot)
            {
                Rotate(new Vector2(mMouseX, mMouseY));
            }

            else if (mShoot)
            {
                Rotate(mEnemyPosition);
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
                    MoveToTarget(mPath.Peek(), Speed);
                }
                else
                {
                    mPath.Pop();
                    MoveToTarget(mPath.Peek(), Speed);
                }
            }

            // these are values needed to properly get the current sprite out of the spritesheet.
            mRow = mRotation / 18;
            mColumn = (mRotation - mRow * 18) / 3;

            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X * mScale / 2, AbsolutePosition.Y + AbsoluteSize.Y * mScale / 2);
            AbsBounds = new Rectangle((int)AbsolutePosition.X + 16, (int) AbsolutePosition.Y + 11, (int)(AbsoluteSize.X * mScale), (int) (AbsoluteSize.Y * mScale));
            Moved = mIsMoving;

            //TODO this needs to be taken out once the military manager takes control of shooting
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                // shoots at mouse and plays laser sound at full volume
                Shoot(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                mDirector.GetSoundManager.PlaySound("LaserSound", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);

            }
        }

        public void Shoot(Vector2 target)
        {
            mShoot = true;
            mEnemyPosition = target;
            Rotate(target);

        }



        /// <summary>
        /// Checks whether the target position is reached or not.
        /// </summary>
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
                Debug.WriteLine("Next waypoint: " +  mPath.Peek());
                return true;
            }

            return false;
        }

        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            // todo: someone look at the ReSharper warning following here:
            var giveThrough = true;

            switch (mouseAction)
            {
                case EMouseAction.LeftClick:
                    // check for if the unit is selected, not moving, the click is not within the bounds of the unit, and the click was on the map.
                    if (mSelected
                        && !mIsMoving
                        && !withinBounds
                        && Map.Map.IsOnTop(new Rectangle((int) (mMouseX - RelativeSize.X / 2f),
                                (int) (mMouseY - RelativeSize.Y / 2f),
                                (int) RelativeSize.X,
                                (int) RelativeSize.Y),
                            mCamera))
                    {
                        if (mMap.GetCollisionMap().GetWalkabilityGrid().IsWalkableAt(
                            (int) mTargetPosition.X / MapConstants.GridWidth,
                            (int) mTargetPosition.Y / MapConstants.GridWidth))
                        {
                            mTargetPosition = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                                Matrix.Invert(mCamera.GetTransform()));

                            FindPath(mTargetPosition, Center);
                        }
                    }


                    if (withinBounds) {
                        mSelected = true;
                        giveThrough = false;
                    }

                    break;

                case EMouseAction.RightClick:
                    mSelected = false;
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

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            mMouseX = screenX;
            mMouseY = screenY;
        }

        /// <summary>
        /// Used to find the coordinates of the given Vector2 based on the overall map
        /// instead of just the camera shot, returns Vector2 of absolute position
        /// </summary>
        /// <returns></returns>
        private Vector2 MapCoordinates(Vector2 v)
        {
            return new Vector2(Vector2.Transform(new Vector2(v.X, v.Y),
                Matrix.Invert(mCamera.GetTransform())).X, Vector2.Transform(new Vector2(v.X, v.Y),
                Matrix.Invert(mCamera.GetTransform())).Y);
        }

        /// <summary>
        /// This is called up every time a selection box is created
        /// if MUnit bounds intersects with the selection box then it become selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="position"> top left corner of the selection box</param>
        /// <param name="size"> size of selection box</param>
        public void BoxSelected(object sender, EventArgs e, Vector2 position, Vector2 size)
        {
            // create a rectangle from given parameters
            Rectangle selBox = new Rectangle((int) position.X, (int) position.Y, (int) size.X, (int) size.Y);

            // check if selection box intersects with MUnit bounds
            if (selBox.Intersects(AbsBounds))
            {
                mSelected = true;
            }
        }

        public bool Die()
        {
            // mDirector.GetMilitaryManager.Kill(this);
            // todo: MilitaryManager implement

            return true;
        }
    }
}
