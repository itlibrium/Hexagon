using System;

namespace ITLibrium.Hexagon.App.Queries
{
    internal struct FinderId
    {
        public static FinderId Create<TResult>() => new FinderId(typeof(TResult));
        public static FinderId Create<TResult, TCriteria>() => new FinderId(typeof(TResult), typeof(TCriteria));

        private readonly Type _resultType;
        private readonly Type _criteriaType;

        public FinderId(Type resultType)
        {
            _criteriaType = null;
            _resultType = resultType ?? throw new ArgumentNullException(nameof(resultType));
        }

        public FinderId(Type resultType, Type criteriaType)
        {
            _resultType = resultType ?? throw new ArgumentNullException(nameof(resultType));
            _criteriaType = criteriaType ?? throw new ArgumentNullException(nameof(criteriaType));
        }

        public bool Equals(FinderId other)
        {
            return _criteriaType == other._criteriaType && _resultType == other._resultType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is FinderId id && Equals(id);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_criteriaType != null ? _criteriaType.GetHashCode() : 0) * 397) ^ _resultType.GetHashCode();
            }
        }

        public override string ToString()
        {
            return _criteriaType != null ? $"Criteria: {_criteriaType.Name}, Result: {_resultType.Name}" : $"Result: {_resultType.Name}";
        }
    }
}