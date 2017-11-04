using System;
using System.Collections.Generic;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public interface IRelatedComponentsSelector
    {
        IEnumerable<Type> GetRelatedComponents(Type type);
    }
}