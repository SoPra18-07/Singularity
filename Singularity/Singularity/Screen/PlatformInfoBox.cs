using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Platforms;

namespace Singularity.Screen
{
    public sealed class PlatformInfoBox : InfoBoxWindow
    {
        public PlatformInfoBox(List<IWindowItem> itemList, Vector2 size, Color borderColor, Color centerColor, Rectangle boundsRectangle, bool boxed, PlatformBlank platform, Director director) : base(itemList, size, borderColor, centerColor, boundsRectangle, boxed, director, false, platform.AbsolutePosition)
        {
        }
    }
}
