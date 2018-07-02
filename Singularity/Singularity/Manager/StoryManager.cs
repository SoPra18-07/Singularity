using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.Levels;
using Singularity.Map;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Serialization;

namespace Singularity.Manager
{
    [DataContract]
    public class StoryManager : IUpdate
    {
        [DataMember]
        private int mEnergyLevel;

        [DataMember]
        public TimeSpan Time { get; set; }


        //The statistics
        [DataMember]
        private Dictionary<string, int> mUnits;
        [DataMember]
        private Dictionary<EResourceType, int> mResources;
        [DataMember]
        private Dictionary<string, int> mPlatforms;

        [DataMember]
        public StructureMap StructureMap { get; set; }

        public ILevel Level { get; set; }

        //Do not serialize this, BUT also do not forget to load the achievements again after deserialization!
        private Achievements mAchievements;


        [DataMember]
        private LevelType mLevelType;

        public StoryManager(LevelType level = LevelType.None)
        {
            mLevelType = level;
            mEnergyLevel = 0;
            Time = new TimeSpan(0, 0, 0, 0, 0);
            LoadAchievements();

            mUnits = new Dictionary<string, int>
            {
                {"created", 0},
                {"lost", 0},
                {"killed", 0}
            };

            mResources = new Dictionary<EResourceType, int>
            {
                {EResourceType.Chip, 0},
                {EResourceType.Concrete, 0},
                {EResourceType.Copper, 0},
                {EResourceType.Fuel, 0},
                {EResourceType.Metal, 0},
                {EResourceType.Oil, 0},
                {EResourceType.Plastic, 0},
                {EResourceType.Sand, 0},
                {EResourceType.Silicon, 0},
                {EResourceType.Steel, 0},
                {EResourceType.Stone, 0},
                {EResourceType.Trash, 0},
                {EResourceType.Water, 0}
            };

            mPlatforms = new Dictionary<string, int>
            {
                {"created", 0},
                {"lost", 0},
                {"destroyed", 0}
            };
        }

        //This will determine what the storymanager will trigger etc.
        public void SetLevelType(LevelType leveltype, ILevel level)
        {
            mLevelType = leveltype;
            Level = level;
        }

        /// <summary>
        /// The Method to load the Achievements. The Achievements file has to be at %USERPROFILE%\Saved Games\Singularity. If no one like this exists
        /// it will just create a new one.
        /// </summary>
        internal void LoadAchievements()
        {
            var path = @"%USERPROFILE%\Saved Games\Singularity";
            path = Environment.ExpandEnvironmentVariables(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (File.Exists(path + @"\Achievements.xml"))
            {
                mAchievements = (Achievements)XSerializer.Load(path + @"\Achievements.xml");
            }
            else
            {
                mAchievements = new Achievements();
            }
        }

        /// <summary>
        /// Called when something happens regarding units, for example a new Unit has been created. This is for tracking the statistics.
        /// </summary>
        /// <param name="action">The action regarding the units that has happened. Has to be created, lost or killed</param>
        public void UpdateUnits(string action)
        {
            int a;
            mUnits.TryGetValue(action, out a);
            mUnits.Add(action, a + 1);
            if (mAchievements.Replicant())
            {
                //trigger Achievement-popup;
            }
        }

        /// <summary>
        /// Called when something happens regarding platforms, for example a new platform has been created. This is for tracking the statistics.
        /// </summary>
        /// <param name="action">The action regarding the platforms that has happened. Has to be created, lost or destroyed</param>
        public void UpdatePlatforms(string action)
        {
            int a;
            mPlatforms.TryGetValue(action, out a);
            mPlatforms.Add(action, a + 1);
            if (mAchievements.Skynet())
            {
                //trigger Achievement-popup;
            }
        }

        /// <summary>
        /// This is for tracking statistics. Has to be called when new resources have been created.
        /// </summary>
        /// <param name="resource">The resourcetype that has been created</param>
        public void UpdateResources(EResourceType resource)
        {
            int a;
            mResources.TryGetValue(resource, out a);
            mResources.Add(resource, a + 1);
        }

        /// <summary>
        /// This is for tracking statistics. Has to be called when trash is burned.
        /// </summary>
        public void Trash()
        {
            if (mAchievements.WallE())
            {
                //trigger Achievement-popup;
            }
        }

        /// <summary>
        /// Calls the corresponding handle method, for the levels. These will handle Events etc.
        /// </summary>
        /// <param name="time"></param>
        public void Update(GameTime time)
        {
            Time = Time.Add(time.ElapsedGameTime);
            switch (mLevelType)
            {
                case LevelType.None:
                    break;
                case LevelType.Tutorial:
                    HandleTutorial();
                    break;
                case LevelType.Skirmish:
                    //Not sure if theres anything to handle in Skirmish
                    //TODO: Make clear whether we want any events or such things in Skirmish
                    break;
            }
        }

        /// <summary>
        /// The handle method of the Tutorial. Triggers events, like for examples infoboxes.
        /// </summary>
        public void HandleTutorial()
        {
            //I thought about some state-system to help the handle method track at what point we are and what to trigger next.
            //Trigger Infoboxes.
            //Trigger Events for tutorial.
        }

        /// <summary>
        /// Return the Ingame time.
        /// </summary>
        /// <returns>The ingame time as TimeSpan</returns>
        public TimeSpan GetIngameTime()
        {
            return Time;
        }

        /// <summary>
        /// Return the energylevel.
        /// </summary>
        /// <returns>An integer representing the energy level. Can be negative if more energy is consumed than created.</returns>
        public int GetEnergyLevel()
        {
            return mEnergyLevel;
        }

        /// <summary>
        /// Is called when energy is consumed / created. For the consuming part just give a negative energy argument.
        /// </summary>
        /// <param name="energy">The amount of energy consumed / created. To consume/create energy this has to be negative/positive.</param>
        public void AddEnergy(int energy)
        {
            mEnergyLevel += energy;
        }
    }
}
