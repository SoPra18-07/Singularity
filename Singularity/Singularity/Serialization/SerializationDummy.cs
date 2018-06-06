using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Singularity.serialization
{
    /// <summary>
    /// A Serializable Dummy written for the Testmethod of XSerializer.cs. For further Documentation of purpose look it up in mentioned Testmethod.
    /// </summary>
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
        [DataMember()]
        public Vector2 mVector = new Vector2(1,2);


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

        /// <summary>
        /// Outputs the content of the fields to the Console.
        /// </summary>
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
