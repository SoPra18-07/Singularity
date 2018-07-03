using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.PlatformActions;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Screen
{
    class PlatformActionIWindowItem : IWindowItem
    {
        // action name
        private TextField mNameTextField;

        // action state
        private TextField mStateTextField;

        // action state toggler button
        private Button mStateToggleButton;

        // horizontal collection containing name, state + button
        private HorizontalCollection mCollection;

        // infoBox for required resources / units
        private InfoBoxWindow mInfoBoxRequirements;

        // list of all items to be able to comfortably iterate through them in update + draw method
        private List<IWindowItem> mItemList;

        // spritefont
        private SpriteFont mSpriteFont;

        public PlatformActionIWindowItem(PlatformActionState state, Dictionary<EResourceType, int> requiredResourcesDict, List<JobType> requiredUnitsList, SpriteFont spriteFont, Vector2 size, Vector2 position)
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
                text: state.ToString(), 
                position: Vector2.Zero, 
                size: new Vector2(mSpriteFont.MeasureString(state.ToString()).X, 0), // the size of the textfield should be as big as the string it contains
                spriteFont: mSpriteFont,
                color: Color.White);
            mStateToggleButton = new Button("toggle", mSpriteFont, Vector2.Zero);

            mCollection = new HorizontalCollection(new List<IWindowItem> {mNameTextField, mStateTextField, mStateToggleButton}, Size, Position);

            mItemList = new List<IWindowItem> {mNameTextField, mStateTextField, mStateToggleButton, mCollection};
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

        public Vector2 Position { get; set; }
        public Vector2 Size { get; }
        public bool ActiveWindow { get; set; }
    }
}
