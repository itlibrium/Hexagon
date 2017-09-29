using System;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public class AssignableSelector : IComponentSelector
    {
        private readonly Type _baseType;

        public AssignableSelector(Type baseType)
        {
            _baseType = baseType;
        }

        public bool IsContainerComponent(Type type)
        {
            return _baseType.IsAssignableFrom(type);
        }
    }
}