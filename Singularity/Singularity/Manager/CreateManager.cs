using System.Collections.Generic;
using System.Runtime.Serialization;
using Singularity.Property;

namespace Singularity.Manager
{
    [DataContract]
    public class CreateManager
    {
        [DataMember]
        private Dictionary<ICreate, List<ICreateAt>> mToCreates;

        private Director mDirector;

        public CreateManager(Director director)
        {
            mDirector = director;
        }

        public void ReloadContent(Director director)
        {
            mDirector = director;
        }


        public void AddObject(ICreate obj, List<ICreateAt> toAdd)
        {
            mToCreates.Add(obj, toAdd);
        }

        public void ActualAdd()
        {
            foreach (var pair in mToCreates)
            {
                pair.Value.ForEach(a => a.AddObject(pair.Key));
            }
            mToCreates = new Dictionary<ICreate, List<ICreateAt>>();
        }
    }
}