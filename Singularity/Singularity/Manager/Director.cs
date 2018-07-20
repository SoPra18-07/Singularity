using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.AI;
using Singularity.AI.Properties;
using Singularity.Graph.Paths;
using Singularity.Input;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Serialization;
using Singularity.Sound;
using Singularity.Utils;

namespace Singularity.Manager
{
    [DataContract]
    public sealed class Director
    {
        internal GlobalVariablesInstance GetGlobalVariablesInstance { get; set; }
        [DataMember]
        public Clock GetClock { get; private set; }
        [DataMember]
        public IdGenerator GetIdGenerator { get; private set; }

        public InputManager GetInputManager { get; private set; }

        [DataMember]
        public StoryManager GetStoryManager { get; private set; }

        [DataMember]
        public PathManager GetPathManager { get; private set; }

        public SoundManager GetSoundManager { get; }

        [DataMember]
        public MilitaryManager GetMilitaryManager { get; private set; }

        [DataMember]
        public DistributionDirector GetDistributionDirector { get; private set; }

        [DataMember]
        public UserInterfaceController GetUserInterfaceController { get; private set; }

        [DataMember]
        public DeathManager GetDeathManager { get; private set; }

        public GraphicsDeviceManager GetGraphicsDeviceManager { get; }

        public EventLog GetEventLog { get; }

        public IScreenManager GetScreenManager { get; private set; }

        public Director(ContentManager content, GraphicsDeviceManager graphics, GlobalVariablesInstance globalVariablesInstance)
        {
            GetGlobalVariablesInstance = globalVariablesInstance;
            GetClock = new Clock();
            GetIdGenerator = new IdGenerator();
            GetSoundManager = new SoundManager();
            GetInputManager = new InputManager();
            GetStoryManager = new StoryManager(this);
            GetPathManager = new PathManager();
            GetUserInterfaceController = new UserInterfaceController(this);
            GetDistributionDirector = new DistributionDirector(this);
            GetMilitaryManager = new MilitaryManager(this);
            // GetFlockingManager = new FlockingManager(this);
            GetEventLog = new EventLog(GetUserInterfaceController, this, content);
            GetGraphicsDeviceManager = graphics;
            GetDeathManager = new DeathManager();

            GetSoundManager.LoadContent(content);
            GetSoundManager.PlaySoundTrack();

            GetStoryManager.LoadAchievements();
        }

        internal void ReloadContent(Director dir, Vector2 mapmeasurements, ContentManager content)
        {
            GetGlobalVariablesInstance = new GlobalVariablesInstance();
            GetGlobalVariablesInstance.UpdateFromStatic();
            GetClock = dir.GetClock;
            GetIdGenerator = dir.GetIdGenerator;
            GetStoryManager = dir.GetStoryManager;
            GetMilitaryManager = dir.GetMilitaryManager;
            GetPathManager = dir.GetPathManager;
            GetUserInterfaceController = dir.GetUserInterfaceController;
            GetDistributionDirector = dir.GetDistributionDirector;
            GetDistributionDirector.ReloadContent(GetUserInterfaceController);
            GetStoryManager.LoadAchievements();
            GetMilitaryManager.ReloadContent(mapmeasurements, this);
            GetStoryManager.ReloadContent(this);
        }

        internal void SaveConfig()
        {
            GetGlobalVariablesInstance.UpdateFromStatic();
            XSerializer.Save(GetGlobalVariablesInstance, @"Config.xml", true);
        }
        
        public void Update(GameTime gametime, bool isActive)
        {
            if (isActive)
            {
                GetInputManager.Update(gametime);
                GetEventLog.Update(gametime);
            }
            GetStoryManager.Update(gametime);
            GetMilitaryManager.Update(gametime);
            if (!GlobalVariables.mGameIsPaused)
            {
                GetClock.Update(gametime);
            }
            GetSoundManager.SetMediaPlayerVolume();
        }
    }
}


