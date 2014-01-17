using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Ioc
{
    public interface IDependencyResolver
    {
        T GetService<T>();
    }
}
