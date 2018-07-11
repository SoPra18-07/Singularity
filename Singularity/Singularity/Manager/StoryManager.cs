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
        public TimeSpan Time { get; set; }

        //The statistics
        [DataMember]
        public Dictionary<string, int> mUnits;
        [DataMember]
        public Dictionary<EResourceType, int> mResources;
        [DataMember]
        public Dictionary<string, int> mPlatforms;

        [DataMember]
        public StructureMap StructureMap { get; set; }

        [DataMember]
        public ILevel Level { get; set; }

        //Do not serialize this, BUT also do not forget to load the achievements again after deserialization!
        private Achievements mAchievements;


        [DataMember]
        private LevelType mLevelType;

        public StoryManager(LevelType level = LevelType.None)
        {
            mLevelType = level;
            Time = new TimeSpan(0, 0, 0, 0);
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
        /// The Method to load the Achievements. The Achievements file has to be at %USERPROFILE%\Saved Games\Singularity\Achievements. If no one like this exists
        /// it will just create a new one.
        /// </summary>
        internal void LoadAchievements()
        {
            var achievements = XSerializer.Load(@"Achievements.xml", true);
            if (achievements.IsPresent())
            {
                mAchievements = (Achievements) achievements.Get();
            }
            else
            {
                mAchievements = new Achievements();
            }

        }

        /// <summary>
        /// The Method to save the Achievements. The Achievements file will be saved to %USERPROFILE%\Saved Games\Singularity\Achievements. If no such directory
        /// exists it will create a new one. You want to call this method before serializing everything.
        /// </summary>
        internal void SaveAchievements()
        {
            XSerializer.Save(mAchievements, @"Achievements.xml" ,true);
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
            return new TimeSpan(Time.Hours, Time.Minutes, Time.Seconds);
        }
    }
}
