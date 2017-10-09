using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    public interface IAssembliesSelection
    {
        IComponentsSelection SelectAssemblies(Func<RuntimeLibrary, bool> selector);
        IComponentsSelection SelectAssemblies(params Assembly[] assemblies);
        IComponentsSelection SelectAssemblies(IEnumerable<Assembly> assemblies);
    }
}