﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Exceptions
{
    internal sealed class UnsupportedTextureSizeException : Exception
    {
        public UnsupportedTextureSizeException(int actualWidth, int actualHeight, int wantedWidth, int wantedHeight) :
            base("The dimensions of the given Texture2D were: (" + actualWidth + ", " + actualHeight +
                 ") but need to be: (" + wantedWidth + ", " + wantedHeight + ").") { }
    }
}