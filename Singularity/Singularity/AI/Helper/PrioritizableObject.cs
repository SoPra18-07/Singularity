﻿using System;
using System.Runtime.Serialization;

namespace Singularity.AI.Helper
{

    /// <summary>
    /// Provides a container for objects and a float, which can practically be used to define an
    /// order on these objects (priority queue, heaps, etc.).
    /// </summary>
    [DataContract]
    public sealed class PrioritizableObject<T> : IComparable
    {
        /// <summary>
        /// A float depicting the prioritization of the object, where higher means higher priority.
        /// </summary>
        [DataMember]
        private float mPrioritization;

        /// <summary>
        /// The object on which the "prioritization" order is defined
        /// </summary>
        [DataMember]
        private readonly T mObjectToPrioritize;

        public PrioritizableObject(T objectToPrioritize, float prioritization)
        {
            mPrioritization = prioritization;
            mObjectToPrioritize = objectToPrioritize;
        }

        public T GetObject()
        {
            return mObjectToPrioritize;
        }

        public void SetPrioritization(float newPrioritization)
        {
            mPrioritization = newPrioritization;
        }

        public float GetPrioritization()
        {
            return mPrioritization;
        }

        public int CompareTo(object obj)
        {
            var asThis = obj as PrioritizableObject<T>;

            if (asThis == null)
            {
                return 0;
            }

            if (asThis.GetPrioritization() < GetPrioritization())
            {
                return -1;
            }

            return asThis.GetPrioritization() > GetPrioritization() ? 1 : 0;
        }
    }
}
