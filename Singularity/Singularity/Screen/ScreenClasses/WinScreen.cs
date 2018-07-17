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
    public sealed class WinScreen : IScreen
    {
        public bool Loaded { get; set; }

        public EScreen Screen { get; } = EScreen.WinScreen;

        public void Draw(SpriteBatch spriteBatch)
        {
            //TODO: implement
        }

        public bool DrawLower()
        {
            return true;
        }

        public void LoadContent(ContentManager content)
        {
            //TODO: implement
        }

        public void Update(GameTime gametime)
        {
            //TODO: implement
        }

        public bool UpdateLower()
        {
            return false;
        }
    }
}
