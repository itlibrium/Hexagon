using System;
using ITLibrium.Hexagon.App.SimpleInjector.Registration;
using ITLibrium.Hexagon.SimpleInjector.Registration;
using SimpleInjector;
using ILifestyleSelection = ITLibrium.Hexagon.App.SimpleInjector.Registration.ILifestyleSelection;

namespace ITLibrium.Hexagon.App.SimpleInjector
{
    public static class SimpleInjectorAppExtensions
    {
        public static void RegisterAppLogic(this Container container, Func<ILifestyleSelection, IDecoratorsSelection> setup)
        {
            var registrationBuilder = new AppRegistrationBuilder(new AppRegistrationPolicy());
            setup(registrationBuilder);
            registrationBuilder.Register(container);
        }
    }
}