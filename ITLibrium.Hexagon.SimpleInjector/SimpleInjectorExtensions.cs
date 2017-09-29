using System;
using ITLibrium.Hexagon.SimpleInjector.Registration;
using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector
{
    public static class SimpleInjectorExtensions
    {
        public static void Register(this Container container, Func<ILifestyleSelection, ITypesSelection> setup)
        {
            var builder = new RegistrationBuilder(container, new RegistrationPolicy());
            setup(builder);
            builder.Register();
        }
    }
}