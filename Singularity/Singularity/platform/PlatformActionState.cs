using System;
namespace Singularity.Platform
{
    /// <summary>
    /// Each PlatformAction is in either one of the following States.
    /// </summary>
    public enum PlatformActionState
    {
        /// <summary> Available: meaning it is currently being executed, so not
        ///  available right now (the button in the ui is deactivated, and thus
        ///  cannot be clicked) <\summary>
        Available,
        /// <summary> Active: meaning it is currently being executed,
        ///  so not available right now (the button cannot be clicked) </summary>
        Active,
        /// <summary> Deactivated: meaning it's manually paused
        /// (like e.g. a factory not producing or a blueprint
        /// currently not getting built), but can be executed (unpaused),
        /// so button is clickable. </summary>
        Deactivated,
        /// <summary> Disabled: Button cannot be clicked, since the action
        /// has been disabled (might be due to missing energy,
        /// unavailability of resources or Unit Limits, etc). </summary>
        Disabled
    }
}
