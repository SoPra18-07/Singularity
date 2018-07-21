using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen
{
    /// <summary>
    /// An IndexSwitcher consists of two buttons and text between them. The buttons can be used to change the index of a given list
    /// </summary>
    internal sealed class IndexSwitcherIWindowItem : IWindowItem
    {
        #region member variables

        /// <summary>
        /// the list's current index that is shown
        /// </summary>
        private int mCurrentIndex;

        /// <summary>
        /// the minimum unused index
        /// </summary>
        private int mNextFreeIndex;

        /// <summary>
        /// buttons to move index up and down
        /// </summary>
        private readonly Button mPreviousIndexButton;
        private readonly Button mNextIndexButton;

        /// <summary>
        /// the description the is given when creating the object. It can show what element's are represented by the integers in the ListOfElements
        /// </summary>
        private readonly string mDescriptionText;

        /// <summary>
        /// the complete text is composed of the description text and the integer of the current index
        /// </summary>
        private string mTextComplete;

        /// <summary>
        /// textfont
        /// </summary>
        private readonly SpriteFont mSpriteFont;

        /// <summary>
        /// the string size is the width of the TextComplete - used to measure the position of the previousIndexButton
        /// </summary>
        private float mStringSize;

        /// <summary>
        /// list of all unused indexes which were used once
        /// </summary>
        private readonly List<int> mFreeIndexList = new List<int>();

        /// <summary>
        /// Dictionaries connecting indexes and elementIds both ways
        /// </summary>
        private readonly Dictionary<int, int> mIndexToIdDict = new Dictionary<int, int> { { 0, 0 } };
        private readonly Dictionary<int, int> mIdToIndexDict = new Dictionary<int, int> { { 0, 0 } };

        #endregion

        /// <summary>
        /// Creates an IndexSwitcher consisting of two buttons and text between them.
        /// The buttons can be used to switch the index and get the values at the index of the set List.
        /// </summary>
        /// <param name="descriptionText">text between buttons (i.e. "graph No: ")</param>
        /// <param name="width">width of the window, where this is placed in - eventual padding</param>
        /// <param name="spriteFont">textFont</param>
        /// <param name="director">director</param>
        public IndexSwitcherIWindowItem(
            string descriptionText,
            float width,
            SpriteFont spriteFont)
        {
            // set members
            mDescriptionText = descriptionText;
            mSpriteFont = spriteFont;
            mCurrentIndex = 0;

            // create the previousIndexButton
            mPreviousIndexButton = new Button(" < ", spriteFont, Vector2.Zero) { Opacity = 1f };
            mPreviousIndexButton.ButtonReleased += PreviousIndex;

            mTextComplete = descriptionText + 0;
            mStringSize = mSpriteFont.MeasureString(mTextComplete).X;

            // create the nextIndexButton
            mNextIndexButton = new Button(" > ", spriteFont, Vector2.Zero) { Opacity = 1f };
            mNextIndexButton.ButtonReleased += NextIndex;

            // set the Size, where the height is simply the height of the measured string of anything using the given SpriteFont
            Size = new Vector2(width, mSpriteFont.MeasureString("A").Y);

            ActiveInWindow = true;
        }

        /// <inheritdoc />
        public void Update(GameTime gametime)
        {

            if (ActiveInWindow && !OutOfScissorRectangle && !InactiveInSelectedPlatformWindow)
            {
                // nextIndexButton is almost aligned to the right of the window
                mNextIndexButton.Position = new Vector2(Position.X + Size.X - mNextIndexButton.Size.X, Position.Y);

                // previousIndexButton is positioned left of the infoText
                mPreviousIndexButton.Position = new Vector2(mNextIndexButton.Position.X - mStringSize - mPreviousIndexButton.Size.X, Position.Y);

                // update both buttons
                mPreviousIndexButton.Update(gametime);
                mNextIndexButton.Update(gametime);
            }
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !OutOfScissorRectangle && !InactiveInSelectedPlatformWindow)
            {
                // draw the buttons
                mPreviousIndexButton.Draw(spriteBatch);
                mNextIndexButton.Draw(spriteBatch);

                // draw the description + current integer from list between the two buttons
                spriteBatch.DrawString(mSpriteFont, mTextComplete, new Vector2(mNextIndexButton.Position.X - mStringSize, Position.Y), Color.White);
            }
        }

        /// <summary>
        /// Set the index to previous index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void PreviousIndex(object sender, EventArgs eventArgs)
        {
            mCurrentIndex -= 1;

            while (mFreeIndexList.Contains(mCurrentIndex))
            {
                mCurrentIndex -= 1;
            }

            if (mCurrentIndex < 0)
            {
                mCurrentIndex = mIndexToIdDict.Keys.Max();
            }

            UpdateText();
        }

        /// <summary>
        /// Set the index to next index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void NextIndex(object sender, EventArgs eventArgs)
        {
            mCurrentIndex += 1;

            while (mFreeIndexList.Contains(mCurrentIndex))
            {
                mCurrentIndex += 1;
            }

            if (mCurrentIndex > mIndexToIdDict.Keys.Max())
            {
                mCurrentIndex = mIndexToIdDict.Keys.Min();
            }

            UpdateText();
        }

        /// <summary>
        /// returns the integer value of the lists current index
        /// </summary>
        /// <returns></returns>
        public int GetCurrentId()
        {
            return mIndexToIdDict[mCurrentIndex];
        }

        /// <summary>
        /// Add a new element into the indexSwitcher
        /// </summary>
        /// <param name="elementId">element to add</param>
        public void AddElement(int elementId)
        {
            UpdateMinFreeIndex();

            // add the graph id in the dicts with the min free index
            mIndexToIdDict.Add(mNextFreeIndex, elementId);
            mIdToIndexDict.Add(elementId, mNextFreeIndex);
        }

        /// <summary>
        /// Merges two elements using the possibly newly created newElementId while deleting the two old ids
        /// </summary>
        /// <param name="newElementId">merged elementID</param>
        /// <param name="oldElementId1">element1 that got merged</param>
        /// <param name="oldElementId2">element2 that got merged</param>
        public void MergeElements(int newElementId, int oldElementId1, int oldElementId2)
        {
            // check which of the two merged elements had the smaller index in the indexSwitcher
            if (mIdToIndexDict[oldElementId1] < mIdToIndexDict[oldElementId2])
            // first element is smaller therefore update the index of this element with the mergedElement
            {
                if (mCurrentIndex == mIdToIndexDict[oldElementId2])
                {
                    mCurrentIndex = mIdToIndexDict[oldElementId1];
                }

                var indexToUpdate = mIdToIndexDict[oldElementId1];

                // update smaller index with new GraphID
                mIndexToIdDict[indexToUpdate] = newElementId;

                // add the index which will become free since it will be removed
                mFreeIndexList.Add(mIdToIndexDict[oldElementId2]);

                // remove merged graphID's index
                mIndexToIdDict.Remove(mIdToIndexDict[oldElementId2]);

                // remove both graphID's from the graphDict
                mIdToIndexDict.Remove(oldElementId1);
                mIdToIndexDict.Remove(oldElementId2);

                // add new graphID
                mIdToIndexDict.Add(newElementId, indexToUpdate);
            }
            else
            // second element is smaller therefore update the index of this element with the mergedElement
            {
                if (mCurrentIndex == mIdToIndexDict[oldElementId1])
                {
                    mCurrentIndex = mIdToIndexDict[oldElementId2];
                }

                var indexToUpdate = mIdToIndexDict[oldElementId2];

                // update smaller index with new GraphID
                mIndexToIdDict[indexToUpdate] = newElementId;

                // add the index which will become free since it will be removed
                mFreeIndexList.Add(mIdToIndexDict[oldElementId1]);

                // remove merged graphID's index
                mIndexToIdDict.Remove(mIdToIndexDict[oldElementId1]);

                // remove both graphID's from the graphDict
                mIdToIndexDict.Remove(oldElementId2);
                mIdToIndexDict.Remove(oldElementId1);

                // add new graphID
                mIdToIndexDict.Add(newElementId, indexToUpdate);
            }
        }

        /// <summary>
        /// Calling all graphs will compare the mIdToIndexDict with the given graphIdDict and update
        /// the mIdToIndexDict + mIndexToIdDict to fit the current situation on the structure map
        /// </summary>
        /// <param name="graphIdToGraphs">the graphIdToGraphs dict from the structure map</param>
        /// <param name="sliderHandler"></param>
        public void CallingAllGraphs(Dictionary<int, Graph.Graph> graphIdToGraphs, SliderHandler sliderHandler)
        {
            // all the keys from the graphIdToGraphs dict
            var graphIdsWithNull = graphIdToGraphs.Keys;

            // all keys from graphIdToGraphsDict that aren't null
            var graphIdsWithoutNull = new List<int>();

            // add all graphIds which are not represented in mIdToIndexDict yet
            foreach (var id in graphIdsWithNull)
            {
                if (graphIdToGraphs[id] == null)
                {
                    continue;
                }

                if (!mIdToIndexDict.Keys.Contains(id))
                {
                    AddElement(id);
                    sliderHandler.SetGraphId(id);
                    sliderHandler.Refresh();
                    sliderHandler.ForceSliderPages();
                }

                graphIdsWithoutNull.Add(id);
            }

            // calculate all removed graphs which are still present in the mIdToIndexDict
            var savedGraphIds = mIdToIndexDict.Keys;
            var listOfIdsToDelete = new List<int>();
            foreach (var savedId in savedGraphIds)
            {
                if (!graphIdsWithoutNull.Contains(savedId))
                {
                    listOfIdsToDelete.Add(savedId);
                }
            }

            // delete all removed graphs from mIdToIndexDict + mIndexToIdDict
            foreach (var idToDelete in listOfIdsToDelete)
            {
                // add the index which will become free since it will be removed
                mFreeIndexList.Add(mIdToIndexDict[idToDelete]);

                // remove graphID's index from indexDict
                mIndexToIdDict.Remove(mIdToIndexDict[idToDelete]);

                // remove graphID from graphDict
                mIdToIndexDict.Remove(idToDelete);
            }

            mCurrentIndex = mIndexToIdDict.Keys.Min();
        }

        /// <summary>
        /// Update the current index to the index of the elementId
        /// </summary>
        /// <param name="elementId">Id to set the index to</param>
        public void UpdateCurrentIndex(int elementId)
        {
            mCurrentIndex = mIdToIndexDict[elementId];
            UpdateText();
        }

        /// <summary>
        /// Update the currently minimum unused index
        /// </summary>
        private void UpdateMinFreeIndex()
        {
            // set new free index either by size
            if (mFreeIndexList.Count == 0)
            {
                // index = dict-size, because all indexes are used
                mNextFreeIndex = mIndexToIdDict.Count;
            }
            else
            {
                // index = min possible index
                mFreeIndexList.Sort();
                mNextFreeIndex = mFreeIndexList[0];
                mFreeIndexList.RemoveAt(0);
            }
        }

        /// <summary>
        /// Update the text shown by the Switcher
        /// </summary>
        private void UpdateText()
        {
            mTextComplete = mDescriptionText + mCurrentIndex;
            mStringSize = mSpriteFont.MeasureString(mTextComplete).X;
        }

        /// <inheritdoc />
        public Vector2 Position { get; set; }
        /// <inheritdoc />
        public Vector2 Size { get; }
        /// <inheritdoc />
        public bool ActiveInWindow { get; set; }
        /// <inheritdoc />
        public bool InactiveInSelectedPlatformWindow { get; set; }
        /// <inheritdoc />
        public bool OutOfScissorRectangle { get; set; }
    }
}
