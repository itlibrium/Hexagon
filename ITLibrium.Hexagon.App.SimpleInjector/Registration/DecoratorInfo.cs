using System;
using SimpleInjector;

namespace ITLibrium.Hexagon.App.SimpleInjector.Registration
{
    internal class DecoratorInfo : IDecoratorInfo
    {
        public Type Type { get; }
        private readonly Lifestyle _lifestyle;
        private readonly Predicate<DecoratorPredicateContext> _predicate;

        public DecoratorInfo(Type type, Lifestyle lifestyle, Predicate<DecoratorPredicateContext> predicate)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            _lifestyle = lifestyle ?? throw new ArgumentNullException(nameof(lifestyle));
            _predicate = predicate;
        }

        public void Register(Container container, Type serviceType)
        {
            if (_predicate == null)
                container.RegisterDecorator(serviceType, Type, _lifestyle);
            else
                container.RegisterDecorator(serviceType, Type, _lifestyle, _predicate);
        }
    }
}