using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.serialization
{
    [DataContract()]
    internal class SerializationDummy
    {
        [DataMember()]
        private static int sCount = 0;
        [DataMember()]
        private CyclicDummy mCDummy;
        [DataMember()]
        private int mId;
        [DataMember()]
        private List<SerializationDummy> mDummyList;


        public SerializationDummy(int randomvalue, List<SerializationDummy> list)
        {
            mId = randomvalue;
            mDummyList = list;
            mCDummy = new CyclicDummy(this);
        }

        public CyclicDummy GetDummy()
        {
            return mCDummy;
        }

        public List<SerializationDummy> GetList()
        {
            return mDummyList;
        }

        public void PrintFields()
        {
            Console.WriteLine("This is the PrintFields method.");
            Console.WriteLine("My mId: " + mId);
            Console.WriteLine("The static count: " + sCount);
            Console.WriteLine("The List that was given to me: " + mDummyList);
            Console.WriteLine("The content of the List: ");
            for (var i = 0; i < mDummyList.Count; i++)
            {
                Console.WriteLine(mDummyList[i]);
            }
            Console.WriteLine("End PrintFields method.");
        }

        public override string ToString()
        {
            return "My Id: " + mId;
        }

        public void Increment()
        {
            sCount++;
        }
    }
}
