using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Singularity.Platform;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.DistributionManager
{
    [DataContract()]
    public class Task
    {
        [DataMember()]
        public JobType Job { get; set; }
        
        [DataMember()]
        public PlatformBlank End { get; set; }

        [DataMember()]
        public IPlatformAction Action { get; set; }

        [DataMember()]
        public EResourceType? Getres { get; set; }

        public Task(JobType job, PlatformBlank end, EResourceType? res, IPlatformAction action)
        {
            Job = job;
            End = end;
            Getres = res;
            Action = action;
        }
    }
}
