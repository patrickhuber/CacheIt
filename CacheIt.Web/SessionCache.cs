using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CacheIt.Diagnostics;
using System.Web;
using System.Runtime.Caching;

namespace CacheIt.Web
{
    /// <summary>
    /// Implements a session cache
    /// </summary>
    public class SessionCache
        : CacheBase
    {
        private IHttpContext httpContext;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionCache"/> class.
        /// </summary>
        public SessionCache(IHttpContext httpContext)
        {
            this.httpContext = httpContext;
            AssertPreConditions(this.httpContext);
            var keys = new List<string>();
        }
        
        /// <summary>
        /// Asserts the pre conditions.
        /// </summary>
        protected virtual void AssertPreConditions(IHttpContext httpContext)
        {
            Assert.IsNotNull(httpContext, Strings.HttpContextIsNullExceptionMessage);
            Assert.IsNotNull(httpContext.Session, Strings.SessionIsNullException);
        }

        protected T GetVariable<T>(string variableName, Func<T> resolver)
        {
            string key = GetSessionKey(variableName);
            object value = httpContext.Session[key];
            if (value == null)
                value = httpContext.Session[key] = resolver();
            if (!typeof(T).IsInstanceOfType(value))
                return default(T);
            return (T)value;
        }

        protected virtual string GetSessionKey(string variable)
        {
            return string.Format("CacheIt.Web.SessionCache.{0}.{1}", Name, variable);
        }        

        protected IDictionary<string, CacheItemPolicy> CacheItemPolicies
        {
            get 
            {
                return GetVariable<IDictionary<string, CacheItemPolicy>>(
                    "CacheItemPolicies", 
                    () => new Dictionary<string, CacheItemPolicy>());
            }
        }

        protected IList<string> Keys
        {
            get
            {
                return GetVariable<IList<string>>(
                    "Keys",
                    () => new List<string>());
            }
        }
                
        public override bool Contains(string key, string regionName = null)
        {
            AssertRegionNameIsSupported(regionName);
            foreach (string sessionKey in httpContext.Session.Keys)
                if (sessionKey == key)
                    return true;
            return false;
        }

        public override System.Runtime.Caching.CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            AssertChangeMonitorIsSupported();
            return null;
        }

        public override System.Runtime.Caching.DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get { return DefaultCacheCapabilities.None; }
        }
        
        public override System.Runtime.Caching.CacheItem GetCacheItem(string key, string regionName = null)
        {
            throw new NotImplementedException();
        }

        public override long GetCount(string regionName = null)
        {
            return httpContext.Session.Count;
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (string key in httpContext.Session.Keys)
                yield return new KeyValuePair<string, object>(key, httpContext.Session[key]);
        }

        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            var dictionary = new Dictionary<string, object>();
            foreach(string key in keys)
            {
                object value = httpContext.Session[key];
                if (value != null)
                    dictionary.Add(key, value);
            }
            return dictionary;
        }

        public override object Remove(string key, string regionName = null)
        {
            AssertRegionNameIsSupported(regionName);
            var instance = Get(key, regionName);
            httpContext.Session.Remove(key);
            return instance;
        }

        public override void Set(System.Runtime.Caching.CacheItem item, System.Runtime.Caching.CacheItemPolicy policy)
        {
            httpContext.Session[item.Key] = item;
        }
    }
}
