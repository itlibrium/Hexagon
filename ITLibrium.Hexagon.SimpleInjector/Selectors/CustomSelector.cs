using System;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public class CustomSelector : IComponentSelector
    {
        private readonly Func<Type, bool> _selector;

        public CustomSelector(Func<Type, bool> selector)
        {
            _selector = selector;
        }

        public bool IsContainerComponent(Type type)
        {
            return _selector(type);
        }
    }
}