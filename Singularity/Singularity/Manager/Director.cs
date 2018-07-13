using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph.Paths;
using Singularity.Input;
using Singularity.Screen;
using Singularity.Sound;
using Singularity.Utils;

namespace Singularity.Manager
{
    [DataContract]
    public class Director
    {

        public Director(ContentManager content, GraphicsDeviceManager graphics)
        {
            GetIdGenerator = new IdGenerator();
            GetInputManager = new InputManager();
            GetStoryManager = new StoryManager();
            GetPathManager = new PathManager();
            GetSoundManager = new SoundManager();
            GetUserInterfaceController = new UserInterfaceController(this);
            GetDistributionDirector = new DistributionDirector(this);
            GetMilitaryManager = new MilitaryManager(); // TODO: Update this code if the MilitaryManager is not getting everything from the StructureMap or sth ...
                                                        // (like units telling it they exist and the like)
            GetEventLog = new EventLog(GetUserInterfaceController, this, content);
            GetGraphicsDeviceManager = graphics;

            GetSoundManager.LoadContent(content);
            GetSoundManager.PlaySoundTrack();
            // Dd}{_:
        }

        internal void ReloadContent(Director dir, Vector2 mapmeasurements)
        {
            GetIdGenerator = dir.GetIdGenerator;
            GetStoryManager = dir.GetStoryManager;
            GetMilitaryManager = dir.GetMilitaryManager;
            GetPathManager = dir.GetPathManager;
            GetUserInterfaceController = dir.GetUserInterfaceController;
            GetDistributionDirector = dir.GetDistributionDirector;
            GetStoryManager.LoadAchievements();
            GetMilitaryManager.ReloadContent(mapmeasurements);
        }

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

        public GraphicsDeviceManager GetGraphicsDeviceManager { get; }

        public EventLog GetEventLog { get; }

        public void Update(GameTime gametime, bool isActive)
        {
            if (isActive)
            {
                GetInputManager.Update(gametime);
                GetEventLog.Update(gametime);
            }
            GetStoryManager.Update(gametime);
            GetMilitaryManager.Update(gametime);
        }
    }
}


