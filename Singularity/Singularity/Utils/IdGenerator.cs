using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Utils
{
	public interface IID
	{
		int mId { get; }
	}
	
    public static class IdGenerator
    {
        static int sId = 0;

        public static int NextiD()
        {
            sId++;
            return sId;
        }

    }
}
