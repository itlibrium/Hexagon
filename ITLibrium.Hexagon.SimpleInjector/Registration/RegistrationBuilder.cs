using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITLibrium.Hexagon.SimpleInjector.Selectors;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    internal class RegistrationBuilder : ILifestyleSelection, IAssembliesSelection, IComponentsSelection, ITypesSelection
    {
        private readonly Container _container;
        private readonly IRegistrationPolicy _registrationPolicy;
        
        private Lifestyle _lifestyle;

        private Func<Assembly, bool> _assemblySelector;
        private IEnumerable<Assembly> _assemblies;

        private IComponentSelector[] _componentSelectors;

        private Type[] _excludedTypes;
        private Type[] _includedTypes;

        public RegistrationBuilder(Container container, IRegistrationPolicy registrationPolicy)
        {
            _container = container;
            _registrationPolicy = registrationPolicy;
        }

        public IAssembliesSelection UseLifestyle(Lifestyle lifestyle)
        {
            _lifestyle = lifestyle ?? throw new ArgumentNullException(nameof(lifestyle));
            return this;
        }

        public IComponentsSelection SelectAssemblies(Func<Assembly, bool> selector)
        {
            _assemblySelector = selector ?? throw new ArgumentNullException(nameof(selector));
            return this;
        }

        public IComponentsSelection SelectAssemblies(params Assembly[] assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
            return this;
        }
        
        public IComponentsSelection SelectAssemblies(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
            return this;
        }

        public ITypesSelection SelectComponents(params IComponentSelector[] selectors)
        {
            _componentSelectors = selectors;
            return this;
        }

        public ITypesSelection ExcludeTypes(params Type[] types)
        {
            _excludedTypes = types;
            return this;
        }

        public ITypesSelection IncludeTypes(params Type[] types)
        {
            _includedTypes = types;
            return this;
        }

        public void Register()
        {
            _registrationPolicy.Register(_container, _lifestyle, GetTypes());
        }

        private IEnumerable<Type> GetTypes()
        {
            IEnumerable<Assembly> assemblies;
            if (_assemblySelector != null)
                assemblies = AssemblyHelpers.GetAssembles(_assemblySelector);
            else if (_assemblies != null)
                assemblies = _assemblies;
            else
                throw new InvalidOperationException();

            IEnumerable<Type> types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => _componentSelectors.Any(s => s.IsContainerComponent(t)));

            var set = new HashSet<Type>(types);
            if (_includedTypes != null)
                set.UnionWith(_includedTypes);
            if (_excludedTypes != null)
                set.ExceptWith(_excludedTypes);

            return set;
        }
    }
}