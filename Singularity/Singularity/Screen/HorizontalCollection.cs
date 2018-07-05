using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen
{
    /// <summary>
    /// HorizontalCollection enables the creation of a new IWindowItem composed of other IWindowItems.
    /// The IWindowItems get placed beside each other horizontally
    /// </summary>
    class HorizontalCollection : IWindowItem
    {
        // list holding the collection of IWindowItems
        private readonly List<IWindowItem> mItemList;

        // padding between the IWindowItems in collection s.t. the items fit the size (if possible) while maximizing the padding between them
        private float mPadding;

        // backup to reset if the horizontalCollection was inactive (->update)
        private Vector2 mSizeBackup;

        /// <summary>
        /// Creates a new IWindowItem with a collection of IWindowItems placed beside one another horizontally
        /// </summary>
        /// <param name="itemList">items </param>
        /// <param name="size">size of the created IWindowItem</param>
        /// <param name="position">top left corner of created IWindowItem</param>
        public HorizontalCollection(List<IWindowItem> itemList, Vector2 size, Vector2 position)
        {
            mItemList = itemList;
            Size = size;
            mSizeBackup = size;
            Position = position;
            ActiveWindow = true;
            ActiveHorizontalCollection = true;
            mPadding = CalcPadding(itemList: itemList, size: size);

            if (mPadding < 0)
                // catch items too big for size ! shouldn't happen, because right now it's NOT automatically fixed !
            {
                mPadding = 0;
            }
        }

        /// <summary>
        /// standard update method
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            // if the horizontalCollection is deactivated shrink size to -10, else use backup Size,
            // so that the windowObject treats this horizontalCollection as if it's not existent
            // (-10 due to the objectPadding used in the UI)
            Size = !ActiveHorizontalCollection ? new Vector2(x: 0, y: -10) : mSizeBackup;

            // activate items in itemList if window + list are active
            if (ActiveWindow && ActiveHorizontalCollection)
            {
                // shift from the left border to place all items
                float shift = 0;

                // activate items, set position, update shift
                foreach (var item in mItemList)
                {
                    item.ActiveWindow = true; // activate all objects if the window is active in case they got deactivated
                    item.Update(gametime: gametime);
                    item.Position = new Vector2(x: Position.X + shift, y: Position.Y);
                    shift = shift + item.Size.X * 0.25f + mPadding;
                }
            }
            else
            {
                // since the window or the list are inactive -> deactivate all objects in list
                foreach (var item in mItemList)
                {
                    item.ActiveWindow = false;
                }
            }
        }

        /// <summary>
        /// standard draw method
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveWindow && ActiveHorizontalCollection)
            {
                foreach (var item in mItemList)
                {
                    item.Draw(spriteBatch: spriteBatch);
                }
            }
        }

        /// <summary>
        /// Calculate equal padding between objects given a size
        /// </summary>
        /// <param name="itemList">list of objects</param>
        /// <param name="size">size to fit the objects</param>
        /// <returns></returns>
        private float CalcPadding(List<IWindowItem> itemList, Vector2 size)
        {
            float width = 0;

            foreach (var item in mItemList)
            {
                width = width + item.Size.X * 0.25f;
            }

            return (size.X - width - 20) / (itemList.Count - 1);
        }

        // top left corner of the horizontalCollection
        public Vector2 Position { get; set; }
        // size of the horizontalCollection
        public Vector2 Size { get; private set; }
        // true if the window that holds this item is active, else false
        public bool ActiveWindow { get; set; }
        // true if this horizontalCollection is active, else false
        public bool ActiveHorizontalCollection { private get; set; }
    }
}
