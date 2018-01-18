using System;

namespace ITLibrium.Hexagon.Domain.Entities
{
    public interface IAggregateEventBus
    {
        void AddConsumer<TAggregate, TEvent>(Action<TAggregate, TEvent> consumer)
            where TAggregate : IAggregate<TAggregate>
            where TEvent : IAggregateEvent<TAggregate>;

        void Publish<TAggregate, TEvent>(TAggregate aggregate, TEvent @event)
            where TAggregate : IAggregate<TAggregate>
            where TEvent : IAggregateEvent<TAggregate>;
    }
}