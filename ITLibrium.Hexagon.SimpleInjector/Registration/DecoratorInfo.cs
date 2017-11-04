using System;
using ITLibrium.Hexagon.SimpleInjector.Selectors;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    internal struct DecoratorInfo
    {
        public Type ServiceType { get; }
        public Type DecoratorType { get; }
        public Predicate<DecoratorPredicateContext> Predicate { get; }

        public DecoratorInfo(Type serviceType, Type decoratorType, Predicate<DecoratorPredicateContext> predicate)
        {
            ServiceType = serviceType;
            DecoratorType = decoratorType;
            Predicate = predicate;
        }
    }
}