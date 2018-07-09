using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;

namespace Singularity.Screen
{
    public sealed class EventLogIWindowItem : IWindowItem
    {
        private Vector2 mPositionOfEvent;

        private readonly Button mPositionButton;
        private readonly TextField mText;

        private readonly float mShiftValue;

        private readonly InfoBoxWindow mInfoBox;

        public EventLogIWindowItem(ELogEventType eventType, string message, Vector2 positionOfEvent, float width, SpriteFont spriteFont, Director director)
        {
            mPositionOfEvent = positionOfEvent;

            mPositionButton = new Button("// " + eventType, spriteFont, Vector2.Zero) {Opacity = 1f};
            mPositionButton.ButtonHovering += ShowInfoBox;
            mPositionButton.ButtonHoveringEnd += HideInfoBox;
            mPositionButton.ButtonClicked += JumpToPosition;

            mShiftValue = spriteFont.MeasureString("// ").X;

            mText = new TextField(message, Vector2.Zero, new Vector2(width - mShiftValue, 0), spriteFont, Color.White);

            Size = new Vector2(width, mPositionButton.Size.Y + mText.Size.Y);

            Position = Vector2.Zero;

            mInfoBox = new InfoBoxWindow(new List<IWindowItem> { new TextField("To event", Vector2.Zero, spriteFont.MeasureString("To event"), spriteFont, Color.White) }, spriteFont.MeasureString("To event"), Color.White, Color.Black, true, director);

            ActiveInWindow = true;
        }

        public void Update(GameTime gametime)
        {
            if (ActiveInWindow && !OutOfScissorRectangle)
            {
                mPositionButton.Update(gametime);
                mText.Update(gametime);

                mPositionButton.Position = Position;
                mText.Position = new Vector2(mPositionButton.Position.X + mShiftValue, Position.Y + mPositionButton.Size.Y);

                mInfoBox.Update(gametime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !OutOfScissorRectangle)
            {
                mPositionButton.Draw(spriteBatch);
                mText.Draw(spriteBatch);

                mInfoBox.Draw(spriteBatch);
            }
        }

        private void ShowInfoBox(object sender, EventArgs eventArgs)
        {
            mInfoBox.Active = true;
        }

        private void HideInfoBox(object sender, EventArgs eventArgs)
        {
            mInfoBox.Active = false;
        }

        private void JumpToPosition(object sender, EventArgs eventArgs)
        {
            // TODO
        }

        public Vector2 Position { get; set; }
        public Vector2 Size { get; }
        public bool ActiveInWindow { get; set; }
        public bool InactiveInSelectedPlatformWindow { get; set; }
        public bool OutOfScissorRectangle { get; set; }
    }
}
