using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITLibrium.Hexagon.SimpleInjector.Selectors;
using Microsoft.Extensions.DependencyModel;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    internal class RegistrationBuilder : ILifestyleSelection, ITypesSelection
    {
        private readonly Container _container;
        private readonly IRegistrationPolicy _registrationPolicy;
        
        private Lifestyle _lifestyle;

        private Func<RuntimeLibrary, bool> _assemblySelector;
        private IEnumerable<Assembly> _assemblies;

        private readonly List<IComponentSelector> _componentSelectors = new List<IComponentSelector>();
        private readonly List<IRelatedComponentsSelector> _relatedComponentSelectors = new List<IRelatedComponentsSelector>();

        private readonly List<Type> _excludedTypes = new List<Type>();
        private readonly List<Type> _includedTypes = new List<Type>();
        
        private readonly List<DecoratorInfo> _decorators = new List<DecoratorInfo>();
        
        public RegistrationBuilder(Container container, IRegistrationPolicy registrationPolicy)
        {
            _container = container;
            _registrationPolicy = registrationPolicy;
        }
        
        public IAssembliesSelection UseScopedLifestyle()
        {
            _lifestyle = _container.Options.DefaultScopedLifestyle;
            return this;
        }

        public IAssembliesSelection UseLifestyle(Lifestyle lifestyle)
        {
            _lifestyle = lifestyle ?? throw new ArgumentNullException(nameof(lifestyle));
            return this;
        }

        public ITypesSelection SelectAssemblies(Func<RuntimeLibrary, bool> selector)
        {
            _assemblySelector = selector ?? throw new ArgumentNullException(nameof(selector));
            return this;
        }

        public ITypesSelection SelectAssemblies(params Assembly[] assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
            return this;
        }
        
        public ITypesSelection SelectAssemblies(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
            return this;
        }
        
        public ITypesSelection Include(IComponentSelector selector)
        {
            _componentSelectors.Add(selector);
            return this;
        }
        
        public ITypesSelection Include(IRelatedComponentsSelector selector)
        {
            _relatedComponentSelectors.Add(selector);
            return this;
        }
        
        public ITypesSelection Include(params Type[] types)
        {
            _includedTypes.AddRange(types);
            return this;
        }
        
        public ITypesSelection Exclude(params Type[] types)
        {
            _excludedTypes.AddRange(types);
            return this;
        }

        public ITypesSelection IncludeDecorator(Type serviceType, Type decoratorType, IComponentSelector decoratedSelector)
        {
            return IncludeDecorator(serviceType, decoratorType, c => decoratedSelector.IsContainerComponent(c.ImplementationType));
        }

        public ITypesSelection IncludeDecorator(Type serviceType, Type decoratorType, Predicate<DecoratorPredicateContext> predicate = null)
        {
            _decorators.Add(new DecoratorInfo(serviceType, decoratorType, predicate));
            return this;
        }

        public void Register()
        {
            Lifestyle lifestyle = _lifestyle ?? _container.Options.DefaultLifestyle;
            _registrationPolicy.Register(_container, lifestyle, GetTypes());

            foreach (DecoratorInfo decorator in _decorators)
            {
                if (decorator.Predicate == null)
                    _container.RegisterDecorator(decorator.ServiceType, decorator.DecoratorType, lifestyle);
                else
                    _container.RegisterDecorator(decorator.ServiceType, decorator.DecoratorType, lifestyle, decorator.Predicate);
            }
        }

        private IEnumerable<Type> GetTypes()
        {
            return GetAssemblies()
                .SelectMany(GetTypesForAssembly)
                .Union(_includedTypes)
                .Except(_excludedTypes)
                .Distinct();
        }

        private IEnumerable<Assembly> GetAssemblies()
        {
            if (_assemblies != null)
                return _assemblies;
            
            if (_assemblySelector != null)
                return DependencyContext.Default.RuntimeLibraries
                    .Where(_assemblySelector)
                    .Select(l => Assembly.Load(l.Name));
            
            throw new InvalidOperationException();
        }

        private IEnumerable<Type> GetTypesForAssembly(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(t => _componentSelectors
                    .Any(s => s.IsContainerComponent(t)))
                .SelectMany(t => _relatedComponentSelectors
                    .SelectMany(s => s.GetRelatedComponents(t))
                    .Append(t));
        }
    }
}