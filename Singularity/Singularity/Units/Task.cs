using System;
using System.Runtime.Serialization;
using Singularity.Platform;
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

        public Task(JobType job, PlatformBlank? end, EResourceType? res, IPlatformAction? action)
        {
            Job = job;
            End = Optional<PlatformBlank>.Of(end);
            GetResource = res;
            Action = Optional<IPlatformAction>.Of(action);
        }
    }
}
