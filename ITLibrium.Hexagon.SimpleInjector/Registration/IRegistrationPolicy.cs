using System;
using System.Collections.Generic;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    public interface IRegistrationPolicy
    {
        void Register(Container container, Lifestyle lifestyle, IEnumerable<Type> types);
    }
}