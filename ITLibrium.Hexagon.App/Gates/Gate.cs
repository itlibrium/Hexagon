using System;
using System.Collections.Generic;
using System.Linq;
using ITLibrium.Hexagon.App.Commands;
using ITLibrium.Hexagon.App.Queries;
using ITLibrium.Hexagon.App.Services;
using ITLibrium.Reflection;

namespace ITLibrium.Hexagon.App.Gates
{
    public class Gate : IGate
    {
        private readonly IDictionary<Type, IActionExecutor> _executors;

        private readonly IDictionary<Type, ICommandHandler> _handlers;

        private readonly IDictionary<FinderId, IFinder> _finders;

        public Gate(IEnumerable<IActionExecutor> executors, IEnumerable<ICommandHandler> handlers, IEnumerable<IFinder> finders, IReadOnlyCollection<IGatePolicy> policies)
        {
            _executors = PrepareExecutors(executors, policies);
            _handlers = PrepareHandlers(handlers, policies);
            _finders = PrepareFinders(finders, policies);
        }

        private static IDictionary<Type, IActionExecutor> PrepareExecutors(IEnumerable<IActionExecutor> executors, IReadOnlyCollection<IGatePolicy> policies)
        {
            var typeToExecutor = new Dictionary<Type, IActionExecutor>();
            foreach (IActionExecutor executor in executors)
            {
                foreach (IGatePolicy policy in policies)
                    policy.Check(executor);

                typeToExecutor.Add(GetServiceType(executor), executor);
            }
            return typeToExecutor;
        }

        private static IDictionary<Type, ICommandHandler> PrepareHandlers(IEnumerable<ICommandHandler> handlers, IReadOnlyCollection<IGatePolicy> policies)
        {
            var typeToHandler = new Dictionary<Type, ICommandHandler>();
            foreach (ICommandHandler handler in handlers)
            {
                foreach (IGatePolicy policy in policies)
                    policy.Check(handler);

                foreach (Type commandType in GetCommandTypes(handler))
                    typeToHandler.Add(commandType, handler);
            }
            return typeToHandler;
        }

        private static IDictionary<FinderId, IFinder> PrepareFinders(IEnumerable<IFinder> finders, IReadOnlyCollection<IGatePolicy> policies)
        {
            var idToFinder = new Dictionary<FinderId, IFinder>();
            foreach (IFinder finder in finders)
            {
                foreach (IGatePolicy policy in policies)
                    policy.Check(finder);

                foreach (FinderId finderId in GetFinderIds(finder))
                    idToFinder.Add(finderId, finder);
            }
            return idToFinder;
        }

        private static Type GetServiceType(IActionExecutor executor)
        {
            return executor.GetType().GetClosedInterfaces(typeof(IActionExecutor<>))
                .Single().GenericTypeArguments[0];
        }

        private static IEnumerable<Type> GetCommandTypes(ICommandHandler handler)
        {
            return handler.GetType().GetClosedInterfaces(typeof(ICommandHandler<>), typeof(ICommandHandler<,>))
                .Select(i => i.GenericTypeArguments[0]);
        }

        private static IEnumerable<FinderId> GetFinderIds(IFinder finder)
        {
            foreach (Type type in finder.GetType().GetClosedInterfaces(typeof(IFinder<>)))
                yield return new FinderId(type.GenericTypeArguments[0]);

            foreach (Type type in finder.GetType().GetClosedInterfaces(typeof(IFinder<,>)))
                yield return new FinderId(type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
        }

        public IActionExecutor<TService> GetService<TService>() 
            where TService : IAppService
        {
            if (!_executors.TryGetValue(typeof(TService), out IActionExecutor executor))
                throw new ActionExecutorNotFoundException($"ActionExecutor for service: {typeof(TService).Name} not found");

            return (IActionExecutor<TService>)executor;
        }

        public ICommandHandler<TCommand> GetHandlerFor<TCommand>() 
            where TCommand : ICommand
        {
            return (ICommandHandler<TCommand>)GetHandlerFor(typeof(TCommand));
        }

        public ICommandHandler<TCommand, TResult> GetHandlerFor<TCommand, TResult>() 
            where TCommand : ICommand<TResult>
        {
            return (ICommandHandler<TCommand, TResult>)GetHandlerFor(typeof(TCommand));
        }

        private ICommandHandler GetHandlerFor(Type commandType)
        {
            if (!_handlers.TryGetValue(commandType, out ICommandHandler handler))
                throw new CommandHandlerNotFoundException($"CommandHandler for command: {commandType.Name} not found");

            return handler;
        }

        public IFinder<TResult> GetFinderFor<TResult>()
        {
            return (IFinder<TResult>)GetFinderFor(FinderId.Create<TResult>());
        }

        public IFinder<TResult, TCriteria> GetFinderFor<TResult, TCriteria>()
        {
            return (IFinder<TResult, TCriteria>)GetFinderFor(FinderId.Create<TResult, TCriteria>());
        }

        private IFinder GetFinderFor(FinderId finderId)
        {
            if (!_finders.TryGetValue(finderId, out IFinder finder))
                throw new FinderNotFoundException($"Finder for: {finderId} not found");

            return finder;
        }
    }
}