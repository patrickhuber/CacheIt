using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Interception
{
    /// <summary>
    /// Dependency resolver interface used for default constructors to initialize dependencies.
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Returns the service of type TService
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        TService GetService<TService>();

        /// <summary>
        /// Returns the service of type serviceType
        /// </summary>
        /// <param name="tService"></param>
        /// <returns></returns>
        object GetService(Type serviceType);
    }
}
