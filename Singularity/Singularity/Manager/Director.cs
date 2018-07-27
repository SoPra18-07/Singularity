using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        private GlobalVariablesInstance GetGlobalVariablesInstance { get; set; }
        [DataMember]
        public Clock GetClock { get; set; }
        [DataMember]
        public IdGenerator GetIdGenerator { get; set; }

        public InputManager GetInputManager { get; }

        [DataMember]
        public StoryManager GetStoryManager { get; set; }

        [DataMember]
        public PathManager GetPathManager { get; set; }

        public SoundManager GetSoundManager { get; }

        [DataMember]
        public MilitaryManager GetMilitaryManager { get; set; }

        [DataMember]
        public DistributionDirector GetDistributionDirector { get; set; }

        [DataMember]
        public UserInterfaceController GetUserInterfaceController { get; private set; }

        [DataMember]
        public DeathManager GetDeathManager { get; set; }

        [DataMember]
        public ActionManager GetActionManager { get; set; }

        public GraphicsDeviceManager GetGraphicsDeviceManager { get; private set; }

        public EventLog GetEventLog { get; private set; }

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
            GetActionManager = new ActionManager();
            GetEventLog = new EventLog(GetUserInterfaceController, this, content);
            GetGraphicsDeviceManager = graphics;
            GetDeathManager = new DeathManager();

            GetSoundManager.LoadContent(content);
            GetSoundManager.PlaySoundTrack();

            GetStoryManager.LoadAchievements();
        }

        internal void ReloadContent(Director dir, Vector2 mapmeasurements, ContentManager content, GraphicsDeviceManager graphics)
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
            GetGraphicsDeviceManager = graphics;
            GetEventLog = new EventLog(GetUserInterfaceController, this, content);
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
            if (!GlobalVariables.mGameIsPaused)
            {
                GetStoryManager.Update(gametime);
                GetMilitaryManager.Update(gametime);
                GetClock.Update(gametime);
            }
            GetSoundManager.SetMediaPlayerVolume();
        }
    }
}


