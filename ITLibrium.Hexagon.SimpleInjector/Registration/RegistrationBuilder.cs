using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITLibrium.Hexagon.SimpleInjector.Selectors;
using Microsoft.Extensions.DependencyModel;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    internal class RegistrationBuilder : ILifestyleSelection, INamespacesSelection
    {
        private readonly Container _container;
        private readonly IRegistrationPolicy _registrationPolicy;
        
        private Lifestyle _lifestyle;

        private Predicate<RuntimeLibrary> _assemblySelector;
        private IEnumerable<Assembly> _assemblies;

        private readonly NamespacesSelector _namespacesSelector = new NamespacesSelector();

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

        public INamespacesSelection SelectAssemblies(Predicate<RuntimeLibrary> predicate)
        {
            _assemblySelector = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return this;
        }

        public INamespacesSelection SelectAssemblies(params Assembly[] assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
            return this;
        }
        
        public INamespacesSelection SelectAssemblies(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
            return this;
        }

        public INamespacesSelection IncludeNamespaces(Predicate<Namespace> predicate)
        {
            _namespacesSelector.Include(predicate);
            return this;
        }

        public INamespacesSelection ExcludeNamespaces(Predicate<Namespace> predicate)
        {
            _namespacesSelector.Exclude(predicate);
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
                    .Where(a => _assemblySelector(a))
                    .Select(l => Assembly.Load(l.Name));
            
            throw new InvalidOperationException();
        }

        private IEnumerable<Type> GetTypesForAssembly(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(t => _namespacesSelector.IsSelected(t))
                .Where(t => _componentSelectors
                    .Any(s => s.IsContainerComponent(t)))
                .SelectMany(t => _relatedComponentSelectors
                    .SelectMany(s => s.GetRelatedComponents(t))
                    .Append(t));
        }

        private class NamespacesSelector
        {
            private readonly List<Predicate<Namespace>> _includePredicates = new List<Predicate<Namespace>>();
            private readonly List<Predicate<Namespace>> _excludePredicates = new List<Predicate<Namespace>>();

            public void Include(Predicate<Namespace> predicate)
            {
                _includePredicates.Add(predicate);
            }

            public void Exclude(Predicate<Namespace> predicate)
            {
                _excludePredicates.Add(predicate);
            }

            public bool IsSelected(Type type)
            {
                if (_includePredicates.Count > 0)
                {
                    if (_excludePredicates.Count > 0)
                        return IsIncluded(type) && IsNotExcluded(type);

                    return IsIncluded(type);
                }

                if (_excludePredicates.Count > 0)
                    return IsNotExcluded(type);

                return true;
            }

            private bool IsIncluded(Type type) => _includePredicates.Any(p => p(type));
            private bool IsNotExcluded(Type type) => _excludePredicates.All(p => !p(type));
        }
    }
}