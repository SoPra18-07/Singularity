using System.Runtime.Serialization;

namespace Singularity.Screen
{
    public enum ELogEventType
    {
        [EnumMember(Value = "Unit attacked")]
        UnitAttacked,

        [EnumMember(Value = "Platform attacked")]
        PlatformAttacked,

        [EnumMember(Value = "Platform built")]
        PlatformBuilt,

        [EnumMember(Value = "Unit built")]
        UnitBuilt,

        [EnumMember(Value = "INFO")]
        InfoMessage,

        [EnumMember(Value = "DEBUGGING")]
        Debugging,

        [EnumMember(Value = "Platform destroyed")]
        PlatformDestroyed,

        [EnumMember(Value = "LOW ENERGY")]
        LowEnergy
    }
}
