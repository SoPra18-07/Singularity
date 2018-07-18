using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen.ScreenClasses
{
    internal class MenuWindow
    {
        protected readonly Vector2 mScreenResolution;
        protected Vector2 mMenuBoxPosition;
        protected Vector2 mMenuBoxSize;


        private Texture2D mGlowEdge;
        private Texture2D mGlowCorner;

        protected SpriteFont mLibSans36;
        protected SpriteFont mLibSans20;
        protected SpriteFont mLibSans14;
        protected SpriteFont mLibSans12;


        // Transitions
        protected float mWindowOpacity;

        internal MenuWindow(Vector2 screenResolution)
        {
            mScreenResolution = screenResolution;
        }

        public virtual void LoadContent(ContentManager content)
        {
            mGlowEdge = content.Load<Texture2D>("WindowGlowEdge");
            mGlowCorner = content.Load <Texture2D>("WindowGlowCorner");

            mLibSans36 = content.Load<SpriteFont>("LibSans36");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mLibSans14 = content.Load<SpriteFont>("LibSans14");
            mLibSans12 = content.Load<SpriteFont>("LibSans12");
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            /* Draw Glow */
            // Horizontal Scale
            var horizontalScale = new Vector2(mMenuBoxSize.X / 200, 1);
            // Vertical Scale
            var verticalScale = new Vector2(mMenuBoxSize.Y / 200, 1);
            // N
            spriteBatch.Draw(mGlowEdge,
                new Vector2(mMenuBoxPosition.X, mMenuBoxPosition.Y - 32),
                null,
                Color.White * mWindowOpacity,
                0f,
                Vector2.Zero,
                horizontalScale,
                SpriteEffects.None,
                0.5f);
            // S
            spriteBatch.Draw(mGlowEdge,
                new Vector2(mMenuBoxPosition.X, mMenuBoxPosition.Y + mMenuBoxSize.Y),
                null,
                Color.White * mWindowOpacity,
                0f,
                Vector2.Zero,
                horizontalScale,
                SpriteEffects.FlipVertically,
                0.5f);
            // W
            spriteBatch.Draw(mGlowEdge,
                new Vector2(mMenuBoxPosition.X, mMenuBoxPosition.Y),
                null,
                Color.White * mWindowOpacity,
                1.57079632679f,
                Vector2.Zero,
                verticalScale,
                SpriteEffects.FlipVertically,
                0.5f);
            // E
            spriteBatch.Draw(mGlowEdge,
                new Vector2(mMenuBoxPosition.X + mMenuBoxSize.X + 32, mMenuBoxPosition.Y),
                null,
                Color.White * mWindowOpacity,
                1.57079632679f,
                Vector2.Zero,
                verticalScale,
                SpriteEffects.None,
                0.5f);

            // NW
            spriteBatch.Draw(mGlowCorner,
                mMenuBoxPosition - new Vector2(32),
                null,
                Color.White * mWindowOpacity,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.5f);
            // NE
            spriteBatch.Draw(mGlowCorner,
                mMenuBoxPosition + new Vector2(mMenuBoxSize.X, 0) + new Vector2(0, -32),
                null,
                Color.White * mWindowOpacity,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.FlipHorizontally,
                0.5f);
            // SW
            spriteBatch.Draw(mGlowCorner,
                mMenuBoxPosition + new Vector2(-32, mMenuBoxSize.Y),
                null,
                Color.White * mWindowOpacity,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.FlipVertically,
                0.5f);
            // SE
            spriteBatch.Draw(mGlowCorner,
                mMenuBoxPosition + new Vector2(mMenuBoxSize.X + 32, mMenuBoxSize.Y),
                null,
                Color.White * mWindowOpacity,
                1.57079632679f,
                Vector2.Zero,
                1f,
                SpriteEffects.FlipHorizontally,
                0.5f);
        }
    }
}
