using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Resources;
using Singularity.screen;
using Singularity.Units;

namespace Singularity.platform
{
    internal class RegularPlatform : IPlatform
    {
        private static int sTimesCreated = 0;
        private int mHealth;
        private int mId;
        private Action[] mActions;
        private Vector2 mPosition;
        private List<CUnit> mUnits;
        private Dictionary<IResources, int> mCost;
        private List<IResources> mResources;


        /// <inheritdoc cref="Singularity.platform.IPlatform"/>
        public List<CUnit> GetAssignedUnits()
        {
            return mUnits;
        }

        /// <inheritdoc cref="Singularity.platform.IPlatform"/>
        public Vector2 GetPosition()
        {
            return mPosition;
        }

        /// <inheritdoc cref="Singularity.platform.IPlatform"/>
        public Action[] GetSpecialActions()
        {
            return mActions;
        }

        /// <inheritdoc cref="Singularity.platform.IPlatform"/>
        public bool DoSpecialAction(Action action)
        {
            //This return is normally an if, I just had to do it this way because resharper would cry otherwise. As soon as doBlueprintBuild is implemented we can change this.
            return (action == Action.BlueprintBuild);
            //{
                //doBlueprintBuild
                //return true;
            //}

            //return false;
        }

        /// <inheritdoc cref="Singularity.platform.IPlatform"/>
        public Dictionary<IResources, int> ResourcesRequired()
        {
            return mCost;
        }

        /// <inheritdoc cref="Singularity.platform.IPlatform"/>
        public List<IResources> GetPlatformResources()
        {
            return mResources;
        }

        /// <inheritdoc cref="Singularity.platform.IPlatform"/>
        public int GetHealth()
        {
            return mHealth;
        }

        /// <inheritdoc cref="Singularity.platform.IPlatform"/>
        public void SetHealth(int newHealth)
        {
            mHealth = newHealth;
        }

        /// <inheritdoc cref="Singularity.platform.IPlatform"/>
        public void Store(IResources resource)
        {
            mResources.Add(resource);
        }

        /// <inheritdoc cref="Singularity.platform.IPlatform"/>
        public void Remove(IResources resource)
        {
            mResources.Remove(resource);
        }

        /// <inheritdoc cref="Singularity.property.IDraw"/>
        /// <summary>
        /// DO NOT USE THIS! USE THE OVERLOADED VERSION!
        /// </summary>
        /// <param name="spritebatch"></param>
        public void Draw(SpriteBatch spritebatch)
        {
            //I added this, so the class would still "implement" IDraw but use the other Draw method.
            throw new System.Exception("Use the overloaded version of the draw method with two arguments (platform)!");
        }

        /// <inheritdoc cref="Singularity.property.IDraw"/>
        /// <summary>
        /// Use this method to Draw the platform.
        /// </summary>
        /// <param name="spritebatch">The spritebatch to draw the platform</param>
        /// <param name="spritesheet">The spritesheet to draw the platform</param>
        public void Draw(SpriteBatch spritebatch, Texture2D spritesheet)
        {

            // the sprite sheet is 148x1744 px, 1x12 sprites
            // The sprites have different heights so, by testing I found out the sprite is about 148x170 px
            spritebatch.Draw(
                spritesheet,
                mPosition,
                new Rectangle(0, 175, 148, 170),
                Color.White,
                0f,
                new Vector2(mPosition.X, mPosition.Y),
                1f,
                SpriteEffects.None,
                0f
            );
        }

        /// <inheritdoc cref="Singularity.property.IUpdate"/>
        public void Update(GameTime t)
        {
            throw new NotImplementedException();
        }

        public RegularPlatform(int x, int y, int health)
        {
            //add boundaries check?
            mPosition = new Vector2(x, y);
            mHealth = health;

            //The ID of the nth platform will be n.
            sTimesCreated++;
            mId = sTimesCreated;

            //The only action available so far is BlueprintBuild.
            mActions = new Action[1];
            mActions[0] = Action.BlueprintBuild;

            mUnits = new List<CUnit>();

            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<IResources, int>();

            mResources = new List<IResources>();

        }
    }
}
