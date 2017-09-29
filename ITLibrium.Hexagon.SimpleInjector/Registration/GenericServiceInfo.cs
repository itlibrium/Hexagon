using System;
using System.Collections.Generic;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    internal class GenericServiceInfo : IServiceInfo
    {
        private readonly Type _definitionType;
        
        private readonly Dictionary<Type, ServiceInfo> _closedServices = new Dictionary<Type, ServiceInfo>();
        
        private Type _openImplementation;

        public GenericServiceInfo(Type definitionType)
        {
            if (!definitionType.IsGenericTypeDefinition)
                throw new ArgumentException(nameof(definitionType));
            
            _definitionType = definitionType;
        }

        public void AddImplementation(Type serviceType, Type implementationType)
        {
            if (!serviceType.IsGenericType || serviceType.GetGenericTypeDefinition() != _definitionType)
                throw new ArgumentException(nameof(serviceType));

            if (serviceType.ContainsGenericParameters)
            {
                _openImplementation = implementationType;
            }
            else
            {
                if (implementationType.ContainsGenericParameters)
                    throw new ArgumentException();

                if (!_closedServices.TryGetValue(serviceType, out ServiceInfo serviceInfo))
                    _closedServices.Add(serviceType, serviceInfo = new ServiceInfo(serviceType));

                serviceInfo.AddImplementation(serviceType, implementationType);
            }
        }

        public void Register(Container container, Lifestyle lifestyle)
        {
            foreach (ServiceInfo serviceInfo in _closedServices.Values)
                serviceInfo.Register(container, lifestyle);

            if (_openImplementation != null)
            {
                if (_closedServices.Count > 0)
                    container.RegisterConditional(_definitionType, _openImplementation, c => !c.Handled);
                else
                    container.Register(_definitionType, _openImplementation);
            }
        }
    }
}