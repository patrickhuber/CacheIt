using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CacheIt.IO
{
    /// <summary>
    /// Factory interface for creating streams. Should help with dependency on a particular stream class.
    /// </summary>
    public interface IStreamFactory
    {
        /// <summary>
        /// Creates a stream
        /// </summary>
        /// <returns>the stream that was created.</returns>
        Stream Create();
    }
}
