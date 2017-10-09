using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNet.SignalR;

using StructureMap;

namespace SignalR.StructureMap
{
    public class SignalRStructureMapDependencyResolver : DefaultDependencyResolver
    {
        private readonly IContainer _container;

        public SignalRStructureMapDependencyResolver(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            _container = container;
        }

        public override object GetService(Type serviceType)
        {
            var service = base.GetService(serviceType);
            if (service != null) return service;

            return !serviceType.IsAbstract && !serviceType.IsInterface && serviceType.IsClass
                               ? _container.GetInstance(serviceType)
                               : (_container.TryGetInstance(serviceType) ?? base.GetService(serviceType));
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.GetAllInstances(serviceType).Cast<object>().Concat(base.GetServices(serviceType));
        }
    }
}