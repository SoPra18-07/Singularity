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
    internal class SliderHandler
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

        public SliderHandler(ref Director director, Slider def, Slider prod, Slider constr, Slider logi)
        {
            mDirector = director;
            mDefSlider = def;
            mProductionSlider = prod;
            mConstructionSlider = constr;
            mLogisticsSlider = logi;
            mDirector.GetDistributionManager.Register(handler: this);
            mCurrentPages = new int[4];

            mDefSlider.PageMoving += DefListen;
            mConstructionSlider.PageMoving += ConstrListen;
            mLogisticsSlider.PageMoving += LogiListen;
            mProductionSlider.PageMoving += ProdListen;
        }

        /// <summary>
        /// Used to refresh the values of the sliders.
        /// </summary>
        public void Refresh()
        {
            var distr = mDirector.GetDistributionManager;
            var free = distr.GetJobCount(job: JobType.Idle);
            var total = distr.GetUnitTotal();

            mDefSlider.Pages = total;
            mProductionSlider.Pages = total;
            mConstructionSlider.Pages = total;
            mLogisticsSlider.Pages = total;

            mCurrentPages[0] = mDefSlider.CurrentPage();
            mCurrentPages[1] = mConstructionSlider.CurrentPage();
            mCurrentPages[2] = mLogisticsSlider.CurrentPage();
            mCurrentPages[3] = mProductionSlider.CurrentPage();

            mDefSlider.MaxIncrement = mDefSlider.CurrentPage() + free;
            mProductionSlider.MaxIncrement = mProductionSlider.CurrentPage() + free;
            mConstructionSlider.MaxIncrement = mConstructionSlider.CurrentPage() + free;
            mLogisticsSlider.MaxIncrement = mLogisticsSlider.CurrentPage() + free;


        }

        public void DefListen(object sender, EventArgs eventArgs, int page)
        {
            var amount = mCurrentPages[0] - page;
            //A negative value means there will be more units assigned to this job and vice versa.
            if (amount < 0)
            {
                mDirector.GetDistributionManager.DistributeJobs(oldj: JobType.Idle, newj: JobType.Defense, amount: -1 * amount);
            }
            else
            {
                mDirector.GetDistributionManager.DistributeJobs(oldj: JobType.Defense, newj: JobType.Idle, amount: amount);
            }
            Refresh();
        }

        public void ProdListen(object sender, EventArgs eventArgs, int page)
        {
            var amount = mCurrentPages[3] - page;
            //A negative value means there will be more units assigned to this job and vice versa.
            if (amount < 0)
            {
                mDirector.GetDistributionManager.DistributeJobs(oldj: JobType.Idle, newj: JobType.Production, amount: -1 * amount);
            }
            else
            {
                mDirector.GetDistributionManager.DistributeJobs(oldj: JobType.Production, newj: JobType.Idle, amount: amount);
            }
            Refresh();
        }

        public void ConstrListen(object sender, EventArgs eventArgs, int page)
        {
            var amount = mCurrentPages[1] - page;
            //A negative value means there will be more units assigned to this job and vice versa.
            if (amount < 0)
            {
                mDirector.GetDistributionManager.DistributeJobs(oldj: JobType.Idle, newj: JobType.Construction, amount: -1 * amount);
            }
            else
            {
                mDirector.GetDistributionManager.DistributeJobs(oldj: JobType.Construction, newj: JobType.Idle, amount: amount);
            }
            Refresh();
        }

        public void LogiListen(object sender, EventArgs eventArgs, int page)
        {
            var amount = mCurrentPages[2] - page;
            //A negative value means there will be more units assigned to this job and vice versa.
            if (amount < 0)
            {
                mDirector.GetDistributionManager.DistributeJobs(oldj: JobType.Idle, newj: JobType.Logistics, amount: -1 * amount);
            }
            else
            {
                mDirector.GetDistributionManager.DistributeJobs(oldj: JobType.Logistics, newj: JobType.Idle, amount: amount);
            }
            Refresh();
        }
    }
}
