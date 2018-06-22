using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Singularity.Graph.Paths;
using Singularity.Input;
using Singularity.Property;
using Singularity.Sound;

namespace Singularity.Manager
{
    public class Director : IUpdate
    {
        
        public Director(ContentManager content)
        {
            GetInputManager = new InputManager();
            GetStoryManager = new StoryManager();
            GetPathManager = new PathManager();
            GetSoundManager = new SoundManager();
            GetDistributionManager = new DistributionManager();
            GetMilitaryManager = new MilitaryManager(); // TODO: Update this code if the MilitaryManager is not getting everything from the StructureMap or sth ... 
                                                        // (like units telling it they exist and the like)

            GetSoundManager.LoadContent(content);
            GetSoundManager.PlaySoundTrack();


            // Dd}{_: 
        }

        public InputManager GetInputManager { get; }
        public StoryManager GetStoryManager { get; }

        public PathManager GetPathManager { get; }

        public SoundManager GetSoundManager { get; }
        public MilitaryManager GetMilitaryManager { get; }
        public DistributionManager GetDistributionManager { get; }


        public void Update(GameTime gametime)
        {
            GetInputManager.Update(gametime);
            GetStoryManager.Update(gametime);
        }
    }
}


