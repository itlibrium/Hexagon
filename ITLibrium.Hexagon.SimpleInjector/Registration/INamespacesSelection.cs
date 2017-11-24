using System;
using ITLibrium.Hexagon.SimpleInjector.Selectors;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    public interface INamespacesSelection : ITypesSelection
    {
        INamespacesSelection IncludeNamespaces(Predicate<Namespace> predicate);
        INamespacesSelection ExcludeNamespaces(Predicate<Namespace> predicate);
    }
}