using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Events;
using Singularity.Manager;
using Singularity.Property;

namespace Singularity.Screen
{
    /// <summary>
    /// EventLog IWindowItems are created by the eventLog when receiving a new event
    /// </summary>
    public sealed class EventLogIWindowItem : IWindowItem
    {
        #region member variables

        // the button used to jump to the position of the object that created this event
        private readonly Button mPositionButton;

        // the text/message to show the user in the eventLog
        private readonly TextField mText;

        // the shift value to shift the text to correctly fit the button
        private readonly float mShiftValue;

        // the infoBox showing when hovering above the button
        private readonly InfoBoxWindow mInfoBox;

        private readonly Director mDirector;

        #endregion

        /// <summary>
        /// Creates an EventLogIWindowItem to give to EventLogWindow in UI consisiting of a button and a message
        /// </summary>
        /// <param name="eventType">the event's type</param>
        /// <param name="message">message for the user</param>
        /// <param name="width">width to fit the text to</param>
        /// <param name="spriteFont">textfont</param>
        /// <param name="director">basic director</param>
        /// <param name="onThis">the object where the event is happening</param>
        public EventLogIWindowItem(ELogEventType eventType, string message, float width, SpriteFont spriteFont, Director director, ISpatial onThis)
        {
            mShiftValue = spriteFont.MeasureString("// ").X;
            mText = new TextField(message, Vector2.Zero, new Vector2(width - mShiftValue, 0), spriteFont, Color.White);
            mInfoBox = new InfoBoxWindow(new List<IWindowItem> { new TextField("To event", Vector2.Zero, spriteFont.MeasureString("To event"), spriteFont, Color.White) }, spriteFont.MeasureString("To event"), Color.White, Color.Black, true, director);

            // button to jump to object that created the event
            mPositionButton = new Button("// " + eventType, spriteFont, Vector2.Zero, false, new SpatialPositionEventArgs(onThis)) {Opacity = 1f};
            mPositionButton.ButtonHovering += ShowInfoBox;
            mPositionButton.ButtonHoveringEnd += HideInfoBox;
            mPositionButton.ButtonClicked += JumpToPosition;

            Size = new Vector2(width, mPositionButton.Size.Y + mText.Size.Y);
            Position = Vector2.Zero;

            mDirector = director;

            ActiveInWindow = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (ActiveInWindow && !OutOfScissorRectangle)
            {
                // update position of all components
                mPositionButton.Position = Position;
                mText.Position = new Vector2(mPositionButton.Position.X + mShiftValue, Position.Y + mPositionButton.Size.Y);

                // update all components
                mPositionButton.Update(gametime);
                mText.Update(gametime);
                mInfoBox.Update(gametime);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !OutOfScissorRectangle)
            {
                // draw all components
                mPositionButton.Draw(spriteBatch);
                mText.Draw(spriteBatch);
                mInfoBox.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Show the button's infoBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ShowInfoBox(object sender, EventArgs eventArgs)
        {
            mInfoBox.Active = true;
        }

        /// <summary>
        /// Hide the button's infoBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void HideInfoBox(object sender, EventArgs eventArgs)
        {
            mInfoBox.Active = false;
        }

        /// <summary>
        /// Jump to the position given by the event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void JumpToPosition(object sender, EventArgs eventArgs)
        {
            var asSpatial = (SpatialPositionEventArgs) eventArgs;

            //TODO: pass accurate coordinates, right now it goes to the top left texture point of the platform blank on the map
            // also note, since the validate position code is atm buggy, the map might disappear when the zoom level is too far out
            // im going to fix this definitely
            mDirector.GetStoryManager.Level.Camera.CenterOn(asSpatial.GetAbsoluteCenter());
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public Vector2 Position { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public Vector2 Size { get; }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public bool ActiveInWindow { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public bool InactiveInSelectedPlatformWindow { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public bool OutOfScissorRectangle { get; set; }
    }
}
