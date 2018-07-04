using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Manager;
using Singularity.PlatformActions;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Screen
{
    class PlatformActionIWindowItem : IWindowItem, IMousePositionListener
    {
        // action name
        private TextField mNameTextField;

        // action state
        private TextField mStateTextField;

        // action state toggler button
        private Button mStateToggleButton;

        // horizontal collection containing name, state + button
        private HorizontalCollection mCollection;

        // infobox items list
        private List<IWindowItem> mInfoBoxItemsList;

        // infoBox for required resources / units
        private InfoBoxWindow mInfoBoxRequirements;

        // list of all items to be able to comfortably iterate through them in update + draw method
        private List<IWindowItem> mItemList;

        // spritefont
        private SpriteFont mSpriteFont;

        // platform action
        private IPlatformAction mPlatformAction;

        // mouse position
        private Vector2 mMousePos;

        // units
        private int mProductionUnits;
        private int mConstructions;
        private int mLogistics;
        private int mDefense;

        // resources

        public PlatformActionIWindowItem(IPlatformAction platformAction, SpriteFont spriteFont, Vector2 size, Vector2 position, Director director)
        {
            mSpriteFont = spriteFont;
            Size = size;
            Position = position;

            var name = "? Name ?";

            mNameTextField = new TextField(
                text: name, 
                position: Vector2.Zero, 
                size: new Vector2(mSpriteFont.MeasureString(name).X, 0), // the size of the textfield should be as big as the string it contains
                spriteFont: mSpriteFont, 
                color: Color.White);

            mStateTextField = new TextField(
                text: platformAction.State.ToString(), 
                position: Vector2.Zero, 
                size: new Vector2(mSpriteFont.MeasureString(platformAction.State.ToString()).X, 0), // the size of the textfield should be as big as the string it contains
                spriteFont: mSpriteFont,
                color: Color.White);

            mStateToggleButton = new Button("toggle", mSpriteFont, Vector2.Zero);

            mStateToggleButton.ButtonClicked += ToggleButton;
            mStateToggleButton.ButtonHovering += ShowRequirements;
            mStateToggleButton.ButtonHoveringEnd += HideRequirements;

            mCollection = new HorizontalCollection(new List<IWindowItem> {mNameTextField, mStateTextField, mStateToggleButton}, Size, Position);

            mItemList = new List<IWindowItem> {mNameTextField, mStateTextField, mStateToggleButton, mCollection};

            mInfoBoxItemsList = new List<IWindowItem>();

            mProductionUnits = 0;
            mConstructions = 0;
            mLogistics = 0;
            mDefense = 0;

            foreach (var units in platformAction.UnitsRequired)
            {
                switch (units)
                {
                    case JobType.Production:
                        mProductionUnits += 1;
                        break;
                    case JobType.Construction:
                        mConstructions += 1;
                        break;
                    case JobType.Logistics:
                        mLogistics += 1;
                        break;
                    case JobType.Defense:
                        mDefense += 1;
                        break;
                }
            }

            if (mProductionUnits > 0)
            {
                var productionUnits = new TextField(mProductionUnits + " production", Vector2.Zero, Vector2.Zero, mSpriteFont, Color.White);
                mInfoBoxItemsList.Add(productionUnits);
            }

            if (mConstructions > 0)
            {
                var constructionUnits = new TextField(mConstructions + " construction", Vector2.Zero, Vector2.Zero, mSpriteFont, Color.White);
                mInfoBoxItemsList.Add(constructionUnits);
            }

            if (mLogistics > 0)
            {
                var logisticsUnits = new TextField(mLogistics + " logistics", Vector2.Zero, Vector2.Zero, mSpriteFont, Color.White);
                mInfoBoxItemsList.Add(logisticsUnits);
            }

            if (mDefense > 0)
            {
                var defenseUnits = new TextField(mDefense + " defense", Vector2.Zero, Vector2.Zero, mSpriteFont, Color.White);
                mInfoBoxItemsList.Add(defenseUnits);
            }

            foreach (var resource in platformAction.GetRequiredResources())
            {
                var currentResource = new ResourceIWindowItem(resource.Key, resource.Value, Vector2.Zero, mSpriteFont);
                mInfoBoxItemsList.Add(currentResource);
            }

            //mInfoBoxRequirements = new InfoBoxWindow(mInfoBoxItemsList, Vector2.Zero, Color.White, Color.White,  );
        }

        public void Update(GameTime gametime)
        {
            if (ActiveWindow)
            {
                mCollection.Position = Position;

                foreach (var item in mItemList)
                {
                    item.Update(gametime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveWindow)
            {
                foreach (var item in mItemList)
                {
                    item.Draw(spriteBatch);
                }
            }
        }

        private void ToggleButton(object sender, EventArgs eventArgs)
        {
            mPlatformAction.UiToggleState();
        }

        private void ShowRequirements(object sender, EventArgs eventArgs)
        {

        }

        private void HideRequirements(object sender, EventArgs eventArgs)
        {

        }

        public Vector2 Position { get; set; }
        public Vector2 Size { get; }
        public bool ActiveWindow { get; set; }
        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            mMousePos = new Vector2(screenX, screenY);
        }
    }
}
