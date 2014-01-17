using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Ioc
{
    /// <summary>
    /// Provides a static interface for the dependency resolver
    /// </summary>
    public static class DependencyResolver
    {
        /// <summary>
        /// The dependency resolver instance
        /// </summary>
        private static IDependencyResolver dependencyResolverInstance;

        /// <summary>
        /// Sets the current dependency resolver.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        public static void SetResolver(IDependencyResolver resolver)
        {
            lock (typeof(DependencyResolver))
            {
                dependencyResolverInstance = resolver;
            }
        }

        /// <summary>
        /// Gets the current dependency resolver instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static IDependencyResolver Instance
        {
            get { return dependencyResolverInstance; }
        }
    }
}
