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
    /// <summary>
    /// All the main menu screens are overlayed on top of this screen.
    /// Since the main menu will have the same animated background, it will
    /// simply use the same background screen and be overlayed on top of it.
    /// </summary>
    class MenuBackgroundScreen : IScreen
    {
        private Texture2D mGlowTexture2D;
        private Texture2D mHoloProjectionTexture2D;

        public void LoadContent(ContentManager content)
        {
            mGlowTexture2D = content.Load<Texture2D>("Glow");
            mHoloProjectionTexture2D = content.Load<Texture2D>("HoloProjection");
        }
        public void Update(GameTime gametime)
        {
            // TODO
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(mGlowTexture2D,
                new Vector2(540, 360),
                null, Color.AliceBlue,
                0f,
                new Vector2(609, 553),
                new Vector2(0.7f),
                SpriteEffects.None,0f);
            
            spriteBatch.End();
        }

        public bool UpdateLower()
        {
            return false;
        }

        public bool DrawLower()
        {
            return false;
        }
    }
}
