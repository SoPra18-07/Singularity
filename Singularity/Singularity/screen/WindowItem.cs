using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Units
{
    interface WindowItem
    {
        void Draw(SpriteBatch spriteBatch);

        void Update(GameTime gameTime);

        // TODO void ReceiveEvents(Event e)

    }
}
