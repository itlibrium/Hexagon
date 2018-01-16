using System;

namespace ITLibrium.Hexagon.Domain.Entities
{
    public interface IAggregateEventBus
    {
        void AddConsumer<TAggregate, TId>(Action<TAggregate, Aggregate<TAggregate, TId>.IEvent> consumer)
            where TAggregate : Aggregate<TAggregate, TId>;
        
        void Publish<TAggregate, TId, TEvent>(TAggregate aggregate, TEvent @event)
            where TAggregate : Aggregate<TAggregate, TId>
            where TEvent : Aggregate<TAggregate, TId>.IEvent;
    }
}