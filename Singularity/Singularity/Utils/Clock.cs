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

        public Clock()
        {
            mIngametime = new TimeSpan(0, 0, 0, 0);
            mProduceTicker = new TimeSpan(0, 0, 0);
        }

        public void Update(GameTime time)
        {
            mIngametime = mIngametime.Add(time.ElapsedGameTime);
            if (mProduceTicker.Seconds > 4)
            {
                mProduceTicker = new TimeSpan(0, 0, 0);
            }

            mProduceTicker = mProduceTicker.Add(time.ElapsedGameTime);
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
    }
}
