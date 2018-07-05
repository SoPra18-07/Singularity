﻿
using Microsoft.Xna.Framework;

namespace Singularity.Property
{
    /// <summary>
    /// Represents any object that can shoot
    /// </summary>
    internal interface IShooting
    {
        void Shoot(Vector2 target);
    }
}