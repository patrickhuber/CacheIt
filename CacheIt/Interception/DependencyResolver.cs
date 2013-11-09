using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Interception
{
    public class DependencyResolver
    {
        public static void SetInstance(IDependencyResolver _instance)
        {
            lock (typeof(DependencyResolver))
            {
                DependencyResolver._instance = _instance;
            }
        }

        private static IDependencyResolver _instance;
        public static IDependencyResolver Instance { get { return _instance; } }
    }
}
