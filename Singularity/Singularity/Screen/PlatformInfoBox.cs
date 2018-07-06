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

        internal PlatformInfoBox(List<IWindowItem> itemList, Vector2 size, PlatformBlank platform, Director director) : base(itemList, size, new Color(0.86f, 0.86f, 0.86f), new Color(1f, 1, 1), new Rectangle(0, 0, 0, 0), true, director, false, platform.AbsolutePosition)
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
