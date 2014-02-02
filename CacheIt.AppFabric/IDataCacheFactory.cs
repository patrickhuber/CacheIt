using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.AppFabric
{
    public interface IDataCacheFactory
    {
        IDataCache GetCache(string name);
    }
}
