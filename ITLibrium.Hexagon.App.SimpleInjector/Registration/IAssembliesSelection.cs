using System;
using System.Collections.Generic;
using System.Reflection;

namespace ITLibrium.Hexagon.App.SimpleInjector.Registration
{
    public interface IAssembliesSelection
    {
        IDecoratorsSelection SelectAssemblies(Func<Assembly, bool> selector);
        IDecoratorsSelection SelectAssemblies(params Assembly[] assemblies);
        IDecoratorsSelection SelectAssemblies(IEnumerable<Assembly> assemblies);
    }
}