using System;
using System.Collections.Generic;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    internal class ServiceInfo : IServiceInfo
    {
        private readonly Type _serviceType;
        
        private readonly Type _enumerableType;

        private readonly List<Type> _implementationTypes = new List<Type>();

        private Type _compositeImplementationType;

        private bool _excludeFromRegistration;

        public ServiceInfo(Type serviceType)
        {
            if (serviceType.IsGenericType && serviceType.ContainsGenericParameters)
                throw new ArgumentException(nameof(serviceType));

            _serviceType = serviceType;
            _enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);
        }

        public void AddImplementation(Type serviceType, Type implementationType)
        {
            if (serviceType == null || serviceType != _serviceType)
                throw new ArgumentException(nameof(serviceType));

            if (implementationType.ContainsGenericParameters)
            {
                _excludeFromRegistration = true;
                return;
            }

            if (implementationType.IsCompositeOf(_enumerableType))
            {
                if (_compositeImplementationType != null)
                    throw new InvalidOperationException(
                        $"Can't register more than one composite implementation. ServiceType: {_serviceType}, ImplementationType: {implementationType}");

                _compositeImplementationType = implementationType;
            }
            else
            {
                _implementationTypes.Add(implementationType);
            }
        }
        
        public void Register(Container container, Lifestyle lifestyle)
        {
            if (_excludeFromRegistration)
                return;
            
            if (_compositeImplementationType != null)
            {
                RegisterCollection(container);
                RegisterComposite(container, lifestyle);
            }
            else if (_implementationTypes.Count == 1)
            {
                RegisterSingle(container, lifestyle);
                RegisterCollection(container);
            }
            else
            {
                RegisterCollection(container);
            }
        }
        
        private void RegisterComposite(Container container, Lifestyle lifestyle)
        {
            container.Register(_serviceType, _compositeImplementationType, lifestyle);
        }

        private void RegisterSingle(Container container, Lifestyle lifestyle)
        {
            Type implementationType = _implementationTypes[0];
            container.Register(_serviceType, implementationType, lifestyle);
        }

        private void RegisterCollection(Container container)
        {            
            container.RegisterCollection(_serviceType, _implementationTypes);
        }
    }
}