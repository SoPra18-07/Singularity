using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Singularity.Manager
{
    [DataContract]
    public sealed class ActionManager
    {
        [DataMember]
        private Dictionary<object, Func<object, bool>> mToCreates;
        [DataMember]
        private Dictionary<object, Func<object, bool>> mToCreatesNew;


        public ActionManager()
        {
            mToCreates = new Dictionary<object, Func<object, bool>>();
            mToCreatesNew = new Dictionary<object, Func<object, bool>>();
        }
        

        public void AddObject(object obj, Func<object, bool> toAdd)
        {
            if (mToCreatesNew.ContainsKey(obj))
            {
                if (mToCreatesNew[obj] == toAdd)
                {
                    return;
                }
                mToCreatesNew[obj] = delegate(object o)
                {
                    mToCreatesNew[obj](o);
                    toAdd(o);
                    return true;
                };
            }
            mToCreatesNew.Add(obj, toAdd);
        }

        public void ActualExec()
        {
            foreach (var pair in mToCreates)
            {
                pair.Value(pair.Key);
            }
            mToCreates = new Dictionary<object, Func<object, bool>>(mToCreatesNew);
            mToCreatesNew = new Dictionary<object, Func<object, bool>>();
        }
    }
}