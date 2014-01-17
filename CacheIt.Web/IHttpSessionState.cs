using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        NameValueCollection.KeysCollection Keys { get; }            
        int Count { get; }
        void Remove(string name);
    }
}
