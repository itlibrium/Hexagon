using ITLibrium.Hexagon.Domain.Entities;
using Shouldly;
using Xunit;

namespace ITLibrium.Hexagon.Domain.Tests.Entities
{
    public class EntityTests
    {
        [Fact]
        public void IdsWithEqualValuesAreEqual()
        {
            var entityAId1 = new EntityA.DomainId("8F55AA20-3C16-4966-8A4A-B0CBCAC7396B");
            var entityAId2 = new EntityA.DomainId("8F55AA20-3C16-4966-8A4A-B0CBCAC7396B");
            entityAId1.Equals(entityAId2).ShouldBeTrue();
            
            var entityBId1 = new EntityB.DomainId(5);
            var entityBId2 = new EntityB.DomainId(5);
            entityBId1.Equals(entityBId2).ShouldBeTrue();
        }
        
        [Fact]
        public void IdsWithNotEqualValuesAreNotEqual()
        {
            var entityAId1 = new EntityA.DomainId("8F55AA20-3C16-4966-8A4A-B0CBCAC7396B");
            var entityAId2 = new EntityA.DomainId("9F3D3E20-56D9-421C-AC36-49DFD56651E4");
            entityAId1.Equals(entityAId2).ShouldBeFalse();
            
            var entityBId1 = new EntityB.DomainId(5);
            var entityBId2 = new EntityB.DomainId(8);
            entityBId1.Equals(entityBId2).ShouldBeFalse();
        }
             
        [Fact]
        public void EntitiesWithEqualIdsAreEqual()
        {
            var entityA1 = new EntityA("77813DDE-5443-45C2-8231-0E77B1B0163D");
            var entityA2 = new EntityA("77813DDE-5443-45C2-8231-0E77B1B0163D");
            entityA1.Equals(entityA2).ShouldBeTrue();
            
            var entityB1 = new EntityB(5);
            var entityB2 = new EntityB(5);
            entityB1.Equals(entityB2).ShouldBeTrue();
        }

        [Fact]
        public void EntityIsNotEqualToNull()
        {
            var entityA = new EntityA("77813DDE-5443-45C2-8231-0E77B1B0163D");
            entityA.Equals(null).ShouldBeFalse();
            entityA.Equals((object)null).ShouldBeFalse();
            
            var entityB = new EntityB(123456);
            entityB.Equals(null).ShouldBeFalse();
            entityB.Equals((object)null).ShouldBeFalse();
        }
        
        [Fact]
        public void EntitiesOfDifferentTypesAreNotEqual()
        {
            var entityA = new EntityA("77813DDE-5443-45C2-8231-0E77B1B0163D");
            var entityB = new EntityB(123456);
            entityA.Equals(entityB).ShouldBeFalse();
        }
        
        [Fact]
        public void EntitiesWithNotEqualIdsAreNotEqual()
        {
            var entityA1 = new EntityA("77813DDE-5443-45C2-8231-0E77B1B0163D");
            var entityA2 = new EntityA("6B58B159-0DEC-48A1-8D06-2AA8834DF746");
            entityA1.Equals(entityA2).ShouldBeFalse();
            
            var entityB1 = new EntityB(5);
            var entityB2 = new EntityB(8);
            entityB1.Equals(entityB2).ShouldBeFalse();
        }

        [Fact]
        public void EntityHashCodeIsEqualToIdHashCode()
        {
            var id = new EntityA.DomainId("77813DDE-5443-45C2-8231-0E77B1B0163D");
            var entity = new EntityA(id);
            
            entity.GetHashCode().ShouldBe(id.GetHashCode());
        }
        
        private class EntityA : Entity<EntityA, string>
        {
            public EntityA(string id) : base(new DomainId(id)) { }

            public EntityA(DomainId id) : base(id) { }
        }

        private class EntityB : Entity<EntityB, int>
        {
            public EntityB(int id) : base(new DomainId(id)) { }

            public EntityB(DomainId id) : base(id) { }
        }
    }
}