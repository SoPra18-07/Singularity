using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.Screen
{
    [DataContract]
    public sealed class HealthBar : IDraw, IUpdate
    {
        [DataMember]
        private Rectangle mBounds;

        [DataMember]
        private readonly ICollider mAttachedTo;

        [DataMember]
        private Rectangle mFilled;

        [DataMember]
        private int mMaxHealth;

        public HealthBar(ICollider die)
        {
            mAttachedTo = die;
            mMaxHealth = die.Health;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!GlobalVariables.mHealthBarEnabled)
            {
                return;
            }

            spriteBatch.StrokedRectangle(new Vector2(mBounds.X, mBounds.Y), new Vector2(mBounds.Width, mBounds.Height), Color.Black, Color.Transparent, 1f, 0f, 0.8f);
            spriteBatch.FillRectangle(mFilled, Color.DarkRed, 0f, 0.79f);
        }

        public void Update(GameTime gametime)
        {
            if (!GlobalVariables.mHealthBarEnabled)
            {
                return;
            }

            if (mMaxHealth <= 0)
            {
                mMaxHealth = mAttachedTo.Health;
            }

            mBounds = new Rectangle(mAttachedTo.AbsBounds.X - 15, mAttachedTo.AbsBounds.Y - 25, mAttachedTo.AbsBounds.Width + 30, 8);

            mFilled = new Rectangle(mBounds.X, mBounds.Y, (int) (mAttachedTo.Health * ((mAttachedTo.AbsBounds.Width + 30) / (float) mMaxHealth)), mBounds.Height);
        }
    }
}
