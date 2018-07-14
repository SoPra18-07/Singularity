using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Platforms;

namespace Singularity.Screen
{
    public sealed class PlatformInfoBox : InfoBoxWindow
    {
        private readonly PlatformBlank mPlatform;

        internal PlatformInfoBox(List<IWindowItem> itemList, Vector2 size, PlatformBlank platform, Director director) :
            base(itemList: itemList,
                size: size,
                borderColor: new Color(r: 0.86f, g: 0.86f, b: 0.86f),
                centerColor: new Color(r: 1f, g: 1, b: 1),
                boxed: true,
                director: director,
                mousePosition: false,
                location: platform.AbsolutePosition)
        {
            mPlatform = platform;
            Position = platform.AbsolutePosition + new Vector2(0, platform.AbsoluteSize.Y + 10);
            Active = true;
            mCounter = 11;
        }

        internal void UpdateString(string resourceString)
        {
            mItemList[0] = new TextField(resourceString, Position, PlatformBlank.mLibSans12.MeasureString(resourceString), PlatformBlank.mLibSans12, Color.Black);
            mSize = PlatformBlank.mLibSans12.MeasureString(resourceString);
        }
    }
}
