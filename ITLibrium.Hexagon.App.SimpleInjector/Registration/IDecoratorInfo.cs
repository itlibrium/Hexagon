using System;
using SimpleInjector;

namespace ITLibrium.Hexagon.App.SimpleInjector.Registration
{
    public interface IDecoratorInfo
    {
        Type Type { get; }
        void Register(Container container, Type serviceType);
    }
}