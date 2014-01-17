using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Ioc.Unity
{
    /// <summary>
    /// Creates a Unity IDependencyResolver
    /// </summary>
    public class UnityDepedencyResolver : IDependencyResolver
    {
        private IUnityContainer container;

        public UnityDepedencyResolver(IUnityContainer container)
        {
            this.container = container;
        }

        public T GetService<T>()
        {
            return container.Resolve<T>();
        }
    }
}