using System;
using ITLibrium.Hexagon.Persistence.Entities;
using Shouldly;
using Xunit;

namespace ITLibrium.Hexagon.Persistence.Tests.Entities
{
    public class TechnicalIdTests
    {
        [Fact]
        public void NewIdsAreNotEqual()
        {
            TechnicalId<int> id1 = TechnicalId<int>.New();
            TechnicalId<int> id2 = TechnicalId<int>.New();
            
            id1.Equals(id2).ShouldBeFalse();
        }
        
        [Fact]
        public void IdsWithEqualValuesAreEqual()
        {
            TechnicalId<int> id1 = TechnicalId<int>.WithValue(123456);
            TechnicalId<int> id2 = TechnicalId<int>.WithValue(123456);
            
            id1.Equals(id2).ShouldBeTrue();
        }
        
        [Fact]
        public void IdsWithNotEqualValuesAreNotEqual()
        {
            TechnicalId<int> id1 = TechnicalId<int>.WithValue(123456);
            TechnicalId<int> id2 = TechnicalId<int>.WithValue(987654);
            
            id1.Equals(id2).ShouldBeFalse();
        }
        
        [Fact]
        public void HashCodeOfNewIdIsZero()
        {
            TechnicalId<int> id = TechnicalId<int>.New();
            id.GetHashCode().ShouldBe(0);
        }
        
        [Fact]
        public void IdHashCodeIsEqualsToValueHashCode()
        {
            int value = 123456;
            TechnicalId<int> id = TechnicalId<int>.WithValue(value);
            id.GetHashCode().ShouldBe(value.GetHashCode());
        }
        
        [Fact]
        public void CanNotGetValueOfNewId()
        {
            TechnicalId<int> id = TechnicalId<int>.New();
            Should.Throw<InvalidOperationException>(() =>
            {
                int value = id.TechnicalValue;
            });
        }
    }
}