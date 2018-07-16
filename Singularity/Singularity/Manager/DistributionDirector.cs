using System.Collections.Generic;
using System.Runtime.Serialization;
using Singularity.PlatformActions;
using Singularity.Platforms;
using Singularity.Screen;
using Singularity.Units;

namespace Singularity.Manager
{
    /// <summary>
    /// A Manager that manages DistributionManagers. This is needed to provide multi-graph-compatibility.
    /// </summary>
    [DataContract]
    public sealed class DistributionDirector
    {
        [DataMember]
        private Dictionary<int, DistributionManager> mDMs;

        private UserInterfaceController mUserInterfaceController;

        public DistributionDirector(Director director)
        {
            mUserInterfaceController = director.GetUserInterfaceController;
            mDMs = new Dictionary<int, DistributionManager>();
        }

        /// <summary>
        /// Register a new Graph / Create a new DistributionManager for it.
        /// </summary>
        /// <param name="graphid">The graphid of the new graph.</param>
        public void AddManager(int graphid)
        {
            mDMs[graphid] = new DistributionManager(graphid);

            // update UI
            mUserInterfaceController.AddGraph(graphid);
        }

        /// <summary>
        /// Merge two Distributionmanagers and create a new one. This means the new DistributionManager will observe all Units and platforms and tasks
        /// the other two had.
        /// </summary>
        /// <param name="graphid1">The graphid of the first DistributionManager</param>
        /// <param name="graphid2">The graphid of the second DistributionManager</param>
        /// <param name="newgraphid">The graphid of the newly merged DistributionManager</param>
        public void MergeManagers(int graphid1, int graphid2, int newgraphid)
        {
            var dist1 = mDMs[graphid1];
            var dist2 = mDMs[graphid2];
            var dist3 = new DistributionManager(newgraphid);

            //Merge the joblists
            var idle = dist1.GetJobUnits(JobType.Idle);
            idle.AddRange(dist2.GetJobUnits(JobType.Idle));
            var prod = dist1.GetJobUnits(JobType.Production);
            prod.AddRange(dist2.GetJobUnits(JobType.Production));
            var logistics = dist1.GetJobUnits(JobType.Logistics);
            logistics.AddRange(dist2.GetJobUnits(JobType.Logistics));
            var construction = dist1.GetJobUnits(JobType.Construction);
            construction.AddRange(dist2.GetJobUnits(JobType.Construction));
            var defense = dist1.GetJobUnits(JobType.Defense);
            defense.AddRange(dist2.GetJobUnits(JobType.Defense));
            var manual = dist1.GetJobUnits(JobType.Manual);
            manual.AddRange(dist2.GetJobUnits(JobType.Manual));

            //Merge the PlatformLists
            var prodplatforms = dist1.GetPlatforms(false);
            prodplatforms.AddRange(dist2.GetPlatforms(false));
            var defplatforms = dist1.GetPlatforms(true);
            defplatforms.AddRange(dist2.GetPlatforms(true));

            //Merge the Tasklists
            var buildingtasks = new Queue<Task>(dist1.GetTasks(true));
            foreach (var task in dist2.GetTasks(true))
            {
                buildingtasks.Enqueue(task);
            }

            var productiontasks = new Queue<Task>(dist1.GetTasks(false));
            foreach (var task in dist2.GetTasks(false))
            {
                productiontasks.Enqueue(task);
            }

            //Merge the PlatformActions
            var actions = new List<IPlatformAction>(dist1.GetPlatformActions());
            actions.AddRange(dist2.GetPlatformActions());

            //Set up the new DistributionManager
            dist3.SetJobUnits(idle, JobType.Idle);
            dist3.SetJobUnits(prod, JobType.Production);
            dist3.SetJobUnits(construction, JobType.Construction);
            dist3.SetJobUnits(defense, JobType.Defense);
            dist3.SetJobUnits(logistics, JobType.Logistics);
            dist3.SetJobUnits(manual, JobType.Manual);

            dist3.SetPlatforms(defplatforms, true);
            dist3.SetPlatforms(prodplatforms, false);

            dist3.SetTasks(buildingtasks, true);
            dist3.SetTasks(productiontasks, false);

            dist3.SetPlatformActions(actions);
            //Remove the two old DMs and add the new DM to the Dictionary
            mDMs.Remove(graphid1);
            mDMs.Remove(graphid2);
            mDMs[newgraphid] = dist3;

            // update UI
            mUserInterfaceController.MergeGraph(newgraphid, graphid1, graphid2);
        }

        /// <summary>
        /// Split A DistributionManager in two DistributionManagers. This means after using this, there will be two DistributionManagers:
        /// An old DistributionManager existing previously and a new DistributionManager that has been split-off.
        /// </summary>
        /// <param name="oldgraphid">The graphid of the DistributionManager to split</param>
        /// <param name="newgraphid">The graphid of the split-off DistributionManager</param>
        /// <param name="platforms">The platforms of the new split-off DistributionManager</param>
        /// <param name="units">The units of the new split-off DistributionManager</param>
        /// <param name="graphIdToGraph">the structuremap's graphIdToGraph-dictionary</param>
        public void SplitManagers(int oldgraphid,
            int newgraphid,
            List<PlatformBlank> platforms,
            List<GeneralUnit> units,
            Dictionary<int, Graph.Graph> graphIdToGraph)
        {
            var olddist = mDMs[oldgraphid];
            mDMs[newgraphid] = new DistributionManager(newgraphid);
            var newdist = mDMs[newgraphid];
            foreach (var platform in platforms)
            {
                var platformcontainer = new List<PlatformBlank>();
                platformcontainer.Add(platform);
                //Only remove the platform from the old distrmanager, when its in it. That is the case only when its a defending or producing platform.
                if (platform.IsDefense() || platform.IsProduction())
                {
                    //Also removes the tasks of this platform from the oldDistributionManager
                    olddist.Unregister(platformcontainer, platform.IsDefense(), false);

                    //Only readd the platform when it was in the old distributionmanager. That is the case only when its a defending or producing platform.
                    newdist.Register(platform);
                }
                //TODO: Make somehow sure the IPlatformactions request their missing things anew, because currently they dont.

            }

            foreach (var unit in units)
            {
                olddist.Kill(unit);
                //These were already added before in the register sttuff
                if (unit.Job != JobType.Defense && unit.Job != JobType.Production)
                {
                    newdist.Register(unit, unit.Job);
                }
            }

            // update UI by "calling all graphs" - see description in UIController
            mUserInterfaceController.CallingAllGraphs(graphIdToGraph);
        }

        public DistributionManager GetManager(int graphid)
        {
             return mDMs[graphid];
        }

        public void RemoveManager(int graphId, Dictionary<int, Graph.Graph> graphIdToGraph)
        {
            mDMs.Remove(graphId);
            mUserInterfaceController.CallingAllGraphs(graphIdToGraph);
        }

        public void ReloadContent(UserInterfaceController uic)
        {
            mUserInterfaceController = uic;
        }
    }
}
