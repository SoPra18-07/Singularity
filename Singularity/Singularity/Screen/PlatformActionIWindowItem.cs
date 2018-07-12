using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;
using Singularity.Units;

namespace Singularity.Screen
{
    sealed class PlatformActionIWindowItem : IWindowItem
    {
        // action name
        private readonly TextField mNameTextField;

        // action state toggler button
        private readonly Button mStateToggleButton;

        // horizontal collection containing name, state + button
        private readonly HorizontalCollection mCollection;

        // infoBox for required resources / units
        private readonly InfoBoxWindow mInfoBoxRequirements;

        // button has been clicked on - to prevent the button from keeping firing
        private bool mClicked;

        // The PlatformAction to take care of
        private IPlatformAction mPlatformAction;

        public PlatformActionIWindowItem(IPlatformAction platformAction, SpriteFont spriteFont, Vector2 position, Vector2 size, Director director)
        {
            Size = new Vector2(size.X, spriteFont.MeasureString("A").Y * 2 + 5);
            Position = position;

            mPlatformAction = platformAction;

            ActiveInWindow = true;

            // infobox items list
            var infoBoxItemsList = new List<IWindowItem>();

            // get action description
            var name = platformAction.ToString().Split('.')[2];

            mNameTextField = new TextField(
                text: name,
                position: Vector2.Zero,
                size: new Vector2(spriteFont.MeasureString(name).X, 0), // the size of the textfield should be as big as the string it contains
                spriteFont: spriteFont,
                color: Color.White);

            var stateTextField = new TextField(
                text: platformAction.State.ToString(),
                position: Vector2.Zero,
                size: new Vector2(spriteFont.MeasureString(platformAction.State.ToString()).X, 0), // the size of the textfield should be as big as the string it contains
                spriteFont: spriteFont,
                color: Color.White);

            mStateToggleButton = new Button("switchState", spriteFont, Vector2.Zero) {Opacity = 1f};

            var emptyToShift = new TextField(
                text: "",
                position: Vector2.Zero,
                size: new Vector2(spriteFont.MeasureString(platformAction.State.ToString()).X, 0), // the size of the textfield should be as big as the string it contains
                spriteFont: spriteFont,
                color: Color.White);

            mStateToggleButton.ButtonClicked += ActivateToggle;
            mStateToggleButton.ButtonReleased += ToggleButton;
            mStateToggleButton.ButtonHovering += ShowRequirements;
            mStateToggleButton.ButtonHoveringEnd += HideRequirements;


            mCollection = new HorizontalCollection(new List<IWindowItem> {stateTextField, mStateToggleButton, emptyToShift}, Size, Position);

            // add unit requirements

            var productionUnits = 0;
            var constructionUnits = 0;
            var logisticUnits = 0;
            var defenseUnits = 0;

            foreach (var units in platformAction.UnitsRequired)
            {
                switch (units)
                {
                    case JobType.Production:
                        productionUnits += 1;
                        break;
                    case JobType.Construction:
                        constructionUnits += 1;
                        break;
                    case JobType.Logistics:
                        logisticUnits += 1;
                        break;
                    case JobType.Defense:
                        defenseUnits += 1;
                        break;
                }
            }

            if (productionUnits > 0)
            {
                infoBoxItemsList.Add(new TextField(productionUnits + " production units",
                    Vector2.Zero,
                    spriteFont.MeasureString(productionUnits + " prouduction units"),
                    spriteFont,
                    Color.White));
            }

            if (constructionUnits > 0)
            {
                infoBoxItemsList.Add(new TextField(constructionUnits + " construction units",
                    Vector2.Zero,
                    spriteFont.MeasureString(constructionUnits + " construction units"),
                    spriteFont,
                    Color.White));
            }

            if (logisticUnits > 0)
            {
                infoBoxItemsList.Add(new TextField(logisticUnits + " logistics units",
                    Vector2.Zero,
                    spriteFont.MeasureString(logisticUnits + " logistics units"),
                    spriteFont,
                    Color.White));
            }

            if (defenseUnits > 0)
            {
                infoBoxItemsList.Add(new TextField(defenseUnits + " defense units",
                    Vector2.Zero,
                    spriteFont.MeasureString(defenseUnits + " defense units"),
                    spriteFont,
                    Color.White));
            }

            infoBoxItemsList.AddRange(platformAction.GetRequiredResources()
                .Select(resource => new ResourceIWindowItem(resource.Key, resource.Value, Vector2.Zero, spriteFont)));

            mInfoBoxRequirements = new InfoBoxWindow(infoBoxItemsList, Vector2.Zero, Color.White, Color.Black, true, director);
        }

        public void Update(GameTime gametime)
        {
            if (!ActiveInWindow || InactiveInSelectedPlatformWindow || OutOfScissorRectangle) return;
            mInfoBoxRequirements.Update(gametime);
            mNameTextField.Position = Position;

            mCollection.Position = new Vector2(Position.X, Position.Y + mNameTextField.Size.Y + 5);

            mStateToggleButton.Update(gametime);
            mCollection.Update(gametime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle)
            {
                mCollection.Draw(spriteBatch);
                mNameTextField.Draw(spriteBatch);

                mInfoBoxRequirements.Draw(spriteBatch);
            }
        }

        private void ActivateToggle(object sender, EventArgs eventArgs)
        {
            mClicked = true;
        }

        private void ToggleButton(object sender, EventArgs eventArgs)
        {
            if (!mClicked) return;
            mPlatformAction.UiToggleState();
            ((TextField) mCollection.mItemList[0]).UpdateText(mPlatformAction.State.ToString());
        }

        private void ShowRequirements(object sender, EventArgs eventArgs)
        {
            mInfoBoxRequirements.Active = true;
        }

        private void HideRequirements(object sender, EventArgs eventArgs)
        {
            mInfoBoxRequirements.Active = false;
        }

        public Vector2 Position { get; set; }
        public Vector2 Size { get; }
        public bool ActiveInWindow { get; set; }
        public bool InactiveInSelectedPlatformWindow { get; set; }
        public bool OutOfScissorRectangle { get; set; }
    }
}
