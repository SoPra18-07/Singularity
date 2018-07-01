using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace Singularity.Levels
{
    interface ILevel
    {
        /// <summary>
        /// This method Loads all the data in the Level.
        /// </summary>
        void LoadContent(ContentManager content);
    }
}
