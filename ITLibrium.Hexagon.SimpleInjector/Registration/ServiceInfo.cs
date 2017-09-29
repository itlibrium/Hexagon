using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    internal class ServiceInfo : IServiceInfo
    {
        private readonly Type _serviceType;
        
        private readonly Type _enumerableType;

        private readonly List<Type> _implementationTypes = new List<Type>();

        private Type _compositeImplementation;

        public ServiceInfo(Type serviceType)
        {
            if (serviceType.IsGenericType && serviceType.ContainsGenericParameters)
                throw new ArgumentException();

            _serviceType = serviceType;
            _enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);
        }

        public void AddImplementation(Type serviceType, Type implementationType)
        {
            if (serviceType != _serviceType)
                throw new ArgumentException();
            
            if (CheckIfComposite(implementationType))
            {
                if (_compositeImplementation != null)
                    throw new InvalidOperationException(
                        $"Can't register more than one composite implementation. ServiceType: {_serviceType}, ImplementationType: {implementationType}");

                _compositeImplementation = implementationType;
            }
            else
            {
                _implementationTypes.Add(implementationType);
            }
        }

        private bool CheckIfComposite(Type implementationType)
        {
            ConstructorInfo[] constructors = implementationType.GetConstructors();
            return constructors.Length == 1 && constructors[0].GetParameters().Any(p => p.ParameterType == _enumerableType);
        }

        public void Register(Container container, Lifestyle lifestyle)
        {
            if (_compositeImplementation != null)
            {
                container.RegisterCollection(_serviceType, _implementationTypes);
                container.Register(_serviceType, _compositeImplementation, lifestyle);
                return;
            }

            if (_implementationTypes.Count == 1)
                container.Register(_serviceType, _implementationTypes[0]);
            else if (_implementationTypes.Count > 1)
                container.RegisterCollection(_serviceType, _implementationTypes);
        }
    }
}