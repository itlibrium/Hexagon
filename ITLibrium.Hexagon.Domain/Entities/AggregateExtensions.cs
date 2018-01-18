using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ITLibrium.Hexagon.Domain.Entities
{
    public static class AggregateExtensions
    {
        private static readonly ConcurrentDictionary<Type, Delegate> _binders = new ConcurrentDictionary<Type, Delegate>(); 
        
        public static TAggregate BindEventsTo<TAggregate>(this TAggregate aggregate, IAggregateEventBus bus)
            where TAggregate : IAggregate<TAggregate>
        {
            var binder = (Action<TAggregate, IAggregateEventBus>) _binders.GetOrAdd(typeof(TAggregate), t => CreateBinder<TAggregate>());
            binder(aggregate, bus);
            return aggregate;
        }

        private static Action<TAggregate, IAggregateEventBus> CreateBinder<TAggregate>()
            where TAggregate : IAggregate<TAggregate>
        {
            ParameterExpression aggregateExp = Expression.Parameter(typeof(TAggregate), "aggregate");
            ParameterExpression busExp = Expression.Parameter(typeof(IAggregateEventBus), "bus");
            
            Type aggregateType = typeof(TAggregate);
            Expression bodyExp = Expression.Block(
                aggregateType.GetEvents()
                    .Where(IsAggregateEvent<TAggregate>)
                    .Select(eventInfo => CreateAssignExp(aggregateType, eventInfo, aggregateExp, busExp)));

            return Expression.Lambda<Action<TAggregate, IAggregateEventBus>>(bodyExp, aggregateExp, busExp)
                .Compile();
        }

        private static bool IsAggregateEvent<TAggregate>(EventInfo eventInfo)
            where TAggregate : IAggregate<TAggregate>
        {
            return eventInfo.EventHandlerType.IsGenericType &&
                   typeof(IAggregateEvent<TAggregate>).IsAssignableFrom(eventInfo.GetEventArgsType());
        }

        private static Type GetEventArgsType(this EventInfo eventInfo) => eventInfo.EventHandlerType.GenericTypeArguments[0];

        private static MethodCallExpression CreateAssignExp(Type aggregateType, EventInfo eventInfo, Expression aggregateExp, Expression busExp)
        {
            LambdaExpression handlerExp = CreateHandlerExp(aggregateType, eventInfo.GetEventArgsType(), busExp);
            return Expression.Call(aggregateExp, eventInfo.GetAddMethod(), handlerExp);
        }
        
        private static LambdaExpression CreateHandlerExp(Type aggregateType, Type eventArgsType, Expression busExp)
        {
            ParameterExpression senderExp = Expression.Parameter(typeof(object), "sender");
            ParameterExpression eventArgsExp = Expression.Parameter(eventArgsType, "eventArgs");

            return Expression.Lambda(
                typeof(EventHandler<>).MakeGenericType(eventArgsType),
                Expression.Call(
                    busExp,
                    typeof(IAggregateEventBus)
                        .GetMethod(nameof(IAggregateEventBus.Publish))
                        .MakeGenericMethod(aggregateType, eventArgsType),
                    Expression.Convert(senderExp, aggregateType),
                    eventArgsExp),
                senderExp, eventArgsExp);
        }

        internal static TAggregate Emit<TAggregate>(this TAggregate aggregate, EventHandler handler)
            where TAggregate : IAggregate<TAggregate>
        {
            handler?.Invoke(aggregate, EventArgs.Empty);
            return aggregate;
        }
    }
}