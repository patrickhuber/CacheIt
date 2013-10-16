﻿using System;
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
        private HttpContext httpContext;

        protected virtual string GetSessionKey(string variable)
        {
            return string.Format("CacheIt.Web.SessionCache.{0}.{1}", Name, variable);
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionCache"/> class.
        /// </summary>
        public SessionCache()
        {
            this.httpContext = HttpContext.Current;
            AssertPreConditions(httpContext);
            var keys = new List<string>();
        }
        
        /// <summary>
        /// Asserts the pre conditions.
        /// </summary>
        protected virtual void AssertPreConditions(HttpContext httpContext)
        {
            Assert.IsNotNull(httpContext, Strings.HttpContextIsNullExceptionMessage);
            Assert.IsNotNull(httpContext.Session, Strings.SessionIsNullException);
        }
        
        public override bool Contains(string key, string regionName = null)
        {
            throw new NotImplementedException();
        }

        public override System.Runtime.Caching.CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            throw new NotImplementedException();
        }

        public override System.Runtime.Caching.DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get { throw new NotImplementedException(); }
        }

        
        public override System.Runtime.Caching.CacheItem GetCacheItem(string key, string regionName = null)
        {
            throw new NotImplementedException();
        }

        public override long GetCount(string regionName = null)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            throw new NotImplementedException();
        }

        public override object Remove(string key, string regionName = null)
        {
            throw new NotImplementedException();
        }

        public override void Set(System.Runtime.Caching.CacheItem item, System.Runtime.Caching.CacheItemPolicy policy)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// The default session cache instance
        /// </summary>
        public static SessionCache Default = new SessionCache();
    }
}
