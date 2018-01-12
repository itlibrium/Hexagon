using System;
using System.Collections.Generic;

namespace ITLibrium.Hexagon.Domain.Entities
{
    public abstract class Aggregate<TAggregate, TId> : Entity<TAggregate, TId>
        where TAggregate : Aggregate<TAggregate, TId>
    {
        protected Aggregate(DomainId id) : base(id) { }

        protected Aggregate(DomainId id, IEnumerable<IEvent> events)
            : base(id)
        {
            var aggregate = (TAggregate) this;
            foreach (IEvent @event in events)
                @event.Apply(aggregate);
        }
        
        protected TEvent Apply<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            @event.Apply((TAggregate) this);
            return @event;
        }

        protected TEvent ApplyAndEmit<TEvent>(TEvent @event, EventHandler<TEvent> handler)
            where TEvent : IEvent
        {
            @event.Apply((TAggregate) this);
            handler?.Invoke(this, @event);
            return @event;
        }

        public interface IEvent
        {
            void Apply(TAggregate aggregate);
        }
    }
    
    public abstract class Aggregate<TAggregate, TId, TSnapshot> : Aggregate<TAggregate, TId>
        where TAggregate : Aggregate<TAggregate, TId, TSnapshot>
        where TSnapshot : Aggregate<TAggregate, TId, TSnapshot>.ISnapshot
    {
        public static TAggregate Restore(TSnapshot snapshot) => snapshot.ToAggregate();
        
        protected Aggregate(DomainId id) : base(id) { }

        protected Aggregate(DomainId id, IEnumerable<IEvent> events) : base(id, events) { }

        public abstract TSnapshot GetSnapshot();

        public interface ISnapshot
        {
            TId Id { get; }

            TAggregate ToAggregate();
        }
    }
}