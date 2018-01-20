using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITLibrium.Hexagon.Domain.Meta;

namespace ITLibrium.Hexagon.Domain.Entities
{
    [Aggregate]
    public abstract class Aggregate<TAggregate, TId> : Entity<TAggregate, TId>, IAggregate<TAggregate>
        where TAggregate : Aggregate<TAggregate, TId>
    {
        protected Aggregate(DomainId id) : base(id) { }

        protected Aggregate(DomainId id, IEnumerable<IEvent> events)
            : base(id)
        {
            Apply(events);
        }

        protected AppliedEvent<TAggregate, TEvent> Apply<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            var aggregate = (TAggregate) this;
            @event.Apply(aggregate);
            return new AppliedEvent<TAggregate, TEvent>(aggregate, @event);
        }

        protected TAggregate Apply(IEnumerable<IEvent> events)
        {
            var aggregate = (TAggregate) this;
            foreach (IEvent @event in events)
                @event.Apply(aggregate);

            return aggregate;
        }

        public interface IEvent : IAggregateEvent<TAggregate>
        {
            void Apply(TAggregate aggregate);
        }

        [Factory]
        public abstract class Reconstructor
        {
            protected readonly IAggregateEventBus EventBus;

            protected Reconstructor(IAggregateEventBus eventBus)
            {
                EventBus = eventBus;
            }

            public virtual TAggregate Reconstruct(TId id, IEnumerable<IEvent> events)
            {
                return GetNewInstance(id)
                    .BindEventsTo(EventBus)
                    .Apply(events);
            }

            protected abstract TAggregate GetNewInstance(TId id);
        }

        public abstract class FactoryBase : Reconstructor
        {
            protected FactoryBase(IAggregateEventBus eventBus) : base(eventBus) { }

            public virtual TAggregate Create(TId id)
            {
                return GetNewInstance(id)
                    .BindEventsTo(EventBus)
                    .Emit(Created);
            }

            public event EventHandler Created;
        }

        public abstract class FactoryBase<TInitialData> : Reconstructor
            where TInitialData : IEvent
        {
            protected FactoryBase(IAggregateEventBus eventBus) : base(eventBus) { }

            public virtual TAggregate Create(TId id, TInitialData initialData)
            {
                return GetNewInstance(id)
                    .BindEventsTo(EventBus)
                    .Apply(initialData)
                    .Emit(Created);
            }

            public event EventHandler<TInitialData> Created;
        }
        
        [Repository]
        public interface IRepository
        {
            Task<TAggregate> Get(TId id);
            Task<TAggregate> Get(DomainId id);
            Task Save(TAggregate aggregate);
        }

        public class EventSourcedRepository : IRepository
        {
            private readonly Reconstructor _reconstructor;

            private readonly IEventStore _eventStore;
            
            private readonly Dictionary<DomainId, AggregateEvents> _newEvents = new Dictionary<DomainId, AggregateEvents>();

            protected EventSourcedRepository(Reconstructor reconstructor, IEventStore eventStore, IAggregateEventBus eventBus)
            {
                _reconstructor = reconstructor;
                _eventStore = eventStore;
                
                eventBus.AddConsumer<TAggregate, IEvent>(AddEvent);
            }
            
            private void AddEvent(TAggregate aggregate, IEvent @event)
            {
                if (!_newEvents.TryGetValue(aggregate.Id, out AggregateEvents events))
                    _newEvents.Add(aggregate.Id, events = new AggregateEvents(aggregate));
                else if(events.AreForDifferentAggregateReferenceThan(aggregate))
                    throw new InvalidOperationException("More than one aggregate with the same Id within one request");
                
                events.Add(@event);
            }
            
            ~EventSourcedRepository()
            {
                if(_newEvents.Count > 0)
                    throw new InvalidOperationException("All new events should be saved before the end of the request");
            }

            public Task<TAggregate> Get(TId aggregateId) => Get(new DomainId(aggregateId));

            public async Task<TAggregate> Get(DomainId id)
            {
                IReadOnlyCollection<IEvent> events = await _eventStore.GetEvents(id);
                return _reconstructor.Reconstruct(id, events);
            }

            public async Task Save(TAggregate aggregate)
            {
                DomainId aggregateId = aggregate.Id;
                if (!_newEvents.TryGetValue(aggregateId, out AggregateEvents events))
                    return;
                
                _newEvents.Remove(aggregateId);
                await _eventStore.SaveEvents(aggregateId, events);
            }
            
            private struct AggregateEvents : IEnumerable<IEvent>
            {
                private readonly List<IEvent> _events;
            
                private readonly TAggregate _aggregate;

                public AggregateEvents(TAggregate aggregate) : this()
                {
                    _events = new List<IEvent>();
                    _aggregate = aggregate;
                }

                public bool AreForDifferentAggregateReferenceThan(TAggregate other) => !ReferenceEquals(_aggregate, other);

                public void Add(IEvent @event) => _events.Add(@event);
            
                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

                public IEnumerator<IEvent> GetEnumerator() => _events.GetEnumerator();
            }
        }

        public interface IEventStore
        {
            Task<IReadOnlyList<IEvent>> GetEvents(DomainId id);
            Task SaveEvents(DomainId id, IEnumerable<IEvent> events);
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