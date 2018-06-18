using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Input;
using Singularity.Property;

namespace Singularity.Screen
{
    interface IWindowItem : IUpdate, IDraw
    {
        //TODO implement the methods specific to IWindowItem
        Vector2 Position { get; set; }

        Vector2 Size { get; }
    }
}