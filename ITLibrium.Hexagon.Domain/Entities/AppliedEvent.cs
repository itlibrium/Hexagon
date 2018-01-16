using System;

namespace ITLibrium.Hexagon.Domain.Entities
{
    public struct AppliedEvent<TAggregate, TId, TEvent>
        where TAggregate : Aggregate<TAggregate, TId>
        where TEvent : Aggregate<TAggregate, TId>.IEvent
    {
        public TAggregate Aggregate { get; }
        public TEvent Event { get; }

        public AppliedEvent(TAggregate aggregate, TEvent @event)
        {
            Aggregate = aggregate;
            Event = @event;
        }

        public AppliedEvent<TAggregate, TId, TEvent> Emit(EventHandler<TEvent> handler)
        {
            handler?.Invoke(Aggregate, Event);
            return this;
        }

        public static implicit operator TAggregate(AppliedEvent<TAggregate, TId, TEvent> appliedEvent)
            => appliedEvent.Aggregate;
        
        public static implicit operator TEvent(AppliedEvent<TAggregate, TId, TEvent> appliedEvent)
            => appliedEvent.Event;
    }
}