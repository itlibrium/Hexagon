using System;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    public interface INamespacesSelection : ITypesSelection
    {
        INamespacesSelection IncludeOnlyNamespaces(Predicate<string> predicate);
        INamespacesSelection ExcludeNamespaces(Predicate<string> predicate);
    }
}