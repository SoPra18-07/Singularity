using System;
using System.Runtime.Serialization;
using Singularity.Manager;
using Singularity.Units;

namespace Singularity.Screen
{
    /// <summary>
    /// A class to handle the communication between sliders and DistributionManager.
    /// </summary>
    [DataContract]
    public sealed class SliderHandler
    {
        [DataMember]
        private Slider mDefSlider;
        [DataMember]
        private Slider mProductionSlider;
        [DataMember]
        private Slider mConstructionSlider;
        [DataMember]
        private Slider mLogisticsSlider;
        [DataMember]
        private Director mDirector;

        //This is an Array of length 4. The entrys will be (with rising index): Defense, Construction, Logistics, Production
        [DataMember]
        private int[] mCurrentPages;

        [DataMember]
        private int mCurrentGraphid;

        public SliderHandler(ref Director director, Slider def, Slider prod, Slider constr, Slider logi)
        {
            mDirector = director;
            mDefSlider = def;
            mProductionSlider = prod;
            mConstructionSlider = constr;
            mLogisticsSlider = logi;
            //TODO: CURRENTLY HARDCODED Change when implementing graphswitch
            mDirector.GetDistributionDirector.GetManager(0).Register(this);
            mCurrentPages = new int[4];

            mDefSlider.PageMoving += DefListen;
            mConstructionSlider.PageMoving += ConstrListen;
            mLogisticsSlider.PageMoving += LogiListen;
            mProductionSlider.PageMoving += ProdListen;
            Refresh();

            //TODO: this won't work, since it is not guaranteed that theres always a graph at 0
            mCurrentGraphid = 0;
        }

        /// <summary>
        /// Used to refresh the values of the sliders.
        /// </summary>
        public void Refresh()
        {
            var distr = mDirector.GetDistributionDirector.GetManager(mCurrentGraphid);
            var free = distr.GetJobCount(JobType.Idle);
            var total = distr.GetUnitTotal();

            mDefSlider.Pages = total;
            mProductionSlider.Pages = total;
            mConstructionSlider.Pages = total;
            mLogisticsSlider.Pages = total;


            mCurrentPages[0] = distr.GetJobCount(JobType.Defense);
            mCurrentPages[1] = distr.GetJobCount(JobType.Construction);
            mCurrentPages[2] = distr.GetJobCount(JobType.Logistics);
            mCurrentPages[3] = distr.GetJobCount(JobType.Production);

            //If true, there are no defending platforms
            if (distr.GetRestrictions(true))
            {
                mDefSlider.MaxIncrement = 0;
            }
            else
            {
                mDefSlider.MaxIncrement = mDefSlider.GetCurrentPage() + free;
            }

            if (distr.GetRestrictions(false))
            {
                mProductionSlider.MaxIncrement = 0;
            }
            else
            {
                mProductionSlider.MaxIncrement = mProductionSlider.GetCurrentPage() + free;
            }
            mConstructionSlider.MaxIncrement = mConstructionSlider.GetCurrentPage() + free;
            mLogisticsSlider.MaxIncrement = mLogisticsSlider.GetCurrentPage() + free;


        }

        /// <summary>
        /// This changes the Graph this SliderHandler displays.
        /// </summary>
        public void ChangeGraph(object sender, EventArgs eventArgs)
        {
            //What to listen to??
        }

        /// <summary>
        /// Used to force the sliders to take the values matching the DistributionManager. Somehow like refresh but it sets new pagevalues.
        /// </summary>
        public void ForceSliderPages()
        {
            mDefSlider.SetCurrentPage(mDirector.GetDistributionDirector.GetManager(mCurrentGraphid).GetJobCount(JobType.Defense));
            mProductionSlider.SetCurrentPage(mDirector.GetDistributionDirector.GetManager(mCurrentGraphid).GetJobCount(JobType.Production));
            mConstructionSlider.SetCurrentPage(mDirector.GetDistributionDirector.GetManager(mCurrentGraphid).GetJobCount(JobType.Construction));
            mLogisticsSlider.SetCurrentPage(mDirector.GetDistributionDirector.GetManager(mCurrentGraphid).GetJobCount(JobType.Logistics));
        }

        public void DefListen(object sender, EventArgs eventArgs, int page)
        {
            var amount = mCurrentPages[0] - page;
            //A negative value means there will be more units assigned to this job and vice versa.
            if (amount < 0)
            {
                mDirector.GetDistributionDirector.GetManager(mCurrentGraphid).DistributeJobs(JobType.Idle, JobType.Defense, -1 * amount);
            }
            else
            {
                mDirector.GetDistributionDirector.GetManager(mCurrentGraphid).DistributeJobs(JobType.Defense, JobType.Idle, amount);
            }
            Refresh();
        }

        public void ProdListen(object sender, EventArgs eventArgs, int page)
        {
            var amount = mCurrentPages[3] - page;
            //A negative value means there will be more units assigned to this job and vice versa.
            if (amount < 0)
            {
                mDirector.GetDistributionDirector.GetManager(mCurrentGraphid).DistributeJobs(JobType.Idle, JobType.Production, -1 * amount);
            }
            else
            {
                mDirector.GetDistributionDirector.GetManager(mCurrentGraphid).DistributeJobs(JobType.Production, JobType.Idle, amount);
            }
            Refresh();
        }

        public void ConstrListen(object sender, EventArgs eventArgs, int page)
        {
            var amount = mCurrentPages[1] - page;
            //A negative value means there will be more units assigned to this job and vice versa.
            if (amount < 0)
            {
                mDirector.GetDistributionDirector.GetManager(mCurrentGraphid).DistributeJobs(JobType.Idle, JobType.Construction, -1 * amount);
            }
            else
            {
                mDirector.GetDistributionDirector.GetManager(mCurrentGraphid).DistributeJobs(JobType.Construction, JobType.Idle, amount);
            }
            Refresh();
        }

        public void LogiListen(object sender, EventArgs eventArgs, int page)
        {
            var amount = mCurrentPages[2] - page;
            //A negative value means there will be more units assigned to this job and vice versa.
            if (amount < 0)
            {
                mDirector.GetDistributionDirector.GetManager(mCurrentGraphid).DistributeJobs(JobType.Idle, JobType.Logistics, -1 * amount);
            }
            else
            {
                mDirector.GetDistributionDirector.GetManager(mCurrentGraphid).DistributeJobs(JobType.Logistics, JobType.Idle, amount);
            }
            Refresh();
        }

        //TODO: this is only used for temporarily not crashing the game and keeping the graphID up to date
        public void SetGraphId(int id)
        {
            mCurrentGraphid = id;
            mDirector.GetDistributionDirector.GetManager(id).Register(this);
        }
    }
}
