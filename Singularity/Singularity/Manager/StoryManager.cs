using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.Levels;
using Singularity.Map;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Serialization;

namespace Singularity.Manager
{
    [DataContract]
    public sealed class StoryManager : IUpdate
    {

        //The statistics
        [DataMember]
        public Dictionary<string, int> Units { get; private set; }
        [DataMember]
        public Dictionary<EResourceType, int> Resources { get; private set; }
        [DataMember]
        public Dictionary<string, int> Platforms { get; private set; }

        [DataMember]
        public StructureMap StructureMap { get; set; }

        [DataMember]
        public ILevel Level { get; set; }

        //Do not serialize this, BUT also do not forget to load the achievements again after deserialization!
        private Achievements mAchievements;

        private Director mDirector;

        private IScreenManager mScreenManager;

        [DataMember]
        private LevelType mLevelType;

        public StoryManager(Director director, LevelType level = LevelType.None)
        {
            mDirector = director;

            mLevelType = level;
            LoadAchievements();

            Units = new Dictionary<string, int>
            {
                {"created", 0},
                {"lost", 0},
                {"killed", 0}
            };

            Resources = new Dictionary<EResourceType, int>
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

            Platforms = new Dictionary<string, int>
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
            Units.TryGetValue(action, out a);
            Units[action] += 1;
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
            Platforms.TryGetValue(action, out a);
            Platforms[action] += 1;
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
            Resources.TryGetValue(resource, out a);
            Resources[resource] += 1;
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
        /// If this is called the game is won
        /// </summary>
        public void Win()
        {
            mScreenManager.RemoveScreen();
            mScreenManager.AddScreen(new WinScreen(mDirector, mScreenManager));
        }

        /// <summary>
        /// If this is called the game is lost
        /// </summary>
        public void Lose()
        {
            mScreenManager.RemoveScreen();
            mScreenManager.AddScreen(new LoseScreen(mDirector, mScreenManager));
        }

        public void SetScreenManager(IScreenManager screenManager)
        {
            mScreenManager = screenManager;
        }

        public void ReloadContent(Director director)
        {
            mDirector = director;
        }
    }
}
