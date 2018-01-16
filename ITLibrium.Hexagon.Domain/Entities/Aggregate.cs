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
            Apply(events);
        }
        
        protected AppliedEvent<TAggregate, TId, TEvent> Apply<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            var aggregate = (TAggregate) this;
            @event.Apply(aggregate);
            return new AppliedEvent<TAggregate, TId, TEvent>(aggregate, @event);
        }

        protected TAggregate Apply(IEnumerable<IEvent> events)
        {
            var aggregate = (TAggregate) this;
            foreach (IEvent @event in events)
                @event.Apply(aggregate);

            return aggregate;
        }

        public interface IEvent
        {
            void Apply(TAggregate aggregate);
        }

        public interface IFactory
        {
            TAggregate Restore(TId id, IEnumerable<IEvent> events);
        }
        
        public interface IFactory<in TInitialData> : IFactory
            where TInitialData : IEvent
        {
            TAggregate Create(TId id, TInitialData initialData);
        }

        public abstract class Factory<TInitialData> : IFactory<TInitialData>
            where TInitialData : IEvent
        {
            private readonly IAggregateEventBus _eventBus;

            protected Factory(IAggregateEventBus eventBus)
            {
                _eventBus = eventBus;
            }

            public virtual TAggregate Create(TId id, TInitialData initialData)
            {
                return Create(id)
                    .BindEventsTo(_eventBus)
                    .Apply(initialData)
                    .Emit(Created);
            }
            
            public event EventHandler<TInitialData> Created;

            public virtual TAggregate Restore(TId id, IEnumerable<IEvent> events)
            {
                return Create(id)
                    .BindEventsTo(_eventBus)
                    .Apply(events);
            }

            protected abstract TAggregate Create(TId id);
        }
        
        public interface IRepository
        {
            TAggregate Restore(TId id);
            void Save(TAggregate aggregate);
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