using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
    public class DistributionManager
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
        private Queue<Pair<GeneralUnit, IPlatformAction>> mRequestedUnits;

        // An Felix: Vielleicht BuildBluePrint nicht in "ProduceResourceAction.cs" reinhauen (da hätte ich nicht danach gesucht) - muss ich eh nochmal refactorn mit PlatformBlank und jz dem hier
        [DataMember()]
        private List<BuildBluePrint> mBlueprintBuilds;

        // L:An der Stelle mit Felix reden, PlatformActionProduce als abstrakte Klasse würde helfen?
        // F:Mhm weiß nicht ob das wirklich notwendig ist ... ich mach mir mal gedanken
        // L:Zumindest ein interface würde benötigt, ich denke nicht dass der sinn hinter der sache ist für jede produzierende plattform ne extra liste mit
        // List<PlatformnamehiereinsetzenActionProduce> zu erstellen. Das gleiche mit mDefensivePlatforms
        // [DataMember()]
        // private List<PlatformActionProduce> mProducingPlatforms;

        // [DataMember()]
        // private List<PlatformActionDefend> mDefensivePlatforms;

        // Alternativ könnte man auch bei den beiden Listen direkt die Platformen einsetzen?
        // Momentan ja, aber wenn du ne plattform haben willst die (rein theoretisch) verteidigen und Produzieren gleichzeitig kann? Oder gleichzeitig KineticDefense und LaserDefense ist?
        // Aber wollen wir das? also entweder so, oder halt wie oben vorgeschlagen.
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

        /// <summary>
        /// This is called by the player, when he wants to distribute the units to certain jobs.
        /// </summary>
        /// <param name="oldj"></param>
        /// <param name="newj"></param>
        /// <param name="change"></param>
        public void DistributeJobs(JobType oldj, JobType newj, int change)
        {
            List<GeneralUnit> oldlist;
            switch (oldj)
            {
                case JobType.Construction:
                    oldlist = mConstruction;
                    break;
                case JobType.Idle:
                    oldlist = mIdle;
                    break;
                case JobType.Production:
                    oldlist = mDefense;
                    break;
                case JobType.Defense:
                    oldlist = mProduction;
                    break;
            }
            List<GeneralUnit> newlist;
            switch (newj)
            {
                case JobType.Construction:
                    newlist = mConstruction;
                    break;
                case JobType.Idle:
                    newlist = mIdle;
                    break;
                case JobType.Production:
                    newlist = mDefense;
                    break;
                case JobType.Defense:
                    newlist = mProduction;
                    break;
            }
            for (int i = change; i >= 0; i++)
            {
                if (oldlist.Count == 0)
                {
                    break;
                }
                var unassigned = oldlist.First();
                unassigned.ChangeJob(newj);
                newlist.Add(unassigned);
            }
        }
        public void ManualAssign(GeneralUnit unit, IPlatformAction action, JobType job)
        {
            throw new NotImplementedException();
            var ujob = unit.Job;
            List<GeneralUnit> oldlist;
            switch (ujob)
            {
                case JobType.Construction:
                    oldlist = mConstruction;
                    break;
                case JobType.Idle:
                    oldlist = mIdle;
                    break;
                case JobType.Production:
                    oldlist = mDefense;
                    break;
                case JobType.Defense:
                    oldlist = mProduction;
                    break;
            }

            var removeit = oldlist.Find(x => x.Equals(unit));
            oldlist.Remove(removeit);
            mManual.Add(removeit);
            action.AssignUnit(unit, job);

        }

        public void ManualUnassign(JobType job, int i)
        {
            throw new NotImplementedException();
            // I guess we need an extra method for that.
            // regarding that: actually we might want to have only the JobType and an integer .. I think I've refactored that somewhere else as well before (Platforms?)
            // I guess it was PlatformActions.
        }

        // Okay yes you're right. We want a PlatformAction here instead of a platform.
        public void RequestResource(PlatformBlank platform, Resource resource, bool isbuilding = false) 
            // explain what 'isbuilding' is for, I got confused for a sec
            // I guessed isbuilding is wether the resource requested, has been requested for building or for producing/Storing
        {
            throw new NotImplementedException();
            // Will repair request ressources or units? And what unit will be used?
            // We do not have repair yet or anytime soon.
            // In that case I guess Ill ignore it for now.
            Pair<EResourceType, IPlatformAction> request;
            if (isbuilding)
            {
                request = mBuildingResources.Dequeue();
                // GeneralUnit has to be changed (Because wrong implemented).
                // I will do that later,
                // to not mess with svn
                // var assignee = mConstruction.Find(x => x.GetTask() == Task.Idle); // no, this is wrong. you need to keep track of units with 'JobType: Idle', as well as those with 'JobType: Construction' but with nothing to do !! (they are supposed to ask for tasks frequently - but easy to get confused here :D
                // assignee.AssignedTask(Task.BuildPlatform, Pair.GetFirst(), Pair.GetSecond().Platform);
            }
            else
            {
                request = mRefiningOrStoringResources.Dequeue();
                // GeneralUnit has to be changed (Because wrong implemented).
                // I will do that later,
                // to not mess with svn
                // var assignee = mConstruction.Find(x => x.GetTask() == Task.Idle); // <- careful with Idle here. They're not assigned to do anything! (we can make that optional later on though)
                // assignee.AssignedTask(Task.BuildPlatform, Pair.GetFirst(), Pair.GetSecond().Platform);
            }
        }

        public void RequestUnits(PlatformBlank platform, JobType job)
        {
            throw new NotImplementedException();
            var request = mRequestedUnits.Dequeue();
            // Here again a reminder that a platformactionproduce interface would be nice
            // Produce, and which others? Defense and Construction also ask for units, with different JobTypes. Still, we might want the PlatformAction instead of the platform for pausing later on ...
            //
            // So, the way this is supposed to work is the following; each platformAction once(!) while being active requests units of the required JobTypes here (Logistics cannot be requested for, it's resulting from ResourceRequests). Now if the PlatformAction pauses it's refernce is getting deleted (it's asked-for units and resources) - and if it's unpaused again it's again requesting once(!) the required units. This can actually be a List with the PlatformActions-id as index or sth ...
            //
            // if (request.GetSecond().GetType() == PlatformActionProduce)
            // {
            //     var assignee = mProduction.Find(x => x.GetTask() == Task.Idle());
            // A new type of task, produce, has to be implemented
            //     assignee.AssignedTask(Task.Produce, request.GetSecond().Platform);
            // }elsif (request.GetSecond().GetType() == PlatformActionDefend)
            // {
            //     var assignee = mDefense.Find(x => x.GetTask() == Task.Idle);
            //     assignee.AssignedTask(Task.Defend, request.GetSecond().Platform);
            // }
        }

        // Do we even need that? I think the units should do that - huh? no this was supposed to be from platformId to Resources on that platform, primarily for internal use when searching resources ... if you have actual platform-references all the better (you could probably get them from the producing (and factory) PlatformActions ...)
        public List<EResourceType> PlatformRequests(PlatformBlank platform)
        {
            throw new NotImplementedException();
            //return platform.GetPlatformResources();
        }

        // Why does this have to return a Task? It should only take it into the queue
        // and thats it, shouldnt it? Furthermore the platformaction shouldnt be optional. This is regarding the architecture.
        // The unit should be optional tho, you give the unit only if there are assigned units for the platform.
        //
        // Ah, I can see where your confusion is coming from. So, the version you're thinking about is absolutely valid but needs more (and better) coding. This would be the only 'bigger' function either way. Oh, and it'd absolutely need a different structure ...
        //
        // Okay, so how was it supposed to work (in my version, if you want to implement it is for you to decide):
        // - The units (with nothing to do (idle, but not 'JobType: Idle') ask for new Tasks here. So what is needed is ... actually yes, unit is not required. So the JobType is required, to return a Task of that JobType. Also, if this unit is assigned to some specific PlatformAction (like building a Blueprint, Logistics for a certain Factory, ...), it is supposed to only get Tasks involving this PlatformAction. However, if a unit is not manually assigned somewhere, what action do you want to get here? 
        public Task RequestNewTask(JobType job, Optional<IPlatformAction> assignedAction)
        {
            throw new NotImplementedException();
            /*switch(job)

            {
                case JobType.Idle:
                    var random = Generator.randomId;
                    return new Pair<Task, int>(Task.Move, random);
            }*/
        }

        public void PausePlatformAction(IPlatformAction action)
        {
            throw new NotImplementedException();
            // Actions need a sleep method
            // No, they're just being removed from occurences in the DistributionManager. As soon as they unpause, they'll send requests for Resources and units again.
            // Ah ok I got that part
        }
    }
}
