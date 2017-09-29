using System;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public interface IComponentSelector
    {
        bool IsContainerComponent(Type type);
    }
}