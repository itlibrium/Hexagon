using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public static class AssemblyHelpers
    {
        public static IEnumerable<Assembly> GetAssembles(Func<RuntimeLibrary, bool> selector)
        {
            return DependencyContext.Default.RuntimeLibraries
                .Where(selector)
                .Select(l => Assembly.Load(l.Name));
        }
    }
}