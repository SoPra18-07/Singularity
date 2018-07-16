using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.AI.Structures
{
    /// <summary>
    /// This class is the enemey platform responsible for spawning enemy units in the game
    /// This is essentially the Barrack Platform except that it doesnt require any resources
    /// in order to produce military units
    /// </summary>
    [DataContract]
    public class Spawner : PlatformBlank
    {
        [DataMember]
        private readonly Vector2 mPosition;

        public Spawner(Vector2 position, Texture2D platformSpriteSheet, Texture2D baseSprite, ref Director director, EStructureType type = EStructureType.Barracks, float centerOffsetY = -36, bool friendly = false) : base(position, platformSpriteSheet, baseSprite, mLibSans12, ref director, type, centerOffsetY, friendly)
        {
            mSpritename = "Cylinders";
            mIsBlueprint = false;
        }



        // method that produces enemy military units when called up by the Ki
        internal EnemyUnit SpawnEnemy(Camera camera, Map.Map map, GameScreen gameScreen)
        {
            var enemyUnit = new EnemyUnit(new Vector2(Center.X + 100, Center.Y), camera, ref mDirector, ref map);
            gameScreen.AddObject(enemyUnit);
            return enemyUnit;
        }
    }
}
