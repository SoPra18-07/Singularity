using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Property;

namespace Singularity.Screen
{
    /// <summary>
    /// An EventLog consisting of a queue of events added.
    /// An event is added giving eventType, string (message) and the position of the object that created the event.
    /// The EventLog creates items for the given events and gives them to the UIController.
    /// </summary>
    public sealed class EventLog : IUpdate
    {
        #region member variables

        // the eventLog's queue consisting of eventLogItems
        private readonly Queue<EventLogIWindowItem> mEventList = new Queue<EventLogIWindowItem>();

        // the event that was newly added
        private EventLogIWindowItem mAddedEvent;

        // bool to show if a new event was added since the last update or not
        private bool mNewEvent;

        // textfont
        private SpriteFont mLibSans10;

        // basic director
        private readonly Director mDirector;

        // UIController to send data to
        private readonly UserInterfaceController mUserInterfaceController;

        // bool to set if the content has already been loaded or not
        private bool mLoaded;

        // contentManager to load the content from
        private readonly ContentManager mContentManager;

        #endregion

        /// <summary>
        /// Creates a new event log consisting of a queue of events added
        /// </summary>
        /// <param name="userInterfaceController">UIController to send data to</param>
        /// <param name="director">basic director</param>
        /// <param name="content">contentManager to load content from</param>
        public EventLog(UserInterfaceController userInterfaceController, Director director, ContentManager content)
        {
            mUserInterfaceController = userInterfaceController;

            mDirector = director;

            mContentManager = content;
        }

        /// <summary>
        /// Add a new Event go the EventLog
        /// </summary>
        /// <param name="eventType">the event's type</param>
        /// <param name="text">the text to show</param>
        /// <param name="onThis">the spatial that created this event</param>
        public void AddEvent(ELogEventType eventType, string text, ISpatial onThis)
        {
            // create a new EventLogItem of the event to add
            mAddedEvent = new EventLogIWindowItem(eventType, text, 180, mLibSans10, mDirector, onThis);

            // enqueue to the event's queue
            mEventList.Enqueue(mAddedEvent);

            mNewEvent = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            // load the spriteFont in first update
            if (!mLoaded)
            {
                mLibSans10 = mContentManager.Load<SpriteFont>("LibSans10");
                mLoaded = true;
            }

            if (mNewEvent)
                // a new event was added since the last update
            {
                // set to null in case there haven't been enough events yet to fill the queue
                EventLogIWindowItem oldEventToDelete = null;

                // prepare oldEven for deletion if the event queue is full
                if (mEventList.Count > 10)
                {
                    oldEventToDelete = mEventList.Dequeue();
                }

                // send data to UIController -> new event to add + oldest event that is thrown out of the log
                mUserInterfaceController.UpdateEventLog(mAddedEvent, oldEventToDelete);

                // reset newEvent bool
                mNewEvent = false;
            }
        }
    }
}
