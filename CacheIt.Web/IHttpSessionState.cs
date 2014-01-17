using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Web
{
    /// <summary>
    /// Exposes an interface only for the methods needed by this library to interface with a HttpSessionState object.
    /// </summary>
    public interface IHttpSessionState
    {
        object this[string key] { get; set; }
        bool Contains(string key);
        int Count { get; }
        void Remove(string name);
    }
}
