using System;
using System.Text.RegularExpressions;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public static class Namespaces
    {
        public static Predicate<Namespace> Exact(string @namespace) => 
            n => n.HasValue && string.Equals(n.Value, @namespace, StringComparison.Ordinal);
        public static Predicate<Namespace> WithPrefix(string prefix) => 
            n => n.HasValue && n.Value.StartsWith(prefix, StringComparison.Ordinal);
        public static Predicate<Namespace> Matching(Regex regex) => 
            n => n.HasValue && regex.IsMatch(n.Value);
    }
}