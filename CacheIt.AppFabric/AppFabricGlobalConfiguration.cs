using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CacheIt.Configuration;
using Microsoft.ApplicationServer.Caching;

namespace CacheIt.AppFabric
{
    /// <summary>
    /// Defines the appfabric global configuration
    /// </summary>
    public class AppFabricGlobalConfiguration : IGlobalConfiguration
    {
        /// <summary>
        /// The data cache factory
        /// </summary>
        private static DataCacheFactory dataCacheFactory;

        /// <summary>
        /// Gets the dat cache factory.
        /// </summary>
        /// <value>
        /// The dat cache factory.
        /// </value>
        public static DataCacheFactory DatCacheFactory 
        {
            get { return dataCacheFactory; }
        }

        /// <summary>
        /// Configures this instance.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Configure()
        {
            dataCacheFactory = new DataCacheFactory();
        }
    }
}
