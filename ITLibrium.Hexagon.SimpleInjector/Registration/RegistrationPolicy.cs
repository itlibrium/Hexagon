using System;
using System.Collections.Generic;
using System.Linq;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    internal class RegistrationPolicy : IRegistrationPolicy
    {
        public void Register(Container container, Lifestyle lifestyle, IEnumerable<Type> types)
        {
            ISet<Type> typesSet = types as ISet<Type> ?? new HashSet<Type>(types);
            Register(container, lifestyle, typesSet);
        }

        private static void Register(Container container, Lifestyle lifestyle, ISet<Type> types)
        {
            var grouping = new Dictionary<Type, IServiceInfo>();
            
            foreach (Type type in types)
            {
                if (type.IsInterface)
                {
                    grouping.Add(type, CreateServiceInfo(type));
                    continue;
                }

                if (!type.IsClass || type.IsAbstract)
                    continue;
                
                IEnumerable<Type> serviceTypes = FindServiceTypes(type, types);
                if (serviceTypes == null)
                {
                    var componentInfo = new ServiceInfo(type);
                    componentInfo.AddImplementation(type, type);
                    grouping.Add(type, componentInfo);
                }
                else
                {
                    foreach (Type serviceType in serviceTypes)
                    {
                        if (serviceType.IsGenericType)
                        {
                            Type definitionType = serviceType.GetGenericTypeDefinition();
                            if (!grouping.TryGetValue(definitionType, out IServiceInfo serviceInfo))
                                grouping.Add(definitionType, serviceInfo = new GenericServiceInfo(definitionType));
                            
                            serviceInfo.AddImplementation(serviceType, type);
                        }
                        else
                        {
                            if (!grouping.TryGetValue(serviceType, out IServiceInfo serviceInfo))
                                grouping.Add(serviceType, serviceInfo = new ServiceInfo(serviceType));
                            
                            serviceInfo.AddImplementation(serviceType, type);
                        }
                    }
                }
            }

            foreach (IServiceInfo serviceInfo in grouping.Values)
                serviceInfo.Register(container, lifestyle);
        }

        private static IServiceInfo CreateServiceInfo(Type serviceType)
        {
            return serviceType.IsGenericType
                ? (IServiceInfo) new GenericServiceInfo(serviceType.GetGenericTypeDefinition())
                : new ServiceInfo(serviceType);
        }

        private static IEnumerable<Type> FindServiceTypes(Type type, ISet<Type> allTypes)
        {
            Type[] interfaces = type.GetInterfaces();
            return interfaces.Length == 0 ? null : interfaces.Where(t => IsSelectedType(t, allTypes));
        }

        private static bool IsSelectedType(Type type, ISet<Type> selectedTypes)
        {
            if (selectedTypes.Contains(type))
                return true;

            return type.IsGenericType && !type.IsGenericTypeDefinition && selectedTypes.Contains(type.GetGenericTypeDefinition());
        }
    }
}