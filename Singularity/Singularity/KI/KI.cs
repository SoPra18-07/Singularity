using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Map;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.KI
{
    class Ki
    {
        // Camera Property (for Spawner)
        internal Camera Camera { get; private set; }

        
        // Map property (for Spawner)
        internal Map.Map Map { get; private set; }

        // GameScreen property (for Spawner)
        internal GameScreen GameScreen { get; private set; }

        // Keeps track of all the military Units present 
        internal List<EnemyUnit> mEnemyUnits;

        // state machine



    }
}
