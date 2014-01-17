using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Web
{
    /// <summary>
    /// Exposes an interface only for the methods needed by this library to interface with a HttpContext object.
    /// </summary>
    public interface IHttpContext
    {
        IHttpSessionState Session { get; }
    }
}
