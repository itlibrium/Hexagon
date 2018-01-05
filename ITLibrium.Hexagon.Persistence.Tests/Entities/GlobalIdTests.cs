using System;
using ITLibrium.Hexagon.Persistence.Entities;
using Shouldly;
using Xunit;

namespace ITLibrium.Hexagon.Persistence.Tests.Entities
{
    public class GlobalIdTests
    {
        [Fact]
        public void IdsWithEqualValuesAreEqual()
        {
            GlobalId id1 = GlobalId.WithValue(new Guid("04E7C705-C5A4-4240-91D2-7D2F3F7CE661"));
            GlobalId id2 = GlobalId.WithValue(new Guid("04E7C705-C5A4-4240-91D2-7D2F3F7CE661"));
            
            id1.Equals(id2).ShouldBeTrue();
        }
        
        [Fact]
        public void IdsWithNotEqualValuesAreNotEqual()
        {
            GlobalId id1 = GlobalId.WithValue(new Guid("04E7C705-C5A4-4240-91D2-7D2F3F7CE661"));
            GlobalId id2 = GlobalId.WithValue(new Guid("E91C81C7-2C62-4BD5-B805-6C42B5A814C2"));
            
            id1.Equals(id2).ShouldBeFalse();
        }
        
        [Fact]
        public void IdHashCodeIsEqualsToValueHashCode()
        {
            var value = new Guid("04E7C705-C5A4-4240-91D2-7D2F3F7CE661");
            GlobalId id = GlobalId.WithValue(value);
            id.GetHashCode().ShouldBe(value.GetHashCode());
        }
        
        [Fact]
        public void CanGetValueOfNewId()
        {
            GlobalId id = GlobalId.New();
            Should.NotThrow(() =>
            {
                Guid value = id.TechnicalValue;
            });
        }
    }
}