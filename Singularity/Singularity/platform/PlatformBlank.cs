﻿using System;
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
    internal class PlatformBlank : IDraw, IUpdate, ISpatial
    {
        [DataMember()]
        private EPlatformType mType = EPlatformType.Blank;
        [DataMember()]
        private const int PlatformWidth = 148;
        [DataMember()]
        private const int PlatformHeight = 172;
        [DataMember()]
        private int mHealth;
        [DataMember()]
        private int mId;
        [DataMember()]
        private bool mIsBlueprint;
        [DataMember()]
        protected Action[] mActions;
        [DataMember()]
        protected Dictionary<IResource, int> mCost;
        private readonly Texture2D mSpritesheet;
        [DataMember()]
        protected string mSpritename;
        [DataMember()]
        protected Dictionary<GeneralUnit, Job> mAssignedUnits;
        [DataMember()]
        protected List<IResource> mResources;
        [DataMember()]
        private Dictionary<IResource, int> mRequested;
        [DataMember()]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember()]
        public Vector2 AbsoluteSize { get; set; }
        [DataMember()]
        public Vector2 RelativePosition { get; set; }
        [DataMember()]
        public Vector2 RelativeSize { get; set; }


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
        public Dictionary<IResource, int> GetResourcesRequired()
        {
            return mCost;
        }

        /// <summary>
        /// Get the Resources on the platform.
        /// </summary>
        /// <returns> a List containing the references to the resource-objects</returns>
        public List<IResource> GetPlatformResources()
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
        public void StoreResource(IResource resource)
        {
            mResources.Add(resource);
        }

        /// <summary>
        /// Use this method to get the resource you asked for. Removes the resource from the platform.
        /// </summary>
        /// <param name="resource">The resource you ask for</param>
        /// <returns>the resource you asked for, null otherwise.</returns>
        public IResource GetResource(IResource resource)
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
        public Dictionary<IResource, int> GetmRequested()
        {
            return mRequested;
        }

        /// <summary>
        /// Change the Resources requested by this platform
        /// </summary>
        /// <param name="resource">the resource to be requested (or not)</param>
        /// <param name="number">the number of that resource</param>
        public void SetmRequested(IResource resource, int number)
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
            var position = 0;
            var sheet = "b"; // b stands for blank, c for cone, cyl for Cylindrical and d for Dome
            switch (mType)
            {
                case EPlatformType.Blank:
                    break;
                case EPlatformType.Energy:
                    sheet = "d";
                    break;
                case EPlatformType.Factory:
                    position = 1;
                    sheet = "d";
                    break;
                case EPlatformType.Junkyard:
                    position = 2;
                    sheet = "d";
                    break;
                case EPlatformType.Mine:
                    position = 3;
                    sheet = "d";
                    break;
                case EPlatformType.Packaging:
                    position = 4;
                    sheet = "d";
                    break;
                case EPlatformType.Quarry:
                    position = 5;
                    sheet = "d";
                    break;
                case EPlatformType.Storage:
                    position = 6;
                    sheet = "d";
                    break;
                case EPlatformType.Well:
                    position = 7;
                    sheet = "d";
                    break;
                case EPlatformType.Kinetic:
                    sheet = "c";
                    break;
                case EPlatformType.Laser:
                    sheet = "c";
                    position = 1;
                    break;
                case EPlatformType.Barracks:
                    sheet = "cyl";
                    break;
                case EPlatformType.Command:
                    sheet = "cyl";
                    position = 1;
                    break;
            }
            spritebatch.Draw(mSpritesheet,
                new Rectangle(
                    (int)AbsolutePosition.X,
                    (int)AbsolutePosition.Y,
                    (int)AbsoluteSize.X,
                    (int)AbsoluteSize.Y),
                new Rectangle(0, 0, (int)AbsoluteSize.X, (int)AbsoluteSize.Y),
                Color.White,
                0f, 
                Vector2.Zero, SpriteEffects.None, LayerConstants.PlatformLayer);
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
            mCost = new Dictionary<IResource, int>();

            mResources = new List<IResource>();

            mSpritesheet = spritesheet;

            mIsBlueprint = true;
            mRequested = new Dictionary<IResource, int>();
          
        }
    }
}
