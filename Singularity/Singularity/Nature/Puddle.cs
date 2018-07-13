﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Nature
{
    public class Puddle : ICollider
    {
        public bool[,] ColliderGrid { get; internal set; }


        public Rectangle AbsBounds { get; }
        public bool Moved { get; } = false;
        public int Id { get; }
        public Vector2 RelativePosition { get; set; }
        public Vector2 RelativeSize { get; set; }
        public Vector2 AbsolutePosition { get; set; }
        public Vector2 AbsoluteSize { get; set; }
        private readonly Vector2 mPosition;
        private readonly bool mBigPuddle;

        public Vector2 Center { get; }
        public bool Friendly { get; } = false;

        public Puddle(Vector2 position, bool bigPuddle = true)
        {
            Id = IdGenerator.NextiD();
            AbsoluteSize = new Vector2(160, 130);

            mBigPuddle = bigPuddle;

            Center = new Vector2((AbsolutePosition.X + AbsoluteSize.X) * 0.5f, (AbsolutePosition.Y + AbsoluteSize.Y) * 0.5f);

            // this is used to draw the puddle correctly within the collision grid
            mPosition = position;
            AbsolutePosition = new Vector2(position.X, position.Y - 50);
            AbsBounds = new Rectangle((int)position.X, (int)position.Y, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);

            if (bigPuddle)
            {
                ColliderGrid = new[,]
                {
                    {false, true, true, true, true, true, true, true, false, false},
                    {true, true, true, true, true, true, true, true, true, false},
                    {true, true, true, true, true, true, true, true, true, true},
                    {true, true, true, true, true, true, true, true, true, true},
                    {true, true, true, true, true, true, true, true, true, true},
                    {false, true, true, true, true, true, true, true, false, false},
                };
            }
            else
            {
                ColliderGrid = new[,]
                {
                    {false, true, true, true, true, true, true, true, false},
                    {true, true, true, true, true, true, true,  true, true},
                    {true, true, true, true, true, true, true, true, true},
                    {true, true, true, true, true, true, true, true, true},
                    {true, true, true, true, true, true, true, true, true},
                    {false, true, true, true, true, true, true, true, false},
                };
            }
        }



        public void Draw(SpriteBatch spriteBatch)
        {
            // by default draws big puddle
            if (mBigPuddle)
            {

                // draw interior of puddle
                spriteBatch.DrawLine(new Vector2(mPosition.X + 90, mPosition.Y - 43), 65, 1.58f, Color.Gray);
                spriteBatch.DrawLine(new Vector2(mPosition.X + 89, mPosition.Y + 21), 47, .46f, Color.Gray);
                spriteBatch.DrawLine(new Vector2(mPosition.X + 89, mPosition.Y + 22), 26, 2.68f, Color.Gray);


                // draw the water in the puddle
                for (int i = 0; i < 45; i++)
                {
                    spriteBatch.DrawLine(
                        new Vector2(mPosition.X + 91 + (i * 1.959f),
                            mPosition.Y - 25 + i), 79.5f, 2.69f, Color.CornflowerBlue * .7f);
                }

                // draw perimeter of puddle
                spriteBatch.DrawLine(mPosition, 100, -.46f, Color.Gray);
                spriteBatch.DrawLine(mPosition, 120, .46f, Color.Gray);
                spriteBatch.DrawLine(new Vector2(mPosition.X + 107, mPosition.Y + 53), 100, -.46f, Color.Gray);
                spriteBatch.DrawLine(new Vector2(mPosition.X + 89, mPosition.Y - 44), 120, .46f, Color.Gray);
            }

            // draws smaller puddle if specified in constructor
            else
            {
                spriteBatch.DrawLine(mPosition, 80, -.46f, Color.Gray);
                spriteBatch.DrawLine(mPosition, 100, .46f, Color.Gray);
                spriteBatch.DrawLine(new Vector2(mPosition.X + 89, mPosition.Y + 44), 80, -.46f, Color.Gray);
                spriteBatch.DrawLine(new Vector2(mPosition.X + 72, mPosition.Y - 35), 100, .45f, Color.Gray);
                spriteBatch.DrawLine(new Vector2(mPosition.X + 72, mPosition.Y - 35), 51, 1.58f, Color.Gray);
                spriteBatch.DrawLine(new Vector2(mPosition.X + 71, mPosition.Y + 15), 43, .45f, Color.Gray);
                spriteBatch.DrawLine(new Vector2(mPosition.X + 71, mPosition.Y + 16), 23, 2.7f, Color.Gray);


                // draw the water in the puddle
                for (int i = 0; i < 38; i++)
                {
                    spriteBatch.DrawLine(new Vector2(mPosition.X + 71 + (i * 1.959f), mPosition.Y - 20 + i), 63, 2.7f, Color.CornflowerBlue * .7f);
                }

            }
        }

        public void Update(GameTime gametime)
        {
            // do nothing
        }

        public bool Die()
        {
            throw new NotImplementedException();
        }

        public int Health { get; }

        public void MakeDamage(int damage)
        {
            // does not obtain damage
        }
    }
}
