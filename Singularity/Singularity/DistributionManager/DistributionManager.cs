using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Singularity.Platform;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;
using Action = Singularity.Platform.Action;

namespace Singularity.DistributionManager
{
    [DataContract()]
    class DistributionManager
    {
        [DataMember()]
        private List<GeneralUnit> mIdle;
        [DataMember()]
        private List<GeneralUnit> mConstruction;
        [DataMember()]
        private List<GeneralUnit> mProduction;
        [DataMember()]
        private List<GeneralUnit> mDefense;
        [DataMember()]
        private List<GeneralUnit> mManual;

        [DataMember()]
        private Queue<Pair<Resource, IPlatformAction>> mResourceRequests;


        //An Felix: Vielleicht BuildBluePrint nicht in "ProduceResourceAction.cs" reinhauen (da hätte ich nicht danach gesucht)
        [DataMember()]
        private List<BuildBluePrint> mBlueprintBuilds;

        //An der Stelle mit Felix reden, PlatformActionProduce als abstrakte Klasse würde helfen?
        //[DataMember()]
        //private List<PlatformActionProduce> mProducingPlatforms;

        //[DataMember()]
        //private List<PlatformActionDefend> mDefensivePlatforms;

        //Alternativ könnte man auch bei den beiden Listen direkt die Platformen einsetzen?

        public void ManualAssign(GeneralUnit unit, IPlatformAction action)
        {
            //Mit Felix reden: Warum ist da überhaupt JobType drin?
            //action.AssignUnit(unit, JobType.);
        }

        public void ManualUnassign(GeneralUnit unit)
        {
            //Siehe oben
            //IPlatformAction.UnAssignUnits(unit, JobType.);
        }

        public void RequestResource(PlatformBlank platform, Resource resource)
        {
            //Wozu?
        }

        public void
    }
}
