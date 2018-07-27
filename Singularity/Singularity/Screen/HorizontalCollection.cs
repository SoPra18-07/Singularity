using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen
{
    /// <summary>
    /// HorizontalCollection enables the creation of a new IWindowItem composed of other IWindowItems.
    /// The IWindowItems get placed beside each other horizontally
    /// </summary>
    sealed class HorizontalCollection : IWindowItem
    {
        #region member variables

        // list holding the collection of IWindowItems
        public readonly List<IWindowItem> mItemList;

        // padding between the IWindowItems in collection s.t. the items fit the size (if possible) while maximizing the padding between them
        private readonly float mPadding;

        // backup to reset if the horizontalCollection was inactive (->update)
        private readonly Vector2 mSizeBackup;

        #endregion

        /// <summary>
        /// Creates a new IWindowItem with a collection of IWindowItems placed beside one another horizontally
        /// </summary>
        /// <param name="itemList">items </param>
        /// <param name="size">size of the created IWindowItem</param>
        /// <param name="position">top left corner of created IWindowItem</param>
        public HorizontalCollection(List<IWindowItem> itemList, Vector2 size, Vector2 position)
        {
            // set starting values + member variables
            mItemList = itemList;
            Size = size;
            mSizeBackup = size;
            Position = position;
            ActiveInWindow = true;
            ActiveHorizontalCollection = true;
            mPadding = CalcPadding(itemList, size);

            if (mPadding < 0)
                // catch items too big for size ! shouldn't happen, because right now it's NOT automatically fixed !
            {
                mPadding = 0;
            }
        }

        /// <inheritdoc />
        public void Update(GameTime gametime)
        {
            // if the horizontalCollection is deactivated shrink size to -10, else use backup Size,
            // so that the windowObject treats this horizontalCollection as if it's not existent
            // (-10 due to the objectPadding used in the UI)
            Size = !ActiveHorizontalCollection ? new Vector2(0, -10) : mSizeBackup;

            // activate items in itemList if window + list are active
            if (ActiveInWindow && ActiveHorizontalCollection && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle && !WindowIsInactive)
            {
                // shift from the left border to place all items
                float shift = 0;

                // activate items, set position, update shift
                foreach (var item in mItemList)
                {
                    item.ActiveInWindow = true; // activate all objects if the window is active in case they got deactivated
                    item.Update(gametime);
                    item.Position = new Vector2(Position.X + shift, Position.Y);
                    shift = shift + item.Size.X * 0.25f + mPadding;
                }
            }
            else
            {
                // since the window or the list are inactive -> deactivate all objects in list
                foreach (var item in mItemList)
                {
                    item.ActiveInWindow = false;
                }
            }
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!ActiveInWindow || !ActiveHorizontalCollection || InactiveInSelectedPlatformWindow || OutOfScissorRectangle || WindowIsInactive) { return; }

            foreach (var item in mItemList)
            {
                item.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Calculate equal padding between objects given a size
        /// </summary>
        /// <param name="itemList">list of objects</param>
        /// <param name="size">size to fit the objects</param>
        /// <returns></returns>
        private float CalcPadding(IReadOnlyCollection<IWindowItem> itemList, Vector2 size)
        {
            float width = mItemList.Aggregate<IWindowItem, float>(0, (current, item) => current + item.Size.X * 0.25f);

            return (size.X - width - 20) / (itemList.Count - 1);
        }

        /// <inheritdoc />
        public Vector2 Position { get; set; }
        /// <inheritdoc />
        public Vector2 Size { get; private set; }
        /// <inheritdoc />
        public bool ActiveInWindow { get; set; }
        /// <inheritdoc />
        public bool InactiveInSelectedPlatformWindow { get; set; }
        /// <inheritdoc />
        public bool OutOfScissorRectangle { get; set; }
        /// <inheritdoc />
        public bool WindowIsInactive { get; set; }

        /// <summary>
        /// true if this horizontalCollection should active, else false - used to shrink size if deactivated
        /// </summary>
        public bool ActiveHorizontalCollection { private get; set; }
    }
}
