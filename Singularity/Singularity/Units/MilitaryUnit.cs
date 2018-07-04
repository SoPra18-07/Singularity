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
        private const int DefaultWidth = 150;
        private const int DefaultHeight = 75;

        private const float Speed = 4;

        private readonly Texture2D mMilSheet;
        private readonly Texture2D mGlowTexture;

        private readonly float mScale;

        private Vector2 mEnemyPosition;

        private bool mShoot;

        public MilitaryUnit(Vector2 position, Texture2D spriteSheet, Texture2D glowSpriteSheet, Camera camera, ref Director director, ref Map.Map map)
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

        public override void Draw(SpriteBatch spriteBatch)
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


        public override void Update(GameTime gameTime)
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
        
    }
}
