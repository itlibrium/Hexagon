using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    public interface IAssembliesSelection
    {
        INamespacesSelection SelectAssemblies(Predicate<RuntimeLibrary> predicate);
        INamespacesSelection SelectAssemblies(params Assembly[] assemblies);
        INamespacesSelection SelectAssemblies(IEnumerable<Assembly> assemblies);
    }
}