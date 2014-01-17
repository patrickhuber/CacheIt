using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CacheIt.Web
{
    /// <summary>
    /// Implements only the methods needed for this library to interface with a HttpSessionState object.
    /// </summary>
    public class HttpSessionStateAdapter : IHttpSessionState
    {
        private HttpSessionStateBase sessionState;

        public HttpSessionStateAdapter(HttpSessionStateBase sessionState)
        {
            this.sessionState = sessionState;             
        }

        public object this[string key]
        {
            get { return sessionState[key]; }
            set { sessionState[key] = value; }
        }

        public System.Collections.Specialized.NameValueCollection.KeysCollection Keys 
        {
            get { return this.sessionState.Keys; }
        }

        public int Count
        {
            get { return sessionState.Count; }
        }

        public void Remove(string name)
        {
            sessionState.Remove(name);
        }
    }
}
