using System;
using System.Collections.Generic;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public static class Components
    {
        public static AnnotatedSelector AnnotatedBy(params Type[] attributes) => new AnnotatedSelector(attributes);
        public static AnnotatedSelector AnnotatedBy(IEnumerable<Type> attributes) => new AnnotatedSelector(attributes);

        public static AssignableSelector AssignableFrom(Type baseType) => new AssignableSelector(baseType);

        public static CustomSelector Custom(Func<Type, bool> selector) => new CustomSelector(selector);
    }
}