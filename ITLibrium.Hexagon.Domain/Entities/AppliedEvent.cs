using System;

namespace ITLibrium.Hexagon.Domain.Entities
{
    public struct AppliedEvent<TAggregate, TEvent>
        where TAggregate : IAggregate<TAggregate>
        where TEvent : IAggregateEvent<TAggregate>
    {
        public TAggregate Aggregate { get; }
        public TEvent Event { get; }

        public AppliedEvent(TAggregate aggregate, TEvent @event)
        {
            Aggregate = aggregate;
            Event = @event;
        }

        public AppliedEvent<TAggregate, TEvent> Emit(EventHandler<TEvent> handler)
        {
            handler?.Invoke(Aggregate, Event);
            return this;
        }

        public static implicit operator TAggregate(AppliedEvent<TAggregate, TEvent> appliedEvent)
            => appliedEvent.Aggregate;
        
        public static implicit operator TEvent(AppliedEvent<TAggregate, TEvent> appliedEvent)
            => appliedEvent.Event;
    }
}