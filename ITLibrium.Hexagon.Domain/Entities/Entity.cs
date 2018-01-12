using System;

namespace ITLibrium.Hexagon.Domain.Entities
{
    public class Entity<TSelf, TId>
        where TSelf : Entity<TSelf, TId>
    {
        public DomainId Id { get; }

        protected Entity(DomainId id)
        {
            Id = id;
        }

        public sealed override bool Equals(object obj) => obj is Entity<TSelf, TId> otherEntity && Equals(otherEntity);
        public bool Equals(Entity<TSelf, TId> other) => other != null && Id.Equals(other.Id);

        public sealed override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Id.ToString();

        public class DomainId
        {
            public TId Value { get; }
            
            public static DomainId FromValue(TId value) => new DomainId(value);

            private DomainId(TId value)
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                Value = value;
            }

            public override bool Equals(object obj) => obj is DomainId id && Equals(id);
            public bool Equals(DomainId other) => Value.Equals(other.Value);

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => $"{typeof(TSelf).Name} Id: {Value}";

            public static implicit operator TId(DomainId id) => id.Value;
            public static implicit operator DomainId(TId value) => new DomainId(value);
        }
    }
}