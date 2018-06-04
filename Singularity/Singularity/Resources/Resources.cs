namespace Singularity.Resources
{
    public class Resource
    {
        internal EResourceType mType;
        private EResourceType eResourceType;

        public Resource(EResourceType eResourceType)
        {
            this.eResourceType = eResourceType;
        }
    }
}
