using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Utils;

namespace Singularity.Screen
{
    internal sealed class ActivationIWindowItem : IWindowItem
    {
        private readonly IWindowItem mActiveItem;
        private readonly IWindowItem mDeactiveItem;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activeItem"></param>
        /// <param name="deactiveItem"></param>
        /// <param name="width"></param>
        /// <param name="director"></param>
        public ActivationIWindowItem(IWindowItem activeItem, IWindowItem deactiveItem, float width, ref Director director)
        {
            mActiveItem = activeItem;
            mDeactiveItem = deactiveItem;

            if (deactiveItem != null)
            {
                mDeactiveItem = deactiveItem;
            }

            Size = new Vector2(width, activeItem.Size.Y);

            Position = Vector2.Zero;

            ActiveInWindow = true;

        }
        public void Update(GameTime gametime)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle)
            {
                mActiveItem.Position = new Vector2(Position.X + Size.X - mActiveItem.Size.X - 50, Position.Y);
                mDeactiveItem.Position = new Vector2(Position.X + Size.X - mDeactiveItem.Size.X - 50, Position.Y);
                mActiveItem.Update(gametime);
                mDeactiveItem.Update(gametime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle)
            {
                mActiveItem.Draw(spriteBatch);
                mDeactiveItem.Draw(spriteBatch);
            }
        }

        public Vector2 Position { get; set; }
        public Vector2 Size { get; }
        public bool ActiveInWindow { get; set; }
        public bool InactiveInSelectedPlatformWindow { get; set; }
        public bool OutOfScissorRectangle { get; set; }
    }
}
