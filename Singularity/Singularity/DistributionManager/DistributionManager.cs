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
        private Queue<Pair<EResourceType, IPlatformAction>> mBuildingResources;
        [DataMember()]
        private Queue<Pair<EResourceType, IPlatformAction>> mRefiningOrStoringResources;
        [DataMember()]
        private Queue<Pair<GeneralUnit, IPlatformAction>> mReque

        //An Felix: Vielleicht BuildBluePrint nicht in "ProduceResourceAction.cs" reinhauen (da hätte ich nicht danach gesucht)
        [DataMember()]
        private List<BuildBluePrint> mBlueprintBuilds;

        //An der Stelle mit Felix reden, PlatformActionProduce als abstrakte Klasse würde helfen?
        //[DataMember()]
        //private List<PlatformActionProduce> mProducingPlatforms;

        //[DataMember()]
        //private List<PlatformActionDefend> mDefensivePlatforms;

        //Alternativ könnte man auch bei den beiden Listen direkt die Platformen einsetzen?

        public DistributionManager()
        { 
            mIdle = new List<GeneralUnit>();
            mConstruction = new List<GeneralUnit>();
            mProduction = new List<GeneralUnit>();
            mDefense = new List<GeneralUnit>();
            mManual = new List<GeneralUnit>();

            mBuildingResources = new Queue<Pair<EResourceType, IPlatformAction>>();
            mRefiningOrStoringResources = new Queue<Pair<EResourceType, IPlatformAction>>();
            mBlueprintBuilds = new List<BuildBluePrint>();
        }

        public void ManualAssign(GeneralUnit unit, IPlatformAction action, JobType job)
        {
            action.AssignUnit(unit, job);
        }

        public void ManualUnassign(GeneralUnit unit)
        {
            throw new NotImplementedException();
            //I guess we need an extra method for that.
        }

        public void RequestResource(PlatformBlank platform, Resource resource, bool isbuilding = false)
        {
            Pair<EResourceType, IPlatformAction> request;
            if (isbuilding)
            {
                request = mBuildingResources.Dequeue();
                //GeneralUnit has to be changed (Because wrong implemented).
                //I will do that later,
                //to not mess with svn
                //var assignee = mConstruction.Find(x => x.GetTask() == Task.Idle);
                //assignee.AssignedTask(Task.BuildPlatform, Pair.GetFirst(), Pair.GetSecond().Platform);
            }
            else
            {
                request = mRefiningOrStoringResources.Dequeue();
                //GeneralUnit has to be changed (Because wrong implemented).
                //I will do that later,
                //to not mess with svn
                //var assignee = mConstruction.Find(x => x.GetTask() == Task.Idle);
                //assignee.AssignedTask(Task.BuildPlatform, Pair.GetFirst(), Pair.GetSecond().Platform);
            }
        }

        public void RequestUnits(PlatformBlank platform, JobType job)
        {
            throw new NotImplementedException();
            var request = m
        }
    }
}
