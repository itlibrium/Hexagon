using System;
using ITLibrium.Hexagon.SimpleInjector.Selectors;
using SimpleInjector;

namespace ITLibrium.Hexagon.App.SimpleInjector.Registration
{
    public interface IDecoratorsSelection
    {
        IDecoratorsSelection AddDecorator(Type decoratorType, Lifestyle lifestyle, params IComponentSelector[] componentSelectors);
        IDecoratorsSelection AddDecorator(Type decoratorType, Lifestyle lifestyle, Predicate<DecoratorPredicateContext> predicate);
    }
}