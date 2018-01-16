using System;
using System.Collections.Generic;
using ITLibrium.Hexagon.Domain.Entities;
using Shouldly;
using Xunit;

namespace ITLibrium.Hexagon.Domain.Tests.Entities
{
    public class AggregateTests
    {
        [Fact]
        public void EvantsAreAppliedOnRestoring()
        {
            var a = new AggregateA("ABC123", new[] {new AggregateA.DoneEvent(2), new AggregateA.DoneEvent(3)});
            a.Value.ShouldBe(5);
        }
        
        [Fact]
        public void EventIsAppliedWhenApplyIsUsed()
        {
            var a = new AggregateA("ABC123");
            a.Do(3);
            a.Value.ShouldBe(3);
        }
        
        [Fact]
        public void EventIsAppliedWhenApplyEndEmitIsUsed()
        {
            var a = new AggregateA("ABC123");
            a.DoEndEmit(3);
            a.Value.ShouldBe(3);
        }

        [Fact]
        public void EventIsEmittedWhenApplyEndEmitIsUsed()
        {
            AggregateA.DoneEvent emmitedEvent = null;
            var a = new AggregateA("ABC123");
            a.Done += (s, args) => emmitedEvent = args;
            
            a.DoEndEmit(3);
            
            emmitedEvent.ShouldNotBeNull();
        }
        
        private class AggregateA : Aggregate<AggregateA, string>
        {
            public AggregateA(DomainId id) : base(id) { }

            public AggregateA(DomainId id, IEnumerable<IEvent> events) : base(id, events) { }
            
            public int Value { get; private set; }

            public void Do(int value)
            {
                Apply(new DoneEvent(value));
            }
            
            public void DoEndEmit(int value)
            {
                Apply(new DoneEvent(value)).Emit(Done);
            }

            public event EventHandler<DoneEvent> Done; 
            
            public class DoneEvent : IEvent
            {
                private readonly int _value;
                
                public DoneEvent(int value) => _value = value;

                public void Apply(AggregateA aggregate) => aggregate.Value += _value;
            }
        }
    }
}