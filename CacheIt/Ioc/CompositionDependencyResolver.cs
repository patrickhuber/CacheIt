using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace CacheIt.Ioc
{
    public class CompositionDependencyResolver : IDependencyResolver
    {
        private CompositionContainer compositionContainer;

        public CompositionDependencyResolver(CompositionContainer container)
        {
            this.compositionContainer = container;
        }

        public void Register<T>(T value)
        {
            this.compositionContainer.ComposeExportedValue<T>(value);
        }

        public void Register<T>(T value, string name)
        {
            this.compositionContainer.ComposeExportedValue<T>(name, value);
        }

        public T GetService<T>()
        {
            return compositionContainer.GetExportedValue<T>();
        }

        public T GetService<T>(string name)
        {
            return compositionContainer.GetExportedValue<T>(name);
        }
    }
}

