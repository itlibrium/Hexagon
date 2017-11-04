using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    public interface IAssembliesSelection
    {
        ITypesSelection SelectAssemblies(Func<RuntimeLibrary, bool> selector);
        ITypesSelection SelectAssemblies(params Assembly[] assemblies);
        ITypesSelection SelectAssemblies(IEnumerable<Assembly> assemblies);
    }
}