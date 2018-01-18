using System;
using System.Collections.Generic;
using System.Linq;

namespace ITLibrium.Hexagon.Domain.Entities
{
    public class DomainEventBus : IAggregateEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _consumers = new Dictionary<Type, List<Delegate>>();

        public void AddConsumer<TAggregate, TEvent>(Action<TAggregate, TEvent> consumer) 
            where TAggregate : IAggregate<TAggregate> 
            where TEvent : IAggregateEvent<TAggregate>
        {
            if (!_consumers.TryGetValue(typeof(TAggregate), out List<Delegate> consumers))
                _consumers.Add(typeof(TAggregate), consumers = new List<Delegate>());
            
            consumers.Add(consumer);
        }

        public void Publish<TAggregate, TEvent>(TAggregate aggregate, TEvent @event) 
            where TAggregate : IAggregate<TAggregate> 
            where TEvent : IAggregateEvent<TAggregate>
        {
            if (_consumers.TryGetValue(typeof(TAggregate), out List<Delegate> consumers))
                foreach (Action<TAggregate, TEvent> consumer in consumers.OfType<Action<TAggregate, TEvent>>())
                    consumer(aggregate, @event);
        }
    }
}