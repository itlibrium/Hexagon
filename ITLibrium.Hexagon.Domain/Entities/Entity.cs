using System;
using System.Runtime.CompilerServices;

namespace ITLibrium.Hexagon.Domain.Entities
{
    public class Entity<TSelf>
        where TSelf : Entity<TSelf>
    {
        public Id EntityId { get; }

        public bool IsNew => EntityId.IsNew;

        protected Entity()
        {
            EntityId = Id.New();
        }

        protected Entity(string id) : this(new Id(id)) { }
        
        protected Entity(Guid id) : this(new Id(id)) { }

        protected Entity(Id id)
        {
            EntityId = id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Entity<TSelf> other) => other != null && EntityId.Equals(other.EntityId);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override bool Equals(object obj) => obj is Entity<TSelf> otherEntity && Equals(otherEntity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override int GetHashCode() => EntityId.GetHashCode();

        public override string ToString() => EntityId.ToString();

        public struct Id
        {
            public static Id New() => new Id(Guid.NewGuid(), true);

            public Guid Value { get; }

            public bool IsNew { get; }

            public Id(string value) : this(new Guid(value)) { }

            public Id(Guid value) : this(value, false) { }

            private Id(Guid value, bool isNew)
            {
                Value = value;
                IsNew = isNew;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(Id other) => !IsNew && !other.IsNew && Value.Equals(other.Value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Equals(object obj) => obj is Id id && Equals(id);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode() => !IsNew ? Value.GetHashCode() : 0;

            public override string ToString() => IsNew ? $"New {typeof(TSelf).Name} Id: {Value}" : $"{typeof(TSelf).Name} Id: {Value}";

            public static implicit operator Guid(Id id) => id.Value;
        }
    }

    public class Entity<TSelf, TId>
        where TSelf : Entity<TSelf, TId>
    {
        public Id EntityId { get; }

        public bool IsNew => EntityId.IsNew;

        protected Entity()
        {
            EntityId = Id.New();
        }
        
        protected Entity(TId id) : this(new Id(id)) { }

        protected Entity(Id id)
        {
            EntityId = id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Entity<TSelf, TId> other) => other != null && EntityId.Equals(other.EntityId);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override bool Equals(object obj) => obj is Entity<TSelf, TId> otherEntity && Equals(otherEntity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override int GetHashCode() => EntityId.GetHashCode();

        public override string ToString() => EntityId.ToString();

        public struct Id
        {
            private static readonly Id _new = new Id();
            public static Id New() => _new;

            private readonly bool _hasValue;

            private readonly TId _value;
            public TId Value
            {
                get
                {
                    if (IsNew)
                        throw new InvalidOperationException("Can't get Id value for new entity");
                    return _value;
                }
            }

            public bool IsNew => !_hasValue;

            public Id(TId value)
            {
                _hasValue = true;
                _value = value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(Id other) => _hasValue && other._hasValue && _value.Equals(other._value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Equals(object obj) => obj is Id id && Equals(id);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode() => _hasValue ? Value.GetHashCode() : 0;

            public override string ToString() => IsNew ? $"New {typeof(TSelf).Name}" : $"{typeof(TSelf).Name} Id: {Value}";

            public static implicit operator TId(Id id) => id.Value;
        }
    }
}