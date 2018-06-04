using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.Platform
{

    public class PlatformBlank : IDraw, IUpdate
    {

        private const int PlatformWidth = 148;
        private const int PlatformHeight = 170;

        private int mHealth;
        private int mId;
        private bool mIsBlueprint;
        private readonly IPlatformAction[] mIPlatformActions;
        private readonly Texture2D mSpritesheet;
        private readonly Dictionary<GeneralUnit, JobType> mAssignedUnits;
        private List<Resource> mResources; // here we need the actual Resources
        private Dictionary<EResourceType, int> mRequested;
        private readonly Dictionary<EResourceType, int> mCost;

        public Vector2 AbsolutePosition { private get; set; }

        public Vector2 AbsoluteSize { private get; set; }


        /// <summary>
        /// Get the assigned Units of this platform.
        /// </summary>
        /// <returns> a list containing references of the units</returns>
        public Dictionary<GeneralUnit, JobType> GetAssignedUnits()
        {
            return mAssignedUnits;
        }

        /// <summary>
        /// Assign Units to this platform.
        /// </summary>
        /// <param name="unit">The unit to be assigned.</param>
        /// <param name="job">The Job to be done by the unit</param>
        public void AssignUnits(GeneralUnit unit, JobType job)
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
        /// Get the special IPlatformActions you can perform on this platform.
        /// </summary>
        /// <returns> an array with the available IPlatformActions.</returns>
        public IPlatformAction[] GetIPlatformActions()
        {
            return mIPlatformActions;
        }

        /// <summary>
        /// Perform the given PlatformAction on the platform. might need to give the ID instead HACK
        /// </summary>
        /// <param name="IPlatformAction"> The IPlatformAction to be performed </param>
        /// <returns> true if it was succesfull</returns>
        public bool DoIPlatformAction(IPlatformAction IPlatformAction)
        {
            //This return is normally an if, I just had to do it this way because resharper would cry otherwise. As soon as doBlueprintBuild is implemented we can change this.
            // return (IPlatformAction == IPlatformAction.BlueprintBuild);
            //{
            //doBlueprintBuild
            //return true;
            //}

            //return false;
            return true;
        }

        /// <summary>
        /// Get the requirements of resources to build this platform.
        /// </summary>
        /// <returns> a dictionary of the resources with a number telling how much of it is required</returns>
        public Dictionary<EResourceType, int> GetResourcesRequired()
        {
            return mCost;
        }

        /// <summary>
        /// Get the Resources on the platform.
        /// </summary>
        /// <returns> a List containing the references to the resource-objects</returns>
        public List<Resource> GetPlatformResources()
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
        public void StoreResource(Resource resource)
        {
            mResources.Add(resource);
        }

        /// <summary>
        /// Use this method to get the resource you asked for. Removes the resource from the platform.
        /// </summary>
        /// <param name="resource">The resource you ask for</param>
        /// <returns>the resource you asked for, null otherwise.</returns>
        public Resource GetResource(EResourceType resourcetype)
        {
            // var index = mResources.FindIndex(x => x.isType(resourcetype));
            // if (index < 0)
            // {
            // return null;
            // }

            // var foundresource = mResources[index];
            // mResources.RemoveAt(index);
            // return foundresource;
            return null;
        }

        /// <summary>
        /// Get the resources that are requested and the amount of it.
        /// </summary>
        /// <returns>A dictionary containing this information.</returns>
        public Dictionary<EResourceType, int> GetmRequested()
        {
            return mRequested;
        }

        /// <summary>
        /// Change the Resources requested by this platform
        /// </summary>
        /// <param name="resource">the resource to be requested (or not)</param>
        /// <param name="number">the number of that resource</param>
        public void SetmRequested(EResourceType resource, int number)
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
                new Rectangle(
                    (int) AbsolutePosition.X,
                    (int) AbsolutePosition.Y,
                    (int) AbsoluteSize.X,
                    (int) AbsoluteSize.Y),
                new Rectangle(0, 0, (int) AbsoluteSize.X, (int) AbsoluteSize.Y),
                Color.White
            );
        }

        /// <inheritdoc cref="Singularity.property.IUpdate"/>
        public void Update(GameTime t)
        {
            //TODO: implement update code
        }

        public PlatformBlank(Vector2 position, Texture2D spritesheet)
        {

            AbsolutePosition = position;
            AbsoluteSize = new Vector2(PlatformWidth, PlatformHeight);

            //default?
            mHealth = 100;

            //The only IPlatformAction available so far is BlueprintBuild.
            mIPlatformActions = new IPlatformAction[1];
            // mIPlatformActions[0] = IPlatformAction.BlueprintBuild;

            mAssignedUnits = new Dictionary<GeneralUnit, JobType>();

            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();

            mResources = new List<Resource>();

            mSpritesheet = spritesheet;

            mIsBlueprint = true;
            mRequested = new Dictionary<EResourceType, int>();
          
        }
    }
}
