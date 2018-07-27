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
        private AchievementInstance mAchievements;

        private Director mDirector;

        private IScreenManager mScreenManager;

        [DataMember]
        private LevelType mLevelType;

        [DataMember]
        private string mTutorialState;

        [DataMember]
        private bool mLoadTutorialScreen;

        private TutorialScreen mTutorialScreen; // needs no serialization since it can simply be recreated when load is called

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
                mAchievements = (AchievementInstance) achievements.Get();
            }
            else
            {
                mAchievements = new AchievementInstance();
            }

            mAchievements.LoadToStatic();
        }

        /// <summary>
        /// The Method to save the Achievements. The Achievements file will be saved to %USERPROFILE%\Saved Games\Singularity\Achievements. If no such directory
        /// exists it will create a new one. You want to call this method before serializing everything.
        /// </summary>
        internal void SaveAchievements()
        {
            mAchievements.UpdateFromStatic();
            XSerializer.Save(mAchievements, @"Achievements.xml" ,true);
        }

        /// <summary>
        /// Called when something happens regarding units, for example a new Unit has been created. This is for tracking the statistics.
        /// </summary>
        /// <param name="action">The action regarding the units that has happened. Has to be created, lost or killed</param>
        public void UpdateUnits(string action)
        {
            int a;
            var temp = action;

            if (temp == "created")
            {
                Achievements.UnitsBuilt++;
            }

            if (temp == "military created")
            {
                Achievements.UnitsBuilt++;
                Achievements.MilitaryUnitsBuilt++;
                temp = "created";
            }

            Units.TryGetValue(temp, out a);
            Units[temp] += 1;

            if (Achievements.Replicant())
            {
                //trigger Achievement-popup;
            }

            if (Achievements.RateOurGame())
            {
                // trigger achievement.
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

            if (action == "created")
            {
                Achievements.PlatformsBuilt++;
            }

            if (Achievements.SelfAware())
            {
                // do something TODO
            }

            if (Achievements.Skynet())
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
            Achievements.TrashBurned++;
            if (Achievements.WallE())
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
            if (mLoadTutorialScreen)
            {
                mScreenManager.AddScreen(mTutorialScreen);
                mLoadTutorialScreen = false;
            }

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
        private void HandleTutorial()
        {
            switch (mTutorialState)
            {
                case "Beginning":
                    if (mTutorialScreen.TutorialState == "AwaitingUserAction")
                    {
                        mTutorialState = "Settler";
                    }
                    break;
                case "Settler":
                    if (mDirector.GetMilitaryManager.PlayerPlatformCount == 1)
                    {
                        mTutorialState = "UI_FirstPlatform";
                        mTutorialScreen.TutorialState = "UI_FirstPlatform";
                    }
                    break;
                case "UI_FirstPlatform":
                    if (mDirector.GetMilitaryManager.PlayerPlatformCount == 2)
                    {
                        mTutorialState = "UI_SecondPlatform";
                        mTutorialScreen.TutorialState = "UI_SecondPlatform";
                    }
                    break;
                case "UI_SecondPlatform":
                    if (mDirector.GetMilitaryManager.PlayerPlatformCount == 3)
                    {
                        mTutorialState = "CivilUnits_Build";
                        mTutorialScreen.TutorialState = "CivilUnits_Build";
                    }
                    break;
                case "CivilUnits_Build":
                    if (mDirector.GetDistributionDirector
                            .GetManager(StructureMap.GetPlatformList().First.Value.GetGraphIndex())
                            .GetNumberOfAssigned()[1] > 0)
                    {
                        mTutorialState = "UserInterface_ProducePlatform";
                        mTutorialScreen.TutorialState = "UserInterface_ProducePlatform";
                    }
                    break;
                case "UserInterface_ProducePlatform":
                    if (mDirector.GetDistributionDirector
                            .GetManager(StructureMap.GetPlatformList().First.Value.GetGraphIndex())
                            .GetNumberOfProdPlatforms() > 0)
                    {
                        mTutorialState = "Factory";
                        mTutorialScreen.TutorialState = "Factory";
                    }
                    break;
                case "Factory":
                    if (mDirector.GetDistributionDirector
                            .GetManager(StructureMap.GetPlatformList().First.Value.GetGraphIndex())
                            .GetNumberOfProdPlatforms() > 1)
                    {
                        mTutorialState = "SelectedPlatform_ActionAssignment";
                        mTutorialScreen.TutorialState = "SelectedPlatform_ActionAssignment";
                    }
                    break;
                case "SelectedPlatform_ActionAssignment":
                    if (mDirector.GetDistributionDirector
                            .GetManager(StructureMap.GetPlatformList().First.Value.GetGraphIndex())
                            .GetNumberOfAssigned()[1] < 1)
                    {
                        mTutorialState = "CivilUnits_Logistics";
                        mTutorialScreen.TutorialState = "CivilUnits_Logistics";
                    }
                    break;
                case "CivilUnits_Logistics":
                    if (mDirector.GetDistributionDirector
                            .GetManager(StructureMap.GetPlatformList().First.Value.GetGraphIndex()).GetNumberOfAssigned()[2] > 0)
                    {
                        mTutorialState = "BuildGeneralUnit";
                        mTutorialScreen.TutorialState = "BuildGeneralUnit";
                    }
                    break;
                case "BuildGeneralUnit":
                    if (mDirector.GetDistributionDirector
                            .GetManager(StructureMap.GetPlatformList().First.Value.GetGraphIndex()).GetNumberOfAssigned()[4] > 0)
                    {
                        mTutorialState = "BuildDefenseBuilding";
                        mTutorialScreen.TutorialState = "BuildDefenseBuilding";
                    }
                    break;
                case "BuildDefenseBuilding":
                    if (mDirector.GetDistributionDirector
                            .GetManager(StructureMap.GetPlatformList().First.Value.GetGraphIndex()).GetNumberOfAssigned()[3] == 0)
                    {
                        mTutorialState = "Force-Deactivation";
                        mTutorialScreen.TutorialState = "Force-Deactivation";
                    }
                    break;
                case "Force-Deactivation":
                    if (mDirector.GetDistributionDirector
                            .GetManager(StructureMap.GetPlatformList().First.Value.GetGraphIndex()).GetNumberOfAssigned()[4] == 0)
                    {
                        mTutorialState = "Powerhouse";
                        mTutorialScreen.TutorialState = "Powerhouse";
                    }
                    break;
                case "Powerhouse":
                    if (mDirector.GetDistributionDirector
                            .GetManager(StructureMap.GetPlatformList().First.Value.GetGraphIndex()).GetNumberOfAssigned()[0] > 0)
                    {
                        mTutorialState = "Barracks";
                        mTutorialScreen.TutorialState = "Barracks";
                    }
                    break;
                case "Barracks":
                    if (mDirector.GetDistributionDirector
                            .GetManager(StructureMap.GetPlatformList().First.Value.GetGraphIndex()).GetNumberOfAssigned()[2] > 2)
                    {
                        mTutorialState = "create_MilitaryUnit";
                        mTutorialScreen.TutorialState = "create_MilitaryUnit";
                    }
                    break;
                case "create_MilitaryUnit":
                    if (mDirector.GetMilitaryManager.PlayerUnitCount > 0)
                    {
                        mTutorialState = "finalPopup";
                        mTutorialScreen.TutorialState = "finalPopup";
                    }
                    break;
                case "finalPopup":
                    var assignedUnitsList = mDirector.GetDistributionDirector.GetManager(StructureMap.GetPlatformList().
                        First.Value.GetGraphIndex()).GetNumberOfAssigned();
                    if (assignedUnitsList[0] == 0 && assignedUnitsList[1] == 0 && assignedUnitsList[2] == 0 && assignedUnitsList[3] == 0)
                    {
                        mTutorialState = "EndOfTutorial";
                        mTutorialScreen.TutorialState = "EndOfTutorial  ";
                    }
                    break;
                case "EndOfTutorial":
                    mTutorialState = "noAction";
                    mScreenManager.RemoveScreen();
                    Win();
                    break;
                case "noAction":
                    break;
                default:
                    mTutorialScreen = new TutorialScreen(mDirector);
                    mTutorialScreen.TutorialState = "Beginning";
                    mTutorialState = "Beginning";
                    mScreenManager.AddScreen(mTutorialScreen);
                    break;
            }
        }

        /// <summary>
        /// If this is called the game is won
        /// </summary>
        public void Win()
        {
            if (mLevelType != LevelType.Techdemo && mLevelType != LevelType.NoWinLose)
            {
                mScreenManager.RemoveScreen();
                mScreenManager.AddScreen(new WinScreen(ref mDirector, mScreenManager));
            }
        }

        /// <summary>
        /// If this is called the game is lost
        /// </summary>
        public void Lose()
        {
            if (mLevelType != LevelType.Techdemo && mLevelType != LevelType.NoWinLose)
            {
                mScreenManager.RemoveScreen();
                mScreenManager.AddScreen(new LoseScreen(ref mDirector, mScreenManager));
            }
        }

        public void SetScreenManager(IScreenManager screenManager)
        {
            mScreenManager = screenManager;
        }

        public void ReloadContent(Director director)
        {
            mDirector = director;

            Console.Out.WriteLine(mTutorialState);

            if (Level is Tutorial)
            {
                mTutorialScreen = new TutorialScreen(mDirector) {TutorialState = mTutorialState};
                mLoadTutorialScreen = true;
            }
        }
    }
}
