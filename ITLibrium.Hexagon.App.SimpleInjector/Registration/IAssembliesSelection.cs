using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace ITLibrium.Hexagon.App.SimpleInjector.Registration
{
    public interface IAssembliesSelection
    {
        IDecoratorsSelection SelectAssemblies(Func<RuntimeLibrary, bool> selector);
        IDecoratorsSelection SelectAssemblies(params Assembly[] assemblies);
        IDecoratorsSelection SelectAssemblies(IEnumerable<Assembly> assemblies);
    }
}