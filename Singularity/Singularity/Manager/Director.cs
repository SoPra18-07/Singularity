using System;
using Singularity.Graph.Paths;
using Singularity.Input;
using Singularity.Sound;

namespace Singularity.Manager
{
    public class Director
    {

        
        public Director(InputManager inputManager, StoryManager storyManager, PathManager pathManager, SoundManager soundManager, MilitaryManager militaryManager)
        {
            GetInputManager = inputManager;
            GetStoryManager = storyManager;
            GetPathManager = pathManager;
            GetSoundManager = soundManager;
            GetMilitaryManager = militaryManager;



            // Dd}{
        }

        public InputManager GetInputManager { get; }
        public StoryManager GetStoryManager { get; }
        public PathManager GetPathManager { get; }
        public SoundManager GetSoundManager { get; }
        public MilitaryManager GetMilitaryManager { get; }
    }
}


