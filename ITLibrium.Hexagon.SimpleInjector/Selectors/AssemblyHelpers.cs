using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public static class AssemblyHelpers
    {
        public static IEnumerable<Assembly> GetAssembles(Func<Assembly, bool> selector)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(selector);
        }
    }
}