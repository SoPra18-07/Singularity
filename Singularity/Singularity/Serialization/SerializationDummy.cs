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
        private static int _sCount; // defaults to 0
        [DataMember()]
        private CyclicDummy _mCDummy;
        [DataMember()]
        private int _mId;
        [DataMember()]
        private List<SerializationDummy> _mDummyList;
        [DataMember()]
        public Vector2 MVector = new Vector2(1,2);


        public SerializationDummy(int randomvalue, List<SerializationDummy> list)
        {
            _mId = randomvalue;
            _mDummyList = list;
            _mCDummy = new CyclicDummy(this);
        }

        public CyclicDummy GetDummy()
        {
            return _mCDummy;
        }

        public List<SerializationDummy> GetList()
        {
            return _mDummyList;
        }

        /// <summary>
        /// Outputs the content of the fields to the Console.
        /// </summary>
        public void PrintFields()
        {
            Console.WriteLine("This is the PrintFields method.");
            Console.WriteLine("My mId: " + _mId);
            Console.WriteLine("The static count: " + _sCount);
            Console.WriteLine("The List that was given to me: " + _mDummyList);
            Console.WriteLine("The content of the List: ");
            for (var i = 0; i < _mDummyList.Count; i++)
            {
                Console.WriteLine(_mDummyList[i]);
            }
            Console.WriteLine("End PrintFields method.");
        }

        public override string ToString()
        {
            return "My Id: " + _mId;
        }

        public void Increment()
        {
            _sCount++;
        }
    }
}
