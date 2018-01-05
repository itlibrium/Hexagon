using System;

namespace ITLibrium.Hexagon.Persistence.Entities
{
    public struct GlobalId : ITechnicalId<Guid>
    {
        public bool IsNew { get; }
        public Guid TechnicalValue { get; }

        public static GlobalId New() => new GlobalId(true, Guid.NewGuid());
        public static GlobalId WithValue(Guid value) => new GlobalId(false, value);

        private GlobalId(bool isNew, Guid value)
        {
            IsNew = isNew;
            TechnicalValue = value;
        }
        
        public override bool Equals(object obj) => obj is GlobalId technicalId && Equals(technicalId);
        public bool Equals(GlobalId other) => TechnicalValue.Equals(other.TechnicalValue);

        public override int GetHashCode() => TechnicalValue.GetHashCode();
    }
}