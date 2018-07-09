using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Property;

namespace Singularity.Screen
{
    public sealed class EventLog : IUpdate
    {
        private readonly Queue<EventLogIWindowItem> mEventList;

        private EventLogIWindowItem mAddedEvent;

        private bool mNewEvent;
        private SpriteFont mLibSans10;

        private readonly Director mDirector;

        private readonly UserInterfaceController mUserInterfaceController;

        private bool mLoaded;

        private readonly ContentManager mContentManager;

        public EventLog(UserInterfaceController userInterfaceController, Director director, ContentManager content)
        {
            mUserInterfaceController = userInterfaceController;

            mDirector = director;

            mEventList = new Queue<EventLogIWindowItem>();

            mContentManager = content;
        }

        public void AddEvent(ELogEventType eventType, string text, Vector2 position)
        {
            mAddedEvent = new EventLogIWindowItem(eventType, text, position, 180, mLibSans10, mDirector);

            mEventList.Enqueue(mAddedEvent);
            mNewEvent = true;
        }

        public void Update(GameTime gametime)
        {
            // load the spriteFont in first update
            if (!mLoaded)
            {
                mLibSans10 = mContentManager.Load<SpriteFont>("LibSans12");
                mLoaded = true;
            }

            if (mNewEvent)
            {
                EventLogIWindowItem oldEventToDelete = null;

                if (mEventList.Count > 10)
                {
                    oldEventToDelete = mEventList.Dequeue();
                }

                mUserInterfaceController.UpdateEventLog(mAddedEvent, oldEventToDelete);
                mNewEvent = false;
            }
        }
    }
}
