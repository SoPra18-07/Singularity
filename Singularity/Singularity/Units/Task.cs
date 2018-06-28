using System.Runtime.Serialization;
using Singularity.PlatformActions;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Utils;

namespace Singularity.Units
{
    [DataContract]
    public class Task
    {
        [DataMember]
        public JobType Job { get; set; }

        [DataMember]
        public Optional<PlatformBlank> Begin { get; set; }

        [DataMember]
        public Optional<PlatformBlank> End { get; set; }

        [DataMember]
        public Optional<IPlatformAction> Action { get; set; }

        [DataMember]
        public EResourceType? GetResource { get; set; }

        public Task(JobType job, Optional<PlatformBlank> end, EResourceType? res, Optional<IPlatformAction> action)
        {
            Job = job;
            End = end;
            GetResource = res;
            Action = action;
        }
    }
}
