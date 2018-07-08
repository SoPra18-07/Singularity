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
        private readonly PlatformBlank mPlatform;

        internal PlatformInfoBox(List<IWindowItem> itemList, Vector2 size, PlatformBlank platform, Director director) : base(itemList: itemList, size: size, borderColor: new Color(r: 0.86f, g: 0.86f, b: 0.86f), centerColor: new Color(r: 1f, g: 1, b: 1), boundsRectangle: new Rectangle(x: 0, y: 0, width: 0, height: 0), boxed: true, director: director, mousePosition: false, location: platform.AbsolutePosition)
        {
            mPlatform = platform;
            Position = platform.AbsolutePosition + new Vector2(0, platform.AbsoluteSize.Y + 10);
        }

        internal void UpdateString(string resourceString)
        {
            mItemList[0] = new TextField(resourceString, Position, mPlatform.mLibSans12.MeasureString(resourceString), mPlatform.mLibSans12);
            mSize = mPlatform.mLibSans12.MeasureString(resourceString);
        }
    }
}
