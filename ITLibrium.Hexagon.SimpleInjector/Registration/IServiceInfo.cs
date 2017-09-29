using System;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    internal interface IServiceInfo
    {
        void AddImplementation(Type serviceType, Type implementationType);
        void Register(Container container, Lifestyle lifestyle);
    }
}