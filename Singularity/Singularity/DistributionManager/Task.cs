using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
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
        public int? StartId { get; set; }

        [DataMember()]
        public int? EndId { get; set; }
        
        [DataMember()]
        public Optional<ResourceType> Getres { get; set; }

        public Task(JobType job, int? start, int? end, Optional<ResourceType> res)
        {
            Job = job;
            StartId = start;
            EndId = end;
            Getres = res;
        }
    }
}
