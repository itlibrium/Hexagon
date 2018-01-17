using System;
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

        [Factory]
        public interface IRestorer
        {
            TAggregate Restore(TId id, IEnumerable<IEvent> events);
        }
        
        [Factory]
        public interface IFactory : IRestorer
        {
            TAggregate Create(TId id);
        }
        
        [Factory]
        public interface IFactory<in TInitialData> : IRestorer
            where TInitialData : IEvent
        {
            TAggregate Create(TId id, TInitialData initialData);
        }
        
        public abstract class Restorer : IRestorer
        {
            protected readonly IAggregateEventBus EventBus;

            protected Restorer(IAggregateEventBus eventBus)
            {
                EventBus = eventBus;
            }

            public virtual TAggregate Restore(TId id, IEnumerable<IEvent> events)
            {
                return GetNewInstance(id)
                    .BindEventsTo(EventBus)
                    .Apply(events);
            }
            
            protected abstract TAggregate GetNewInstance(TId id);
        }
        
        public abstract class FactoryBase : Restorer, IFactory
        {
            protected FactoryBase(IAggregateEventBus eventBus) : base(eventBus) { }

            public TAggregate Create(TId id)
            {
                return GetNewInstance(id)
                    .BindEventsTo(EventBus)
                    .Emit(Created);
            }
           
            public event EventHandler Created;
        }

        public abstract class FactoryBase<TInitialData> : Restorer, IFactory<TInitialData>
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
            Task<TAggregate> Restore(TId id);
            Task Save(TAggregate aggregate);
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