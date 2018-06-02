using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;

namespace Singularity.ScreenClasses
{
    interface IWindowItem: IDraw, IUpdate
    {
    
        // TODO void ReceiveEvents(Event e)
    }
}
