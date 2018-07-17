using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Utils
{
    [DataContract]
    public sealed class Clock
    {
        [DataMember]
        private TimeSpan mIngametime;

        [DataMember]
        private TimeSpan mProduceTicker;

        [DataMember]
        private TimeSpan mShootLaserTicker;

        [DataMember]
        private TimeSpan mBfsTicker;

        public Clock()
        {
            mIngametime = new TimeSpan(0, 0, 0, 0);
            mProduceTicker = new TimeSpan(0, 0, 0);
            mShootLaserTicker = new TimeSpan(0, 0, 0, 0);
            mBfsTicker = new TimeSpan(0, 0, 0, 0);
        }

        public void Update(GameTime time)
        {
            mIngametime = mIngametime.Add(time.ElapsedGameTime);
            //The ticker goes only above 4 to give everyone the chance to produce and resets then
            if (mProduceTicker.Seconds > 4)
            {
                mProduceTicker = new TimeSpan(0, 0, 0);
            }
            //THIS determines the attackspeed of a laser tower.
            //When changing this also change the values in the towers!
            if (mShootLaserTicker.TotalMilliseconds > 1000)
            {
                mShootLaserTicker = new TimeSpan(0, 0, 0, 0);
            }
            //this is the ticker for the "banlist" for resources in the FindBegin method of the Distrmanager
            if (mBfsTicker.TotalMilliseconds > 2000)
            {
                mBfsTicker = new TimeSpan(0, 0, 0, 0);
            }

            mShootLaserTicker = mShootLaserTicker.Add(time.ElapsedGameTime);
            mProduceTicker = mProduceTicker.Add(time.ElapsedGameTime);
            mBfsTicker = mBfsTicker.Add(time.ElapsedGameTime);
        }

        public TimeSpan GetShootingLaserTime()
        {
            return mShootLaserTicker;
        }

        /// <summary>
        /// Return the Ingame time.
        /// </summary>
        /// <returns>The ingame time as TimeSpan</returns>
        public TimeSpan GetIngameTime()
        {
            return new TimeSpan(mIngametime.Hours, mIngametime.Minutes, mIngametime.Seconds);
        }

        public TimeSpan GetProduceTicker()
        {
            return mProduceTicker;
        }

        public TimeSpan GetBfsTicker()
        {
            return mBfsTicker;
        }
    }
}
