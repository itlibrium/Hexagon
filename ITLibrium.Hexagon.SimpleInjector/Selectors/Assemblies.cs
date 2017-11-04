using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyModel;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public static class Assemblies
    {
        public static Func<RuntimeLibrary, bool> WithPrefix(string prefix) => a => a.Name.StartsWith(prefix, StringComparison.Ordinal);

        public static Func<RuntimeLibrary, bool> Matching(Regex regex) => a => regex.IsMatch(a.Name);
    }
}