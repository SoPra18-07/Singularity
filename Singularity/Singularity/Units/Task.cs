using System.Runtime.Serialization;
using Singularity.Platform;
using Singularity.Resources;

namespace Singularity.Units
{
    [DataContract]
    public class Task
    {
        [DataMember]
        public JobType Job { get; set; }
        
        [DataMember]
        public PlatformBlank End { get; set; }

        [DataMember]
        public IPlatformAction Action { get; set; }

        [DataMember]
        public EResourceType? GetResource { get; set; }

        public Task(JobType job, PlatformBlank end, EResourceType? res, IPlatformAction action)
        {
            Job = job;
            End = end;
            GetResource = res;
            Action = action;
        }
    }
}
