using System;
using System.Collections.Generic;
using Singularity.AI.Properties;

namespace Singularity.Property
{
    internal static class GlobalVariables
    {
        private static float sEffectsVolume = 1f;
        private static float sMusicVolume = 1f;
        private static float sUiVolume = 1f;
        private static float sMasterVolume = 1f;

        public static bool FowEnabled { get; set; } = true;

        public static bool HealthBarEnabled { get; set; } = true;

        public static bool IsFullScreen { get; set; }

        public static List<Tuple<int, int>> ResolutionList { get; set; } = new List<Tuple<int, int>>
        {
            new Tuple<int, int>(960, 720),
            new Tuple<int, int>(1024, 576),
            new Tuple<int, int>(1024, 768),
            new Tuple<int, int>(1024, 800),
            new Tuple<int, int>(1152, 648),
            new Tuple<int, int>(1280, 720),
            new Tuple<int, int>(1280, 800),
            new Tuple<int, int>(1280, 960),
            new Tuple<int, int>(1280, 1024),
            new Tuple<int, int>(1366, 768),
            new Tuple<int, int>(1440, 900),
            new Tuple<int, int>(1600, 900),
            new Tuple<int, int>(1680, 1050),
            new Tuple<int, int>(1920, 1080),
            new Tuple<int, int>(1920, 1200),
            new Tuple<int, int>(2560, 1440),
            new Tuple<int, int>(2560, 1600),
            new Tuple<int, int>(3840, 2160)
        };
        public static int ChosenResolution { get; set; }

        /// <summary>
        /// Indicates whether the game is in debug mode or not
        /// </summary>
        internal static bool DebugState { get; set; }

        internal static float MasterVolume
        {
            get { return (AudioMute ? 0 : 1) * sMasterVolume; }
            set { sMasterVolume = value; }
        }

        internal static bool AudioMute { get; set; } = false;

        internal static float EffectsVolume
        {
            get { return sEffectsVolume * MasterVolume; }
            set { sEffectsVolume = value; }
        }

        internal static float MusicVolume
        {
            get { return sMusicVolume * MasterVolume; }
            set { sMusicVolume = value; }
        }

        internal static float UiVolume
        {
            get { return sUiVolume * MasterVolume; }
            set { sUiVolume = value; }
        }

        // Flag used to control the mission time.
        public static bool mGameIsPaused = false;

        public static EaiDifficulty Difficulty { get; set; } = EaiDifficulty.Easy;
    }
}
