using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Singularity.AI.Properties;

namespace Singularity.Property
{
    [DataContract]
    public sealed class GlobalVariablesInstance
    {
        [DataMember]
        public float EffectsVolume { get; set; } = 1f;

        [DataMember]
        public float MusicVolume { get; set; } = 1f;

        [DataMember]
        public float UiVolume { get; set; }= 1f;

        [DataMember]
        public float MasterVolume { get; set; }= 1f;

        [DataMember]
        public bool FowEnabled { get; set; } = true;

        [DataMember]
        public bool HealthBarEnabled { get; set; } = true;

        [DataMember]
        public bool IsFullScreen { get; set; }

        [DataMember]
        public int ChosenResolution { get; set; }

        [DataMember]
        internal bool AudioMute { get; set; }
        [DataMember]
        internal EaiDifficulty Difficulty { get; set; }

        public void LoadToStatic()
        {
            GlobalVariables.EffectsVolume = EffectsVolume;
            GlobalVariables.MusicVolume = MusicVolume;
            GlobalVariables.UiVolume = UiVolume;
            GlobalVariables.MasterVolume = MasterVolume;
            GlobalVariables.FowEnabled = FowEnabled;
            GlobalVariables.HealthBarEnabled = HealthBarEnabled;
            GlobalVariables.IsFullScreen = IsFullScreen;
            GlobalVariables.ChosenResolution = ChosenResolution;
            GlobalVariables.AudioMute = AudioMute;
            GlobalVariables.Difficulty = Difficulty;
        }

        public void UpdateFromStatic()
        {
            EffectsVolume = GlobalVariables.EffectsVolume;
            MusicVolume = GlobalVariables.MusicVolume;
            UiVolume = GlobalVariables.UiVolume;
            MasterVolume = GlobalVariables.MasterVolume;
            FowEnabled = GlobalVariables.FowEnabled;
            HealthBarEnabled = GlobalVariables.HealthBarEnabled;
            IsFullScreen = GlobalVariables.IsFullScreen;
            ChosenResolution = GlobalVariables.ChosenResolution;
            AudioMute = GlobalVariables.AudioMute;
            Difficulty = GlobalVariables.Difficulty;
        }
    }
}
