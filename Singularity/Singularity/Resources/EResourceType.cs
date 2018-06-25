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
    public enum EResourceType
    {
        /// <summary>
        /// This represents the water resource and is a raw resource.
        /// </summary>
        Water,

        /// <summary>
        /// This represents the sand resource and is a raw resource.
        /// </summary>
        Sand,

        /// <summary>
        /// This represents the oil resource and is a raw resource.
        /// </summary>
        Oil,

        /// <summary>
        /// This represents the metal resource and is a raw resource.
        /// </summary>
        Metal,

        /// <summary>
        /// This represents the stone resource and is a raw resource.
        /// </summary>
        Stone,

        /// <summary>
        /// This represents the concrete resource and is made from sand and water.
        /// </summary>
        Concrete,

        /// <summary>
        /// This represents the silicon resource and is made from sand.
        /// </summary>
        Silicon,

        /// <summary>
        /// This represents the plastic resource and is made from oil.
        /// </summary>
        Plastic,

        /// <summary>
        /// This represents the fuel resource and is made from oil.
        /// </summary>
        Fuel,

        /// <summary>
        /// This represents the steel resource and is made from metal.
        /// </summary>
        Steel,

        /// <summary>
        /// This represents the copper resource and is made from metal.
        /// </summary>
        Copper,

        /// <summary>
        /// This represents the chip resource and is made from copper, silicon,
        /// and plastic.
        /// </summary>
        Chip,

        /// <summary>
        /// This represents a trash resource. This resource appears at a fixed rate
        /// when other resources are process by factories.
        /// </summary>
        Trash
    }
}
