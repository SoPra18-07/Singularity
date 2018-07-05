namespace Singularity.Property
{
    public interface IDie
    {

        /// <summary>
        /// Letting object 'Die', by removing all it's references from other things
        /// </summary>
        bool Die();
    }
}
