using System;

namespace ITLibrium.Hexagon.Domain.Entities
{
    public static class AggregateExtensions
    {
        public static TAggregate BindEventsTo<TAggregate, TId>(this Aggregate<TAggregate, TId> aggregate, IAggregateEventBus bus)
            where TAggregate : Aggregate<TAggregate, TId>
        {
            throw new NotImplementedException();
        }

        internal static AppliedEvent<TAggregate, TId, TEvent> Apply<TAggregate, TId, TEvent>(this TAggregate aggregate, TEvent @event)
            where TAggregate : Aggregate<TAggregate, TId>
            where TEvent : Aggregate<TAggregate, TId>.IEvent
        {
            @event.Apply(aggregate);
            return new AppliedEvent<TAggregate, TId, TEvent>(aggregate, @event);
        }
        
        internal static TAggregate Emit<TAggregate>(this TAggregate aggregate, EventHandler handler)
            where TAggregate : IAggregate<TAggregate>
        {
            handler?.Invoke(aggregate, EventArgs.Empty);
            return aggregate;
        }
    }
}