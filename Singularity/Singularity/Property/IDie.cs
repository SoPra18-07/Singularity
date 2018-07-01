using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
