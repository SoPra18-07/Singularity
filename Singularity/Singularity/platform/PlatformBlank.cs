﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.property;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.platform
{

    internal class PlatformBlank : IDraw, IUpdate
    {
        private int mHealth;
        private int mId;
        private bool mIsBlueprint;
        private readonly Action[] mActions;
        private readonly Vector2 mPosition;
        private readonly Texture2D mSpritesheet;
        private readonly Dictionary<CUnit, Job> mAssignedUnits;
        private List<IResources> mResources;
        private Dictionary<IResources, int> mRequested;
        private readonly Dictionary<IResources, int> mCost;



        /// <summary>
        /// Get the assigned Units of this platform.
        /// </summary>
        /// <returns> a list containing references of the units</returns>
        public Dictionary<CUnit, Job> GetAssignedUnits()
        {
            return mAssignedUnits;
        }

        /// <summary>
        /// Assign Units to this platform.
        /// </summary>
        /// <param name="unit">The unit to be assigned.</param>
        /// <param name="job">The Job to be done by the unit</param>
        public void AssignUnits(CUnit unit, Job job)
        {
            mAssignedUnits.Add(unit, job);
        }

        /// <summary>
        /// Remove an Assigned Unit from the Assigned List.
        /// </summary>
        /// <param name="unit">The unit to unassign.</param>
        public void UnAssignUnits(CUnit unit)
        {
            mAssignedUnits.Remove(unit);
        }

        /// <summary>
        /// Get the Position of the platform as a 2dimensional vector.
        /// </summary>
        /// <returns>a Vector2 containing the position</returns>
        public Vector2 GetPosition()
        {
            return mPosition;
        }

        /// <summary>
        /// Get the special actions you can perform on this platform.
        /// </summary>
        /// <returns> an array with the available actions.</returns>
        public Action[] GetActions()
        {
            return mActions;
        }

        /// <summary>
        /// Perform the given action on the platform.
        /// </summary>
        /// <param name="action"> The action to be performed </param>
        /// <returns> true if it was succesfull</returns>
        public bool DoAction(Action action)
        {
            //This return is normally an if, I just had to do it this way because resharper would cry otherwise. As soon as doBlueprintBuild is implemented we can change this.
            return (action == Action.BlueprintBuild);
            //{
                //doBlueprintBuild
                //return true;
            //}

            //return false;
        }

        /// <summary>
        /// Get the requirements of resources to build this platform.
        /// </summary>
        /// <returns> a dictionary of the resources with a number telling how much of it is required</returns>
        public Dictionary<IResources, int> GetResourcesRequired()
        {
            return mCost;
        }

        /// <summary>
        /// Get the Resources on the platform.
        /// </summary>
        /// <returns> a List containing the references to the resource-objects</returns>
        public List<IResources> GetPlatformResources()
        {
            return mResources;
        }

        /// <summary>
        /// Get the health points of the platform
        /// </summary>
        /// <returns> the health points as integer</returns>
        public int GetHealth()
        {
            return mHealth;
        }

        /// <summary>
        /// Heal the platform or inflict damage on it.
        /// </summary>
        /// <param name="damage">Negative values for healing, positive for damage</param>
        public void TakeHealDamage(int damage)
        {
            mHealth += damage;
            if (mHealth <= 0)
            {
                //destroyplatform
            }
        }

        /// <summary>
        /// Add a new resource to the platform.
        /// </summary>
        /// <param name="resource"> the resource to be added to the platform </param>
        public void StoreResource(IResources resource)
        {
            mResources.Add(resource);
        }

        /// <summary>
        /// Use this method to get the resource you asked for. Removes the resource from the platform.
        /// </summary>
        /// <param name="resource">The resource you ask for</param>
        /// <returns>the resource you asked for, null otherwise.</returns>
        public IResources GetResource(IResources resource)
        {
            var index = mResources.IndexOf(resource);
            if (index < 0)
            {
                return null;
            }

            var foundresource = mResources[index];
            mResources.RemoveAt(index);
            return foundresource;
        }

        /// <summary>
        /// Get the resources that are requested and the amount of it.
        /// </summary>
        /// <returns>A dictionary containing this information.</returns>
        public Dictionary<IResources, int> GetmRequested()
        {
            return mRequested;
        }

        /// <summary>
        /// Change the Resources requested by this platform
        /// </summary>
        /// <param name="resource">the resource to be requested (or not)</param>
        /// <param name="number">the number of that resource</param>
        public void SetmRequested(IResources resource, int number)
        {
            mRequested.Add(resource, number);
        }

        public void Produce()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="Singularity.property.IDraw"/>
        public void Draw(SpriteBatch spritebatch)
        {

            // the sprite sheet is 148x1744 px, 1x12 sprites
            // The sprites have different heights so, by testing I found out the sprite is about 148x170 px
            spritebatch.Draw(
                mSpritesheet,
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

        public PlatformBlank(Vector2 position, Texture2D spritesheet)
        {
            //add boundaries check?
            mPosition = position;

            //default?
            mHealth = 100;

            //The only action available so far is BlueprintBuild.
            mActions = new Action[1];
            mActions[0] = Action.BlueprintBuild;

            mAssignedUnits = new Dictionary<CUnit, Job>();

            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<IResources, int>();

            mResources = new List<IResources>();

            mSpritesheet = spritesheet;

            mIsBlueprint = true;
            mRequested = new Dictionary<IResources, int>();
        }
    }
}
