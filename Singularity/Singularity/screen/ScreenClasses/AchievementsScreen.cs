﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen.ScreenClasses
{
    /// <summary>
    /// Used to show achievements that the player has earned.
    /// It shows achievements already earned in a list format, not
    /// unsimilar to how steam shows achievements. It has no buttons
    /// other than back and simply shows textures of the achievements
    /// and their description. If an achievement has not been earned yet,
    /// the achievement texture is blacked out.
    /// </summary>
    class AchievementsScreen : IScreen
    {
        public void Update(GameTime gametime)
        {
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public bool UpdateLower()
        {
            throw new NotImplementedException();
        }

        public bool DrawLower()
        {
            throw new NotImplementedException();
        }
    }
}
