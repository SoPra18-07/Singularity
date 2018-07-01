using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using Singularity.Screen.ScreenClasses;

namespace Singularity.Levels
{
    public interface ILevel
    {
        [DataMember]
        GameScreen GameScreen { get; set; }
    }
}