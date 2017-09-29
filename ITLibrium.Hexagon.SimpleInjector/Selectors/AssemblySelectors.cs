using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public static class AssemblySelectors
    {
        public static Func<Assembly, bool> Prefix(string prefix) => a => a.GetName().Name.StartsWith(prefix, StringComparison.Ordinal);

        public static Func<Assembly, bool> Regex(Regex regex) => a => regex.IsMatch(a.GetName().Name);
    }
}