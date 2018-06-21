using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Platform
{
    [DataContract()]
    public class PlatformBlank : IRevealing, INode, ICollider

    {

        private List<IEdge> _mInwardsEdges;

        private List<IEdge> _mOutwardsEdges;

        [DataMember()]
        protected EPlatformType MType = EPlatformType.Blank;
        [DataMember()]
        private const int PlatformWidth = 148;
        [DataMember()]
        private const int PlatformHeight = 172;
        [DataMember()]
        private int _mHealth;
        [DataMember()]
        private int _mId;
        [DataMember()]
        protected bool MIsBlueprint;
        [DataMember()]
        protected Dictionary<EResourceType, int> MCost;
        [DataMember()]
        protected IPlatformAction[] MiPlatformActions;
        private readonly Texture2D _mSpritesheet;
        [DataMember()]
        protected string MSpritename;
        [DataMember()]
        protected Dictionary<GeneralUnit, JobType> MAssignedUnits;
        [DataMember()]
        protected List<Resource> MResources;
        [DataMember()]
        protected Dictionary<EResourceType, int> MRequested;

        public Vector2 Center { get; set; }

        public int RevelationRadius { get; private set; }

        public Rectangle AbsBounds { get; private set; }

        public bool Moved { get; private set; }

        public int Id { get; private set; }

        internal Vector2 GetLocation()
        {
            throw new NotImplementedException();
        }

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
        public Dictionary<GeneralUnit, JobType> GetAssignedUnits()
        {
            return MAssignedUnits;
        }

        /// <summary>
        /// Assign Units to this platform.
        /// </summary>
        /// <param name="unit">The unit to be assigned.</param>
        /// <param name="job">The Job to be done by the unit</param>
        public void AssignUnits(GeneralUnit unit, JobType job)
        {
            MAssignedUnits.Add(unit, job);
        }

        /// <summary>
        /// Remove an Assigned Unit from the Assigned List.
        /// </summary>
        /// <param name="unit">The unit to unassign.</param>
        public void UnAssignUnits(GeneralUnit unit)
        {
            MAssignedUnits.Remove(unit);
        }

        /// <summary>
        /// Get the special IPlatformActions you can perform on this platform.
        /// </summary>
        /// <returns> an array with the available IPlatformActions.</returns>
        public IPlatformAction[] GetIPlatformActions()
        {
            return MiPlatformActions;
        }

        /// <summary>
        /// Perform the given PlatformAction on the platform.
        /// </summary>
        /// <param name="platformAction"> The IPlatformAction to be performed </param>
        /// <returns> true if it was succesfull</returns>
        public bool DoIPlatformAction(IPlatformAction platformAction)
        {
            // FIXME might need to give the ID instead
            // This return is normally an if, I just had to do it this way because resharper would cry otherwise.
            // As soon as doBlueprintBuild is implemented we can change this.
            // return (IPlatformAction == IPlatformAction.BlueprintBuild);
            // {
            // doBlueprintBuild
            // return true;
            // }

            //return false;
            return true;
        }

        /// <summary>
        /// Get the requirements of resources to build this platform.
        /// </summary>
        /// <returns> a dictionary of the resources with a number telling how much of it is required</returns>
        public Dictionary<EResourceType, int> GetResourcesRequired()
        {
            return MCost;
        }

        /// <summary>
        /// Get the Resources on the platform.
        /// </summary>
        /// <returns> a List containing the references to the resource-objects</returns>
        public List<Resource> GetPlatformResources()
        {
            return MResources;
        }

        /// <summary>
        /// Get the health points of the platform
        /// </summary>
        /// <returns> the health points as integer</returns>
        public int GetHealth()
        {
            return _mHealth;
        }

        /// <summary>
        /// Heal the platform or inflict damage on it.
        /// </summary>
        /// <param name="damage">Negative values for healing, positive for damage</param>
        public void TakeHealDamage(int damage)
        {
            _mHealth += damage;
            if (_mHealth <= 0)
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
            MResources.Add(resource);
        }

        /// <summary>
        /// Use this method to get the resource you asked for. Removes the resource from the platform.
        /// </summary>
        /// <param name="resourcetype">The resource you ask for</param>
        /// <returns>the resource you asked for, null otherwise.</returns>
        public MapResource GetResource(EResourceType resourcetype)
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
            return MRequested;
        }

        /// <summary>
        /// Change the Resources requested by this platform
        /// </summary>
        /// <param name="resource">the resource to be requested (or not)</param>
        /// <param name="number">the number of that resource</param>
        public void SetmRequested(EResourceType resource, int number)
        {
            MRequested.Add(resource, number);
        }

        public virtual void Produce()
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc cref="Singularity.Property.IDraw"/>
        public void Draw(SpriteBatch spritebatch)
        {
            var position = 0;
            var sheet = "b"; // b stands for blank, c for cone or cylindrical and d for Dome
            switch (MType)
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
                    sheet = "c";
                    break;
                case EPlatformType.Command:
                    sheet = "c";
                    position = 1;
                    break;
            }

            switch (sheet)
            {
                case "b":
                    spritebatch.Draw(_mSpritesheet,
                        new Rectangle(
                            (int) AbsolutePosition.X,
                            (int) AbsolutePosition.Y,
                            (int) AbsoluteSize.X,
                            (int) AbsoluteSize.Y),
                        new Rectangle(0, 0, (int) AbsoluteSize.X, (int) AbsoluteSize.Y),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        LayerConstants.PlatformLayer);
                    break;
                case "d":
                    spritebatch.Draw(_mSpritesheet,
                        new Rectangle(
                            (int)AbsolutePosition.X,
                            (int)AbsolutePosition.Y,
                            (int)AbsoluteSize.X,
                            (int)AbsoluteSize.Y),
                        new Rectangle(position % 4 * (int)AbsoluteSize.X, position / 4 * (int)AbsoluteSize.Y, (int)AbsoluteSize.X, (int)AbsoluteSize.Y),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        LayerConstants.PlatformLayer);
                    break;
                case "c":
                    spritebatch.Draw(_mSpritesheet,
                        new Rectangle(
                            (int)AbsolutePosition.X,
                            (int)AbsolutePosition.Y,
                            (int)AbsoluteSize.X,
                            (int)AbsoluteSize.Y),
                        new Rectangle((int)AbsoluteSize.X, position * (int)AbsoluteSize.Y, (int)AbsoluteSize.X, (int)AbsoluteSize.Y),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        LayerConstants.PlatformLayer);
                    break;

            }
        }

        /// <inheritdoc cref="Singularity.Property.IUpdate"/>
        public void Update(GameTime t)
        {

        }

        public PlatformBlank(Vector2 position, Texture2D spritesheet, Vector2 center = new Vector2())
        {

            Id = IdGenerator.NextiD();

            _mInwardsEdges = new List<IEdge>();
            _mOutwardsEdges = new List<IEdge>();

            AbsolutePosition = position;
            AbsoluteSize = new Vector2(PlatformWidth, PlatformHeight);

            //default?
            _mHealth = 100;

            //Waiting for PlatformActions to be completed.
            //Something like "Hello Distributionmanager I exist now(GiveBlueprint)"
            //The only IPlatformAction available so far is BlueprintBuild.
            MiPlatformActions = new IPlatformAction[1];
            //mIPlatformActions[0] = IPlatformAction.BlueprintBuild;

            MAssignedUnits = new Dictionary<GeneralUnit, JobType>();

            //Add Costs of the platform here if you got them.
            MCost = new Dictionary<EResourceType, int>();

            MResources = new List<Resource>();

            _mSpritesheet = spritesheet;
            MSpritename = "PlatformBasic";

            MIsBlueprint = true;
            MRequested = new Dictionary<EResourceType, int>();

            RevelationRadius = (int)AbsoluteSize.Y;
            AbsBounds = new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);
            Moved = false;

            if (center == Vector2.Zero)
            {
                // no value was specified so just use the platform blank implementation.
                Center = new Vector2(AbsolutePosition.X + PlatformWidth / 2, AbsolutePosition.Y + PlatformHeight - 36);
            }
            else
            {
                //value was given by subclass thus take that
                Center = center;
            }

        }

        public bool PlatformHasSpace()
        {
            return MResources.Count < 10;
        }

        public void AddEdge(IEdge edge, EEdgeFacing facing)
        {
            if (facing == EEdgeFacing.Inwards)
            {
                _mInwardsEdges.Add(edge);
                return;
            }
            _mOutwardsEdges.Add(edge);

        }

        public void RemoveEdge(IEdge edge, EEdgeFacing facing)
        {
            if (facing == EEdgeFacing.Inwards)
            {
                _mInwardsEdges.Remove(edge);
                return;
            }
            _mOutwardsEdges.Remove(edge);

        }

        public IEnumerable<IEdge> GetOutwardsEdges()
        {
            return _mOutwardsEdges;
        }

        public IEnumerable<IEdge> GetInwardsEdges()
        {
            return _mInwardsEdges;
        }
    }
}
