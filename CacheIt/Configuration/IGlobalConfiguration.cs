using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Configuration
{
    /// <summary>
    /// A global configuration interface used by 
    /// </summary>
    public interface IGlobalConfiguration
    {
        /// <summary>
        /// Configures this instance.
        /// </summary>
        void Configure();
    }
}
