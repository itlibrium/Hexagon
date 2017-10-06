using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITLibrium.Hexagon.App.Commands;
using ITLibrium.Hexagon.App.Gates;
using ITLibrium.Hexagon.App.Queries;
using ITLibrium.Hexagon.App.Services;
using ITLibrium.Hexagon.App.SimpleInjector.Gates;
using ITLibrium.Hexagon.SimpleInjector.Registration;
using ITLibrium.Hexagon.SimpleInjector.Selectors;
using SimpleInjector;

namespace ITLibrium.Hexagon.App.SimpleInjector.Registration
{
    internal class AppRegistrationPolicy : IAppRegistrationPolicy
    {
        private readonly RegistrationPolicy _registrationPolicy = new RegistrationPolicy();

        public void Register(Container container, Lifestyle lifestyle, IEnumerable<Assembly> assemblies, IEnumerable<IDecoratorInfo> decoratorsInfo)
        {
            container.RegisterSingleton<IGate>(new SimpleInjectorGate(container));
            
            IReadOnlyCollection<Assembly> assembliesCollection = assemblies as IReadOnlyCollection<Assembly> ?? new List<Assembly>(assemblies);
            
            IEnumerable<Type> types = assembliesCollection
                .SelectMany(a => a.GetTypes())
                .Where(IsAppLogic)
                .SelectMany(GetWithRelatedTypes)
                .Union(new[]
                {
                    typeof(IAppService),
                    typeof(IActionExecutor), typeof(IActionExecutor<>),
                    typeof(ICommandHandler), typeof(ICommandHandler<>), typeof(ICommandHandler<,>),
                    typeof(IFinder), typeof(IFinder<>), typeof(IFinder<,>),
                    typeof(IGatePolicy)
                });
            _registrationPolicy.Register(container, lifestyle, types);
            
            RegisterDecorators(container, decoratorsInfo);
            
        }
        
        private static bool IsAppLogic(Type type)
        {
            return typeof(IAppService).IsAssignableFrom(type)
                   || typeof(ICommandHandler).IsAssignableFrom(type)
                   || typeof(IFinder).IsAssignableFrom(type)
                   || typeof(IGatePolicy).IsAssignableFrom(type);
        }

        private static IEnumerable<Type> GetWithRelatedTypes(Type type)
        {
            yield return type;

            if (typeof(IAppService).IsAssignableFrom(type))
            {
                yield return typeof(IActionExecutor<>).MakeGenericType(type);
                yield return typeof(ActionExecutor<>).MakeGenericType(type);
            }
        }
        
        private static void RegisterDecorators(Container container, IEnumerable<IDecoratorInfo> decoratorsInfo)
        {
            if (decoratorsInfo == null)
                return;
            
            foreach (IDecoratorInfo decoratorInfo in decoratorsInfo)
            {
                Type decoratorType = decoratorInfo.Type;
                if (!decoratorType.ContainsGenericParameters)
                    throw new ArgumentException(nameof(decoratorInfo));
                
                foreach (Type interfaceType in decoratorType.GetInterfaces())
                {
                    if (!interfaceType.IsGenericType)
                        continue;

                    Type interfaceDefinition = interfaceType.GetGenericTypeDefinition();
                    if(interfaceDefinition == typeof(IActionExecutor<>))
                        decoratorInfo.Register(container, typeof(IActionExecutor<>));
                    else if(interfaceDefinition == typeof(ICommandHandler<>))
                        decoratorInfo.Register(container, typeof(ICommandHandler<>));
                    else if(interfaceDefinition == typeof(ICommandHandler<,>))
                        decoratorInfo.Register(container, typeof(ICommandHandler<,>));
                    else if(interfaceDefinition == typeof(IFinder<>))
                        decoratorInfo.Register(container, typeof(IFinder<>));
                    else if(interfaceDefinition == typeof(IFinder<,>))
                        decoratorInfo.Register(container, typeof(IFinder<,>));
                }
            }
        }
    }
}