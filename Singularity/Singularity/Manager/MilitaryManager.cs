using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;
using Singularity.Units;

namespace Singularity.Manager
{
    public class MilitaryManager : IUpdate
    {
        private Map.Map mMap;

        internal List<MilitaryUnit> Units;
        

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

        internal void AddUnit(Vector2 position)
        {

        }


        public void Update(GameTime gametime)
        {
            
        }
    }
}
