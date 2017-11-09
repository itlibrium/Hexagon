using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyModel;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public static class Assemblies
    {
        public static Predicate<RuntimeLibrary> WithPrefix(string prefix) => a => a.Name.StartsWith(prefix, StringComparison.Ordinal);
        public static Predicate<RuntimeLibrary> Matching(Regex regex) => a => regex.IsMatch(a.Name);
    }
}