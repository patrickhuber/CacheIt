using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CacheIt.Web
{
    public class HttpContextAdapter : IHttpContext
    {
        private HttpContextBase httpContextBase;
        public HttpContextAdapter(HttpContextBase httpContextBase)
        {
            this.httpContextBase = httpContextBase;            
        }

        public IHttpSessionState Session
        {
            get { return new HttpSessionStateAdapter(httpContextBase.Session); }
        }
    }
}
