﻿using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph.Paths;
using Singularity.Input;
using Singularity.Screen;
using Singularity.Sound;

namespace Singularity.Manager
{
    public class Director
    {

        public Director(ContentManager content, GraphicsDeviceManager graphics)
        {
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

        public InputManager GetInputManager { get; }
        public StoryManager GetStoryManager { get; }

        public PathManager GetPathManager { get; }

        public SoundManager GetSoundManager { get; }
        public MilitaryManager GetMilitaryManager { get; }
        public DistributionDirector GetDistributionDirector { get; }

        public UserInterfaceController GetUserInterfaceController { get; }

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


