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
        private readonly SpriteFont mLibSans10;

        private readonly Director mDirector;

        private readonly UserInterfaceController mUserInterfaceController;

        public EventLog(UserInterfaceController userInterfaceController, Director director, ContentManager content)
        {
            mLibSans10 = content.Load<SpriteFont>("LibSans10");

            mUserInterfaceController = userInterfaceController;

            mDirector = director;

            mEventList = new Queue<EventLogIWindowItem>();
        }

        public void AddEvent(string text, Vector2 position)
        {
            mAddedEvent = new EventLogIWindowItem(text, position, 180, mLibSans10, mDirector);

            mEventList.Enqueue(mAddedEvent);
            mNewEvent = true;
        }

        public void Update(GameTime gametime)
        {
            if (mNewEvent)
            {
                mUserInterfaceController.UpdateEventLog(mAddedEvent, mEventList.Dequeue());
                mNewEvent = false;
            }
        }
    }
}
