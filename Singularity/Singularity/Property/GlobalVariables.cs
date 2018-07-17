namespace Singularity.Property
{
    internal static class GlobalVariables
    {
        private static float sEffectsVolume = 1f;
        private static float sMusicVolume = 1f;
        private static float sUiVolume = 1f;
        private static float sMasterVolume = 1f;

        public static bool mFowEnabled = true;

        public static bool mHealthBarEnabled = true;

        /// <summary>
        /// Indicates whether the game is in debug mode or not
        /// </summary>
        internal static bool DebugState { get; set; } = false;

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

        internal static bool MonoSound { get; set; }
    }
}
