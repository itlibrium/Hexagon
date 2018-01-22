using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    internal class RegistrationPolicy : IRegistrationPolicy
    {
        public void Register(Container container, Lifestyle lifestyle, IEnumerable<Type> types)
        {
            ISet<Type> typesSet = types as ISet<Type> ?? new HashSet<Type>(types);
            foreach (IServiceInfo serviceInfo in CreateServicesInfo(typesSet))
                serviceInfo.Register(container, lifestyle);
        }

        private static IEnumerable<IServiceInfo> CreateServicesInfo(ISet<Type> types)
        {
            var grouping = new Dictionary<Type, IServiceInfo>();
            foreach (Type type in types)
            {
                if (!IsConstructableClass(type))
                {
                    GetOrAddServiceInfo(grouping, type);
                    continue;
                }
                
                foreach (Type serviceType in FindServiceTypes(type, types))
                {
                    IServiceInfo serviceInfo = GetOrAddServiceInfo(grouping, serviceType);
                    serviceInfo.AddImplementation(serviceType, type);
                }
            }
            return grouping.Values;
        }

        private static IServiceInfo GetOrAddServiceInfo(IDictionary<Type, IServiceInfo> grouping, Type serviceType)
        {
            IServiceInfo serviceInfo;
            if (serviceType.IsGenericType)
            {
                Type keyType = serviceType.GetGenericTypeDefinition();
                if (!grouping.TryGetValue(keyType, out serviceInfo))
                    grouping.Add(keyType, serviceInfo = new GenericServiceInfo(keyType));
            }
            else
            {
                if (!grouping.TryGetValue(serviceType, out serviceInfo))
                    grouping.Add(serviceType, serviceInfo = new ServiceInfo(serviceType));
            }
            return serviceInfo;
        }
        
        private static bool IsConstructableClass(Type type)
        {
            return type.IsClass && !type.IsAbstract &&
                   type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length > 0;
        }

        private static IEnumerable<Type> FindServiceTypes(Type type, ISet<Type> selectedTypes)
        {
            return type.GetInterfaces()
                .Union(GetClassesHierarchy(type))
                .Where(i => IsSelectedType(i, selectedTypes));
        }

        private static IEnumerable<Type> GetClassesHierarchy(Type type)
        {
            for (Type t = type; t != null && t != typeof(object); t = t.BaseType)
                yield return t;
        }

        private static bool IsSelectedType(Type type, ISet<Type> selectedTypes)
        {
            if (selectedTypes.Contains(type))
                return true;

            return type.IsGenericType && !type.IsGenericTypeDefinition && selectedTypes.Contains(type.GetGenericTypeDefinition());
        }
    }
}