using System;
using System.Collections.Generic;
using System.Reflection;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    public interface IAssembliesSelection
    {
        IComponentsSelection SelectAssemblies(Func<Assembly, bool> selector);
        IComponentsSelection SelectAssemblies(params Assembly[] assemblies);
        IComponentsSelection SelectAssemblies(IEnumerable<Assembly> assemblies);
    }
}