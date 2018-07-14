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
    public sealed class Director
    {
        [DataMember]
        public Clock Clock { get; private set; }
        [DataMember]
        public IdGenerator IdGenerator { get; private set; }

        public InputManager InputManager { get; private set; }

        [DataMember]
        public StoryManager StoryManager { get; private set; }

        [DataMember]
        public PathManager PathManager { get; private set; }

        public SoundManager SoundManager { get; }

        [DataMember]
        public MilitaryManager MilitaryManager { get; private set; }

        [DataMember]
        public DistributionDirector DistributionDirector { get; private set; }

        [DataMember]
        public UserInterfaceController UserInterfaceController { get; private set; }

        public GraphicsDeviceManager GraphicsDeviceManager { get; }

        public EventLog EventLog { get; }

        public Director(ContentManager content, GraphicsDeviceManager graphics)
        {
            Clock = new Clock();
            IdGenerator = new IdGenerator();
            SoundManager = new SoundManager();
            InputManager = new InputManager();
            StoryManager = new StoryManager();
            PathManager = new PathManager();
            UserInterfaceController = new UserInterfaceController(this);
            DistributionDirector = new DistributionDirector(this);
            MilitaryManager = new MilitaryManager(this); // TODO: Update this code if the MilitaryManager is not getting everything from the StructureMap or sth ...
                                                        // (like units telling it they exist and the like)
            EventLog = new EventLog(UserInterfaceController, this, content);
            GraphicsDeviceManager = graphics;

            SoundManager.LoadContent(content);
            SoundManager.PlaySoundTrack();
            // Dd}{_:
        }

        internal void ReloadContent(Director dir, Vector2 mapmeasurements)
        {
            Clock = dir.Clock;
            IdGenerator = dir.IdGenerator;
            StoryManager = dir.StoryManager;
            MilitaryManager = dir.MilitaryManager;
            PathManager = dir.PathManager;
            UserInterfaceController = dir.UserInterfaceController;
            DistributionDirector = dir.DistributionDirector;
            StoryManager.LoadAchievements();
            MilitaryManager.ReloadContent(mapmeasurements, this);
        }

        

        public void Update(GameTime gametime, bool isActive)
        {
            if (isActive)
            {
                InputManager.Update(gametime);
                EventLog.Update(gametime);
            }
            StoryManager.Update(gametime);
            MilitaryManager.Update(gametime);
            Clock.Update(gametime);
        }
    }
}


