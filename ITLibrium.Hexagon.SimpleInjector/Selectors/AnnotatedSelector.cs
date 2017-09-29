using System;
using System.Collections.Generic;
using System.Linq;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public class AnnotatedSelector : IComponentSelector
    {
        private readonly HashSet<Type> _attributes;

        public AnnotatedSelector(IEnumerable<Type> attributes)
        {
            _attributes = new HashSet<Type>(attributes);
        }

        public bool IsContainerComponent(Type type)
        {
            object[] typeAttributes = type.GetCustomAttributes(true);
            IEnumerable<object> interfacesAttributes = type.GetInterfaces().SelectMany(i => i.GetCustomAttributes(true));
            IEnumerable<Type> allAttributeTypes = typeAttributes.Union(interfacesAttributes).Select(a => a.GetType());
            return _attributes.Overlaps(allAttributeTypes);
        }
    }
}