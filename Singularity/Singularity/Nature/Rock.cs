using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Property;

namespace Singularity.Nature
{
    [DataContract]
    public class Rock : ADie, ICollider
    {
        public bool[,] ColliderGrid { get; internal set; }

        // specifies the angle of the given rock
        private float[,] mDrawAngle;

        // specifies whethere a rock should be drawn at that location
        private bool[,] mDrawRock;

        [DataMember]
        private readonly Vector2 mDrawSize;

        // used to first generate the rock drawing matrix
        [DataMember]
        private bool mNotGenerated;
        [DataMember]
        public Rectangle AbsBounds { get; private set; }
        [DataMember]
        public bool Moved { get; private set; }
        [DataMember]
        public int Id { get; private set; }
        [DataMember]
        public Vector2 RelativePosition { get; set; }
        [DataMember]
        public Vector2 RelativeSize { get; set; }
        [DataMember]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember]
        public Vector2 AbsoluteSize { get; set; }
        [DataMember]
        public static Random mRandom = new Random();
        [DataMember]
        public bool Friendly { get; private set; }
        [DataMember]
        public Vector2 Center { get; private set; }

        public Rock(Vector2 position, ref Director director) : base(ref director)
        {
            Id = director.GetIdGenerator.NextiD();
            AbsoluteSize = new Vector2(160, 130);
            mDrawSize = new Vector2(80,40);
            AbsolutePosition = position;
            Center = new Vector2((AbsolutePosition.X + AbsoluteSize.X) * 0.5f, (AbsolutePosition.Y + AbsoluteSize.Y) * 0.5f);
            AbsBounds = new Rectangle((int) position.X, (int) position.Y, (int) AbsoluteSize.X, (int) AbsoluteSize.Y );
            ColliderGrid = new [,]
            {
                {false, false, true, true, true, true, false, false},
                {false, true, true, true, true, true, true, false},
                {true, true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true, true},
                {false, true, true, true, true, true, true, false},
                {false, false, true, true, true, true, false, false},
            };

            mDrawAngle = new float [14, 18];

            mDrawRock = new bool[14, 18];
        }

        public new void ReloadContent(ref Director dir)
        {
            base.ReloadContent(ref dir);
            ColliderGrid = new[,]
            {
                {false, false, true, true, true, true, false, false},
                {false, true, true, true, true, true, true, false},
                {true, true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true, true},
                {false, true, true, true, true, true, true, false},
                {false, false, true, true, true, true, false, false},
            };

            mDrawAngle = new float[14, 18];

            mDrawRock = new bool[14, 18];

            // create the rotation and placement chart for rocks
            for (var i = 0; i < 14; i++)
            {
                for (var j = 0; j < 18; j++)
                {
                    // this give the rotation of the rock
                    mDrawAngle[i, j] = mRandom.Next(1, 101);

                    // this gives whether a rock will be placed at that position
                    mDrawRock[i, j] = (mRandom.Next(1, 60) % 2 == 0);
                }
            }


            // make the rock formation into a diamond shape to fit with isometric view
            for (var i = 0; i < 18; i++)
            {
                if (i != 8 && i != 9)
                {
                    mDrawRock[0, i] = false;
                    mDrawRock[13, i] = false;
                    if (i != 7 && i != 10)
                    {
                        mDrawRock[1, i] = false;
                        mDrawRock[12, i] = false;

                        if (i != 6 && i != 11)
                        {
                            mDrawRock[2, i] = false;
                            mDrawRock[11, i] = false;

                            if (i != 5 && i != 12)
                            {
                                mDrawRock[3, i] = false;
                                mDrawRock[10, i] = false;

                                if (i != 4 && i != 13)
                                {
                                    mDrawRock[4, i] = false;
                                    mDrawRock[9, i] = false;

                                    if (i != 3 && i != 14)
                                    {
                                        mDrawRock[5, i] = false;
                                        mDrawRock[8, i] = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the Rotation Matrix and the Placement Matrix for the rock formation
        /// </summary>
        private void CreateRock()
        {


            // create the rotation and placement chart for rocks
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 18; j++)
                {
                    // this give the rotation of the rock
                    mDrawAngle[i,j] = mRandom.Next(1,101);

                    // this gives whether a rock will be placed at that position
                    mDrawRock[i, j] = (mRandom.Next(1, 60) % 2 == 0);
                }
            }


            // make the rock formation into a diamond shape to fit with isometric view
            for (int i = 0; i < 18; i++)
            {
                if (i != 8 && i != 9)
                {
                    mDrawRock[0, i] = false;
                    mDrawRock[13, i] = false;
                    if (i != 7 && i != 10)
                    {
                        mDrawRock[1, i] = false;
                        mDrawRock[12, i] = false;

                        if (i != 6 && i != 11)
                        {
                            mDrawRock[2, i] = false;
                            mDrawRock[11, i] = false;

                            if (i != 5 && i != 12)
                            {
                                mDrawRock[3, i] = false;
                                mDrawRock[10, i] = false;

                                if (i != 4 && i != 13)
                                {
                                    mDrawRock[4, i] = false;
                                    mDrawRock[9, i] = false;

                                    if (i != 3 && i != 14)
                                    {
                                        mDrawRock[5, i] = false;
                                        mDrawRock[8, i] = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
            {
                // generate the rotation and placement matrix at the beginning
                if (!mNotGenerated)
                {
                    CreateRock();
                    mNotGenerated = true;
                }

                // draw the rock formation

                for (int i = 0; i < 18; i++)
                {
                    for (int j = 0; j< 14; j++) {

                        // if a rock should be placed at this position
                        if (mDrawRock[j, i])
                        {
                            // draw a circle rock if rotation divisible by 3
                            if (mDrawAngle[j,i] % 3 == 0)
                            {

                                spriteBatch.FillCircle(
                                    new Vector2(
                                        ((AbsolutePosition.X + 20 + (mDrawSize.X / 9 * (j)) - 5) +
                                         mDrawSize.X / 16),
                                        ((AbsolutePosition.Y + 20 + (mDrawSize.Y / 7 * (i)) - 5) +
                                         mDrawSize.X / 16)),
                                    mDrawSize.X / 18,
                                    20,
                                    (mDrawAngle[j, i] % 2 == 0) ? Color.DimGray * .9f : Color.Gray * .9f,
                                    .45f);
                            }

                            // otherwise draw a square
                            else
                            {
                                spriteBatch.FillRectangle(
                                    new Rectangle((int)(AbsolutePosition.X + 20 + (mDrawSize.X / 9 * (j)) - 5),
                                        (int)(AbsolutePosition.Y + 20 + (mDrawSize.Y / 7 * (i)) - 5),
                                        (int)(mDrawSize.X / 8),
                                        (int)(mDrawSize.X / 8)),
                                    (mDrawAngle[j, i] % 2 == 0) ? Color.DimGray * .8f : Color.Gray * .8f,
                                    mDrawAngle[j, i] * .01f,
                                    .45f);
                            }
                        }
                    }
                }
            }

        public void Update(GameTime gametime)
        {
            // do nothing
        }

        public override bool Die()
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
