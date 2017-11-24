using System;

namespace ITLibrium.Hexagon.SimpleInjector.Selectors
{
    public struct Namespace
    {
        public bool HasValue { get; }
        public string Value { get; }

        public Namespace(string value)
        {
            HasValue = !string.IsNullOrWhiteSpace(value);
            Value = value;
        }
        
        public static implicit operator Namespace(Type type) => new Namespace(type.Namespace);
    }
}