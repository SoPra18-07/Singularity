using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Singularity.Serialization
{
    /// <summary>
    /// A Serializable Dummy written for the Testmethod of XSerializer.cs. For further Documentation of purpose look it up in mentioned Testmethod.
    /// </summary>
    [DataContract]
    internal class SerializationDummy
    {
        [DataMember]
        private static int sCount; // defaults to 0
        [DataMember]
        private CyclicDummy mCDummy;
        [DataMember]
        private int mId;
        [DataMember]
        private List<SerializationDummy> mDummyList;
        [DataMember]
        public Vector2 mVector = new Vector2(x: 1,y: 2);


        public SerializationDummy(int randomvalue, List<SerializationDummy> list)
        {
            mId = randomvalue;
            mDummyList = list;
            mCDummy = new CyclicDummy(reference: this);
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
            Console.WriteLine(value: "This is the PrintFields method.");
            Console.WriteLine(value: "My mId: " + mId);
            Console.WriteLine(value: "The static count: " + sCount);
            Console.WriteLine(value: "The List that was given to me: " + mDummyList);
            Console.WriteLine(value: "The content of the List: ");
            for (var i = 0; i < mDummyList.Count; i++)
            {
                Console.WriteLine(value: mDummyList[index: i]);
            }
            Console.WriteLine(value: "End PrintFields method.");
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
