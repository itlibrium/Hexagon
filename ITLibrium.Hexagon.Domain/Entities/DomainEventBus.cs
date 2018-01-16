using System;
using System.Collections.Generic;
using System.Linq;

namespace ITLibrium.Hexagon.Domain.Entities
{
    public class DomainEventBus : IAggregateEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _consumers = new Dictionary<Type, List<Delegate>>();

        public void AddConsumer<TAggregate, TId>(Action<TAggregate, Aggregate<TAggregate, TId>.IEvent> consumer) 
            where TAggregate : Aggregate<TAggregate, TId>
        {
            if(!_consumers.TryGetValue(typeof(TAggregate), out List<Delegate> consumers))
                _consumers.Add(typeof(TAggregate), consumers = new List<Delegate>());
            
            consumers.Add(consumer);
        }

        public void Publish<TAggregate, TId, TEvent>(TAggregate aggregate, TEvent @event) 
            where TAggregate : Aggregate<TAggregate, TId> 
            where TEvent : Aggregate<TAggregate, TId>.IEvent
        {
            if (_consumers.TryGetValue(typeof(TAggregate), out List<Delegate> consumers))
                foreach (Action<TAggregate, TEvent> consumer in consumers.OfType<Action<TAggregate, TEvent>>())
                    consumer(aggregate, @event);
        }
    }
}