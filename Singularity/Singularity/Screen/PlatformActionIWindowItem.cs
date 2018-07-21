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
    /// <summary>
    /// Represents a platformAction with it's name, it's state and a button to toggle the state
    /// </summary>
    internal sealed class PlatformActionIWindowItem : IWindowItem
    {
        #region member variables

        // action name
        private readonly TextField mNameTextField;

        // action state toggler button
        private readonly Button mStateToggleButton;

        // horizontal collection containing name, state + button
        private readonly HorizontalCollection mCollection;

        // infoBox for required resources / units
        private readonly InfoBoxWindow mInfoBoxRequirements;

/*        // barItem under the action to increase visibility between the actions
        private readonly BarIWindowItem mBottomBar;*/

        // The PlatformAction to take care of
        private readonly IPlatformAction mPlatformAction;

        // button has been clicked on - to prevent the button from keeping firing
        private bool mClicked;
        
        #endregion

        /// <summary>
        /// Creates a PlatformAction IWindowItem which represents a platformaction with it's name, state and a button to (de)activate it.
        /// </summary>
        /// <param name="platformAction">the platformaction to be represented</param>
        /// <param name="spriteFont">spritefont for text</param>
        /// <param name="position">position of the IWindowItem (Vector.Zero if added to window)</param>
        /// <param name="size">size to fit the item in (width is important)</param>
        /// <param name="director">director</param>
        /// <param name="refineRes">the action is of type refineRes</param>
        public PlatformActionIWindowItem(IPlatformAction platformAction, SpriteFont spriteFont, Vector2 position, Vector2 size, Director director, bool refineRes = false)
        {
            Size = new Vector2(size.X, spriteFont.MeasureString("A").Y * 2 + 5);
            Position = position;

            mPlatformAction = platformAction;

            ActiveInWindow = true;

            // infobox items list
            var infoBoxItemsList = new List<IWindowItem>();

            // get action description
            var name = platformAction.ToString().Split('.')[2];

            if (refineRes)
            {
                // since the action is of type refine resource
                name = "refining to " + ((RefineResourceAction)platformAction).GetRefiningTo();
            }

            // the platformAction's name
            mNameTextField = new TextField(
                text: name,
                position: Vector2.Zero,
                size: new Vector2(spriteFont.MeasureString(name).X, 0), // the size of the textfield should be as big as the string it contains
                spriteFont: spriteFont,
                color: Color.White);

            // the platformAction's current state ((de)active)
            var stateTextField = new TextField(
                text: platformAction.State.ToString(),
                position: Vector2.Zero,
                size: new Vector2(spriteFont.MeasureString(platformAction.State.ToString()).X, 0), // the size of the textfield should be as big as the string it contains
                spriteFont: spriteFont,
                color: Color.White);

            if (mPlatformAction is MakeFastMilitaryUnit ||
                mPlatformAction is MakeHeavyMilitaryUnit ||
                mPlatformAction is MakeStandardMilitaryUnit ||
                mPlatformAction is MakeGeneralUnit ||
                mPlatformAction is MakeSettlerUnit)
            {
                // don't use the standard text for the button since the action is different (just creat button - will toggle back when created)
                mStateToggleButton = new Button("create", spriteFont, Vector2.Zero) { Opacity = 1f };
            }
            else
            {
                // button that (de)activates the platformAction
                mStateToggleButton = new Button("(de)activate", spriteFont, Vector2.Zero) { Opacity = 1f };
            }

            // a textfiel that is just added to shift the button in the horizontal collection
            var emptyToShift = new TextField(
                text: "",
                position: Vector2.Zero,
                size: new Vector2(spriteFont.MeasureString(platformAction.State.ToString()).X, 0), // the size of the textfield should be as big as the string it contains
                spriteFont: spriteFont,
                color: Color.White);

            // button management
            mStateToggleButton.ButtonClicked += ActivateToggle;
            mStateToggleButton.ButtonReleased += ToggleButton;
            mStateToggleButton.ButtonHovering += ShowRequirements;
            mStateToggleButton.ButtonHoveringEnd += HideRequirements;

            // create a horizontal collection of the state and the button
            mCollection = new HorizontalCollection(new List<IWindowItem> {stateTextField, mStateToggleButton, emptyToShift}, new Vector2(size.X, spriteFont.MeasureString("A").Y + 5), Position);

            //mBottomBar = new BarIWindowItem(size.X - 50, Color.White);

            #region manage the requirements

            // set up
            var productionUnits = 0;
            var constructionUnits = 0;
            var logisticUnits = 0;
            var defenseUnits = 0;

            // count the required units
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

            // add a unit type + required count to infoBox if it's needed to enable the platformAction, disable it else
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

            // add required resources infoBox
            if (mPlatformAction is AMakeUnit)
            {
                infoBoxItemsList.AddRange(((AMakeUnit)platformAction).GetBuildingCost().Select(resource => new ResourceIWindowItem(resource.Key, resource.Value, Vector2.Zero, spriteFont)));
            }
            
            // create a infoBox containing all requirements to activate the platformAction
            mInfoBoxRequirements = new InfoBoxWindow(infoBoxItemsList, Vector2.Zero, Color.White, Color.Black, true, director);

            #endregion
        }

        /// <inheritdoc />
        public void Update(GameTime gametime)
        {
            if (!ActiveInWindow || InactiveInSelectedPlatformWindow || OutOfScissorRectangle || WindowIsInactive) { return; }
            // update all positions
            mNameTextField.Position = Position;
            mCollection.Position = new Vector2(Position.X, Position.Y + mNameTextField.Size.Y + 5);
            //mBottomBar.Position = new Vector2(Position.X, mCollection.Position.Y + mCollection.Size.Y + 5 + 5);

            // update all components
            mInfoBoxRequirements.Update(gametime);
            mStateToggleButton.Update(gametime);
            mCollection.Update(gametime);
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!ActiveInWindow || InactiveInSelectedPlatformWindow || OutOfScissorRectangle || WindowIsInactive) { return; }
            //draw all components
            mCollection.Draw(spriteBatch);
            mNameTextField.Draw(spriteBatch);
            //mBottomBar.Draw(spriteBatch);
            mInfoBoxRequirements.Draw(spriteBatch);
        }

        /// <summary>
        /// Enable toggling of the platform's state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ActivateToggle(object sender, EventArgs eventArgs)
        {
            mClicked = true;
        }

        /// <summary>
        /// Toggles the button, meaning (de)activating the platform's action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ToggleButton(object sender, EventArgs eventArgs)
        {
            if (!mClicked) { return; }

            mPlatformAction.UiToggleState();
            ((TextField) mCollection.mItemList[0]).UpdateText(mPlatformAction.State.ToString());
        }

        /// <summary>
        /// Show the infoBox above the button that (de)activates the action with requirements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ShowRequirements(object sender, EventArgs eventArgs)
        {
            mInfoBoxRequirements.Active = true;
        }

        /// <summary>
        /// Hide the infoBox above the button that (de)activates the action with requirements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void HideRequirements(object sender, EventArgs eventArgs)
        {
            mInfoBoxRequirements.Active = false;
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
        /// <inheritdoc />
        public bool WindowIsInactive { get; set; }
    }
}
