using ITLibrium.Hexagon.App.Commands;
using ITLibrium.Hexagon.App.Gates;
using ITLibrium.Hexagon.App.Queries;
using ITLibrium.Hexagon.App.Services;
using SimpleInjector;

namespace ITLibrium.Hexagon.App.SimpleInjector.Gates
{
    internal class SimpleInjectorGate : IGate
    {
        private readonly Container _container;

        public SimpleInjectorGate(Container container)
        {
            _container = container;
        }

        public IActionExecutor<TService> GetService<TService>() where TService : IAppService
        {
            return _container.GetInstance<IActionExecutor<TService>>();
        }

        public ICommandHandler<TCommand> GetHandlerFor<TCommand>() where TCommand : ICommand
        {
            return _container.GetInstance<ICommandHandler<TCommand>>();
        }

        public ICommandHandler<TCommand, TResult> GetHandlerFor<TCommand, TResult>() where TCommand : ICommand<TResult>
        {
            return _container.GetInstance<ICommandHandler<TCommand, TResult>>();
        }

        public IFinder<TResult> GetFinderFor<TResult>()
        {
            return _container.GetInstance<IFinder<TResult>>();
        }

        public IFinder<TResult, TCriteria> GetFinderFor<TResult, TCriteria>()
        {
            return _container.GetInstance<IFinder<TResult, TCriteria>>();
        }
    }
}