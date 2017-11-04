using System;
using ITLibrium.Hexagon.SimpleInjector.Selectors;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    public interface ITypesSelection
    {
        ITypesSelection Include(IComponentSelector selector);
        ITypesSelection Include(IRelatedComponentsSelector selector);
        
        ITypesSelection Exclude(params Type[] types);
        ITypesSelection Include(params Type[] types);

        ITypesSelection IncludeDecorator(Type serviceType, Type decoratorType, IComponentSelector decoratedSelector);
        ITypesSelection IncludeDecorator(Type serviceType, Type decoratorType, Predicate<DecoratorPredicateContext> predicate = null);
    }
}