using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Levels;
using Singularity.Property;
using Singularity.Resources;
using Singularity.serialization;

namespace Singularity.StoryManager
{
    [DataContract()]
    class StoryManager : IUpdate
    {
        [DataMember()]
        private int mEnergyLevel;

        [DataMember()]
        private TimeSpan mTime;

        [DataMember()]
        private Dictionary<string, int> mUnits;

        [DataMember()]
        private Dictionary<EResourceType, int> mResources;

        [DataMember()]
        private Dictionary<string, int> mPlatforms;

        [DataMember()]
        private LevelType mLevelType;

        private Achievements mAchievements;

        public StoryManager()
        {
            mLevelType = LevelType.None;
            mEnergyLevel = 0;
            mTime = new TimeSpan(0, 0, 0, 0, 0);
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

        public void SetLevelType(LevelType leveltype)
        {
            mLevelType = leveltype;
        }
        public void LoadAchievements()
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

        public void UpdateUnits(string action)
        {
            int a;
            mUnits.TryGetValue(action, out a);
            mUnits.Add(action, a + 1);
            if (mAchievements.Replicant())
            {
                //trigger Achievement;
            }
        }

        public void UpdatePlatforms(string action)
        {
            int a;
            mPlatforms.TryGetValue(action, out a);
            mPlatforms.Add(action, a + 1);
            if (mAchievements.Skynet())
            {
                //trigger Achievement;
            }
        }

        public void UpdateResources(EResourceType resource)
        {
            int a;
            mResources.TryGetValue(resource, out a);
            mResources.Add(resource, a + 1);
        }

        public void Trash()
        {
            if (mAchievements.WallE())
            {
                //trigger Achievement;
            }
        }
        public void Update(GameTime time)
        {
            mTime = mTime.Add(time.ElapsedGameTime);
            switch (mLevelType)
            {
                case LevelType.None:
                    break;
                case LevelType.Tutorial:
                    HandleTutorial();
                    break;

            }
        }

        public void HandleTutorial()
        {
            //Trigger Infoboxes.
            //Trigger Events for tutorial.
        }

        public TimeSpan GetIngameTime()
        {
            return mTime;
        }

        public int GetEnergyLevel()
        {
            return mEnergyLevel;
        }

        public void AddEnergy(int energy)
        {
            mEnergyLevel += energy;
        }
    }
}
