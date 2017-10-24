using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITLibrium.Hexagon.SimpleInjector.Selectors;
using Microsoft.Extensions.DependencyModel;
using SimpleInjector;

namespace ITLibrium.Hexagon.App.SimpleInjector.Registration
{
    internal class AppRegistrationBuilder : ILifestyleSelection, IDecoratorsSelection
    {
        private readonly Container _container;
        private readonly IAppRegistrationPolicy _registrationPolicy;

        private Lifestyle _lifestyle;
        
        private Func<RuntimeLibrary, bool> _assemblySelector;
        private IReadOnlyList<Assembly> _assemblies;
        
        private readonly List<DecoratorInfo> _decorators = new List<DecoratorInfo>();

        public AppRegistrationBuilder(Container container, IAppRegistrationPolicy registrationPolicy)
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

        public IDecoratorsSelection SelectAssemblies(Func<RuntimeLibrary, bool> selector)
        {
            _assemblySelector = selector;
            return this;
        }

        public IDecoratorsSelection SelectAssemblies(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
            return this;
        }

        public IDecoratorsSelection SelectAssemblies(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies as IReadOnlyList<Assembly> ?? assemblies.ToList();
            return this;
        }

        public IDecoratorsSelection AddDecorator(Type decoratorType, Lifestyle lifestyle, params IComponentSelector[] componentSelectors)
        {
            return AddDecorator(decoratorType, lifestyle, c => componentSelectors.Any(s => s.IsContainerComponent(c.ImplementationType)));
        }

        public IDecoratorsSelection AddDecorator(Type decoratorType, Lifestyle lifestyle, Predicate<DecoratorPredicateContext> predicate)
        {
            _decorators.Add(new DecoratorInfo(decoratorType, lifestyle, predicate));
            return this;
        }

        public void Register()
        {
            _registrationPolicy.Register(_container, _lifestyle ?? _container.Options.DefaultLifestyle, GetAssemblies(), _decorators);
        }

        private IEnumerable<Assembly> GetAssemblies()
        {
            if (_assemblySelector != null)
                return AssemblyHelpers.GetAssembles(_assemblySelector);
            
            if (_assemblies != null)
                return _assemblies;
            
            throw new InvalidOperationException();
        }
    }
}