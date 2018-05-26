namespace Singularity.Resources
{
    /// <summary>
    /// This enum provides all the resources there are in this game. The purpose of this enum
    /// is only to differ between different instances of the Resource class. We avoid a lot of
    /// overhead by differing between the different Resource class instances by their enum instead
    /// of creating a new different instance which at its core is doing the exact same, except for
    /// being a different resource. This way we can dynamically differ between the different instances
    /// as stated above.
    /// </summary>
    internal enum EResourceType
    {
        /// <summary>
        /// This represents the water resource.
        /// </summary>
        Water,

        /// <summary>
        /// This represents the sand resource.
        /// </summary>
        Sand,

        /// <summary>
        /// This represents the oil resource.
        /// </summary>
        Oil,

        /// <summary>
        /// This represents the metal resource.
        /// </summary>
        Metal,

        /// <summary>
        /// This represents the stone resource.
        /// </summary>
        Stone,

        /// <summary>
        /// This represents the concrete resource.
        /// </summary>
        Concrete,

        /// <summary>
        /// This represents the silicon resource.
        /// </summary>
        Silicon,

        /// <summary>
        /// This represents the plastic resource.
        /// </summary>
        Plastic,

        /// <summary>
        /// This represents the fuel resource.
        /// </summary>
        Fuel,

        /// <summary>
        /// This represents the steel resource.
        /// </summary>
        Steel,

        /// <summary>
        /// This represents the copper resource.
        /// </summary>
        Copper,

        /// <summary>
        /// This represents the chip resource.
        /// </summary>
        Chip

    };
}
