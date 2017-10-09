using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyModel;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public static class AssemblySelectors
    {
        public static Func<RuntimeLibrary, bool> Prefix(string prefix) => a => a.Name.StartsWith(prefix, StringComparison.Ordinal);

        public static Func<RuntimeLibrary, bool> Regex(Regex regex) => a => regex.IsMatch(a.Name);
    }
}