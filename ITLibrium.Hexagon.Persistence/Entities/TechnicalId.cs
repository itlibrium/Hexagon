using System;

namespace ITLibrium.Hexagon.Persistence.Entities
{
    public struct TechnicalId<TValue> : ITechnicalId<TValue>
    {
        public bool IsNew { get; }
        
        private readonly TValue _technicalValue;
        public TValue TechnicalValue
        {
            get
            {
                if (IsNew)
                    throw new InvalidOperationException($"Can't get {nameof(TechnicalId<TValue>)} value for new entity");
                return _technicalValue;
            }
        }

        public static TechnicalId<TValue> New() => new TechnicalId<TValue>(true, default(TValue));
        public static TechnicalId<TValue> WithValue(TValue value) => new TechnicalId<TValue>(false, value);

        private TechnicalId(bool isNew, TValue value)
        {
            if (!isNew && value == null)
                throw new ArgumentNullException(nameof(value));
            
            IsNew = isNew;
            _technicalValue = value;
        }

        public override bool Equals(object obj) => obj is TechnicalId<TValue> technicalId && Equals(technicalId);
        public bool Equals(TechnicalId<TValue> other) => !IsNew && !other.IsNew && _technicalValue.Equals(other._technicalValue);

        public override int GetHashCode() => IsNew ? 0 : _technicalValue.GetHashCode();
    }

    public static class TechnicalId
    {
        public static TechnicalId<TValue> New<TValue>() => TechnicalId<TValue>.New();
        public static TechnicalId<TValue> WithValue<TValue>(TValue value) => TechnicalId<TValue>.WithValue(value);
    }
}