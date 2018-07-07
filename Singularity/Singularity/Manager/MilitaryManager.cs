using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Units;

namespace Singularity.Manager
{
    public class MilitaryManager
    {
        private Map.Map mMap;

        internal List<FreeMovingUnit> Units;

        internal void SetMap(ref Map.Map map)
        {
            mMap = map;
        }


        internal static void LoadContents(ContentManager content)
        {
            var milUnitSheet = content.Load<Texture2D>("UnitSpriteSheet");
            var milGlowSheet = content.Load<Texture2D>("UnitGlowSprite");

            MilitaryUnit.mMilSheet = milUnitSheet;
            MilitaryUnit.mGlowTexture = milGlowSheet;
        }


    }
}
