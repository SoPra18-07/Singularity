using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.platform
{

    [DataContract()]
    class PlatformBlank : IDraw, IUpdate
    {

        [DataMember()]
        private const int PlatformWidth = 148;
        [DataMember()]
        private const int PlatformHeight = 170;
        [DataMember()]
        private int mHealth;
        [DataMember()]
        private int mId;
        [DataMember()]
        private bool mIsBlueprint;
        [DataMember()]
        protected Action[] mActions;
        [DataMember()]
        protected Dictionary<IResources, int> mCost;
        private readonly Texture2D mSpritesheet;
        [DataMember()]
        private string mSpritename;
        [DataMember()]
        protected Dictionary<GeneralUnit, Job> mAssignedUnits;
        [DataMember()]
        protected List<IResources> mResources;
        [DataMember()]
        private Dictionary<IResources, int> mRequested;
        [DataMember()]
        public Vector2 AbsolutePosition { private get; set; }
        [DataMember()]
        public Vector2 AbsoluteSize { private get; set; }


        /// <summary>
        /// Get the assigned Units of this platform.
        /// </summary>
        /// <returns> a list containing references of the units</returns>
        public Dictionary<GeneralUnit, Job> GetAssignedUnits()
        {
            return mAssignedUnits;
        }

        /// <summary>
        /// Assign Units to this platform.
        /// </summary>
        /// <param name="unit">The unit to be assigned.</param>
        /// <param name="job">The Job to be done by the unit</param>
        public void AssignUnits(GeneralUnit unit, Job job)
        {
            mAssignedUnits.Add(unit, job);
        }

        /// <summary>
        /// Remove an Assigned Unit from the Assigned List.
        /// </summary>
        /// <param name="unit">The unit to unassign.</param>
        public void UnAssignUnits(GeneralUnit unit)
        {
            mAssignedUnits.Remove(unit);
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

        public virtual void Produce()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IDraw"/>
        public void Draw(SpriteBatch spritebatch)
        {
            // the sprite sheet is 148x1744 px, 1x12 sprites
            // The sprites have different heights so, by testing I found out the sprite is about 148x170 px
            spritebatch.Draw(
                mSpritesheet,
                new Rectangle(
                    (int) AbsolutePosition.X,
                    (int) AbsolutePosition.Y,
                    (int) AbsoluteSize.X,
                    (int) AbsoluteSize.Y),
                new Rectangle(0, 0, (int) AbsoluteSize.X, (int) AbsoluteSize.Y),
                Color.White
            );
        }

        /// <inheritdoc cref="IUpdate"/>
        public virtual void Update(GameTime t)
        {
            //TODO: implement update code
        }

        public PlatformBlank(Vector2 position, Texture2D spritesheet)
        {

            AbsolutePosition = position;
            AbsoluteSize = new Vector2(PlatformWidth, PlatformHeight);

            //default?
            mHealth = 100;

            //Waiting for PlatformActions to be completed.
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //TODO: Change the action thing.
            mActions = new Action[1];
            mActions[0] = Action.BlueprintBuild;

            mAssignedUnits = new Dictionary<GeneralUnit, Job>();

            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<IResources, int>();

            mResources = new List<IResources>();

            mSpritesheet = spritesheet;

            mIsBlueprint = true;
            mRequested = new Dictionary<IResources, int>();
          
        }
    }
}
