using System;
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
            var entityAId1 = new EntityA.Id("8F55AA20-3C16-4966-8A4A-B0CBCAC7396B");
            var entityAId2 = new EntityA.Id("8F55AA20-3C16-4966-8A4A-B0CBCAC7396B");
            entityAId1.Equals(entityAId2).ShouldBeTrue();
            
            var entityBId1 = new EntityB.Id(5);
            var entityBId2 = new EntityB.Id(5);
            entityBId1.Equals(entityBId2).ShouldBeTrue();
        }
        
        [Fact]
        public void IdsWithNotEqualValuesAreNotEqual()
        {
            var entityAId1 = new EntityA.Id("8F55AA20-3C16-4966-8A4A-B0CBCAC7396B");
            var entityAId2 = new EntityA.Id("9F3D3E20-56D9-421C-AC36-49DFD56651E4");
            entityAId1.Equals(entityAId2).ShouldBeFalse();
            
            var entityBId1 = new EntityB.Id(5);
            var entityBId2 = new EntityB.Id(8);
            entityBId1.Equals(entityBId2).ShouldBeFalse();
        }
        
        [Fact]
        public void NewIdsAreNotEqual()
        {
            EntityA.Id entityAId1 = EntityA.Id.New();
            EntityA.Id entityAId2 = EntityA.Id.New();
            entityAId1.Equals(entityAId2).ShouldBeFalse();
            
            EntityB.Id entityBId1 = EntityB.Id.New();
            EntityB.Id entityBId2 = EntityB.Id.New();
            entityBId1.Equals(entityBId2).ShouldBeFalse();
        }
        
        [Fact]
        public void HashCodeOfNewIdIsZero()
        {
            EntityA.Id entityAId = EntityA.Id.New();
            entityAId.GetHashCode().ShouldBe(0);
            
            EntityB.Id entityBId = EntityB.Id.New();
            entityBId.GetHashCode().ShouldBe(0);
        }
        
        [Fact]
        public void EntitiesWithEqualIdAreEqual()
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
            var entityA = new EntityA();
            entityA.Equals(null).ShouldBeFalse();
            entityA.Equals((object)null).ShouldBeFalse();
            
            var entityB = new EntityB();
            entityB.Equals(null).ShouldBeFalse();
            entityB.Equals((object)null).ShouldBeFalse();
        }
        
        [Fact]
        public void EntitiesOfDifferentTypesAreNotEqual()
        {
            var entityA = new EntityA();
            var entityB = new EntityB();
            entityA.Equals(entityB).ShouldBeFalse();
        }
        
        [Fact]
        public void EntitiesWithNotEqualIdAreNotEqual()
        {
            var entityA1 = new EntityA("77813DDE-5443-45C2-8231-0E77B1B0163D");
            var entityA2 = new EntityA("6B58B159-0DEC-48A1-8D06-2AA8834DF746");
            entityA1.Equals(entityA2).ShouldBeFalse();
            
            var entityB1 = new EntityB(5);
            var entityB2 = new EntityB(8);
            entityB1.Equals(entityB2).ShouldBeFalse();
        }
        
        [Fact]
        public void NewEntitiesAreNotEqual()
        {
            var entityA1 = new EntityA();
            var entityA2 = new EntityA();
            entityA1.Equals(entityA2).ShouldBeFalse();
            
            var entityB1 = new EntityB();
            var entityB2 = new EntityB();
            entityB1.Equals(entityB2).ShouldBeFalse();
        }
        
        [Fact]
        public void EntityWithNewIdIsNew()
        {
            EntityA.Id entityAId = EntityA.Id.New();
            var entityA = new EntityA(entityAId);
            entityA.IsNew.ShouldBeTrue();
            
            EntityB.Id entityBId = EntityB.Id.New();
            var entityB = new EntityB(entityBId);
            entityB.IsNew.ShouldBeTrue();
        }
        
        [Fact]
        public void HashCodeOfNewEntityIsZero()
        {
            var entityA1 = new EntityA();
            entityA1.GetHashCode().ShouldBe(0);
            
            var entityB1 = new EntityB();
            entityB1.GetHashCode().ShouldBe(0);
        }
        
        [Fact]
        public void CanGetValueOfNewGuidId()
        {
            EntityA.Id entityAId = EntityA.Id.New();
            Should.NotThrow(() =>
            {
                Guid id = entityAId.Value;
            });
        }

        [Fact]
        public void CantGetValueOfNewNonGuidId()
        {
            EntityB.Id entityBId = EntityB.Id.New();
            Should.Throw<InvalidOperationException>(() =>
            {
                int id = entityBId.Value;
            });
        }

        private class EntityA : Entity<EntityA>
        {
            public EntityA() { }

            public EntityA(string id) : base(id) { }

            public EntityA(Guid id) : base(id) { }

            public EntityA(Id id) : base(id) { }
        }

        private class EntityB : Entity<EntityB, int>
        {
            public EntityB() { }

            public EntityB(int id) : base(id) { }

            public EntityB(Id id) : base(id) { }
        }
    }
}