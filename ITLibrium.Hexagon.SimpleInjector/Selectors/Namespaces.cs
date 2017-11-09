using System;
using System.Text.RegularExpressions;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public static class Namespaces
    {
        public static Predicate<string> WithPrefix(string prefix) => n => n.StartsWith(prefix);
        public static Predicate<string> Matching(Regex regex) => regex.IsMatch;
    }
}