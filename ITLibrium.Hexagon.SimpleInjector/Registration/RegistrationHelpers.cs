using System;
using System.Linq;
using System.Reflection;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    internal static class RegistrationHelpers
    {
        public static bool IsCompositeOf(this Type implementationType, Type enumerableType)
        {
            ConstructorInfo[] constructors = implementationType.GetConstructors();
            return constructors.Length == 1 && constructors[0].GetParameters().Any(p => p.ParameterType == enumerableType);
        }
    }
}