using System;
using System.Collections.Generic;
using System.Linq;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    internal class GenericServiceInfo : IServiceInfo
    {
        private readonly Type _definitionType;
        
        private readonly Type _enumerableType;
        
        private readonly Dictionary<Type, ServiceInfo> _closedServices = new Dictionary<Type, ServiceInfo>();
        
        private Type _openImplementationType;

        public GenericServiceInfo(Type definitionType)
        {
            if (!definitionType.IsGenericTypeDefinition)
                throw new ArgumentException(nameof(definitionType));
            
            _definitionType = definitionType;
            _enumerableType = typeof(IEnumerable<>).MakeGenericType(_definitionType);
        }

        public void AddImplementation(Type serviceType, Type implementationType)
        {
            if (!serviceType.IsGenericType || serviceType.GetGenericTypeDefinition() != _definitionType)
                throw new ArgumentException(nameof(serviceType));

            if (serviceType.ContainsGenericParameters)
            {
                if (serviceType.GetGenericArguments().Any(a => !a.IsGenericParameter))
                    return;
                
                if (implementationType.IsCompositeOf(_enumerableType))
                    throw new NotSupportedException(
                        $"Open generic composites are not supported. Service typs: {serviceType.Name}, Implementation type: {implementationType.Name}");

                _openImplementationType = implementationType;
            }
            else
            {
                if (implementationType.ContainsGenericParameters)
                    throw new ArgumentException(nameof(implementationType));

                if (!_closedServices.TryGetValue(serviceType, out ServiceInfo serviceInfo))
                    _closedServices.Add(serviceType, serviceInfo = new ServiceInfo(serviceType));

                serviceInfo.AddImplementation(serviceType, implementationType);
            }
        }

        public void Register(Container container, Lifestyle lifestyle)
        {
            foreach (ServiceInfo serviceInfo in _closedServices.Values)
                serviceInfo.Register(container, lifestyle);

            if (_openImplementationType != null)
            {
                if (_closedServices.Count > 0)
                    container.RegisterConditional(_definitionType, _openImplementationType, lifestyle, c => !c.Handled);
                else
                    container.Register(_definitionType, _openImplementationType, lifestyle);

                container.AppendToCollection(_definitionType, _openImplementationType);
            }
        }
    }
}