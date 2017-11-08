using ITLibrium.Hexagon.Domain.Meta;
using ITLibrium.Hexagon.SimpleInjector.Registration;
using ITLibrium.Hexagon.SimpleInjector.Selectors;

namespace ITLibrium.Hexagon.Domain.SimpleInjector
{
    public static class SimpleInjectorDomainExtensions
    {
        public static ITypesSelection IncludeDomainStatelessLogic(this ITypesSelection typesSelection)
        {
            return typesSelection.Include(Components.AnnotatedBy(
                typeof(DomainServiceAttribute),
                typeof(PolicyAttribute),
                typeof(RepositoryAttribute),
                typeof(FactoryAttribute)));
        }
    }
}