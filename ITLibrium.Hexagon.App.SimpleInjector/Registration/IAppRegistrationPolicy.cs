using System.Collections.Generic;
using System.Reflection;
using SimpleInjector;

namespace ITLibrium.Hexagon.App.SimpleInjector.Registration
{
    public interface IAppRegistrationPolicy
    {
        void Register(Container container, Lifestyle lifestyle, IEnumerable<Assembly> assemblies, IEnumerable<IDecoratorInfo> decoratorsInfo);
    }
}